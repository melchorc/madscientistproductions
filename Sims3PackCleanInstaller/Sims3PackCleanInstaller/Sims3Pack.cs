using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace MadScience
{
    public class Sims3Pack
    {

		public class Sims3PackFile
		{
			public ushort UnknownInt = 0;
			public string Type = "";
			public string SubType = "0x00000000";
			public string ArchiveVersion = "1.4";
			public string CodeVersion = "0.2.0.76";
			public string GameVersion = "";
			public string DisplayName = "";
			public string Description = "";
			public string PackageId = "";
			public string Date = "";
			public int AssetVersion = -1;
			public string MinReqVersion = "";
			public List<string> Dependencies = new List<string>();
			public List<LocalisedString> LocalizedNames = new List<LocalisedString>();
			public List<LocalisedString> LocalizedDescriptions = new List<LocalisedString>();
			public List<PackagedFile> PackagedFiles = new List<PackagedFile>();

			public string rawXml = "";

			public class PackagedFile
			{
				public bool isDisabled = false;
				public string Name = "";
				public int Length = -1;
				public int Offset = -1;
				public string Crc = "";
				public string Guid = "";
				public string ContentType = "";
				public string Thumbnail = "";
				public string EPFlags = "";
				public MetaTag MetaTags = new MetaTag();
				public MemoryStream DBPF = new MemoryStream();

				public class MetaTag
				{
					public string name = "";
					public string description = "";
					public string bio = "";
					public int numOfThumbs = -999999;
					// Lots
					public int worldNoLots = -999999;
					public int maxLevel = -999999;
					public int minLevel = -999999;
					public int dimX = -999999;
					public int dimZ = -999999;
					public int lottype = -999999;
					public int numLotThumbs = -999999;
					public List<string> lotThumb = new List<string>();
					public string lotName = "";
					public int lotResSubType = -999999;
					public int lotComSubType = -999999;
					// Sims
					public string age = "";
					public string species = "";
					public string gender = "";
					public List<string> traits = new List<string>();
					public string fitness = "";
					public string weight = "";
					public string favcolor = "";
					public int favmusic = -999999;
					public int favfood = -999999;
					public string skintoneindex = "";
					public string handedness = "";
					public Outfits outfits = new Outfits();

					public class Outfits
					{
						public int numOutfits = 0;
						public string athletic = "";
						public string everyday = "";
						public string formalwear = "";
						public string naked = "";
						public string sleepwear = "";
						public string swimwear = "";
					}


				}
			}

			public class LocalisedString
			{
				public string Language = "";
				public string CDATA = "";
				public bool useDesc = false;
			}
		}

		public string errorMessage = "";

		private string filename = "";
		private string cleanName = "";

		/*
		public Stream getFile(Sims3PackFileInfo file)
		{
			if (String.IsNullOrEmpty(this.filename)) return new MemoryStream();

			FileStream s3pfile = new FileStream(this.filename, FileMode.Open, FileAccess.Read, FileShare.Read);
			BinaryReader readFile = new BinaryReader(s3pfile);

			uint version = readFile.ReadUInt32();
			string s3pack = Encoding.ASCII.GetString(readFile.ReadBytes(7));
			if (s3pack != "TS3Pack")
			{
				errorMessage = "Not a Sims3Pack file";
				readFile.Close();
				s3pfile.Close();
				return new MemoryStream();
			}

			uint unknown = readFile.ReadUInt16();

			int xmlLength = readFile.ReadInt32();
			s3pfile.Seek(xmlLength + file.offset, SeekOrigin.Current);

			MemoryStream retStream = new MemoryStream(readFile.ReadBytes(file.length));

			readFile.Close();
			s3pfile.Close();

			return retStream;
		}
		*/

		public Sims3PackFile Load(string filename)
        {
            FileStream s3pfile = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
            BinaryReader readFile = new BinaryReader(s3pfile);

            FileInfo f = new FileInfo(filename);
            this.cleanName = f.Name.Replace(f.Extension, "");

            uint version = readFile.ReadUInt32();
            string s3pack = Encoding.ASCII.GetString(readFile.ReadBytes(7));
            if (s3pack != "TS3Pack")
            {
                errorMessage = "Not a Sims3Pack file";
                readFile.Close();
                s3pfile.Close();
                return new Sims3PackFile();
            }
            this.filename = filename;

			Sims3PackFile s3p = new Sims3PackFile();

            s3p.UnknownInt = readFile.ReadUInt16();

            int xmlLength = readFile.ReadInt32();

            byte[] xmlBytes = readFile.ReadBytes(xmlLength);

			s3p.rawXml = System.Text.UTF8Encoding.UTF8.GetString(xmlBytes);

            MemoryStream mem = new MemoryStream(xmlBytes);
			XmlTextReader xtr = new XmlTextReader(mem);

			int level = 0;
			string curName = "";
			string parent = "";
			string temp = "";

			while (xtr.Read())
			{
				if (xtr.NodeType == XmlNodeType.Element)
				{
					curName = xtr.Name;
					switch (curName)
					{
						case "Sims3Package":
							if (xtr.HasAttributes)
							{
								xtr.MoveToAttribute("Type");
								s3p.Type = xtr.Value;
								xtr.MoveToAttribute("SubType");
								s3p.SubType = xtr.Value;
							}
							break;
						case "Dependencies":
							if (!xtr.IsEmptyElement)
							{
								level++;
							}
							break;
						case "LocalizedNames":
						case "LocalizedDescriptions":
							if (!xtr.IsEmptyElement)
							{
								parent = curName;
								level++;
							}
							break;
						case "LocalizedName":
						case "LocalizedDescription":
							xtr.MoveToAttribute("Language");
							temp = xtr.Value;
							break;
						case "PackagedFile":
							Sims3PackFile.PackagedFile packagedFile = ReadPackagedFileXml(xtr, readFile, xmlLength);
							s3p.PackagedFiles.Add(packagedFile);
							break;
					}

				}
				if (xtr.NodeType == XmlNodeType.CDATA)
				{
					switch (curName)
					{
						case "LocalizedName":
							if (level == 1 && parent == "LocalizedDescriptions")
							{
								Sims3PackFile.LocalisedString localizedName = new Sims3PackFile.LocalisedString();
								localizedName.Language = temp;
								localizedName.CDATA = xtr.Value.ToString();
								s3p.LocalizedDescriptions.Add(localizedName);
							}
							if (level == 1 && parent == "LocalizedNames")
							{
								Sims3PackFile.LocalisedString localizedName = new Sims3PackFile.LocalisedString();
								localizedName.Language = temp;
								localizedName.CDATA = xtr.Value;
								s3p.LocalizedNames.Add(localizedName);
							}
							break;
						case "LocalizedDescription":
							if (level == 1)
							{
								Sims3PackFile.LocalisedString localizedDesc = new Sims3PackFile.LocalisedString();
								localizedDesc.Language = temp;
								localizedDesc.CDATA = xtr.Value;
								localizedDesc.useDesc = true;
								s3p.LocalizedDescriptions.Add(localizedDesc);
							}
							break;
					}
				}
				if (xtr.NodeType == XmlNodeType.Text)
				{
					switch (curName.ToLower())
					{
						case "archiveversion":
							s3p.ArchiveVersion = xtr.Value;
							break;
						case "codeversion":
							s3p.CodeVersion = xtr.Value;
							break;
						case "gameversion":
							s3p.GameVersion = xtr.Value;
							break;
						case "assetversion":
							if (!String.IsNullOrEmpty(xtr.Value)) s3p.AssetVersion = Convert.ToInt32(xtr.Value);
							break;
						case "minreqversion":
							s3p.MinReqVersion = xtr.Value;
							break;
						case "displayname":
							s3p.DisplayName = xtr.Value;
							break;
						case "description":
							if (level == 0) s3p.Description = xtr.Value;
							break;
						case "packageid":
							s3p.PackageId = xtr.Value;
							break;
						case "date":
							s3p.Date = xtr.Value;
							break;
						case "dependency":
							if (level == 1)
							{
								string dependency = xtr.Value;
								s3p.Dependencies.Add(dependency);

							}
							break;

					}
				}
				if (xtr.NodeType == XmlNodeType.EndElement)
				{
					switch (xtr.Name.ToLower())
					{
						case "dependencies":
						case "localizednames":
						case "localizeddescriptions":
							level--;
							break;
					}
				}
			}

			s3pfile.Close();
			return s3p;
        }

		public void Save(string filename, Sims3PackFile s3p)
		{
			Stream output = File.Open(filename, FileMode.Create, FileAccess.Write, FileShare.Write);

			StreamHelpers.WriteValueU32(output, 7);
			StreamHelpers.WriteStringASCII(output, "TS3Pack");
			StreamHelpers.WriteValueU16(output, s3p.UnknownInt);

			// First pass.  
			// Recalculate all the package file offsets, based on which ones are checked, and remove disabled ones
			int curOffset = 0;
			for (int i = 0; i < s3p.PackagedFiles.Count; i++)
			{
				if (!s3p.PackagedFiles[i].isDisabled)
				{
					s3p.PackagedFiles[i].Offset = curOffset;
					curOffset += s3p.PackagedFiles[i].Length;

					if (s3p.PackagedFiles[i].MetaTags.lotThumb.Count > 0)
					{
						List<string> tempThumbs = new List<string>();
						for (int y = 0; y < s3p.PackagedFiles[i].MetaTags.lotThumb.Count; y++)
						{
							for (int z = 0; z < s3p.PackagedFiles.Count; z++)
							{
								if (!s3p.PackagedFiles[z].isDisabled && s3p.PackagedFiles[z].Name == s3p.PackagedFiles[i].MetaTags.lotThumb[y] + ".png")
								{
									tempThumbs.Add(s3p.PackagedFiles[i].MetaTags.lotThumb[y]);
								}
							}
						}
						s3p.PackagedFiles[i].MetaTags.lotThumb = tempThumbs;

					}
				}
			}

			string xml = WriteSims3PackXml(s3p);
			StreamHelpers.WriteValueS32(output, 0);
			StreamHelpers.WriteStringUTF8(output, xml);

			int xmlLength = (int)output.Position - 17;

			for (int i = 0; i < s3p.PackagedFiles.Count; i++)
			{
				if (!s3p.PackagedFiles[i].isDisabled)
				{
					StreamHelpers.CopyStream(s3p.PackagedFiles[i].DBPF, output, true);
				}
			}

			output.Seek(4 + 7 + 2, SeekOrigin.Begin);
			StreamHelpers.WriteValueS32(output, xmlLength);
			
			output.Close();
		}

		public string WriteSims3PackXml(Sims3PackFile s3p)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");

			sb.AppendLine("<Sims3Package Type=\"" + s3p.Type + "\" SubType=\"" + s3p.SubType + "\">");
			if (!String.IsNullOrEmpty(s3p.ArchiveVersion)) sb.AppendLine("  <ArchiveVersion>" + s3p.ArchiveVersion + "</ArchiveVersion>");
			if (!String.IsNullOrEmpty(s3p.CodeVersion)) sb.AppendLine("  <CodeVersion>" + s3p.CodeVersion + "</CodeVersion>");
			if (!String.IsNullOrEmpty(s3p.GameVersion)) sb.AppendLine("  <GameVersion>" + s3p.GameVersion + "</GameVersion>");
			if (!String.IsNullOrEmpty(s3p.DisplayName)) sb.AppendLine("  <DisplayName>" + s3p.DisplayName + "</DisplayName>");
			if (!String.IsNullOrEmpty(s3p.Description)) sb.AppendLine("  <Description>" + s3p.Description + "</Description>");
			if (!String.IsNullOrEmpty(s3p.PackageId)) sb.AppendLine("  <PackageId>" + s3p.PackageId + "</PackageId>");
			if (!String.IsNullOrEmpty(s3p.Date)) sb.AppendLine("  <Date>" + s3p.Date + "</Date>");
			if (s3p.AssetVersion > -1) sb.AppendLine("  <AssetVersion>" + s3p.AssetVersion.ToString() + "</AssetVersion>");
			if (!String.IsNullOrEmpty(s3p.MinReqVersion)) sb.AppendLine("  <MinReqVersion>" + s3p.MinReqVersion + "</MinReqVersion>");
			if (s3p.Dependencies.Count > 0)
			{
				sb.AppendLine("  <Dependencies>");
				for (int i = 0; i < s3p.Dependencies.Count; i++)
				{
					sb.AppendLine("    <Dependency>" + s3p.Dependencies[i] + "</Dependency>");
				}
				sb.AppendLine("  </Dependencies>");
			}
			else
			{
				sb.AppendLine("  <Dependencies />");
			}
			if (s3p.LocalizedNames.Count > 0)
			{
				sb.AppendLine("  <LocalizedNames>");
				for (int i = 0; i < s3p.LocalizedNames.Count; i++)
				{
					sb.AppendLine("    <LocalizedName Language=\"" + s3p.LocalizedNames[i].Language + "\"><![CDATA[" + s3p.LocalizedNames[i].CDATA + "]]></LocalizedName>");
				}
				sb.AppendLine("  </LocalizedNames>");
			}
			else
			{
				sb.AppendLine("  <LocalizedNames />");
			}
			if (s3p.LocalizedDescriptions.Count > 0)
			{
				sb.AppendLine("  <LocalizedDescriptions>");
				for (int i = 0; i < s3p.LocalizedDescriptions.Count; i++)
				{
					sb.Append("    <Localized");
					if (s3p.LocalizedDescriptions[i].useDesc)
					{
						sb.Append("Description");
					}
					else
					{
						sb.Append("Name");
					}
					sb.AppendLine(" Language=\"" + s3p.LocalizedDescriptions[i].Language + "\"><![CDATA[" + s3p.LocalizedDescriptions[i].CDATA + "]]></LocalizedName>");
				}
				sb.AppendLine("  </LocalizedDescriptions>");
			}
			else
			{
				sb.AppendLine("  <LocalizedDescriptions />");
			}

			// Packaged Files
			int curOffset = 0;
			if (s3p.PackagedFiles.Count > 0)
			{
				for (int i = 0; i < s3p.PackagedFiles.Count; i++)
				{
					if (!s3p.PackagedFiles[i].isDisabled)
					{
						//s3p.PackagedFiles[i].Offset = curOffset;
						sb.Append(WritePackagedFileXml(s3p.PackagedFiles[i], "  "));
						//curOffset += s3p.PackagedFiles[i].Length;
					}
				}
			}
			
			sb.Append("</Sims3Package>");

			Console.WriteLine(sb.Length);
			return sb.ToString();
		}

		private Sims3PackFile.PackagedFile ReadPackagedFileXml(XmlTextReader xtr, BinaryReader reader, int xmlLength)
		{
			Sims3PackFile.PackagedFile packagedFile = new Sims3PackFile.PackagedFile();
			string curName = "";

			while (xtr.Read())
			{
				if (xtr.NodeType == XmlNodeType.Element)
				{
					curName = xtr.Name;
					switch (curName)
					{
						case "metatags":
							if (!xtr.IsEmptyElement)
							{
								packagedFile.MetaTags = ReadMetaTagsXml(xtr);
							}
							break;
					}
				}
				if (xtr.NodeType == XmlNodeType.Text)
				{
					switch (curName.ToLower())
					{
						case "name":
							packagedFile.Name = xtr.Value;
							break;
						case "length":
							packagedFile.Length = Convert.ToInt32(xtr.Value);
							break;
						case "offset":
							packagedFile.Offset = Convert.ToInt32(xtr.Value);
							break;
						case "crc":
							packagedFile.Crc = xtr.Value;
							break;
						case "guid":
							packagedFile.Guid = xtr.Value;
							break;
						case "thumbnail":
							packagedFile.Thumbnail = xtr.Value;
							break;
						case "contenttype":
							packagedFile.ContentType = xtr.Value;
							break;
						case "epflags":
							packagedFile.EPFlags = xtr.Value;
							break;
					}
				}
				if (xtr.NodeType == XmlNodeType.EndElement && xtr.Name.ToLower() == "packagedfile")
				{
					reader.BaseStream.Seek(7 + 4 + 2 + 4 + xmlLength + packagedFile.Offset, SeekOrigin.Begin);
					packagedFile.DBPF = new MemoryStream(reader.ReadBytes(packagedFile.Length));
					break;
				}
			}

			return packagedFile;
		}

		private string WritePackagedFileXml(Sims3PackFile.PackagedFile packagedFile, string indent)
		{
			if (packagedFile.isDisabled) return "";

			StringBuilder sb = new StringBuilder();
			sb.AppendLine(indent + "<PackagedFile>");
			if (!String.IsNullOrEmpty(packagedFile.Name)) sb.AppendLine(indent + "  <Name>" + packagedFile.Name + "</Name>");
			if (packagedFile.Length > -1) sb.AppendLine(indent + "  <Length>" + packagedFile.Length.ToString() + "</Length>");
			if (packagedFile.Offset > -1) sb.AppendLine(indent + "  <Offset>" + packagedFile.Offset.ToString() + "</Offset>");
			if (!String.IsNullOrEmpty(packagedFile.Crc)) sb.AppendLine(indent + "  <Crc>" + packagedFile.Crc + "</Crc>");
			if (!String.IsNullOrEmpty(packagedFile.Guid)) sb.AppendLine(indent + "  <Guid>" + packagedFile.Guid + "</Guid>");
			if (!String.IsNullOrEmpty(packagedFile.ContentType)) sb.AppendLine(indent + "  <ContentType>" + packagedFile.ContentType + "</ContentType>");
			if (!String.IsNullOrEmpty(packagedFile.EPFlags)) sb.AppendLine(indent + "  <EPFlags>" + packagedFile.EPFlags + "</EPFlags>");
			if (!String.IsNullOrEmpty(packagedFile.Thumbnail))
			{
				sb.AppendLine(indent + "  <Thumbnail>" + packagedFile.Thumbnail + "</Thumbnail>");
			}
			else
			{
				sb.AppendLine(indent + "  <Thumbnail />");
			}
			if (!String.IsNullOrEmpty(packagedFile.MetaTags.name))
			{
				sb.Append(WriteMetaTagsXml(packagedFile.MetaTags, indent + "  "));
			}
			else
			{
				sb.AppendLine(indent + "  <metatags />");
			}
			sb.AppendLine(indent + "</PackagedFile>");

			return sb.ToString();
		}

		private Sims3PackFile.PackagedFile.MetaTag ReadMetaTagsXml(XmlTextReader xtr)
		{
			Sims3PackFile.PackagedFile.MetaTag metatag = new Sims3PackFile.PackagedFile.MetaTag();
			string curName = "";
			while (xtr.Read())
			{
				if (xtr.NodeType == XmlNodeType.Element)
				{
					curName = xtr.Name;
					if (curName.ToLower() == "lotthumbs")
					{
						if (!xtr.IsEmptyElement)
						{
							// Read the lotThumbs structure
							while (xtr.Read())
							{
								if (xtr.NodeType == XmlNodeType.Element)
								{
									switch (xtr.Name.ToLower())
									{
										case "lotthumb":
											xtr.Read();
											if (xtr.NodeType == XmlNodeType.Whitespace) xtr.Read();
											metatag.lotThumb.Add(xtr.Value);
											break;
									}
								}
								if (xtr.NodeType == XmlNodeType.EndElement && xtr.Name.ToLower() == "lotthumbs")
								{
									break;
								}
							}
						}
					}
					if (curName.ToLower() == "traits")
					{
						if (!xtr.IsEmptyElement)
						{
							// Read the traits structure
							while (xtr.Read())
							{
								if (xtr.NodeType == XmlNodeType.Element)
								{
									switch (xtr.Name.ToLower())
									{
										case "trait":
											xtr.Read();
											if (xtr.NodeType == XmlNodeType.Whitespace) xtr.Read();
											metatag.traits.Add(xtr.Value);
											break;
									}
								}
								if (xtr.NodeType == XmlNodeType.EndElement && xtr.Name.ToLower() == "traits")
								{
									break;
								}
							}
						}
					}
					if (curName.ToLower() == "outfits")
					{
						if (!xtr.IsEmptyElement)
						{
							// Read the outfits structure
							while (xtr.Read())
							{
								if (xtr.NodeType == XmlNodeType.Element)
								{
									switch (xtr.Name)
									{
										case "outfitAthletic":
											xtr.Read();
											if (xtr.NodeType == XmlNodeType.Whitespace) xtr.Read();
											xtr.Read();
											metatag.outfits.athletic = xtr.Value;
											metatag.outfits.numOutfits++;
											break;
										case "outfitEveryday":
											xtr.Read();
											if (xtr.NodeType == XmlNodeType.Whitespace) xtr.Read();
											xtr.Read();
											metatag.outfits.everyday = xtr.Value;
											metatag.outfits.numOutfits++;
											break;
										case "outfitFormalwear":
											xtr.Read();
											if (xtr.NodeType == XmlNodeType.Whitespace) xtr.Read();
											xtr.Read();
											metatag.outfits.formalwear = xtr.Value;
											metatag.outfits.numOutfits++;
											break;
										case "outfitNaked":
											xtr.Read();
											if (xtr.NodeType == XmlNodeType.Whitespace) xtr.Read();
											xtr.Read();
											metatag.outfits.naked = xtr.Value;
											metatag.outfits.numOutfits++;
											break;
										case "outfitSleepwear":
											xtr.Read();
											if (xtr.NodeType == XmlNodeType.Whitespace) xtr.Read();
											xtr.Read();
											metatag.outfits.sleepwear = xtr.Value;
											metatag.outfits.numOutfits++;
											break;
										case "outfitSwimwear":
											xtr.Read();
											if (xtr.NodeType == XmlNodeType.Whitespace) xtr.Read();
											xtr.Read();
											metatag.outfits.swimwear = xtr.Value;
											metatag.outfits.numOutfits++;
											break;

									}
								}
								if (xtr.NodeType == XmlNodeType.EndElement && xtr.Name.ToLower() == "outfits")
								{
									break;
								}
							}
						}
					}

				}
				if (xtr.NodeType == XmlNodeType.Text)
				{
					switch (curName.ToLower())
					{
						case "name":
							metatag.name = xtr.Value;
							break;
						case "description":
							metatag.description = xtr.Value;
							break;
						case "bio":
							metatag.bio = xtr.Value;
							break;
						case "numofthumbs":
							metatag.numLotThumbs = Convert.ToInt32(xtr.Value);
							break;
						case "maxlevel":
							metatag.maxLevel = Convert.ToInt32(xtr.Value);
							break;
						case "minlevel":
							metatag.minLevel = Convert.ToInt32(xtr.Value);
							break;
						case "dimx":
							metatag.dimX = Convert.ToInt32(xtr.Value);
							break;
						case "dimz":
							metatag.dimZ = Convert.ToInt32(xtr.Value);
							break;
						case "lottype":
							metatag.lottype = Convert.ToInt32(xtr.Value);
							break;
						case "numlotthumbs":
							metatag.numLotThumbs = Convert.ToInt32(xtr.Value);
							break;
						case "lotname":
							metatag.lotName = xtr.Value;
							break;
						case "lotressubtype":
							metatag.lotResSubType = Convert.ToInt32(xtr.Value);
							break;
						case "lotcomsubtype":
							metatag.lotComSubType = Convert.ToInt32(xtr.Value);
							break;
						// Sims:
						case "age":
							metatag.age = xtr.Value;
							break;
						case "species":
							metatag.species = xtr.Value;
							break;
						case "gender":
							metatag.gender = xtr.Value;
							break;
						case "fitness":
							metatag.fitness = xtr.Value;
							break;
						case "weight":
							metatag.weight = xtr.Value;
							break;
						case "favcolor":
							metatag.favcolor = xtr.Value;
							break;
						case "favmusic":
							metatag.favmusic = Convert.ToInt32(xtr.Value);
							break;
						case "favfood":
							metatag.favfood = Convert.ToInt32(xtr.Value);
							break;
						case "skintoneindex":
							metatag.skintoneindex = xtr.Value;
							break;
						case "handedness":
							metatag.handedness = xtr.Value;
							break;
					}
				}
				if (xtr.NodeType == XmlNodeType.EndElement && xtr.Name.ToLower() == "metatags") break;
			}
			return metatag;
		}

		private string WriteMetaTagsXml(Sims3PackFile.PackagedFile.MetaTag metatag, string indent)
		{
			StringBuilder sb = new StringBuilder();
			if (String.IsNullOrEmpty(metatag.name))
			{
				return "<metatags />";
			}

			sb.AppendLine(indent + "<metatags>");

			sb.AppendLine(indent + "  <name>" + metatag.name + "</name>");
			if (!String.IsNullOrEmpty(metatag.description)) sb.AppendLine(indent + "  <description>" + metatag.description + "</description>");
			if (!String.IsNullOrEmpty(metatag.bio)) sb.AppendLine(indent + "  <bio>" + metatag.bio + "</bio>");
			if (metatag.numOfThumbs > -999999) sb.AppendLine(indent + "  <numOfThumbs>" + metatag.numOfThumbs.ToString() + "</numOfThumbs>");

			// Lot information
			if (metatag.maxLevel > -999999) sb.AppendLine(indent + "  <maxlevel>" + metatag.maxLevel.ToString() + "</maxlevel>");
			if (metatag.minLevel > -999999) sb.AppendLine(indent + "  <minlevel>" + metatag.minLevel.ToString() + "</minlevel>");
			if (metatag.dimX > -999999) sb.AppendLine(indent + "  <dimX>" + metatag.dimX.ToString() + "</dimX>");
			if (metatag.dimZ > -999999) sb.AppendLine(indent + "  <dimZ>" + metatag.dimZ.ToString() + "</dimZ>");
			if (metatag.lottype > -999999) sb.AppendLine(indent + "  <lottype>" + metatag.lottype.ToString() + "</lottype>");
			if (metatag.lotThumb.Count > 0)
			{
				sb.AppendLine(indent + "  <numLotThumbs>" + metatag.lotThumb.Count.ToString() + "</numLotThumbs>");
				sb.AppendLine(indent + "  <lotThumbs>");
				for (int i = 0; i < metatag.lotThumb.Count; i++)
				{
					sb.AppendLine(indent + "    <lotThumb>" + metatag.lotThumb[i] + "</lotThumb>");
				}
				sb.AppendLine(indent + "  </lotThumbs>");
			}
			if (!String.IsNullOrEmpty(metatag.lotName)) sb.AppendLine(indent + "  <lotName>" + metatag.lotName + "</lotName>");
			if (metatag.lotResSubType > -999999) sb.AppendLine(indent + "  <lotResSubType>" + metatag.lotResSubType.ToString() + "</lotResSubType>");
			if (metatag.lotComSubType > -999999) sb.AppendLine(indent + "  <lotComSubType>" + metatag.lotComSubType.ToString() + "</lotComSubType>");

			if (!String.IsNullOrEmpty(metatag.age)) sb.AppendLine(indent + "  <age>" + metatag.age + "</age>");
			if (!String.IsNullOrEmpty(metatag.species)) sb.AppendLine(indent + "  <species>" + metatag.species + "</species>");
			if (!String.IsNullOrEmpty(metatag.gender)) sb.AppendLine(indent + "  <gender>" + metatag.gender + "</gender>");
			if (metatag.traits.Count > 0)
			{
				sb.AppendLine(indent + "  <traits>");
				for (int i = 0; i < metatag.traits.Count; i++)
				{
					sb.AppendLine(indent + "    <trait>" + metatag.traits[i] + "</trait>");
				}
				sb.AppendLine(indent + "  </traits>");
			}
			if (!String.IsNullOrEmpty(metatag.fitness)) sb.AppendLine(indent + "  <fitness>" + metatag.fitness + "</fitness>");
			if (!String.IsNullOrEmpty(metatag.weight)) sb.AppendLine(indent + "  <weight>" + metatag.weight + "</weight>");
			if (!String.IsNullOrEmpty(metatag.favcolor)) sb.AppendLine(indent + "  <favcolor>" + metatag.age + "</favcolor>");
			if (metatag.favmusic > -999999) sb.AppendLine(indent + "  <favmusic>" + metatag.favmusic.ToString() + "</favmusic>");
			if (metatag.favfood > -999999) sb.AppendLine(indent + "  <favfood>" + metatag.favfood.ToString() + "</favfood>");
			if (!String.IsNullOrEmpty(metatag.skintoneindex)) sb.AppendLine(indent + "  <skintoneindex>" + metatag.skintoneindex + "</skintoneindex>");
			if (!String.IsNullOrEmpty(metatag.handedness)) sb.AppendLine(indent + "  <handedness>" + metatag.handedness + "</handedness>");

			if (metatag.outfits.numOutfits > 0)
			{
				sb.AppendLine(indent + "  <outfits>");
				if (!String.IsNullOrEmpty(metatag.outfits.athletic))
				{
					sb.AppendLine(indent + "    <outfitAthletic>");
					sb.AppendLine(indent + "      <outfitId>" + metatag.outfits.athletic + "</outfitId>");
					sb.AppendLine(indent + "    </outfitAthletic>");
				}
				if (!String.IsNullOrEmpty(metatag.outfits.everyday))
				{
					sb.AppendLine(indent + "    <outfitEveryday>");
					sb.AppendLine(indent + "      <outfitId>" + metatag.outfits.everyday + "</outfitId>");
					sb.AppendLine(indent + "    </outfitEveryday>");
				}
				if (!String.IsNullOrEmpty(metatag.outfits.formalwear))
				{
					sb.AppendLine(indent + "    <outfitFormalwear>");
					sb.AppendLine(indent + "      <outfitId>" + metatag.outfits.formalwear + "</outfitId>");
					sb.AppendLine(indent + "    </outfitFormalwear>");
				}
				if (!String.IsNullOrEmpty(metatag.outfits.naked))
				{
					sb.AppendLine(indent + "    <outfitNaked>");
					sb.AppendLine(indent + "      <outfitId>" + metatag.outfits.naked + "</outfitId>");
					sb.AppendLine(indent + "    </outfitNaked>");
				}
				if (!String.IsNullOrEmpty(metatag.outfits.sleepwear))
				{
					sb.AppendLine(indent + "    <outfitSleepwear>");
					sb.AppendLine(indent + "      <outfitId>" + metatag.outfits.sleepwear + "</outfitId>");
					sb.AppendLine(indent + "    </outfitSleepwear>");
				}
				if (!String.IsNullOrEmpty(metatag.outfits.swimwear))
				{
					sb.AppendLine(indent + "    <outfitSwimwear>");
					sb.AppendLine(indent + "      <outfitId>" + metatag.outfits.swimwear + "</outfitId>");
					sb.AppendLine(indent + "    </outfitSwimwear>");
				}

			}

			sb.AppendLine(indent + "</metatags>");

			return sb.ToString();
		}

		/*
        private Stack getXmlFields(XPathDocument doc, string xpath)
        {
            Stack myStack = new Stack();

            XPathNavigator nav = doc.CreateNavigator();
            XPathExpression expr;

            expr = nav.Compile(xpath);
            XPathNodeIterator iterator = nav.Select(expr);

            try
            {
                while (iterator.MoveNext())
                {
                    XPathNavigator nav2 = iterator.Current.Clone();
                    myStack.Push(nav2.Value);                                        
                    //Console.WriteLine(nav2.Value);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return myStack;
        }
		*/

		/*
        public bool extractFile(Sims3PackFileInfo file, string destinationFolder)
        {
            return extractFile(file, destinationFolder, true);
        }
        public bool extractFile(Sims3PackFileInfo file, string destinationFolder, bool cleanNames )
        {
            FileStream s3pfile = new FileStream(this.filename, FileMode.Open, FileAccess.Read, FileShare.Read);
            BinaryReader readFile = new BinaryReader(s3pfile);

            uint version = readFile.ReadUInt32();
            string s3pack = Encoding.ASCII.GetString(readFile.ReadBytes(7));
            if (s3pack != "TS3Pack")
            {
                errorMessage = "Not a Sims3Pack file";
                readFile.Close();
                s3pfile.Close();
                return false;
            }

            uint unknown = readFile.ReadUInt16();

            int xmlLength = readFile.ReadInt32();
            s3pfile.Seek(xmlLength + file.offset, SeekOrigin.Current);

            string outputName = "";
            if (cleanNames && this.packagedFiles.Count == 1)
            {
                outputName = this.cleanName + ".package";
            }
            else
            {
                outputName = file.name;
            }

            FileStream destFile = new FileStream(destinationFolder + "\\" + outputName, FileMode.Create, FileAccess.Write, FileShare.Write);
            BinaryWriter writeFile = new BinaryWriter(destFile);

            writeFile.Write(readFile.ReadBytes(file.length));

            writeFile.Close();
            destFile.Close();

            readFile.Close();
            s3pfile.Close();

            return true;
        }
		 */
    }

}
