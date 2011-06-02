using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using MadScience.Wrappers;

namespace MadScience
{
	public class Detective
	{

		private enum ResourceTypes : uint
		{
			STBL = 0x220557DA,
			TXTC = 0x033A1435,
			MLOD = 0x01D10F34,
			OBJK = 0x02DC343F,
			OBJD = 0x319E4F1D,
			VPXY = 0x736884F1,
			_RIG = 0x8EAF13DE,
			LITE = 0x03B4C61D,
			FTPT = 0xD382BF57,
			S3SA = 0x073FAA07,
			_IMG = 0x00B2D882,
			CASP = 0x034AEECB,
			PTRN = 0xD4D9FBE5,
			FBLN = 0xB52F5055,
			FACE = 0x0358B08A,
			BOND = 0x0355E0A6,
			UNKW1 = 0x033B2B66,
			WLOT = 0x03D86EA4,
			WLTL = 0x04EE6ABB, // Lot terrain texture list
			ARY2 = 0x05FF6BA4,
			SIMO = 0x025ED6F4, // Sim Outfit
			SIME = 0x04F88964, // Townie data
			SNAPL = 0x0580A2CF, // Large image of Sims
			SNAP = 0x0580A2CD,
			_XML = 0x73E93EEB,
			_XML2 = 0x0333406C,
			NULL = 0x00000000,
			COMP = 0x044AE110, // Complate

		}

		public enum PackageTypes : uint
		{
			genericPackage = 0,
			sims3Store = 1,
			emptyPackage = 2,
			disabledPackage = 3,
			duplicatePackage = 4,
			conflictPackage = 5,
			sims2Package = 9,
			corruptBadDownload = 10,
			corruptNotADBPF = 11,
			corruptChaavik = 12,
			corruptTXTC = 13,
			corruptIndex = 14,
			corruptPeggy = 15,
			corruptBadAges = 16,
			corruptRecursive = 17,
			patternGeneric = 20,
			casPartGeneric = 30,
			casPartClothing = 31,
			casPartHair = 32,
			casPartMakeup = 33,
			casPartFaceOverlay = 34,
			casPartAccessory = 35,
			coremod = 40,
			xmltuningmod = 41,
			objectGeneric = 50,
			casSlider = 90,
			textureReplacement = 91,
			neighbourhood = 99,
			lot = 150,
			sim = 160,
			pngThumbnail = 200,
			
		}

		public class PackageType
		{
			public PackageTypes MainType = PackageTypes.genericPackage;
			public string SubType = "";

			public override string ToString()
			{
				return this.ToString(this.MainType);
			}

			public string ToString(PackageTypes pType)
			{
				switch (pType)
				{
					case PackageTypes.sims3Store: return "Sims 3 Store Package";
					case PackageTypes.emptyPackage: return "Empty Package";
					case PackageTypes.sims2Package: return "Sims 2 Package";
					case PackageTypes.corruptBadDownload: return "Corrupt (Bad download)";
					case PackageTypes.corruptNotADBPF: return "Corrupt (Not a DBPF)";
					case PackageTypes.corruptChaavik: return "Corrupt (Chaavik)";
					case PackageTypes.corruptTXTC: return "Corrupt (TXTC)";
					case PackageTypes.corruptPeggy: return "Corrupt (Peggy)";
					case PackageTypes.corruptIndex: return "Corrupt (Bad Index)";
					case PackageTypes.corruptBadAges: return "Corrupt (Bad Ages - Child + Adult)";
					case PackageTypes.corruptRecursive: return "Corrupt (Contains another DBPF)";
					case PackageTypes.patternGeneric: return "Pattern";
					case PackageTypes.casPartGeneric: return "CAS Part";
					case PackageTypes.casPartClothing: return "CAS Part (Clothing)";
					case PackageTypes.casPartHair: return "CAS Part (Hair)";
					case PackageTypes.casPartMakeup: return "CAS Part (Makeup)";
					case PackageTypes.casPartFaceOverlay: return "CAS Part (Face Overlay)";
					case PackageTypes.casPartAccessory: return "CAS Part (Accessory)";
					case PackageTypes.coremod: return "Core Mod";
					case PackageTypes.xmltuningmod: return "XML Tuning Mod";
					case PackageTypes.objectGeneric: return "Object";
					case PackageTypes.casSlider: return "CAS Slider";
					case PackageTypes.textureReplacement: return "Texture Replacement";
					case PackageTypes.neighbourhood: return "Neighbourhood (World)";
					case PackageTypes.lot: return "Lot";
					case PackageTypes.sim: return "Sim";
					case PackageTypes.pngThumbnail: return "Thumbnail";
				}

				return "Sims 3 Package";
			}

