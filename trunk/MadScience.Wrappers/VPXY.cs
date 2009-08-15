using System.Collections.Generic;
using System.IO;
using System;

namespace MadScience.Wrappers
{
    // Create a new VPXY file with associated RCOL header
    public class VPXYFile
    {
        public RcolHeader rcolHeader = new RcolHeader();
        public VPXY vpxy = new VPXY();

        public VPXYFile()
        {
        }

        public VPXYFile(Stream input)
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
            this.vpxy.Load(input);
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
                return;
            }
            rcolHeader.Save(output);
            vpxy.Save(output);

            rcolHeader.chunks[0] = vpxy.offsize;

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

    public class VPXY
    {
        public string magic = "VPXY";
        public uint version = 4;
        public MadScience.Wrappers.KeyTable keytable = new MadScience.Wrappers.KeyTable();

        // Split the entries into Linked and Seperate
        public List<VPXYEntry> linkEntries = new List<VPXYEntry>();
        public List<VPXYEntry> seprEntries = new List<VPXYEntry>();

        public MadScience.Wrappers.BoundingBox boundingbox = new MadScience.Wrappers.BoundingBox();
        public uint unk1 = 0;

        public byte hasFTPT = 0;
        public uint ftptIndex = 0;

        public MadScience.Wrappers.OffsetSize offsize = new MadScience.Wrappers.OffsetSize();

        public VPXY()
        {
        }

        public VPXY(Stream input)
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

            this.offsize.offset = (uint)input.Position;

            this.magic = MadScience.StreamHelpers.ReadStringASCII(input, 4);
            this.version = reader.ReadUInt32();

            this.keytable.offset = reader.ReadUInt32();
            this.keytable.size = reader.ReadUInt32();

            byte entryCount = reader.ReadByte();
            for (int i = 0; i < entryCount; i++)
            {
                VPXYEntry entry = new VPXYEntry(input);
                if (entry.type == 0)
                {
                    linkEntries.Add(entry);
                }
                if (entry.type == 1)
                {
                    seprEntries.Add(entry);
                }
                entry = null;
            }

            byte boundBox = reader.ReadByte();
            if (boundBox == 2)
            {
                this.boundingbox = new MadScience.Wrappers.BoundingBox(input);
            }
            this.unk1 = reader.ReadUInt32();

            this.hasFTPT = reader.ReadByte();
            if (this.hasFTPT == 1)
            {
                this.ftptIndex = reader.ReadUInt32();
            }

            this.keytable.Load(input);

            // Now that we have the keytable, lets load the TGIs into the linked and seperate entries.  This is to
            // make editing easier
            if (this.keytable.keys.Count > 0)
            {
                for (int i = 0; i < seprEntries.Count; i++)
                {
                    this.seprEntries[i].tgiList.Add(this.keytable.keys[(int)this.seprEntries[i].tgiIndex[0]]);
                }
                for (int i = 0; i < linkEntries.Count; i++)
                {
                    for (int j = 0; j < this.linkEntries[i].tgiIndex.Count; j++)
                    {
                        this.linkEntries[i].tgiList.Add(this.keytable.keys[(int)this.linkEntries[i].tgiIndex[j]]);
                    }
                }
            }

            this.offsize.size = (uint)input.Position - this.offsize.offset;

            reader = null;
        }

        public void Save(Stream output)
        {
            BinaryWriter writer = new BinaryWriter(output);

            long startOfChunk = output.Position;
            this.offsize.offset = (uint)startOfChunk;

            MadScience.StreamHelpers.WriteStringASCII(output, this.magic);
            writer.Write(this.version);

            int curOffset = (int)output.Position;
            writer.Write((uint)0);
            writer.Write((uint)0);

            int numEntries = this.linkEntries.Count + this.seprEntries.Count;

            // Clear off the TGI list, so that we can just add entries to it
            this.keytable.keys.Clear();
            
            writer.Write((byte)numEntries);
            for (int i = 0; i < this.seprEntries.Count; i++)
            {
                // Add the TGI to the list
                this.keytable.keys.Add(this.seprEntries[i].tgiList[0]);
                this.seprEntries[i].tgiIndex[0] = (uint)this.keytable.keys.Count - 1;
                this.seprEntries[i].Save(output);
            }
            for (int i = 0; i < this.linkEntries.Count; i++)
            {
                this.linkEntries[i].typeZero = (byte)i;
                this.linkEntries[i].tgiIndex.Clear();
                for (int j = 0; j < this.linkEntries[i].tgiList.Count; j++)
                {
                    this.keytable.keys.Add(this.linkEntries[i].tgiList[j]);
                    this.linkEntries[i].tgiIndex.Add((uint)this.keytable.keys.Count - 1);
                }
                this.linkEntries[i].Save(output);
            }

            writer.Write((byte)2);
            this.boundingbox.Save(output);

            writer.Write(this.unk1);
            writer.Write(this.hasFTPT);
            if (this.hasFTPT == 1)
            {
                writer.Write(this.ftptIndex);
            }

            this.keytable.offset = (uint)(output.Position - curOffset - 4);
            this.keytable.size = 0;
            this.keytable.Save(output);

            long chunkSize = output.Position - startOfChunk;
            this.offsize.size = (uint)chunkSize;

            output.Seek(curOffset, SeekOrigin.Begin);
            writer.Write(this.keytable.offset);
            writer.Write(this.keytable.size);

            // Seek back into the RCOL header

            writer = null;

        }

        public Stream Save()
        {
            Stream temp = new MemoryStream();
            Save(temp);
            return temp;
        }
    }

    public class VPXYEntry
    {
        public byte type = 0;
        // Note that in the spec there is a msIndex, but it's always incremental for type 0 entries
        public List<uint> tgiIndex = new List<uint>();
        public List<MadScience.Wrappers.ResourceKey> tgiList = new List<MadScience.Wrappers.ResourceKey>();
        public byte typeZero = 0;

        public VPXYEntry()
        {
        }

        private void loadFromStream(Stream input)
        {
            BinaryReader reader = new BinaryReader(input);
            this.type = reader.ReadByte();
            if (this.type == 0)
            {
                reader.ReadByte();
                byte count = reader.ReadByte();
                for (int i = 0; i < count; i++)
                {
                    this.tgiIndex.Add(reader.ReadUInt32());
                }
            }
            if (this.type == 1)
            {
                this.tgiIndex.Add(reader.ReadUInt32());
            }

            reader = null;
        }

        public VPXYEntry(Stream input)
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
            writer.Write(this.type);
            //byte numTypeZero = 0;
            if (this.type == 0)
            {
                writer.Write(this.typeZero);
                writer.Write((byte)this.tgiIndex.Count);
                for (int i = 0; i < this.tgiIndex.Count; i++)
                {
                    writer.Write(this.tgiIndex[i]);
                }
            }
            if (this.type == 1)
            {
                writer.Write(this.tgiIndex[0]);
            }

            //if (this.type == 0) numTypeZero++;
            writer = null;
        }
    }

}
