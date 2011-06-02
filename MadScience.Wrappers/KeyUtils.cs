using System;
using System.Collections.Generic;
using System.IO;

namespace MadScience
{
	public class keyName
	{
		public uint typeId = 0;
		public uint groupId = 0;
		public ulong instanceId = 0;
		public string name = "";

		public keyName()
		{
		}

		public string Blank = "key:00000000:00000000:0000000000000000";

		/*
		public keyName(tgi64 tgi)
		{
			this.typeId = tgi.typeid;
			this.groupId = tgi.groupid;
			this.instanceId = tgi.instanceid;
		}
		*/

		public keyName(string keyString)
		{
			keyString = keyString.Replace("key:", "");
			string[] temp = keyString.Split(":".ToCharArray());


			this.typeId = MadScience.StringHelpers.ParseHex32("0x" + temp[0]);
			this.groupId = MadScience.StringHelpers.ParseHex32("0x" + temp[1]);
			this.instanceId = MadScience.StringHelpers.ParseHex64("0x" + temp[2]);

		}

		public keyName(string keyString, string meshName)
		{
			keyString = keyString.Trim();
			if (keyString != "")
			{
				if (keyString.StartsWith("key:"))
				{
					keyString = keyString.Replace("key:", "");
					string[] temp = keyString.Split(":".ToCharArray());

					this.typeId = MadScience.StringHelpers.ParseHex32("0x" + temp[0]);
					this.groupId = MadScience.StringHelpers.ParseHex32("0x" + temp[1]);
					this.instanceId = MadScience.StringHelpers.ParseHex64("0x" + temp[2]);
				}
				else
				{
					if (keyString.Length == 16)
					{
						this.typeId = 0x00B2D882;
						this.groupId = 0x0;
						this.instanceId = MadScience.StringHelpers.ParseHex64("0x" + keyString);
					}
					else
					{
						this.typeId = 0x00B2D882;
						this.groupId = 0x0;
						this.name = meshName;
						this.instanceId = MadScience.StringHelpers.HashFNV64(meshName);
					}
				}
			}
		}

		public keyName(uint typeId, uint groupId, ulong iId, string kName)
		{
			this.typeId = typeId;
			this.groupId = groupId;
			this.instanceId = iId;
			this.name = kName;
		}

		public keyName(uint typeId, uint groupId, ulong iId)
		{
			this.typeId = typeId;
			this.groupId = groupId;
			this.instanceId = iId;
		}

		public keyName(uint typeId, ulong iId, string kName)
		{
			this.typeId = typeId;
			this.instanceId = iId;
			this.name = kName;
		}

		public keyName(ulong iId, string kName)
		{
			this.instanceId = iId;
			this.name = kName;
		}

		public keyName(uint typeId, uint groupId, string kName)
		{
			this.typeId = typeId;
			this.groupId = groupId;
			this.name = kName;
			this.instanceId = MadScience.StringHelpers.HashFNV64(kName);
		}

		public override string ToString()
		{
			return "key:" + this.typeId.ToString("X8") + ":" + this.groupId.ToString("X8") + ":" + this.instanceId.ToString("X16");
		}

		/*
		public tgi64 ToTGI()
		{
			tgi64 temp = new tgi64();
			temp.typeid = this.typeId;
			temp.groupid = this.groupId;
			temp.instanceid = this.instanceId;
			return temp;
		}
		*/

		/*
		public MadScience.Wrappers.ResourceKey ToResourceKey()
		{
			MadScience.Wrappers.ResourceKey temp = new MadScience.Wrappers.ResourceKey();
			temp.TypeId = this.typeId;
			temp.GroupId = this.groupId;
			temp.InstanceId = this.instanceId;
			return temp;
		}
		*/

		public MadScience.Wrappers.ResourceKey ToResourceKey()
		{
			return new MadScience.Wrappers.ResourceKey(this.typeId, this.groupId, this.instanceId);
		}
	}

	public class KeyUtils
	{
		#region Key Validation and Searching
		public static bool validateKey(string keyString)
		{
			return validateKey(keyString, true);
		}

		public static bool validateKey(string keyString, bool showMessage)
		{
			bool retVal = true;

			if (String.IsNullOrEmpty(keyString))
			{
				return false;
			}
			if (!keyString.StartsWith("key:")) retVal = false;
			if (!keyString.Contains(":")) retVal = false;
			string[] temp = keyString.Split(":".ToCharArray());
			if (temp.Length < 4) retVal = false;

			if (!retVal)
			{
				if (showMessage) { System.Windows.Forms.MessageBox.Show("Key is not in the correct format!"); }
				return false;
			}
			else
			{
				return true;
			}
		}

		public static Stream searchForKey(string keyString, string filename)
		{

			// Validate keystring
			if (validateKey(keyString) == false)
			{
				return null;
			}

			if (!File.Exists(filename))
			{
				return null;
			}

			keyName tKey = new keyName(keyString);
			Stream tStream = null;

			Stream input = File.OpenRead(filename);
			MadScience.Wrappers.Database db = new MadScience.Wrappers.Database(input, true);

			try
			{

				tStream = db.GetResourceStream(tKey.ToResourceKey());
			}
			catch (System.Collections.Generic.KeyNotFoundException ex)
			{
				Helpers.logMessageToFile(ex.Message);
			}
			catch (Exception ex)
			{
				Helpers.logMessageToFile(ex.Message);
			}

			input.Close();

			return tStream;
		}

