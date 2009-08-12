using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using MadScience;

namespace PatternBrowser
{
    public partial class PatternBrowser : Form
    {
        public PatternBrowser()
        {
            try
            {

                Helpers.logMessageToFile("Initialising PatternBrowser");

                InitializeComponent();

                Helpers.logMessageToFile("Creating cache folder");
                if (!Directory.Exists(Application.StartupPath + "\\patterncache\\"))
                {
                    Directory.CreateDirectory(Application.StartupPath + "\\patterncache\\");
                }

                loadPatternList();

            }
            catch (Exception e)
            {
                Helpers.logMessageToFile(e.Message + Environment.NewLine + e.StackTrace);
                MessageBox.Show(e.Message + Environment.NewLine + e.StackTrace);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TextReader r = new StreamReader(@"P:\Stuart\Desktop\fullPatternList.xml");
            XmlSerializer s = new XmlSerializer(typeof(files));
            files lookupList = (files)s.Deserialize(r);
            r.Close();

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < lookupList.Items.Count; i++)
            {

                // Open XML file 
                string patternTexture = "";
                string color0 = "";
                string color1 = "";
                string color2 = "";
                string color3 = "";
                string color4 = "";
                string HBg = "";
                string SBg = "";
                string VBg = "";
                string HSVShiftBg = "";

                Stream mem = File.OpenRead(@"P:\Stuart\Desktop\FullBuild0\config\xml\root\" + lookupList.Items[i].fullCasPartname + ".xml");
                XmlTextReader xtr = new XmlTextReader(mem);
                while (xtr.Read() )
                {
                    if (xtr.NodeType == XmlNodeType.Element)
                    {
                        switch (xtr.Name)
                        {
                            case "param":
                                xtr.MoveToAttribute("type");
                                if (xtr.Value == "texture")
                                {
                                    xtr.MoveToAttribute("name");
                                    if (xtr.Value == "rgbmask")
                                    {
                                        xtr.MoveToAttribute("default");
                                        if (patternTexture == "") patternTexture = xtr.Value;
                                    }
                                    if (xtr.Value == "Background Image")
                                    {
                                        xtr.MoveToAttribute("default");
                                        patternTexture = xtr.Value;
                                    }
                                }
                                if (xtr.Value == "float")
                                {
                                    xtr.MoveToAttribute("name");
                                    if (xtr.Value == "H Bg")
                                    {
                                        xtr.MoveToAttribute("default");
                                        HBg = xtr.Value;
                                    }
                                    if (xtr.Value == "S Bg")
                                    {
                                        xtr.MoveToAttribute("default");
                                        SBg = xtr.Value;
                                    }
                                    if (xtr.Value == "V Bg")
                                    {
                                        xtr.MoveToAttribute("default");
                                        VBg = xtr.Value;
                                    }
                                }
                                if (xtr.Value == "string")
                                {
                                    xtr.MoveToAttribute("name");
                                    if (xtr.Value == "HSVShift Bg")
                                    {
                                        xtr.MoveToAttribute("default");
                                        HSVShiftBg = xtr.Value;
                                    }
                                }
                                if (xtr.Value == "color")
                                {
                                    xtr.MoveToAttribute("name");
                                    if (xtr.Value == "Color 0")
                                    {
                                        xtr.MoveToAttribute("default");
                                        color0 = xtr.Value;
                                    }
                                    if (xtr.Value == "Color 1")
                                    {
                                        xtr.MoveToAttribute("default");
                                        color1 = xtr.Value;
                                    }
                                    if (xtr.Value == "Color 2")
                                    {
                                        xtr.MoveToAttribute("default");
                                        color2 = xtr.Value;
                                    }
                                    if (xtr.Value == "Color 3")
                                    {
                                        xtr.MoveToAttribute("default");
                                        color3 = xtr.Value;
                                    }
                                    if (xtr.Value == "Color 4")
                                    {
                                        xtr.MoveToAttribute("default");
                                        color4 = xtr.Value;
                                    }

                                }
                                break;
                        }
                    }
                }

                if (patternTexture != "")
                {
                   //Console.WriteLine(lookupList.Items[i].fullCasPartname);

                    patternTexture = patternTexture.Replace(@"($assetRoot)\InGame\Complates\Materials\", "");
                    patternTexture = patternTexture.Replace(@".tga", "");
                    patternTexture = patternTexture.Replace(@".dds", "");
                    //Console.WriteLine(patternTexture);
                    string fullName = patternTexture.Substring(patternTexture.LastIndexOf("\\") + 1);
                    string category = patternTexture.Substring(0, patternTexture.IndexOf("\\"));
                    string subCategory = patternTexture.Substring(patternTexture.IndexOf("\\") + 1);

                    if (subCategory.Contains("\\"))
                    {
                        subCategory = subCategory.Remove(subCategory.IndexOf("\\"));
                    }
                    else
                    {
                        subCategory = "";
                    }

                    sb.AppendLine("<file groupid=\"" + lookupList.Items[i].groupid + "\" instanceid=\"" + lookupList.Items[i].instanceid + "\" typeid=\"" + lookupList.Items[i].typeid + "\" category=\"" + category + "\" subcategory=\"" + subCategory + "\" texturename=\"" + fullName + "\" color0=\"" + color0 + "\" color1=\"" + color1 + "\" color2=\"" + color2 + "\" color3=\"" + color3 + "\" color4=\"" + color4 + "\" HBg=\"" + HBg + "\" SBg=\"" + SBg + "\" VBg=\"" + VBg + "\" HSVShiftBg=\"" + HSVShiftBg + "\" >" + lookupList.Items[i].fullCasPartname + "</file>");

                }
                xtr.Close();
                mem.Close();
            }

            Clipboard.SetText(sb.ToString());
            Console.WriteLine("Done");

        }
        patterns patterns;
        patterns customPatterns;
        int imageWidth = 128;
        int imageHeight = 128;
        
        private patternsFile _selectedPattern = new patternsFile();
        //public patternsFile selectedPattern = new patternsFile();
        //private patternDetails _selectedPattern = new patternDetails();
        public patternDetails selectedPattern = new patternDetails();

        //Dictionary<ulong, Gibbed.Sims3.FileFormats.ResourceKey> castEntries = new Dictionary<ulong, Gibbed.Sims3.FileFormats.ResourceKey>();

        public int curCategory = 0;

        private void loadPatternList()
        {
            Helpers.logMessageToFile("Loading patternList.xml");
            TextReader r = new StreamReader(Application.StartupPath + "\\patternList.xml");
            XmlSerializer s = new XmlSerializer(typeof(patterns));
            this.patterns = (patterns)s.Deserialize(r);
            r.Close();
            Helpers.logMessageToFile(patterns.Items.Count + " patterns found");

            if (File.Exists(Application.StartupPath + "\\customPatterns.xml"))
            {
                Helpers.logMessageToFile("Loading customPatterns.xml");
                TextReader r2 = new StreamReader(Application.StartupPath + "\\customPatterns.xml");
                XmlSerializer s2 = new XmlSerializer(typeof(patterns));
                this.customPatterns = (patterns)s2.Deserialize(r2);
                r2.Close();
                Helpers.logMessageToFile(patterns.Items.Count + " patterns found");
            }
            if (this.customPatterns == null) this.customPatterns = new patterns();
        }

        public patternsFile findPattern(string resKey)
        {
            // Get pattern details from XML
            keyName patternXML = new keyName(resKey);
            patternsFile temp = new patternsFile();

            bool hasMatch = false;

            for (int i = 0; i < this.customPatterns.Items.Count; i++)
            {
                patternsFile pattern = this.customPatterns.Items[i];

                if (Gibbed.Helpers.StringHelpers.ParseHex32(pattern.typeid) == patternXML.typeId && Gibbed.Helpers.StringHelpers.ParseHex32(pattern.groupid) == patternXML.groupId && Gibbed.Helpers.StringHelpers.ParseHex64(pattern.instanceid) == patternXML.instanceId)
                {
                    hasMatch = true;
                    temp = pattern;
                    break;
                }
            }

            if (!hasMatch)
            {
                for (int i = 0; i < this.patterns.Items.Count; i++)
                {
                    patternsFile pattern = this.patterns.Items[i];
                    if (pattern.typeid == null || pattern.groupid == null || pattern.instanceid == null)
                    {
                        Console.WriteLine("Oops");
                    }

                    if (Gibbed.Helpers.StringHelpers.ParseHex32(pattern.typeid) == patternXML.typeId && Gibbed.Helpers.StringHelpers.ParseHex32(pattern.groupid) == patternXML.groupId && Gibbed.Helpers.StringHelpers.ParseHex64(pattern.instanceid) == patternXML.instanceId)
                    {
                        hasMatch = true;
                        temp = pattern;
                        break;
                    }
                }
            }

            return temp;
        }

        public Image makePatternThumb(string resKey)
        {
            return makePatternThumb(findPattern(resKey), true, null);
        }
        public Image makePatternThumb(patternsFile pattern, bool saveImage, patternsFile pOverride)
        {
            //keyName patternXML = new keyName(resKey);

            PictureBox picBox = new PictureBox();
            picBox.BackColor = System.Drawing.Color.White;
            picBox.Size = new System.Drawing.Size(this.imageWidth, this.imageHeight);
            picBox.SizeMode = PictureBoxSizeMode.StretchImage;

            if (pattern.isCustom)
            {
                /*
                            bool hasMatch = false;

                            for (int i = 0; i < this.customPatterns.Items.Count; i++)
                            {
                                patternsFile pattern = this.customPatterns.Items[i];

                                if (Gibbed.Helpers.StringHelpers.ParseHex32(pattern.typeid) == patternXML.typeId && Gibbed.Helpers.StringHelpers.ParseHex32(pattern.groupid) == patternXML.groupId && Gibbed.Helpers.StringHelpers.ParseHex64(pattern.instanceid) == patternXML.instanceId)
                                {
                                    hasMatch = true;
                 */
                if (File.Exists(pattern.subcategory))
                {
                    Stream cast = File.Open(pattern.subcategory, FileMode.Open, FileAccess.Read, FileShare.Read);
                    Gibbed.Sims3.FileFormats.Database castdb = new Gibbed.Sims3.FileFormats.Database(cast, true);

                    keyName temp = new keyName(pattern.texturename);
                    Stream patternThumb = null;

                    try
                    {
                        patternThumb = castdb.GetResourceStream(temp.ToResourceKey());
                    }
                    catch (System.Collections.Generic.KeyNotFoundException ex)
                    {
                    }
                    catch (Exception ex)
                    {
                        Helpers.logMessageToFile(ex.Message);
                    }

                    if (patternThumb != null)
                    {
                        if (pOverride != null)
                        {
                            // Only use the pattern colours
                            pattern.color0 = pOverride.color0;
                            pattern.color1 = pOverride.color1;
                            pattern.color2 = pOverride.color2;
                            pattern.color3 = pOverride.color3;
                            pattern.color4 = pOverride.color4;
                            pattern.HBg = pOverride.HBg;
                            pattern.SBg = pOverride.SBg;
                            pattern.VBg = pOverride.VBg;
                        }


                        picBox.Image = makePatternThumb(patternThumb, pattern);
                        if (saveImage)
                        {
                            try
                            {
                                picBox.Image.Save(Application.StartupPath + "\\patterncache\\" + pattern.casPart + ".png", System.Drawing.Imaging.ImageFormat.Png);
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }

                    cast.Close();
                }
                /*
                    break;
                }
                 */
            }
            else 
            {
                string s3root = MadScience.Helpers.findSims3Root();
                string thumbnailPackage = @"\GameData\Shared\Packages\FullBuild2.package";

                Console.WriteLine("Starting at: " + DateTime.Now.ToString());

                Stream cast2 = File.Open(s3root + thumbnailPackage, FileMode.Open, FileAccess.Read, FileShare.Read);
                Gibbed.Sims3.FileFormats.Database castdb2 = new Gibbed.Sims3.FileFormats.Database(cast2);

                /*
                for (int i = 0; i < this.patterns.Items.Count; i++)
                {
                    patternsFile pattern = this.patterns.Items[i];

                    if (Gibbed.Helpers.StringHelpers.ParseHex32(pattern.typeid) == patternXML.typeId && Gibbed.Helpers.StringHelpers.ParseHex32(pattern.groupid) == patternXML.groupId && Gibbed.Helpers.StringHelpers.ParseHex64(pattern.instanceid) == patternXML.instanceId)
                    {
                        hasMatch = true;
                */
                        ulong instanceid = Gibbed.Helpers.StringHelpers.HashFNV64(pattern.casPart);
                        keyName temp = new keyName(0x00B2D882, 0x00000000, instanceid);
                        Stream patternThumb = null;

                        try
                        {
                            patternThumb = castdb2.GetResourceStream(temp.ToResourceKey());
                        }
                        catch (System.Collections.Generic.KeyNotFoundException ex)
                        {
                            temp.instanceId = Gibbed.Helpers.StringHelpers.HashFNV64(pattern.texturename);
                            try
                            {
                                patternThumb = castdb2.GetResourceStream(temp.ToResourceKey());
                            }
                            catch (System.Collections.Generic.KeyNotFoundException kex)
                            {
                            }
                            catch (Exception ex2)
                            {
                                Helpers.logMessageToFile(ex.Message);
                            }
                        }
                        catch (Exception ex)
                        {
                            Helpers.logMessageToFile(ex.Message);
                        }

                        if (patternThumb != null)
                        {
                            if (pOverride != null)
                            {
                                // Only use the pattern colours
                                pattern.color0 = pOverride.color0;
                                pattern.color1 = pOverride.color1;
                                pattern.color2 = pOverride.color2;
                                pattern.color3 = pOverride.color3;
                                pattern.color4 = pOverride.color4;
                                pattern.HBg = pOverride.HBg;
                                pattern.SBg = pOverride.SBg;
                                pattern.VBg = pOverride.VBg;
                            }

                            picBox.Image = makePatternThumb(patternThumb, pattern);
                            if (saveImage)
                            {
                                try
                                {
                                    picBox.Image.Save(Application.StartupPath + "\\patterncache\\" + pattern.casPart + ".png", System.Drawing.Imaging.ImageFormat.Png);
                                }
                                catch (Exception ex)
                                {
                                }
                            }
                        }
                /*
                        break;
                    }

                }
                 */
            }

            return picBox.Image;
        }

        public patternsFile pDetailsTopFile(patternDetails pDetails)
        {
            patternsFile pFile = new patternsFile();

            keyName pKey = new keyName(pDetails.key);
            pFile.casPart = pDetails.filename;
            pFile.typeid = "0x" + pKey.typeId.ToString("X8");
            pFile.groupid = "0x" + pKey.groupId.ToString("X8");
            pFile.instanceid = "0x" + pKey.instanceId.ToString("X16");

            pFile.category = pDetails.category;
            pFile.color0 = pDetails.ColorP[0];
            pFile.color1 = pDetails.ColorP[1];
            pFile.color2 = pDetails.ColorP[2];
            pFile.color3 = pDetails.ColorP[3];
            pFile.color4 = pDetails.ColorP[4];

            pFile.HBg = pDetails.HBg;
            pFile.SBg = pDetails.SBg;
            pFile.VBg = pDetails.VBg;

            return pFile;
        }

        public Image makePatternThumb(Stream patternThumb, patternsFile pattern)
        {
            // Save original DDS file too

            //Stream patternDDS = File.OpenWrite(Application.StartupPath + "\\patterncache\\" + pattern.casPart + ".dds");
            //Helpers.CopyStream(patternThumb, patternDDS, true);
            //patternDDS.Close();

            Image temp;

            DdsFileTypePlugin.DdsFile ddsP = new DdsFileTypePlugin.DdsFile();
            ddsP.Load(patternThumb);
            
            // Figure out colour channels
            if (pattern.color1 == "" && pattern.color2 == "" && pattern.color3 == "" && pattern.color4 == "")
            {
                if (pattern.HBg == "" && pattern.SBg == "" && pattern.VBg == "")
                {
                    if (pattern.HSVShiftBg == "")
                    {
                        temp = ddsP.Image();
                    }
                    else
                    {
                        temp = ddsP.Image();
                    }
                }
                else
                {
                    temp = ddsP.Image();
                    //picBox.Image = ddsP.Image(Color.Black, Helpers.HsvToRgb(-Convert.ToDouble(pattern.HBg), -Convert.ToDouble(pattern.SBg), -Convert.ToDouble(pattern.VBg)));
                }

            }
            else
            {
                Color bgColor = Helpers.convertColour(pattern.color0, true);
                if (bgColor == Color.Empty)
                {
                    bgColor = Color.Black;
                }
                temp = ddsP.Image(bgColor, Helpers.convertColour(pattern.color1, true), Helpers.convertColour(pattern.color2, true), Helpers.convertColour(pattern.color3, true), Helpers.convertColour(pattern.color4, true));
                //picBox.Image = Helpers.imagePreview(ddsP.Image(), Color.Black, Helpers.convertColour(pattern.color1, true), Helpers.convertColour(pattern.color2, true), Helpers.convertColour(pattern.color3, true), Helpers.convertColour(pattern.color4, true));
            }
            return temp;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            panel1.Controls.Clear();
            panel1.Visible = false;

            label2.Text = "Loading patterns... Please wait...";
            label2.Refresh();

            int numFound = 0;
            int horizontal = 0;
            int vertical = 0;

            bool hasFound = false;

            ToolTip tt = new ToolTip();

            int curWidth = panel1.Width;

            int maxAcross = curWidth / imageWidth;
            if ((maxAcross * (imageWidth + 6)) > curWidth) maxAcross--;

            DdsFileTypePlugin.DdsFile ddsP = new DdsFileTypePlugin.DdsFile();

            if (comboBox1.Text == "* Custom")
            {

                for (int i = 0; i < this.customPatterns.Items.Count; i++)
                {
                    patternsFile pattern = this.customPatterns.Items[i];
                    //if (pattern.category == comboBox1.Text)
                    //{

                        numFound++;

                        PictureBox picBox = new PictureBox();
                        picBox.BackColor = System.Drawing.Color.White;
                        picBox.Location = new System.Drawing.Point(horizontal, vertical);
                        picBox.Name = pattern.casPart;
                        picBox.Size = new System.Drawing.Size(this.imageWidth, this.imageHeight);
                        picBox.SizeMode = PictureBoxSizeMode.StretchImage;
                        picBox.Click += new System.EventHandler(pictureBox_Click);

                        string toolTip = pattern.category + "\\";
                        //if (pattern.subcategory != "")
                        //{
                            //toolTip += pattern.subcategory + "\\";
                        //}
                        toolTip += pattern.casPart;
                        tt.SetToolTip(picBox, toolTip);

                        // Find thumbnail
                        if (File.Exists(Application.StartupPath + "\\patterncache\\" + pattern.casPart + ".png"))
                        {
                            Stream tmpImage = File.OpenRead(Application.StartupPath + "\\patterncache\\" + pattern.casPart + ".png");
                            picBox.Image = Image.FromStream(tmpImage);
                            tmpImage.Close();
                        }
                        else
                        {
                            if (File.Exists(pattern.subcategory))
                            {
                                Stream cast = File.Open(pattern.subcategory, FileMode.Open, FileAccess.Read, FileShare.Read);
                                Gibbed.Sims3.FileFormats.Database castdb = new Gibbed.Sims3.FileFormats.Database(cast, true);

                                keyName temp = new keyName(pattern.texturename);
                                Stream patternThumb = null;

                                try
                                {
                                    patternThumb = castdb.GetResourceStream(temp.ToResourceKey());
                                }
                                catch (System.Collections.Generic.KeyNotFoundException ex)
                                {
                                }
                                catch (Exception ex)
                                {
                                    Helpers.logMessageToFile(ex.Message);
                                }

                                if (patternThumb != null)
                                {
                                    picBox.Image = makePatternThumb(patternThumb, pattern);
                                    try
                                    {
                                        picBox.Image.Save(Application.StartupPath + "\\patterncache\\" + pattern.casPart + ".png", System.Drawing.Imaging.ImageFormat.Png);
                                    }
                                    catch (Exception ex)
                                    {
                                    }
                                }

                                cast.Close();
                            }

                        }

                        // Add picturebox to panel
                        panel1.Controls.Add(picBox);

                        // Calc picture box horizontal and vertical
                        if ((numFound % maxAcross) == 0)
                        {
                            vertical += imageHeight + 6; horizontal = 0;
                        }
                        else
                        {
                            horizontal += imageWidth + 6;
                        }
                    //}

                }

            }
            else
            {

                string s3root = MadScience.Helpers.findSims3Root();
                string thumbnailPackage = @"\GameData\Shared\Packages\FullBuild2.package";

                Console.WriteLine("Starting at: " + DateTime.Now.ToString());

                Stream cast = File.Open(s3root + thumbnailPackage, FileMode.Open, FileAccess.Read, FileShare.Read);
                Gibbed.Sims3.FileFormats.Database castdb = new Gibbed.Sims3.FileFormats.Database(cast);

                for (int i = 0; i < this.patterns.Items.Count; i++)
                {
                    patternsFile pattern = this.patterns.Items[i];
                    if (pattern.category == comboBox1.Text)
                    {

                        numFound++;

                        PictureBox picBox = new PictureBox();
                        picBox.BackColor = System.Drawing.Color.White;
                        picBox.Location = new System.Drawing.Point(horizontal, vertical);
                        picBox.Name = pattern.casPart;
                        picBox.Size = new System.Drawing.Size(this.imageWidth, this.imageHeight);
                        picBox.SizeMode = PictureBoxSizeMode.StretchImage;
                        picBox.Click += new System.EventHandler(pictureBox_Click);

                        string toolTip = pattern.category + "\\";
                        if (pattern.subcategory != "")
                        {
                            toolTip += pattern.subcategory + "\\";
                        }
                        toolTip += pattern.casPart;
                        tt.SetToolTip(picBox, toolTip);

                        // Find thumbnail
                        if (File.Exists(Application.StartupPath + "\\patterncache\\" + pattern.casPart + ".png"))
                        {
                            Stream tmpImage = File.OpenRead(Application.StartupPath + "\\patterncache\\" + pattern.casPart + ".png");
                            picBox.Image = Image.FromStream(tmpImage);
                            tmpImage.Close();
                        }
                        else
                        {
                            ulong instanceid = Gibbed.Helpers.StringHelpers.HashFNV64(pattern.casPart);
                            keyName temp = new keyName(0x00B2D882, 0x00000000, instanceid);
                            Stream patternThumb = null;

                            try
                            {
                                patternThumb = castdb.GetResourceStream(temp.ToResourceKey());
                            }
                            catch (System.Collections.Generic.KeyNotFoundException ex)
                            {
                                temp.instanceId = Gibbed.Helpers.StringHelpers.HashFNV64(pattern.texturename);
                                try
                                {
                                    patternThumb = castdb.GetResourceStream(temp.ToResourceKey());
                                }
                                catch (System.Collections.Generic.KeyNotFoundException kex)
                                {
                                }
                                catch (Exception ex2)
                                {
                                    Helpers.logMessageToFile(ex.Message);
                                }
                            }
                            catch (Exception ex)
                            {
                                Helpers.logMessageToFile(ex.Message);
                            }

                            if (patternThumb != null)
                            {
                                picBox.Image = makePatternThumb(patternThumb, pattern);
                                try
                                {
                                    picBox.Image.Save(Application.StartupPath + "\\patterncache\\" + pattern.casPart + ".png", System.Drawing.Imaging.ImageFormat.Png);
                                }
                                catch (Exception ex)
                                {
                                }
                            }
                        }

                        // Add picturebox to panel
                        panel1.Controls.Add(picBox);

                        // Calc picture box horizontal and vertical
                        if ((numFound % maxAcross) == 0)
                        {
                            vertical += imageHeight + 6; horizontal = 0;
                        }
                        else
                        {
                            horizontal += imageWidth + 6;
                        }
                    }

                }

                Console.WriteLine("Stopping at: " + DateTime.Now.ToString());

                //dbpf = null;
                cast.Close();
                //castdb = null;
            }
            label2.Text = numFound.ToString() + " patterns";
            panel1.Visible = true;
            this.curCategory = comboBox1.SelectedIndex;
        }

        private void pictureBox_Click(object sender, EventArgs e)
        {

            for (int i = 0; i < panel1.Controls.Count; i++)
            {
                PictureBox temp2 = (PictureBox)panel1.Controls[i];
                temp2.BorderStyle = BorderStyle.None;
            }

            PictureBox temp = (PictureBox)sender;
            toolStripStatusLabel1.Text = temp.Name;
            temp.BorderStyle = BorderStyle.Fixed3D;

            if (comboBox1.Text == "* Custom")
            {

                for (int i = 0; i < this.customPatterns.Items.Count; i++)
                {
                    patternsFile pattern = this.customPatterns.Items[i];
                    if (pattern.casPart == temp.Name)
                    {

                        toolStripStatusLabel2.Text = pattern.category;
                        if (pattern.subcategory != "" && pattern.subcategory != null && pattern.isCustom == false)
                        {
                            toolStripStatusLabel2.Text += "\\" + pattern.subcategory;
                        }

                        toolStripStatusLabel3.Text = pattern.instanceid;
                        //toolStripStatusLabel4.Text = "key:" + pattern.typeid.ToLower() + ":" + pattern.groupid.ToLower() + ":" + pattern.instanceid.ToLower();

                        this._selectedPattern = pattern;
                        button2.Enabled = true;



                        break;
                    }
                }

            }
            else
            {

                for (int i = 0; i < this.patterns.Items.Count; i++)
                {
                    patternsFile pattern = this.patterns.Items[i];
                    if (pattern.casPart == temp.Name)
                    {

                        toolStripStatusLabel2.Text = pattern.category;
                        if (pattern.subcategory != "")
                        {
                            toolStripStatusLabel2.Text += "\\" + pattern.subcategory;
                        }

                        toolStripStatusLabel3.Text = pattern.instanceid;
                        //toolStripStatusLabel4.Text = "key:" + pattern.typeid.ToLower() + ":" + pattern.groupid.ToLower() + ":" + pattern.instanceid.ToLower();

                        this._selectedPattern = pattern;
                        button2.Enabled = true;



                        break;
                    }
                }
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Show();
            this.Invalidate();
            //comboBox1.SelectedIndex = this.curCategory;
        }

        private void panel1_Resize(object sender, EventArgs e)
        {
            int horizontal = 0;
            int vertical = 0;

            int curWidth = panel1.Width;

            int maxAcross = curWidth / imageWidth;
            if ((maxAcross * (imageWidth + 6)) > curWidth) maxAcross--;
//            Console.WriteLine(curWidth + " " + imageWidth + " " + maxAcross );


            for (int i = 1; i <= panel1.Controls.Count; i++)
            {
                PictureBox temp = (PictureBox)panel1.Controls[i-1];

                temp.Location = new System.Drawing.Point(horizontal, vertical);

                // Calc picture box horizontal and vertical
                if ((i % maxAcross) == 0)
                {
                    vertical += imageHeight + 6; horizontal = 0;
                }
                else
                {
                    horizontal += imageWidth + 6;
                }

            }

        }

        private void button2_Click(object sender, EventArgs e)
        {


            //this.selectedPattern = this._selectedPattern;

            string instanceid = this._selectedPattern.instanceid.Remove(0, 2);
            string typeid = this._selectedPattern.typeid.Remove(0, 2);
            string groupid = this._selectedPattern.groupid.Remove(0, 2);

            string reskey = "key:" + typeid + ":" + groupid + ":" + instanceid;

            if (_selectedPattern.isCustom == false)
            {
                this.selectedPattern = Helpers.parsePatternComplate(KeyUtils.searchForKey(reskey, 0));
            }
            else
            {
                this.selectedPattern = Helpers.parsePatternComplate(KeyUtils.searchForKey(reskey, _selectedPattern.subcategory));
            }
            this.selectedPattern.key = reskey;

            this.DialogResult = DialogResult.OK;

            this.Close();
        }

        private void PatternBrowser_Shown(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = this.curCategory;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnLoadCustom_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Sims 3 Package|*.package";
            if (openFileDialog1.ShowDialog() == DialogResult.OK && openFileDialog1.FileName != "")
            {
                Stream cast = File.Open(openFileDialog1.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                Gibbed.Sims3.FileFormats.Database castdb = new Gibbed.Sims3.FileFormats.Database(cast, true);

                // Open XML file 
                string patternTexture = "";
                patternsFile cPattern = new patternsFile();

                foreach (Gibbed.Sims3.FileFormats.ResourceKey key in castdb.Entries.Keys)
                {
                    if ((key.GroupId == 0x00000000) && (key.TypeId == 0x0333406C))
                    {
                        Stream mem = castdb.GetResourceStream(key);
                        XmlTextReader xtr = new XmlTextReader(mem);
                        while (xtr.Read())
                        {
                            if (xtr.NodeType == XmlNodeType.Element)
                            {
                                switch (xtr.Name)
                                {

                                    case "complate":
                                        xtr.MoveToAttribute("category");
                                        cPattern.category = xtr.Value;
                                        xtr.MoveToAttribute("name");
                                        cPattern.casPart = xtr.Value;
                                        break;
                                    case "step":
                                        xtr.MoveToAttribute("type");
                                        if (xtr.Value == "ColorFill")
                                        {
                                            if (xtr.AttributeCount == 2)
                                            {
                                                xtr.MoveToAttribute("color");
                                                cPattern.color0 = xtr.Value;
                                            }
                                        }
                                        break;
                                    case "param":
                                        xtr.MoveToAttribute("type");
                                        if (xtr.Value == "texture")
                                        {
                                            xtr.MoveToAttribute("name");
                                            if (xtr.Value == "rgbmask")
                                            {
                                                xtr.MoveToAttribute("default");
                                                if (patternTexture == "") patternTexture = xtr.Value;
                                            }
                                            if (xtr.Value == "Background Image")
                                            {
                                                xtr.MoveToAttribute("default");
                                                patternTexture = xtr.Value;
                                            }
                                        }
                                        if (xtr.Value == "float")
                                        {
                                            xtr.MoveToAttribute("name");
                                            if (xtr.Value == "H Bg")
                                            {
                                                xtr.MoveToAttribute("default");
                                                cPattern.HBg = xtr.Value;
                                            }
                                            if (xtr.Value == "S Bg")
                                            {
                                                xtr.MoveToAttribute("default");
                                                cPattern.SBg = xtr.Value;
                                            }
                                            if (xtr.Value == "V Bg")
                                            {
                                                xtr.MoveToAttribute("default");
                                                cPattern.VBg = xtr.Value;
                                            }
                                        }
                                        if (xtr.Value == "string")
                                        {
                                            xtr.MoveToAttribute("name");
                                            if (xtr.Value == "HSVShift Bg")
                                            {
                                                xtr.MoveToAttribute("default");
                                                cPattern.HSVShiftBg = xtr.Value;
                                            }
                                        }
                                        if (xtr.Value == "color")
                                        {
                                            xtr.MoveToAttribute("name");
                                            if (xtr.Value == "Color 0")
                                            {
                                                xtr.MoveToAttribute("default");
                                                cPattern.color1 = xtr.Value;
                                            }
                                            if (xtr.Value == "Color 1")
                                            {
                                                xtr.MoveToAttribute("default");
                                                cPattern.color2 = xtr.Value;
                                            }
                                            if (xtr.Value == "Color 2")
                                            {
                                                xtr.MoveToAttribute("default");
                                                cPattern.color3 = xtr.Value;
                                            }
                                            if (xtr.Value == "Color 3")
                                            {
                                                xtr.MoveToAttribute("default");
                                                cPattern.color4 = xtr.Value;
                                            }

                                        }
                                        break;
                                }
                            }
                        }

                        if (patternTexture != "")
                        {
                            cPattern.typeid = "0x" + key.TypeId.ToString("X8");
                            cPattern.groupid = "0x" + key.GroupId.ToString("X8");
                            cPattern.instanceid = "0x" + key.InstanceId.ToString("X16");
                            cPattern.texturename = patternTexture;
                            cPattern.isCustom = true;
                            cPattern.subcategory = openFileDialog1.FileName;

                            customPatterns.Items.Add(cPattern);

                            TextWriter r = new StreamWriter(Application.StartupPath + "\\customPatterns.xml");
                            XmlSerializer s = new XmlSerializer(typeof(patterns));
                            s.Serialize(r, customPatterns);
                            r.Close();

                            keyName temp = new keyName(cPattern.texturename);
                            foreach (Gibbed.Sims3.FileFormats.ResourceKey key2 in castdb.Entries.Keys)
                            {
                                if ((key2.GroupId == temp.groupId) && (key2.TypeId == temp.typeId) && (key2.InstanceId == temp.instanceId))
                                {

                                    int horizontal = 0;
                                    int vertical = 0;

                                    int curWidth = panel1.Width;

                                    int maxAcross = curWidth / imageWidth;
                                    if ((maxAcross * (imageWidth + 6)) > curWidth) maxAcross--;
                                    //            Console.WriteLine(curWidth + " " + imageWidth + " " + maxAcross );


                                    for (int i = 1; i <= panel1.Controls.Count; i++)
                                    {
                                        // Calc picture box horizontal and vertical
                                        if ((i % maxAcross) == 0)
                                        {
                                            vertical += imageHeight + 6; horizontal = 0;
                                        }
                                        else
                                        {
                                            horizontal += imageWidth + 6;
                                        }

                                    }

                                    // Extract texture
                                    //DdsFileTypePlugin.DdsFile ddsP = new DdsFileTypePlugin.DdsFile();
                                    Stream patternThumb = castdb.GetResourceStream(key2);

                                    Stream patternDDS = File.OpenWrite(Application.StartupPath + "\\patterncache\\" + cPattern.casPart + ".dds");
                                    Helpers.CopyStream(patternThumb, patternDDS, true);
                                    patternDDS.Close();

                                    patternThumb.Seek(0, SeekOrigin.Begin);
                                    //ddsP.Load(patternThumb);

                                    PictureBox picBox = new PictureBox();
                                    picBox.BackColor = System.Drawing.Color.White;
                                    picBox.Location = new System.Drawing.Point(horizontal, vertical);
                                    picBox.Name = cPattern.casPart;
                                    picBox.Size = new System.Drawing.Size(this.imageWidth, this.imageHeight);
                                    picBox.Click += new System.EventHandler(pictureBox_Click);

                                    string toolTip = cPattern.category + "\\";
                                    ToolTip tt = new ToolTip();
                                    toolTip += cPattern.casPart;
                                    tt.SetToolTip(picBox, toolTip);


                                    picBox.Image = makePatternThumb(patternThumb, cPattern);
                                    //picBox.Image =     castdb.GetResourceStream(entry.Key);
                                    try
                                    {
                                        picBox.Image.Save(Application.StartupPath + "\\patterncache\\" + cPattern.casPart + ".png", System.Drawing.Imaging.ImageFormat.Png);
                                    }
                                    catch (Exception ex)
                                    {
                                    }

                                    panel1.Controls.Add(picBox);


                                    break;
                                }
                            }

                            //Console.WriteLine(lookupList.Items[i].fullCasPartname);
                            
                            //Console.WriteLine("<file groupid=\"" + key.GroupId + "\" instanceid=\"" + key.InstanceId + "\" typeid=\"" + key.TypeId + "\" category=\"" + category + "\" subcategory=\"" + subCategory + "\" texturename=\"" + cPattern.texturename + "\" color0=\"" + color0 + "\" color1=\"" + color1 + "\" color2=\"" + color2 + "\" color3=\"" + color3 + "\" color4=\"" + color4 + "\">" + cPattern.casPart + "</file>");

                        }
                        xtr.Close();
                        mem.Close();

                        break;

                    }
                }

                comboBox1.SelectedIndex = 15;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            foreach (patternsFile pattern in customPatterns.Items)
            {
                if (File.Exists(Application.StartupPath + "\\patterncache\\" + pattern.casPart + ".png"))
                {
                    try {
                        File.Delete(Application.StartupPath + "\\patterncache\\" + pattern.casPart + ".png");
                    }
                    catch(Exception ex)
                    {
                    }
                }
            }


            customPatterns.Items.Clear();
            TextWriter r = new StreamWriter(Application.StartupPath + "\\customPatterns.xml");
            XmlSerializer s = new XmlSerializer(typeof(patterns));
            s.Serialize(r, customPatterns);
            r.Close();

            if (comboBox1.Text == "* Custom")
            {
                toolStripStatusLabel1.Text = "";
                toolStripStatusLabel2.Text = "";
                toolStripStatusLabel3.Text = "";
                toolStripStatusLabel4.Text = "";
                for (int i = 0; i < panel1.Controls.Count; i++)
                {
                    panel1.Controls.RemoveAt(i);
                }
            }

        }
    }

    
    /// <remarks/>
    [System.Xml.Serialization.XmlRootAttribute()]
    public class patterns
    {
        //public List<patternsFile> Items;

        public patterns()
        {
            Items = new List<patternsFile>();
        }
      
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("file", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = true)]
        public List<patternsFile> Items = new List<patternsFile>();

    }
    
 
    /// <remarks/>
    public class patternsFile
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string typeid;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string groupid;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string instanceid;

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string texturename;

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string category;

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string subcategory;

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string color0;
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string color1;
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string color2;
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string color3;
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string color4;

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string HBg;

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string SBg;

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string VBg;

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string HSVShiftBg;

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool isCustom;

        /// <remarks/>

        //private string _casPart = "";
        [System.Xml.Serialization.XmlTextAttribute()]
        public string casPart;

    }



    /// <remarks/>
    [System.Xml.Serialization.XmlRootAttribute()]
    public class files
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("file", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = true)]
        public List<filesFile> Items = new List<filesFile>();

    }

    /// <remarks/>
    public class filesFile
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string groupid;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string instanceid;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string typeid;

        public string cGender;
        public string cAge;
        public string cType;

        /// <remarks/>
        public string fullCasPartname;

        [System.Xml.Serialization.XmlTextAttribute()]
        public string casPart
        {
            get { return this.fullCasPartname; }
            set
            {
                string temp = value.Replace(@"config\xml\root\", "");

                this.fullCasPartname = temp.Replace(".xml", "");
            }
        }

    }

}
