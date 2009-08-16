using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MadScience.Wrappers
{
    public class NameMap
    {
        private uint version = 1;
        public Dictionary<ulong, string> entries = new Dictionary<ulong, string>();

        public NameMap()
        {
        }

        public NameMap(Stream input)
        {
            loadFromStream(input);
        }

        public void Load(Stream input)
        {
            loadFromStream(input);
        }

        private void loadFromStream(Stream input)
        {
            this.version = StreamHelpers.ReadValueU32(input);
            uint count = StreamHelpers.ReadValueU32(input);
            for (int i = 0; i < count; i++)
            {
                ulong instanceId = StreamHelpers.ReadValueU64(input);
                uint nameLength = StreamHelpers.ReadValueU32(input);
                string nameMap = StreamHelpers.ReadStringASCII(input, nameLength);

                this.entries.Add(instanceId, nameMap);
            }
        }

        public void Save(Stream output)
        {
            StreamHelpers.WriteValueU32(output, 1);
            StreamHelpers.WriteValueU32(output, (uint)this.entries.Count);
            foreach (var kvp in this.entries)
            {
                StreamHelpers.WriteValueU64(output, kvp.Key);
                StreamHelpers.WriteValueU32(output, (uint)kvp.Value.Length);
                StreamHelpers.WriteStringASCII(output, kvp.Value);
            }
        }

        public Stream Save()
        {
            Stream temp = new MemoryStream();
            Save(temp);
            return temp;
        }        

    }
}
