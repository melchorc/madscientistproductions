using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;
using OX.Copyable;
using MadScience.Wrappers;
using MadScience;

namespace CASPartEditor
{
    public partial class Form1 : Form
    {

        public Form1()
        {

            //Helpers.logPath = Application.StartupPath + "\\" + Application.ProductName + ".log";
            Helpers.logMessageToFile("Initialising CTU main form");

            InitializeComponent();

            Helpers.logMessageToFile("Setting icon");
            //this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            pBrowser.Icon = this.Icon;

            //MadScience.Helpers.productName = Application.ProductName;

            Helpers.logMessageToFile("Checking license");
            MadScience.Helpers.checkAndShowLicense(Application.ProductName);

            Helpers.logMessageToFile("Setting version number");
            Version vrs = new Version(Application.ProductVersion);
            this.Text = this.Text + " v" + vrs.Major + "." + vrs.Minor + "." + vrs.Build + "." + vrs.Revision;

            Helpers.logMessageToFile("Checking debug mode");
            if (MadScience.Helpers.getRegistryValue("debugMode") == "True")
            {
                debugModeToolStripMenuItem.Checked = true;
            }

            Helpers.logMessageToFile("Creating cache folder");
            if (!Directory.Exists(Application.StartupPath + "\\cache\\"))
            {
                Directory.CreateDirectory(Application.StartupPath + "\\cache\\");
            }

            if (MadScience.Helpers.getRegistryValue("show3dRender") == "True")
            {
                cEnable3DPreview.Checked = true;
                cEnable3DPreview_CheckedChanged(null, null);
            }
            else
            {
                cEnable3DPreview.Checked = false;
            }

            if (String.IsNullOrEmpty(MadScience.Helpers.getRegistryValue("renderBackgroundColour")))
            {
                MadScience.Helpers.saveRegistryValue("renderBackgroundColour", MadScience.Helpers.convertColour(Color.SlateBlue));
            }

            renderWindow1.BackColor = MadScience.Helpers.convertColour(MadScience.Helpers.getRegistryValue("renderBackgroundColour"));

            lookupTypes();

            Helpers.logMessageToFile("Populating types list");
            MadScience.Helpers.lookupTypes(Application.StartupPath + "\\metaTypes.xml");

            Helpers.logMessageToFile("Finished Initialisation");
        }

        public List<stencilDetails> stencilPool = new List<stencilDetails>(20);

        public files lookupList;
        public casPart casPartNew;
        public casPart casPartSrc = new casPart();

        public bool isNew = false;
        public bool fromPackage = false;

        string logPath = Helpers.logPath(Application.StartupPath + "\\" + Application.ProductName + ".log", true);

        //private int patternBrowserCategory = 0;
        PatternBrowser.PatternBrowser pBrowser = new PatternBrowser.PatternBrowser();

        //public string filename;

        //public Hashtable newDDSFiles = new Hashtable();
        public Hashtable newVPXYFiles = new Hashtable();
        public Hashtable newGEOMFiles = new Hashtable();
        public Dictionary<int, string> newPNGfiles = new Dictionary<int, string>();

        private void lookupTypes()
        {

            Helpers.logMessageToFile("LookupTypes");

            TextReader r = new StreamReader(Application.StartupPath + "\\files.xml");
            XmlSerializer s = new XmlSerializer(typeof(files));
            this.lookupList = (files)s.Deserialize(r);
            lookupList.makeCTypes();
            r.Close();

            Helpers.logMessageToFile("lookupList length: " + lookupList.Items.Length);

            foreach (string cType in lookupList.cTypes)
            {
                string blah = "";
                if (cType == "") { blah = "(none)"; } else { blah = cType; }
                cmbPartTypes.Items.Add(blah);
            }
            cmbPartTypes.Sorted = true;

            Helpers.logMessageToFile("LookupTypes done");
        }

        private void displayCasPartFile()
        {
            //this.casPartNew = (casPart)casPartSrc.Copy();

            listView1.Items.Clear();
            setComboBoxes(casPartSrc.meshName);
            /*
            if (casPart.xmlChunkRaw.Count > 0) { cmbStencilList.Items.Clear(); }
            for (int i = 0; i < casPart.xmlChunkRaw.Count; i++)
            {
                cmbStencilList.Items.Add("Stencil #" + (i + 1));
            }
            if (casPart.xmlChunkRaw.Count > 0) { cmbStencilList.SelectedIndex = 0; }
            */
            //textBox1.Text = (string)casPart.xmlChunk[0];

            for (int i = 1; i <= casPartSrc.xmlChunk.Count; i++)
            {
                ListViewItem item = new ListViewItem();
                item.Text = "Design #" + i.ToString();
                listView1.Items.Add(item);
                if (i == 1) listView1.Items[0].Selected = true;
            }

            /*
            chkStencilAEnabled.Enabled = true;
            chkStencilBEnabled.Enabled = true;
            chkStencilCEnabled.Enabled = true;
            chkStencilDEnabled.Enabled = true;
            chkStencilEEnabled.Enabled = true;
            chkStencilFEnabled.Enabled = true;
            */
            chkPatternAEnabled.Enabled = true;
            //chkPatternBEnabled.Enabled = true;
            //chkPatternCEnabled.Enabled = true;
            chkLogoEnabled.Enabled = true;

        }

        private void setComboBoxes(string casPartName)
        {
            cmbSimAge.Enabled = true;
            cmbSimGender.Enabled = true;
            cmbPartTypes.Enabled = true;

            string temp2 = casPartName.Substring(1, 1);
            switch (temp2)
            {
                case "f":
                    cmbSimGender.SelectedIndex = 0;
                    break;
                case "m":
                    cmbSimGender.SelectedIndex = 1;
                    break;
                case "u":
                    cmbSimGender.SelectedIndex = 2;
                    break;
            }

            string temp3 = casPartName.Substring(0, 1);
            switch (temp3)
            {
                case "a":
                    cmbSimAge.SelectedIndex = 4;
                    break;
                case "t":
                    cmbSimAge.SelectedIndex = 2;
                    break;
                case "y":
                    cmbSimAge.SelectedIndex = 3;
                    break;
                case "e":
                    cmbSimAge.SelectedIndex = 5;
                    break;
                case "p":
                    cmbSimAge.SelectedIndex = 0;
                    break;
                case "c":
                    cmbSimAge.SelectedIndex = 1;
                    break;
                case "u":
                    cmbSimAge.SelectedIndex = 6;
                    break;
            }

            string temp = casPartName.Remove(0, 2);
            bool isSet = false;
            string tS = "";

            if (temp.ToLower().StartsWith("accessory")) { tS = "Accessory"; cmbPartTypes.SelectedIndex = 1; isSet = true; }
            if (temp.ToLower().StartsWith("beard")) { tS = "Beard"; cmbPartTypes.SelectedIndex = 2; isSet = true; }
            if (temp.ToLower().StartsWith("body")) { tS = "Body"; cmbPartTypes.SelectedIndex = 3; isSet = true; }
            if (temp.ToLower().StartsWith("bottom")) { tS = "Bottom"; cmbPartTypes.SelectedIndex = 4; isSet = true; }
            if (temp.ToLower().StartsWith("costume")) { tS = "Costume"; cmbPartTypes.SelectedIndex = 5; isSet = true; }
            if (temp.ToLower().StartsWith("hair")) { tS = "Hair"; cmbPartTypes.SelectedIndex = 6; isSet = true; }
            if (temp.ToLower().StartsWith("makeup")) { tS = "Makeup"; cmbPartTypes.SelectedIndex = 7; isSet = true; }
            if (temp.ToLower().StartsWith("shoes")) { tS = "Shoes"; cmbPartTypes.SelectedIndex = 8; isSet = true; }
            if (temp.ToLower().StartsWith("top")) { tS = "Top"; cmbPartTypes.SelectedIndex = 9; isSet = true; }
            if (isSet == false) { cmbPartTypes.SelectedIndex = 0; }

            if (tS != "")
            {
                temp = temp.Replace(tS, "");
            }

            isSet = false;
            for (int i = 0; i < cmbMeshName.Items.Count; i++)
            {
                if (cmbMeshName.Items[i].ToString() == temp)
                {
                    isSet = true;
                    cmbMeshName.SelectedIndex = i;
                    break;
                }
            }

            if (!isSet)
            {
                cmbMeshName.Items.Add("* Custom");
                cmbMeshName.SelectedIndex = cmbMeshName.Items.Count - 1;
            }

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "Sims 3 Package|*.package|CAS Part File|*.caspart|All Files|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK && openFileDialog1.FileName.Trim() != "")
            {
                loadFile(openFileDialog1.FileName);
            }
        }

        public void loadFile(string filename)
        {
            toolStripStatusLabel1.Text = filename;

            if (debugModeToolStripMenuItem.Checked)
            {
                Helpers.logMessageToFile("Opening file " + filename);
            }

            casPartFile cPartFile;
            Stream inputCasPart = new MemoryStream(); ;

            FileInfo f = new FileInfo(filename);
            if (f.Extension.ToLower() == ".caspart")
            {
                inputCasPart = File.Open(f.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);
                cPartFile = new casPartFile();
                this.casPartSrc = cPartFile.Load(inputCasPart);
                inputCasPart.Close();
                this.isNew = false;
                this.fromPackage = false;
                this.stencilPool.Clear();
                //this.stencilPool.TrimExcess();

                displayCasPartFile();
            }
            else if (f.Extension.ToLower() == ".package")
            {
                inputCasPart = searchInPackage(f.FullName, (int)0x034AEECB, -1, -1);

                if (inputCasPart != null)
                {
                    cPartFile = new casPartFile();
                    this.casPartSrc = cPartFile.Load(inputCasPart);
                    inputCasPart.Close();
                    this.isNew = false;
                    this.fromPackage = true;
                    Helpers.currentPackageFile = f.FullName;
                    this.stencilPool.Clear();
                    //this.stencilPool.TrimExcess();

                    saveToolStripMenuItem.Enabled = true;
                    copyDefaultsToolStripMenuItem.Enabled = false;

                    displayCasPartFile();
                }
                else
                {
                    MessageBox.Show("No CAS Part file can be found in this package!");
                }
            }
            saveAsToolStripMenuItem.Enabled = true;

        }

        public Stream searchInPackage(string filename, string keyString)
        {
            keyString = keyString.Replace("key:", "");
            string[] temp = keyString.Split(":".ToCharArray());

            int typeID = (int)MadScience.StringHelpers.ParseHex32("0x" + temp[0]);
            int groupID = (int)MadScience.StringHelpers.ParseHex32("0x" + temp[1]);
            long instanceID = (long)MadScience.StringHelpers.ParseHex64("0x" + temp[2]);

            return searchInPackage(filename, typeID, groupID, instanceID);
        }

        ResourceKey loadedCasPart = new ResourceKey();
        public Stream searchInPackage(string filename, int typeID, int groupID, long instanceID)
        {

            Stream matchChunk = null;

            if (filename == "" || filename == null) { return matchChunk; }

            // Open the package file and search
            Stream package = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
            MadScience.Wrappers.Database db = new MadScience.Wrappers.Database(package, true);

            int searchType = 0;
            if (typeID != -1) { searchType += 1; }
            if (groupID != -1) { searchType += 2; }
            if (instanceID != -1) { searchType += 4; }

            bool foundMatch = false;



            foreach (MadScience.Wrappers.ResourceKey entry in db._Entries.Keys) 
            {
                //ResourceKey key = db.Entries.Keys[i];
                //DatabasePackedFile.Entry entry = db.Entries.Keys[i];
                //DatabasePackedFile.Entry entry = db.dbpfEntries[i];
                //MadScience.Wrappers.ResourceKey entry = new MadScience.Wrappers.ResourceKey(keyString);

                switch (searchType)
                {
                    case 7:
                        if (entry.typeId == typeID && entry.groupId == groupID && entry.instanceId == (ulong)instanceID)
                        {
                            loadedCasPart = entry;
                            matchChunk = db.GetResourceStream(entry);
                            foundMatch = true;
                        }
                        break;
                    case 6:
                        if (entry.groupId == groupID && entry.instanceId == (ulong)instanceID)
                        {
                            loadedCasPart = entry;
                            matchChunk = db.GetResourceStream(entry);
                            foundMatch = true;
                        }
                        break;
                    case 5:
                        if (entry.typeId == typeID && entry.instanceId == (ulong)instanceID)
                        {
                            loadedCasPart = entry;
                            matchChunk = db.GetResourceStream(entry);
                            foundMatch = true;
                        }
                        break;
                    case 4:
                        if (entry.instanceId == (ulong)instanceID)
                        {
                            loadedCasPart = entry;
                            matchChunk = db.GetResourceStream(entry);
                            foundMatch = true;
                        }
                        break;
                    case 3:
                        if (entry.typeId == typeID && entry.groupId == groupID)
                        {
                            loadedCasPart = entry;
                            matchChunk = db.GetResourceStream(entry);
                            foundMatch = true;
                        }
                        break;
                    case 2:
                        if (entry.groupId == groupID)
                        {
                            loadedCasPart = entry;
                            matchChunk = db.GetResourceStream(entry);
                            foundMatch = true;
                        }
                        break;
                    case 1:
                        if (entry.typeId == typeID)
                        {
                            loadedCasPart = entry;
                            matchChunk = db.GetResourceStream(entry);
                            foundMatch = true;
                        }
                        break;
                }
                if (foundMatch)
                {
                    break;
                }

            }
            package.Close();

            return matchChunk;

        }


