using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;

namespace MadScience.Wrappers
{

	public struct RIGChunkInt1
	{
		public uint unk1;
		public ulong unk2;
	}

	public class RIGChunk
	{
		private ulong magic1 = 0xB867B0CAF86DB10F;
		private ulong magic2 = 0x84728C7E5E19001E;
		public uint headerSize = 0;
		public uint blank_1 = 0; // Blank
		public uint blank_2 = 0; // Blank
		public uint blank_3 = 0; // Blank

		public uint version = 6;

		public uint dataSize = 0;
		public uint crc32 = 0;
		public uint unknown3 = 0;

		public List<uint> unkList = new List<uint>(); // 10 values

		public uint unknown4 = 2; // Always 2?
		public uint scriptOffset = 0;
		public uint scriptSize = 0;
		
		// 4 more values... add to unkList

		public uint int1Offset = 0;
		public uint int1Count = 0;
		public uint int1EndOffset = 0;

		public uint unknown5 = 0;

		public uint int1unk1 = 0;
		public uint int1unk2 = 0;

		public List<RIGChunkInt1> int1 = new List<RIGChunkInt1>();

		public uint unknown6 = 0;

		public RIGChunk()
		{
		}

		public RIGChunk(Stream input)
		{
			loadFromStream(input);
		}

		public void Load(Stream input)
		{
			loadFromStream(input);
		}

		private void loadFromStream(Stream input)
		{
			this.magic1 = StreamHelpers.ReadValueU64(input);
			this.magic2 = StreamHelpers.ReadValueU64(input);

			this.headerSize = StreamHelpers.ReadValueU32(input);
			this.blank_1 = StreamHelpers.ReadValueU32(input); // Blank
			this.blank_2 = StreamHelpers.ReadValueU32(input); // Blank
			this.blank_3 = StreamHelpers.ReadValueU32(input); // Blank

			this.version = StreamHelpers.ReadValueU32(input);

			this.dataSize = StreamHelpers.ReadValueU32(input);
			this.crc32 = StreamHelpers.ReadValueU32(input);
			this.unknown3 = StreamHelpers.ReadValueU32(input);

			for (int i = 0; i < 10; i++)
			{
				this.unkList.Add(StreamHelpers.ReadValueU32(input));
			}

			this.unknown4 = StreamHelpers.ReadValueU32(input); // Always 2?
			this.scriptOffset = StreamHelpers.ReadValueU32(input);
			this.scriptSize = StreamHelpers.ReadValueU32(input);

			// 4 more values... add to unkList
			for (int i = 0; i < 4; i++)
			{
				this.unkList.Add(StreamHelpers.ReadValueU32(input));
			}

			this.int1Offset = StreamHelpers.ReadValueU32(input);
			this.int1Count = StreamHelpers.ReadValueU32(input);
			this.int1EndOffset = StreamHelpers.ReadValueU32(input);

			// Knock one off int1Count
			this.int1Count--;

			this.unknown5 = StreamHelpers.ReadValueU32(input);

			// Int 1 seems to start with 1 based, not 0?
			// Always 4 then 0
			this.int1unk1 = StreamHelpers.ReadValueU32(input);
			this.int1unk2 = StreamHelpers.ReadValueU32(input);

			for (int i = 0; i < this.int1Count; i++)
			{
				RIGChunkInt1 rChunk = new RIGChunkInt1();
				rChunk.unk1 = StreamHelpers.ReadValueU32(input);
				rChunk.unk2 = StreamHelpers.ReadValueU64(input);
				this.int1.Add(rChunk);
			}

			// Extra value
			this.unknown6 = StreamHelpers.ReadValueU32(input);

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
