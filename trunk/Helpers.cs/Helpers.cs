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
            catch (Exception e)
            {
                //MessageBox.Show(e.Message);
            }

            return myDocuments;
        }

        public static void setSims3Root()
        {
            System.Windows.Forms.FolderBrowserDialog fBrowse = new System.Windows.Forms.FolderBrowserDialog();
            fBrowse.Description = @"Please find your Sims 3 root (usually C:\Program Files\Electronic Arts\The Sims 3\)";
            if (fBrowse.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (fBrowse.SelectedPath != "")
                {
                    Helpers.saveCommonRegistryValue("sims3root", fBrowse.SelectedPath);
                    if (File.Exists(fBrowse.SelectedPath + "\\GameData\\Shared\\Packages\\FullBuild0.package"))
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

        private static string sims3root = "";
        public static string findSims3Root()
        {
            //string path32 = "Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\{C05D8CDB-417D-4335-A38C-A0659EDFD6B8}";
            //string path64 = "Software\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\{C05D8CDB-417D-4335-A38C-A0659EDFD6B8}"

            if (sims3root != "")
            {
                return sims3root;
            }

            string installLocation = "";
            try
            {

                installLocation = Helpers.getCommonRegistryValue("sims3root");
                if (installLocation != "") return installLocation;

                string path32 = "Software\\Sims\\The Sims 3";
                string path64 = "Software\\Wow6432Node\\Sims\\The Sims 3";

                string path32Alt = "Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\{C05D8CDB-417D-4335-A38C-A0659EDFD6B8}";
                string path64Alt = "Software\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\{C05D8CDB-417D-4335-A38C-A0659EDFD6B8}";

                RegistryKey key;
                key = Registry.LocalMachine.OpenSubKey(path32, false);
                if (key == null)
                {
                    // No Key exists... check 64 bit location
                    key = Registry.LocalMachine.OpenSubKey(path64, false);
                    if (key == null)
                    {

                        // Try Alt location
                        key = Registry.LocalMachine.OpenSubKey(path32Alt, false);
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
            catch (Exception e)
            {
                //MessageBox.Show(e.Message);
            }

            // Check to see if FullBuild0.package exists within this root
            bool getManualPath = false;
            if (installLocation != "")
            {
                if (File.Exists(installLocation + "\\GameData\\Shared\\Packages\\FullBuild0.package"))
                {
                    sims3root = installLocation;
                    return installLocation;
                }
                else
                {
                    // No FullBuild0 found, have to get a manual path
                    getManualPath = true;
                }
            }
            else
            {
                getManualPath = true;
            }

            if (getManualPath)
            {
                System.Windows.Forms.FolderBrowserDialog fBrowse = new System.Windows.Forms.FolderBrowserDialog();
                fBrowse.Description = @"Please find your Sims 3 root (usually C:\Program Files\Electronic Arts\The Sims 3\)";
                if (fBrowse.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (fBrowse.SelectedPath != "")
                    {
                        Helpers.saveCommonRegistryValue("sims3root", fBrowse.SelectedPath);
                        if (File.Exists(fBrowse.SelectedPath + "\\GameData\\Shared\\Packages\\FullBuild0.package"))
                        {
                            sims3root = fBrowse.SelectedPath;
                            return fBrowse.SelectedPath;
                        }
                        else
                        {
                            return "";
                        }

                    }

                }

            }

            sims3root = installLocation;
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

        #region Colour Conversions and functions
        public static string convertColour(Color color)
        {
            // Converts a colour dialog box colour into a 0 to 1 style colour
            float newR = color.R / 255f;
            float newG = color.G / 255f;
            float newB = color.B / 255f;
            float newA = color.A / 255f;

            //CultureInfo englishCulture = new CultureInfo("");
            string red = String.Format(CultureInfo.InvariantCulture, "{0:0.0000000}", newR);
            string green = String.Format(CultureInfo.InvariantCulture, "{0:0.0000000}", newG);
            string blue = String.Format(CultureInfo.InvariantCulture, "{0:0.0000000}", newB);
            string alpha = String.Format(CultureInfo.InvariantCulture, "{0:0.0000000}", newA);

            return red + "," + green + "," + blue + "," + alpha;

        }

        public static String ColorToHex(Color actColor)
        {
            return "#" + actColor.R.ToString("X2") + actColor.G.ToString("X2") + actColor.B.ToString("X2");
        }

        public static Color HexToColor(string hex)
        {
            return ColorFromHex(hex);
        }

        public static Color ColorFromHex(string hex)
        {
            if (hex.StartsWith("#")) { hex = hex.Remove(0, 1); }

            if (hex.Length != 6)
            {
                throw new Exception("Invalid hex");
            }
            int red = Convert.ToByte(hex.Substring(0, 2), 0x10);
            int green = Convert.ToByte(hex.Substring(2, 2), 0x10);
            int blue = Convert.ToByte(hex.Substring(4, 2), 0x10);
            return Color.FromArgb(0xff, red, green, blue);
        }

        public static Color convertColour(string colourString)
        {
            return convertColour(colourString, false);
        }
        public static Color convertColour(string colourString, bool returnNull)
        {
            Color newC;
            if (colourString != "" && colourString != null)
            {

                // Split the colour string using the ,
                string[] colours = colourString.Split(",".ToCharArray());
                float red = Convert.ToSingle(colours[0], CultureInfo.InvariantCulture);
                float green = Convert.ToSingle(colours[1], CultureInfo.InvariantCulture);
                float blue = Convert.ToSingle(colours[2], CultureInfo.InvariantCulture);
                float alpha = 255;
                if (colours[3] != null)
                {
                    alpha = Convert.ToSingle(colours[3], CultureInfo.InvariantCulture);
                }
                newC = Color.FromArgb((int)(alpha * 255), (int)(red * 255), (int)(green * 255), (int)(blue * 255));
            }
            else
            {
                if (!returnNull)
                {
                    newC = Color.Black;
                }
                else
                {
                    newC = Color.Empty;
                }
            }
            return newC;
        }

        /// <summary>
        /// Convert HSV to RGB
        /// h is from 0-360
        /// s,v values are 0-1
        /// r,g,b values are 0-255
        /// Based upon http://ilab.usc.edu/wiki/index.php/HSV_And_H2SV_Color_Space#HSV_Transformation_C_.2F_C.2B.2B_Code_2
        /// </summary>
        public static Color HsvToRgb(double h, double S, double V)
        {
            Color temp = new Color();

            // ######################################################################
            // T. Nathan Mundhenk
            // mundhenk@usc.edu
            // C/C++ Macro HSV to RGB

            double H = h;
            while (H < 0) { H += 360; };
            while (H >= 360) { H -= 360; };
            double R, G, B;
            if (V <= 0)
            { R = G = B = 0; }
            else if (S <= 0)
            {
                R = G = B = V;
            }
            else
            {
                double hf = H / 60.0;
                int i = (int)Math.Floor(hf);
                double f = hf - i;
                double pv = V * (1 - S);
                double qv = V * (1 - S * f);
                double tv = V * (1 - S * (1 - f));
                switch (i)
                {

                    // Red is the dominant color

                    case 0:
                        R = V;
                        G = tv;
                        B = pv;
                        break;

                    // Green is the dominant color

                    case 1:
                        R = qv;
                        G = V;
                        B = pv;
                        break;
                    case 2:
                        R = pv;
                        G = V;
                        B = tv;
                        break;

                    // Blue is the dominant color

                    case 3:
                        R = pv;
                        G = qv;
                        B = V;
                        break;
                    case 4:
                        R = tv;
                        G = pv;
                        B = V;
                        break;

                    // Red is the dominant color

                    case 5:
                        R = V;
                        G = pv;
                        B = qv;
                        break;

                    // Just in case we overshoot on our math by a little, we put these here. Since its a switch it won't slow us down at all to put these here.

                    case 6:
                        R = V;
                        G = tv;
                        B = pv;
                        break;
                    case -1:
                        R = V;
                        G = pv;
                        B = qv;
                        break;

                    // The color is not defined, we should throw an error.

                    default:
                        //LFATAL("i Value error in Pixel conversion, Value is %d", i);
                        R = G = B = V; // Just pretend its black/white
                        break;
                }
            }

            temp = Color.FromArgb(Clamp((int)(R * 255.0)), Clamp((int)(G * 255.0)), Clamp((int)(B * 255.0)));

            return temp;
        }

        /// <summary>
        /// Clamp a value to 0-255
        /// </summary>
        private static int Clamp(int i)
        {
            if (i < 0) return 0;
            if (i > 255) return 255;
            return i;
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
        #endregion

        #region Stream functions
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

        public static patternDetails parsePatternComplate(Stream xmlStream)
        {
            int level = 0;
            string curPattern = "";

            patternDetails pDetail = new patternDetails();

            if (xmlStream == null) return pDetail;
            string patternTexture = "";

            XmlTextReader xtr = new XmlTextReader(xmlStream);

            while (xtr.Read())
            {
                if (xtr.NodeType == XmlNodeType.Element)
                {
                    switch (xtr.Name)
                    {
                        case "complate":
                            level++;
                            //xtr.MoveToAttribute("category");
                            //pDetail.category = xtr.Value;

                            xtr.MoveToAttribute("name");
                            pDetail.name = xtr.Value;
                            xtr.MoveToAttribute("category");
                            pDetail.category = xtr.Value;

                            //xtr.MoveToAttribute(1);
                            //pDetail.rreskey = xtr.Value;
                            break;
                        case "variables":
                            level++;
                            break;
                        case "param":
                            if (level == 1)
                            {
                                // Normal complate stuff
                                xtr.MoveToNextAttribute();
                                //Console.WriteLine(xtr.Value);
                            }
                            if (level == 2)
                            {
                                // Inside pattern
                                xtr.MoveToAttribute("name");

                                //Console.WriteLine(xtr.Value);
                                switch (xtr.Value)
                                {
                                    case "assetRoot":
                                        xtr.MoveToAttribute("default");
                                        pDetail.assetRoot = xtr.Value;
                                        break;

                                    case "Color":
                                        xtr.MoveToAttribute("default");
                                        pDetail.Color = xtr.Value;
                                        break;
                                    case "Color 0":
                                        xtr.MoveToAttribute("default");
                                        pDetail.ColorP[0] = xtr.Value;
                                        break;
                                    case "Color 1":
                                        xtr.MoveToAttribute("default");
                                        pDetail.ColorP[1] = xtr.Value;
                                        break;
                                    case "Color 2":
                                        xtr.MoveToAttribute("default");
                                        pDetail.ColorP[2] = xtr.Value;
                                        break;
                                    case "Color 3":
                                        xtr.MoveToAttribute("default");
                                        pDetail.ColorP[3] = xtr.Value;
                                        break;
                                    case "Color 4":
                                        xtr.MoveToAttribute("default");
                                        pDetail.ColorP[4] = xtr.Value;
                                        break;

                                    case "Channel 1":
                                        xtr.MoveToAttribute("default");
                                        pDetail.Channel[0] = xtr.Value;
                                        break;
                                    case "Channel 2":
                                        xtr.MoveToAttribute("default");
                                        pDetail.Channel[2] = xtr.Value;
                                        break;
                                    case "Channel 3":
                                        xtr.MoveToAttribute("default");
                                        pDetail.Channel[3] = xtr.Value;
                                        break;
                                    case "Channel 4":
                                        xtr.MoveToAttribute("default");
                                        pDetail.Channel[4] = xtr.Value;
                                        break;

                                    case "Channel 1 Enabled":
                                        xtr.MoveToAttribute("default");
                                        pDetail.ChannelEnabled[0] = xtr.Value;
                                        break;
                                    case "Channel 2 Enabled":
                                        xtr.MoveToAttribute("default");
                                        pDetail.ChannelEnabled[1] = xtr.Value;
                                        break;
                                    case "Channel 3 Enabled":
                                        xtr.MoveToAttribute("default");
                                        pDetail.ChannelEnabled[2] = xtr.Value;
                                        break;
                                    case "Channel 4 Enabled":
                                        xtr.MoveToAttribute("default");
                                        pDetail.ChannelEnabled[3] = xtr.Value;
                                        break;


                                    case "Background Image":
                                        xtr.MoveToAttribute("default");
                                        pDetail.BackgroundImage = xtr.Value;
                                        if (patternTexture == "") patternTexture = xtr.Value;
                                        break;

                                    case "H Bg":
                                        xtr.MoveToAttribute("default");
                                        pDetail.HBg = xtr.Value;
                                        break;
                                    case "H 1":
                                        xtr.MoveToAttribute("default");
                                        pDetail.H[0] = xtr.Value;
                                        break;
                                    case "H 2":
                                        xtr.MoveToAttribute("default");
                                        pDetail.H[1] = xtr.Value;
                                        break;
                                    case "H 3":
                                        xtr.MoveToAttribute("default");
                                        pDetail.H[2] = xtr.Value;
                                        break;
                                    case "H 4":
                                        xtr.MoveToAttribute("default");
                                        pDetail.H[3] = xtr.Value;
                                        break;

                                    case "S Bg":
                                        xtr.MoveToAttribute("default");
                                        pDetail.SBg = xtr.Value;
                                        break;
                                    case "S 1":
                                        xtr.MoveToAttribute("default");
                                        pDetail.S[0] = xtr.Value;
                                        break;
                                    case "S 2":
                                        xtr.MoveToAttribute("default");
                                        pDetail.S[1] = xtr.Value;
                                        break;
                                    case "S 3":
                                        xtr.MoveToAttribute("default");
                                        pDetail.S[2] = xtr.Value;
                                        break;
                                    case "S 4":
                                        xtr.MoveToAttribute("default");
                                        pDetail.S[3] = xtr.Value;
                                        break;

                                    case "V Bg":
                                        xtr.MoveToAttribute("default");
                                        pDetail.VBg = xtr.Value;
                                        break;
                                    case "V 1":
                                        xtr.MoveToAttribute("default");
                                        pDetail.V[0] = xtr.Value;
                                        break;
                                    case "V 2":
                                        xtr.MoveToAttribute("default");
                                        pDetail.V[1] = xtr.Value;
                                        break;
                                    case "V 3":
                                        xtr.MoveToAttribute("default");
                                        pDetail.V[2] = xtr.Value;
                                        break;
                                    case "V 4":
                                        xtr.MoveToAttribute("default");
                                        pDetail.V[3] = xtr.Value;
                                        break;


                                    case "Base H Bg":
                                        xtr.MoveToAttribute("default");
                                        pDetail.BaseHBg = xtr.Value;
                                        break;
                                    case "Base H 1":
                                        xtr.MoveToAttribute("default");
                                        pDetail.BaseH[0] = xtr.Value;
                                        break;
                                    case "Base H 2":
                                        xtr.MoveToAttribute("default");
                                        pDetail.BaseH[1] = xtr.Value;
                                        break;
                                    case "Base H 3":
                                        xtr.MoveToAttribute("default");
                                        pDetail.BaseH[2] = xtr.Value;
                                        break;
                                    case "Base H 4":
                                        xtr.MoveToAttribute("default");
                                        pDetail.BaseH[3] = xtr.Value;
                                        break;

                                    case "Base S Bg":
                                        xtr.MoveToAttribute("default");
                                        pDetail.BaseSBg = xtr.Value;
                                        break;
                                    case "Base S 1":
                                        xtr.MoveToAttribute("default");
                                        pDetail.BaseS[0] = xtr.Value;
                                        break;
                                    case "Base S 2":
                                        xtr.MoveToAttribute("default");
                                        pDetail.BaseS[1] = xtr.Value;
                                        break;
                                    case "Base S 3":
                                        xtr.MoveToAttribute("default");
                                        pDetail.BaseS[2] = xtr.Value;
                                        break;
                                    case "Base S 4":
                                        xtr.MoveToAttribute("default");
                                        pDetail.BaseS[3] = xtr.Value;
                                        break;

                                    case "Base V Bg":
                                        xtr.MoveToAttribute("default");
                                        pDetail.BaseVBg = xtr.Value;
                                        break;
                                    case "Base V 1":
                                        xtr.MoveToAttribute("default");
                                        pDetail.BaseV[0] = xtr.Value;
                                        break;
                                    case "Base V 2":
                                        xtr.MoveToAttribute("default");
                                        pDetail.BaseV[1] = xtr.Value;
                                        break;
                                    case "Base V 3":
                                        xtr.MoveToAttribute("default");
                                        pDetail.BaseV[2] = xtr.Value;
                                        break;
                                    case "Base V 4":
                                        xtr.MoveToAttribute("default");
                                        pDetail.BaseV[3] = xtr.Value;
                                        break;

                                    case "HSVShift Bg":
                                        xtr.MoveToAttribute("default");
                                        pDetail.HSVShiftBg = xtr.Value;
                                        break;
                                    case "HSVShift 1":
                                        xtr.MoveToAttribute("default");
                                        pDetail.HSVShift[0] = xtr.Value;
                                        break;
                                    case "HSVShift 2":
                                        xtr.MoveToAttribute("default");
                                        pDetail.HSVShift[1] = xtr.Value;
                                        break;
                                    case "HSVShift 3":
                                        xtr.MoveToAttribute("default");
                                        pDetail.HSVShift[2] = xtr.Value;
                                        break;
                                    case "HSVShift 4":
                                        xtr.MoveToAttribute("default");
                                        pDetail.HSVShift[3] = xtr.Value;
                                        break;

                                    case "rgbmask":
                                        xtr.MoveToAttribute("default");
                                        pDetail.rgbmask = xtr.Value;
                                        if (patternTexture == "") patternTexture = xtr.Value;
                                        break;
                                    case "specmap":
                                        xtr.MoveToAttribute("default");
                                        pDetail.specmap = xtr.Value;
                                        break;
                                    case "filename":
                                        xtr.MoveToAttribute("default");
                                        pDetail.filename = xtr.Value;
                                        break;

                                }
                            }
                            break;
                        case "pattern":
                            level++;
                            string a1 = "";
                            string a2 = "";
                            string a3 = "";

                            xtr.MoveToNextAttribute();
                            a1 = xtr.Value;
                            xtr.MoveToNextAttribute();
                            a2 = xtr.Value;
                            xtr.MoveToNextAttribute();
                            a3 = xtr.Value;
                            curPattern = a3;

                            break;
                    }
                }
                if (xtr.NodeType == XmlNodeType.EndElement)
                {
                    switch (xtr.Name)
                    {
                        case "variables":
                            level--;
                            break;
                        case "complate":
                            level--;
                            break;
                    }
                }
            }

            if (patternTexture != "")
            {
                //Console.WriteLine(lookupList.Items[i].fullCasPartname);

                if (!MadScience.KeyUtils.validateKey(patternTexture, false))
                {
                    patternTexture = patternTexture.Replace(@"($assetRoot)\InGame\Complates\", "");
                    patternTexture = patternTexture.Replace(@".tga", "");
                    patternTexture = patternTexture.Replace(@".dds", "");
                }
                else
                {
                    // This is a custom pattern so we have to construct up the filename ourselves
                    patternTexture = "Materials\\" + pDetail.category + "\\" + pDetail.name;
                }

                pDetail.filename = patternTexture;
            }

            if (!String.IsNullOrEmpty(pDetail.rgbmask) && !MadScience.KeyUtils.validateKey(pDetail.rgbmask, false))
            {
                // Need to change rgbmask to key format
                pDetail.rgbmask = "key:00B2D882:00000000:" + MadScience.StringHelpers.HashFNV64(pDetail.rgbmask.Substring(pDetail.rgbmask.LastIndexOf("\\") + 1).Replace(@".tga", "").Replace(@".dds", "")).ToString("X16");
            }
            if (!String.IsNullOrEmpty(pDetail.specmap) && !MadScience.KeyUtils.validateKey(pDetail.specmap, false))
            {
                // Need to change rgbmask to key format
                pDetail.specmap = "key:00B2D882:00000000:" + MadScience.StringHelpers.HashFNV64(pDetail.specmap.Substring(pDetail.specmap.LastIndexOf("\\") + 1).Replace(@".tga", "").Replace(@".dds", "")).ToString("X16");
            }

            if (!String.IsNullOrEmpty(pDetail.BackgroundImage) && !MadScience.KeyUtils.validateKey(pDetail.BackgroundImage, false))
            {
                pDetail.BackgroundImage = "key:00B2D882:00000000:" + MadScience.StringHelpers.HashFNV64(pDetail.BackgroundImage.Substring(pDetail.BackgroundImage.LastIndexOf("\\") + 1).Replace(@".tga", "").Replace(@".dds", "")).ToString("X16");
            }

            for (int i = 0; i < 4; i++)
            {
                if (pDetail.name.StartsWith("solidColor") || pDetail.name.StartsWith("Flat Color"))
                {
                    pDetail.type = "solidColor";
                }
                if (pDetail.HSVShiftBg != "" || pDetail.HSVShift[0] != null)
                {
                    pDetail.type = "HSV";
                }
                if (pDetail.ColorP[0] != null || pDetail.ColorP[1] != null)
                {
                    pDetail.type = "Coloured";
                }
                //Console.WriteLine("Pattern " + i + ": " + xcd.pattern[i].name + " - " + xcd.pattern[i].type);
            }

            //Console.WriteLine(Environment.NewLine);
            xtr.Close();

            return pDetail;
        }

    }

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
}
