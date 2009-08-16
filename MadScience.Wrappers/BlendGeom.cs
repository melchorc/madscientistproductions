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
        public uint s1subcount = 0;
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
            this.s1subcount = reader.ReadUInt32();

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

            reader = null;
        }

        public void Save(Stream output)
        {
            MadScience.StreamHelpers.WriteStringASCII(output, this.magic);
            MadScience.StreamHelpers.WriteValueU32(output, this.version);
            MadScience.StreamHelpers.WriteValueU32(output, (uint)this.section1.Count);
            MadScience.StreamHelpers.WriteValueU32(output, this.s1subcount);
            MadScience.StreamHelpers.WriteValueU32(output, (uint)this.section2.Count);
            MadScience.StreamHelpers.WriteValueU32(output, (uint)this.section3.Count);
            MadScience.StreamHelpers.WriteValueU32(output, this.s1presubcount);
            MadScience.StreamHelpers.WriteValueU32(output, this.s1subentryentrysize);

            long subSectionOffset = output.Position;
            // Write null offsets for now
            MadScience.StreamHelpers.WriteValueU32(output, 0);
            MadScience.StreamHelpers.WriteValueU32(output, 0);
            MadScience.StreamHelpers.WriteValueU32(output, 0);

            this.s1offset = (uint)output.Position;
            for (int i = 0; i < this.section1.Count; i++)
            {
                this.section1[i].Save(output);
            }

            this.s2offset = (uint)output.Position;
            for (int i = 0; i < this.section2.Count; i++)
            {
                MadScience.StreamHelpers.WriteValueS16(output, this.section2[i]);
            }

            this.s3offset = (uint)output.Position;
            for (int i = 0; i < this.section3.Count; i++)
            {
                this.section3[i].Save(output);
            }

            // Write offsets
            output.Seek(subSectionOffset, SeekOrigin.Begin);
            MadScience.StreamHelpers.WriteValueU32(output, this.s1offset);
            MadScience.StreamHelpers.WriteValueU32(output, this.s2offset);
            MadScience.StreamHelpers.WriteValueU32(output, this.s3offset);


        }

        public Stream Save()
        {
            Stream temp = new MemoryStream();
            Save(temp);
            return temp;
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
            this.ageGenderFlags = MadScience.StreamHelpers.ReadValueU32(input);
            this.regionFlags = MadScience.StreamHelpers.ReadValueU32(input);
            for (int i = 0; i < this.subEntryCount; i++)
            {
                this.subentries.Add(new BlendGeomSection1SubEntry(input));
            }
            reader = null;
        }

        public void Save(Stream output)
        {
            MadScience.StreamHelpers.WriteValueU32(output, this.ageGenderFlags);
            MadScience.StreamHelpers.WriteValueU32(output, this.regionFlags);
            for (int i = 0; i < this.subEntryCount; i++)
            {
                this.subentries[i].Save(output);
            }
        }

        public Stream Save()
        {
            Stream temp = new MemoryStream();
            Save(temp);
            return temp;
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
            this.lastFaceIndex = MadScience.StreamHelpers.ReadValueU32(input);
            this.faceIndexList = MadScience.StreamHelpers.ReadValueU32(input);
            this.blendVectorList = MadScience.StreamHelpers.ReadValueU32(input);
        }

        public void Save(Stream output)
        {
            MadScience.StreamHelpers.WriteValueU32(output, this.lastFaceIndex);
            MadScience.StreamHelpers.WriteValueU32(output, this.faceIndexList);
            MadScience.StreamHelpers.WriteValueU32(output, this.blendVectorList);
        }

        public Stream Save()
        {
            Stream temp = new MemoryStream();
            Save(temp);
            return temp;
        }

    }

    public class BlendGeomSection3
    {
        public ushort v1 = 0;
        public ushort v2 = 0; 
        public ushort v3 = 0;

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
            ushort v1n = MadScience.StreamHelpers.ReadValueU16(input);
            ushort v2n = MadScience.StreamHelpers.ReadValueU16(input);
            ushort v3n = MadScience.StreamHelpers.ReadValueU16(input);

            this.v1 = v1n;
            this.v2 = v2n;
            this.v3 = v3n;

            //float half1 = (v1n - 0x8000);
            //float half2 = (v2n - 0x8000);
            //float half3 = (v3n - 0x8000);
        }

        public void Save(Stream output)
        {
            MadScience.StreamHelpers.WriteValueU16(output, this.v1);
            MadScience.StreamHelpers.WriteValueU16(output, this.v2);
            MadScience.StreamHelpers.WriteValueU16(output, this.v3);
        }

        public Stream Save()
        {
            Stream temp = new MemoryStream();
            Save(temp);
            return temp;
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
