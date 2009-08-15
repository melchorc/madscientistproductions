using System.Collections.Generic;
using System.IO;

namespace MadScience.Wrappers
{
    public class RcolHeader
    {
        public uint version = 3;
        public uint datatype = 1;

        // These are IGT lists *not TGI*
        public List<MadScience.Wrappers.ResourceKey> index3 = new List<ResourceKey>();
        public List<MadScience.Wrappers.ResourceKey> externalChunks = new List<ResourceKey>();
        public List<MadScience.Wrappers.ResourceKey> internalChunks = new List<ResourceKey>();

        public List<MadScience.Wrappers.OffsetSize> chunks = new List<OffsetSize>();

        public uint chunkListOffset = 0;

        public RcolHeader()
        {
        }

        public RcolHeader(Stream input)
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

            //addListItem("Start RCOL header", "");
            this.version = reader.ReadUInt32();
            this.datatype = reader.ReadUInt32();
            uint rcolIndex3 = reader.ReadUInt32();
            uint rcolIndex1 = reader.ReadUInt32();
            uint rcolIndex2 = reader.ReadUInt32();
            for (int i = 0; i < rcolIndex2; i++)
            {
                MadScience.Wrappers.ResourceKey rKey = new ResourceKey(input, (int)ResourceKeyOrder.ITG);
                this.internalChunks.Add(rKey);
                //rKey = null;
            }

            for (int i = 0; i < rcolIndex1; i++)
            {
                MadScience.Wrappers.ResourceKey rKey = new ResourceKey(input, (int)ResourceKeyOrder.ITG);
                this.externalChunks.Add(rKey);
                //rKey = null;
            }

            for (int i = 0; i < rcolIndex2; i++)
            {
                MadScience.Wrappers.OffsetSize offSize = new OffsetSize(input);
                this.chunks.Add(offSize);
                offSize = null;

            }
            //addListItem("End RCOL header", "");

            reader = null;
        }

        public void Save(Stream output)
        {
            BinaryWriter writer = new BinaryWriter(output);
            writer.Write(this.version);
            writer.Write(this.datatype);
            writer.Write(this.index3.Count);
            writer.Write(this.externalChunks.Count);
            writer.Write(this.internalChunks.Count);
            for (int i = 0; i < this.internalChunks.Count; i++)
            {
                this.internalChunks[i].Save(output);
            }
            for (int i = 0; i < this.externalChunks.Count; i++)
            {
                this.externalChunks[i].Save(output);
            }

            this.chunkListOffset = (uint)output.Position;
            for (int i = 0; i < this.internalChunks.Count; i++)
            {
                this.chunks[i].Save(output);
            }
            writer = null;
        }
    }

}
