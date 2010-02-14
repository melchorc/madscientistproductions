using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MadScience.Wrappers
{
	public class CatalogResource
	{
		public class Common
		{
			public uint version = 0;
			public ulong nameGuid = 0;
			public ulong descGuid = 0;
			public string catalogName = "";
			public string catalogDesc = "";
			public float price = 0.0f;
			public float unkFloat = 1;
			public uint unk4Word = 0;
			public byte unkByte;
			public ulong pngIcon = 0;
			public byte unkByte2;

			public Common()
			{
			}

			public Common(Stream input)
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
				this.nameGuid = reader.ReadUInt64();
				this.descGuid = reader.ReadUInt64();
				this.catalogName = MadScience.Helpers.stripControlFromString(reader.ReadString()).Replace("CatalogObjects/Name:", "");
				this.catalogDesc = MadScience.Helpers.stripControlFromString(reader.ReadString()).Replace("CatalogObjects/Description:", "");
				this.price = reader.ReadSingle();
				this.unkFloat = reader.ReadSingle();
				this.unk4Word = reader.ReadUInt32();
				this.unkByte = reader.ReadByte();
				this.pngIcon = reader.ReadUInt64();
				if (this.version >= 0x0D)
				{
					this.unkByte2 = reader.ReadByte();
				}
				reader = null;
			}
		}

		public class MaterialList
		{
			public uint count1 = 0;
			public uint typeId = 0;
			public List<Entry> entries = new List<Entry>();

			public MaterialList()
			{
			}

			public MaterialList(Stream input)
			{
				loadFromStream(input);
			}

			public void Load(Stream input)
			{
				loadFromStream(input);
			}

			private void loadFromStream(Stream input)
			{
				this.count1 = StreamHelpers.ReadValueU32(input);
				for (int i = 0; i < count1; i++)
				{
					Entry e = new Entry(this.typeId);
					e.Load(input);
					this.entries.Add(e);
				}

			}

			public class Entry
			{
				private uint typeId;

				public byte entryType;
				public uint unkDword;
				public uint offset;
				public int unkInt;
				public OffsetSize offSize = new OffsetSize();
				public MaterialBlock materialBlock = new MaterialBlock();
				public KeyTable keyTable = new KeyTable();
				public uint unkDword2;
				public uint wallDword1;
				public uint wallDword2;
				public uint wallDword3;

				public Entry(uint typeId)
				{
					this.typeId = typeId;
				}

				public Entry(Stream input)
				{
					loadFromStream(input);
				}

				public void Load(Stream input)
				{
					loadFromStream(input);
				}

				private void loadFromStream(Stream input)
				{
					this.entryType = StreamHelpers.ReadValueU8(input);
					if (this.entryType != 1) this.unkDword = StreamHelpers.ReadValueU32(input);
					this.offset = StreamHelpers.ReadValueU32(input);
					//this.unkInt = StreamHelpers.ReadValueU16(input);
					//this.offSize.Load(input);
					// Skip the materialBlock for now
					//this.materialBlock.Load(input);

					input.Seek(this.offset, SeekOrigin.Current);
					this.unkDword2 = StreamHelpers.ReadValueU32(input);
					if (this.typeId == 0x515CA4CD)
					{
						this.wallDword1 = StreamHelpers.ReadValueU32(input);
						this.wallDword2 = StreamHelpers.ReadValueU32(input);
						this.wallDword3 = StreamHelpers.ReadValueU32(input);
					}

				}

			}

		}

		public class MaterialBlock
		{
			public byte index;
			public uint count1;

			public MaterialBlock()
			{
			}

			public MaterialBlock(Stream input)
			{
				loadFromStream(input);
			}

			public void Load(Stream input)
			{
				loadFromStream(input);
			}

			private void loadFromStream(Stream input)
			{
			}
		}

	}
}
