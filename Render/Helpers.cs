using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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

            if (input.Length == 0) return model;

            BinaryReader reader = new BinaryReader(input);

            // Read in the RCOL Header
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
            float minX = 50f;
            float minY = 50f;
            float minZ = 50f;
            float maxX = -50f;
            float maxY = -50f;
            float maxZ = -50f;

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
                            model.vertexTangentList.Add(new Vector3(x, y, z));
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
                model.vertexData.Add(new vertex(-model.vertexList[i].X, model.vertexList[i].Y, model.vertexList[i].Z, model.textureVertex[i].u, model.textureVertex[i].v, -model.vertexNormal[i].X, model.vertexNormal[i].Y, model.vertexNormal[i].Z, -model.vertexTangentList[i].X, model.vertexTangentList[i].Y, model.vertexTangentList[i].Z));
            }

            // Go through the vertex lists again, this time normalising the values
            float midX = minX + ((maxX - minX) / 2);
            float midY = minY + ((maxY - minY) / 2);
            float midZ = minZ + ((maxZ - minZ) / 2);

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

    public struct vertex
    {
        public float x;
        public float y;
        public float z;

        public float tu;
        public float tv;

        public float bnx;
        public float bny;
        public float bnz;

        public float tx;
        public float ty;
        public float tz;

        public float nx;
        public float ny;
        public float nz;

        public vertex(float newx, float newy, float newz)
        {
            x = newx;
            y = newy;
            z = newz;
            nx = 0;
            ny = 1;
            nz = 0;
            tu = 0;
            tv = 0;
            bnx = 0;
            bny = 0;
            bnz = 1;
            tx = 1;
            ty = 0;
            tz = 1;
        }
        public vertex(float newx, float newy, float newz, float nu, float nv)
        {
            x = newx;
            y = newy;
            z = newz;
            nx = 0;
            ny = 1;
            nz = 0;
            tu = nu;
            tv = nv;
            bnx = 0;
            bny = 0;
            bnz = 1;
            tx = 1;
            ty = 0;
            tz = 1;
        }

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
            bnx = 0;
            bny = 0;
            bnz = 1;
            tx = 1;
            ty = 0;
            tz = 1;
        }

        public vertex(float newx, float newy, float newz, float nu, float nv, float nnx, float nny, float nnz, float tnx, float tny, float tnz)
        {
            x = newx;
            y = newy;
            z = newz;
            nx = nnx;
            ny = nny;
            nz = nnz;
            tu = nu;
            tv = nv;
            tx = tnx;
            ty = tny;
            tz = tnz;
            // Calculate binormal based on the cross product of the normal and tangent
            // Should really be saving the sign of the cross product here
            Vector3 binormal = Vector3.Normalize(Vector3.Cross(new Vector3(nx, ny, nz), new Vector3(tx, ty, tz)));
            bnx = binormal.X;
            bny = binormal.Y;
            bnz = binormal.Z;
        }

        public static readonly VertexFormats FVF_Flags = VertexFormats.Position | VertexFormats.Texture1;
        public static readonly VertexElement[] Elements = new VertexElement[] {
            new VertexElement(0, 0, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Position, 0),
            new VertexElement(0, 12, DeclarationType.Float2, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 0),
            new VertexElement(0, 20, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.BiNormal, 0),
            new VertexElement(0, 32, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Tangent, 0),
            new VertexElement(0, 44, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Normal, 0),
            VertexElement.VertexDeclarationEnd
        };
        public static readonly int SizeInBytes = 56;
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
        public Texture specularTexture;
        public Texture ambientTexture;
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
        public List<Vector3> vertexTangentList = new List<Vector3>();
        public List<Vector3> vertexBiNormalList = new List<Vector3>();
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
