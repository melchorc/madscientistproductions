using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MadScience.Wrappers
{
    public class BoneDeltaFile
    {
        public RcolHeader rcolHeader = new RcolHeader();
        public BoneDelta bonedelta = new BoneDelta();

        public BoneDeltaFile()
        {
        }

        public BoneDeltaFile(Stream input)
        {
            loadFromStream(input);
        }

        public void Load(Stream input)
        {
            loadFromStream(input);
        }

        private void loadFromStream(Stream input)
        {
            this.rcolHeader.Load(input);
            this.bonedelta.Load(input);
        }

        public void Save(Stream output)
        {
            if (this.rcolHeader.chunks.Count == 0)
            {
                this.rcolHeader.chunks.Add(new MadScience.Wrappers.OffsetSize());
            }
            if (this.rcolHeader.externalChunks.Count == 0 && this.rcolHeader.internalChunks.Count == 0)
            {
                throw new Exception("You must have a valid RCOL header");
                //return;
            }

            this.rcolHeader.Save(output);
            this.bonedelta.Save(output);

            rcolHeader.chunks[0] = bonedelta.offSize;

            output.Seek(rcolHeader.chunkListOffset, SeekOrigin.Begin);
            for (int i = 0; i < rcolHeader.chunks.Count; i++)
            {
                rcolHeader.chunks[i].Save(output);
            }
        }

        public Stream Save()
        {
            Stream temp = new MemoryStream();
            Save(temp);
            return temp;
        }
    }

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
