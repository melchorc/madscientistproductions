using System;
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
                if (!Directory.Exists(Path.Combine(Application.StartupPath, "patterncache")))
                {
                    Directory.CreateDirectory(Path.Combine(Application.StartupPath, "patterncache"));
                }

                loadPatternList();

            }
            catch (Exception e)
            {
                Helpers.logMessageToFile(e.Message + Environment.NewLine + e.StackTrace);
                MessageBox.Show(e.Message + Environment.NewLine + e.StackTrace);
            }
        }

        #region import pattern list
        private void button1_Click(object sender, EventArgs e)
        {
            TextReader r = new StreamReader(@"P:\Stuart\Desktop\fullPatternList.xml");
            XmlSerializer s = new XmlSerializer(typeof(files));
            files lookupList = (files)s.Deserialize(r);
            r.Close();

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < lookupList.Items.Count; i++)
            {


                Stream mem = File.OpenRead(@"P:\Stuart\Desktop\FullBuild0\config\xml\root\" + lookupList.Items[i].fullCasPartname + ".xml");

                patternDetails pDetails = Patterns.parsePatternComplate(mem);

                if (String.IsNullOrEmpty(pDetails.category)) continue;

                patternsFile cPattern = new patternsFile();

                MadScience.Wrappers.ResourceKey rKey = new MadScience.Wrappers.ResourceKey("key:" + lookupList.Items[i].typeid.Remove(0, 2) + ":" + lookupList.Items[i].groupid.Remove(0, 2) + ":" + lookupList.Items[i].instanceid.Remove(0,2));

                cPattern.key = rKey.ToString();

                if (!StreamHelpers.isValidStream(KeyUtils.findKey("key:00B2D882:00000000:" + StringHelpers.HashFNV64(pDetails.name.Substring(pDetails.name.LastIndexOf("\\") + 1)).ToString("X16"))))
                {
                    cPattern.texturename = pDetails.BackgroundImage;
                }
                else
                {
                    cPattern.texturename = "key:00B2D882:00000000:" + StringHelpers.HashFNV64(pDetails.name.Substring(pDetails.name.LastIndexOf("\\") + 1)).ToString("X16");
                }
                cPattern.casPart = pDetails.name.Substring(pDetails.name.LastIndexOf("\\") + 1);

                switch (pDetails.category)
                {
                    case "Old":
                        break;
                    default:
                        //string fullName = pDetails.Substring(patternTexture.LastIndexOf("\\") + 1);
                        string category = pDetails.filename;
                        category = category.Replace(@"($assetRoot)\InGame\Complates\", "");
                        category = category.Replace(@"Materials\", "");
                        category = category.Replace(@".tga", ""); 
                        category = category.Replace(@".dds", ""); 

                        if (category.IndexOf("\\") > -1)
                        {
                            category = category.Substring(0, category.IndexOf("\\"));
                        }
                        else
                        {
                            category = pDetails.category;
                        }

                        string subCategory = pDetails.filename;
                        subCategory = subCategory.Replace(@"($assetRoot)\InGame\Complates\", "");
                        subCategory = subCategory.Replace(@"Materials\", "");
                        subCategory = subCategory.Replace(@".tga", "");
                        subCategory = subCategory.Replace(@".dds", ""); 
                        if (subCategory.IndexOf("\\") > -1)
                        {
                            subCategory = subCategory.Substring(subCategory.IndexOf("\\") + 1);
                        }

                        if (subCategory.Contains("\\"))
                        {
                            subCategory = subCategory.Remove(subCategory.IndexOf("\\"));
                        }
                        else
                        {
                            subCategory = "";
                        }

                        if (subCategory == pDetails.name) subCategory = "";

                        cPattern.category = category;
                        cPattern.subcategory = subCategory;

                        Console.WriteLine(pDetails.name + " " + category + " " + subCategory);

                        sb.AppendLine("<pattern key=\"" + cPattern.key + "\" texturename=\"\" category=\"" + category + "\" subcategory=\"" + subCategory + "\">" + pDetails.name + "</file>");

                        break;
                }
                
                //xtr.Close();
                 
                mem.Close();
            }

            Clipboard.SetText(sb.ToString());
            Console.WriteLine("Done");

        }
        #endregion

        patterns patterns;
        patterns customPatterns;

        int imageWidth = 128;
        int imageHeight = 128;
        int numFound = 0;
        int vertical = 0;
        int horizontal = 0;

        private patternsFile _selectedPattern = new patternsFile();
        public patternDetails selectedPattern = new patternDetails();

        public int curCategory = 0;

        private void loadPatternList()
        {
            Helpers.logMessageToFile("Loading patternList.xml");
            TextReader r = new StreamReader(Path.Combine(Application.StartupPath,  Path.Combine("xml", "patternList.xml")));
            XmlSerializer s = new XmlSerializer(typeof(patterns));
            this.patterns = (patterns)s.Deserialize(r);
            r.Close();
            Helpers.logMessageToFile(patterns.Items.Count + " patterns found");

            if (File.Exists(Path.Combine(Application.StartupPath, Path.Combine("xml", "customPatterns.xml"))))
            {
                Helpers.logMessageToFile("Loading customPatterns.xml");
                TextReader r2 = new StreamReader(Path.Combine(Application.StartupPath, Path.Combine("xml", "customPatterns.xml")));
                XmlSerializer s2 = new XmlSerializer(typeof(patterns));
                this.customPatterns = (patterns)s2.Deserialize(r2);
                r2.Close();
                Helpers.logMessageToFile(customPatterns.Items.Count + " custom patterns found");
            }
            if (this.customPatterns == null) this.customPatterns = new patterns();
        }

        /*
        public Stream findPattern(patternsFile pattern)
        {
            if (File.Exists(pattern.subcategory))
            {
                Stream cast = File.Open(pattern.subcategory, FileMode.Open, FileAccess.Read, FileShare.Read);
                MadScience.Wrappers.Database castdb = new MadScience.Wrappers.Database(cast, true);

                MadScience.Wrappers.ResourceKey temp = new MadScience.Wrappers.ResourceKey(pattern.texturename);
                Stream patternThumb = null;

                try
                {
                    patternThumb = castdb.GetResourceStream(temp);
                }
                catch (System.Collections.Generic.KeyNotFoundException ex)
                {
                }
                catch (Exception ex)
                {
                    Helpers.logMessageToFile(ex.Message);
                }
                cast.Close();

                if (patternThumb != null)
                {
                    return patternThumb;
                }
            }

            return Stream.Null;
        }
        */
        /*
        public patternsFile findPattern(string resKey)
        {
            // Get pattern details from XML
            keyName patternXML = new keyName(resKey);
            patternsFile temp = new patternsFile();

            bool hasMatch = false;

            for (int i = 0; i < this.customPatterns.Items.Count; i++)
            {
                patternsFile pattern = this.customPatterns.Items[i];

                if (MadScience.StringHelpers.ParseHex32(pattern.typeid) == patternXML.typeId && MadScience.StringHelpers.ParseHex32(pattern.groupid) == patternXML.groupId && MadScience.StringHelpers.ParseHex64(pattern.instanceid) == patternXML.instanceId)
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

                    if (MadScience.StringHelpers.ParseHex32(pattern.typeid) == patternXML.typeId && MadScience.StringHelpers.ParseHex32(pattern.groupid) == patternXML.groupId && MadScience.StringHelpers.ParseHex64(pattern.instanceid) == patternXML.instanceId)
                    {
                        hasMatch = true;
                        temp = pattern;
                        break;
                    }
                }
            }

            return temp;
        }
        */
        /*
        public Image makePatternThumb(string resKey)
        {
            return makePatternThumb(findPattern(resKey), null);
        }
        */
        /*
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
        */

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            panel1.Controls.Clear();
            panel1.Visible = false;

            label2.Text = "Loading patterns... Please wait...";
            label2.Refresh();

            numFound = 0;
            horizontal = 0;
            vertical = 0;

            ToolTip tt = new ToolTip();


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
                        picBox.Name = pattern.casPart;

                        string toolTip = pattern.category + "\\";
                        //if (pattern.subcategory != "")
                        //{
                            //toolTip += pattern.subcategory + "\\";
                        //}
                        toolTip += pattern.casPart;
                        tt.SetToolTip(picBox, toolTip);

                        // Find thumbnail
                        if (File.Exists(Path.Combine(Application.StartupPath, Path.Combine("patterncache", pattern.casPart + ".png"))))
                        {
                            Stream tmpImage = File.OpenRead(Path.Combine(Application.StartupPath, Path.Combine("patterncache",  pattern.casPart + ".png")));
                            picBox.Image = Image.FromStream(tmpImage);
                            tmpImage.Close();
                        }
                        else
                        {
                            if (File.Exists(pattern.subcategory))
                            {

                                //if (pattern.typeid.StartsWith("0x")) pattern.typeid = pattern.typeid.Remove(0, 2);
                                //if (pattern.groupid.StartsWith("0x")) pattern.groupid = pattern.groupid.Remove(0, 2);
                                //if (pattern.instanceid.StartsWith("0x")) pattern.instanceid = pattern.instanceid.Remove(0, 2);

                                Stream patternXml = KeyUtils.searchForKey(pattern.key, pattern.subcategory);
								if (StreamHelpers.isValidStream(patternXml))
                                {
                                    patternDetails pDetails = Patterns.parsePatternComplate(patternXml);
                                    //Stream patternThumb = KeyUtils.searchForKey(pattern.texturename, pattern.subcategory);
                                    //if (Helpers.isValidStream(patternThumb))
                                    //{
                                        //picBox.Image = Patterns.makePatternThumb(patternThumb, pDetails);
                                    picBox.Image = Patterns.makePatternThumb(pDetails);
                                        try
                                        {
                                            picBox.Image.Save(Path.Combine(Application.StartupPath, Path.Combine("patterncache", pattern.casPart + ".png")), System.Drawing.Imaging.ImageFormat.Png);
                                        }
                                        catch (Exception)
                                        {
                                        }
                                    //}
                                }
                            }

                        }

                        addToPanel(picBox);
                    //}

                }

            }
            else
            {

                string s3root = MadScience.Helpers.findSims3Root();
                string thumbnailPackage = Helpers.getGameSubPath(@"\GameData\Shared\Packages\FullBuild2.package");

                Console.WriteLine("Starting at: " + DateTime.Now.ToString());

                Stream cast = File.Open(Path.Combine(s3root, thumbnailPackage), FileMode.Open, FileAccess.Read, FileShare.Read);
                MadScience.Wrappers.Database castdb = new MadScience.Wrappers.Database(cast);

                Stream fullBuild0 = File.Open(Path.Combine(s3root, Helpers.getGameSubPath(@"\GameData\Shared\Packages\FullBuild0.package")), FileMode.Open, FileAccess.Read, FileShare.Read);
                MadScience.Wrappers.Database xmldb = new MadScience.Wrappers.Database(fullBuild0);


                for (int i = 0; i < this.patterns.Items.Count; i++)
                {
                    patternsFile pattern = this.patterns.Items[i];
                    if (pattern.category == comboBox1.Text)
                    {

                        numFound++;

                        PictureBox picBox = new PictureBox();
                        picBox.BackColor = System.Drawing.Color.White;
                        picBox.Name = pattern.casPart;


                        string toolTip = pattern.category + "\\";
                        if (pattern.subcategory != "")
                        {
                            toolTip += pattern.subcategory + "\\";
                        }
                        toolTip += pattern.casPart;
                        tt.SetToolTip(picBox, toolTip);

                        // Find thumbnail
                        if (File.Exists(Path.Combine(Application.StartupPath, Path.Combine("patterncache", pattern.casPart + ".png"))))
                        {
                            Stream tmpImage = File.OpenRead(Path.Combine(Application.StartupPath, Path.Combine("patterncache", pattern.casPart + ".png")));
                            picBox.Image = Image.FromStream(tmpImage);
                            tmpImage.Close();
                        }
                        else
                        {
                            //Console.WriteLine(pattern.casPart);

                            Stream patternXml = KeyUtils.findKey(new MadScience.Wrappers.ResourceKey(pattern.key), 0, xmldb);
							if (StreamHelpers.isValidStream(patternXml))
                            {
                                patternDetails pDetails2 = Patterns.parsePatternComplate(patternXml);
                                //Stream patternThumb = KeyUtils.findKey(new MadScience.Wrappers.ResourceKey("key:00B2D882:00000000:" + StringHelpers.HashFNV64(pDetails2.name).ToString("X16")), 0, castdb);
                                //if (!Helpers.isValidStream(patternThumb))
                                //{
                                //    patternThumb = KeyUtils.findKey(new MadScience.Wrappers.ResourceKey(pDetails2.BackgroundImage), 0, castdb);
                                //}
                                //if (Helpers.isValidStream(patternThumb))
                                //{
                                    picBox.Image = Patterns.makePatternThumb(pDetails2, castdb);
                                    try
                                    {
                                        picBox.Image.Save(Path.Combine(Application.StartupPath, Path.Combine("patterncache", pattern.casPart + ".png")), System.Drawing.Imaging.ImageFormat.Png);
                                    }
                                    catch (Exception)
                                    {
                                    }
                                //}
                            }
                        }

                        addToPanel(picBox);

                    }

                }

                Console.WriteLine("Stopping at: " + DateTime.Now.ToString());

                //dbpf = null;
                fullBuild0.Close();
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

                        toolStripStatusLabel3.Text = pattern.key;
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

                        toolStripStatusLabel3.Text = pattern.key;
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
            horizontal = 6;
            vertical = 6;

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
                    vertical += imageHeight + 6; horizontal = 6;
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

            //if (_selectedPattern.typeid.StartsWith("0x")) _selectedPattern.typeid = _selectedPattern.typeid.Remove(0, 2);
            //if (_selectedPattern.groupid.StartsWith("0x")) _selectedPattern.groupid = _selectedPattern.groupid.Remove(0, 2);
            //if (_selectedPattern.instanceid.StartsWith("0x")) _selectedPattern.instanceid = _selectedPattern.instanceid.Remove(0, 2);

            //string reskey = "key:" + _selectedPattern.typeid + ":" + _selectedPattern.groupid + ":" + _selectedPattern.instanceid;

            if (_selectedPattern.isCustom == false)
            {
                this.selectedPattern = Patterns.parsePatternComplate(KeyUtils.findKey(_selectedPattern.key, 0));
            }
            else
            {
                this.selectedPattern = Patterns.parsePatternComplate(KeyUtils.searchForKey(_selectedPattern.key, _selectedPattern.subcategory));
                this.selectedPattern.isCustom = true;
                this.selectedPattern.customFilename = _selectedPattern.subcategory;
            }
            this.selectedPattern.key = _selectedPattern.key;

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

        private void addToPanel(PictureBox picBox)
        {

            int curWidth = panel1.Width;

            int maxAcross = curWidth / imageWidth;
            if ((maxAcross * (imageWidth + 6)) > curWidth) maxAcross--;

            picBox.Size = new System.Drawing.Size(this.imageWidth, this.imageHeight);
            picBox.SizeMode = PictureBoxSizeMode.StretchImage;
            picBox.Click += new System.EventHandler(pictureBox_Click);

            if (horizontal == 0) horizontal += 6;
            if (vertical == 0) vertical += 6;

            picBox.Location = new System.Drawing.Point(horizontal, vertical);

            // Add picturebox to panel
            panel1.Controls.Add(picBox);

            // Calc picture box horizontal and vertical
            if ((numFound % maxAcross) == 0)
            {
                vertical += imageHeight + 6; 
                horizontal = 6;
            }
            else
            {
                horizontal += imageWidth + 6;
            }
        }

        private void btnLoadCustom_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Sims 3 Package|*.package";
            if (openFileDialog1.ShowDialog() == DialogResult.OK && openFileDialog1.FileName != "")
            {
                Stream cast = File.Open(openFileDialog1.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                MadScience.Wrappers.Database castdb = new MadScience.Wrappers.Database(cast, true);

                // Open XML file 
                //patternsFile cPattern = new patternsFile();
                patternDetails pDetails = new patternDetails();

                foreach (MadScience.Wrappers.ResourceKey key in castdb._Entries.Keys)
                {
                    if ((key.groupId == 0x00000000) && (key.typeId == 0x0333406C))
                    {
                        pDetails = Patterns.parsePatternComplate(castdb.GetResourceStream(key));
                        patternsFile cPattern = new patternsFile();

                        //MadScience.Wrappers.ResourceKey rKey = new MadScience.Wrappers.ResourceKey(pDetails.key);

                        //cPattern.typeid = rKey.typeId.ToString("X8");
                        //cPattern.groupid = rKey.groupId.ToString("X8");
                        //cPattern.instanceid = rKey.instanceId.ToString("X16");

                        cPattern.key = pDetails.key;

						if (!StreamHelpers.isValidStream(KeyUtils.findKey("key:00B2D882:00000000:" + StringHelpers.HashFNV64(pDetails.name.Substring(pDetails.name.LastIndexOf("\\") + 1)).ToString("X16"))))
                        {
                            if (String.IsNullOrEmpty(pDetails.BackgroundImage))
                            {
                                cPattern.texturename = pDetails.rgbmask;
                            }
                            else
                            {
                                cPattern.texturename = pDetails.BackgroundImage;
                            }
                        }
                        else
                        {
                            cPattern.texturename = "key:00B2D882:00000000:" + StringHelpers.HashFNV64(pDetails.name.Substring(pDetails.name.LastIndexOf("\\") + 1)).ToString("X16");
                        }
                        cPattern.casPart = pDetails.name.Substring(pDetails.name.LastIndexOf("\\") + 1);

                        cPattern.subcategory = openFileDialog1.FileName;
                        pDetails.customFilename = openFileDialog1.FileName;
                        cPattern.isCustom = true;
                        pDetails.isCustom = true;

                        customPatterns.Items.Add(cPattern);

                            TextWriter r = new StreamWriter(Path.Combine(Application.StartupPath, Path.Combine("xml", "customPatterns.xml")));
                            XmlSerializer s = new XmlSerializer(typeof(patterns));
                            s.Serialize(r, customPatterns);
                            r.Close();

                        break;
                    }
                }

                //Stream patternThumb = KeyUtils.searchForKey("key:00B2D882:00000000:" + StringHelpers.HashFNV64(pDetails.name), openFileDialog1.FileName);
                //if (!Helpers.isValidStream(patternThumb))
                //{
                //    patternThumb = KeyUtils.searchForKey("key:00B2D882:00000000:" + StringHelpers.HashFNV64(pDetails.BackgroundImage), openFileDialog1.FileName);
                //}

                //if (Helpers.isValidStream(patternThumb))
                //{
                    //Stream patternDDS = File.OpenWrite(Path.Combine(Application.StartupPath, Path.Combine("patterncache", pDetails.name + ".dds")));
                    //Helpers.CopyStream(patternThumb, patternDDS, true);
                    //patternDDS.Close();

                    //patternThumb.Seek(0, SeekOrigin.Begin);

                    PictureBox picBox = new PictureBox();
                    picBox.BackColor = System.Drawing.Color.White;
                    //picBox.Location = new System.Drawing.Point(horizontal, vertical);
                    picBox.Name = pDetails.name;


                    string toolTip = pDetails.category + "\\";
                    ToolTip tt = new ToolTip();
                    toolTip += pDetails.name;
                    tt.SetToolTip(picBox, toolTip);

                    picBox.Image = Patterns.makePatternThumb(pDetails, castdb);
                    //picBox.Image =     castdb.GetResourceStream(entry.Key);
                    try
                    {
                        picBox.Image.Save(Path.Combine(Application.StartupPath, Path.Combine("patterncache", pDetails.name + ".png")), System.Drawing.Imaging.ImageFormat.Png);
                    }
                    catch (Exception)
                    {
                    }

                    addToPanel(picBox);


                    cast.Close();
                    //picBox.Dispose();
                //}

                comboBox1.SelectedIndex = 15;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            foreach (patternsFile pattern in customPatterns.Items)
            {
                if (File.Exists(Path.Combine(Application.StartupPath, Path.Combine("patterncache", pattern.casPart + ".png"))))
                {
                    try {
                        File.Delete(Path.Combine(Application.StartupPath, Path.Combine("patterncache", pattern.casPart + ".png")));
                    }
                    catch(Exception)
                    {
                    }
                }
            }


            customPatterns.Items.Clear();
            TextWriter r = new StreamWriter(Path.Combine(Application.StartupPath, Path.Combine("xml", "customPatterns.xml")));
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
        [System.Xml.Serialization.XmlElementAttribute("pattern", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = true)]
        public List<patternsFile> Items = new List<patternsFile>();

    }
    
 
    /// <remarks/>
    public class patternsFile
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string key;

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string texturename;

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string category;

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string subcategory;

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
