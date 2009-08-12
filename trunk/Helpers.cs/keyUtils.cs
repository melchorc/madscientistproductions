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

        public keyName(tgi64 tgi)
        {
            this.typeId = tgi.typeid;
            this.groupId = tgi.groupid;
            this.instanceId = tgi.instanceid;
        }

        public keyName(string keyString)
        {
            keyString = keyString.Replace("key:", "");
            string[] temp = keyString.Split(":".ToCharArray());

            this.typeId = Gibbed.Helpers.StringHelpers.ParseHex32("0x" + temp[0]);
            this.groupId = Gibbed.Helpers.StringHelpers.ParseHex32("0x" + temp[1]);
            this.instanceId = Gibbed.Helpers.StringHelpers.ParseHex64("0x" + temp[2]);

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

                    this.typeId = Gibbed.Helpers.StringHelpers.ParseHex32("0x" + temp[0]);
                    this.groupId = Gibbed.Helpers.StringHelpers.ParseHex32("0x" + temp[1]);
                    this.instanceId = Gibbed.Helpers.StringHelpers.ParseHex64("0x" + temp[2]);
                }
                else
                {
                    if (keyString.Length == 16)
                    {
                        this.typeId = 0x00B2D882;
                        this.groupId = 0x0;
                        this.instanceId = Gibbed.Helpers.StringHelpers.ParseHex64("0x" + keyString);
                    }
                    else
                    {
                        this.typeId = 0x00B2D882;
                        this.groupId = 0x0;
                        this.name = meshName;
                        this.instanceId = Gibbed.Helpers.StringHelpers.HashFNV64(meshName);
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
            this.instanceId = Gibbed.Helpers.StringHelpers.HashFNV64(kName);
        }

        public override string ToString()
        {
            return "key:" + this.typeId.ToString("X8") + ":" + this.groupId.ToString("X8") + ":" + this.instanceId.ToString("X16");
        }

        public tgi64 ToTGI()
        {
            tgi64 temp = new tgi64();
            temp.typeid = this.typeId;
            temp.groupid = this.groupId;
            temp.instanceid = this.instanceId;
            return temp;
        }

        public Gibbed.Sims3.FileFormats.ResourceKey ToResourceKey()
        {
            Gibbed.Sims3.FileFormats.ResourceKey temp = new Gibbed.Sims3.FileFormats.ResourceKey();
            temp.TypeId = this.typeId;
            temp.GroupId = this.groupId;
            temp.InstanceId = this.instanceId;
            return temp;
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

            if (keyString.Trim() == "")
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

        public static Stream searchForKey(string keyString)
        {
            return searchForKey(keyString, 2);
        }

        public static Stream searchForKey(string keyString, int fullBuild)
        {

            // Validate keystring
            if (validateKey(keyString) == false)
            {
                return null;
            }

            string sims3root = MadScience.Helpers.findSims3Root();
            if (sims3root == "")
            {
                return null;
            }

            // Split the input key
            keyName tKey = new keyName(keyString);
            Stream tStream = null;

            Stream input = File.OpenRead(sims3root + "\\GameData\\Shared\\Packages\\FullBuild" + fullBuild.ToString() + ".package");
            Gibbed.Sims3.FileFormats.Database db = new Gibbed.Sims3.FileFormats.Database(input, true);

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
            Gibbed.Sims3.FileFormats.Database db = new Gibbed.Sims3.FileFormats.Database(input, true);

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
        }

        public static Stream findKey(string keyName)
        {
            return findKey(keyName, 2);
        }

        public static Stream findKey(string keyName, int fullBuildNum)
        {
            List<string> keyNames = new List<string>();
            keyNames.Add(keyName);

            return findKey(keyNames, fullBuildNum)[0];
        }

        /*
        public static Stream findKey(string keyName, int fullBuildNum)
        {
            Stream tempChunk = new MemoryStream();

            bool hasMatch = false;

            // Check local files first
            if (newDDSFiles.Count > 0)
            {
                //DDSPreview ddsP = new DDSPreview();

                //for (int i = 0; i < newDDSFiles.Count; i++)
                //{
                if (KeyUtils.validateKey(keyName) == true && newDDSFiles.ContainsKey(keyName))
                {
                    Stream blah = File.OpenRead((string)newDDSFiles[keyName]);
                    Helpers.CopyStream(blah, tempChunk);
                    tempChunk.Seek(0, SeekOrigin.Begin);
                    blah.Close();
                    hasMatch = true;
                    //break;
                }

            }

            if (!hasMatch)
            {
                if (KeyUtils.validateKey(keyName) == true)
                {
                    tempChunk = searchInPackage(this.filename, keyName);
                    if (tempChunk == null)
                    {
                        tempChunk = searchForKey(keyName, fullBuildNum);
                    }
                }
            }

            return tempChunk;
        }
        */

        public static List<Stream> findKey(List<string> keyNames, int fullBuildNum)
        {
            List<Stream> tempChunks = new List<Stream>();

            // Add one Stream per keyName, even if null
            for (int i = 0; i < keyNames.Count; i++)
            {
                tempChunks.Add(new MemoryStream());
            }

            // Check local files first
            if (Helpers.localFiles.Count > 0)
            {

                for (int i = 0; i < keyNames.Count; i++)
                {
                    if (KeyUtils.validateKey(keyNames[i]) == true && Helpers.localFiles.ContainsKey(keyNames[i]))
                    {
                        Stream blah = File.OpenRead((string)Helpers.localFiles[keyNames[i]]);
                        Helpers.CopyStream(blah, tempChunks[i]);
                        tempChunks[i].Seek(0, SeekOrigin.Begin);
                        blah.Close();
                    }
                }

            }


            if (!String.IsNullOrEmpty(Helpers.currentPackageFile))
            {
                Stream localPackage = File.Open(Helpers.currentPackageFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                Gibbed.Sims3.FileFormats.Database localDb = new Gibbed.Sims3.FileFormats.Database(localPackage, true);

                for (int i = 0; i < keyNames.Count; i++)
                {
                    if (tempChunks[i].Length == 0 && KeyUtils.validateKey(keyNames[i]) == true)
                    {
                        try
                        {
                            tempChunks[i] = localDb.GetResourceStream(new keyName(keyNames[i]).ToResourceKey());
                        }
                        catch (System.Collections.Generic.KeyNotFoundException ex)
                        {
                            //Helpers.logMessageToFile(ex.Message);
                        }
                        catch (Exception ex)
                        {
                            //Helpers.logMessageToFile(ex.Message);
                        }
                    }
                }
            }

            Stream input = File.OpenRead(Helpers.findSims3Root() + "\\GameData\\Shared\\Packages\\FullBuild" + fullBuildNum.ToString() + ".package");
            Gibbed.Sims3.FileFormats.Database db = new Gibbed.Sims3.FileFormats.Database(input, true);

            for (int i = 0; i < keyNames.Count; i++)
            {
                if (tempChunks[i].Length == 0 && KeyUtils.validateKey(keyNames[i]) == true)
                {

                    keyName tKey = new keyName(keyNames[i]);
                    try
                    {
                        tempChunks[i] = db.GetResourceStream(tKey.ToResourceKey());
                    }
                    catch (System.Collections.Generic.KeyNotFoundException ex)
                    {
                        //Helpers.logMessageToFile(ex.Message);
                    }
                    catch (Exception ex)
                    {
                        //Helpers.logMessageToFile(ex.Message);
                    }
                }
            }
            input.Close();


            return tempChunks;
        }

        #endregion

    }
}
