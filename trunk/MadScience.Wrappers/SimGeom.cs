using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MadScience.Wrappers
{
    public class SimGeomFile
    {
        public RcolHeader rcolHeader = new RcolHeader();
        public SimGeom simgeom = new SimGeom();

        public SimGeomFile()
        {
        }

        public SimGeomFile(Stream input)
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
            this.simgeom.Load(input);
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
            this.simgeom.Save(output);

            rcolHeader.chunks[0] = simgeom.offSize;

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
    

    public class SimGeom
    {
        public string magic = "GEOM";
        public uint version = 5;
        public OffsetSize offSize = new OffsetSize();
        public uint embeddedId = 0;
        public byte[] embeddedChunk;
        public uint unk1 = 0;
        public uint unk2 = 0;

        public uint numVertices = 0;

        public KeyTable keytable = new KeyTable();

        public SimGeom()
        {
        }

        public SimGeom(Stream input)
        {
            loadFromStream(input);
        }

        public void Load(Stream input)
        {
            loadFromStream(input);
        }

        private void loadFromStream(Stream input)
        {
            this.magic = MadScience.StreamHelpers.ReadStringASCII(input, 4);
            this.version = MadScience.StreamHelpers.ReadValueU32(input);
            this.keytable.offset = MadScience.StreamHelpers.ReadValueU32(input);
            this.keytable.size = MadScience.StreamHelpers.ReadValueU32(input);

            long seekFrom = input.Position - 4;

            input.Seek(this.keytable.offset + seekFrom, SeekOrigin.Begin);

            this.keytable.Load(input);

        }

        public void Save(Stream output)
        {
            this.offSize.offset = (uint)output.Position;



            this.offSize.size = (uint)output.Position - this.offSize.offset;
        }

        public Stream Save()
        {
            Stream temp = new MemoryStream();
            Save(temp);
            return temp;
        }
    }
}