			public PackageTypes ToType(string pType)
			{
				switch (pType)
				{
					case "Sims 3 Store Package": return PackageTypes.sims3Store;
					case "Empty Package": return PackageTypes.emptyPackage;
					case "Sims 2 Package": return PackageTypes.sims2Package;
					case "Corrupt (Bad download)": return PackageTypes.corruptBadDownload;
					case "Corrupt (Not a DBPF)": return PackageTypes.corruptNotADBPF;
					case "Corrupt (Chaavik)": return PackageTypes.corruptChaavik;
					case "Corrupt (TXTC)": return PackageTypes.corruptTXTC;
					case "Corrupt (Peggy)": return PackageTypes.corruptPeggy;
					case "Corrupt (Bad Index)": return PackageTypes.corruptIndex;
					case "Corrupt (Bad Ages - Child + Adult)": return PackageTypes.corruptBadAges;
					case "Corrupt (Contains another DBPF)": return PackageTypes.corruptRecursive;
					case "Pattern": return PackageTypes.patternGeneric;
					case "CAS Part": return PackageTypes.casPartGeneric;
					case "CAS Part (Clothing)": return PackageTypes.casPartClothing;
					case "CAS Part (Hair)": return PackageTypes.casPartHair;
					case "CAS Part (Makeup)": return PackageTypes.casPartMakeup;
					case "CAS Part (Face Overlay)": return PackageTypes.casPartFaceOverlay;
					case "CAS Part (Accessory)": return PackageTypes.casPartAccessory;
					case "Core Mod": return PackageTypes.coremod;
					case "XML Tuning Mod": return PackageTypes.xmltuningmod;
					case "Object": return PackageTypes.objectGeneric;
					case "CAS Slider": return PackageTypes.casSlider;
					case "Texture Replacement": return PackageTypes.textureReplacement;
					case "Neighbourhood (World)": return PackageTypes.neighbourhood;
					case "Lot": return PackageTypes.lot;
					case "Sim": return PackageTypes.sim;
					case "Thumbnail": return PackageTypes.pngThumbnail;
				}

				return PackageTypes.genericPackage;
			}

			public Color ToColor()
			{
				return this.ToColor(this.MainType);
			}

			public Color ToColor(PackageTypes pType)
			{
				switch (pType)
				{
					case PackageTypes.emptyPackage: return Color.Teal;
					case PackageTypes.disabledPackage: return Color.PowderBlue;
					case PackageTypes.duplicatePackage: return Color.Yellow;
					case PackageTypes.conflictPackage: return Color.Goldenrod;
					//case PackageTypes.sims3Store: return "Sims 3 Store Package";
					case PackageTypes.sims2Package: return Color.SteelBlue;
					case PackageTypes.pngThumbnail: return Color.BlanchedAlmond;
					case PackageTypes.corruptBadDownload:
					case PackageTypes.corruptNotADBPF:
					case PackageTypes.corruptChaavik:
					case PackageTypes.corruptTXTC:
					case PackageTypes.corruptIndex:
					case PackageTypes.corruptPeggy:
					case PackageTypes.corruptBadAges:
					case PackageTypes.corruptRecursive:
						return Color.Salmon;
					//case PackageTypes.patternGeneric: return "Pattern";
					//case PackageTypes.casPartGeneric: return "CAS Part";
					//case PackageTypes.casPartClothing: return "CAS Part (Clothing)";
					//case PackageTypes.coremod: return "Core Mod";
					//case PackageTypes.objectGeneric: return "Object";
				}

				return Color.Empty;

			}

		}

