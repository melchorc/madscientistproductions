using System.Collections.Generic;
using System.IO;

namespace MadScience.Wrappers
{
    public enum facialRegions : uint
    {
        Eyes = 0x1,
        Nose = 0x2,
        Mouth = 0x4,
        TranslateMouth = 0x8,
        Ears = 0x10,
        TranslateEyes = 0x20,
        Face = 0x40,
        Head = 0x80,
        Brow = 0x100,
        Jaw = 0x200,
        Body = 0x400,
        Eyelashes = 0x800,
    }

    public class FacialBlend
    {
        public uint version = 8;
        public MadScience.Wrappers.KeyTable keytable = new MadScience.Wrappers.KeyTable();
        public string partName = "";
        // 1 = Archtype, 2 = Modifier
        public uint blendType = 0;
        public MadScience.Wrappers.ResourceKey blendTgi = new MadScience.Wrappers.ResourceKey();
        public List<FacialBlendGeomBoneEntry> geomBoneEntries = new List<FacialBlendGeomBoneEntry>();

        private void loadFromStream(Stream input)
        {
            BinaryReader reader = new BinaryReader(input);
            this.version = reader.ReadUInt32();
            this.keytable.offset = reader.ReadUInt32();
            this.keytable.size = reader.ReadUInt32();

            byte nameLength = reader.ReadByte(); 
            this.partName = MadScience.StreamHelpers.ReadStringUTF16(input, false, (uint)nameLength);
            this.blendType = reader.ReadUInt32();

            this.blendTgi = new MadScience.Wrappers.ResourceKey(input, (int)MadScience.Wrappers.ResourceKeyOrder.TGI);

            uint geomCount = reader.ReadUInt32();
            for (int i = 0; i < geomCount; i++)
            {
                FacialBlendGeomBoneEntry geomBoneEntry = new FacialBlendGeomBoneEntry(input);
                this.geomBoneEntries.Add(geomBoneEntry);
                geomBoneEntry = null;
            }
            this.keytable.Load(input);

            reader = null;
        }

        public FacialBlend()
        {
        }

        public FacialBlend(Stream input)
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
            writer.Write((int)0);
            writer.Write((int)0);

            // Double the string length since it's UTF16
            writer.Write((byte)(this.partName.Length * 2));
            MadScience.StreamHelpers.WriteStringUTF16(output, false, this.partName);

            writer.Write(this.blendType);
            this.blendTgi.Save(output);

            writer.Write((uint)this.geomBoneEntries.Count);
            for (int i = 0; i < this.geomBoneEntries.Count; i++)
            {
                this.geomBoneEntries[i].Save(output);
            }

            uint tgiOffset = (uint)output.Position - 8;
            // Why is this +12?  I dunno. :)
            this.keytable.size = 12;
            this.keytable.Save(output);
            output.Seek(4, SeekOrigin.Begin);
            writer.Write(tgiOffset);
            writer.Write(this.keytable.size);

            writer = null;
        }

        public Stream Save()
        {
            Stream temp = new MemoryStream();
            Save(temp);
            return temp;
        }
    }

    public class FacialBlendGeomBoneEntry
    {
        public uint regionFlag = 0;
        public uint hasGeomAndBone = 0;
        public uint hasGeomEntry = 0;
        public uint ageGenderFlags = 0;
        public float amount = 0;
        public uint geomEntryIndex = 0;
        public uint hasBoneEntry = 0;
        public uint ageGenderFlags2 = 0;
        public float amount2 = 0;
        public uint boneIndex = 0;

        public FacialBlendGeomBoneEntry()
        {
        }

        public FacialBlendGeomBoneEntry(Stream input)
        {
            BinaryReader reader = new BinaryReader(input);
            this.regionFlag = reader.ReadUInt32();
            this.hasGeomAndBone = reader.ReadUInt32();
            if (this.hasGeomAndBone == 0)
            {
                this.hasGeomEntry = reader.ReadUInt32();
            }
            this.ageGenderFlags = reader.ReadUInt32();
            this.amount = reader.ReadSingle();

            this.geomEntryIndex = reader.ReadUInt32();
            if (this.hasGeomAndBone == 1)
            {
                this.hasBoneEntry = reader.ReadUInt32();
                if (this.hasBoneEntry == 1)
                {
                    this.ageGenderFlags2 = reader.ReadUInt32();
                    this.amount2 = reader.ReadSingle();
                    this.boneIndex = reader.ReadUInt32();
                }
            }
            reader = null;
        }

        public void Save(Stream output)
        {
            BinaryWriter writer = new BinaryWriter(output);
            writer.Write(this.regionFlag);
            writer.Write(this.hasGeomAndBone);
            if (this.hasGeomAndBone == 0)
            {
                writer.Write(this.hasGeomEntry);
            }
            writer.Write(this.ageGenderFlags);
            writer.Write(this.amount);
            writer.Write(this.geomEntryIndex);
            if (this.hasGeomAndBone == 1)
            {
                writer.Write(this.hasBoneEntry);
                if (this.hasBoneEntry == 1)
                {
                    writer.Write(this.ageGenderFlags2);
                    writer.Write(this.amount2);
                    writer.Write(this.boneIndex);
                }
            }
            writer = null;
        }
    }

}
