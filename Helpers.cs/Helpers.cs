using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using Microsoft.Win32;
using System.Xml;
using System.Xml.Serialization;
//using System.Linq;
using System.IO;
//using Gibbed.Helpers;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.ComponentModel;

namespace MadScience
{

    [System.Xml.Serialization.XmlRootAttribute()]
    public class metaEntries
    {
        /// <remarks/>
        [System.Xml.Serialization.XmlElement("metaEntry", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = true)]
        public List<metaEntry> Items = new List<metaEntry>();
    }

    public class metaEntry
    {
        [XmlAttributeAttribute("key")]
        public uint key;

        [XmlAttributeAttribute("shortName")]
        public string shortName;

        [XmlAttributeAttribute("longName")]
        public string longName;

    }

    public class Helpers
    {

        public static Hashtable localFiles = new Hashtable();
        public static string currentPackageFile = "";

        public static BindingList<string> globalPackageFiles;

        static Helpers()
        {
            string filelist = MadScience.Helpers.getRegistryValue("globalPackageFiles");
            string[] arr = filelist.Split('?'); //The questionmark is the separator, because it can't be included in filenames
            globalPackageFiles = new BindingList<string>();
            foreach (string file in arr)
            {
                if (!String.IsNullOrEmpty(file))
                {
                    if (new FileInfo(file).Exists)
                    {
                        globalPackageFiles.Add(file);
                    }
                }
            }
        }

		public static void OpenWindow(string path)
		{
			if (!String.IsNullOrEmpty(path) && Directory.Exists(path) )
			{
				System.Diagnostics.Process.Start(path);
			}
			//If the above doesn't work try
			//System.Diagnostics.Process.Start("explorer.exe", path);
		}

        public static metaEntries metaEntryList;

        public static metaEntry findMetaEntry(uint key)
        {
            metaEntry temp = new metaEntry();
            for (int i = 0; i < metaEntryList.Items.Count; i++)
            {
                if (metaEntryList.Items[i].key == key)
                {
                    temp = metaEntryList.Items[i];
                    break;
                }
            }
            return temp;
        }

        public static void lookupTypes(string metaTypePath)
        {
            if (File.Exists(metaTypePath))
            {
                TextReader r = new StreamReader(metaTypePath);
                XmlSerializer s = new XmlSerializer(typeof(metaEntries));
                metaEntryList = (metaEntries)s.Deserialize(r);
                r.Close();
            }
        }

        //public static string productName = "";

        #region Registry Values Save / Load

        public static string getRegistryValue(string keyName)
        {
            string temp = "";
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\Mad Scientist Productions\\" + Application.ProductName, false);
            if (key != null)
            {
                if (key.GetValue(keyName) != null)
                {
                    temp = key.GetValue(keyName).ToString();
                }
            }
            return temp;
        }

        public static string getCommonRegistryValue(string keyName)
        {
            string temp = "";
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\Mad Scientist Productions", false);
            if (key != null)
            {
                if (key.GetValue(keyName) != null)
                {
                    temp = key.GetValue(keyName).ToString();
                }
            }
            return temp;
        }

        public static void saveRegistryValue(string keyName, string value)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\Mad Scientist Productions\\" + Application.ProductName, true);
            if (key == null)
            {
                key = Registry.CurrentUser.CreateSubKey("Software\\Mad Scientist Productions\\" + Application.ProductName);
            }
            key.SetValue(keyName, value);
            key.Close();
            
        }

