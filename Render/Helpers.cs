using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace MadScience.Render
{
    public class Helpers
    {

        private static List<vertexFormat> vertexFormats = new List<vertexFormat>();

        public static modelInfo geomToModel(Stream input)
        {
            modelInfo model = new modelInfo();

            BinaryReader reader = new BinaryReader(input);
            uint rcolVersion = reader.ReadUInt32();
            uint rcolDatatype = reader.ReadUInt32();

            uint rcolIndex3 = reader.ReadUInt32();
            uint rcolIndex1 = reader.ReadUInt32();
            uint rcolIndex2 = reader.ReadUInt32();

            for (int i = 0; i < rcolIndex2; i++)
            {
                ulong instanceId = reader.ReadUInt64();
                uint typeId = reader.ReadUInt32();
                uint groupId = reader.ReadUInt32();
            }

            for (int i = 0; i < rcolIndex2; i++)
            {
                uint chunkOffset = reader.ReadUInt32();
                uint chunkSize = reader.ReadUInt32();
            }

            // Real GEOM chunk
            string geomString = Encoding.ASCII.GetString(reader.ReadBytes(4));

            uint geomVersion = reader.ReadUInt32();

            uint tailOffset = reader.ReadUInt32();
            uint tailSize = reader.ReadUInt32();

            uint embeddedId = reader.ReadUInt32();
            if (embeddedId != 0)
            {
                uint embeddedSize = reader.ReadUInt32();
                reader.ReadBytes((int)embeddedSize);
            }

            uint unk1 = reader.ReadUInt32();
            uint unk2 = reader.ReadUInt32();

            uint numVerts = reader.ReadUInt32();
            model.numVertices = numVerts;

            uint vfCount = reader.ReadUInt32();
            
            vertexFormats.Clear();
            for (int i = 0; i < vfCount; i++)
            {
                vertexFormat vf = new vertexFormat();
                vf.dataType = reader.ReadUInt32();
                vf.subType = reader.ReadUInt32();
                vf.bytesPerElement = reader.ReadByte();
                //textBox1.Text += "Vertex Format #" + i.ToString() + ": " + vf.dataType.ToString() + ":" + vf.subType.ToString() + ":" + vf.bytesPerElement.ToString() + Environment.NewLine;
                vertexFormats.Add(vf);
            }

            // Used for bounding box
            float minX = 0;
            float minY = 0;
            float minZ = 0;
            float maxX = 0;
            float maxY = 0;
            float maxZ = 0;

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < numVerts; i++)
            {
                sb.AppendLine("Vertex #" + i.ToString());

                for (int j = 0; j < vertexFormats.Count; j++)
                {
                    float x = 0;
                    float y = 0;
                    float z = 0;

                    vertexFormat vf = (vertexFormat)vertexFormats[j];
                    switch (vf.dataType)
                    {
                        case 1:
                            x = reader.ReadSingle();
                            y = reader.ReadSingle();
                            z = reader.ReadSingle();

                            if (x < minX) minX = x;
                            if (x > maxX) maxX = x;
                            if (y < minY) minY = y;
                            if (y > maxY) maxY = y;
                            if (z < minZ) minZ = z;
                            if (z > maxZ) maxZ = z;

                            sb.AppendLine("  XYZ: " + x.ToString() + " " + y.ToString() + " " + z.ToString());
                            model.vertexList.Add(new Vector3(x, y, z));
                            break;
                        case 2:
                            x = reader.ReadSingle();
                            y = reader.ReadSingle();
                            z = reader.ReadSingle();
                            sb.AppendLine("  Normal: " + x.ToString() + " " + y.ToString() + " " + z.ToString());
                            model.vertexNormal.Add(new Vector3(x, y, z));

                            break;
                        case 3:
                            float u = reader.ReadSingle();
                            float v = reader.ReadSingle();
                            sb.AppendLine("  UV: " + u.ToString() + " " + v.ToString());
                            model.textureVertex.Add(new uvcoord(u, v));
                            //model.vertexList[i] = new vertex(model.vertexList[i].x, model.vertexList[i].y, model.vertexList[i].z, u, v);
                            break;
                        case 4:
                            sb.AppendLine("  Bone: " + reader.ReadUInt32().ToString());
                            break;
                        case 5:
                            float w1 = reader.ReadSingle();
                            float w2 = reader.ReadSingle();
                            float w3 = reader.ReadSingle();
                            float w4 = reader.ReadSingle();
                            sb.AppendLine("  Weights: " + w1.ToString() + " " + w2.ToString() + " " + w3.ToString() + " " + w4.ToString());
                            break;
                        case 6:
                            x = reader.ReadSingle();
                            z = reader.ReadSingle();
                            y = reader.ReadSingle();
                            sb.AppendLine("  Tangent Normal: " + x.ToString() + " " + y.ToString() + " " + z.ToString());
                            break;
                        case 7:
                            sb.AppendLine("  TagVal: " + reader.ReadUInt32().ToString());
                            break;
                        case 10:
                            sb.AppendLine("  VertexID: " + reader.ReadUInt32().ToString());
                            break;

                    }

                }
            }

            for (int i = 0; i < model.numVertices; i++)
            {
                //model.vertexData.Add(new vertex(model.vertexList[i].X, model.vertexList[i].X, model.vertexList[i].X, model.textureVertex[i].u, model.textureVertex[i].v, model.vertexNormal[i].X, model.vertexNormal[i].Y, model.vertexNormal[i].Z));
                model.vertexData.Add(new vertex(-model.vertexList[i].X, model.vertexList[i].Y, model.vertexList[i].Z, model.textureVertex[i].u, model.textureVertex[i].v));
            }

            // Go through the vertex lists again, this time normalising the values
            float midX = maxX - minX;
            float midY = maxY - minY;
            float midZ = maxZ - minZ;

            model.bounds.min = new Vector3(minX, minY, minZ);
            model.bounds.mid = new Vector3(midX, midY, midZ);
            model.bounds.max = new Vector3(maxX, maxY, maxZ);

            //model.midPoint = new Vector3(midX, midY, midZ);

            uint itemCount = reader.ReadUInt32();
            byte bytesPerFacePoint = reader.ReadByte();

            uint numFacePoints = reader.ReadUInt32();
            model.numPolygons = numFacePoints / 3;

            for (int i = 0; i < model.numPolygons; i++)
            {
                switch (bytesPerFacePoint)
                {
                    case 2:
                        uint v1 = reader.ReadUInt16();
                        uint v2 = reader.ReadUInt16();
                        uint v3 = reader.ReadUInt16();
                        //model.faces.Add(new polygon(v1, v2, v3));
                        model.faceData.Add(v1);
                        model.faceData.Add(v2);
                        model.faceData.Add(v3);
                        break;
                }
            }
            //textBox1.Text = sb.ToString();

            return model;
        }

    }

    class vertexFormat
    {
        public uint dataType;
        public uint subType;
        public byte bytesPerElement;
    }

    public class CameraNew
    {

        Microsoft.DirectX.Direct3D.Device _device;
        Matrix _viewMatrix;
        Matrix _projectionMatrix;
        Vector3 _position;
        Vector3 _direction;
        Vector3 _upDirection;

        float _fieldOfView;
        float _aspectRatio;
        float _nearPlane;
        float _farPlane;

        public float InputTranslationScale
        {
            get
            {
                return 450f; //(float)((NearPlane + FarPlane) / 2.0 * 300);
            }
        }

        public float InputRotationScale
        {
            get
            {
                return 100f; //(float)((NearPlane + FarPlane) / 2.0 * 100);
            }
        }

        public float InputScaleScale
        {
            get
            {
                return 100f; //(float)((NearPlane + FarPlane) / 2.0 * 100);
            }
        }

        public CameraNew(Microsoft.DirectX.Direct3D.Device device)
        {

            if (device == null)
            {
                throw new Exception("Device must not be null");
            }

            _device = device;
            Initialize();
        }

        void Initialize()
        {

            _viewMatrix = Matrix.Identity;
            _projectionMatrix = Matrix.Identity;

            //			_position = new Vector3(0, 0, 0);
            //			_direction = new Vector3(0, 0, 0);
            //			_upDirection = new Vector3(0, 1, 0);
            //UpdateView();

            //			_fieldOfView = 0.75f;
            //			_aspectRatio = 1.0f;
            //			_nearPlane = 0.5f;
            //			_farPlane = 500.0f;
            //UpdateProjection();

            Reset();
        }

        public void Render()
        {
            //_position = new Vector3(this._x, this._y, this._z);
            UpdateView();
            UpdateProjection();
        }

        public Microsoft.DirectX.Direct3D.Device Device
        {

            get { return _device; }
        }

        public Matrix View
        {

            get { return _viewMatrix; }
        }

        public Matrix Projection
        {

            get { return _projectionMatrix; }
        }

        public float FieldOfView
        {

            get { return _fieldOfView; }
            set { _fieldOfView = value; }
        }

        public float AspectRatio
        {

            get { return _aspectRatio; }
            set { _aspectRatio = value; }
        }

        public Vector3 Position
        {

            get { return _position; }
            set { _position = value; }
        }

        public Vector3 Direction
        {

            get { return _direction; }
            set { _direction = value; }
        }

        public Vector3 UpDirection
        {

            get { return _upDirection; }
            set { _upDirection = value; }
        }

        float angx, angy, angz, scale;
        Vector3 campos, camtarget, camup, pos, center;
        float fov, aspect, near, far;


        /// <summary>
        /// Reset the Parameters
        /// </summary>
        public void Reset()
        {
            angx = angy = angz = 0;
            pos = new Vector3(0, 0, 0);
            center = new Vector3(0, 0, 0);
            scale = 1;

            fov = (float)(Math.PI / 4);
            aspect = 1;
            near = 1;
            far = 100;

            campos = new Vector3(0.0f, 3.0f, 5.0f);
            camtarget = new Vector3(0.0f, 0.0f, 0.0f);
            camup = new Vector3(0.0f, 1.0f, 0.0f);
        }

        public Vector3 CameraUpVector
        {
            get { return camup; }
            set { camup = value; }
        }

        public Vector3 CameraTarget
        {
            get { return camtarget; }
            set { camtarget = value; }
        }

        public Vector3 CameraPosition
        {
            get { return campos; }
            set { campos = value; }
        }

        public Vector3 ObjectCenter
        {
            get { return center; }
            set { center = value; }
        }

        public float NearPlane
        {
            get { return near; }
            set { near = value; }
        }

        public float FarPlane
        {
            get { return far; }
            set { far = value; }
        }

        public float Aspect
        {
            get { return aspect; }
            set { aspect = value; }
        }

        public float FoV
        {
            get { return fov; }
            set { fov = value; }
        }

        public float AngelX
        {
            get { return angx; }
            set { angx = value; }
        }

        public float AngelZ
        {
            get { return angz; }
            set { angz = value; }
        }

        public float AngelY
        {
            get { return angy; }
            set { angy = value; }
        }

        public float X
        {
            get { return pos.X; }
            set { pos.X = value; }
        }

        public float Y
        {
            get { return pos.Y; }
            set { pos.Y = value; }
        }

        public float Z
        {
            get { return pos.Z; }
            set { pos.Z = value; }
        }

        public float Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        void UpdateView()
        {

            _viewMatrix = Matrix.LookAtLH(_position, _direction, _upDirection);
            _device.SetTransform(TransformType.View, _viewMatrix);
        }

        void UpdateProjection()
        {

            _projectionMatrix = Matrix.PerspectiveFovLH(_fieldOfView, _aspectRatio, _nearPlane, _farPlane);
            _device.SetTransform(TransformType.Projection, _projectionMatrix);
        }

        float ConvertDegreesToRadians(float degree)
        {
            return degree * (float)(Math.PI / 180);
        }
    }

    public struct vertex
    {
        public float x;
        public float y;
        public float z;

        //public float nx;
        //public float ny;
        //public float nz;

        public float tu;
        public float tv;
        public float tu2;
        public float tv2;

        public vertex(float newx, float newy, float newz)
        {
            x = newx;
            y = newy;
            z = newz;
            //nx = 0;
            //ny = 0;
            //nz = 0;
            tu = 0;
            tv = 0;
            tu2 = 0;
            tv2 = 0;
        }
        public vertex(float newx, float newy, float newz, float nu, float nv)
        {
            x = newx;
            y = newy;
            z = newz;
            //nx = 0;
            //ny = 0;
            //nz = 0;
            tu = nu;
            tv = nv;
            tu2 = nu;
            tv2 = nv;
        }
        /*
        public vertex(float newx, float newy, float newz, float nu, float nv, float nnx, float nny, float nnz)
        {
            x = newx;
            y = newy;
            z = newz;
            nx = nnx;
            ny = nny;
            nz = nnz;
            tu = nu;
            tv = nv;
            tu2 = nu;
            tv2 = nv;
        }
        */
        public static readonly VertexFormats FVF_Flags = VertexFormats.Position | VertexFormats.Texture2 ;
    };

    public class polygon
    {
        uint v1 = 0;
        uint v2 = 0;
        uint v3 = 0;
        short n1 = 0;
        short n2 = 0;
        short n3 = 0;
        short u1 = 0;
        short u2 = 0;
        short u3 = 0;

        public polygon(uint nv1, uint nv2, uint nv3)
        {
            this.v1 = nv1;
            this.v2 = nv2;
            this.v3 = nv3;
        }

    }

    public class uvcoord
    {
        public float u = 0f;
        public float v = 0f;

        public uvcoord(float nu, float nv)
        {
            this.u = nu;
            this.v = nv;
        }
    }

    public class modelTextures
    {
        public Texture baseTexture;
        public Texture curStencil;
    }

    public class boundingBoxes
    {
        public Vector3 max;
        public Vector3 min;
        public Vector3 mid;
    }

    public class modelInfo
    {

        public string name = "";
        public uint numVertices = 0;

        //public vertex[] vertexList;
        public List<Vector3> vertexList = new List<Vector3>();

        public boundingBoxes bounds = new boundingBoxes();

        //public vertex[] vertexList;
        public uint numVertexNormals = 0;
        public List<Vector3> vertexNormal = new List<Vector3>();
        //public vertex[] vertexNormal;
        public uint numUVs = 0;
        public List<uvcoord> textureVertex = new List<uvcoord>();
        //public uvcoord[] textureVertex;
        public uint numPolygons = 0;
        
        //public polygon[] faces;
        public bool visible = true;

        //public Vector3 midPoint = new Vector3();

        public List<uint> faceData = new List<uint>();
        public List<vertex> vertexData = new List<vertex>();

        //public List<polygon> faces = new List<polygon>();

        public modelTextures textures = new modelTextures();


    }

}
