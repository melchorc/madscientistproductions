using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MadScience.Wrappers
{
    public class STBL
    {
        private string magic = "STBL";
        public byte version = 2;
        private ushort blank = 0;
        public List<STBLEntry> Items = new List<STBLEntry>();

        public STBL()
        {
        }

        public STBL(Stream input)
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
            this.magic = Encoding.ASCII.GetString(reader.ReadBytes(4));

            this.version = reader.ReadByte();
            this.blank = reader.ReadUInt16();

            uint count = reader.ReadUInt32();
            reader.ReadBytes(6);

            for (int i = 0; i < count; i++)
            {
                this.Items.Add(new STBLEntry(input));
            }

            reader = null;
        }

        public void Save(Stream output)
        {
            BinaryWriter writer = new BinaryWriter(output);

            MadScience.StreamHelpers.WriteStringASCII(output, this.magic);
            writer.Write(this.version);
            writer.Write(this.blank);
            writer.Write((uint)this.Items.Count);
            writer.Write((uint)0);
            writer.Write((ushort)0);

            for (int i = 0; i < this.Items.Count; i++)
            {
                this.Items[i].Save(output);
            }

            writer = null;
        }

        public Stream Save()
        {
            Stream temp = new MemoryStream();
            Save(temp);
            return temp;
        }

        
    }

    public class STBLEntry
    {
        ulong hash = 0;
        string stringText = "";

        public STBLEntry()
        {
        }

        public STBLEntry(ulong nHash, string nString)
        {
            this.hash = nHash;
            this.stringText = nString;
        }

        public STBLEntry(Stream input)
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
            this.hash = reader.ReadUInt64();
            uint stringLength = reader.ReadUInt32();
            this.stringText = MadScience.StreamHelpers.ReadStringUTF16(input, stringLength * 2);
            reader = null;
        }

        public void Save(Stream output)
        {
            BinaryWriter writer = new BinaryWriter(output);
            writer.Write(this.hash);
            writer.Write((uint)this.stringText.Length);
            MadScience.StreamHelpers.WriteStringUTF16(output, this.stringText);
            writer = null;
        }

        public Stream Save()
        {
            Stream temp = new MemoryStream();
            Save(temp);
            return temp; 
        }
    }
}