        private void cmbSimGender_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSimAge.Text != "" && cmbPartTypes.Text != "")
            {
                getMeshNames(cmbSimGender.Text, cmbSimAge.Text, cmbPartTypes.Text);
            }
        }

        private void cmbSimAge_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSimGender.Text != "" && cmbPartTypes.Text != "")
            {
                getMeshNames(cmbSimGender.Text, cmbSimAge.Text, cmbPartTypes.Text);
            }
        }

        private void cmbPartTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSimAge.Text != "" && cmbSimGender.Text != "")
            {
                getMeshNames(cmbSimGender.Text, cmbSimAge.Text, cmbPartTypes.Text);
            }
        }

        private void pictureBoxMeshPic_Click(object sender, EventArgs e)
        {

            for (int i = 0; i < panelMeshThumbs.Controls.Count; i++)
            {
                PictureBox temp2 = (PictureBox)panelMeshThumbs.Controls[i];
                temp2.BorderStyle = BorderStyle.None;
            }

            PictureBox temp = (PictureBox)sender;
            //toolStripStatusLabel1.Text = temp.Name;
            temp.BorderStyle = BorderStyle.FixedSingle;

            if (cmbMeshName.SelectedItem.ToString() != temp.Name)
            {
                for (int i = 0; i < cmbMeshName.Items.Count; i++)
                {
                    if ((string)cmbMeshName.Items[i] == temp.Name)
                    {
                        cmbMeshName.SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        int meshPicThumbsCurrent = 0;
        int meshPicHorizontal = 0;
        int meshPicVertical = 0;
        private void addMeshPicToPanel(string meshName, string shortName)
        {
            ToolTip tt = new ToolTip();
            PictureBox picBox = new PictureBox();
            picBox.BackColor = System.Drawing.Color.White;
            picBox.Location = new System.Drawing.Point(this.meshPicHorizontal, this.meshPicVertical);
            picBox.Name = shortName;
            picBox.Tag = meshName;
            picBox.Size = new System.Drawing.Size(128, 128);
            picBox.Click += new System.EventHandler(pictureBoxMeshPic_Click);
            picBox.SizeMode = PictureBoxSizeMode.StretchImage;

            tt.SetToolTip(picBox, meshName);

            int curWidth = panelMeshThumbs.Width;

            int maxAcross = curWidth / 128;
            if ((maxAcross * (128 + 6)) > curWidth) maxAcross--;

            // Find thumbnail
            if (!File.Exists(Application.StartupPath + "\\cache\\" + meshName + ".png"))
            {
                picBox.Image = extractCASThumbnail(meshName);
            }
            else
            {
                Stream picBoxImage = File.OpenRead(Application.StartupPath + "\\cache\\" + meshName + ".png");
                picBox.Image = Image.FromStream(picBoxImage);
                picBoxImage.Close();
            }

            this.meshPicThumbsCurrent++;

            // Add picturebox to panel
            panelMeshThumbs.Controls.Add(picBox);

            // Calc picture box horizontal and vertical
            if ((this.meshPicThumbsCurrent % maxAcross) == 0)
            {
                this.meshPicVertical += 128 + 6; this.meshPicHorizontal = 0;
            }
            else
            {
                this.meshPicHorizontal += 128 + 6;
            }

        }

        private void getMeshNames(string gender, string age, string parttype)
        {
            cmbMeshName.Items.Clear();

            panelMeshThumbs.Controls.Clear();
            meshPicHorizontal = 0;
            meshPicVertical = 0;
            meshPicThumbsCurrent = 0;

            txtCasPartInstance.Text = "";
            txtCasPartName.Text = "";

            label110.Visible = true;
            label110.Refresh();

            panelMeshThumbs.Visible = false;

            if (parttype == "(none)") { parttype = ""; }

            int numFound = 0;

            for (int i = 0; i < this.lookupList.Items.Length; i++)
            {
                filesFile f = lookupList.Items[i];
                if (f.cAge == age && f.cGender == gender && f.cType == parttype)
                {
                    cmbMeshName.Items.Add(f.casPart);
                    addMeshPicToPanel(f.fullCasPartname, f.casPart);
                    numFound++;
                }
            }
            label110.Visible = false;
            panelMeshThumbs.Visible = true;

            if (cmbMeshName.Items.Count == 0)
            {
                cmbMeshName.Enabled = false;
                groupBox11.Enabled = false;
                groupBox11.Visible = false;
                button9.Enabled = false;
                label70.Visible = true;
                btnDumpFromFullbuild2.Enabled = false;
            }
            else
            {
                cmbMeshName.Enabled = true;
                groupBox11.Enabled = true;
                groupBox11.Visible = true;
                label70.Visible = false;
                btnDumpFromFullbuild2.Enabled = true;
                button9.Enabled = true;
                cmbMeshName.SelectedIndex = 0;

                groupBox11.Text = "Quick Find (" + numFound.ToString() + " found)";
            }

        }

        Dictionary<ulong, ResourceKey> casThumbsKeyList = new Dictionary<ulong, ResourceKey>();
        Dictionary<ulong, ResourceKey> casThumbsKeyList2 = new Dictionary<ulong, ResourceKey>();
        private Image extractCASThumbnail(string meshName)
        {

            //Console.WriteLine("Started " + meshName + " at " + DateTime.Now.ToString());

            Helpers.logMessageToFile("Attemping to extract thumbnail for " + meshName);

            Image tempImage = null;

            string s3root = MadScience.Helpers.findSims3Root();
            string thumbnailPackage = @"\Thumbnails\CASThumbnails.package";
            if (s3root != "" && File.Exists(s3root + thumbnailPackage))
            {
                // Open CAS Thumbnails package
                Stream cast = File.Open(s3root + thumbnailPackage, FileMode.Open, FileAccess.Read, FileShare.Read);
                Database castdb = new Database(cast, true);

                //cast.Seek(0, SeekOrigin.Begin);
                //DatabasePackedFile dbpf = new DatabasePackedFile();
                //try
                //{
                //dbpf.Read(cast);
                //}
                //catch (NotAPackageException)
                //{
                //  MessageBox.Show("bad file: {0}", s3root + thumbnailPackage);
                //                    cast.Close();
                //                  return null ;
                //            }
                ulong instanceid = MadScience.StringHelpers.HashFNV64(meshName);

                if (casThumbsKeyList.Count == 0)
                {
                    Helpers.logMessageToFile("Populating casThumbs entry lists from " + castdb._Entries.Count.ToString() + " entries");
                    foreach (MadScience.Wrappers.ResourceKey entry in castdb._Entries.Keys)
                    {
                        //DatabasePackedFile.Entry entry = castdb.dbpfEntries[i];
                        if (entry.groupId == 0x00000000 && entry.typeId == 0x626f60ce)
                        {
                            casThumbsKeyList.Add(entry.instanceId, entry);
                        }
                        if (entry.groupId == 0x00000001 && entry.typeId == 0x626f60ce)
                        {
                            casThumbsKeyList2.Add(entry.instanceId, entry);
                        }
                    }
                    Helpers.logMessageToFile("casThumbs now contains " + casThumbsKeyList.Count.ToString() + " entries");
                    Helpers.logMessageToFile("casThumbs2 now contains " + casThumbsKeyList2.Count.ToString() + " entries");
                }

                bool foundPic = false;
                ResourceKey temp = new ResourceKey();
                if (casThumbsKeyList.ContainsKey(instanceid))
                {
                    temp = casThumbsKeyList[instanceid];
                    foundPic = true;
                }
                else
                {
                    if (casThumbsKeyList2.ContainsKey(instanceid))
                    {
                        temp = casThumbsKeyList2[instanceid];
                        foundPic = true;
                    }
                }

                if (foundPic)
                {
                    tempImage = Image.FromStream(castdb.GetResourceStream(temp));
                    try
                    {
                        tempImage.Save(Application.StartupPath + "\\cache\\" + meshName + ".png", System.Drawing.Imaging.ImageFormat.Png);
                    }
                    catch (Exception ex)
                    {
                    }

                }
                else
                {
                    Helpers.logMessageToFile("Couldn't find a match for " + meshName + " (0x" + instanceid.ToString("X16") + ")");
                }

                cast.Close();
                //dbpf = null;
                castdb = null;
            }
            else
            {
                Helpers.logMessageToFile(@"Can't find sims3root or Thumbnails\CASThumbnails.package");
            }

            //Console.WriteLine("Stopped " + meshName + " at " + DateTime.Now.ToString());

            return tempImage;
        }

        private void scrollPanelToThumb(string meshName)
        {
            for (int i = 0; i < panelMeshThumbs.Controls.Count; i++)
            {
                PictureBox temp = (PictureBox)panelMeshThumbs.Controls[i];
                temp.BorderStyle = BorderStyle.None;

                if (panelMeshThumbs.Controls[i].Tag.ToString() == meshName)
                {
                    panelMeshThumbs.ScrollControlIntoView(panelMeshThumbs.Controls[i]);
                    temp.BorderStyle = BorderStyle.FixedSingle;
                    break;
                }
            }
        }

        private void cmbMeshName_SelectedIndexChanged(object sender, EventArgs e)
        {
 
            string meshName = "";

            // Translate selection back into af am etc
            switch (cmbSimAge.Text)
            {
                case "Toddler":
                    meshName += "p";
                    break;
                case "Child":
                    meshName += "c";
                    break;
                case "Teen":
                    meshName += "t";
                    break;
                case "Young Adult":
                    meshName += "y";
                    break;
                case "Adult":
                    meshName += "a";
                    break;
                case "Elder":
                    meshName += "e";
                    break;
                case "All Ages":
                    meshName += "u";
                    break;
            }

            switch (cmbSimGender.Text)
            {
                case "Female":
                    meshName += "f";
                    break;
                case "Male":
                    meshName += "m";
                    break;
                case "Unisex":
                    meshName += "u";
                    break;
            }

            if (cmbMeshName.Text == "* Custom")
            {

                //if (this.isNew == false)
                //{
                    //meshName += casPartSrc.meshName;
                //}
                txtCasPartName.Text = casPartSrc.meshName;
                txtMeshName.Text = casPartSrc.meshName;

                txtCasPartInstance.Text = MadScience.StringHelpers.HashFNV64(meshName).ToString("X16");
                picMeshPreview.Image = null;
                picMeshPreview.Invalidate();

            }
            else
            {

                if (cmbPartTypes.Text == "(none)")
                {
                    meshName += cmbMeshName.Text;
                }
                else
                {
                    meshName += cmbPartTypes.Text + cmbMeshName.Text;
                }

                txtCasPartName.Text = meshName;
                txtMeshName.Text = meshName;

                scrollPanelToThumb(meshName);

                for (int i = 0; i < this.lookupList.Items.Length; i++)
                {
                    filesFile f = lookupList.Items[i];
                    if (f.fullCasPartname == meshName)
                    {
                        txtCasPartInstance.Text = f.instanceid;
                        break;
                    }
                }

                picMeshPreview.Image = null;
                picMeshPreview.Invalidate();

                // Find thumbnail
                if (File.Exists(Application.StartupPath + "\\cache\\" + meshName + ".png"))
                {
                    Stream picMeshPreviewStream = File.OpenRead(Application.StartupPath + "\\cache\\" + meshName + ".png");
                    picMeshPreview.Image = Image.FromStream(picMeshPreviewStream);
                    picMeshPreviewStream.Close();
                }
                else
                {
                    extractCASThumbnail(meshName);
                }
            }

            if (this.isNew == true)
            {
                // Attempt to load the existing caspart into memory so we can extract data later.
                Stream casPartFile = File.Open(Application.StartupPath + "\\casparts\\" + txtCasPartName.Text + ".caspart", FileMode.Open, FileAccess.Read, FileShare.Read);
                casPartFile cPartFile = new casPartFile();
                this.casPartSrc = cPartFile.Load(casPartFile);
                casPartFile.Close();
            }

            for (int i = 0; i < checkedListAge.Items.Count; i++)
            {
                checkedListAge.SetItemChecked(i, false);
            }
            for (int i = 0; i < checkedListCategory.Items.Count; i++)
            {
                checkedListCategory.SetItemChecked(i, false);
            }
            for (int i = 0; i < checkedListCategoryExtended.Items.Count; i++)
            {
                checkedListCategoryExtended.SetItemChecked(i, false);
            }
            for (int i = 0; i < checkedListGender.Items.Count; i++)
            {
                checkedListGender.SetItemChecked(i, false);
            }
            for (int i = 0; i < checkedListOther.Items.Count; i++)
            {
                checkedListOther.SetItemChecked(i, false);
            }
            for (int i = 0; i < checkedListType.Items.Count; i++)
            {
                checkedListType.SetItemChecked(i, false);
            }


            lstCasPartDetails.Items.Clear();
            // Populate the CAS Part Details 
            addCasPartItem("Mesh Name", casPartSrc.meshName);
            addCasPartItem("Clothing Order", casPartSrc.clothingOrder.ToString());
            addCasPartItem("CAS Part Type", casPartSrc.clothingType.ToString() + " (0x" + casPartSrc.clothingType.ToString("X8") + ")");
            addCasPartItem("Type", casPartSrc.typeFlag.ToString() + " (0x" + casPartSrc.typeFlag.ToString("X8") + ")");
            addCasPartItem("Age/Gender", casPartSrc.ageGenderFlag.ToString() + " (0x" + casPartSrc.ageGenderFlag.ToString("X8") + ")");
            addCasPartItem("Clothing Category", casPartSrc.clothingCategory.ToString() + " (0x" + casPartSrc.clothingCategory.ToString("X8") + ")");
            addCasPartItem("Unk String", casPartSrc.unkString);

            addCasPartItem("Unk2", casPartSrc.unk2.ToString());
            addCasPartItem("TGI Index Body Part 1", casPartSrc.tgiIndexBodyPart1.ToString());
            addCasPartItem("TGI Index Body Part 2", casPartSrc.tgiIndexBodyPart2.ToString());
            addCasPartItem("TGI Index Blend Info Fat", casPartSrc.tgiIndexBlendInfoFat.ToString());
            addCasPartItem("TGI Index Blend Info Fit", casPartSrc.tgiIndexBlendInfoFit.ToString());
            addCasPartItem("TGI Index Blend Info Thin", casPartSrc.tgiIndexBlendInfoThin.ToString());
            addCasPartItem("TGI Index Blend Info Special", casPartSrc.tgiIndexBlendInfoSpecial.ToString());
            addCasPartItem("Unk5", casPartSrc.unk5.ToString());
            addCasPartItem("VPXY", casPartSrc.tgiIndexVPXY.ToString());

            //tgi64 tempvpxy = (tgi64)casPartSrc.tgi64list[casPartSrc.tgiIndexVPXY];
            //txtVPXYPrimary.Text = "key:" + tempvpxy.typeid.ToString("X8") + ":" + tempvpxy.groupid.ToString("X8") + ":" + tempvpxy.instanceid.ToString("X16");

            addCasPartItem("Count 2", casPartSrc.count2.ToString());
            for (int i = 0; i < casPartSrc.count2; i++)
            {
                unkRepeat unk = (unkRepeat)casPartSrc.count2repeat[i];
                addCasPartItem("#" + i.ToString() + ": unkNum ", unk.unkNum.ToString());
                addCasPartItem("#" + i.ToString() + ": unk2", unk.unk2.ToString());
                addCasPartItem("#" + i.ToString() + ": unkRepeatInner", unk.unkRepeatInnerCount.ToString());
                for (int j = 0; j < unk.unkRepeatInnerCount; j++)
                {
                    intTriple iT = (intTriple)unk.unkRepeatInnerLoop[j];
                    addCasPartItem("#" + i.ToString() + "." + j.ToString() + ": One", iT.one.ToString());
                    addCasPartItem("#" + i.ToString() + "." + j.ToString() + ": Two", iT.two.ToString());
                    addCasPartItem("#" + i.ToString() + "." + j.ToString() + ": Three", iT.three.ToString());
                }
            }

            addCasPartItem("TGI Index Diffuse", casPartSrc.tgiIndexDiffuse.ToString());
            addCasPartItem("TGI Index Specular", casPartSrc.tgiIndexSpecular.ToString());

            addCasPartItem("Diffuse Links", casPartSrc.count3.ToString());
            for (int i = 0; i < casPartSrc.count3; i++)
            {
                byte cRepeat = (byte)casPartSrc.count3repeat[i];
                addCasPartItem("#" + i.ToString(), cRepeat.ToString());
            }

            addCasPartItem("Specular Links", casPartSrc.count4.ToString());
            for (int i = 0; i < casPartSrc.count4; i++)
            {
                byte cRepeat = (byte)casPartSrc.count4repeat[i];
                addCasPartItem("#" + i.ToString(), cRepeat.ToString());
            }

            addCasPartItem("Count 5", casPartSrc.count5.ToString());
            for (int i = 0; i < casPartSrc.count5; i++)
            {
                byte cRepeat = (byte)casPartSrc.count5repeat[i];
                addCasPartItem("#" + i.ToString(), cRepeat.ToString());
            }

            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                checkedListBox1.SetItemChecked(i, true);
            }

            for (int i = 0; i < casPartSrc.count6; i++)
            {
                MadScience.Wrappers.ResourceKey tgi = casPartSrc.tgi64list[i];
                string tgiType = MadScience.Helpers.findMetaEntry(tgi.typeId).shortName;
                Console.WriteLine(tgi.typeId.ToString() + " " + tgiType);

                if (tgi.typeId == 0x0333406C)
                {
                    if (tgi.instanceId == 0x52E8BE209C703561)
                    {
                        checkedListBox1.SetItemChecked(0, true);
                    }
                    if (tgi.instanceId == 0xE37696463F6B2D6E)
                    {
                        checkedListBox1.SetItemChecked(1, true);
                    }
                    if (tgi.instanceId == 0x01625DDC220C08C6)
                    {
                        checkedListBox1.SetItemChecked(2, true);
                    }

                }

                addCasPartItem("TGI #" + i.ToString() + " " + tgiType, tgi.ToString());
            }

            // Category flags
            if ((casPartSrc.typeFlag & 0x1) == 0x1) checkedListType.SetItemChecked(0, true); // Hair
            if ((casPartSrc.typeFlag & 0x2) == 0x2) checkedListType.SetItemChecked(1, true); // Scalp
            if ((casPartSrc.typeFlag & 0x4) == 0x4) checkedListType.SetItemChecked(2, true); // Face Overlay
            if ((casPartSrc.typeFlag & 0x8) == 0x8) checkedListType.SetItemChecked(3, true); // Body
            if ((casPartSrc.typeFlag & 0x10) == 0x10) checkedListType.SetItemChecked(4, true); // Accessory

            switch (casPartSrc.clothingType)
            {
                case 1: checkedListClothingType.SetItemChecked(0, true); break;
                case 2: checkedListClothingType.SetItemChecked(1, true); break;
                case 3: checkedListClothingType.SetItemChecked(2, true); break;
                case 4: checkedListClothingType.SetItemChecked(3, true); break;
                case 5: checkedListClothingType.SetItemChecked(4, true); break;
                case 6: checkedListClothingType.SetItemChecked(5, true); break;
                case 7: checkedListClothingType.SetItemChecked(6, true); break;
                case 11: checkedListClothingType.SetItemChecked(7, true); break;
                case 12: checkedListClothingType.SetItemChecked(8, true); break;
                case 13: checkedListClothingType.SetItemChecked(9, true); break;
                case 14: checkedListClothingType.SetItemChecked(10, true); break;
                case 15: checkedListClothingType.SetItemChecked(11, true); break;
                case 16: checkedListClothingType.SetItemChecked(12, true); break;
                case 17: checkedListClothingType.SetItemChecked(13, true); break;
                case 18: checkedListClothingType.SetItemChecked(14, true); break;
                case 19: checkedListClothingType.SetItemChecked(15, true); break;
                case 20: checkedListClothingType.SetItemChecked(16, true); break;
                case 21: checkedListClothingType.SetItemChecked(17, true); break;
                case 22: checkedListClothingType.SetItemChecked(18, true); break;
                case 24: checkedListClothingType.SetItemChecked(19, true); break;
                case 25: checkedListClothingType.SetItemChecked(20, true); break;
                case 26: checkedListClothingType.SetItemChecked(21, true); break;
                case 29: checkedListClothingType.SetItemChecked(22, true); break;
                case 30: checkedListClothingType.SetItemChecked(23, true); break;
                case 31: checkedListClothingType.SetItemChecked(24, true); break;
            }

            if ((casPartSrc.ageGenderFlag & 0x1) == 0x1) checkedListAge.SetItemChecked(0, true); // Baby
            if ((casPartSrc.ageGenderFlag & 0x2) == 0x2) checkedListAge.SetItemChecked(1, true); // Toddler
            if ((casPartSrc.ageGenderFlag & 0x4) == 0x4) checkedListAge.SetItemChecked(2, true); // Child
            if ((casPartSrc.ageGenderFlag & 0x8) == 0x8) checkedListAge.SetItemChecked(3, true); // Teen
            if ((casPartSrc.ageGenderFlag & 0x10) == 0x10) checkedListAge.SetItemChecked(4, true); // YoungAdult
            if ((casPartSrc.ageGenderFlag & 0x20) == 0x20) checkedListAge.SetItemChecked(5, true); // Adult
            if ((casPartSrc.ageGenderFlag & 0x40) == 0x40) checkedListAge.SetItemChecked(6, true); // Elder

            if ((casPartSrc.ageGenderFlag & 0x1000) == 0x1000) checkedListGender.SetItemChecked(0, true); // Male
            if ((casPartSrc.ageGenderFlag & 0x2000) == 0x2000) checkedListGender.SetItemChecked(1, true); // Female

            if ((casPartSrc.ageGenderFlag & 0x100000) == 0x100000) checkedListOther.SetItemChecked(0, true); // LeftHanded
            if ((casPartSrc.ageGenderFlag & 0x200000) == 0x200000) checkedListOther.SetItemChecked(1, true); // RightHanded
            if ((casPartSrc.ageGenderFlag & 0x10000) == 0x10000) checkedListOther.SetItemChecked(2, true); // Human

            if ((casPartSrc.clothingCategory & 0x1) == 0x1) checkedListCategory.SetItemChecked(0, true); // Naked
            if ((casPartSrc.clothingCategory & 0x2) == 0x2) checkedListCategory.SetItemChecked(1, true); // Everyday
            if ((casPartSrc.clothingCategory & 0x4) == 0x4) checkedListCategory.SetItemChecked(2, true); // Formalwear
            if ((casPartSrc.clothingCategory & 0x8) == 0x8) checkedListCategory.SetItemChecked(3, true); // Sleepwear
            if ((casPartSrc.clothingCategory & 0x10) == 0x10) checkedListCategory.SetItemChecked(4, true); // Swimwear
            if ((casPartSrc.clothingCategory & 0x20) == 0x20) checkedListCategory.SetItemChecked(5, true); // Athletic
            if ((casPartSrc.clothingCategory & 0x40) == 0x40) checkedListCategory.SetItemChecked(6, true); // Singed
            if ((casPartSrc.clothingCategory & 0x100) == 0x100) checkedListCategory.SetItemChecked(7, true); // Career
            if ((casPartSrc.clothingCategory & 0xFFFF) == 0xFFFF) checkedListCategory.SetItemChecked(8, true); // All

            if ((casPartSrc.clothingCategory & 0x100000) == 0x100000) checkedListCategoryExtended.SetItemChecked(0, true); // ValidForMaternity
            if ((casPartSrc.clothingCategory & 0x200000) == 0x200000) checkedListCategoryExtended.SetItemChecked(1, true); // ValidForRandom
            if ((casPartSrc.clothingCategory & 0x400000) == 0x400000) checkedListCategoryExtended.SetItemChecked(2, true); // IsHat
            if ((casPartSrc.clothingCategory & 0x800000) == 0x800000) checkedListCategoryExtended.SetItemChecked(3, true); // IsRevealing
            if ((casPartSrc.clothingCategory & 0x1000000) == 0x1000000) checkedListCategoryExtended.SetItemChecked(4, true); // IsHiddenInCas

            saveAsToolStripMenuItem.Enabled = true;
            btnDumpFromFullbuild2.Enabled = true;

            listView3.Items.Clear();

            // Default all stencil boxes to blank
            for (int i = 1; i <= 15; i++)
            {
                if (stencilPool.Count < i) { stencilPool.Add(new stencilDetails()); }
                //updateStencilBoxes(i, new stencilDetails());
            }



            // Calculate all the stencils so we can build up the stencil pool
            int curStencilNum = 1;
            for (int i = 0; i < casPartSrc.xmlChunk.Count; i++)
            {
                if (debugModeToolStripMenuItem.Checked)
                {
                    Helpers.logMessageToFile(casPartSrc.xmlChunkRaw[i].ToString().Replace("/><", "/>" + Environment.NewLine + "<"));
                }

                xmlChunkDetails chunk = (xmlChunkDetails)casPartSrc.xmlChunk[i];
                
                if (!inStencilList(chunk.stencil.A))
                {
                    updateStencilBoxes(curStencilNum, chunk.stencil.A);
                    curStencilNum++;

                }
                if (!inStencilList(chunk.stencil.B))
                {
                    updateStencilBoxes(curStencilNum, chunk.stencil.B);
                    curStencilNum++;

                }
                if (!inStencilList(chunk.stencil.C))
                {
                    updateStencilBoxes(curStencilNum, chunk.stencil.C);
                    curStencilNum++;

                }
                if (!inStencilList(chunk.stencil.D))
                {
                    updateStencilBoxes(curStencilNum, chunk.stencil.D);
                    curStencilNum++;

                }
                if (!inStencilList(chunk.stencil.E))
                {
                    updateStencilBoxes(curStencilNum, chunk.stencil.E);
                    curStencilNum++;

                }
                if (!inStencilList(chunk.stencil.F))
                {
                    updateStencilBoxes(curStencilNum, chunk.stencil.F);
                    curStencilNum++;
                }

            }

            for (int i = curStencilNum; i <= 15; i++)
            {
                //if (stencilPool.Count < i) { stencilPool.Add(new stencilDetails()); }
                updateStencilBoxes(i, new stencilDetails());
            }
           
            if (this.casPartSrc.xmlChunkRaw.Count == 0)
            {
                label8.Visible = true;
                listView1.Enabled = false;
                //btnDumpFromFullbuild2.Enabled = false;
                btnAddNewDesign.Enabled = false;
                btnDeleteDesign.Enabled = false;
            }
            else
            {
             
                label8.Visible = false;
                listView1.Enabled = true;
                //btnDumpFromFullbuild2.Enabled = true;
                btnAddNewDesign.Enabled = true;
                btnDeleteDesign.Enabled = true;

                this.casPartNew = (casPart)casPartSrc.Copy();
                if (this.isNew)
                {
                    this.casPartNew.xmlChunk.Clear();
                    this.casPartNew.xmlChunkRaw.Clear();
                    listView1.Items.Clear();

                    //btnAddNewDesign_Click(this, null);
                }

            }

            //Console.WriteLine(meshName);
        }

        private bool inStencilList(stencilDetails stencil)
        {
            bool inList = false;
            for (int i = 0; i < stencilPool.Count; i++)
            {
                if (stencilPool[i].key == stencil.key && stencilPool[i].Rotation == stencil.Rotation && stencilPool[i].Tiling == stencil.Tiling)
                {
                    inList = true;
                    break;
                }
            }
            return inList;
        }

        private int getStencilFromPool(stencilDetails stencil)
        {
            int temp = 0;
            for (int i = 0; i < stencilPool.Count; i++)
            {
                if (stencilPool[i].key == stencil.key && stencilPool[i].Rotation == stencil.Rotation && stencilPool[i].Tiling == stencil.Tiling)
                {
                    temp = i;
                    break;
                }
            }
            return temp;
        }

        private void addListHeader(ListView lView, string headerText)
        {
            ListViewItem item = new ListViewItem();
            item.Text = "";
            item.SubItems.Add(" " + headerText);
            item.BackColor = Color.LightGray;
            item.Tag = "readonly";
            item.Font = new Font(item.Font.FontFamily, item.Font.Size + 1, FontStyle.Bold);
            lView.Items.Add(item);
        }

        private void addListItem(ListView lView, string itemText, string subItemText, string itemType)
        {
            ListViewItem item = new ListViewItem();
            item.Text = itemText;
            item.SubItems.Add(subItemText);
            item.Tag = itemType;
            //item.Group = listView3.Groups["groupStencil" + stencilBoxNo];
            lView.Items.Add(item);
        }

        private void addListBlank(ListView lView)
        {
            ListViewItem item = new ListViewItem();
            item.Tag = "readonly";
            item.Text = "";
            item.SubItems.Add("");
            lView.Items.Add(item);
        }

        private void updateStencilBoxes(int stencilBoxNo, stencilDetails stencil)
        {

            addListHeader(listView3, "Stencil " + stencilBoxNo);
            addListItem(listView3, stencil.key, "Texture", "texture");
            addListItem(listView3, stencil.Rotation, "Rotation", "");
            addListItem(listView3, stencil.Tiling, "Tiling", "");
            addListBlank(listView3);

            stencil.Enabled = "True";
            stencilPool[stencilBoxNo - 1] = stencil;
        }

        private void addCasPartItem(string name, string value)
        {
            ListViewItem item = new ListViewItem();
            item.Text = value;
            item.SubItems.Add(name);
            lstCasPartDetails.Items.Add(item);

            if (debugModeToolStripMenuItem.Checked)
            {
                Helpers.logMessageToFile(name + ": " + value);
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cmbSimAge.Enabled = true;
            cmbSimGender.Enabled = true;
            cmbPartTypes.Enabled = true;

            btnListOtherFind.Enabled = false;
            btnLstOtherReplace.Enabled = false;

            lstCasPartDetails.Items.Clear();
            listView1.Items.Clear();
            lstOtherDetails.Items.Clear();

            saveAsToolStripMenuItem.Enabled = false;
            saveToolStripMenuItem.Enabled = false;
            this.isNew = true;
            this.fromPackage = false;
            Helpers.currentPackageFile = "";

            Helpers.localFiles.Clear();
            //this.newDDSFiles.Clear();

            this.stencilPool.Clear();
            this.stencilPool.TrimExcess();

            //cmbStencilList.Enabled = true;
        }

        private void btnAddNewDesign_Click(object sender, EventArgs e)
        {
            if (listView1.Items.Count > 0)
            {
                addNewCopyLastToolStripMenuItem.Enabled = true;
            }
            else
            {
                addNewCopyLastToolStripMenuItem.Enabled = false;
            }
            contextMenuStrip1.Show(PointToScreen(new Point(btnAddNewDesign.Left + btnAddNewDesign.Width + 3, btnAddNewDesign.Top + contextMenuStrip1.Height )));

        }

        private int lastSelected = -1;
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                if (listView1.SelectedIndices[0] != lastSelected)
                {
                    lastSelected = listView1.SelectedIndices[0];
                    showCasPart(lastSelected);
                }
            }
                       
        }

        public void showCasPart(int chunkNo)
        {
            // Just takes the first XML chunk and uses that
            xmlChunkDetails chunk = (xmlChunkDetails)casPartNew.xmlChunk[chunkNo];

            Console.WriteLine("Showing design " + chunkNo.ToString());
            /*
            chkStencilAEnabled.Enabled = true;
            chkStencilBEnabled.Enabled = true;
            chkStencilCEnabled.Enabled = true;
            chkStencilDEnabled.Enabled = true;
            chkStencilEEnabled.Enabled = true;
            chkStencilFEnabled.Enabled = true;
             */

            chkPatternAEnabled.Enabled = true;
            cmbPatternSelect.SelectedIndex = 0;
            showPatternDetails(chunkNo);

            //chkPatternBEnabled.Enabled = true;
            //chkPatternCEnabled.Enabled = true;
            chkLogoEnabled.Enabled = true;

            btnPatternStencil1Preview.Enabled = false;
            btnPatternStencil2Preview.Enabled = false;
            btnPatternStencil3Preview.Enabled = false;
            btnPatternStencil4Preview.Enabled = false;
            btnPatternStencil5Preview.Enabled = false;
            btnPatternStencil6Preview.Enabled = false;
            cmbPatternStencil1.Enabled = true;
            cmbPatternStencil2.Enabled = true;
            cmbPatternStencil3.Enabled = true;
            cmbPatternStencil4.Enabled = true;
            cmbPatternStencil5.Enabled = true;
            cmbPatternStencil6.Enabled = true;

            if (chunk.hasCustomThumbnail)
            {
                chkUseCustomThumbnail.Checked = true;
                txtCustomThumbnailPath.Text = newPNGfiles[chunkNo];
            }

            //ListViewItem item;
            // Stencil Details

            cmbPatternStencil1.SelectedIndex = 0;
            if (chunk.stencil.A.key != "" && chunk.stencil.A.Enabled.ToLower() == "true" )
            {
                if (inStencilList(chunk.stencil.A)) cmbPatternStencil1.SelectedIndex = getStencilFromPool(chunk.stencil.A) + 1;
            }
            cmbPatternStencil2.SelectedIndex = 0;
            if (chunk.stencil.B.key != "" && chunk.stencil.B.Enabled.ToLower() == "true")
            {
                if (inStencilList(chunk.stencil.B)) cmbPatternStencil2.SelectedIndex = getStencilFromPool(chunk.stencil.B) + 1;
            }
            cmbPatternStencil3.SelectedIndex = 0;
            if (chunk.stencil.C.key != "" && chunk.stencil.C.Enabled.ToLower() == "true")
            {
                if (inStencilList(chunk.stencil.C)) cmbPatternStencil3.SelectedIndex = getStencilFromPool(chunk.stencil.C) + 1;
            }
            cmbPatternStencil4.SelectedIndex = 0;
            if (chunk.stencil.D.key != "" && chunk.stencil.D.Enabled.ToLower() == "true")
            {
                if (inStencilList(chunk.stencil.D)) cmbPatternStencil4.SelectedIndex = getStencilFromPool(chunk.stencil.D) + 1;
            }
            cmbPatternStencil5.SelectedIndex = 0;
            if (chunk.stencil.E.key != "" && chunk.stencil.E.Enabled.ToLower() == "true")
            {
                if (inStencilList(chunk.stencil.E)) cmbPatternStencil5.SelectedIndex = getStencilFromPool(chunk.stencil.E) + 1;
            }
            cmbPatternStencil6.SelectedIndex = 0;
            if (chunk.stencil.F.key != "" && chunk.stencil.F.Enabled.ToLower() == "true")
            {
                if (inStencilList(chunk.stencil.F)) cmbPatternStencil6.SelectedIndex = getStencilFromPool(chunk.stencil.F) + 1;
            }

            // Pattern Details

            // Other Details
            lstOtherDetails.Items.Clear();
            lstTextureDetails.Items.Clear();

            addListHeader(lstTextureDetails, "Texture Details");
            addListItem(lstTextureDetails, chunk.Multiplier, "Base Texture", "texture");
            addListBlank(lstTextureDetails);

            addListHeader(lstTextureDetails, "Clothing Details");
            addListItem(lstTextureDetails, chunk.ClothingSpecular, "Clothing Specular", "texture");
            addListItem(lstTextureDetails, chunk.ClothingAmbient, "Clothing Ambient", "texture");
            addListBlank(lstTextureDetails);

            if (checkedListClothingType.GetItemChecked(17) || checkedListType.GetItemChecked(2))
            {
                addListHeader(lstTextureDetails, "Face Overlay Details");
            }

            if (checkedListClothingType.GetItemChecked(17))
            {
                addListItem(lstTextureDetails, chunk.TintColor, "Tint Color", "color");
            }

            if (checkedListType.GetItemChecked(2))
            {
                addListItem(lstTextureDetails, chunk.tint.A.color, "Tint Color A", "color");
                addListItem(lstTextureDetails, chunk.tint.B.color, "Tint Color B", "color");
                addListItem(lstTextureDetails, chunk.tint.C.color, "Tint Color C", "color");
                addListItem(lstTextureDetails, chunk.tint.D.color, "Tint Color D", "color");
                addListItem(lstTextureDetails, chunk.tint.A.enabled, "Tint Color A Enabled", "color");
                addListItem(lstTextureDetails, chunk.tint.B.enabled, "Tint Color B Enabled", "color");
                addListItem(lstTextureDetails, chunk.tint.C.enabled, "Tint Color C Enabled", "color");
                addListItem(lstTextureDetails, chunk.tint.D.enabled, "Tint Color D Enabled", "color");
                addListBlank(lstTextureDetails);
                addListItem(lstTextureDetails, chunk.faceOverlay, "Face Overlay", "texture");
                addListItem(lstTextureDetails, chunk.faceSpecular, "Face Specular", "texture");
                addListBlank(lstTextureDetails);
            }

            addListHeader(lstTextureDetails, "Mask & Overlay Details");
            addListItem(lstTextureDetails, chunk.Overlay, "Overlay", "texture");
            addListItem(lstTextureDetails, chunk.ControlMap, "Control Map", "texture");
            addListItem(lstTextureDetails, chunk.DiffuseMap, "Diffuse Map", "texture");
            addListItem(lstTextureDetails, chunk.Mask, "Mask", "texture");
            addListItem(lstTextureDetails, chunk.PartMask, "Part Mask", "texture");
            addListBlank(lstTextureDetails);

            addListHeader(lstOtherDetails, "Mask & Overlay Details");
            addListItem(lstOtherDetails, chunk.MaskHeight, "MaskHeight", "");
            addListItem(lstOtherDetails, chunk.MaskWidth, "MaskWidth", "");
            addListBlank(lstOtherDetails);

            // Hair details
            if (checkedListType.GetItemChecked(0))
            {

                addListHeader(lstTextureDetails, "Hair Details");
                addListItem(lstTextureDetails, chunk.hair.RootColor, "Root Color", "color");
                addListItem(lstTextureDetails, chunk.hair.DiffuseColor, "Diffuse Color", "color");
                addListItem(lstTextureDetails, chunk.hair.HighlightColor, "Highlight Color", "color");
                addListItem(lstTextureDetails, chunk.hair.TipColor, "Tip Color", "color");
                addListBlank(lstTextureDetails);
                addListItem(lstTextureDetails, chunk.hair.ScalpSpecularMap, "Scalp Specular Map", "texture");
                addListItem(lstTextureDetails, chunk.hair.ScalpDiffuseMap, "Scalp Diffuse Map", "texture");
                addListItem(lstTextureDetails, chunk.hair.ScalpSpecularMap, "Scalp Control Map", "texture");
                addListItem(lstTextureDetails, chunk.hair.FaceSpecularMap, "Face Specular Map", "texture");
                addListItem(lstTextureDetails, chunk.hair.FaceDiffuseMap, "Face Diffuse Map", "texture");
                addListItem(lstTextureDetails, chunk.hair.FaceSpecularMap, "Face Control Map", "texture");
                addListBlank(lstTextureDetails);

                addListHeader(lstOtherDetails, "Hair Details");
                addListItem(lstOtherDetails, chunk.IsHat, "IsHat", "");
                addListItem(lstOtherDetails, chunk.DrawsOnFace, "DrawsOnFace", "");
                addListItem(lstOtherDetails, chunk.DrawsOnScalp, "DrawsOnScalp", "");
                addListItem(lstOtherDetails, chunk.hair.ScalpAO, "Scalp AO", "texture");
                addListItem(lstOtherDetails, chunk.hair.FaceAO, "Face AO", "texture");
                addListBlank(lstTextureDetails);

            }

            addListHeader(lstTextureDetails, "Skin Details");
            addListItem(lstTextureDetails, chunk.SkinAmbient, "Skin Ambient", "texture");
            addListItem(lstTextureDetails, chunk.SkinSpecular, "Skin Specular", "texture");

            if (chunk.logo.enabled.ToLower() == "true") { chkLogoEnabled.Checked = true; }
            else { chkLogoEnabled.Checked = false; }

            txtLogoUpperLeft.Text = chunk.logo.upperLeft;
            txtLogoLowerRight.Text = chunk.logo.lowerRight;
            txtLogoFilename.Text = chunk.logo.filename;
            txtLogoKey.Text = chunk.logo.key;
            txtLogoName.Text = chunk.logo.name;

            addListHeader(lstOtherDetails, "CAS Details");
            addListItem(lstOtherDetails, chunk.IsNaked, "IsNaked", "");
            addListItem(lstOtherDetails, chunk.IsNotNaked, "IsNotNaked", "");
            addListItem(lstOtherDetails, chunk.partType, "partType", "");
            addListItem(lstOtherDetails, chunk.age, "age", "");
            addListItem(lstOtherDetails, chunk.gender, "gender", "");
            addListItem(lstOtherDetails, chunk.bodyType, "bodyType", "");
            addListBlank(lstOtherDetails);

            addListHeader(lstOtherDetails, "Misc. Details");
            addListItem(lstOtherDetails, chunk.name, "Name", "");
            addListItem(lstOtherDetails, chunk.reskey, "reskey", "");
            addListItem(lstOtherDetails, chunk.daeFileName, "daeFileName", "");
            addListItem(lstOtherDetails, chunk.filename, "filename", "");
            addListItem(lstOtherDetails, chunk.assetRoot, "assetRoot", "");
            addListItem(lstOtherDetails, chunk.Rotation, "Rotation", "");

            toolStripStatusLabel1.Text = "Initialising 3d view... please wait...";
            statusStrip1.Refresh();

            if (renderWindow1.RenderEnabled)
                btnReloadTextures_Click(null, null);
            else
                btnStart3D_Click(null, null);

            toolStripStatusLabel1.Text = "";

        }

        private void btnPreviewTexture_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {

                ListViewItem item = listView1.SelectedItems[0];

                DDSPreview ddsP = new DDSPreview();
                ddsP.loadDDS(item.SubItems[2].Text);
                ddsP.ShowDialog();
            }
        }

        private void btnReplaceTexture_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {

                ListViewItem item = listView1.SelectedItems[0];

                openFileDialog1.FileName = "";
                openFileDialog1.Filter = "Sims 3 Texture File|*.dds";
                if (openFileDialog1.ShowDialog() == DialogResult.OK && openFileDialog1.FileName.Trim() != "")
                {
                    FileInfo f = new FileInfo(openFileDialog1.FileName);
                    item.SubItems[1].Text = f.Name;
                    item.SubItems[2].Text = f.FullName;
                    item.SubItems[3].Text = f.Length.ToString();

                }
            }
        }

        private void btnDumpFromFullbuild2_Click(object sender, EventArgs e)
        {
            // Go through the list of DDS files and dump them
            string s3root = MadScience.Helpers.findSims3Root();

            if (s3root != "")
            {

                bool hasShownDialog = false;

                toolStripProgressBar1.Minimum = 0;
                toolStripProgressBar1.Value = 0;
                toolStripStatusLabel1.Text = "Searching for textures... please wait";
                statusStrip1.Refresh();

                Stream fbuild2 = File.Open(s3root + "\\GameData\\Shared\\Packages\\FullBuild2.package", FileMode.Open, FileAccess.Read, FileShare.Read);
                MadScience.Wrappers.Database db = new MadScience.Wrappers.Database(fbuild2, true);

                toolStripProgressBar1.Maximum = casPartSrc.tgi64list.Count;

                Dictionary<ulong, string> keyNames = new Dictionary<ulong, string>();
                long nowTicks = DateTime.Now.Ticks;
                Console.WriteLine("Started at: " + nowTicks);
                foreach (MadScience.Wrappers.ResourceKey entry in db._Entries.Keys)
                {
                    //DatabasePackedFile.Entry entry = db.dbpfEntries[i];
                    if (entry.typeId == (int)0x0166038C)
                    {
                        keyNames = Helpers.getKeyNames(db.GetResourceStream(entry));
                        break;
                    }
                }

                int numFound = 0;
                folderBrowserDialog1.SelectedPath = "";

                for (int j = 0; j < casPartSrc.tgi64list.Count; j++)
                {
                    toolStripProgressBar1.Value++;
                    //keyName tgi = new keyName((tgi64)casPartSrc.tgi64list[j]);
                    MadScience.Wrappers.ResourceKey tgi = casPartSrc.tgi64list[j];
                    if (tgi.typeId == (int)0x00B2D882)
                    {
                        //Stream textureStream = KeyUtils.searchForKey(tgi.ToString(), 2);
                        Stream textureStream = KeyUtils.findKey(tgi);
                        if (textureStream != null)
                        {
                            string fileNameToSave = "";
                            if (keyNames.ContainsKey(tgi.instanceId))
                            {
                                fileNameToSave = keyNames[tgi.instanceId];
                            }
                            else
                            {
                                fileNameToSave = tgi.typeId.ToString("X8") + "_" + tgi.groupId.ToString("X8") + "_" + tgi.instanceId.ToString("X16");
                            }

                            if (!hasShownDialog)
                            {
                                folderBrowserDialog1.Description = "Please select a folder to save the extracted textures to.";
                                folderBrowserDialog1.ShowDialog();
                                hasShownDialog = true;
                            }
                            if (folderBrowserDialog1.SelectedPath != "")
                            {

                                Stream output = db.GetResourceStream(tgi);
                                FileStream saveFile = new FileStream(folderBrowserDialog1.SelectedPath + "\\" + fileNameToSave + ".dds", FileMode.Create, FileAccess.Write);
                                Helpers.CopyStream(output, saveFile);
                                saveFile.Close();
                                output.Close();
                                numFound++;
                            }

                        }
                    }
                }


                /*
                foreach (ResourceKey entry in db.Entries.Keys)
                {

                    toolStripProgressBar1.Value++;

                    //MadScience.Wrappers.DatabasePackedFile.Entry entry = db.dbpfEntries[i];
                    if (entry.TypeId == 0x00B2D882)
                    {
                        for (int j = 0; j < casPartSrc.tgi64list.Count; j++)
                        {
                            tgi64 tgi = (tgi64)casPartSrc.tgi64list[j];
                            if (tgi.typeid == (int)0x00B2D882)
                            {
                                if (entry.TypeId == tgi.typeid && entry.GroupId == tgi.groupid && entry.InstanceId == tgi.instanceid)
                                {

                                    string fileNameToSave = "";
                                    if (keyNames.ContainsKey(entry.InstanceId))
                                    {
                                        fileNameToSave = keyNames[entry.InstanceId];
                                    }
                                    else
                                    {
                                        fileNameToSave = entry.TypeId.ToString("X8") + "_" + entry.GroupId.ToString("X8") + "_" + entry.InstanceId.ToString("X16");
                                    }

                                    if (!hasShownDialog)
                                    {
                                        folderBrowserDialog1.Description = "Please select a folder to save the extracted textures to.";
                                        folderBrowserDialog1.ShowDialog();
                                        hasShownDialog = true;
                                    }
                                    if (folderBrowserDialog1.SelectedPath != "")
                                    {

                                        Stream output = db.GetResourceStream(entry);
                                        FileStream saveFile = new FileStream(folderBrowserDialog1.SelectedPath + "\\" + fileNameToSave + ".dds", FileMode.Create, FileAccess.Write);
                                        Helpers.CopyStream(output, saveFile);
                                        saveFile.Close();
                                        output.Close();
                                        numFound++;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    //Console.WriteLine("Time taken: " + (DateTime.Now.Ticks - nowTicks));

                }
                */

                toolStripProgressBar1.Value = 0;
                toolStripStatusLabel1.Text = numFound + " textures found.";
                statusStrip1.Refresh();

                fbuild2.Close();

            }

        }

        MadScience.ListViewSorter Sorter = new MadScience.ListViewSorter();

        private void lstOtherDetails_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            Sorter.Sort(lstOtherDetails, e);
        }

        private void lstOtherDetails_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstOtherDetails.SelectedItems.Count == 1)
            {
                ListViewItem item = lstOtherDetails.SelectedItems[0];
                switch (item.SubItems[1].Text)
                {
                    case "Skin Specular":
                    case "Skin Ambient":
                    case "Clothing Specular":
                    case "Clothing Ambient":
                    case "Overlay":
                    case "Face Overlay":
                    case "Face Specular":
                    case "Control Map":
                    case "Diffuse Map":
                    case "Scalp Diffuse Map":
                    case "Scalp Control Map":
                    case "Scalp Specular Map":
                    case "Scalp AO":
                    case "Face Diffuse Map":
                    case "Face Control Map":
                    case "Face Specular Map":
                    case "Face AO":
                    case "Mask":
                    case "Part Mask":
                    case "Base Texture":
                    case "Multiplier":
                        btnListOtherFind.Enabled = true;
                        btnLstOtherReplace.Enabled = true;
                        break;
                    case "Tint Color":
                    case "Tint Color A":
                    case "Tint Color B":
                    case "Tint Color C":
                    case "Tint Color D":
                    case "Root Color":
                    case "Diffuse Color":
                    case "Highlight Color":
                    case "Tip Color":
                        picLstOtherColour.Enabled = true;
                        picLstOtherColour.BackColor = Helpers.convertColour(item.SubItems[0].Text);
                        break;
                    default:
                        btnListOtherFind.Enabled = false;
                        btnLstOtherReplace.Enabled = false;
                        picLstOtherColour.Enabled = false;
                        break;
                }
            }
        }

        private void chkPatternAEnabled_CheckedChanged(object sender, EventArgs e)
        {
        }


        private void chkLogoEnabled_CheckedChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {

                xmlChunkDetails chunk = (xmlChunkDetails)casPartNew.xmlChunk[listView1.SelectedIndices[0]];
                if (chkLogoEnabled.Enabled == true)
                {
                    grpLogo.Enabled = true;
                    chunk.logo.enabled = "True";
                }
                else
                {
                    grpLogo.Enabled = false;
                    chunk.logo.enabled = "False";
                }
            }
        }

        private void btnDeleteDesign_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                casPartNew.xmlChunk.RemoveAt(listView1.SelectedIndices[0]);
                casPartNew.xmlChunkRaw.RemoveAt(listView1.SelectedIndices[0]);
                listView1.Items.RemoveAt(listView1.SelectedIndices[0]);

                if (listView1.Items.Count == 0)
                {
                    saveAsToolStripMenuItem.Enabled = false;
                }
            }
        }

        private void btnPatternAReplaceBGImage_Click(object sender, EventArgs e)
        {
            txtPatternBGImage.Text = replaceImageKey(txtPatternBGImage.Text);
        }

        private void btnPatternAReplaceRGBMask_Click(object sender, EventArgs e)
        {
            txtPatternARGBMask.Text = replaceImageKey(txtPatternARGBMask.Text);
        }

        private void btnPatternACommit_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {

                int chunkNo = cmbPatternSelect.SelectedIndex;

                xmlChunkDetails chunk = (xmlChunkDetails)casPartNew.xmlChunk[listView1.SelectedIndices[0]];
                chunk.pattern[chunkNo] = commitPatternDetails();
            }
        }

        private patternDetails commitPatternDetails()
        {

            patternDetails pDetails = new patternDetails();

            if (chkPatternAEnabled.Checked == true)
            {
                //grpPatternA.Enabled = true;
                pDetails.Enabled = "True";
            }
            else
            {
                //grpPatternA.Enabled = false;
                pDetails.Enabled = "False";
            }

            if (txtPatternATiling.Text.Trim() != "") { pDetails.Tiling = txtPatternATiling.Text; }
            if (chkPatternALinked.Checked == true) { pDetails.Linked = "true"; } else { pDetails.Linked = "false"; }
            if (txtPatternAName.Text.Trim() != "") { pDetails.name = txtPatternAName.Text; }
            if (txtPatternAFilename.Text.Trim() != "") { pDetails.filename = txtPatternAFilename.Text; }
            if (txtPatternAKey.Text.Trim() != "") { pDetails.key = txtPatternAKey.Text; }
            pDetails.rgbmask = txtPatternARGBMask.Text;
            pDetails.specmap = txtPatternASpecular.Text;

            switch (cmbPatternAType.SelectedIndex)
            {
                case 0:
                    pDetails.type = "solidColor";
                    break;
                case 2:
                    pDetails.type = "HSV";
                    break;
                case 1:
                    pDetails.type = "Coloured";
                    break;
            }

            if (pDetails.type == "solidColor")
            {
                pDetails.nameHigh = pDetails.filename;
                pDetails.Color = MadScience.Helpers.convertColour(picPatternSolidColour.BackColor);
            }
            if (pDetails.type == "HSV")
            {
                pDetails.nameHigh = pDetails.filename;
                if (txtPatternBGImage.Text.Trim() != "") { pDetails.BackgroundImage = txtPatternBGImage.Text; }
                if (txtPatternAHBg.Text.Trim() != "") { pDetails.HBg = txtPatternAHBg.Text; }
                if (txtPatternASBg.Text.Trim() != "") { pDetails.SBg = txtPatternASBg.Text; }
                if (txtPatternAVBg.Text.Trim() != "") { pDetails.VBg = txtPatternAVBg.Text; }
                if (txtPatternABaseHBg.Text.Trim() != "") { pDetails.BaseHBg = txtPatternABaseHBg.Text; }
                if (txtPatternABaseSBg.Text.Trim() != "") { pDetails.BaseSBg = txtPatternABaseSBg.Text; }
                if (txtPatternABaseVBg.Text.Trim() != "") { pDetails.BaseVBg = txtPatternABaseVBg.Text; }
                if (txtPatternAHSVShiftBG.Text.Trim() != "") { pDetails.HSVShiftBg = txtPatternAHSVShiftBG.Text; }
            }
            if (pDetails.type == "Coloured")
            {
                pDetails.nameHigh = @"Materials\Miscellaneous\solidColor_1";
                if (picPatternColourBg.Visible) { pDetails.ColorP[0] = MadScience.Helpers.convertColour(picPatternColourBg.BackColor); }
                else { pDetails.ColorP[0] = null; }
                if (picPatternColour1.Visible) { pDetails.ColorP[1] = MadScience.Helpers.convertColour(picPatternColour1.BackColor); }
                else { pDetails.ColorP[1] = null; }
                if (picPatternColour2.Visible) { pDetails.ColorP[2] = MadScience.Helpers.convertColour(picPatternColour2.BackColor); }
                else { pDetails.ColorP[2] = null; }
                if (picPatternColour3.Visible) { pDetails.ColorP[3] = MadScience.Helpers.convertColour(picPatternColour3.BackColor); }
                else { pDetails.ColorP[3] = null; }
                if (picPatternColour4.Visible) { pDetails.ColorP[4] = MadScience.Helpers.convertColour(picPatternColour4.BackColor); }
                else { pDetails.ColorP[4] = null; }
            }

            return pDetails;
        }

        private void txtLogoCommit_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                xmlChunkDetails chunk = (xmlChunkDetails)casPartNew.xmlChunk[listView1.SelectedIndices[0]];
                //if (txtLogoTiling.Text.Trim() != "") { chunk.pattern.Logo.Tiling = txtLogoTiling.Text; }
                if (txtLogoName.Text.Trim() != "") { chunk.logo.name = txtLogoName.Text; }
                if (txtLogoFilename.Text.Trim() != "") { chunk.logo.filename = txtLogoFilename.Text; }
                if (txtLogoKey.Text.Trim() != "") { chunk.logo.key = txtLogoKey.Text; }
                if (txtLogoUpperLeft.Text.Trim() != "") { chunk.logo.upperLeft = txtLogoUpperLeft.Text; }
                if (txtLogoLowerRight.Text.Trim() != "") { chunk.logo.lowerRight = txtLogoLowerRight.Text; }
                //chunk.logo.Color = MadScience.Helpers.convertColour(picLogoColour.BackColor);
            }
        }

        private void saveGeom(Stream input, Stream output, List<keyName> keynames)
        {
            BinaryReader reader = new BinaryReader(input);
            BinaryWriter writer = new BinaryWriter(output);

            uint rcolVersion = reader.ReadUInt32();
            writer.Write(rcolVersion);
            uint rcolDatatype = reader.ReadUInt32();
            writer.Write(rcolDatatype);

            uint rcolIndex3 = reader.ReadUInt32();
            writer.Write(rcolIndex3);
            uint rcolIndex1 = reader.ReadUInt32();
            writer.Write(rcolIndex1);
            uint rcolIndex2 = reader.ReadUInt32();
            writer.Write(rcolIndex2);

            for (int i = 0; i < rcolIndex2; i++)
            {
                ulong instanceId = reader.ReadUInt64();
                writer.Write(instanceId);
                uint typeId = reader.ReadUInt32();
                writer.Write(typeId);
                uint groupId = reader.ReadUInt32();
                writer.Write(groupId);
            }

            for (int i = 0; i < rcolIndex2; i++)
            {
                uint chunkOffset = reader.ReadUInt32();
                writer.Write(chunkOffset);
                uint chunkSize = reader.ReadUInt32();
                writer.Write(chunkSize);
            }

            // Real GEOM chunk
            string geomString = MadScience.StreamHelpers.ReadStringASCII(input, 4);
            MadScience.StreamHelpers.WriteStringASCII(output, geomString);

            uint geomVersion = reader.ReadUInt32();
            writer.Write(geomVersion);

            uint tailOffset = reader.ReadUInt32();
            writer.Write(tailOffset);

            long seekFrom = input.Position;
            //uint tailSize = reader.ReadUInt32();
            //writer.Write(tailSize);

            //int bytesLeft = (int)((tailOffset + startGeom) - input.Position);
            int bytesLeft = (int)(input.Length - input.Position);
            writer.Write(reader.ReadBytes(bytesLeft));

            output.Seek(tailOffset + seekFrom, SeekOrigin.Begin);
            input.Seek(tailOffset + seekFrom, SeekOrigin.Begin);

            // Tail section
            /*
            uint tailStart = reader.ReadUInt32();
            if (tailStart != 4)
            {
                // Looks like we seeked to the wrong place... try going back a bit
                output.Seek(tailOffset + 40, SeekOrigin.Begin);
                input.Seek(tailOffset + 40, SeekOrigin.Begin);
                tailStart = reader.ReadUInt32();

            }
            writer.Write(tailStart);
            uint countTail = reader.ReadUInt32();
            writer.Write(countTail);
            for (int i = 0; i < countTail; i++)
            {
                writer.Write(reader.ReadUInt32());
            }
            */
            // TGI list
            uint numOrig = reader.ReadUInt32();
            writer.Write(numOrig);

            // How many does the original mesh have? - Some meshes (Accessories) have only 3 TGI links.  Some have 4, some have 5.
            // If 3 then chop off first two
            // If 4 then chop off first one
            // If 5 then save all

            int startAt = 0;
            if (numOrig == 3) {
                startAt = 2;
            }
            if (numOrig == 4) {
                startAt = 1;
            }

            for (int i = startAt; i < keynames.Count; i++)
            {
                if (keynames[i].ToString() != keynames[i].Blank)
                {
                    writer.Write(keynames[i].typeId);
                    writer.Write(keynames[i].groupId);
                    writer.Write(keynames[i].instanceId);
                }
                else
                {
                    // Skip this key
                    writer.Write(reader.ReadUInt32());
                    writer.Write(reader.ReadUInt32());
                    writer.Write(reader.ReadUInt64());
                }
            }

            Console.WriteLine("Done saving mesh...");
        }

        private MemoryStream makeBlendFile(keyName proxy)
        {
            MemoryStream output = new MemoryStream();
            MadScience.StreamHelpers.WriteValueU32(output, 7);
            MadScience.StreamHelpers.WriteValueU32(output, 0);
            MadScience.StreamHelpers.WriteValueU32(output, 20);
            MadScience.StreamHelpers.WriteValueU8(output, (byte)(proxy.name.Length * 2));
            MadScience.StreamHelpers.WriteStringUTF16(output, false, proxy.name);
            MadScience.StreamHelpers.WriteValueU32(output, 2);
            MadScience.StreamHelpers.WriteValueU32(output, 1);
            MadScience.StreamHelpers.WriteValueU32(output, 1024);
            MadScience.StreamHelpers.WriteValueU32(output, 1);
            MadScience.StreamHelpers.WriteValueU32(output, 77951);
            MadScience.StreamHelpers.WriteValueU16(output, 0);
            MadScience.StreamHelpers.WriteValueU32(output, 16256);
            MadScience.StreamHelpers.WriteValueU32(output, 0);
            MadScience.StreamHelpers.WriteValueU16(output, 0);
            uint position = (uint)output.Position;
            MadScience.StreamHelpers.WriteValueU32(output, 1);
            //MadScience.StreamHelpers.WriteValueU64(output, 6231196913);
            MadScience.StreamHelpers.WriteValueU32(output, proxy.typeId);
            MadScience.StreamHelpers.WriteValueU32(output, proxy.groupId);
            MadScience.StreamHelpers.WriteValueU64(output, proxy.instanceId);

            output.Seek(4, SeekOrigin.Begin);
            MadScience.StreamHelpers.WriteValueU32(output, (position - 8));

            // Should be 101 bytes long at this point
            return output;
        }

        private MemoryStream makeVPXYfile(MadScience.Wrappers.ResourceKey headerKey)
        {
            MemoryStream mem = new MemoryStream();
            MadScience.Wrappers.VPXYFile vpxyFile = new MadScience.Wrappers.VPXYFile();
            vpxyFile.rcolHeader.internalChunks.Add(headerKey);

            vpxyFile.Save(mem);

            return mem;
        }

        private void saveToDBPF(Database db, ulong instanceId, bool newInstance)
        {
            ResourceKey rkey;

            MemoryStream mem = new MemoryStream();
            casPartFile casPF = new casPartFile();

            // Do we have new meshes?  If so, we need to do some pretty heft modifications. :)

            string meshName = txtMeshName.Text;

            if (!String.IsNullOrEmpty(txtMeshLod1.Text))
            {
                uint customGroup = MadScience.StringHelpers.HashFNV24(meshName);
                keyName meshLod1 = new keyName(0x015A1849, customGroup, (ulong)MadScience.StringHelpers.HashFNV32(meshName + "_lod1"), meshName + "_lod1");
                keyName meshLod2 = new keyName(0x015A1849, customGroup, (ulong)MadScience.StringHelpers.HashFNV32(meshName + "_lod2"), meshName + "_lod2");
                keyName meshLod3 = new keyName(0x015A1849, customGroup, (ulong)MadScience.StringHelpers.HashFNV32(meshName + "_lod3"), meshName + "_lod3");

                keyName vpxyKey = new keyName(0x736884F1, 0x00000001, (ulong)customGroup);

                // Load in the VPXY - we need to modify it.
                //keyName oldVpxyKey = new keyName((tgi64)casPartSrc.tgi64list[casPartSrc.tgiIndexVPXY]);
                Stream vpxyStream = KeyUtils.findKey(casPartSrc.tgi64list[casPartSrc.tgiIndexVPXY].ToString(), 0);
                if (vpxyStream != null)
                {

                    keyName proxyFit = new keyName(0x736884F1, 0x00000001, meshName + "_fit");
                    keyName proxyFat = new keyName(0x736884F1, 0x00000001, meshName + "_fat");
                    keyName proxyThin = new keyName(0x736884F1, 0x00000001, meshName + "_thin");
                    keyName proxySpecial = new keyName(0x736884F1, 0x00000001, meshName + "_special");
                    keyName bodyBlendFat = new keyName(0x062C8204, 0x0, meshName + "_fat");
                    keyName bodyBlendFit = new keyName(0x062C8204, 0x0, meshName + "_fit");
                    keyName bodyBlendThin = new keyName(0x062C8204, 0x0, meshName + "_thin");
                    keyName bodyBlendSpecial = new keyName(0x062C8204, 0x0, meshName + "_special");

                    vpxyStream.Seek(0x14, SeekOrigin.Begin);
                    MadScience.StreamHelpers.WriteValueU64(vpxyStream, vpxyKey.instanceId);
                    MadScience.StreamHelpers.WriteValueU32(vpxyStream, vpxyKey.typeId);
                    MadScience.StreamHelpers.WriteValueU32(vpxyStream, vpxyKey.groupId);

                    vpxyStream.Seek(vpxyStream.Length - 48, SeekOrigin.Begin);
                    MadScience.StreamHelpers.WriteValueU32(vpxyStream, meshLod1.typeId);
                    MadScience.StreamHelpers.WriteValueU32(vpxyStream, meshLod1.groupId);
                    MadScience.StreamHelpers.WriteValueU64(vpxyStream, meshLod1.instanceId);
                    MadScience.StreamHelpers.WriteValueU32(vpxyStream, meshLod2.typeId);
                    MadScience.StreamHelpers.WriteValueU32(vpxyStream, meshLod2.groupId);
                    MadScience.StreamHelpers.WriteValueU64(vpxyStream, meshLod2.instanceId);
                    MadScience.StreamHelpers.WriteValueU32(vpxyStream, meshLod3.typeId);
                    MadScience.StreamHelpers.WriteValueU32(vpxyStream, meshLod3.groupId);
                    MadScience.StreamHelpers.WriteValueU64(vpxyStream, meshLod3.instanceId);


                    MemoryStream proxyFitFile = makeVPXYfile(proxyFit.ToResourceKey());
                    MemoryStream proxyFatFile = makeVPXYfile(proxyFat.ToResourceKey());
                    MemoryStream proxyThinFile = makeVPXYfile(proxyThin.ToResourceKey());
                    MemoryStream proxySpecialFile = makeVPXYfile(proxySpecial.ToResourceKey());

                    MemoryStream bodyBlendFitFile = makeBlendFile(proxyFit);
                    MemoryStream bodyBlendFatFile = makeBlendFile(proxyFat);
                    MemoryStream bodyBlendThinFile = makeBlendFile(proxyThin);
                    MemoryStream bodyBlendSpecialFile = makeBlendFile(proxySpecial);

                    db.SetResourceStream(vpxyKey.ToResourceKey(), vpxyStream);
                    db.SetResourceStream(proxyFit.ToResourceKey(), proxyFitFile);
                    db.SetResourceStream(proxyFat.ToResourceKey(), proxyFatFile);
                    db.SetResourceStream(proxyThin.ToResourceKey(), proxyThinFile);
                    db.SetResourceStream(proxySpecial.ToResourceKey(), proxySpecialFile);
                    db.SetResourceStream(bodyBlendFit.ToResourceKey(), bodyBlendFitFile);
                    db.SetResourceStream(bodyBlendFat.ToResourceKey(), bodyBlendFatFile);
                    db.SetResourceStream(bodyBlendThin.ToResourceKey(), bodyBlendThinFile);
                    db.SetResourceStream(bodyBlendSpecial.ToResourceKey(), bodyBlendSpecialFile);

                    // Update the CAS part TGI links with the new VPXY
                    casPartNew.tgi64list[casPartNew.tgiIndexVPXY] = vpxyKey.ToResourceKey();
                    casPartNew.tgi64list[casPartNew.tgiIndexBlendInfoFat] = bodyBlendFat.ToResourceKey();
                    casPartNew.tgi64list[casPartNew.tgiIndexBlendInfoFit] = bodyBlendFit.ToResourceKey();
                    casPartNew.tgi64list[casPartNew.tgiIndexBlendInfoThin] = bodyBlendThin.ToResourceKey();
                    casPartNew.tgi64list[casPartNew.tgiIndexBlendInfoSpecial] = bodyBlendSpecial.ToResourceKey();


                    // Modify the meshes if they need a replacement bump map
                    List<keyName> kNames = new List<keyName>();

                    kNames.Add(new keyName());
                    kNames.Add(new keyName());
                    if (String.IsNullOrEmpty(txtOtherBumpMap.Text) == false)
                    {
                        keyName bumpMapKey = new keyName(txtOtherBumpMap.Text, meshName + "_n");
                        kNames.Add(bumpMapKey);
                        if (txtOtherBumpMap.Text != "" && !txtOtherBumpMap.Text.StartsWith("key:")) db.SetResourceStream(bumpMapKey.ToResourceKey(), File.OpenRead(txtOtherBumpMap.Text));                        
                    }
                    else
                    {
                        kNames.Add(new keyName());
                    }
                    kNames.Add(new keyName());
                    kNames.Add(new keyName());

                    Stream blah;

                    MemoryStream geomLod1 = new MemoryStream();
                    if (txtMeshLod1.Text.Trim() != "")
                    {
                        blah = File.Open(txtMeshLod1.Text, FileMode.Open);
                        saveGeom(blah, geomLod1, kNames);
                        blah.Close();
                        db.SetResourceStream(meshLod1.ToResourceKey(), geomLod1);
                    }
                    geomLod1.Close();

                    MemoryStream geomLod2 = new MemoryStream();
                    if (txtMeshLod2.Text.Trim() != "")
                    {
                        blah = File.Open(txtMeshLod2.Text, FileMode.Open);
                        saveGeom(blah, geomLod2, kNames);
                        blah.Close();
                        db.SetResourceStream(meshLod2.ToResourceKey(), geomLod2);
                    }
                    geomLod2.Close();

                    MemoryStream geomLod3 = new MemoryStream();
                    if (txtMeshLod3.Text.Trim() != "")
                    {
                        blah = File.Open(txtMeshLod3.Text, FileMode.Open);
                        saveGeom(blah, geomLod3, kNames);
                        blah.Close();
                        db.SetResourceStream(meshLod3.ToResourceKey(), geomLod3);
                    }
                    geomLod3.Close();

                    blah = null;
                }

            }

            if (casPartNew != null)
            {
                casPF.Save(mem, casPartNew);
            }
            else
            {
                casPF.Save(mem, casPartSrc);
            }
            casPF = null;

            if (this.loadedCasPart.ToString() == "00000000:00000000:0000000000000000")
            {
                rkey = new ResourceKey((uint)0x034AEECB, (uint)0, instanceId, (uint)ResourceKeyOrder.ITG);
            }
            else
            {
                if (!newInstance)
                {
                    rkey = this.loadedCasPart;
                }
                else
                {
                    rkey = new ResourceKey((uint)0x034AEECB, (uint)0, instanceId, (uint)ResourceKeyOrder.ITG);
                }
            }
            db.SetResourceStream(rkey, mem);

            if (casPartNew != null)
            {
                // Go through a list of all the keys and see if they are "local"
                for (int i = 0; i < casPartNew.xmlChunk.Count; i++)
                {
                    xmlChunkDetails chunk = (xmlChunkDetails)casPartNew.xmlChunk[i];

                    for (int j = 0; j < 10; j++)
                    {
                        writeLocalResource(db, stencilPool[j].key);
                    }
                    writeLocalResource(db, chunk.Multiplier);
                    writeLocalResource(db, chunk.Overlay);
                    writeLocalResource(db, chunk.hair.RootColor);
                    writeLocalResource(db, chunk.hair.DiffuseColor);
                    writeLocalResource(db, chunk.hair.HighlightColor);
                    writeLocalResource(db, chunk.hair.TipColor);
                    writeLocalResource(db, chunk.hair.ScalpDiffuseMap);
                    writeLocalResource(db, chunk.hair.ScalpControlMap);
                    writeLocalResource(db, chunk.hair.ScalpSpecularMap);
                    writeLocalResource(db, chunk.hair.ScalpAO);
                    writeLocalResource(db, chunk.hair.FaceDiffuseMap);
                    writeLocalResource(db, chunk.hair.FaceControlMap);
                    writeLocalResource(db, chunk.hair.FaceSpecularMap);
                    writeLocalResource(db, chunk.hair.FaceAO);
                    writeLocalResource(db, chunk.Mask);
                    writeLocalResource(db, chunk.SkinSpecular);
                    writeLocalResource(db, chunk.SkinAmbient);
                    writeLocalResource(db, chunk.ClothingSpecular);
                    writeLocalResource(db, chunk.ClothingAmbient);
                    writeLocalResource(db, chunk.PartMask);
                    writeLocalResource(db, chunk.pattern[0].BackgroundImage);
                    writeLocalResource(db, chunk.pattern[1].BackgroundImage);
                    writeLocalResource(db, chunk.pattern[2].BackgroundImage);
                    writeLocalResource(db, chunk.pattern[3].BackgroundImage);
                    writeLocalResource(db, chunk.pattern[0].rgbmask);
                    writeLocalResource(db, chunk.pattern[1].rgbmask);
                    writeLocalResource(db, chunk.pattern[2].rgbmask);
                    writeLocalResource(db, chunk.pattern[3].rgbmask);
                    writeLocalResource(db, chunk.pattern[0].specmap);
                    writeLocalResource(db, chunk.pattern[1].specmap);
                    writeLocalResource(db, chunk.pattern[2].specmap);
                    writeLocalResource(db, chunk.pattern[3].specmap);
                    writeLocalResource(db, chunk.pattern[0].filename);
                    writeLocalResource(db, chunk.pattern[1].filename);
                    writeLocalResource(db, chunk.pattern[2].filename);
                    writeLocalResource(db, chunk.pattern[3].filename);
                    writeLocalResource(db, chunk.faceOverlay);
                    writeLocalResource(db, chunk.faceSpecular);
                    writeLocalResource(db, chunk.ControlMap);
                    writeLocalResource(db, chunk.DiffuseMap);

                    if (newPNGfiles.ContainsKey(i))
                    {
                        Stream newPNG = File.Open(newPNGfiles[i], FileMode.Open, FileAccess.Read, FileShare.Read);
                        ResourceKey keyPNG = new ResourceKey(0x626F60CE, (uint)(i + 1), instanceId, (uint)ResourceKeyOrder.ITG);
                        db.SetResourceStream(keyPNG, newPNG);
                        newPNG.Close();
                    }
                }
            }

            mem.Close();

        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.FileName = "";
            saveFileDialog1.Filter = "Sims 3 Packages|*.package|CASPart|*.caspart";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {

                FileInfo f = new FileInfo(saveFileDialog1.FileName);

                if (f.Extension == ".package")
                {

                    Stream saveFile = File.Open(saveFileDialog1.FileName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);

                    ulong instanceId = MadScience.StringHelpers.HashFNV64("CTU_" + DateTime.Now.Ticks + "_" + MadScience.Helpers.sanitiseString(f.Name));
                    Database db = new Database(saveFile, false);

                    saveToDBPF(db, instanceId, true);

                    db.Commit(true);

                    saveFile.Close();
                }
                if (f.Extension == ".caspart")
                {
                    Stream saveFile = File.Open(saveFileDialog1.FileName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);

                    MemoryStream mem = new MemoryStream();
                    casPartFile casPF = new casPartFile();
                    casPF.Save(mem, casPartNew);
                    casPF = null;

                    saveFile.Write(mem.GetBuffer(), 0, (int)mem.Length);

                    saveFile.Close();

                }
            }
        }

        private void writeLocalResource(Database db, string keyName)
        {
            if (keyName.Trim() == "") return;
            //if (!validateKey(keyName)) return;

            if (Helpers.localFiles.ContainsKey(keyName))
            {
                ResourceKey key = new ResourceKey(keyName);
                Stream newDDS = File.Open((string)Helpers.localFiles[keyName], FileMode.Open, FileAccess.Read, FileShare.Read);
                db.SetResourceStream(key, newDDS);
                newDDS.Close();
            }

        }

        private void btnListOtherFind_Click(object sender, EventArgs e)
        {
            listFindOrReplace(lstOtherDetails, true);
        }

        private void btnPatternAFindBGImage_Click(object sender, EventArgs e)
        {
            KeyUtils.findAndShowImage(txtPatternBGImage.Text);
        }

        private void btnPatternAFindRGBMask_Click(object sender, EventArgs e)
        {
            KeyUtils.findAndShowImage(txtPatternARGBMask.Text);
        }

        private void btnPatternAFindSpec_Click(object sender, EventArgs e)
        {
            KeyUtils.findAndShowImage(txtPatternASpecular.Text);
        }

        private void btnPatternAReplaceSpec_Click(object sender, EventArgs e)
        {
            txtPatternASpecular.Text = replaceImageKey(txtPatternASpecular.Text);
        }

        private string replaceImageKey(string keyString)
        {
            //if (keyString.Trim() == "") { return ""; }

            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "Sims 3 Texture File|*.dds";
            if (openFileDialog1.ShowDialog() == DialogResult.OK && openFileDialog1.FileName.Trim() != "")
            {
                FileInfo f = new FileInfo(openFileDialog1.FileName);
                // Generate Instance ID from the name plus some other stuff
                string newKey = "";
                string instanceID = "";
                if (keyString.Trim() == "")
                {
                    instanceID = MadScience.StringHelpers.HashFNV64("CTU_" + DateTime.Now.Ticks.ToString() + "_" + f.Name).ToString("X16");
                    newKey = "key:00B2D882:00000000:" + instanceID;
                }
                else
                {
                    keyString = keyString.Replace("key:", "");
                    string[] temp = keyString.Split(":".ToCharArray());
                    instanceID = MadScience.StringHelpers.HashFNV64("CTU_" + DateTime.Now.Ticks.ToString() + "_" + f.Name).ToString("X16");

                    newKey = "key:" + temp[0] + ":" + temp[1] + ":" + instanceID;
                }
                Helpers.localFiles.Add(newKey, f.FullName);
                return newKey;

            }
            return keyString;
        }

        private void btnLstOtherReplace_Click(object sender, EventArgs e)
        {
            listFindOrReplace(lstOtherDetails, false);
        }

        private void picPatternAColor_Click(object sender, EventArgs e)
        {
            picPatternSolidColour.BackColor = showColourDialog(picPatternSolidColour.BackColor);
        }

        private Color showColourDialog(Color input)
        {

            ColorPicker.ColorPickerDialog cpd = new ColorPicker.ColorPickerDialog();
            cpd.Color = input;
            if (cpd.ShowDialog() == DialogResult.OK)
            {
                return cpd.Color;
            }
            else
            {
                return input;
            }

        }

        private void picLogoColour_Click(object sender, EventArgs e)
        {
            picLogoColour.BackColor = showColourDialog(picLogoColour.BackColor);
        }

        private void btnCategoryDetailsCommit_Click(object sender, EventArgs e)
        {
            uint typeFlag = 0;
            uint ageGenderFlag = 0;
            uint clothingCategory = 0;
            uint clothingType = 0;

            if (lstCasPartDetails.Items.Count > 0)
            {

                if (checkedListType.GetItemChecked(0)) typeFlag += 0x1;
                if (checkedListType.GetItemChecked(1)) typeFlag += 0x2;
                if (checkedListType.GetItemChecked(2)) typeFlag += 0x4;
                if (checkedListType.GetItemChecked(3)) typeFlag += 0x8;
                if (checkedListType.GetItemChecked(4)) typeFlag += 0x10;

                if (checkedListClothingType.GetItemChecked(0)) clothingType += 1;
                if (checkedListClothingType.GetItemChecked(1)) clothingType += 2;
                if (checkedListClothingType.GetItemChecked(2)) clothingType += 3;
                if (checkedListClothingType.GetItemChecked(3)) clothingType += 4;
                if (checkedListClothingType.GetItemChecked(4)) clothingType += 5;
                if (checkedListClothingType.GetItemChecked(5)) clothingType += 6;
                if (checkedListClothingType.GetItemChecked(6)) clothingType += 7;
                if (checkedListClothingType.GetItemChecked(7)) clothingType += 11;
                if (checkedListClothingType.GetItemChecked(8)) clothingType += 12;
                if (checkedListClothingType.GetItemChecked(9)) clothingType += 13;
                if (checkedListClothingType.GetItemChecked(10)) clothingType += 14;
                if (checkedListClothingType.GetItemChecked(11)) clothingType += 15;
                if (checkedListClothingType.GetItemChecked(12)) clothingType += 16;
                if (checkedListClothingType.GetItemChecked(13)) clothingType += 17;
                if (checkedListClothingType.GetItemChecked(14)) clothingType += 18;
                if (checkedListClothingType.GetItemChecked(15)) clothingType += 19;
                if (checkedListClothingType.GetItemChecked(16)) clothingType += 20;
                if (checkedListClothingType.GetItemChecked(17)) clothingType += 21;
                if (checkedListClothingType.GetItemChecked(18)) clothingType += 22;
                if (checkedListClothingType.GetItemChecked(19)) clothingType += 24;
                if (checkedListClothingType.GetItemChecked(20)) clothingType += 25;
                if (checkedListClothingType.GetItemChecked(21)) clothingType += 26;
                if (checkedListClothingType.GetItemChecked(22)) clothingType += 29;
                if (checkedListClothingType.GetItemChecked(23)) clothingType += 30;
                if (checkedListClothingType.GetItemChecked(24)) clothingType += 31;

                if (checkedListAge.GetItemChecked(0)) ageGenderFlag += 0x1;
                if (checkedListAge.GetItemChecked(1)) ageGenderFlag += 0x2;
                if (checkedListAge.GetItemChecked(2)) ageGenderFlag += 0x4;
                if (checkedListAge.GetItemChecked(3)) ageGenderFlag += 0x8;
                if (checkedListAge.GetItemChecked(4)) ageGenderFlag += 0x10;
                if (checkedListAge.GetItemChecked(5)) ageGenderFlag += 0x20;
                if (checkedListAge.GetItemChecked(6)) ageGenderFlag += 0x40;

                if (checkedListGender.GetItemChecked(0)) ageGenderFlag += 0x1000;
                if (checkedListGender.GetItemChecked(1)) ageGenderFlag += 0x2000;

                if (checkedListOther.GetItemChecked(0)) ageGenderFlag += 0x100000;
                if (checkedListOther.GetItemChecked(1)) ageGenderFlag += 0x200000;
                if (checkedListOther.GetItemChecked(2)) ageGenderFlag += 0x10000;

                if (checkedListCategory.GetItemChecked(0)) clothingCategory += 0x1;
                if (checkedListCategory.GetItemChecked(1)) clothingCategory += 0x2;
                if (checkedListCategory.GetItemChecked(2)) clothingCategory += 0x4;
                if (checkedListCategory.GetItemChecked(3)) clothingCategory += 0x8;
                if (checkedListCategory.GetItemChecked(4)) clothingCategory += 0x10;
                if (checkedListCategory.GetItemChecked(5)) clothingCategory += 0x20;
                if (checkedListCategory.GetItemChecked(6)) clothingCategory += 0x40;
                if (checkedListCategory.GetItemChecked(7)) clothingCategory += 0x100;
                if (checkedListCategory.GetItemChecked(8)) clothingCategory += 0xFFFF;

                if (checkedListCategoryExtended.GetItemChecked(0)) clothingCategory += 0x100000;
                if (checkedListCategoryExtended.GetItemChecked(1)) clothingCategory += 0x200000;
                if (checkedListCategoryExtended.GetItemChecked(2)) clothingCategory += 0x400000;
                if (checkedListCategoryExtended.GetItemChecked(3)) clothingCategory += 0x800000;
                if (checkedListCategoryExtended.GetItemChecked(4)) clothingCategory += 0x1000000;

                lstCasPartDetails.Items[3].Text = typeFlag + " (0x" + typeFlag.ToString("X8") + ")";
                lstCasPartDetails.Items[4].Text = ageGenderFlag + " (0x" + ageGenderFlag.ToString("X8") + ")";
                lstCasPartDetails.Items[5].Text = clothingCategory + " (0x" + clothingCategory.ToString("X8") + ")";
                lstCasPartDetails.Items[2].Text = clothingType + " (0x" + clothingType.ToString("X8") + ")";

            }
            if (casPartNew == null)
            {
                casPartSrc.ageGenderFlag = ageGenderFlag;
                casPartSrc.clothingCategory = clothingCategory;
                casPartSrc.typeFlag = typeFlag;
                casPartSrc.clothingType = clothingType;
            }
            else
            {
                casPartNew.ageGenderFlag = ageGenderFlag;
                casPartNew.clothingCategory = clothingCategory;
                casPartNew.typeFlag = typeFlag;
                casPartNew.clothingType = clothingType;
            }

        }

        private void btnCommitLstOther_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                int curDesign = listView1.SelectedItems[0].Index;
                xmlChunkDetails chunk = (xmlChunkDetails)casPartNew.xmlChunk[curDesign];

                updateChunkFromLists(lstOtherDetails, chunk);

            }
        }

        private void btnPatternChannelTextureFind_Click(object sender, EventArgs e)
        {
            KeyUtils.findAndShowImage(txtPatternChannelTexture.Text);
        }

        private void btnPatternChannelTextureReplace_Click(object sender, EventArgs e)
        {
            txtPatternChannelTexture.Text = replaceImageKey(txtPatternChannelTexture.Text);
        }

        private void btnPatternChannelCommit_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {

                int patternNo = cmbPatternSelect.SelectedIndex;
                int channelNo = cmbChannelSelect.SelectedIndex;
                xmlChunkDetails chunk = (xmlChunkDetails)casPartNew.xmlChunk[listView1.SelectedIndices[0]];

                if (chkPatternChannelEnabled.Checked)
                {
                    chunk.pattern[patternNo].ChannelEnabled[channelNo] = "True";
                }
                else
                {
                    chunk.pattern[patternNo].ChannelEnabled[channelNo] = "False";
                }

                chunk.pattern[patternNo].Channel[channelNo] = txtPatternChannelTexture.Text;
                chunk.pattern[patternNo].H[channelNo] = txtPatternChannelH.Text;
                chunk.pattern[patternNo].S[channelNo] = txtPatternChannelS.Text;
                chunk.pattern[patternNo].V[channelNo] = txtPatternChannelV.Text;
                chunk.pattern[patternNo].BaseH[channelNo] = txtPatternChannelBaseH.Text;
                chunk.pattern[patternNo].BaseS[channelNo] = txtPatternChannelBaseS.Text;
                chunk.pattern[patternNo].BaseV[channelNo] = txtPatternChannelBaseV.Text;
                chunk.pattern[patternNo].HSVShift[channelNo] = txtPatternChannelBaseHSVShift.Text;

            }
        }

        private void makePatternPreviewThumb()
        {
            if (listView1.SelectedItems.Count == 1)
            {

                patternDetails temp = commitPatternDetails();

                picPatternThumb.Image = this.pBrowser.makePatternThumb(pBrowser.findPattern(temp.key), false, pBrowser.pDetailsTopFile(temp));
                picPatternThumb.Visible = true;
            }

        }

        private void picPatternColourBg_Click(object sender, EventArgs e)
        {
            picPatternColourBg.BackColor = showColourDialog(picPatternColourBg.BackColor);
            picPatternColourBg.Tag = "color";

            makePatternPreviewThumb();
        }

        private void picPatternColour1_Click(object sender, EventArgs e)
        {
            picPatternColour1.BackColor = showColourDialog(picPatternColour1.BackColor);
            picPatternColour1.Tag = "color";
            makePatternPreviewThumb();
        }

        private void picPatternColour2_Click(object sender, EventArgs e)
        {
            picPatternColour2.BackColor = showColourDialog(picPatternColour2.BackColor);
            picPatternColour2.Tag = "color";
            makePatternPreviewThumb();
        }

        private void picPatternColour3_Click(object sender, EventArgs e)
        {
            picPatternColour3.BackColor = showColourDialog(picPatternColour3.BackColor);
            picPatternColour3.Tag = "color";
            makePatternPreviewThumb();
        }

        private void picPatternColour4_Click(object sender, EventArgs e)
        {
            picPatternColour4.BackColor = showColourDialog(picPatternColour4.BackColor);
            picPatternColour4.Tag = "color";
            makePatternPreviewThumb();
        }

        private void cmbPatternSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                showPatternDetails(listView1.SelectedIndices[0]);
            }

        }

        private void showPatternDetails(int designNo)
        {
            xmlChunkDetails chunk = (xmlChunkDetails)casPartNew.xmlChunk[designNo];

            int i = cmbPatternSelect.SelectedIndex;
            if (chunk.filename == "CasRgbMask" && i == 3)
            {
                grpPatternA.Enabled = false;
            }
            else
            {
                grpPatternA.Enabled = true;
                showPatternDetails(chunk.pattern[i]);
            }
        }

        private void showPatternDetails(patternDetails pDetails)
        {
            showPatternDetails(pDetails, true);
        }

        private void showPatternDetails(patternDetails pDetails, bool doEnable)
        {

            switch (pDetails.type)
            {
                case "solidColor":
                    cmbPatternAType.SelectedIndex = 0;
                    break;
                case "HSV":
                    cmbPatternAType.SelectedIndex = 2;
                    break;
                case "Coloured":
                    cmbPatternAType.SelectedIndex = 1;
                    break;
            }

            if (doEnable)
            {
                if (pDetails.Enabled.ToLower() == "true") { chkPatternAEnabled.Checked = true; }
                else { chkPatternAEnabled.Checked = false; }
            }
            txtPatternABaseHBg.Text = pDetails.BaseHBg;
            txtPatternABaseSBg.Text = pDetails.BaseSBg;
            txtPatternABaseVBg.Text = pDetails.BaseVBg;
            txtPatternBGImage.Text = pDetails.BackgroundImage;
            txtPatternAFilename.Text = pDetails.filename;
            txtPatternAHBg.Text = pDetails.HBg;
            txtPatternASBg.Text = pDetails.SBg;
            txtPatternAVBg.Text = pDetails.VBg;
            txtPatternAHSVShiftBG.Text = pDetails.HSVShiftBg;
            txtPatternAKey.Text = pDetails.key;
            txtPatternAName.Text = pDetails.name;
            if (pDetails.Linked.ToLower() == "true") { chkPatternALinked.Checked = true; }
            else { chkPatternALinked.Checked = false; }
            txtPatternARGBMask.Text = pDetails.rgbmask;
            txtPatternASpecular.Text = pDetails.specmap;
            txtPatternATiling.Text = pDetails.Tiling;
            picPatternSolidColour.BackColor = MadScience.Helpers.convertColour(pDetails.Color);

            grpPatternA.Enabled = true;

            if (pDetails.type == "Coloured")
            {
                PatternBrowser.patternsFile pFile = pBrowser.pDetailsTopFile(pDetails);
                PatternBrowser.patternsFile pattern = pBrowser.findPattern(pDetails.key);

                // Check if pFile has the correct colours
                if (String.IsNullOrEmpty(pattern.color0)) { pFile.color0 = ""; pDetails.ColorP[0] = null; }
                if (String.IsNullOrEmpty(pattern.color1)) { pFile.color1 = ""; pDetails.ColorP[1] = null; }
                if (String.IsNullOrEmpty(pattern.color2)) { pFile.color2 = ""; pDetails.ColorP[2] = null; }
                if (String.IsNullOrEmpty(pattern.color3)) { pFile.color3 = ""; pDetails.ColorP[3] = null; }
                if (String.IsNullOrEmpty(pattern.color4)) { pFile.color4 = ""; pDetails.ColorP[4] = null; }

                picPatternThumb.Image = this.pBrowser.makePatternThumb(pattern, false, pFile);
                picPatternThumb.Visible = true;
            }
            
            if (pDetails.type == "HSV")
            {
                if (File.Exists(Application.StartupPath + "\\patterncache\\" + pDetails.name + ".png"))
                {
                    Stream pngThumb = File.OpenRead(Application.StartupPath + "\\patterncache\\" + pDetails.name + ".png");
                    picPatternThumb.Image = Image.FromStream(pngThumb);
                    pngThumb.Close();
                    picPatternThumb.Visible = true;
                }
                else
                {
                    picPatternThumb.Image = this.pBrowser.makePatternThumb(pDetails.key);
                    picPatternThumb.Visible = true;
                }
            }

            Color temp;
            temp = MadScience.Helpers.convertColour(pDetails.ColorP[0], true);
            picPatternColourBg.BackColor = temp;
            if (temp == Color.Empty)
            {
                picPatternColourBg.Tag = "empty";
                picPatternColourBg.Visible = false;
            }
            else
            {
                picPatternColourBg.Tag = "color";
                picPatternColourBg.Visible = true;
            }

            temp = MadScience.Helpers.convertColour(pDetails.ColorP[1], true);
            picPatternColour1.BackColor = temp;
            if (temp == Color.Empty)
            {
                picPatternColour1.Tag = "empty";
                picPatternColour1.Visible = false;
            }
            else
            {
                picPatternColour1.Tag = "color";
                picPatternColour1.Visible = true;
            }

            temp = MadScience.Helpers.convertColour(pDetails.ColorP[2], true);
            picPatternColour2.BackColor = temp;
            if (temp == Color.Empty)
            {
                picPatternColour2.Tag = "empty";
                picPatternColour2.Visible = false;
            }
            else
            {
                picPatternColour2.Tag = "color";
                picPatternColour2.Visible = true;
            }

            temp = MadScience.Helpers.convertColour(pDetails.ColorP[3], true);
            picPatternColour3.BackColor = temp;
            if (temp == Color.Empty)
            {
                picPatternColour3.Tag = "empty";
                picPatternColour3.Visible = false;
            }
            else
            {
                picPatternColour3.Tag = "color";
                picPatternColour3.Visible = true;
            }

            temp = MadScience.Helpers.convertColour(pDetails.ColorP[4], true);
            picPatternColour4.BackColor = temp;
            if (temp == Color.Empty)
            {
                picPatternColour4.Tag = "empty";
                picPatternColour4.Visible = false;
            }
            else
            {
                picPatternColour4.Tag = "color";
                picPatternColour4.Visible = true;
            }

        }

        private void picLstOtherColour_Click(object sender, EventArgs e)
        {
            if (lstOtherDetails.SelectedItems.Count == 1)
            {

                ColorPicker.ColorPickerDialog cpd = new ColorPicker.ColorPickerDialog();
                cpd.Color = picLstOtherColour.BackColor;
                cpd.ShowDialog();

                picLstOtherColour.BackColor = cpd.Color;

                ListViewItem item = lstOtherDetails.SelectedItems[0];
                
                item.SubItems[0].Text = MadScience.Helpers.convertColour(picLstOtherColour.BackColor);
                item.Text = item.SubItems[0].Text;
            }

        }

        private void btnCasPartDetailsCommit_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lstCasPartDetails.Items.Count; i++)
            {

                switch (lstCasPartDetails.Items[i].SubItems[1].Text)
                {
                    case "Mesh Name":
                        casPartNew.meshName = lstCasPartDetails.Items[i].Text;
                        break;
                    case "Clothing Order":
                        casPartNew.clothingOrder = Convert.ToSingle(lstCasPartDetails.Items[i].Text);
                        break;
                }
                if (lstCasPartDetails.Items[i].SubItems[1].Text.StartsWith("TGI #"))
                {
                    string igtText = lstCasPartDetails.Items[i].SubItems[1].Text.Replace("IGT #", "");
                    igtText = igtText.Substring(0, igtText.IndexOf(" ") - 1);
                    int igtNo = Convert.ToInt32(igtText);
                    casPartNew.tgi64list[igtNo] = new ResourceKey(lstCasPartDetails.Items[i].Text);
                    //tgi64 tgi = (tgi64)casPartNew.tgi64list[igtNo];
                }

            }

        }


        private void cmbPatternAType_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cmbPatternAType.SelectedIndex)
            {
                case 0: // solidColour
                    groupBox3.Visible = true;
                    groupBox5.Visible = false;
                    groupBox7.Visible = false;
                    break;
                case 1: // Coloured
                    groupBox3.Visible = false;
                    groupBox5.Visible = false;
                    groupBox7.Visible = true;
                    break;
                case 2: // HSV
                    groupBox3.Visible = false;
                    groupBox5.Visible = true;
                    groupBox7.Visible = false;
                    cmbChannelSelect.SelectedIndex = 0;
                    break;
            }
        }

        private void cmbChannelSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {

                int patternNo = cmbPatternSelect.SelectedIndex;
                int channelNo = cmbChannelSelect.SelectedIndex;
                xmlChunkDetails chunk = (xmlChunkDetails)casPartNew.xmlChunk[listView1.SelectedIndices[0]];

                if (chunk.pattern[patternNo].ChannelEnabled[channelNo] == "True")
                {
                    chkPatternChannelEnabled.Checked = true;
                }
                else
                {
                    chkPatternChannelEnabled.Checked = false;
                }

                txtPatternChannelTexture.Text = chunk.pattern[patternNo].Channel[channelNo];
                txtPatternChannelH.Text = chunk.pattern[patternNo].H[channelNo];
                txtPatternChannelS.Text = chunk.pattern[patternNo].S[channelNo];
                txtPatternChannelV.Text = chunk.pattern[patternNo].V[channelNo];
                txtPatternChannelBaseH.Text = chunk.pattern[patternNo].BaseH[channelNo];
                txtPatternChannelBaseS.Text = chunk.pattern[patternNo].BaseS[channelNo];
                txtPatternChannelBaseV.Text = chunk.pattern[patternNo].BaseV[channelNo];
                txtPatternChannelBaseHSVShift.Text = chunk.pattern[patternNo].HSVShift[channelNo];

            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (cmbPatternStencil1.SelectedIndex > 0 && listView1.SelectedItems.Count == 1)
            {
                KeyUtils.findAndShowImage(stencilPool[cmbPatternStencil1.SelectedIndex - 1].key);
            }
        }

        private void findKeyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form frm = new Form2();
            frm.Icon = this.Icon;
            frm.ShowDialog();
            frm.Close();
            frm = null;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (cmbPatternStencil2.SelectedIndex > 0 && listView1.SelectedItems.Count == 1)
            {
                KeyUtils.findAndShowImage(stencilPool[cmbPatternStencil2.SelectedIndex - 1].key);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (cmbPatternStencil3.SelectedIndex > 0 && listView1.SelectedItems.Count == 1)
            {
                KeyUtils.findAndShowImage(stencilPool[cmbPatternStencil3.SelectedIndex - 1].key);
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (cmbPatternStencil4.SelectedIndex > 0 && listView1.SelectedItems.Count == 1)
            {
                KeyUtils.findAndShowImage(stencilPool[cmbPatternStencil4.SelectedIndex - 1].key);
            }

        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (cmbPatternStencil5.SelectedIndex > 0 && listView1.SelectedItems.Count == 1)
            {
                KeyUtils.findAndShowImage(stencilPool[cmbPatternStencil5.SelectedIndex - 1].key);
            }

        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (cmbPatternStencil6.SelectedIndex > 0 && listView1.SelectedItems.Count == 1)
            {
                KeyUtils.findAndShowImage(stencilPool[cmbPatternStencil6.SelectedIndex - 1].key);
            }

        }

        private void btnDesignStencilCommit_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                int designNo = listView1.SelectedIndices[0];
                xmlChunkDetails chunk = (xmlChunkDetails)casPartNew.xmlChunk[designNo];
                Console.WriteLine("Setting design " + designNo.ToString() + ".StencilEnabled to " + cmbPatternStencil1.SelectedIndex);
                btnPatternStencil1Preview.Enabled = true;

                stencilDetails blank = new stencilDetails();
                blank.Enabled = "False";

                if (cmbPatternStencil1.SelectedIndex == 0) { chunk.stencil.A = blank; } else { chunk.stencil.A = stencilPool[cmbPatternStencil1.SelectedIndex - 1]; chunk.stencil.A.Enabled = "True"; }
                if (cmbPatternStencil2.SelectedIndex == 0) { chunk.stencil.B = blank; } else { chunk.stencil.B = stencilPool[cmbPatternStencil2.SelectedIndex - 1]; chunk.stencil.B.Enabled = "True"; }
                if (cmbPatternStencil3.SelectedIndex == 0) { chunk.stencil.C = blank; } else { chunk.stencil.C = stencilPool[cmbPatternStencil3.SelectedIndex - 1]; chunk.stencil.C.Enabled = "True"; }
                if (cmbPatternStencil4.SelectedIndex == 0) { chunk.stencil.D = blank; } else { chunk.stencil.D = stencilPool[cmbPatternStencil4.SelectedIndex - 1]; chunk.stencil.D.Enabled = "True"; }
                if (cmbPatternStencil5.SelectedIndex == 0) { chunk.stencil.E = blank; } else { chunk.stencil.E = stencilPool[cmbPatternStencil5.SelectedIndex - 1]; chunk.stencil.E.Enabled = "True"; }
                if (cmbPatternStencil6.SelectedIndex == 0) { chunk.stencil.F = blank; } else { chunk.stencil.F = stencilPool[cmbPatternStencil6.SelectedIndex - 1]; chunk.stencil.F.Enabled = "True"; }

                btnReloadTextures_Click(sender, null);
            }
        
        }

        private void button8_Click(object sender, EventArgs e)
        {
            txtPatternAKey.Text = "key:0333406C:00000000:71D5EFB6C391BC17";
            txtPatternAName.Text = "solidColor_1";
            txtPatternARGBMask.Text = "";
            txtPatternASpecular.Text = "";
            txtPatternATiling.Text = "4.0000,4.0000";
            txtPatternAFilename.Text = @"Materials\Miscellaneous\solidColor_1";
        }

        private void button9_Click(object sender, EventArgs e)
        {
            // Go through the list of DDS files and dump them
            string s3root = MadScience.Helpers.findSims3Root();

            if (s3root != "")
            {

                bool hasShownDialog = false;

                toolStripProgressBar1.Minimum = 0;
                toolStripProgressBar1.Value = 0;
                toolStripStatusLabel1.Text = "Searching for meshes... please wait";
                statusStrip1.Refresh();

                Stream fbuild0 = File.Open(s3root + "\\GameData\\Shared\\Packages\\FullBuild0.package", FileMode.Open, FileAccess.Read, FileShare.Read);
                MadScience.Wrappers.Database db = new MadScience.Wrappers.Database(fbuild0, true);

                /*
                fbuild0.Seek(0, SeekOrigin.Begin);

                MadScience.Wrappers.DatabasePackedFile dbpf = new MadScience.Wrappers.DatabasePackedFile();
                try
                {
                    dbpf.Read(fbuild0);
                }
                catch (MadScience.Wrappers.NotAPackageException)
                {
                    MessageBox.Show("bad file: {0}");
                    fbuild0.Close();
                    return;
                }
                */

                toolStripProgressBar1.Maximum = db._Entries.Count;

                Dictionary<ulong, string> keyNames = new Dictionary<ulong, string>();
                long nowTicks = DateTime.Now.Ticks;
                Console.WriteLine("Started at: " + nowTicks);
                foreach (MadScience.Wrappers.ResourceKey entry in db._Entries.Keys)
                {
                    //MadScience.Wrappers.ResourceKey entry = new MadScience.Wrappers.ResourceKey(keyString);
                    //DatabasePackedFile.Entry entry = db.dbpfEntries[i];
                    if (entry.typeId == (int)0x0166038C)
                    {
                        keyNames = Helpers.getKeyNames(db.GetResourceStream(entry));
                        break;
                    }
                }

                // Calc the 4 lod files
                ulong lod0 = (ulong)MadScience.StringHelpers.HashFNV32(txtCasPartName.Text + "_lod0");
                ulong lod1 = (ulong)MadScience.StringHelpers.HashFNV32(txtCasPartName.Text + "_lod1");
                ulong lod2 = (ulong)MadScience.StringHelpers.HashFNV32(txtCasPartName.Text + "_lod2");
                ulong lod3 = (ulong)MadScience.StringHelpers.HashFNV32(txtCasPartName.Text + "_lod3");
                ulong vpxy = (ulong)MadScience.StringHelpers.HashFNV24(txtCasPartName.Text);

                Console.WriteLine("0x00000000" + MadScience.StringHelpers.HashFNV32(txtCasPartName.Text + "_lod0").ToString("X8").ToUpper());

                int numFound = 0;
                folderBrowserDialog1.SelectedPath = "";

                foreach (MadScience.Wrappers.ResourceKey entry in db._Entries.Keys)
                {

                    //MadScience.Wrappers.ResourceKey entry = new MadScience.Wrappers.ResourceKey(keyString);

                    toolStripProgressBar1.Value++;
                    bool searchChunk = false;
                    string extension = "";
                    //DatabasePackedFile.Entry entry = db.dbpfEntries[i];
                    if (entry.typeId == 0x736884F1)
                    {
                        extension = ".vpxy";
                        if (entry.instanceId == vpxy) searchChunk = true;

                    }
                    if (entry.typeId == 0x015A1849)
                    {
                        extension = ".simgeom";
                        if (entry.instanceId == lod0) searchChunk = true;
                        if (entry.instanceId == lod1) searchChunk = true;
                        if (entry.instanceId == lod2) searchChunk = true;
                        if (entry.instanceId == lod3) searchChunk = true;
                    }
                    if (searchChunk)
                    {
                        string fileNameToSave = "";
                        if (keyNames.ContainsKey(entry.instanceId))
                        {
                            fileNameToSave = keyNames[entry.instanceId];
                            if (fileNameToSave.Contains("0x") == false) { fileNameToSave += "_0x" + entry.instanceId.ToString("X16"); }
                        }
                        else
                        {
                            fileNameToSave = entry.typeId.ToString("X8") + "_" + entry.groupId.ToString("X8") + "_" + entry.instanceId.ToString("X16");
                        }

                        Stream output = db.GetResourceStream(entry);

                        if (!hasShownDialog)
                        {
                            folderBrowserDialog1.Description = "Please select a folder to save the extracted meshes to.";
                            folderBrowserDialog1.ShowDialog();
                            hasShownDialog = true;
                        }
                        if (folderBrowserDialog1.SelectedPath != "")
                        {
                            FileStream saveFile = new FileStream(folderBrowserDialog1.SelectedPath + "\\" + fileNameToSave + extension, FileMode.Create, FileAccess.Write);
                            Helpers.CopyStream(output, saveFile);
                            saveFile.Close();
                            output.Close();
                            numFound++;
                        }


                    }

                    
                }

                toolStripProgressBar1.Value = 0;
                toolStripStatusLabel1.Text = numFound.ToString() + " meshes found";
                statusStrip1.Refresh();

                fbuild0.Close();
            }


        }

        private void button11_Click(object sender, EventArgs e)
        {

            //pBrowser.curCategory = this.patternBrowserCategory;
            if (pBrowser.ShowDialog() == DialogResult.OK)
            {
                showPatternDetails(pBrowser.selectedPattern, false);
            }

            pBrowser.Hide();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //MessageBox.Show(Environment.GetCommandLineArgs().Length.ToString());
            if (Environment.GetCommandLineArgs().Length > 1)
            {
                //MessageBox.Show(Environment.GetCommandLineArgs()[0] + " " + Environment.GetCommandLineArgs()[1]);
                loadFile(Environment.GetCommandLineArgs()[1].ToString());
            }
            else
            {
                newToolStripMenuItem_Click(this, null);
            }
        }

        private void btnCustomThumbnail_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                xmlChunkDetails chunk = (xmlChunkDetails)casPartNew.xmlChunk[listView1.SelectedIndices[0]];
                if (chkUseCustomThumbnail.Checked == true)
                {
                    if (txtCustomThumbnailPath.Text.Trim() != "")
                    {
                        chunk.hasCustomThumbnail = true;
                        this.newPNGfiles.Add(listView1.SelectedIndices[0], txtCustomThumbnailPath.Text);
                    }
                }
            }
        }

        private void lstCasPartDetails_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button11_Click_1(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "PNG Thumbnail|*.png";
            if (openFileDialog1.ShowDialog() == DialogResult.OK && openFileDialog1.FileName.Trim() != "")
            {
                txtCustomThumbnailPath.Text = openFileDialog1.FileName;
            }
        }

        private void debugModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            debugModeToolStripMenuItem.Checked = !debugModeToolStripMenuItem.Checked;
        }

        private void debugModeToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (debugModeToolStripMenuItem.Checked == true)
            {
                btnCasPartDetailsCommit.Visible = true;
                lstCasPartDetails.LabelEdit = true;
                btnGetOrigXML.Visible = true;
                btnGetNewXML.Visible = true;

                Helpers.saveRegistryValue("debugMode", "True");
            }
            else
            {
                btnCasPartDetailsCommit.Visible = false;
                lstCasPartDetails.LabelEdit = false;
                btnGetOrigXML.Visible = false;
                btnGetNewXML.Visible = false;

                Helpers.saveRegistryValue("debugMode", "False");
            }
        }

        private void cmbPatternStencil1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbPatternStencil1.SelectedIndex == 0) { btnPatternStencil1Preview.Enabled = false; } else { btnPatternStencil1Preview.Enabled = true; }
        }

        private void cmbPatternStencil2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbPatternStencil2.SelectedIndex == 0) { btnPatternStencil2Preview.Enabled = false; } else { btnPatternStencil2Preview.Enabled = true; }
        }

        private void cmbPatternStencil3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbPatternStencil3.SelectedIndex == 0) { btnPatternStencil3Preview.Enabled = false; } else { btnPatternStencil3Preview.Enabled = true; }
        }

        private void cmbPatternStencil4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbPatternStencil4.SelectedIndex == 0) { btnPatternStencil4Preview.Enabled = false; } else { btnPatternStencil4Preview.Enabled = true; }
        }

        private void cmbPatternStencil5_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbPatternStencil5.SelectedIndex == 0) { btnPatternStencil5Preview.Enabled = false; } else { btnPatternStencil5Preview.Enabled = true; }
        }

        private void cmbPatternStencil6_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbPatternStencil6.SelectedIndex == 0) { btnPatternStencil6Preview.Enabled = false; } else { btnPatternStencil6Preview.Enabled = true; }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //saveFileDialog1.FileName = "";
            //saveFileDialog1.Filter = "Sims 3 Packages|*.package|CASPart|*.caspart";
            if (Helpers.currentPackageFile != "")
            {

                FileInfo f = new FileInfo(Helpers.currentPackageFile);

                if (f.Extension == ".package")
                {

                    Stream saveFile = File.Open(Helpers.currentPackageFile, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);

                    //ulong instanceId = MadScience.StringHelpers.HashFNV64("CTU_" + DateTime.Now.Ticks + "_" + MadScience.Helpers.sanitiseString(f.Name));
                    ulong instanceId = MadScience.StringHelpers.HashFNV64(casPartNew.meshName);
                    Database db = new Database(saveFile, true);

                    saveToDBPF(db, instanceId, false);

                    db.Commit(true);

                    saveFile.Close();
                }
            }
        }

        private void checkedListType_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // Uncheck all other boxes
            for (int i = 0; i < checkedListType.Items.Count; i++)
            {
                if (i != e.Index)
                {
                    checkedListType.SetItemChecked(i, false);
                }
            }
        }

        private void checkedListClothingType_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // Uncheck all other boxes
            for (int i = 0; i < checkedListClothingType.Items.Count; i++)
            {
                if (i != e.Index)
                {
                    checkedListClothingType.SetItemChecked(i, false);
                }
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {

            toolStripProgressBar1.Value = 0;
            toolStripProgressBar1.Minimum = 0;
            toolStripProgressBar1.Maximum = this.lookupList.Items.Length;

            for (int i = 0; i < this.lookupList.Items.Length; i++)
            {
                toolStripProgressBar1.Value = i;

                toolStripStatusLabel1.Text = "Generating CAS Part, please wait..." + (i + 1).ToString() + " of " + toolStripProgressBar1.Maximum;
                statusStrip1.Refresh();

                filesFile temp = lookupList.Items[i];

                casPartFile cFile = new casPartFile();
                Stream cFileOrig = File.OpenRead(Application.StartupPath + "\\casparts\\" + temp.fullCasPartname + ".caspart");
                cFile.Load(cFileOrig);
                cFileOrig.Close();

                if (cFile.cFile.xmlChunkRaw.Count == 0)
                {
                    // Find complate preset XML and load it in
                    String complate = File.ReadAllText(@"P:\Stuart\Desktop\FullBuild0\config\xml\root\" + temp.fullCasPartname + ".xml", System.Text.Encoding.ASCII);
                    cFile.cFile.xmlChunkRaw.Add(complate);
                    cFile.parseRawXML(1);

                    File.Delete(Application.StartupPath + "\\casparts\\" + temp.fullCasPartname + ".caspart");
                    Stream casPartSave = File.OpenWrite(Application.StartupPath + "\\casparts\\" + temp.fullCasPartname + ".caspart");
                    cFile.Save(casPartSave, cFile.cFile, false);
                    casPartSave.Close();

                    Console.WriteLine("Finished writing " + cFile.cFile.meshName + ".caspart");
                }
            }
            
        }

        private void generateThumbnailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripProgressBar1.Value = 0;
            toolStripProgressBar1.Minimum = 0;
            toolStripProgressBar1.Maximum = this.lookupList.Items.Length;

            for (int i = 0; i < this.lookupList.Items.Length; i++)
            {
                toolStripProgressBar1.Value = i;

                toolStripStatusLabel1.Text = "Generating CAS Part thumbnails, please wait..." + (i + 1).ToString() + " of " + toolStripProgressBar1.Maximum;
                statusStrip1.Refresh();

                filesFile temp = lookupList.Items[i];
                if (File.Exists(Application.StartupPath + "\\cache\\" + temp.fullCasPartname + ".png"))
                {
                    File.Delete(Application.StartupPath + "\\cache\\" + temp.fullCasPartname + ".png");
                }
                Image img = extractCASThumbnail(temp.fullCasPartname);
                if (img != null)
                {
                    try
                    {
                        img.Save(Application.StartupPath + "\\cache\\" + temp.fullCasPartname + ".png");
                    }
                    catch (Exception ex)
                    {
                    }
                }

            }

            toolStripStatusLabel1.Text = "";
            toolStripProgressBar1.Value = 0;
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form3 frm = new Form3();
            frm.Icon = this.Icon;
            frm.ShowDialog();
            frm = null;
        }

        private void lstTextureDetails_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstTextureDetails.SelectedItems.Count == 1)
            {
                ListViewItem item = lstTextureDetails.SelectedItems[0];
                switch (item.Tag.ToString())
                {
                    case "texture":
                        btnListTextureFind.Enabled = true;
                        btnListTextureReplace.Enabled = true;
                        break;
                    case "color":
                        picLstTextureColour.Enabled = true;
                        picLstTextureColour.BackColor = Helpers.convertColour(item.SubItems[0].Text);
                        break;
                    default:
                        btnListTextureFind.Enabled = false;
                        btnListTextureReplace.Enabled = false;
                        picLstTextureColour.Enabled = false;
                        break;
                }
            }
        }

        private void listFindOrReplace(object sender, bool isFind)
        {
            ListView lView = (ListView)sender;
            if (lView.SelectedItems.Count == 1)
            {
                ListViewItem item = lView.SelectedItems[0];
                switch (item.Tag.ToString())
                {
                    case "texture":
                        if (isFind)
                        {
                            KeyUtils.findAndShowImage(item.SubItems[0].Text);
                        }
                        else
                        {
                            item.SubItems[0].Text = replaceImageKey(item.SubItems[0].Text);
                            item.Text = item.SubItems[0].Text;
                        }

                        break;
                    default:
                        break;
                }
            }

        }

        private void btnListTextureFind_Click(object sender, EventArgs e)
        {

            listFindOrReplace(lstTextureDetails, true);

        }

        private void btnListTextureReplace_Click(object sender, EventArgs e)
        {
            listFindOrReplace(lstTextureDetails, false);
        }

        private void picListTextureColour_Click(object sender, EventArgs e)
        {
            if (lstTextureDetails.SelectedItems.Count == 1)
            {

                ColorPicker.ColorPickerDialog cpd = new ColorPicker.ColorPickerDialog();
                cpd.Color = picLstTextureColour.BackColor;
                cpd.ShowDialog();

                picLstTextureColour.BackColor = cpd.Color;

                ListViewItem item = lstTextureDetails.SelectedItems[0];

                item.SubItems[0].Text = MadScience.Helpers.convertColour(picLstTextureColour.BackColor);
                item.Text = item.SubItems[0].Text;
            }

        }

        private void updateChunkFromLists(ListView lView, xmlChunkDetails chunk)
        {
            for (int i = 0; i < lView.Items.Count; i++)
            {
                ListViewItem item = lView.Items[i];

                switch (item.SubItems[1].Text)
                {
                    case "Name":
                        chunk.name = item.Text;
                        break;
                    case "reskey":
                        chunk.reskey = item.Text;
                        break;
                    case "assetRoot":
                        chunk.assetRoot = item.Text;
                        break;

                    case "Overlay":
                        chunk.Overlay = item.Text;
                        break;

                    case "Control Map":
                        chunk.ControlMap = item.Text;
                        break;
                    case "Diffuse Map":
                        chunk.DiffuseMap = item.Text;
                        break;

                    case "Mask":
                        chunk.Mask = item.Text;
                        break;

                    case "MaskHeight":
                        chunk.MaskHeight = item.Text;
                        break;

                    case "MaskWidth":
                        chunk.MaskWidth = item.Text;
                        break;

                    case "Base Texture":
                    case "Multiplier":
                        chunk.Multiplier = item.Text;
                        break;

                    case "IsHat":
                        chunk.IsHat = item.Text;
                        break;
                    case "DrawsOnFace":
                        chunk.DrawsOnFace = item.Text;
                        break;
                    case "DrawsOnScalp":
                        chunk.DrawsOnScalp = item.Text;
                        break;
                    case "IsNaked":
                        chunk.IsNaked = item.Text;
                        break;

                    case "IsNotNaked":
                        chunk.IsNotNaked = item.Text;
                        break;

                    case "Skin Specular":
                        chunk.SkinSpecular = item.Text;
                        break;

                    case "Skin Ambient":
                        chunk.SkinAmbient = item.Text;
                        break;

                    case "Clothing Specular":
                        chunk.ClothingSpecular = item.Text;
                        break;

                    case "Clothing Ambient":
                        chunk.ClothingAmbient = item.Text;
                        break;

                    case "Rotation":
                        chunk.Rotation = item.Text;
                        break;

                    case "Root Color":
                        chunk.hair.RootColor = item.Text;
                        break;
                    case "Diffuse Color":
                        chunk.hair.DiffuseColor = item.Text;
                        break;
                    case "Highlight Color":
                        chunk.hair.HighlightColor = item.Text;
                        break;
                    case "Tip Color":
                        chunk.hair.TipColor = item.Text;
                        break;

                    case "Face Overlay":
                        chunk.faceOverlay = item.Text;
                        break;
                    case "Face Specular":
                        chunk.faceSpecular = item.Text;
                        break;

                    case "Tint Color":
                        chunk.TintColor = item.Text;
                        break;
                    case "Tint Color A":
                        chunk.tint.A.color = item.Text;
                        break;
                    case "Tint Color B":
                        chunk.tint.B.color = item.Text;
                        break;
                    case "Tint Color C":
                        chunk.tint.C.color = item.Text;
                        break;
                    case "Tint Color D":
                        chunk.tint.D.color = item.Text;
                        break;
                    case "Tint Color A Enabled":
                        chunk.tint.A.enabled = item.Text;
                        break;
                    case "Tint Color B Enabled":
                        chunk.tint.B.enabled = item.Text;
                        break;
                    case "Tint Color C Enabled":
                        chunk.tint.C.enabled = item.Text;
                        break;
                    case "Tint Color D Enabled":
                        chunk.tint.D.enabled = item.Text;
                        break;

                    case "Scalp Diffuse Map":
                        chunk.hair.ScalpDiffuseMap = item.Text;
                        break;
                    case "Scalp Control Map":
                        chunk.hair.ScalpControlMap = item.Text;
                        break;
                    case "Scalp Specular Map":
                        chunk.hair.ScalpSpecularMap = item.Text;
                        break;
                    case "Scalp AO":
                        chunk.hair.ScalpAO = item.Text;
                        break;
                    case "Face Diffuse Map":
                        chunk.hair.FaceDiffuseMap = item.Text;
                        break;
                    case "Face Control Map":
                        chunk.hair.FaceControlMap = item.Text;
                        break;
                    case "Face Specular Map":
                        chunk.hair.FaceSpecularMap = item.Text;
                        break;
                    case "Face AO":
                        chunk.hair.FaceAO = item.Text;
                        break;

                    case "Part Mask":
                        chunk.PartMask = item.Text;
                        break;

                    case "partType":
                        chunk.partType = item.Text;
                        break;

                    case "age":
                        chunk.age = item.Text;
                        break;

                    case "gender":
                        chunk.gender = item.Text;
                        break;

                    case "bodyType":
                        chunk.bodyType = item.Text;
                        break;

                    case "daeFileName":
                        chunk.daeFileName = item.Text;
                        break;

                    case "filename":
                        chunk.filename = item.Text;
                        break;

                }

            }

        }

        private void btnCommitLstTexture_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {

                int curDesign = listView1.SelectedItems[0].Index;
                xmlChunkDetails chunk = (xmlChunkDetails)casPartNew.xmlChunk[curDesign];

                updateChunkFromLists(lstTextureDetails, chunk);

                btnReloadTextures_Click(sender, null);
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            listFindOrReplace(listView3, true);
        }

        private void listView3_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            listToggleSelected((ListView)sender, e);
        }

        private void listToggleSelected(ListView lView, ListViewItemSelectionChangedEventArgs e)
        {
            if (lView.SelectedItems.Count == 1)
            {
                ListViewItem item = lView.SelectedItems[0];
                if (item.Tag != null && item.Tag.ToString() == "readonly")
                {
                    e.Item.Selected = false;
                    e.Item.Focused = false;
                }
            }
        }

        private void listToggleFocus(ListView lView)
        {
            if (lView.Items.Count > 2)
            {
                if (lView.Items[0].Tag != null && lView.Items[0].Tag.ToString() == "readonly")
                {
                    lView.Items[0].Focused = false;
                    lView.Items[1].Selected = true;
                    lView.Items[1].Focused = true;
                }
            }
        }

        private void listView3_Enter(object sender, EventArgs e)
        {
            listToggleFocus((ListView)sender);
        }

        private void button7_Click_1(object sender, EventArgs e)
        {
            listFindOrReplace(listView3, false);
        }

        private void listView3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView3.SelectedItems.Count == 1)
            {
                ListViewItem item = listView3.SelectedItems[0];
                switch (item.Tag.ToString())
                {
                    case "texture":
                        button12.Enabled = true;
                        button7.Enabled = true;
                        break;
                    default:
                        button12.Enabled = false;
                        button7.Enabled = false;
                        break;
                }
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {

            // Fill up the rest of the stencil pool
            int numLeft = 20 - stencilPool.Count;
            for (int i = 0; i < numLeft; i++)
            {
                stencilPool.Add(new stencilDetails());
            }

            int curStencilNum = 0;

            stencilDetails stencil = new stencilDetails();
            for (int i = 0; i < listView3.Items.Count; i++)
            {
                ListViewItem item = listView3.Items[i];

                switch (item.SubItems[1].Text)
                {
                    case " Stencil 1":
                        curStencilNum = 0;
                        break;
                    case "Texture":
                        stencil.key = item.Text;
                        break;
                    case "Rotation":
                        stencil.Rotation = item.Text;
                        break;
                    case "Tiling":
                        stencil.Tiling = item.Text;
                        break;
                    case "":
                        stencilPool[curStencilNum] = stencil;
                        curStencilNum++;
                        stencil = new stencilDetails();
                        break;
                }
            }
        }

        private void lstTextureDetails_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            listToggleSelected((ListView)sender, e);
        }

        private void lstTextureDetails_Enter(object sender, EventArgs e)
        {
            listToggleFocus((ListView)sender);
        }

        private void lstOtherDetails_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            listToggleSelected((ListView)sender, e);
        }

        private void lstOtherDetails_Enter(object sender, EventArgs e)
        {
            listToggleFocus((ListView)sender);
        }

        private void btnGetOrigXML_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                string xmlData = (string)casPartNew.xmlChunkRaw[listView1.SelectedIndices[0]];
                Clipboard.SetDataObject(xmlData.Replace("/><", "/>" + Environment.NewLine + "<"));
            }
        }

        private void btnGetNewXML_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {

                //casPart.casPartFile casPF = new casPartFile();
                //Clipboard.SetDataObject(casPF.getXmlChunk(casPartNew, listView1.SelectedIndices[0]));
                casPartFile cFile = new casPartFile();

                string xmlData = cFile.generateXMLChunk(casPartNew, listView1.SelectedIndices[0], false);
                Clipboard.SetDataObject(xmlData.Replace("/><", "/>" + Environment.NewLine + "<"));

            }

        }

        private void btnReloadTextures_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1 && cEnable3DPreview.Checked == true)
            {

                xmlChunkDetails details = (xmlChunkDetails)casPartNew.xmlChunk[listView1.SelectedIndices[0]];

                renderWindow1.RenderEnabled = false;

                DateTime startTime = DateTime.Now;
                toolStripStatusLabel1.Text = "Reloading 3d view... please wait...";
                statusStrip1.Refresh();
                
                renderWindow1.loadTexture(KeyUtils.findKey(details.ClothingAmbient), "ambientTexture");
                renderWindow1.loadTexture(KeyUtils.findKey(details.ClothingSpecular), "specularTexture");
                renderWindow1.loadTexture(KeyUtils.findKey(details.Multiplier), "baseTexture");
                generate3DTexture(details);
                /*
                if (details.stencil.A.Enabled == "True")
                {
                    renderWindow1.loadTexture(findKey(details.stencil.A.key), "stencilA");
                }
                else
                {
                    renderWindow1.loadTexture(null, "stencilA");
                }
                */
                renderWindow1.resetDevice();

                DateTime stopTime = DateTime.Now;
                TimeSpan duration = stopTime - startTime;

                toolStripStatusLabel1.Text = "Reloaded 3D in " + duration.Milliseconds + "ms";

                renderWindow1.RenderEnabled = true;
            }

        }

        private void btnStart3D_Click(object sender, EventArgs e)
        {

            if (listView1.SelectedItems.Count == 1 && cEnable3DPreview.Checked == true)
            {
                xmlChunkDetails details = (xmlChunkDetails)casPartNew.xmlChunk[listView1.SelectedIndices[0]];

                toolStripStatusLabel1.Text = "Initialising 3d view... please wait...";
                statusStrip1.Refresh();

                DateTime startTime = DateTime.Now;

                Stream meshStream = Stream.Null;

                // Use the VPXY to get the mesh lod
                Stream vpxyStream = KeyUtils.findKey(casPartNew.tgi64list[casPartNew.tgiIndexVPXY], 0);

                if (vpxyStream != null)
                {
                    VPXYFile vpxyFile = new VPXYFile(vpxyStream);
                    // Get the first VPXY internal link
                    if (vpxyFile.vpxy.linkEntries.Count >= 1 && vpxyFile.vpxy.linkEntries[0].tgiList.Count >= 1 )
                    {
                        meshStream = KeyUtils.findKey(vpxyFile.vpxy.linkEntries[0].tgiList[0], 0);
                    }
                    vpxyStream.Close();
                }

                /*
                uint groupId = MadScience.StringHelpers.HashFNV24(txtMeshName.Text);
                keyName lod0 = new keyName(0x15a1849, groupId, (ulong)MadScience.StringHelpers.HashFNV32(txtMeshName.Text + "_lod0"));
                Console.WriteLine("Checking for lod0: " + lod0.ToString());
                Stream meshStream = searchInPackage(Helpers.currentPackageFile, lod0.ToString());
                if (meshStream == null)
                {
                    meshStream = KeyUtils.findKey(lod0.ToString(), 0);
                }
                if (meshStream == null)
                {
                    keyName lod1 = new keyName(0x15a1849, groupId, (ulong)MadScience.StringHelpers.HashFNV32(txtMeshName.Text + "_lod1"));
                    Console.WriteLine("Checking for lod1: " + lod1.ToString());
                    meshStream = KeyUtils.findKey(lod1.ToString(), 0);
                }
                */

                if (meshStream != Stream.Null)
                {

                    MadScience.Render.modelInfo newModel = MadScience.Render.Helpers.geomToModel(meshStream);
                    newModel.name = txtMeshName.Text;
                    renderWindow1.BackgroundColour = MadScience.Helpers.convertColour(MadScience.Helpers.getRegistryValue("renderBackgroundColour"));
                    renderWindow1.loadDefaultTextures();
                    renderWindow1.setModel(newModel);

                    renderWindow1.loadTexture(KeyUtils.findKey(details.ClothingAmbient), "ambientTexture");
                    renderWindow1.loadTexture(KeyUtils.findKey(details.ClothingSpecular), "specularTexture");
                    renderWindow1.loadTexture(KeyUtils.findKey(details.Multiplier), "baseTexture");
                    generate3DTexture(details);

                    /*
                    if (details.stencil.A.Enabled == "True")
                    {
                        renderWindow1.loadTexture(findKey(details.stencil.A.key), "stencilA");
                    }
                    else
                    {
                        renderWindow1.loadTexture(null, "stencilA");
                    }
                    */

                    renderWindow1.resetDevice();

                    renderWindow1.RenderEnabled = true;


                }
                else
                {
                    renderWindow1.statusLabel.Text = "Sorry, we could not find a mesh!";
                }

                meshStream.Close();

                DateTime stopTime = DateTime.Now;
                TimeSpan duration = stopTime - startTime;
                this.toolStripStatusLabel1.Text = "Loaded 3D in " + duration.Milliseconds + "ms";


            }

        }

        private void generate3DTexture(xmlChunkDetails details)
        {
            if (!bwGenTexture.IsBusy)
                bwGenTexture.RunWorkerAsync(details);
            else
            {
                bwGenTexture.CancelAsync();
            }
        }

        private void bwGenTexture_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            xmlChunkDetails details = (xmlChunkDetails)e.Argument;
            renderWindow1.loadTextureFromBitmap(composeMultiplier(details, details.filename != "CasRgbMask"), "baseTexture");
            e.Result = details;
            renderWindow1.resetDevice();
            renderWindow1.RenderEnabled = true;
        }

        private void bwGenTexture_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if(e.Result != (xmlChunkDetails)casPartNew.xmlChunk[listView1.SelectedIndices[0]])
                generate3DTexture((xmlChunkDetails)casPartNew.xmlChunk[listView1.SelectedIndices[0]]);
        }

        private Bitmap composeMultiplier(xmlChunkDetails details, bool RGBA)
        {
            Color[] pattern1colors = createColorArray(details.pattern[0]);
            Color[] pattern2colors = createColorArray(details.pattern[1]);
            Color[] pattern3colors = createColorArray(details.pattern[2]);
            Color[] pattern4colors = createColorArray(details.pattern[3]);

            List<Stream> stencils = new List<Stream>();

            //Stream[] stencils = new Stream[6];
            if (details.stencil.A.Enabled == "True") stencils.Add(KeyUtils.findKey(details.stencil.A.key));
            if (details.stencil.B.Enabled == "True") stencils.Add(KeyUtils.findKey(details.stencil.B.key));
            if (details.stencil.C.Enabled == "True") stencils.Add(KeyUtils.findKey(details.stencil.C.key));
            if (details.stencil.D.Enabled == "True") stencils.Add(KeyUtils.findKey(details.stencil.D.key));
            if (details.stencil.E.Enabled == "True") stencils.Add(KeyUtils.findKey(details.stencil.E.key));
            if (details.stencil.F.Enabled == "True") stencils.Add(KeyUtils.findKey(details.stencil.F.key));

            DateTime startTime2 = DateTime.Now;
            List<MadScience.Wrappers.ResourceKey> tempList = new List<MadScience.Wrappers.ResourceKey>();
            tempList.Add(new MadScience.Wrappers.ResourceKey(details.Multiplier));
            tempList.Add(new MadScience.Wrappers.ResourceKey(details.Mask));
            tempList.Add(new MadScience.Wrappers.ResourceKey(details.Overlay));
            tempList.Add(new MadScience.Wrappers.ResourceKey(details.pattern[0].rgbmask));
            tempList.Add(new MadScience.Wrappers.ResourceKey(details.pattern[1].rgbmask));
            tempList.Add(new MadScience.Wrappers.ResourceKey(details.pattern[2].rgbmask));
            tempList.Add(new MadScience.Wrappers.ResourceKey(details.pattern[3].rgbmask));

            List<Stream> textures = KeyUtils.findKey(tempList, 2);
            DateTime stopTime2 = DateTime.Now;
            TimeSpan duration2 = stopTime2 - startTime2;
            Console.WriteLine("Key search time: " + duration2.TotalMilliseconds);

            DateTime startTime = DateTime.Now;
            Bitmap output =  PatternProcessor.ProcessTexture(
                textures, 
                pattern1colors, pattern2colors, pattern3colors, pattern4colors, stencils, RGBA);

            DateTime stopTime = DateTime.Now;
            TimeSpan duration = stopTime - startTime;
            Console.WriteLine("Total Texture generation time: " + duration.TotalMilliseconds);
            return output;
        }

        private Color[] createColorArray(patternDetails pDetail)
        {
            Color[] colors = new Color[1];

            if (pDetail.Enabled.ToLower() == "false")
            {
                colors[0] = Color.Empty;
                return colors;
            }

            if (pDetail.type == "solidColor")
            {
                colors[0] = MadScience.Helpers.convertColour( pDetail.Color);
            }
            if (pDetail.type == "HSV")
            {
                //unsupported
                colors[0] = Color.Magenta;
            }
            if (pDetail.type == "Coloured")
            {
                // Always copy directly - we check individual colours in the pattern processor
                colors = new Color[5];
                colors[0] = MadScience.Helpers.convertColour(pDetail.ColorP[0], true);
                colors[1] = MadScience.Helpers.convertColour(pDetail.ColorP[1], true);
                colors[2] = MadScience.Helpers.convertColour(pDetail.ColorP[2], true);
                colors[3] = MadScience.Helpers.convertColour(pDetail.ColorP[3], true);
                colors[4] = MadScience.Helpers.convertColour(pDetail.ColorP[4], true);

            }
            return colors;
        }

        private void renderWindow1_Scroll(object sender, ScrollEventArgs e)
        {
            Console.WriteLine("Blah");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            KeyUtils.findAndShowImage("key:00B2D882:00000000:75F8F21E0F143CAC");
        }

        private void updateLists(string searchText, string replaceValue)
        {
            for (int i = 0; i < lstCasPartDetails.Items.Count; i++)
            {
                ListViewItem item = lstCasPartDetails.Items[i];
                if (item.SubItems[1].Text == searchText) item.Text = replaceValue;
            }
            for (int i = 0; i < lstTextureDetails.Items.Count; i++)
            {
                ListViewItem item = lstCasPartDetails.Items[i];
                if (item.SubItems[1].Text == searchText) item.Text = replaceValue;

            }
            for (int i = 0; i < lstOtherDetails.Items.Count; i++)
            {
                ListViewItem item = lstOtherDetails.Items[i];
                if (item.SubItems[1].Text == searchText) item.Text = replaceValue;
            }
        }

        private void txtMeshNameCommit_Click(object sender, EventArgs e)
        {
            txtCasPartName.Text = txtMeshName.Text;
            casPartNew.meshName = txtMeshName.Text;
            updateLists("daeFileName", txtMeshName.Text);
            updateLists("Mesh Name", txtMeshName.Text);

            for (int i = 0; i < casPartNew.xmlChunk.Count; i++)
            {
                xmlChunkDetails chunk = (xmlChunkDetails)casPartNew.xmlChunk[i];
                chunk.daeFileName = txtMeshName.Text;
            }

        }

        private void button18_Click(object sender, EventArgs e)
        {
            txtMeshLod1.Text = selectLodFile();
        }

        private void button17_Click(object sender, EventArgs e)
        {
            txtMeshLod2.Text = selectLodFile();
        }

        private void button16_Click(object sender, EventArgs e)
        {
            txtMeshLod3.Text = selectLodFile();
        }

        private string selectLodFile()
        {

            string temp = "";

            openFileDialog1.Filter = "Sims 3 Geometry Mesh|*.simgeom";
            if (openFileDialog1.ShowDialog() == DialogResult.OK && openFileDialog1.FileName.Trim() != "")
            {
                //FileInfo f = new FileInfo(openFileDialog1.FileName);
                temp = openFileDialog1.FileName;
            }

            return temp;
        }

        private string selectTextureFile()
        {

            string temp = "";

            openFileDialog1.Filter = "Sims 3 Texture|*.dds";
            if (openFileDialog1.ShowDialog() == DialogResult.OK && openFileDialog1.FileName.Trim() != "")
            {
                //FileInfo f = new FileInfo(openFileDialog1.FileName);
                temp = openFileDialog1.FileName;
            }

            return temp;
        }

        private void button20_Click(object sender, EventArgs e)
        {
            txtOtherBumpMap.Text = selectTextureFile();
        }

        private void button33_Click(object sender, EventArgs e)
        {
            KeyUtils.findAndShowImage(txtOtherBumpMap.Text);
        }

        private void checkedListType_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void checkedListComplateBlend_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // Uncheck all other boxes
            for (int i = 0; i < checkedListComplateBlend.Items.Count; i++)
            {
                if (i != e.Index)
                {
                    checkedListComplateBlend.SetItemChecked(i, false);
                }
            }
        }

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // Uncheck all other boxes
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                if (i != e.Index)
                {
                    checkedListBox1.SetItemChecked(i, false);
                }
            }
        }

        private void btnDesignTypeCommit_Click(object sender, EventArgs e)
        {
            if (checkedListBox1.GetItemChecked(0) == true)
            {
                updateLists("reskey", "key:0333406C:00000000:52E8BE209C703561");
                updateLists("filename", "CasRgbMask");
                updateLists("Name", "CasRgbMask");
                for (int i = 0; i < casPartNew.xmlChunk.Count; i++)
                {
                    xmlChunkDetails chunk = (xmlChunkDetails)casPartNew.xmlChunk[i];
                    chunk.reskey = "key:0333406C:00000000:52E8BE209C703561";
                    chunk.name = "CasRgbMask";
                    chunk.filename = "CasRgbMask";
                }
            }
            if (checkedListBox1.GetItemChecked(1) == true)
            {
                updateLists("reskey", "key:0333406C:00000000:E37696463F6B2D6E");
                updateLists("filename", "CasRgbaMask");
                updateLists("Name", "CasRgbaMask");

                for (int i = 0; i < casPartNew.xmlChunk.Count; i++)
                {
                    xmlChunkDetails chunk = (xmlChunkDetails)casPartNew.xmlChunk[i];
                    chunk.reskey = "key:0333406C:00000000:E37696463F6B2D6E";
                    chunk.name = "CasRgbaMask";
                    chunk.filename = "CasRgbaMask";
                }
            }
            if (checkedListBox1.GetItemChecked(2) == true)
            {
                updateLists("reskey", "key:0333406C:00000000:01625DDC220C08C6");
                updateLists("filename", "CasSkinOverlayTintable");
                updateLists("Name", "CasSkinOverlayTintable");

                for (int i = 0; i < casPartNew.xmlChunk.Count; i++)
                {
                    xmlChunkDetails chunk = (xmlChunkDetails)casPartNew.xmlChunk[i];
                    chunk.reskey = "key:0333406C:00000000:01625DDC220C08C6";
                    chunk.name = "CasSkinOverlayTintable";
                    chunk.filename = "CasSkinOverlayTintable";
                }
            }

            
        }

        private void grpPatternA_Enter(object sender, EventArgs e)
        {

        }

        private void addNewBlankToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListViewItem item = new ListViewItem();
            item.Text = "Design #" + (listView1.Items.Count + 1).ToString();

            listView1.Items.Add(item);

            // Add a new XML chunk to the casPart
            xmlChunkDetails chunk = new xmlChunkDetails();
            // Copy chunk from casPartSrc index 0
            chunk = (xmlChunkDetails)casPartSrc.xmlChunk[0].Copy();
            chunk.stencil.A.Enabled = "False";
            chunk.stencil.B.Enabled = "False";
            chunk.stencil.C.Enabled = "False";
            chunk.stencil.D.Enabled = "False";
            chunk.stencil.E.Enabled = "False";
            chunk.stencil.F.Enabled = "False";
            casPartNew.xmlChunk.Add(chunk);
            casPartNew.xmlChunkRaw.Add("");

            saveAsToolStripMenuItem.Enabled = true;

            listView1.Items[listView1.Items.Count - 1].Selected = true;
        }

        private void addNewCopyLastToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListViewItem item = new ListViewItem();
            item.Text = "Design #" + (listView1.Items.Count + 1).ToString();

            // Add a new XML chunk to the casPart
            xmlChunkDetails chunk = new xmlChunkDetails();
            // Copy chunk from casPartSrc index 0
            chunk = (xmlChunkDetails)casPartNew.xmlChunk[listView1.Items.Count - 1].Copy();
            casPartNew.xmlChunk.Add(chunk);
            casPartNew.xmlChunkRaw.Add("");

            saveAsToolStripMenuItem.Enabled = true;

            listView1.Items.Add(item);
            listView1.Items[listView1.Items.Count - 1].Selected = true;
        }

        private void copyDefaultsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < casPartSrc.xmlChunk.Count; i++)
            {
                ListViewItem item = new ListViewItem();
                item.Text = "Design #" + (listView1.Items.Count + 1).ToString();

                listView1.Items.Add(item);

                // Add a new XML chunk to the casPart
                xmlChunkDetails chunk = new xmlChunkDetails();
                // Copy chunk from casPartSrc index i
                chunk = (xmlChunkDetails)casPartSrc.xmlChunk[i].Copy();


                casPartNew.xmlChunk.Add(chunk);
                casPartNew.xmlChunkRaw.Add("");
            }
            saveAsToolStripMenuItem.Enabled = true;
            listView1.Items[listView1.Items.Count - 1].Selected = true;
        }

        private void cEnable3DPreview_CheckedChanged(object sender, EventArgs e)
        {

            if (cEnable3DPreview.Checked)
            {
                MadScience.Helpers.saveRegistryValue("show3dRender", "True");
                this.MinimumSize=new Size(1000,650);
                btnStart3D.Visible = true;
                btnReloadTextures.Visible = true;
                btnResetView.Visible = true;
                renderWindow1.Visible = true;
                btnStart3D_Click(null,null);
            }
            else
            {
                //there is a bug when making the window smaller than that. Probably the 3d view doesn't like it to have no width.
                MadScience.Helpers.saveRegistryValue("show3dRender", "False");
                this.MinimumSize = new Size(590,650);
                this.Size = new Size(590, this.Height);
                btnStart3D.Visible = false;
                btnReloadTextures.Visible = false;
                btnResetView.Visible = false;
                renderWindow1.Visible = false;
                renderWindow1.RenderEnabled = false;
            }
        }

        private void btnResetView_Click(object sender, EventArgs e)
        {
            renderWindow1.ResetView();
        }

    }

    /// <remarks/>
    [System.Xml.Serialization.XmlRootAttribute()]
    public class files
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("file", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = true)]
        public filesFile[] Items;
        public ArrayList cTypes = new ArrayList();

        public void makeCTypes()
        {
            if (Items != null && Items.Length > 0)
            {
                foreach (filesFile fEntry in Items)
                {
                    if (cTypes.Contains(fEntry.cType) == false)
                    {
                        cTypes.Add(fEntry.cType);
                    }
                }
            }
        }
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

        private string _casPart;
        [System.Xml.Serialization.XmlTextAttribute()]
        public string casPart
        {
            get { return this._casPart; }
            set
            {
                string temp = value.Replace("game\\caspart\\root\\", "");

                this.fullCasPartname = temp.Replace(".caspart", "");

                string temp2 = temp.Substring(1, 1);
                switch (temp2)
                {
                    case "f":
                        this.cGender = "Female";
                        break;
                    case "m":
                        this.cGender = "Male";
                        break;
                    case "u":
                        this.cGender = "Unisex";
                        break;
                }

                string temp3 = temp.Substring(0, 1);
                switch (temp3)
                {
                    case "a":
                        this.cAge = "Adult";
                        break;
                    case "t":
                        this.cAge = "Teen";
                        break;
                    case "y":
                        this.cAge = "Young Adult";
                        break;
                    case "e":
                        this.cAge = "Elder";
                        break;
                    case "p":
                        this.cAge = "Toddler";
                        break;
                    case "c":
                        this.cAge = "Child";
                        break;
                    case "u":
                        this.cAge = "All Ages";
                        break;
                }

                temp = temp.Remove(0, 2);

                if (temp.StartsWith("Accessory")) { this.cType = "Accessory"; }
                if (temp.StartsWith("Body")) { this.cType = "Body"; }
                if (temp.StartsWith("Bottom")) { this.cType = "Bottom"; }
                if (temp.StartsWith("Costume")) { this.cType = "Costume"; }
                if (temp.StartsWith("Hair")) { this.cType = "Hair"; }
                if (temp.StartsWith("Makeup")) { this.cType = "Makeup"; }
                if (temp.StartsWith("Shoes")) { this.cType = "Shoes"; }
                if (temp.StartsWith("Top")) { this.cType = "Top"; }
                if (temp.StartsWith("Beard")) { this.cType = "Beard"; }

                if (this.cType == null)
                {
                    this.cType = "";
                }
                else
                {
                    temp = temp.Remove(0, this.cType.Length);
                }
                temp = temp.Replace(".caspart", "");

                // Okay so by here we should be left with just the actual cas part name...
                this._casPart = temp;
            }
        }

    }

}
