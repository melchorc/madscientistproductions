using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MadScience.Wrappers
{
	public class OBJD
	{
		public uint version = 0;
		public OffsetSize offSize = new OffsetSize();
		public CatalogResource.MaterialList materialList;
		public CatalogResource.Common common = new CatalogResource.Common();

		public uint unkDword1;
		public byte unkByte;
		public uint unkDword2;
		public byte unkByte2;
		public byte unkByte3;
		public uint unk4Byte;
		public uint index1;
		public uint unkDword3;
		public uint unkDword4;
		public uint unkDword5;
		public uint unkDword6;
		public uint unkDword7;		

		public byte count1 = 0;
		public List<WallMask> wallMasks = new List<WallMask>();
		public byte unkByte4 = 0;
		public uint index2 = 0;
		public uint hash;
		public uint roomFlags = 0;
		public uint functionCategoryFlags = 0;
		public ulong subCategoryFlags = 0;
		public ulong subRoomFlags = 0;
		public uint buildCategoryFlags = 0;
		public uint index3 = 0;
		public uint unkDword = 0;
		public string materialGroup1 = "";
		public string materialGroup2 = "";

		public override string ToString()
		{
			string retString = this.common.catalogName + " - " + this.common.catalogDesc;
			return retString;
		}

		public struct WallMask
		{
			public float float1;
			public float float2;
			public float float3;
			public float float4;
			public uint dword1;
			public uint dword2;

		}

		public OBJD()
		{
		}

		public OBJD(Stream input)
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

			this.version = StreamHelpers.ReadValueU32(input);
			this.offSize.Load(input);
			this.materialList = new CatalogResource.MaterialList();
			this.materialList.typeId = 0x319E4F1D;
			this.materialList.Load(input);
			if (this.version >= 0x16)
			{
				reader.ReadString();
			}
			this.common.Load(input);

			this.unkDword1 = StreamHelpers.ReadValueU32(input);
			this.unkByte = StreamHelpers.ReadValueU8(input);
			this.unkDword2 = StreamHelpers.ReadValueU32(input);
			this.unkByte2 = StreamHelpers.ReadValueU8(input);
			this.unkByte3 = StreamHelpers.ReadValueU8(input);
			this.unk4Byte = StreamHelpers.ReadValueU32(input);
			this.index1 = StreamHelpers.ReadValueU32(input);
			this.unkDword3 = StreamHelpers.ReadValueU32(input);
			this.unkDword4 = StreamHelpers.ReadValueU32(input);
			this.unkDword5 = StreamHelpers.ReadValueU32(input);
			this.unkDword6 = StreamHelpers.ReadValueU32(input);
			this.unkDword7 = StreamHelpers.ReadValueU32(input);

			this.count1 = StreamHelpers.ReadValueU8(input);
			for (int i = 0; i < count1; i++)
			{
				WallMask wm = new WallMask();
				wm = StreamHelpers.ReadStructure<WallMask>(input);
				this.wallMasks.Add(wm);
			}
			this.unkByte = StreamHelpers.ReadValueU8(input);
			this.index2 = StreamHelpers.ReadValueU32(input);
			this.hash = StreamHelpers.ReadValueU32(input);
			this.roomFlags = StreamHelpers.ReadValueU32(input);
			this.functionCategoryFlags = StreamHelpers.ReadValueU32(input);
			this.subCategoryFlags = StreamHelpers.ReadValueU64(input);
			this.subRoomFlags = StreamHelpers.ReadValueU64(input);
			this.buildCategoryFlags = StreamHelpers.ReadValueU32(input);
			this.index3 = StreamHelpers.ReadValueU32(input);
			this.unkDword = StreamHelpers.ReadValueU32(input);
			this.materialGroup1 = MadScience.Helpers.stripControlFromString(reader.ReadString());
			this.materialGroup2 = MadScience.Helpers.stripControlFromString(reader.ReadString());

		}

		public void Save(Stream output)
		{

		}

		public Stream Save()
		{
			Stream temp = new MemoryStream();
			Save(temp);
			return temp;
		}


	}

}