		Dictionary<string, uint> rc = new Dictionary<string, uint>();

		public bool isCorrupt = false;
		public bool isDisabled = false;

		public PackageType pType = new PackageType();

		private int checkValidEntry(DatabasePackedFile.Entry entry, Database db)
		{
			
			int validEntry = 0; // 0 = valid
			if (!checkTXTCEntry(entry, db))
			{
				validEntry = 2; // 2 = BlueLot TXTC
			}

			return validEntry;
		}

		private bool checkTXTCEntry(DatabasePackedFile.Entry entry, Database db)
		{
			if (entry.Key.typeId == 0x033A1435)
			{
				//textBox1.Text += "  " + entry.Key.ToString() + Environment.NewLine;
				// textBox1.Text += "    Checking for corrupted TGI offset... ";

				// Quick and dirty way
				Stream TXTC = db.GetResourceStream(entry.Key);
				// Read offset, first 4 bytes after ID
				StreamHelpers.ReadValueU32(TXTC);

				uint offset = StreamHelpers.ReadValueU32(TXTC);
				// textBox1.Text += offset.ToString() + "...";

				// Seek to this offset + 8 and read the number there.
				TXTC.Seek(offset + 8, SeekOrigin.Begin);

				uint numTGIs = StreamHelpers.ReadValueU8(TXTC);
				// textBox1.Text += numTGIs.ToString() + " TGIs... ";

				// Since each TGI is 16 bytes we can calculate how many bytes they are.
				uint tgiSize = numTGIs * 16;
				uint tgiOffsetEnd = offset + 8 + 1 + tgiSize;

				//textBox1.Text += "TGI block end is at " + tgiOffsetEnd.ToString() + "...";

				if (tgiOffsetEnd == TXTC.Length)
				{
					TXTC = null;
					return true;
				}
				else
				{
					TXTC = null;
					return false;
				}

			}

			return true;
		}

		public PackageType getType(string filename)
		{
			Stream input = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
			this.pType = getType(input);
			input.Close();

			return this.pType;
		}

		public PackageType getType(Stream input)
		{
			Database db = new Database(input, true, false);
			return getType(db, false);
		}

