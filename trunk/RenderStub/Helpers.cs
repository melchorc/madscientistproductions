using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MadScience.Render
{
    public class Helpers
    {

        private static List<vertexFormat> vertexFormats = new List<vertexFormat>();

        public static modelInfo geomToModel(Stream input)
        {

            modelInfo model = new modelInfo();
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
            bnx = 0f;
            bny = 0f;
            bnz = 0f;
        }

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

    public class modelInfo
    {

        public string name = "";
        public uint numVertices = 0;

        //public vertex[] vertexList;
        //public List<Vector3> vertexList = new List<Vector3>();

        //public boundingBoxes bounds = new boundingBoxes();

        //public vertex[] vertexList;
        public uint numVertexNormals = 0;
        //public List<Vector3> vertexNormal = new List<Vector3>();
        //public List<Vector3> vertexTangentList = new List<Vector3>();
        //public List<Vector3> vertexBiNormalList = new List<Vector3>();
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

        //public modelTextures textures = new modelTextures();


    }

}
