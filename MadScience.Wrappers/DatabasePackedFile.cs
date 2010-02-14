using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
//using Gibbed.Helpers;

namespace MadScience.Wrappers
{
    public class DatabasePackedFile
	{
        public struct Entry
        {
            public ResourceKey Key;
            #region public bool Compressed;
            public bool Compressed
            {
                get
                {
                    return this.CompressionFlags == -1;
                }
            }
            #endregion

            public Int64 Offset;
            public uint CompressedSize;
            public uint DecompressedSize;
            public short CompressionFlags;
            public ushort Flags;
        }

		public Header smallHeader;
		public BigHeader bigHeader;

        public bool Big;
		public Version Version = new Version();
        public List<Entry> Entries = new List<Entry>();
        public long IndexOffset;
        public int IndexType;
        public long EndOfDataOffset;
		public string magic;

		public Detective.PackageTypes packageType = Detective.PackageTypes.genericPackage;

		public void Read(Stream input)
		{
			Read(input, true);
		}

		public void Read(Stream input, bool throwError)
		{
			Int64 indexCount;
			Int64 indexSize;
			Int64 indexOffset;

			bool hasError = false;

			this.magic = MadScience.StreamHelpers.ReadStringASCII(input, 4);
			if (this.magic != "DBPF" && this.magic != "DBBF") // DBPF & DBBF
			{
				if (this.magic == "DBPP")
				{
					if (throwError)
					{
						throw new MadScience.Exceptions.UnsupportedPackageVersionException("protected store content");
					}
					else
					{
						// We don't do anything with Store files, so just return
						this.packageType = Detective.PackageTypes.sims3Store;
						return;
					}
				}
				else
				{
					if (magic.EndsWith("PNG"))
					{
						this.packageType = Detective.PackageTypes.pngThumbnail;
						return;
					}

					if (throwError)
					{
						throw new MadScience.Exceptions.NotAPackageException("not a valid package file");
					}
					else
					{
						hasError = true;
					}
				}
			}

			// If we have an error already then it's not a DBPF file, so no point in going on
			if (hasError && !throwError)
			{
				this.packageType = Detective.PackageTypes.corruptNotADBPF;
				return;
			}

			this.Big = this.magic == "DBBF";

            if (this.Big == true)
			{
                BigHeader header = MadScience.StreamHelpers.ReadStructure<BigHeader>(input);
				this.bigHeader = header;

				// Nab useful stuff
				this.Version = new Version(header.MajorVersion, header.MinorVersion);
				indexCount = header.IndexCount;
				indexOffset = header.IndexOffset;
				indexSize = header.IndexSize;

				if (header.MajorVersion == 2 && header.MinorVersion == 0)
				{
					if (header.IndexVersion < 2)
					{
						if (throwError)
						{
							throw new MadScience.Exceptions.UnsupportedPackageVersionException("index version was not 2 or 3");
						}
						else
						{
							hasError = true;
						}
					}
				}
				else
				{
					if (throwError)
					{
						throw new MadScience.Exceptions.UnsupportedPackageVersionException("package version was not 2.0 - possibly TS2 content?");
					}
					else
					{
						hasError = true;
					}
				}

			}
			else
			{
                Header header = MadScience.StreamHelpers.ReadStructure<Header>(input);
				this.smallHeader = header;

				// Nab useful stuff
				this.Version = new Version(header.MajorVersion, header.MinorVersion);
				indexCount = header.IndexCount;
				indexOffset = header.IndexOffset;
				indexSize = header.IndexSize;

				if (header.MajorVersion == 2 && header.MinorVersion == 0)
				{
					if (header.IndexVersion < 2)
					{
						if (throwError)
						{
							throw new MadScience.Exceptions.DatabasePackedFileException("index version was not 2 or 3");
						}
						else
						{
							hasError = true;
						}
					}
				}
				else
				{
					if (throwError)
					{
						throw new MadScience.Exceptions.DatabasePackedFileException("package version was not 2.0 - possibly TS2 content?");
					}
					else
					{
						this.packageType = Detective.PackageTypes.sims2Package;
						hasError = true;
					}
				}

			}

			this.IndexOffset = indexOffset;

			// Check for invalid indexOffsets
			if (indexOffset > input.Length)
			{
				// This is a corrupted file.  We can do nothing.
				this.packageType = Detective.PackageTypes.corruptBadDownload;
				return;
			}

            this.Entries.Clear();

			// Do some stuff for TS2
			if (this.Version.Major == 1)
			{
				if (this.IndexOffset == 0) 
				{
					this.IndexOffset = this.smallHeader.IndexOffsetTS2;
				}
			}

			if (this.Version.Major == 538976258)
			{
				// This is seen in some of Chaavik's packages - spaces in the header.  
				// We can do nothing with these, so just give up
				this.packageType = Detective.PackageTypes.corruptChaavik;
				return;
			}

			// If we get this far, DONT have throw errors on, and have an error, then just plain exit.
			//if (!throwError && hasError) return;

			if (indexCount > 0)
			{
				// Read index
				int presentPackageValues = 0;

				bool hasPackageTypeId = false;
				bool hasPackageGroupId = false;
				bool hasPackageHiInstanceId = false;

				uint packageTypeId = 0;
				uint packageGroupId = 0;
				uint packageHiInstanceId = 0;

				input.Seek(this.IndexOffset, SeekOrigin.Begin);

			// Do some stuff for TS2
				if (this.Version.Major == 2)
				{

					presentPackageValues = MadScience.StreamHelpers.ReadValueS32(input);
					this.IndexType = presentPackageValues;
					if ((presentPackageValues & ~7) != 0)
					{
						this.packageType = Detective.PackageTypes.corruptIndex;

						// Check backwards 3 bytes (some files from peggy have invalid 
						input.Seek(this.IndexOffset - 3, SeekOrigin.Begin);
						presentPackageValues = MadScience.StreamHelpers.ReadValueS32(input);
						this.IndexType = presentPackageValues;
						if ((presentPackageValues & ~7) != 0)
						{

							if (throwError)
							{
								throw new InvalidDataException("don't know how to handle this index data");
							}
							else
							{
								this.packageType = Detective.PackageTypes.corruptPeggy;
								return;
							}
						}
					}

					hasPackageTypeId = (presentPackageValues & (1 << 0)) == 1 << 0;
					hasPackageGroupId = (presentPackageValues & (1 << 1)) == 1 << 1;
					hasPackageHiInstanceId = (presentPackageValues & (1 << 2)) == 1 << 2;

					packageTypeId = hasPackageTypeId ? MadScience.StreamHelpers.ReadValueU32(input) : 0xFFFFFFFF;
					packageGroupId = hasPackageGroupId ? MadScience.StreamHelpers.ReadValueU32(input) : 0xFFFFFFFF;
					packageHiInstanceId = hasPackageHiInstanceId ? MadScience.StreamHelpers.ReadValueU32(input) : 0xFFFFFFFF;
				} 

				for (int i = 0; i < indexCount; i++)
				{
                    Entry entry = new Entry();
                    entry.Key = new ResourceKey();

                    if (hasPackageTypeId) entry.Key.typeId = packageTypeId;
                    else entry.Key.typeId = MadScience.StreamHelpers.ReadValueU32(input);
                    if (hasPackageGroupId) entry.Key.groupId = packageGroupId;
                    else entry.Key.groupId =  MadScience.StreamHelpers.ReadValueU32(input);
                    entry.Key.instanceId = 0;
                    entry.Key.instanceId |= (hasPackageHiInstanceId ? packageHiInstanceId : MadScience.StreamHelpers.ReadValueU32(input));
                    entry.Key.instanceId <<= 32;
                    entry.Key.instanceId |= MadScience.StreamHelpers.ReadValueU32(input);

                    entry.Offset = (this.Big == true) ? MadScience.StreamHelpers.ReadValueS64(input) : MadScience.StreamHelpers.ReadValueS32(input);
                    entry.CompressedSize = MadScience.StreamHelpers.ReadValueU32(input);

					if (this.Version.Major == 2)
					{
						entry.DecompressedSize = MadScience.StreamHelpers.ReadValueU32(input);

						// compressed bit
						if ((entry.CompressedSize & 0x80000000) == 0x80000000)
						{
							entry.CompressedSize &= ~0x80000000;
							entry.CompressionFlags = MadScience.StreamHelpers.ReadValueS16(input);
							entry.Flags = MadScience.StreamHelpers.ReadValueU16(input);
						}
						else
						{
							if (entry.CompressedSize != entry.DecompressedSize)
							{
								entry.CompressionFlags = -1;
							}
							else
							{
								entry.CompressionFlags = 0;
							}

							entry.Flags = 0;

							if (throwError)
							{
								throw new MadScience.Exceptions.DatabasePackedFileException("strange index data");
							}
							else
							{
								this.packageType = Detective.PackageTypes.corruptIndex;
								return;
							}
						}

						if (entry.CompressionFlags != 0 && entry.CompressionFlags != -1)
						{
							throw new MadScience.Exceptions.DatabasePackedFileException("bad compression flags");
						}
					}
                    if (entry.Offset + entry.CompressedSize > this.EndOfDataOffset)
                    {
                        this.EndOfDataOffset = entry.Offset + entry.CompressedSize;
                    }

					this.Entries.Add(entry);
				}
			}
		}