		public PackageType getType(Database db, bool loopAll)
		{
			// Do some quick sanity checks
			switch (db.dbpf.packageType)
			{
				case PackageTypes.genericPackage:
					break;
				case PackageTypes.corruptBadDownload:
				case PackageTypes.corruptChaavik:
				case PackageTypes.corruptIndex:
				case PackageTypes.corruptPeggy:
				case PackageTypes.corruptNotADBPF:
				case PackageTypes.corruptTXTC:
					this.isCorrupt = true;
					this.pType.SubType = "";
					this.pType.MainType = db.dbpf.packageType;
					return this.pType;
				case PackageTypes.sims3Store:
				case PackageTypes.sims2Package:
				case PackageTypes.pngThumbnail:
					this.pType.SubType = "";
					this.pType.MainType = db.dbpf.packageType;
					return this.pType;
			}

			rc.Clear();
			this.pType = new PackageType();

			//print(db.dbpf.Entries.Count + " entries found");
			for (int i = 0; i < db.dbpf.Entries.Count; i++)
			{
				DatabasePackedFile.Entry entry = db.dbpf.Entries[i];

				if ((entry.Key.typeId == (uint)ResourceTypes.NULL) && (entry.Key.groupId == (uint)ResourceTypes.NULL) && (entry.Key.instanceId == (uint)ResourceTypes.NULL))
				{
					// Check the first 4 bytes of the stream
					Stream checkDbpf = db.GetResourceStream(entry.Key);
					string magic = MadScience.StreamHelpers.ReadStringASCII(checkDbpf, 4);
					if (magic == "DBPF" || magic == "DBBF") // DBPF & DBBF
					{
						this.isCorrupt = true;
						this.pType.MainType = PackageTypes.corruptRecursive;
						this.pType.SubType = "This package contains another package inside it.";
						if (!loopAll) return this.pType;
					}
					checkDbpf.Close();
				}

				if (entry.Key.typeId == (uint)ResourceTypes.TXTC)
				{
					int isValid = checkValidEntry(entry, db);
					if (isValid > 0)
					{
						if (isValid == 2)
						{
							this.isCorrupt = true;
							this.pType.MainType = PackageTypes.corruptTXTC;
							if (!loopAll) return this.pType;
						}
					}
				}

				if (entry.Key.typeId == (uint)ResourceTypes.PTRN)
				{
					if (this.pType.MainType == PackageTypes.genericPackage) this.pType.MainType = PackageTypes.patternGeneric;
					if (!loopAll) return this.pType;
				}

				if (Enum.IsDefined(typeof(ResourceTypes), entry.Key.typeId))
				{
					if (rc.ContainsKey(Enum.GetName(typeof(ResourceTypes), entry.Key.typeId)))
					{
						rc[Enum.GetName(typeof(ResourceTypes), entry.Key.typeId)]++;
					}
					else
					{
						rc.Add(Enum.GetName(typeof(ResourceTypes), entry.Key.typeId), 1);
					}
				}

			}

			//print("Done");

			if (rc.ContainsKey("WLOT") && rc.ContainsKey("UNKW1"))
			{
				if (this.pType.MainType == PackageTypes.genericPackage) this.pType.MainType = PackageTypes.neighbourhood;
				this.isCorrupt = true;
				return this.pType;
			}

			if (rc.ContainsKey("WLTL") && rc.ContainsKey("ARY2"))
			{
				this.pType.MainType = PackageTypes.lot;
				return this.pType;
			}

			if (rc.ContainsKey("SIMO") && rc.ContainsKey("SIME") && rc.ContainsKey("SNAP") && rc.ContainsKey("SNAPL"))
			{
				this.pType.MainType = PackageTypes.sim;
				return this.pType;
			}

			//this.pType.MainType = PackageTypes.genericPackage;
			// Check Objects
			if (rc.ContainsKey("OBJD"))
			{
				if (this.pType.MainType == PackageTypes.genericPackage) this.pType.MainType = PackageTypes.objectGeneric;
				Stream objStream = MadScience.Package.Search.getStream(db, 0x319E4F1D, -1, -1);
				if (StreamHelpers.isValidStream(objStream))
				{
					OBJD objd = new OBJD(objStream);
					this.pType.SubType = objd.ToString();
					objd = null;
					
				}
				return this.pType;
			}
			if (rc.ContainsKey("S3SA"))
			{
				if (this.pType.MainType == PackageTypes.genericPackage)	this.pType.MainType = PackageTypes.coremod;
			}

			if (rc.ContainsKey("CASP"))
			{
				if (this.pType.MainType == PackageTypes.genericPackage) this.pType.MainType = PackageTypes.casPartGeneric;

				Stream casStream = MadScience.Package.Search.getStream(db, 0x034AEECB, -1, -1);
				if (StreamHelpers.isValidStream(casStream))
				{
					casPartFile cFile = new casPartFile();
					cFile.Load(casStream);

					this.pType.SubType = cFile.clothingType();

					switch (cFile.casType())
					{
						case "Hair":
							this.pType.MainType = PackageTypes.casPartHair;
							break;
						case "Scalp":
							break;
						case "Face Overlay":
							switch (cFile.clothingType())
							{
								case "Lipstick":
								case "Eyeshadow":
								case "Eyeliner":
								case "Blush":
								case "Makeup":
								case "Mascara":
									this.pType.MainType = PackageTypes.casPartMakeup;
									break;
								default:
									this.pType.MainType = PackageTypes.casPartFaceOverlay;
									break;
							}
							break;
						case "Body":
							this.pType.MainType = PackageTypes.casPartClothing;
							this.pType.SubType = cFile.clothingCategory();

							// Check the TYPE of clothing we have
							switch (cFile.clothingType())
							{
								case "Body":
								case "Top":
								case "Bottom":
								case "Shoes":
									// Check the age too
									// If we have Toddler OR Child OR Teen, plus other ages
									bool ageCorrupt = false;
									//if ((cFile.cFile.ageGender.baby || cFile.cFile.ageGender.toddler || cFile.cFile.ageGender.child || cFile.cFile.ageGender.teen) && (cFile.cFile.ageGender.youngAdult || cFile.cFile.ageGender.adult || cFile.cFile.ageGender.elder))
									//{
									//	ageCorrupt = true;
									//}
									// If we have Baby AND any other age...
									if (cFile.cFile.ageGender.baby && (cFile.cFile.ageGender.toddler || cFile.cFile.ageGender.child || cFile.cFile.ageGender.teen || cFile.cFile.ageGender.youngAdult || cFile.cFile.ageGender.adult || cFile.cFile.ageGender.elder))
									{
										ageCorrupt = true;
									}
									// If we have Toddler AND any other age...
									if (cFile.cFile.ageGender.toddler && (cFile.cFile.ageGender.child || cFile.cFile.ageGender.teen || cFile.cFile.ageGender.youngAdult || cFile.cFile.ageGender.adult || cFile.cFile.ageGender.elder))
									{
										ageCorrupt = true;
									}
									// If we have Child AND any other age
									if (cFile.cFile.ageGender.child && (cFile.cFile.ageGender.teen || cFile.cFile.ageGender.youngAdult || cFile.cFile.ageGender.adult || cFile.cFile.ageGender.elder))
									{
										ageCorrupt = true;
									}
									// If we have Teen AND any other age
									if (cFile.cFile.ageGender.teen && (cFile.cFile.ageGender.youngAdult || cFile.cFile.ageGender.adult || cFile.cFile.ageGender.elder))
									{
										ageCorrupt = true;
									}

									if (ageCorrupt)
									{
										this.isCorrupt = true;
										this.pType.MainType = PackageTypes.corruptBadAges;
										if (!loopAll) return this.pType;
									}
									break;
								default:
									break;
							}

							break;
						case "Accessory":
							this.pType.MainType = PackageTypes.casPartAccessory;
							break;
					}
					this.pType.SubType += " (" + cFile.ageGender() + ")";
					
				}
				return this.pType;
			}

			if (rc.ContainsKey("FBLN") && rc.ContainsKey("FACE") && rc.ContainsKey("BOND"))
			{
				if (this.pType.MainType == PackageTypes.genericPackage) this.pType.MainType = PackageTypes.casSlider;
				return this.pType;
			}

			if (rc.ContainsKey("_IMG") && rc.Count == 1)
			{
				if (this.pType.MainType == PackageTypes.genericPackage) this.pType.MainType = PackageTypes.textureReplacement;
			}

			if (rc.Count == 1 && (rc.ContainsKey("_XML") || rc.ContainsKey("_XML2")))
			{
				if (this.pType.MainType == PackageTypes.genericPackage) this.pType.MainType = PackageTypes.xmltuningmod;
			}

			return this.pType;
		}

		public Dictionary<string, uint> Dictionary()
		{
			return this.rc;
		}

		private void print(string message)
		{
			Console.WriteLine( message );
		}
	}
}
