using System.IO;

namespace MadScience.Wrappers
{

    public enum AgeGenderFlags : uint
    {
        Toddler = 0x2,
        Child = 0x4,
        Teen = 0x8,
        YoungAdult = 0x10,
        Adult = 0x20,
        Elder = 0x40,
        Male = 0x1000,
        Female = 0x2000,
        Human = 0x10000,
        LeftHanded = 0x100000,
        RightHanded = 0x200000,
    }

    public enum casPartType : uint
    {
        Hair = 1,
        Scalp = 2,
        Face = 3,
        Body = 4,
        Top = 5,
        Bottom = 6,
        Shoes = 7,
        Earrings = 11,
        GlassesF = 12,
        Bracelets = 13,
        RingLeft = 14,
        RingRight = 15,
        Beard = 16,
        Lipstick = 17,
        Eyeshadow = 18,
        Eyeliner = 19,
        Blush = 20,
        Makeup = 21,
        Eyebrow = 22,
        Glove = 24,
        Socks = 25,
        Mascara = 26,
        Weathering = 29,
        EarringLeft = 30,
        EarringRight = 31,
    }

    public class Vector2
    {
        public float u = 0f;
        public float v = 0f;

        public Vector2()
        {
        }

        private void loadFromStream(Stream input)
        {
            BinaryReader reader = new BinaryReader(input);
            this.u = reader.ReadSingle();
            this.v = reader.ReadSingle();
            reader = null;
        }
        public Vector2(Stream input)
        {
            loadFromStream(input);
        }

        public Vector2(float nx, float ny, float nz)
        {
            this.u = nx;
            this.v = ny;
        }

        public void Load(Stream input)
        {
            loadFromStream(input);
        }

        public void Save(Stream output)
        {
            BinaryWriter writer = new BinaryWriter(output);
            writer.Write(this.u);
            writer.Write(this.v);
            writer = null;
        }
    }

    public class UShort3
    {
        public ushort a = 0;
        public ushort b = 0;
        public ushort c = 0;

        public UShort3()
        {
        }

        private void loadFromStream(Stream input)
        {
            BinaryReader reader = new BinaryReader(input);
            this.a = reader.ReadUInt16();
            this.b = reader.ReadUInt16();
            this.c = reader.ReadUInt16();
            reader = null;
        }
        public UShort3(Stream input)
        {
            loadFromStream(input);
        }

        public UShort3(ushort na, ushort nb, ushort nc)
        {
            this.a = na;
            this.b = nb;
            this.c = nc;
        }

        public void Load(Stream input)
        {
            loadFromStream(input);
        }

        public void Save(Stream output)
        {
            BinaryWriter writer = new BinaryWriter(output);
            writer.Write(this.a);
            writer.Write(this.b);
            writer.Write(this.c);
            writer = null;
        }
    }

    public class Vector3
    {
        public float x = 0f;
        public float z = 0f;
        public float y = 0f;

        public Vector3()
        {
        }

        private void loadFromStream(Stream input)
        {
            BinaryReader reader = new BinaryReader(input);
            this.x = reader.ReadSingle();
            this.y = reader.ReadSingle();
            this.z = reader.ReadSingle();
            reader = null;
        }
        public Vector3(Stream input)
        {
            loadFromStream(input);
        }

        public Vector3(float nx, float ny, float nz)
        {
            this.x = nx;
            this.y = ny;
            this.z = nz;
        }

        public void Load(Stream input)
        {
            loadFromStream(input);
        }

        public void Save(Stream output)
        {
            BinaryWriter writer = new BinaryWriter(output);
            writer.Write(this.x);
            writer.Write(this.y);
            writer.Write(this.z);
            writer = null;
        }
    }

    public class Vector4
    {
        public float x = 0f;
        public float z = 0f;
        public float y = 0f;
        public float w = 0f;

        public Vector4()
        {
        }

        private void loadFromStream(Stream input)
        {
            BinaryReader reader = new BinaryReader(input);
            this.x = reader.ReadSingle();
            this.y = reader.ReadSingle();
            this.z = reader.ReadSingle();
            this.w = reader.ReadSingle();
            reader = null;
        }
        public Vector4(Stream input)
        {
            loadFromStream(input);
        }

        public Vector4(float nx, float ny, float nz, float nw)
        {
            this.x = nx;
            this.y = ny;
            this.z = nz;
            this.w = nw;
        }

        public void Load(Stream input)
        {
            loadFromStream(input);
        }

        public void Save(Stream output)
        {
            BinaryWriter writer = new BinaryWriter(output);
            writer.Write(this.x);
            writer.Write(this.y);
            writer.Write(this.z);
            writer.Write(this.w);
            writer = null;
        }
    }

    public class BoundingBox
    {
        public MadScience.Wrappers.Vector3 min = new MadScience.Wrappers.Vector3();
        public MadScience.Wrappers.Vector3 max = new MadScience.Wrappers.Vector3();

        public BoundingBox()
        {
        }

        private void loadFromStream(Stream input)
        {
            this.min.Load(input);
            this.max.Load(input);
        }

        public BoundingBox(Stream input)
        {
            loadFromStream(input);
        }
        public void Load(Stream input)
        {
            loadFromStream(input);
        }

        public void Save(Stream output)
        {
            this.min.Save(output);
            this.max.Save(output);
        }
    }

    public class OffsetSize
    {
        public uint offset = 0;
        public uint size = 0;

        public OffsetSize()
        {
        }

        public OffsetSize(uint Offset, uint Size)
        {
            this.offset = Offset;
            this.size = Size;
        }

        private void loadFromStream(Stream input)
        {
            BinaryReader reader = new BinaryReader(input);
            this.offset = reader.ReadUInt32();
            this.size = reader.ReadUInt32();
            reader = null;
        }
        public OffsetSize(Stream input)
        {
            loadFromStream(input);
        }
        public void Load(Stream input)
        {
            loadFromStream(input);
        }

        public void Save(Stream output)
        {
            BinaryWriter writer = new BinaryWriter(output);
            writer.Write(this.offset);
            writer.Write(this.size);
            writer = null;
        }
    }

}
