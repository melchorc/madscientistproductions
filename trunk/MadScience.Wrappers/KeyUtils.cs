using System.Collections.Generic;
using System.IO;

namespace MadScience.Wrappers
{
    public class KeyTable
    {
        public uint offset = 0;
        public uint size = 0;
        public List<ResourceKey> keys = new List<ResourceKey>();

        public KeyTable()
        {
        }

        public KeyTable(Stream input)
        {
            loadFromStream(input, (int)MadScience.Wrappers.ResourceKeyOrder.TGI);
        }
        public KeyTable(Stream input, uint order)
        {
            loadFromStream(input, order);
        }
        public void Load(Stream input)
        {
            loadFromStream(input, (int)MadScience.Wrappers.ResourceKeyOrder.TGI);
        }
        public void Load(Stream input, uint order)
        {
            loadFromStream(input, order);
        }

        private void loadFromStream(Stream input, uint order)
        {
            if (this.size == 0) return;

            BinaryReader reader = new BinaryReader(input);
            uint numTGIs = reader.ReadUInt32();
            for (int i = 0; i < numTGIs; i++)
            {
                this.keys.Add(new MadScience.Wrappers.ResourceKey(input, order));
            }
            reader = null;
        }

        public void Save(Stream output)
        {
            if (this.keys.Count == 0) return;

            BinaryWriter writer = new BinaryWriter(output);
            this.size += 4;
            writer.Write(this.keys.Count);
            for (int i = 0; i < this.keys.Count; i++)
            {
                this.keys[i].Save(output);
                this.size += 16;
            }
            writer = null;
        }
    }

    public enum ResourceKeyOrder : uint
    {
        TGI = 0,
        IGT = 1,
        ITG = 2,
    }

    public struct ResourceKey
    {
        public uint typeId;
        public uint groupId;
        public ulong instanceId;
        public uint order; // Default to TGI
        
        /*
        public ResourceKey()
        {
            this.typeId = 0;
            this.groupId = 0;
            this.instanceId = 0;
            this.order = 0;
        }
        */

        public ResourceKey(Stream input)
        {
            BinaryReader reader = new BinaryReader(input);
            this.typeId = reader.ReadUInt32();
            this.groupId = reader.ReadUInt32();
            this.instanceId = reader.ReadUInt64();
            this.order = 0;
            reader = null;
        }

        public ResourceKey(Stream input, uint order)
        {
            BinaryReader reader = new BinaryReader(input);
            this.order = order;
            switch (order)
            {
                case 2: // ITG
                    this.instanceId = reader.ReadUInt64();
                    this.typeId = reader.ReadUInt32();
                    this.groupId = reader.ReadUInt32();
                    break;
                case 1: // IGT
                    this.instanceId = reader.ReadUInt64();
                    this.groupId = reader.ReadUInt32();
                    this.typeId = reader.ReadUInt32();
                    break;
                case 0: // TGI
                default:
                    this.typeId = reader.ReadUInt32();
                    this.groupId = reader.ReadUInt32();
                    this.instanceId = reader.ReadUInt64();
                    break;
            }
            reader = null;

        }

        public ResourceKey(string keyString)
        {
            this.order = 0;
            if (Helpers.validateKey(keyString) == false)
            {
                this.typeId = 0;
                this.groupId = 0;
                this.instanceId = 0;
            }
            else
            {
                keyString = keyString.Replace("key:", "");
                string[] temp = keyString.Split(":".ToCharArray());
                this.typeId = MadScience.StringHelpers.ParseHex32("0x" + temp[0]);
                this.groupId = MadScience.StringHelpers.ParseHex32("0x" + temp[1]);
                this.instanceId = MadScience.StringHelpers.ParseHex64("0x" + temp[2]);
            }
        }

        public ResourceKey(string keyString, uint order)
        {
            this.order = order;
            if (Helpers.validateKey(keyString) == false)
            {
                this.typeId = 0;
                this.groupId = 0;
                this.instanceId = 0;
            }
            else
            {
                keyString = keyString.Replace("key:", "");
                string[] temp = keyString.Split(":".ToCharArray());
                this.typeId = MadScience.StringHelpers.ParseHex32("0x" + temp[0]);
                this.groupId = MadScience.StringHelpers.ParseHex32("0x" + temp[1]);
                this.instanceId = MadScience.StringHelpers.ParseHex64("0x" + temp[2]);
            }
        }

        public ResourceKey(uint type, uint group, ulong instance)
        {
            this.order = 0;
            this.typeId = type;
            this.groupId = group;
            this.instanceId = instance;
        }

        public ResourceKey(uint type, uint group, ulong instance, uint order)
        {
            this.order = order;
            this.typeId = type;
            this.groupId = group;
            this.instanceId = instance;
        }

        public override string ToString()
        {
            return "key:" + this.typeId.ToString("X8") + ":" + this.groupId.ToString("X8") + ":" + this.instanceId.ToString("X16");
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != this.GetType())
            {
                return false;
            }

            return (ResourceKey)obj == this;
        }

        public static bool operator !=(ResourceKey a, ResourceKey b)
        {
            return a.typeId != b.typeId || a.groupId != b.groupId || a.instanceId != b.instanceId;
        }

        public static bool operator ==(ResourceKey a, ResourceKey b)
        {
            return a.typeId == b.typeId && a.groupId == b.groupId && a.instanceId == b.instanceId;
        }

        public override int GetHashCode()
        {
            return this.instanceId.GetHashCode() ^ ((int)(this.typeId ^ (this.groupId << 16)));
        }

        public void Save(Stream output)
        {
            Save(output, this.order);
        }

        public void Save(Stream output, uint order)
        {
            BinaryWriter writer = new BinaryWriter(output);
            switch (order)
            {
                case 2: // ITG
                    writer.Write(this.instanceId);
                    writer.Write(this.typeId);
                    writer.Write(this.groupId);
                    break;
                case 1: // IGT
                    writer.Write(this.instanceId);
                    writer.Write(this.groupId);
                    writer.Write(this.typeId);
                    break;
                case 0: // TGI
                default:
                    writer.Write(this.typeId);
                    writer.Write(this.groupId);
                    writer.Write(this.instanceId);
                    break;
            }

            writer = null;
        }
    }

    class Helpers
    {
        public static bool validateKey(string keyString)
        {
            bool retVal = true;

            if (keyString.Trim() == "")
            {
                return false;
            }
            if (!keyString.StartsWith("key:")) retVal = false;
            if (!keyString.Contains(":")) retVal = false;
            string[] temp = keyString.Split(":".ToCharArray());
            if (temp.Length < 4) retVal = false;

            return retVal;
        }
    }
}
