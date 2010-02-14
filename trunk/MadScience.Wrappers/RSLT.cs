using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;

namespace MadScience.Wrappers
{

	// Create a new RSLT file with associated RCOL header
	public class RSLTFile
	{
		public RcolHeader rcolHeader = new RcolHeader();
		public RSLT rslt = new RSLT();

		public RSLTFile()
		{
		}

		public RSLTFile(Stream input)
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
			this.rslt.Load(input);
		}

		public void Save(Stream output)
		{
			if (this.rcolHeader.chunks.Count == 0)
			{
				this.rcolHeader.chunks.Add(new MadScience.Wrappers.OffsetSize());
			}
			if (this.rcolHeader.externalChunks.Count == 0 && this.rcolHeader.internalChunks.Count == 0)
			{
				//throw new Exception("You must have a valid RCOL header");
				return;
			}
			rcolHeader.Save(output);
			rslt.Save(output);

			rcolHeader.chunks[0] = rslt.offsize;

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

	public class RSLT
	{
		private string magic = "RSLT";
		public uint version = 2;

		public List<RSLTEntry> Routes = new List<RSLTEntry>();
		public List<RSLTEntry> Containers = new List<RSLTEntry>();
		public List<RSLTEntry> Effects = new List<RSLTEntry>();
		public List<RSLTEntry> IKTargets = new List<RSLTEntry>();

		public uint count5 = 0;

		public MadScience.Wrappers.OffsetSize offsize = new MadScience.Wrappers.OffsetSize();

		public RSLT()
		{
		}

		public RSLT(Stream input)
		{
			loadFromStream(input);
		}

		public void Load(Stream input)
		{
			loadFromStream(input);
		}

		private void loadFromStream(Stream input)
		{
			this.magic = StreamHelpers.ReadStringASCII(input, 4);
			this.version = StreamHelpers.ReadValueU32(input);

			uint count1 = StreamHelpers.ReadValueU32(input);
			uint count2 = StreamHelpers.ReadValueU32(input);
			uint count3 = StreamHelpers.ReadValueU32(input);
			uint count4 = StreamHelpers.ReadValueU32(input);
			this.count5 = StreamHelpers.ReadValueU32(input);

			// Routes
			for (int i = 0; i < count1; i++)
			{
				this.Routes.Add(new RSLTEntry());
				this.Routes[i].slotName = StreamHelpers.ReadValueU32(input);
			}
			for (int i = 0; i < count1; i++)
			{
				this.Routes[i].boneName = StreamHelpers.ReadValueU32(input);
			}
			for (int i = 0; i < count1; i++)
			{
				this.Routes[i].matrix = new Matrix4by3(input);
			}
			if (count1 > 0) StreamHelpers.ReadValueU32(input);

			// Containers
			for (int i = 0; i < count2; i++)
			{
				this.Containers.Add(new RSLTEntry());
				this.Containers[i].slotName = StreamHelpers.ReadValueU32(input);
			}
			for (int i = 0; i < count2; i++)
			{
				this.Containers[i].boneName = StreamHelpers.ReadValueU32(input);
			}
			for (int i = 0; i < count2; i++)
			{
				this.Containers[i].flags = StreamHelpers.ReadValueU32(input);
			}
			for (int i = 0; i < count2; i++)
			{
				this.Containers[i].matrix = new Matrix4by3(input);
			}
			if (count2 > 0) StreamHelpers.ReadValueU32(input);

			// Effects
			for (int i = 0; i < count3; i++)
			{
				this.Effects.Add(new RSLTEntry());
				this.Effects[i].slotName = StreamHelpers.ReadValueU32(input);
			}
			for (int i = 0; i < count3; i++)
			{
				this.Effects[i].boneName = StreamHelpers.ReadValueU32(input);
			}
			for (int i = 0; i < count3; i++)
			{
				this.Effects[i].matrix = new Matrix4by3(input);
			}
			if (count3 > 0) StreamHelpers.ReadValueU32(input);

			// IK Targets
			for (int i = 0; i < count4; i++)
			{
				this.IKTargets.Add(new RSLTEntry());
				this.IKTargets[i].slotName = StreamHelpers.ReadValueU32(input);
			}
			for (int i = 0; i < count4; i++)
			{
				this.IKTargets[i].boneName = StreamHelpers.ReadValueU32(input);
			}
			for (int i = 0; i < count4; i++)
			{
				this.IKTargets[i].matrix = new Matrix4by3(input);
			}
			if (count4 > 0) StreamHelpers.ReadValueU32(input);

		}

		public void Save(Stream output)
		{
			long startOfChunk = output.Position;
			this.offsize.offset = (uint)startOfChunk;

			StreamHelpers.WriteStringASCII(output, this.magic);
			StreamHelpers.WriteValueU32(output, this.version);

			StreamHelpers.WriteValueU32(output, (uint)this.Routes.Count);
			StreamHelpers.WriteValueU32(output, (uint)this.Containers.Count);
			StreamHelpers.WriteValueU32(output, (uint)this.Effects.Count);
			StreamHelpers.WriteValueU32(output, (uint)this.IKTargets.Count);
			StreamHelpers.WriteValueU32(output, this.count5);

			for (int i = 0; i < this.Routes.Count; i++)
			{
				StreamHelpers.WriteValueU32(output, this.Routes[i].slotName);
			}
			for (int i = 0; i < this.Routes.Count; i++)
			{
				StreamHelpers.WriteValueU32(output, this.Routes[i].boneName);
			}
			for (int i = 0; i < this.Routes.Count; i++)
			{
				this.Routes[i].matrix.Save(output);
			}
			if (this.Routes.Count > 0) StreamHelpers.WriteValueU32(output, 0);

			// Containers
			for (int i = 0; i < this.Containers.Count; i++)
			{
				StreamHelpers.WriteValueU32(output, this.Containers[i].slotName);
			}
			for (int i = 0; i < this.Containers.Count; i++)
			{
				StreamHelpers.WriteValueU32(output, this.Containers[i].boneName);
			}
			for (int i = 0; i < this.Containers.Count; i++)
			{
				StreamHelpers.WriteValueU32(output, this.Containers[i].flags);
			}
			for (int i = 0; i < this.Containers.Count; i++)
			{
				this.Containers[i].matrix.Save(output);
			}
			if (this.Containers.Count > 0) StreamHelpers.WriteValueU32(output, 0);

			// Effects
			for (int i = 0; i < this.Effects.Count; i++)
			{
				StreamHelpers.WriteValueU32(output, this.Effects[i].slotName);
			}
			for (int i = 0; i < this.Effects.Count; i++)
			{
				StreamHelpers.WriteValueU32(output, this.Effects[i].boneName);
			}
			for (int i = 0; i < this.Effects.Count; i++)
			{
				this.Effects[i].matrix.Save(output);
			}
			if (this.Effects.Count > 0) StreamHelpers.WriteValueU32(output, 0);

			// IK Targets
			for (int i = 0; i < this.IKTargets.Count; i++)
			{
				StreamHelpers.WriteValueU32(output, this.IKTargets[i].slotName);
			}
			for (int i = 0; i < this.IKTargets.Count; i++)
			{
				StreamHelpers.WriteValueU32(output, this.IKTargets[i].boneName);
			}
			for (int i = 0; i < this.IKTargets.Count; i++)
			{
				this.IKTargets[i].matrix.Save(output);
			}
			if (this.IKTargets.Count > 0) StreamHelpers.WriteValueU32(output, 0);

			long chunkSize = output.Position - startOfChunk;
			this.offsize.size = (uint)chunkSize;

		}

		public Stream Save()
		{
			Stream temp = new MemoryStream();
			Save(temp);
			return temp;
		}


	}

	public class Matrix4by3
	{
		public float rc11 = 0f;
		public float rc12 = 0f;
		public float rc13 = 0f;
		public float rc14 = 0f;
		public float rc21 = 0f;
		public float rc22 = 0f;
		public float rc23 = 0f;
		public float rc24 = 0f;
		public float rc31 = 0f;
		public float rc32 = 0f;
		public float rc33 = 0f;
		public float rc34 = 0f;

		public Matrix4by3()
		{
		}

		public Matrix4by3(Stream input)
		{
			LoadFromStream(input);
		}

		public void Load(Stream input)
		{
			LoadFromStream(input);
		}

		private void LoadFromStream(Stream input)
		{
			this.rc11 = StreamHelpers.ReadValueF32(input);
			this.rc12 = StreamHelpers.ReadValueF32(input);
			this.rc13 = StreamHelpers.ReadValueF32(input);
			this.rc14 = StreamHelpers.ReadValueF32(input);
			this.rc21 = StreamHelpers.ReadValueF32(input);
			this.rc22 = StreamHelpers.ReadValueF32(input);
			this.rc23 = StreamHelpers.ReadValueF32(input);
			this.rc24 = StreamHelpers.ReadValueF32(input);
			this.rc31 = StreamHelpers.ReadValueF32(input);
			this.rc32 = StreamHelpers.ReadValueF32(input);
			this.rc33 = StreamHelpers.ReadValueF32(input);
			this.rc34 = StreamHelpers.ReadValueF32(input);
		}

		public void Save(Stream output)
		{
			StreamHelpers.WriteValueF32(output, this.rc11);
			StreamHelpers.WriteValueF32(output, this.rc12);
			StreamHelpers.WriteValueF32(output, this.rc13);
			StreamHelpers.WriteValueF32(output, this.rc14);
			StreamHelpers.WriteValueF32(output, this.rc21);
			StreamHelpers.WriteValueF32(output, this.rc22);
			StreamHelpers.WriteValueF32(output, this.rc23);
			StreamHelpers.WriteValueF32(output, this.rc24);
			StreamHelpers.WriteValueF32(output, this.rc31);
			StreamHelpers.WriteValueF32(output, this.rc32);
			StreamHelpers.WriteValueF32(output, this.rc33);
			StreamHelpers.WriteValueF32(output, this.rc34);

		}

		public Stream Save()
		{
			Stream temp = new MemoryStream();
			Save(temp);
			return temp;
		}

	}

	public class RSLTEntry
	{
		public uint slotName = 0;
		public uint boneName = 0;
		public uint flags = 0;  // Flags is only used for Containers
		public MadScience.Wrappers.Matrix4by3 matrix = new MadScience.Wrappers.Matrix4by3();
	}

}