        public void WriteHeader(Stream output, long indexOffset, long indexSize)
		{
            if (this.Big == true)
            {
                MadScience.StreamHelpers.WriteStringASCII(output, "DBBF");
                BigHeader header = new BigHeader();
                header.MajorVersion = this.Version.Major;
                header.MinorVersion = this.Version.Minor;
                header.IndexVersion = 3;
                header.IndexCount = this.Entries.Count;
                header.IndexOffset = indexOffset;
                header.IndexSize = indexSize;
                MadScience.StreamHelpers.WriteStructure<BigHeader>(output, header);
            }
            else
            {
                MadScience.StreamHelpers.WriteStringASCII(output, "DBPF");
                Header header = new Header();
                header.MajorVersion = this.Version.Major;
                header.MinorVersion = this.Version.Minor;
                header.IndexVersion = 3;
                header.IndexCount = this.Entries.Count;
                header.IndexOffset = (int)indexOffset;
                header.IndexSize = (int)indexSize;
                MadScience.StreamHelpers.WriteStructure<Header>(output, header);
            }
		}

		public void WriteIndex(Stream output)
		{
            if (this.Entries.Count == 0)
            {
                MadScience.StreamHelpers.WriteValueU32(output, 0); // present package values
            }
            else
            {
                bool hasPackageTypeId = true;
                bool hasPackageGroupId = true;
                bool hasPackageHiInstanceId = true;

                uint packageTypeId = this.Entries[0].Key.typeId;
                uint packageGroupId = this.Entries[0].Key.groupId;
                uint packageHiInstanceId = (uint)(this.Entries[0].Key.instanceId >> 32);

                for (int i = 1; i < this.Entries.Count; i++)
                {
                    hasPackageTypeId = hasPackageTypeId && (packageTypeId == this.Entries[i].Key.typeId);
                    hasPackageGroupId = hasPackageGroupId && (packageGroupId == this.Entries[i].Key.groupId);
                    hasPackageHiInstanceId = hasPackageHiInstanceId && (packageHiInstanceId == (uint)(this.Entries[i].Key.instanceId >> 32));

                    if (hasPackageTypeId == false && hasPackageGroupId == false && hasPackageHiInstanceId == false)
                    {
                        break;
                    }
                }

                int presentPackageValues = 0;
                presentPackageValues |= (hasPackageTypeId ? 1 : 0) << 0;
                presentPackageValues |= (hasPackageGroupId ? 1 : 0) << 1;
                presentPackageValues |= (hasPackageHiInstanceId ? 1 : 0) << 2;

                MadScience.StreamHelpers.WriteValueS32(output, presentPackageValues);

                if (hasPackageTypeId == true)
                {
                    MadScience.StreamHelpers.WriteValueU32(output, packageTypeId);
                }

                if (hasPackageGroupId == true)
                {
                    MadScience.StreamHelpers.WriteValueU32(output, packageGroupId);
                }

                if (hasPackageHiInstanceId == true)
                {
                    MadScience.StreamHelpers.WriteValueU32(output, packageHiInstanceId);
                }

                foreach (Entry entry in this.Entries)
                {
                    if (hasPackageTypeId == false)
                    {
                        MadScience.StreamHelpers.WriteValueU32(output, entry.Key.typeId);
                    }

                    if (hasPackageGroupId == false)
                    {
                        MadScience.StreamHelpers.WriteValueU32(output, entry.Key.groupId);
                    }

                    if (hasPackageHiInstanceId == false)
                    {
                        MadScience.StreamHelpers.WriteValueU32(output, (UInt32)(entry.Key.instanceId >> 32));
                    }

                    MadScience.StreamHelpers.WriteValueU32(output, (UInt32)(entry.Key.instanceId & 0xFFFFFFFF));

                    if (this.Big == true)
                    {
                        MadScience.StreamHelpers.WriteValueS64(output, entry.Offset);
                    }
                    else
                    {
                        MadScience.StreamHelpers.WriteValueS32(output, (int)entry.Offset);
                    }

                    MadScience.StreamHelpers.WriteValueU32(output, entry.CompressedSize);
                    MadScience.StreamHelpers.WriteValueU32(output, entry.DecompressedSize);
                    MadScience.StreamHelpers.WriteValueS16(output, entry.CompressionFlags);
                    MadScience.StreamHelpers.WriteValueU16(output, entry.Flags);
                }
            }
		}

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Header
        {
            //public uint Magic;		// 00
            public int MajorVersion;	// 04
            public int MinorVersion;	// 08
            public uint Unknown0C;		// 0C
            public uint Unknown10;		// 10
            public uint Unknown14;		// 14 - always 0?
			public uint DateCreated;		// 18 - For Sims 2
			public uint DateModified;		// 1C - For Sims 2
			public uint IndexVersionMajor;		// 20 - For Sims 2
            public int IndexCount;		// 24 - Number of index entries in the package.
            public uint IndexOffsetTS2;		// 28 - Used to be index Offset in The Sims 2
            public int IndexSize;		// 2C - The total size in bytes of index entries.
            public uint HolesCount;		// 30 - Holes Count - TS2
            public uint HolesOffset;		// 34 - Holes Offset - TS2
            public uint HolesSize;		// 38 - Holes Size - TS2
            public uint IndexVersion;	// 3C - Always 3? (Listed as MinorVersion for TS2)
            public int IndexOffset;		// 40 - Absolute offset in package to the index header. (TS3)
            public uint Unknown44;		// 44
            public uint Unknown48;		// 48
            public uint Unknown4C;		// 4C
            public uint Unknown50;		// 50
            public uint Unknown54;		// 54
            public uint Unknown58;		// 58
            public uint Unknown5C;		// 5C
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct BigHeader
        {
            //public uint Magic;		// 00
            public int MajorVersion;	// 04
            public int MinorVersion;	// 08
            public uint Unknown0C;		// 0C
            public uint Unknown10;		// 10
            public uint Unknown14;		// 14 - always 0?
            public uint dateCreated;		// 18 - For Sims 2
            public uint dateModified;		// 1C - For Sims 2
            public uint IndexMajor;		// 20 - For Sims 2
            public int IndexCount;		// 24 - Number of index entries in the package.
            public Int64 IndexSize;		// 28 - The total size in bytes of index entries.
            public uint Unknown30;		// 30
            public uint IndexVersion;	// 34 - Always 3?
            public Int64 IndexOffset;	// 38 - Absolute offset in package to the index header.
            public uint Unknown40;		// 40
            public uint Unknown44;		// 44
            public uint Unknown48;		// 48
            public uint Unknown4C;		// 4C
            public uint Unknown50;		// 50
            public uint Unknown54;		// 54
            public uint Unknown58;		// 58
            public uint Unknown5C;		// 5C
            public uint Unknown60;		// 60
            public uint Unknown64;		// 64
            public uint Unknown68;		// 68
            public uint Unknown6C;		// 6C
            public uint Unknown70;		// 70
            public uint Unknown74;		// 74
        }
	}
}
