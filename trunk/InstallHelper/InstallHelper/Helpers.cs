using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Globalization;
using Microsoft.Win32;
using System.Xml;
using System.Xml.Serialization;
//using System.Linq;
using System.IO;
//using Gibbed.Helpers;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Text.RegularExpressions;

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

            return myDocuments;
        }

        public static void setSims3Root()
        {
            System.Windows.Forms.FolderBrowserDialog fBrowse = new System.Windows.Forms.FolderBrowserDialog();
            fBrowse.Description = @"Please find your Sims 3 root ";
            if (String.IsNullOrEmpty(sims3root)) {
                fBrowse.Description += @"(usually C:\Program Files\Electronic Arts\The Sims 3\)";
            } else {
                fBrowse.Description += @"(currently " + sims3root + ")";
                fBrowse.SelectedPath = sims3root;
            }
            fBrowse.Description += " NOTE: This is NOT where your Sims3.exe file is!";
            if (fBrowse.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (fBrowse.SelectedPath != "")
                {
                    Helpers.saveCommonRegistryValue("sims3root", fBrowse.SelectedPath);
                    if (File.Exists(Path.Combine(fBrowse.SelectedPath, Helpers.getGameSubPath("\\GameData\\Shared\\Packages\\FullBuild0.package"))))
                    {
                        sims3root = fBrowse.SelectedPath;
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

        private static string sims3root = "";
        public static string findSims3Root()
        {
            //string path32 = "Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\{C05D8CDB-417D-4335-A38C-A0659EDFD6B8}";
            //string path64 = "Software\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\{C05D8CDB-417D-4335-A38C-A0659EDFD6B8}"

            if (sims3root != "")
            {
                //Console.WriteLine("Returning already stored path");
                return sims3root;
            }

            string installLocation = "";
            try
            {

                //Console.WriteLine("Attempting to get path from common registry...");
                installLocation = Helpers.getCommonRegistryValue("sims3root");
                if (!String.IsNullOrEmpty(installLocation))
                {
                    // Check install location just in case...
                    try
                    {
                        if (File.Exists(Path.Combine(installLocation, Helpers.getGameSubPath("\\GameData\\Shared\\Packages\\FullBuild0.package"))))
                        {
                            //Helpers.saveCommonRegistryValue("sims3root", installLocation);
                            sims3root = installLocation;
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
                string path32 = "Software\\Sims\\The Sims 3";
                string path64 = "Software\\Wow6432Node\\Sims\\The Sims 3";

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
                    if (File.Exists(Path.Combine(installLocation, Helpers.getGameSubPath("\\GameData\\Shared\\Packages\\FullBuild0.package"))))
                    {
                        Helpers.saveCommonRegistryValue("sims3root", installLocation);
                        sims3root = installLocation;
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

            if (getManualPath)
            {
                getManualPath = false;

                // Check for existance of XML file in the Application.Startup folder - this can be used to override
                // paths where none can be found (ie on Macs)
                if (File.Exists(Path.Combine(Application.StartupPath, "pathOverrides.xml")))
                {
                    Stream xmlStream = File.OpenRead(Path.Combine(Application.StartupPath, "pathOverrides.xml"));
                    XmlTextReader xtr = new XmlTextReader(xmlStream);

                    while (xtr.Read())
                    {
                        if (xtr.NodeType == XmlNodeType.Element)
                        {
                            switch (xtr.Name)
                            {
                                case "path":
                                    xtr.MoveToAttribute("name");
                                    switch (xtr.Value)
                                    {
                                        case "sims3root":
                                            installLocation = xtr.GetAttribute("location");
                                            break;
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
                            if (File.Exists(Path.Combine(installLocation, Helpers.getGameSubPath("\\GameData\\Shared\\Packages\\FullBuild0.package"))))
                            {
                                Helpers.saveCommonRegistryValue("sims3root", installLocation);
                                sims3root = installLocation;
                                return installLocation;
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

                // If we got to this point we have to show a dialog to the user asking them where to find the sims3root

                System.Windows.Forms.FolderBrowserDialog fBrowse = new System.Windows.Forms.FolderBrowserDialog();
                fBrowse.Description = @"Please find your Sims 3 root (usually C:\Program Files\Electronic Arts\The Sims 3\) ";
                fBrowse.Description += "NOTE: This is NOT where your Sims3.exe file is!";
                if (fBrowse.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (fBrowse.SelectedPath != "")
                    {
                        try
                        {
                            if (File.Exists(Path.Combine(fBrowse.SelectedPath, Helpers.getGameSubPath("\\GameData\\Shared\\Packages\\FullBuild0.package"))))
                            {
                                Helpers.saveCommonRegistryValue("sims3root", fBrowse.SelectedPath);
                                sims3root = fBrowse.SelectedPath;
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

            sims3root = installLocation;
            return installLocation;
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

        #region Stream functions
        public static bool isValidStream(Stream input)
        {
            if (input == null) return false;
            if (input == Stream.Null) return false;
            if (input.Length == 0) return false;

            return true;
        }

        public static void CopyStream(Stream readStream, Stream writeStream)
        {
            CopyStream(readStream, writeStream, false);
        }
        public static void CopyStream(Stream readStream, Stream writeStream, bool fromStart)
        {

            if (fromStart)
            {
                readStream.Seek(0, SeekOrigin.Begin);
            }

            int Length = 256;
            Byte[] buffer = new Byte[Length];
            int bytesRead = readStream.Read(buffer, 0, Length);
            // write the required bytes
            while (bytesRead > 0)
            {
                writeStream.Write(buffer, 0, bytesRead);
                bytesRead = readStream.Read(buffer, 0, Length);
            }
        }
        #endregion

        #region String functions
        public static string sanitiseString(string input)
        {
            string temp = Regex.Replace(input, "[^a-zA-Z0-9]", "");
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

    /*
    internal class FastPixel
    {
        // Fields
        private Bitmap _bitmap;
        private int _height;
        private bool _isAlpha;
        private int _width;
        private BitmapData bmpData;
        private IntPtr bmpPtr;
        private bool locked;
        private byte[] rgbValues;

        // Methods
        public FastPixel(Bitmap bitmap)
        {
            if (bitmap.PixelFormat == (bitmap.PixelFormat | PixelFormat.Indexed))
            {
                throw new Exception("Cannot lock an Indexed image.");
            }
            this._bitmap = bitmap;
            this._isAlpha = this.Bitmap.PixelFormat == (this.Bitmap.PixelFormat | PixelFormat.Alpha);
            this._width = bitmap.Width;
            this._height = bitmap.Height;
        }

        public void Clear(Color colour)
        {
            if (!this.locked)
            {
                throw new Exception("Bitmap not locked.");
            }
            if (this.IsAlphaBitmap)
            {
                for (int i = 0; i < this.rgbValues.Length; i += 4)
                {
                    this.rgbValues[i] = colour.B;
                    this.rgbValues[i + 1] = colour.G;
                    this.rgbValues[i + 2] = colour.R;
                    this.rgbValues[i + 3] = colour.A;
                }
            }
            else
            {
                for (int j = 0; j < this.rgbValues.Length; j += 3)
                {
                    this.rgbValues[j] = colour.B;
                    this.rgbValues[j + 1] = colour.G;
                    this.rgbValues[j + 2] = colour.R;
                }
            }
        }

        public Color GetPixel(Point location)
        {
            return this.GetPixel(location.X, location.Y);
        }

        public Color GetPixel(int x, int y)
        {
            if (!this.locked)
            {
                throw new Exception("Bitmap not locked.");
            }
            if (this.IsAlphaBitmap)
            {
                int num = ((y * this.Width) + x) * 4;
                int num2 = this.rgbValues[num];
                int num3 = this.rgbValues[num + 1];
                int num4 = this.rgbValues[num + 2];
                int alpha = this.rgbValues[num + 3];
                return Color.FromArgb(alpha, num4, num3, num2);
            }
            int index = ((y * this.Width) + x) * 3;
            int blue = this.rgbValues[index];
            int green = this.rgbValues[index + 1];
            int red = this.rgbValues[index + 2];
            return Color.FromArgb(red, green, blue);
        }

        public void Lock()
        {
            if (this.locked)
            {
                throw new Exception("Bitmap already locked.");
            }
            Rectangle rect = new Rectangle(0, 0, this.Width, this.Height);
            this.bmpData = this.Bitmap.LockBits(rect, ImageLockMode.ReadWrite, this.Bitmap.PixelFormat);
            this.bmpPtr = this.bmpData.Scan0;
            if (this.IsAlphaBitmap)
            {
                int num = (this.Width * this.Height) * 4;
                this.rgbValues = new byte[num];
                Marshal.Copy(this.bmpPtr, this.rgbValues, 0, this.rgbValues.Length);
            }
            else
            {
                int num2 = (this.Width * this.Height) * 3;
                this.rgbValues = new byte[num2];
                Marshal.Copy(this.bmpPtr, this.rgbValues, 0, this.rgbValues.Length);
            }
            this.locked = true;
        }

        public void SetPixel(Point location, Color colour)
        {
            this.SetPixel(location.X, location.Y, colour);
        }

        public void SetPixel(int x, int y, Color colour)
        {
            if (!this.locked)
            {
                throw new Exception("Bitmap not locked.");
            }
            if (this.IsAlphaBitmap)
            {
                int index = ((y * this.Width) + x) * 4;
                this.rgbValues[index] = colour.B;
                this.rgbValues[index + 1] = colour.G;
                this.rgbValues[index + 2] = colour.R;
                this.rgbValues[index + 3] = colour.A;
            }
            else
            {
                int num2 = ((y * this.Width) + x) * 3;
                this.rgbValues[num2] = colour.B;
                this.rgbValues[num2 + 1] = colour.G;
                this.rgbValues[num2 + 2] = colour.R;
            }
        }

        public void Unlock(bool setPixels)
        {
            if (!this.locked)
            {
                throw new Exception("Bitmap not locked.");
            }
            if (setPixels)
            {
                Marshal.Copy(this.rgbValues, 0, this.bmpPtr, this.rgbValues.Length);
            }
            this.Bitmap.UnlockBits(this.bmpData);
            this.locked = false;
        }

        // Properties
        public Bitmap Bitmap
        {
            get
            {
                return this._bitmap;
            }
        }

        public int Height
        {
            get
            {
                return this._height;
            }
        }

        public bool IsAlphaBitmap
        {
            get
            {
                return this._isAlpha;
            }
        }

        public int Width
        {
            get
            {
                return this._width;
            }
        }
    }
     */
}
