using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MadScience.Wrappers
{
    public class BlendGeom
    {
        public string magic = "BGEO";
        public uint version = 768;

        //public uint s1count = 0;
        //public uint s1subcount = 0;
        //public uint s2count = 0;
        //public uint s3count = 0;
        public uint s1presubcount = 0;
        public uint s1subentryentrysize = 0;

        public uint s1offset = 0;
        public uint s2offset = 0;
        public uint s3offset = 0;

        public List<BlendGeomSection1> section1 = new List<BlendGeomSection1>();
        public List<Int16> section2 = new List<Int16>();
        public List<BlendGeomSection3> section3 = new List<BlendGeomSection3>();

        public BlendGeom()
        {
        }

        public BlendGeom(Stream input)
        {
            loadFromStream(input);
        }

        public void Load(Stream input)
        {
            loadFromStream(input);
        }

        private void loadFromStream(Stream input)
        {
            BinaryReader reader = new BinaryReader(input);

            this.magic = MadScience.StreamHelpers.ReadStringASCII(input, 4);
            this.version = reader.ReadUInt32();

            uint s1count = reader.ReadUInt32();
            uint s1subcount = reader.ReadUInt32();

            uint s2count = reader.ReadUInt32();
            uint s3count = reader.ReadUInt32();

            this.s1presubcount = reader.ReadUInt32();
            this.s1subentryentrysize = reader.ReadUInt32();

            this.s1offset = reader.ReadUInt32();
            this.s2offset = reader.ReadUInt32();
            this.s3offset = reader.ReadUInt32();

            for (int i = 0; i < s1count; i++)
            {
                BlendGeomSection1 entry = new BlendGeomSection1();
                entry.subEntryCount = s1subcount;
                entry.Load(input);
                this.section1.Add(entry);
                entry = null;
            }

            for (int i = 0; i < s2count; i++)
            {
                this.section2.Add(reader.ReadInt16());
            }

            BoundingBox bbox = new BoundingBox();

            for (int i = 0; i < s3count; i++)
            {
                this.section3.Add(new BlendGeomSection3(input));
                float v1 = this.section3[i].v1 / 256f;
                float v2 = this.section3[i].v2 / 256f;
                float v3 = this.section3[i].v3 / 256f;

                if (v1 < bbox.min.x) bbox.min.x = v1;
                if (v2 < bbox.min.y) bbox.min.y = v2; 
                if (v3 < bbox.min.z) bbox.min.z = v3;
                if (v1 > bbox.max.x) bbox.max.x = v1;
                if (v2 > bbox.max.y) bbox.max.y = v2;
                if (v3 > bbox.max.z) bbox.max.z = v3;
            }

            printS3(this.section3[0]);
            printS3(this.section3[1]);
            printS3(this.section3[4]);

            reader = null;
        }

        private void printS3(BlendGeomSection3 entry)
        {
            Console.WriteLine(entry.v1.ToString() + " " + entry.v2.ToString() + " " + entry.v3.ToString());
            Console.WriteLine((entry.v1 / 256).ToString() + " " + (entry.v2 / 256).ToString() + " " + (entry.v3 / 256).ToString());
            Console.WriteLine((entry.v1 / 512).ToString() + " " + (entry.v2 / 512).ToString() + " " + (entry.v3 / 512).ToString());
            Console.WriteLine((entry.v1 / 1024).ToString() + " " + (entry.v2 / 1024).ToString() + " " + (entry.v3 / 1024).ToString());
            Console.WriteLine((entry.v1 / 2048).ToString() + " " + (entry.v2 / 2048).ToString() + " " + (entry.v3 / 2048).ToString());
            Console.WriteLine((entry.v1 / 4096).ToString() + " " + (entry.v2 / 4096).ToString() + " " + (entry.v3 / 4096).ToString());
            Console.WriteLine((entry.v1 / 8192).ToString() + " " + (entry.v2 / 8192).ToString() + " " + (entry.v3 / 8192).ToString());
            Console.WriteLine((entry.v1 / 16384).ToString() + " " + (entry.v2 / 16384).ToString() + " " + (entry.v3 / 16384).ToString());

        }

        public void Save(Stream output)
        {
        }

        public Stream Save()
        {
            Stream temp = new MemoryStream();
            Save(temp);
            return temp;
        }
    }

    public class BlendGeomSection1
    {
        public uint ageGenderFlags = 0;
        public uint regionFlags = 0;
        public uint subEntryCount = 4;
        public List<BlendGeomSection1SubEntry> subentries = new List<BlendGeomSection1SubEntry>();

        public BlendGeomSection1()
        {
        }

        public BlendGeomSection1(Stream input)
        {
            loadFromStream(input);
        }


        public void Load(Stream input)
        {
            loadFromStream(input);
        }

        private void loadFromStream(Stream input)
        {
            BinaryReader reader = new BinaryReader(input);
            this.ageGenderFlags = reader.ReadUInt32();
            this.regionFlags = reader.ReadUInt32();
            for (int i = 0; i < this.subEntryCount; i++)
            {
                this.subentries.Add(new BlendGeomSection1SubEntry(input));
            }
            reader = null;
        }

        public void Save(Stream output)
        {
        }
    }

    public class BlendGeomSection1SubEntry
    {
        public uint lastFaceIndex = 0;
        public uint faceIndexList = 0;
        public uint blendVectorList = 0;

        public BlendGeomSection1SubEntry()
        {
        }

        public BlendGeomSection1SubEntry(Stream input)
        {
            loadFromStream(input);
        }

        public void Load(Stream input)
        {
            loadFromStream(input);
        }

        private void loadFromStream(Stream input)
        {
            BinaryReader reader = new BinaryReader(input);
            this.lastFaceIndex = reader.ReadUInt32();
            this.faceIndexList = reader.ReadUInt32();
            this.blendVectorList = reader.ReadUInt32();
            reader = null;

        }

        public void Save(Stream output)
        {
            BinaryWriter writer = new BinaryWriter(output);
            writer.Write(this.lastFaceIndex);
            writer.Write(this.faceIndexList);
            writer.Write(this.blendVectorList);
            writer = null;
        }
    }

    public class BlendGeomSection3
    {
        public float v1 = 0f;
        public float v2 = 0f; 
        public float v3 = 0f;

        public BlendGeomSection3()
        {
        }

        public BlendGeomSection3(Stream input)
        {
            loadFromStream(input);   
        }

        public void Load(Stream input)
        {
            loadFromStream(input);
        }

        private void loadFromStream(Stream input)
        {
            BinaryReader reader = new BinaryReader(input);

            ushort v1n = reader.ReadUInt16();
            ushort v2n = reader.ReadUInt16();
            ushort v3n = reader.ReadUInt16();

            //this.v1 = GetSingle16(v1n);
            //this.v2 = GetSingle16(v2n);
            //this.v3 = GetSingle16(v3n);

            this.v1 = (v1n - 0x8000);
            this.v2 = (v2n - 0x8000);
            this.v3 = (v3n - 0x8000);

            //float half1 = (v1n - 0x8000);
            //float half2 = (v2n - 0x8000);
            //float half3 = (v3n - 0x8000);

            //Console.WriteLine(v1n.ToString() + "(0x" + v1n.ToString("X4") + ") -> " + this.v1.ToString() + " / " + half1.ToString());
            //Console.WriteLine(v2n.ToString() + "(0x" + v2n.ToString("X4") + ") -> " + this.v2.ToString() + " / " + half2.ToString());
            //Console.WriteLine(v3n.ToString() + "(0x" + v3n.ToString("X4") + ") -> " + this.v3.ToString() + " / " + half3.ToString());

            reader = null;
        }

        protected static float GetSingle16(UInt16 val)
        {
            if (val == 0x0) { return 0f; }
            if (val == 0x8000) { return -0f; }

            UInt32 sign = (UInt32)val >> 15;
            Int32 exponent = (Int32)((val >> 10) & ((1 << 5) - 1)) - ((1 << 4) - 1);
            UInt32 significand = (UInt32)val & ((1 << 10) - 1);
            return BitConverter.ToSingle(BitConverter.GetBytes((UInt32)((sign << 31) | ((UInt32)(exponent + 127) << 23) | significand)), 0);
        }
    }
}
