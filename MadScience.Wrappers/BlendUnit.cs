using System.Collections.Generic;
using System.IO;

namespace MadScience.Wrappers
{
    public enum casPanelGroup : uint
    {
        HeadAndEars = 2,
        Mouth = 8,
        Nose = 16,
        Eyelash = 64,
        Eyes = 128,
    }

    public class BlendUnit
    {

        public uint version = 3;
        private MadScience.Wrappers.KeyTable keytable = new MadScience.Wrappers.KeyTable();
        public ulong localeHash = 0;
        public List<ResourceKey> blendLinks = new List<ResourceKey>();
        public byte bidirectional = 0;
        public uint casPanelGroup = 0;
        public uint casPanelSubGroup = 0;

        public BlendUnit()
        {
        }

        private void loadFromStream(Stream input)
        {
            BinaryReader reader = new BinaryReader(input);

            this.version = reader.ReadUInt32();
            this.keytable.offset = reader.ReadUInt32();
            this.keytable.size = reader.ReadUInt32();
            this.localeHash = reader.ReadUInt64();

            uint indexersCount = reader.ReadUInt32();
            List<uint> indexers = new List<uint>();
            for (int i = 0; i < indexersCount; i++)
            {
                indexers.Add(reader.ReadUInt32());
            }

            this.bidirectional = reader.ReadByte();
            this.casPanelGroup = reader.ReadUInt32();
            this.casPanelSubGroup = reader.ReadUInt32();

            reader.ReadUInt32();

            uint tgiCount = reader.ReadUInt32();
            for (int i = 0; i < tgiCount; i++)
            {
                MadScience.Wrappers.ResourceKey temp = new MadScience.Wrappers.ResourceKey(input);
                this.keytable.keys.Add(temp);
                //temp = null;
            }

            for (int i = 0; i < indexersCount; i++)
            {
                this.blendLinks.Add(this.keytable.keys[(int)indexers[i]]);
            }

            reader = null;
        }

        public BlendUnit(Stream input)
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

            writer.Write(this.version);
            writer.Write((uint)0);
            writer.Write((uint)0);
            writer.Write(this.localeHash);
            writer.Write(this.blendLinks.Count);
            for (int i = 0; i < this.blendLinks.Count; i++)
            {
                writer.Write(i);
            }
            writer.Write(this.bidirectional);
            writer.Write(this.casPanelGroup);
            writer.Write(this.casPanelSubGroup);
            writer.Write((uint)0);

            uint tgiOffset = (uint)output.Position - 8;
            this.keytable.size = 4;
            writer.Write(this.blendLinks.Count);
            for (int i = 0; i < blendLinks.Count; i++)
            {
                this.blendLinks[i].Save(output);
                this.keytable.size += 16;
            }
            // Seek back to byte 4 to output the keytable length and size
            output.Seek(4, SeekOrigin.Begin);
            writer.Write(tgiOffset);
            writer.Write(this.keytable.size);
        }

        public Stream Save()
        {
            Stream temp = new MemoryStream();
            Save(temp);
            return temp;
        }
    }

}
