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

                    sb.AppendLine("<file groupid=\"" + lookupList.Items[i].groupid + "\" instanceid=\"" + lookupList.Items[i].instanceid + "\" typeid=\"" + lookupList.Items[i].typeid + "\" category=\"" + category + "\" subcategory=\"" + subCategory + "\" texturename=\"" + fullName + "\" color0=\"" + color0 + "\" color1=\"" + color1 + "\" color2=\"" + color2 + "\" color3=\"" + color3 + "\" color4=\"" + color4 + "\">" + lookupList.Items[i].fullCasPartname + "</file>");

                }
                xtr.Close();
                mem.Close();
            }

            Clipboard.SetText(sb.ToString());
            Console.WriteLine("Done");

        }
        patterns patterns;
        int imageWidth = 128;
        int imageHeight = 128;
        private patternsFile _selectedPattern = new patternsFile();
        public patternsFile selectedPattern = new patternsFile();
        public int curCategory = 0;

        private void loadPatternList()
        {
            Helpers.logMessageToFile("Loading patternList.xml");
            TextReader r = new StreamReader(Application.StartupPath + "\\patternList.xml");
            XmlSerializer s = new XmlSerializer(typeof(patterns));
            this.patterns = (patterns)s.Deserialize(r);
            r.Close();
            Helpers.logMessageToFile(patterns.Items.Count + " patterns found");
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            panel1.Controls.Clear();

            label2.Text = "Loading patterns... please wait...";
            label2.Refresh();

            string s3root = MadScience.Helpers.findSims3Root();
            string thumbnailPackage = @"\GameData\Shared\Packages\FullBuild2.package";

            Gibbed.Sims3.FileFormats.DatabasePackedFile dbpf = new Gibbed.Sims3.FileFormats.DatabasePackedFile();
            Stream cast = null;
            Gibbed.Sims3.FileFormats.Database castdb = null;

            Dictionary<ulong, Gibbed.Sims3.FileFormats.DatabasePackedFile.Entry> entries = new Dictionary<ulong, Gibbed.Sims3.FileFormats.DatabasePackedFile.Entry>();

            if (s3root != "" && File.Exists(s3root + thumbnailPackage))
            {
                // Open CAS Thumbnails package
                cast = File.Open(s3root + thumbnailPackage, FileMode.Open, FileAccess.Read, FileShare.Read);
                castdb = new Gibbed.Sims3.FileFormats.Database(cast, true);
                cast.Seek(0, SeekOrigin.Begin);
                try
                {
                    dbpf.Read(cast);
                }
                catch (Gibbed.Sims3.FileFormats.NotAPackageException)
                {
                    MessageBox.Show("bad file: {0}", s3root + thumbnailPackage);
                    cast.Close();
                    return;
                }

                for (int j = 0; j < dbpf.Entries.Count; j++)
                {
                    if ((dbpf.Entries[j].Key.GroupId == 0x00000000) && (dbpf.Entries[j].Key.TypeId == 0x00B2D882)) {
                        entries.Add(dbpf.Entries[j].Key.InstanceId, dbpf.Entries[j]);
                    }
                }

            }

            int numFound = 0;
            int horizontal = 0;
            int vertical = 0;

            bool hasFound = false;
            
            ToolTip tt = new ToolTip();

            int curWidth = panel1.Width;

            int maxAcross = curWidth / imageWidth;
            if ((maxAcross * (imageWidth + 6)) > curWidth) maxAcross--;

            DdsFileTypePlugin.DdsFile ddsP = new DdsFileTypePlugin.DdsFile();

            Console.WriteLine("Starting at: " + DateTime.Now.ToString());

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
                        picBox.Image = Image.FromFile(Application.StartupPath + "\\patterncache\\" + pattern.casPart + ".png");
                    }
                    else
                    {
                            ulong instanceid = Gibbed.Helpers.StringHelpers.HashFNV64(pattern.casPart);
                            Gibbed.Sims3.FileFormats.DatabasePackedFile.Entry entry = new Gibbed.Sims3.FileFormats.DatabasePackedFile.Entry();
                            if (entries.ContainsKey(instanceid))
                            {
                                entry = entries[instanceid];
                                hasFound = true;
                            } 
                            else 
                            {
                                instanceid = Gibbed.Helpers.StringHelpers.HashFNV64(pattern.texturename);
                                if (entries.ContainsKey(instanceid)) 
                                {
                                    entry = entries[instanceid];
                                    hasFound = true;
                                }
                            }
                            if (hasFound == true) 
                            {
                                ddsP.Load(castdb.GetResourceStream(entry.Key));
                                // Figure out colour channels
                                if (pattern.color1 == "" && pattern.color2 == "" && pattern.color3 == "" && pattern.color4 == "")
                                {
                                    picBox.Image = ddsP.Image();
                                }
                                else
                                {
                                    picBox.Image = ddsP.Image(Color.Black, Helpers.convertColour(pattern.color1, true), Helpers.convertColour(pattern.color2, true), Helpers.convertColour(pattern.color3, true), Helpers.convertColour(pattern.color4, true));
                                    //picBox.Image = Helpers.imagePreview(ddsP.Image(), Color.Black, Helpers.convertColour(pattern.color1, true), Helpers.convertColour(pattern.color2, true), Helpers.convertColour(pattern.color3, true), Helpers.convertColour(pattern.color4, true));
                                }
                             
                                //picBox.Image =     castdb.GetResourceStream(entry.Key);
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
                    if ((numFound % maxAcross) == 0) { 
                        vertical += imageHeight + 6; horizontal = 0; 
                    }
                    else
                    {
                        horizontal += imageWidth + 6;
                    }

                }
            }

            Console.WriteLine("Stopping at: " + DateTime.Now.ToString());

            label2.Text = numFound.ToString() + " patterns";
            cast.Close();
            dbpf = null;
            castdb = null;

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

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = this.curCategory;
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

            this.selectedPattern = this._selectedPattern;

            this.selectedPattern.instanceid = this.selectedPattern.instanceid.Remove(0, 2);
            this.selectedPattern.typeid = this.selectedPattern.typeid.Remove(0, 2);
            this.selectedPattern.groupid = this.selectedPattern.groupid.Remove(0, 2);

            this.Close();
        }

        private void PatternBrowser_Shown(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = this.curCategory;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlRootAttribute()]
    public class patterns
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("file", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = true)]
        public List<patternsFile> Items = new List<patternsFile>();

    }

    /// <remarks/>
    public class patternsFile
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

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string subcategory;

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string category;

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string texturename;

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


        /// <remarks/>

        private string _casPart = "";
        [System.Xml.Serialization.XmlTextAttribute()]
        public string casPart
        {
            get { return this._casPart; }
            set
            {
                this._casPart = value;
            }
        }

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