        public static void saveCommonRegistryValue(string keyName, string value)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\Mad Scientist Productions", true);
            if (key == null)
            {
                key = Registry.CurrentUser.CreateSubKey("Software\\Mad Scientist Productions");
            }
            key.SetValue(keyName, value);
            key.Close();

        }

		public static List<ListViewItem> messages = new List<ListViewItem>();

		public static void showMessage(string message)
		{
			showMessage(message, Color.White);
			//messages.AppendLine(message);
		}

		public static void showMessage(string message, Color backColor)
		{
			//messages.AppendLine(message);
			ListViewItem item = new ListViewItem();
			item.Text = message;
			item.BackColor = backColor;
			messages.Add(item);
		}

		public static bool hasFramework(string checkPath, GameNumber epNumber)
		{
			bool retVal = false;

			if (String.IsNullOrEmpty(checkPath)) return false;

			switch (epNumber)
			{
				case GameNumber.highEndLoftStuff:
					retVal = hasFramework(checkPath, "FullBuild_p03");
					break;
				case GameNumber.worldAdventures:
					retVal = hasFramework(checkPath, "FullBuildEP1");
					break;
				case GameNumber.baseGame:
				default:
					retVal = hasFramework(checkPath, "FullBuild0");
					break;
			}
			return retVal;
		}

		public static bool hasFramework(string checkPath, string fullBuild)
		{

			if (String.IsNullOrEmpty(checkPath)) return false;

			showMessage("Checking path " + checkPath);

			// Check for resource.cfg presence
			bool hasFoundResource = false;
			if (File.Exists(checkPath + "\\resource.cfg"))
			{
				hasFoundResource = true;
				showMessage("Found resource.cfg: Yes");
			}
			else
			{
				showMessage("Found resource.cfg: No", Color.Salmon);
			}

			// Check for Mods\\Packages folder
			bool hasFoundModsPackages = false;
			if (Directory.Exists(checkPath + "\\Mods\\Packages\\"))
			{
				hasFoundModsPackages = true;
				showMessage(@"Found Mods\Packages folder: Yes");
			}
			else
			{
				showMessage(@"Found Mods\Packages folder: No", Color.Salmon);
			}

			if (File.Exists(Path.Combine(checkPath, MadScience.Helpers.getGameSubPath("\\GameData\\Shared\\Packages\\" + fullBuild + ".package"))))
			{
				showMessage(@"Found " + fullBuild + ": Yes");
			}
			else
			{
				showMessage(@"Found " + fullBuild + ": No", Color.Salmon);
			}

			bool hasFoundDLLFramework = false;
			if (File.Exists(Path.Combine(checkPath, MadScience.Helpers.getGameSubPath("\\Game\\Bin\\d3dx9_31.dll"))))
			{
				showMessage(@"Found custom framework d3dx9_31.dll: Yes");
				hasFoundDLLFramework = true;
			}
			else
			{
				showMessage(@"Found custom framework d3dx9_31.dll: No", Color.Salmon);
			}

			//textBox1.Text += Environment.NewLine;

			if (!hasFoundModsPackages && !hasFoundResource)
			{
				showMessage(@"The Installer could not find either a Resource.cfg or a Mods\Packages folder! This means that your game WILL NOT currently accept custom content and installation via the Helper Monkey", Color.Salmon);
				showMessage(@"Please install the Framework by clicking the Install button.");
			}
			if (!hasFoundModsPackages && hasFoundResource)
			{
				showMessage(@"The Installer found a Resource.cfg but not the Mods\Packages folder!  This means that your game WILL NOT currently accept custom content, and installation via the Helper Monkey will fail.", Color.Salmon);
				showMessage(@"Please install the Framework by clicking the Install button.");
			}
			if (hasFoundModsPackages && !hasFoundResource)
			{
				showMessage(@"The Installer found a Mods\Packages folder but not Resource.cfg!  This means that your game WILL NOT currently accept custom content, and any custom packages will not show up in game.", Color.Salmon);
				showMessage(@"Please install the Framework by clicking the Install button.");
			}
			if (hasFoundModsPackages && hasFoundResource && !hasFoundDLLFramework)
			{
				showMessage(@"The Installer found both a Mods\Packages folder and the Resource.cfg, but NOT a custom DLL.  This means that your game SHOULD accept basic custom content, but NOT core mods.", Color.Salmon);
			}
			if (hasFoundModsPackages && hasFoundResource && hasFoundDLLFramework)
			{
				showMessage(@"The Installer found all Framework components.  This means that your game SHOULD accept basic custom content, and also core mods.");
				return true;
			}

			return false;

}

        public static string findMyDocs()
        {
            string myDocuments = "";

            string path32 = @"Software\Microsoft\Windows\CurrentVersion\Explorer\User Shell Folders";
            string path64 = @"Software\Wow6432Node\Microsoft\Windows\CurrentVersion\Explorer\User Shell Folders";

            try
            {
                RegistryKey key;
                key = Registry.CurrentUser.OpenSubKey(path32, false);
                if (key == null)
                {
                    // No Key exists... check 64 bit location
                    key = Registry.CurrentUser.OpenSubKey(path64, false);
                    if (key == null)
                    {
                        key.Close();
                        return "";
                    }
                }
                myDocuments = key.GetValue("Personal").ToString();
                key.Close();
            }
            catch (Exception)
            {
                //MessageBox.Show(e.Message);
            }

			if (myDocuments == "")
			{
				myDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			}

            return myDocuments;
        }

        public static void setSims3Root()
        {
			setSims3Root("", "FullBuild0");
        }

		private static Version getSKUVersion(string gamePath)
		{
			Version skuVersion = new Version();

			string skuPath = Path.Combine(Path.Combine(gamePath, Path.Combine("Game", "Bin")), "skuversion.txt");

			if (File.Exists(skuPath))
			{
				StreamReader reader = new StreamReader(skuPath);
				string input = "";
				input = reader.ReadLine();
				if (!String.IsNullOrEmpty(input))
				{
					if (input.StartsWith("GameVersion"))
					{
						input = input.Replace("GameVersion = ", "").Trim();
						skuVersion = new Version(input);
					}
				}
				reader.Close();
			}

			return skuVersion;
		}

		public static void findInstalledGames()
		{
			string sims3root = findSims3Root();
			if (sims3root == "")
			{
				return;
			}

			GameInfo gameInfo = new GameInfo();

			gameInfo.gameName = "Sims 3";
			gameInfo.path = sims3root;
			gameInfo.isInstalled = true;
			gameInfo.version = getSKUVersion(sims3root);

			gamesInstalled.gameCheck += 1;

			if (hasFramework(sims3root, GameNumber.baseGame))
			{
				gameInfo.hasFramework = true;
				gamesInstalled.flagCheck += 1;
			}
			
			gamesInstalled.Items.Add(gameInfo);

			gameInfo = new GameInfo();
			gameInfo.gameName = "World Adventures";

			string waLocation = findSims3Root(GameNumber.worldAdventures, false);
			if (!String.IsNullOrEmpty(waLocation))
			{
				gameInfo.path = waLocation;
				gameInfo.isInstalled = true;
				gameInfo.version = getSKUVersion(waLocation);
				gamesInstalled.gameCheck += 2;

				if (hasFramework(waLocation, GameNumber.worldAdventures))
				{
					gameInfo.hasFramework = true;
					gamesInstalled.flagCheck += 2;
				}
			}
			gamesInstalled.Items.Add(gameInfo);

			gameInfo = new GameInfo();
			gameInfo.gameName = "High End Loft Stuff";

			string helsLocation = findSims3Root(GameNumber.highEndLoftStuff, false);
			if (!String.IsNullOrEmpty(helsLocation))
			{
				gameInfo.path = helsLocation;
				gameInfo.isInstalled = true;
				gameInfo.version = getSKUVersion(helsLocation);
				gamesInstalled.gameCheck += 4;

				if (hasFramework(helsLocation, GameNumber.highEndLoftStuff))
				{
					gameInfo.hasFramework = true;
					gamesInstalled.flagCheck += 4;
				}
			}
			gamesInstalled.Items.Add(gameInfo);

			return;
		}

		public class GameInfo
		{
			public string gameName = "";
			public string path = "";
			public Version version = new Version();
			public bool isInstalled = false;
			public bool hasFramework = false;
		}

		public class gamesInstalled
		{
			public static List<GameInfo> Items = new List<GameInfo>();
			public static int gameCheck = 0;
			public static int flagCheck = 0;
		}

		public enum GameNumber : uint
		{
			baseGame = 0,
			worldAdventures = 1,
			highEndLoftStuff = 2,
		}

		public static void setSims3Root(GameNumber epNumber)
		{
			switch (epNumber)
			{
				case GameNumber.highEndLoftStuff:
					setSims3Root(" High End Loft Stuff", "FullBuild_p03");
					break;
				case GameNumber.worldAdventures:
					setSims3Root(" World Adventures", "FullBuildEP1");
					break;
				case GameNumber.baseGame:
				default:
					setSims3Root("", "FullBuild0");
					break;
			}
			
		}

		public static void setSims3Root(string epSuffix, string fullBuild)
		{
			System.Windows.Forms.FolderBrowserDialog fBrowse = new System.Windows.Forms.FolderBrowserDialog();
			fBrowse.Description = @"Please find your Sims 3" + epSuffix + @" root (usually C:\Program Files\Electronic Arts\The Sims 3" + epSuffix + "\\)";
			if (fBrowse.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				if (fBrowse.SelectedPath != "")
				{
					Helpers.saveCommonRegistryValue("sims3root" + epSuffix, fBrowse.SelectedPath);
					if (File.Exists(Path.Combine(fBrowse.SelectedPath, Helpers.getGameSubPath("\\GameData\\Shared\\Packages\\" + fullBuild + ".package"))))
					{
						//sims3root = fBrowse.SelectedPath;
						sims3folders[epSuffix] = fBrowse.SelectedPath;
						//return fBrowse.SelectedPath;
					}
					else
					{
						//return "";
					}

				}

			}
		}

        public static string getGameSubPath(string subPath)
        {
            if (subPath.IndexOf("\\") == -1) return subPath;
            string[] temp = subPath.Split("\\".ToCharArray());
            string ret = "";
            foreach (string p in temp)
            {
                if (!String.IsNullOrEmpty(p)) ret = Path.Combine(ret, p);
                
            }

            return ret;
        }

		private static Dictionary<string, string> sims3folders = new Dictionary<string, string>();

		//private static string sims3root = "";



		public static string findSims3Root()
		{
			return findSims3Root("", "FullBuild0", true);
		}

		public static string findSims3Root(GameNumber epNumber)
		{
			return findSims3Root(epNumber, true);
		}

		public static string findSims3Root(GameNumber epNumber, bool showManualPath)
		{
			string retVal = "";
			switch (epNumber)
			{
				case GameNumber.highEndLoftStuff:
					retVal = findSims3Root(" High-End Loft Stuff", "FullBuild_p03", showManualPath);
					break;
				case GameNumber.worldAdventures:
					retVal = findSims3Root(" World Adventures", "FullBuildEP1", showManualPath);
					break;
				case GameNumber.baseGame:
				default:
					retVal = findSims3Root("", "FullBuild0", showManualPath);
					break;
			}
			return retVal;
		}



		public static string findSims3Root(string epSuffix, string fullBuild, bool showManualPath)
		{
			//string path32 = "Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\{C05D8CDB-417D-4335-A38C-A0659EDFD6B8}";
			//string path64 = "Software\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\{C05D8CDB-417D-4335-A38C-A0659EDFD6B8}"

			if (sims3folders.ContainsKey(epSuffix))
			//if (sims3root != "")
			{
				if (sims3folders[epSuffix] != "")
				{
					//Console.WriteLine("Returning already stored path");
					return sims3folders[epSuffix];
				}
				//return sims3root;
			}

			string installLocation = "";
			try
			{

				//Console.WriteLine("Attempting to get path from common registry...");
				installLocation = Helpers.getCommonRegistryValue("sims3root" + epSuffix);
				if (!String.IsNullOrEmpty(installLocation))
				{
					// Check install location just in case...
					try
					{
						if (File.Exists(Path.Combine(installLocation, Helpers.getGameSubPath("\\GameData\\Shared\\Packages\\" + fullBuild + ".package"))))
						{
							//Helpers.saveCommonRegistryValue("sims3root", installLocation);
							sims3folders[epSuffix] = installLocation;
							//sims3root = installLocation;
							return installLocation;
						}
					}
					catch (DirectoryNotFoundException dex)
					{
					}
					catch (FileNotFoundException fex)
					{
					}
				}

				//Console.WriteLine("Attempting to get path from registry...");
				string path32 = "Software\\Sims\\The Sims 3" + epSuffix;
				string path64 = "Software\\Wow6432Node\\Sims\\The Sims 3" + epSuffix;

				string path32Alt = "Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\{C05D8CDB-417D-4335-A38C-A0659EDFD6B8}";
				string path64Alt = "Software\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\{C05D8CDB-417D-4335-A38C-A0659EDFD6B8}";

				RegistryKey key;
				key = Registry.LocalMachine.OpenSubKey(path32, false);
				if (key == null)
				{
					// Try Alt location
					key = Registry.LocalMachine.OpenSubKey(path32Alt, false);
					if (key == null)
					{

						// No Key exists... check 64 bit location
						key = Registry.LocalMachine.OpenSubKey(path64, false);
						if (key == null)
						{
							key = Registry.LocalMachine.OpenSubKey(path64Alt, false);
							if (key == null)
							{
								// Can't find Sims 3 root - uh oh!
								key.Close();
								return "";
							}
						}
					}
				}
				installLocation = key.GetValue("Install Dir").ToString();
				key.Close();
			}
			catch (Exception)
			{
				//MessageBox.Show(e.Message);
			}

			// Check to see if FullBuild0.package exists within this root
			bool getManualPath = false;
			if (!String.IsNullOrEmpty(installLocation))
			{
				try
				{
					if (File.Exists(Path.Combine(installLocation, Helpers.getGameSubPath("\\GameData\\Shared\\Packages\\" + fullBuild + ".package"))))
					{
						Helpers.saveCommonRegistryValue("sims3root" + epSuffix, installLocation);
						sims3folders[epSuffix] = installLocation;
						//sims3root = installLocation;
						return installLocation;
					}
					else
					{
						// No FullBuild0 found, have to get a manual path
						getManualPath = true;
					}
				}
				catch (DirectoryNotFoundException dex)
				{
					getManualPath = true;
				}
				catch (FileNotFoundException fex)
				{
					getManualPath = true;
				}
			}
			else
			{
				getManualPath = true;
			}

			// Show Manual Path is not set, but we can't find a valid install... so return nothing
			if (!showManualPath && getManualPath)
			{

				return checkPathOverrides(epSuffix, fullBuild);
				//return "";
			}

			if (showManualPath && getManualPath)
			{
				getManualPath = false;

				string overridePath = checkPathOverrides(epSuffix, fullBuild);
				if (String.IsNullOrEmpty(overridePath))
				{
					getManualPath = true;
				}

				// If we got to this point we have to show a dialog to the user asking them where to find the sims3root

				System.Windows.Forms.FolderBrowserDialog fBrowse = new System.Windows.Forms.FolderBrowserDialog();
				fBrowse.Description = @"Please find your Sims 3" + epSuffix + @" root (usually C:\Program Files\Electronic Arts\The Sims 3" + epSuffix + "\\) ";
				fBrowse.Description += "NOTE: This is NOT where your Sims3.exe file is!";
				if (fBrowse.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					if (fBrowse.SelectedPath != "")
					{
						try
						{
							if (File.Exists(Path.Combine(fBrowse.SelectedPath, Helpers.getGameSubPath("\\GameData\\Shared\\Packages\\" + fullBuild + ".package"))))
							{
								Helpers.saveCommonRegistryValue("sims3root" + epSuffix, fBrowse.SelectedPath);
								sims3folders[epSuffix] = fBrowse.SelectedPath;
								//sims3root = fBrowse.SelectedPath;
								return fBrowse.SelectedPath;
							}
							else
							{
								return "";
							}
						}
						catch (DirectoryNotFoundException dex)
						{
							getManualPath = true;
						}
						catch (FileNotFoundException fex)
						{
							getManualPath = true;
						}
					}

				}

			}

			//sims3root = installLocation;
			//sims3folders[epSuffix] = installLocation;
			return installLocation;
		}

		private static string checkPathOverrides(string epSuffix, string fullBuild)
		{
			// Check for existance of XML file in the Application.Startup folder - this can be used to override
			// paths where none can be found (ie on Macs)

			bool hasOverrides = false;
			string overridePath = "";
			string installLocation = "";

			if (File.Exists(Path.Combine(Application.StartupPath, "pathOverrides.xml")))
			{
				hasOverrides = true;
				overridePath = Path.Combine(Application.StartupPath, "pathOverrides.xml");
			}

			if (!hasOverrides)
			{
				string myDocs = findMyDocs();
				if (!String.IsNullOrEmpty(myDocs))
				{
					if (File.Exists(Path.Combine(myDocs, "pathOverrides.xml")))
					{
						hasOverrides = true;
						overridePath = Path.Combine(Application.StartupPath, "pathOverrides.xml");
					}
				}
			}

			if (hasOverrides)
			{

				Stream xmlStream = File.OpenRead(overridePath);
				XmlTextReader xtr = new XmlTextReader(xmlStream);

				while (xtr.Read())
				{
					if (xtr.NodeType == XmlNodeType.Element)
					{
						switch (xtr.Name)
						{
							case "path":
								xtr.MoveToAttribute("name");
								if (xtr.Value == "sims3root" + epSuffix)
								{
									installLocation = xtr.GetAttribute("location");
								}
								break;
						}
					}
				}

				xtr.Close();
				xmlStream.Close();

				if (!String.IsNullOrEmpty(installLocation))
				{
					try
					{
						if (File.Exists(Path.Combine(installLocation, Helpers.getGameSubPath("\\GameData\\Shared\\Packages\\" + fullBuild + ".package"))))
						{
							Helpers.saveCommonRegistryValue("sims3root" + epSuffix, installLocation);
							sims3folders[epSuffix] = installLocation;
							//sims3root = installLocation;
							return installLocation;
						}
					}
					catch (DirectoryNotFoundException dex)
					{
						return "";
					}
					catch (FileNotFoundException fex)
					{
						return "";
					}
				}

			}

			return installLocation;

		}

		#endregion


        #region License and other functions
        public static void checkAndShowLicense(string productName)
        {
            // Check for registry key
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\Mad Scientist Productions\\" + productName, true);
            if (key == null)
            {
                key = Registry.CurrentUser.CreateSubKey("Software\\Mad Scientist Productions\\" + productName);
            }

            if (key.GetValue("acceptLicense") == null || key.GetValue("acceptLicense").ToString() == "false")
            {
                licenseForm frm = new licenseForm();
                frm.ShowDialog();
                frm = null;
            }

            key.Close();
        }

        public static Dictionary<ulong, string> getKeyNames(Stream input)
        {
            Dictionary<ulong, string> temp = new Dictionary<ulong, string>();

            BinaryReader reader = new BinaryReader(input);
            reader.ReadUInt32();            
            //input.ReadValueU32();
            int count = reader.ReadInt32();
            //int count = input.ReadValueS32();
            for (int i = 0; i < count; i++)
            {
                ulong instanceId = reader.ReadUInt64();
                //ulong instanceId = input.ReadValueU64();
                uint nLength = reader.ReadUInt32();
                //uint nLength = input.ReadValueU32();
                temp.Add(instanceId, MadScience.StreamHelpers.ReadStringASCII(input, nLength));
            }

            return temp;
        }
        #endregion

        #region Logging functions
        private static string _logPath = "";
        public static string logPath()
        {
            return _logPath;
        }
        public static string logPath(string logPath, bool deleteLog)
        {
            if (File.Exists(logPath)) File.Delete(logPath);
            _logPath = logPath;
            return _logPath;
        }

        public static string logPath(string newLogPath)
        {
            return logPath(newLogPath, false);
        }

        public static void logMessageToFile(string msg)
        {
            if (_logPath == "") { return; }
            
            System.IO.StreamWriter sw = System.IO.File.AppendText(_logPath);
            try
            {
                string logLine = System.String.Format(
                    "{0:G}: {1}.", System.DateTime.Now, msg);
                sw.WriteLine(logLine);
            }
            finally
            {
                sw.Close();
            }
        }
        #endregion

        #region Image functions
        /*
        public static Image previewImage2(Image sourceImage, Color background, Color c1, Color c2, Color c3, Color c4)
        {
            unsafe {

            int height = sourceImage.Height;
            int width = sourceImage.Width;
            Bitmap newBitmap = new Bitmap(width, height);

            Bitmap sourceBitmap = new Bitmap(sourceImage);
            BitmapData originalData = sourceBitmap.LockBits(new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData newData = newBitmap.LockBits(new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            int pixelSize = 4;
            //byte[] readPixelData = sourceBitmap

            //MemoryStream ms = new MemoryStream();
            //bitmap.Save(ms, ImageFormat.Bmp);
            //ms.Seek(54, SeekOrigin.Begin);

            uint maxValue = 0;

            for (int y = 0; y < height; y++)
            {
                //get the data from the original image
                byte* oRow = (byte*)originalData.Scan0 + (y * originalData.Stride);

                //get the data from the new image
                byte* nRow = (byte*)newData.Scan0 + (y * newData.Stride);

                for (int x = 0; x < width; x++)
                {

                    //int readPixelOffset = (y * width * 4) + (x * 4);

                    int cred = 0;
                    int cgreen = 0;
                    int cblue = 0;
                    int calpha = 0;

                    if (c1 != Color.Empty) { cred = oRow[x * pixelSize + 2]; }
                    if (c2 != Color.Empty) { cgreen = oRow[x * pixelSize + 1]; }
                    if (c3 != Color.Empty) { cblue = oRow[x * pixelSize]; }
                    if (c4 != Color.Empty)
                    {
                        calpha = oRow[x * pixelSize + 3];
                        // Inverse the alpha
                        //calpha = (255 - calpha);
                    }

                    Color color = Color.Empty;
                    if (c4 != Color.Empty)
                    {
                        color = Color.FromArgb(calpha, cred, cgreen, cblue);
                    }
                    else
                    {
                        color = Color.FromArgb(cred, cgreen, cblue);
                    }

                    Color color2 = Color.Black;
                    float num3 = 0f;

                    if (c1 != Color.Empty)
                    {
                        maxValue = ((uint)((((((uint)Convert.ToSingle(1.00)) * 0xff) << 0x18) + ((((uint)Convert.ToSingle(0.00) * 0xff) << 0x10)) + ((((uint)Convert.ToSingle(0.00)) * 0xff) << 8))) + (((uint)Convert.ToSingle(0.00)) * 0xff));
                        if (maxValue == uint.MaxValue)
                        {
                            num3 = 1f;
                        }
                        else if ((maxValue & 0xff000000) == 0xff000000)
                        {
                            num3 = 0.003921569f * color.R;
                        }
                        else if ((maxValue & 0xff0000) == 0xff0000)
                        {
                            num3 = 0.003921569f * color.G;
                        }
                        else if ((maxValue & 0xff00) == 0xff00)
                        {
                            num3 = 0.003921569f * color.B;
                        }
                        else if ((maxValue & 0xff) == 0xff)
                        {
                            num3 = 0.003921569f * color.A;
                        }
                        color2 = Color.FromArgb(0xff, Math.Max(0, Math.Min(0xff, (int)((color2.R + (color2.R * -num3)) + (((int)(c1.R * (0.003921569f * c1.A))) * num3)))), Math.Max(0, Math.Min(0xff, (int)((color2.G + (color2.G * -num3)) + (((int)(c1.G * (0.003921569f * c1.A))) * num3)))), Math.Max(0, Math.Min(0xff, (int)((color2.B + (color2.B * -num3)) + (((int)(c1.B * (0.003921569f * c1.A))) * num3)))));

                    }
                    if (c2 != Color.Empty)
                    {
                        maxValue = ((0 * 0xff) << 0x18) + ((1 * 0xff) << 0x10) + ((0 * 0xff) << 8) + ((0 * 0xff));
                        if (maxValue == uint.MaxValue)
                        {
                            num3 = 1f;
                        }
                        else if ((maxValue & 0xff000000) == 0xff000000)
                        {
                            num3 = 0.003921569f * color.R;
                        }
                        else if ((maxValue & 0xff0000) == 0xff0000)
                        {
                            num3 = 0.003921569f * color.G;
                        }
                        else if ((maxValue & 0xff00) == 0xff00)
                        {
                            num3 = 0.003921569f * color.B;
                        }
                        else if ((maxValue & 0xff) == 0xff)
                        {
                            num3 = 0.003921569f * color.A;
                        }
                        color2 = Color.FromArgb(0xff, Math.Max(0, Math.Min(0xff, (int)((color2.R + (color2.R * -num3)) + (((int)(c2.R * (0.003921569f * c2.A))) * num3)))), Math.Max(0, Math.Min(0xff, (int)((color2.G + (color2.G * -num3)) + (((int)(c2.G * (0.003921569f * c2.A))) * num3)))), Math.Max(0, Math.Min(0xff, (int)((color2.B + (color2.B * -num3)) + (((int)(c2.B * (0.003921569f * c2.A))) * num3)))));
                    }
                    if (c3 != Color.Empty)
                    {
                        maxValue = ((0 * 0xff) << 0x18) + ((0 * 0xff) << 0x10) + ((1 * 0xff) << 8) + ((0 * 0xff));
                        if (maxValue == uint.MaxValue)
                        {
                            num3 = 1f;
                        }
                        else if ((maxValue & 0xff000000) == 0xff000000)
                        {
                            num3 = 0.003921569f * color.R;
                        }
                        else if ((maxValue & 0xff0000) == 0xff0000)
                        {
                            num3 = 0.003921569f * color.G;
                        }
                        else if ((maxValue & 0xff00) == 0xff00)
                        {
                            num3 = 0.003921569f * color.B;
                        }
                        else if ((maxValue & 0xff) == 0xff)
                        {
                            num3 = 0.003921569f * color.A;
                        }
                        color2 = Color.FromArgb(0xff, Math.Max(0, Math.Min(0xff, (int)((color2.R + (color2.R * -num3)) + (((int)(c3.R * (0.003921569f * c3.A))) * num3)))), Math.Max(0, Math.Min(0xff, (int)((color2.G + (color2.G * -num3)) + (((int)(c3.G * (0.003921569f * c3.A))) * num3)))), Math.Max(0, Math.Min(0xff, (int)((color2.B + (color2.B * -num3)) + (((int)(c3.B * (0.003921569f * c3.A))) * num3)))));

                    }
                    if (c4 != Color.Empty)
                    {
                        maxValue = ((0 * 0xff) << 0x18) + ((0 * 0xff) << 0x10) + ((0 * 0xff) << 8) + ((1 * 0xff));
                        if (maxValue == uint.MaxValue)
                        {
                            num3 = 1f;
                        }
                        else if ((maxValue & 0xff000000) == 0xff000000)
                        {
                            num3 = 0.003921569f * color.R;
                        }
                        else if ((maxValue & 0xff0000) == 0xff0000)
                        {
                            num3 = 0.003921569f * color.G;
                        }
                        else if ((maxValue & 0xff00) == 0xff00)
                        {
                            num3 = 0.003921569f * color.B;
                        }
                        else if ((maxValue & 0xff) == 0xff)
                        {
                            num3 = 0.003921569f * color.A;
                        }
                        color2 = Color.FromArgb(0xff, Math.Max(0, Math.Min(0xff, (int)((color2.R + (color2.R * -num3)) + (((int)(c4.R * (0.003921569f * c4.A))) * num3)))), Math.Max(0, Math.Min(0xff, (int)((color2.G + (color2.G * -num3)) + (((int)(c4.G * (0.003921569f * c4.A))) * num3)))), Math.Max(0, Math.Min(0xff, (int)((color2.B + (color2.B * -num3)) + (((int)(c4.B * (0.003921569f * c4.A))) * num3)))));
                    }

                    //Color color = pixel.GetPixel(i, j);
                    //Color color2 = pixel2.GetPixel(i, j);

                    nRow[x * pixelSize] = color2.B;
                    nRow[x * pixelSize + 1] = color2.G;
                    nRow[x * pixelSize + 2] = color2.R;
                    nRow[x * pixelSize + 3] = color2.A;
                    //ms.WriteByte(color2.B);
                    //ms.WriteByte(color2.G);
                    //ms.WriteByte(color2.R);
                    //ms.WriteByte(color2.A);
                    //bitmap.SetPixel(x, y, color2);

                }
            }

            //bitmap = new Bitmap(ms);

            newBitmap.UnlockBits(newData);
            sourceBitmap.UnlockBits(originalData);

            return newBitmap;
            }
        }

        private static void colourFill(Image mask, Image dst, Color c2, uint channel, bool blend)
        {
            FastPixel pixel = new FastPixel(mask as Bitmap);
            FastPixel pixel2 = new FastPixel(dst as Bitmap);
            pixel.Lock();
            pixel2.Lock();
            for (int i = 0; i < pixel.Width; i++)
            {
                for (int j = 0; j < pixel.Height; j++)
                {
                    Color color = pixel.GetPixel(i, j);
                    Color color2 = pixel2.GetPixel(i, j);
                    float num3 = 0f;
                    if (channel == uint.MaxValue)
                    {
                        num3 = 1f;
                    }
                    else if ((channel & 0xff000000) == 0xff000000)
                    {
                        num3 = 0.003921569f * color.R;
                    }
                    else if ((channel & 0xff0000) == 0xff0000)
                    {
                        num3 = 0.003921569f * color.G;
                    }
                    else if ((channel & 0xff00) == 0xff00)
                    {
                        num3 = 0.003921569f * color.B;
                    }
                    else if ((channel & 0xff) == 0xff)
                    {
                        num3 = 0.003921569f * color.A;
                    }
                    Color black = Color.Black;
                    if (!blend)
                    {
                        black = (num3 == 0f) ? color2 : Color.FromArgb(0xff, c2);
                    }
                    else
                    {
                        black = Color.FromArgb(0xff, Math.Max(0, Math.Min(0xff, (int)((color2.R + (color2.R * -num3)) + (((int)(c2.R * (0.003921569f * c2.A))) * num3)))), Math.Max(0, Math.Min(0xff, (int)((color2.G + (color2.G * -num3)) + (((int)(c2.G * (0.003921569f * c2.A))) * num3)))), Math.Max(0, Math.Min(0xff, (int)((color2.B + (color2.B * -num3)) + (((int)(c2.B * (0.003921569f * c2.A))) * num3)))));
                    }
                    pixel2.SetPixel(i, j, black);
                }
            }
            pixel.Unlock(false);
            pixel2.Unlock(true);
        }

        public static Image imagePreview(Image sourceImage, Color background, Color c1, Color c2, Color c3, Color c4)
        {
            Image destImage = new Bitmap(256, 256);
            Graphics.FromImage(destImage);

            Color white = Color.White;
            uint maxValue = 0;

            colourFill(sourceImage, destImage, background, 0, false);

            if (c1 != Color.Empty)
            {
                maxValue = ((uint)((((((uint)Convert.ToSingle(1.00)) * 0xff) << 0x18) + ((((uint)Convert.ToSingle(0.00) * 0xff) << 0x10)) + ((((uint)Convert.ToSingle(0.00)) * 0xff) << 8))) + (((uint)Convert.ToSingle(0.00)) * 0xff));
                white = Color.FromArgb(c1.A, c1.R, c1.G, c1.B);
                colourFill(sourceImage, destImage, white, maxValue, true);
            }
            if (c2 != Color.Empty)
            {
                maxValue = ((0 * 0xff) << 0x18) + ((1 * 0xff) << 0x10) + ((0 * 0xff) << 8) + ((0 * 0xff));
                white = Color.FromArgb(c2.A, c2.R, c3.G, c4.B);
                colourFill(sourceImage, destImage, white, maxValue, true);
            }
            if (c3 != Color.Empty)
            {
                maxValue = ((0 * 0xff) << 0x18) + ((0 * 0xff) << 0x10) + ((1 * 0xff) << 8) + ((0 * 0xff));
                white = Color.FromArgb(c3.A, c3.R, c3.G, c3.B);
                colourFill(sourceImage, destImage, white, maxValue, true);
            }
            if (c4 != Color.Empty)
            {
                maxValue = ((0 * 0xff) << 0x18) + ((0 * 0xff) << 0x10) + ((0 * 0xff) << 8) + ((1 * 0xff));
                white = Color.FromArgb(c4.A, c4.R, c4.G, c4.B);
                colourFill(sourceImage, destImage, white, maxValue, true);
            }

            return destImage;
        }
         */
        #endregion

		public static void resetControl(System.Windows.Forms.Control control)
		{
			if (control is System.Windows.Forms.TextBox)
			{
				control.Text = "";
			}
			if (control is System.Windows.Forms.ComboBox)
			{
				System.Windows.Forms.ComboBox cmb = (System.Windows.Forms.ComboBox)control;
				if (cmb.Items.Count > 0) cmb.SelectedIndex = 0;
			}
			if (control is System.Windows.Forms.CheckBox)
			{
				System.Windows.Forms.CheckBox chk = (System.Windows.Forms.CheckBox)control;
				chk.Checked = false;
			}
			if (control is System.Windows.Forms.CheckedListBox)
			{
				System.Windows.Forms.CheckedListBox chkList = (System.Windows.Forms.CheckedListBox)control;
				for (int i = 0; i < chkList.Items.Count; i++)
				{
					chkList.SetItemChecked(i, false);
				}
			}
			if (control is System.Windows.Forms.ListView)
			{
				System.Windows.Forms.ListView lstView = (System.Windows.Forms.ListView)control;
				lstView.Items.Clear();
			}

		}

        #region String functions
        public static string sanitiseString(string input)
        {
            string temp = Regex.Replace(input, "[^a-zA-Z0-9]", "");
            return temp;

            //var s = from ch in input where char.IsLetterOrDigit(ch) select ch;
            //return UnicodeEncoding.ASCII.GetString(UnicodeEncoding.ASCII.GetBytes(s.ToArray()));
        }
		public static string stripControlFromString(string input)
		{
			string temp = "";
			for (int i = 0; i < input.Length; i++)
			{
				if (!Char.IsControl(input[i])) temp += input[i];
			}
			//string temp = Regex.Replace(input, "[^a-zA-Z0-9]", "");
			return temp;

			//var s = from ch in input where char.IsLetterOrDigit(ch) select ch;
			//return UnicodeEncoding.ASCII.GetString(UnicodeEncoding.ASCII.GetBytes(s.ToArray()));
		}
        #endregion


        public static string numberToString(double num)
        {
            return String.Format(CultureInfo.InvariantCulture, "{0:0.0000000}", num); ;
        }


    }
}
