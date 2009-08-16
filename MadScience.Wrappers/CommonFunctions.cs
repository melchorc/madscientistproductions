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