		public static void findAndShowImage(string keyName)
		{

			Stream foundChunk = findKey(keyName);
			if (foundChunk != null && foundChunk.Length != 0)
			{
				DDSPreview ddsP = new DDSPreview();
				ddsP.loadDDS(foundChunk);
				ddsP.ShowDialog();
			}
			else
			{
				System.Windows.Forms.MessageBox.Show("Could not find an image with " + keyName + "!");
			}
		}

		public static Stream findKey(string keyName)
		{
			return findKey(keyName, 2);
		}

		public static Stream findKey(string keyName, int fullBuildNum)
		{
			if (KeyUtils.validateKey(keyName) == false) return null;

			List<MadScience.Wrappers.ResourceKey> resourceKeys = new List<MadScience.Wrappers.ResourceKey>();
			resourceKeys.Add(new MadScience.Wrappers.ResourceKey(keyName));

			return findKey(resourceKeys, fullBuildNum, null)[0];
		}

		public static Stream findKey(MadScience.Wrappers.ResourceKey resourceKey)
		{
			return findKey(resourceKey, 2);
		}

		public static Stream findKey(MadScience.Wrappers.ResourceKey resourceKey, int fullBuildNum)
		{
			List<MadScience.Wrappers.ResourceKey> resourceKeys = new List<MadScience.Wrappers.ResourceKey>();
			resourceKeys.Add(resourceKey);

			return findKey(resourceKeys, fullBuildNum, null)[0];
		}

		public static Stream findKey(MadScience.Wrappers.ResourceKey resourceKey, int fullBuildNum, Wrappers.Database db)
		{
			List<MadScience.Wrappers.ResourceKey> resourceKeys = new List<MadScience.Wrappers.ResourceKey>();
			resourceKeys.Add(resourceKey);

			return findKey(resourceKeys, fullBuildNum, db)[0];
		}

		public static List<Stream> findKey(List<MadScience.Wrappers.ResourceKey> resourceKeys, int fullBuildNum)
		{
			return findKey(resourceKeys, fullBuildNum, null);
		}

		public static List<Stream> findKey(List<MadScience.Wrappers.ResourceKey> resourceKeys, int fullBuildNum, MadScience.Wrappers.Database db)
		{
			List<Stream> tempChunks = new List<Stream>();

			// Add one Stream per keyName, even if null
			for (int i = 0; i < resourceKeys.Count; i++)
			{
				tempChunks.Add(new MemoryStream());
			}

			// Check local files first
			if (Helpers.localFiles.Count > 0)
			{

				for (int i = 0; i < resourceKeys.Count; i++)
				{
					if (Helpers.localFiles.ContainsKey(resourceKeys[i].ToString()))
					{
						Stream blah = File.OpenRead((string)Helpers.localFiles[resourceKeys[i].ToString()]);
						StreamHelpers.CopyStream(blah, tempChunks[i]);
						tempChunks[i].Seek(0, SeekOrigin.Begin);
						blah.Close();
					}
				}

			}

			//check current package
			if (!String.IsNullOrEmpty(Helpers.currentPackageFile))
			{
				Stream localPackage = File.Open(Helpers.currentPackageFile, FileMode.Open, FileAccess.Read, FileShare.Read);
				MadScience.Wrappers.Database localDb = new MadScience.Wrappers.Database(localPackage, true);

				for (int i = 0; i < resourceKeys.Count; i++)
				{
					if (tempChunks[i].Length == 0)
					{
						try
						{
							tempChunks[i] = localDb.GetResourceStream(resourceKeys[i]);
						}
						catch (System.Collections.Generic.KeyNotFoundException)
						{
							//Helpers.logMessageToFile(ex.Message);
						}
						catch (Exception)
						{
							//Helpers.logMessageToFile(ex.Message);
						}
					}
				}

				localPackage.Close();
			}

			//check global packages
			foreach (string filename in Helpers.globalPackageFiles)
			{
				if (!String.IsNullOrEmpty(filename))
				{
					using (Stream localPackage = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
					{
						MadScience.Wrappers.Database localDb = new MadScience.Wrappers.Database(localPackage, true);

						for (int i = 0; i < resourceKeys.Count; i++)
						{
							if (tempChunks[i].Length == 0)
							{
								try
								{
									tempChunks[i] = localDb.GetResourceStream(resourceKeys[i]);
								}
								catch (System.Collections.Generic.KeyNotFoundException)
								{
									//Helpers.logMessageToFile(ex.Message);
								}
								catch (Exception)
								{
									//Helpers.logMessageToFile(ex.Message);
								}
							}
						}

						localPackage.Close();
					}
				}
			}

			// If input stream isn't null then we use that, otherwise open the fullbuild we want...
			Stream input = Stream.Null;
			bool closeDb = false;
			if (db == null)
			{
				closeDb = true;
                try
                {
                    input = File.OpenRead(Path.Combine(Helpers.findSims3Root(), Helpers.getGameSubPath("\\GameData\\Shared\\Packages\\FullBuild" + fullBuildNum.ToString() + ".package")));
                    db = new MadScience.Wrappers.Database(input, true);
                }
                catch (Exception)
                {
                    return tempChunks;
                }
			}

			for (int i = 0; i < resourceKeys.Count; i++)
			{
				if (tempChunks[i].Length == 0)
				{

					//keyName tKey = new keyName(resourceKeys[i]);
					try
					{
						tempChunks[i] = db.GetResourceStream(resourceKeys[i]);
					}
					catch (System.Collections.Generic.KeyNotFoundException)
					{
						//Helpers.logMessageToFile(ex.Message);
					}
					catch (Exception)
					{
						//Helpers.logMessageToFile(ex.Message);
					}
				}
			}
			if (closeDb)
			{
				input.Close();
			}

			return tempChunks;
		}

		#endregion

	}
}


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
