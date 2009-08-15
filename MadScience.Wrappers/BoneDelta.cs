using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MadScience.Wrappers
{
    public class BoneDelta
    {
        public uint version = 1;
        public List<BoneDeltaEntry> entries = new List<BoneDeltaEntry>();
        public MadScience.Wrappers.OffsetSize offSize = new MadScience.Wrappers.OffsetSize();

        public BoneDelta()
        {
        }

        public BoneDelta(Stream input)
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
            this.version = reader.ReadUInt32();
            uint count = reader.ReadUInt32();
            for (int i = 0; i < count; i++)
            {
                this.entries.Add(new BoneDeltaEntry(input));
            }
            reader = null;
        }

        public void Save(Stream output)
        {
            BinaryWriter writer = new BinaryWriter(output);
            this.offSize.offset = (uint)output.Position;

            writer.Write(this.version);
            writer.Write((uint)this.entries.Count);
            for (int i = 0; i < this.entries.Count; i++)
            {
                this.entries[i].Save(output);
            }
            this.offSize.size = (uint)output.Position - this.offSize.offset;

            writer = null;
        }

        public Stream Save()
        {
            Stream temp = new MemoryStream();
            Save(temp);
            return temp;
        }
    }

    public class BoneDeltaEntry 
    {
        public uint boneHash = 0;
        public MadScience.Wrappers.Vector3 offset = new MadScience.Wrappers.Vector3();
        public MadScience.Wrappers.Vector3 scale = new MadScience.Wrappers.Vector3();
        public MadScience.Wrappers.Vector4 quat = new MadScience.Wrappers.Vector4();

        public BoneDeltaEntry()
        {
        }

        public BoneDeltaEntry(Stream input)
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
            this.boneHash = reader.ReadUInt32();
            this.offset.Load(input);
            this.scale.Load(input);
            this.quat.Load(input);
            reader = null;
        }

        public void Save(Stream output)
        {
            BinaryWriter writer = new BinaryWriter(output);
            writer.Write(this.boneHash);
            this.offset.Save(output);
            this.scale.Save(output);
            this.quat.Save(output);
            writer = null;
        }
    }


}
