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
            if (!Directory.Exists(Path.Combine(Application.StartupPath, "cache")))
            {
                Directory.CreateDirectory(Path.Combine(Application.StartupPath, "cache"));
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
                MadScience.Helpers.saveRegistryValue("renderBackgroundColour", MadScience.Colours.convertColour(Color.SlateBlue));
            }

            renderWindow1.BackColor = MadScience.Colours.convertColour(MadScience.Helpers.getRegistryValue("renderBackgroundColour"));

            lookupTypes();

            Helpers.logMessageToFile("Populating types list");
            MadScience.Helpers.lookupTypes(Path.Combine(Application.StartupPath, Path.Combine("xml", "metaTypes.xml")));

            Helpers.logMessageToFile("Finished Initialisation");
        }

        public List<stencilDetails> stencilPool = new List<stencilDetails>(20);

        public files lookupList;
        public casPart casPartNew;
        public casPart casPartSrc = new casPart();

        public bool isNew = false;
        public bool fromPackage = false;

        string logPath = Helpers.logPath(Path.Combine(Application.StartupPath , Application.ProductName + ".log"), true);

        //public string filename;

        //public Hashtable newDDSFiles = new Hashtable();
        //public Hashtable newVPXYFiles = new Hashtable();
        //public Hashtable newGEOMFiles = new Hashtable();
        public Dictionary<int, string> newPNGfiles = new Dictionary<int, string>();

        private void lookupTypes()
        {

            Helpers.logMessageToFile("LookupTypes");

            TextReader r = new StreamReader(Path.Combine(Application.StartupPath, Path.Combine("xml", "casPartList.xml")));
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

            lastSelected = -1;
            listView1.Items.Clear();
            lstTextureDetails.Items.Clear();
            lstOtherDetails.Items.Clear();
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

                    refreshDisplay(0);
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

            if (cmbMeshName.SelectedItem != null)
            {
                if (cmbMeshName.SelectedItem.ToString() != temp.Name)
                {
                    newToolStripMenuItem_Click(null, null);
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
            if (!File.Exists(Path.Combine(Application.StartupPath, Path.Combine("cache", meshName + ".png"))))
            {
                picBox.Image = extractCASThumbnail(meshName);
            }
            else
            {
                Stream picBoxImage = File.OpenRead(Path.Combine(Application.StartupPath, Path.Combine("cache", meshName + ".png")));
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
                if (isNew) cmbMeshName.SelectedIndex = 0;

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
                        tempImage.Save(Path.Combine(Application.StartupPath, Path.Combine("cache", meshName + ".png")), System.Drawing.Imaging.ImageFormat.Png);
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
            renderWindow1.DeInit();
            showMeshDetails();
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
            item.Font = new Font(item.Font.FontFamily, item.Font.Size, FontStyle.Bold);
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

            addListHeader(lstStencilPool, "Stencil " + stencilBoxNo);
            addListItem(lstStencilPool, stencil.key, "Texture", "texture");
            addListItem(lstStencilPool, stencil.Rotation, "Rotation", "");
            addListItem(lstStencilPool, stencil.Tiling, "Tiling", "");
            addListBlank(lstStencilPool);

            //stencil.Enabled = "True";
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
            lastSelected = -1;
            listView1.Items.Clear();
            lstTextureDetails.Items.Clear();
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

            copyDefaultsToolStripMenuItem.Enabled = true;
            renderWindow1.DeInit();
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
            contextMenuStrip1.Show(PointToScreen(new Point(btnAddNewDesign.Left + btnAddNewDesign.Width + 3, (btnAddNewDesign.Top - btnAddNewDesign.Height) + contextMenuStrip1.Height )));

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
            /*
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
                        picLstOtherColour.BackColor = Colours.convertColour(item.SubItems[0].Text);
                        break;
                    default:
                        btnListOtherFind.Enabled = false;
                        btnLstOtherReplace.Enabled = false;
                        picLstOtherColour.Enabled = false;
                        break;
                }
            }
             */
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

                lstTextureDetails.Items.Clear();
                lstOtherDetails.Items.Clear();

                if (listView1.Items.Count == 0)
                {
                    saveAsToolStripMenuItem.Enabled = false;
                }
            }
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
                //chunk.logo.Color = MadScience.Colours.convertColour(picLogoColour.BackColor);
            }
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

        private void btnListOtherFind_Click(object sender, EventArgs e)
        {
            listFindOrReplace(lstOtherDetails, true);
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
                try
                {
                    Helpers.localFiles.Add(newKey, f.FullName);
                }
                catch (Exception ex)
                {
                }
                return newKey;

            }
            return keyString;
        }

        private void btnLstOtherReplace_Click(object sender, EventArgs e)
        {
            listFindOrReplace(lstOtherDetails, false);
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

        private void picLstOtherColour_Click(object sender, EventArgs e)
        {
            if (lstOtherDetails.SelectedItems.Count == 1)
            {

                ColorPicker.ColorPickerDialog cpd = new ColorPicker.ColorPickerDialog();
                cpd.Color = picLstOtherColour.BackColor;
                cpd.ShowDialog();

                picLstOtherColour.BackColor = cpd.Color;

                ListViewItem item = lstOtherDetails.SelectedItems[0];
                
                item.SubItems[0].Text = MadScience.Colours.convertColour(picLstOtherColour.BackColor);
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

                refreshDisplay();
            }
        
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
                        try
                        {
                            this.newPNGfiles.Add(listView1.SelectedIndices[0], txtCustomThumbnailPath.Text);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
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
                Stream cFileOrig = File.OpenRead(Path.Combine(Application.StartupPath, Path.Combine("casparts", temp.fullCasPartname + ".caspart")));
                cFile.Load(cFileOrig);
                cFileOrig.Close();

                if (cFile.cFile.xmlChunkRaw.Count == 0)
                {
                    // Find complate preset XML and load it in
                    String complate = File.ReadAllText(@"P:\Stuart\Desktop\FullBuild0\config\xml\root\" + temp.fullCasPartname + ".xml", System.Text.Encoding.ASCII);
                    cFile.cFile.xmlChunkRaw.Add(complate);
                    cFile.parseRawXML(1);

                    File.Delete(Path.Combine(Application.StartupPath, Path.Combine("casparts", temp.fullCasPartname + ".caspart")));
                    Stream casPartSave = File.OpenWrite(Path.Combine(Application.StartupPath, Path.Combine("casparts", temp.fullCasPartname + ".caspart")));
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
                if (File.Exists(Path.Combine(Application.StartupPath, Path.Combine("casparts", temp.fullCasPartname + ".png"))))
                {
                    File.Delete(Path.Combine(Application.StartupPath, Path.Combine("casparts", temp.fullCasPartname + ".png")));
                }
                Image img = extractCASThumbnail(temp.fullCasPartname);
                if (img != null)
                {
                    try
                    {
                        img.Save(Path.Combine(Application.StartupPath, Path.Combine("casparts", temp.fullCasPartname + ".png")));
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
            /*
            if (lstTextureDetails.SelectedItems.Count == 1)
            {
                ListViewItem item = lstTextureDetails.SelectedItems[0];
                switch (item.Tag.ToString())
                {
                    case "texture":
                        btnListTextureFind.Enabled = true;
                        btnListTextureReplace.Enabled = true;
                        picLstTextureColour.Enabled = false;
                        break;
                    case "color":
                        btnListTextureFind.Enabled = false;
                        btnListTextureReplace.Enabled = false;
                        picLstTextureColour.Enabled = true;
                        picLstTextureColour.BackColor = Colours.convertColour(item.SubItems[0].Text);
                        break;
                    default:
                        btnListTextureFind.Enabled = false;
                        btnListTextureReplace.Enabled = false;
                        picLstTextureColour.Enabled = false;
                        break;
                }
            }
             */
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

                item.SubItems[0].Text = MadScience.Colours.convertColour(picLstTextureColour.BackColor);
                item.Text = item.SubItems[0].Text;
            }

        }

        private void btnCommitLstTexture_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {

                int curDesign = listView1.SelectedItems[0].Index;
                xmlChunkDetails chunk = (xmlChunkDetails)casPartNew.xmlChunk[curDesign];

                updateChunkFromLists(lstTextureDetails, chunk);

                refreshDisplay();
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            listFindOrReplace(lstStencilPool, true);
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
            listFindOrReplace(lstStencilPool, false);
        }

        private void listView3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstStencilPool.SelectedItems.Count == 1)
            {
                ListViewItem item = lstStencilPool.SelectedItems[0];
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
            for (int i = 0; i < lstStencilPool.Items.Count; i++)
            {
                ListViewItem item = lstStencilPool.Items[i];

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

        #region Pattern tab 
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
                /*
                int chunkNo = cmbPatternSelect.SelectedIndex;

                xmlChunkDetails chunk = (xmlChunkDetails)casPartNew.xmlChunk[listView1.SelectedIndices[0]];
                chunk.pattern[chunkNo] = commitPatternDetails();

                if (renderWindow1.RenderEnabled)
                    btnReloadTextures_Click(null, null);
                else
                    btnStart3D_Click(null, null);
                 */
            }
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

        private void picPatternAColor_Click(object sender, EventArgs e)
        {
            picPatternSolidColour.BackColor = showColourDialog(picPatternSolidColour.BackColor);
            commitPatternDetails("Color", Colours.convertColour(picPatternSolidColour.BackColor));

            refreshDisplay();
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


        private void picPatternColourBg_Click(object sender, EventArgs e)
        {
            picPatternColourBg.BackColor = showColourDialog(picPatternColourBg.BackColor);
            picPatternColourBg.Tag = "color";
            commitPatternDetails("ColorP0", Colours.convertColour(picPatternColourBg.BackColor));
            makePatternPreviewThumb();
        }

        private void picPatternColour1_Click(object sender, EventArgs e)
        {
            picPatternColour1.BackColor = showColourDialog(picPatternColour1.BackColor);
            picPatternColour1.Tag = "color";
            commitPatternDetails("ColorP1", Colours.convertColour(picPatternColour1.BackColor));
            makePatternPreviewThumb();
        }

        private void picPatternColour2_Click(object sender, EventArgs e)
        {
            picPatternColour2.BackColor = showColourDialog(picPatternColour2.BackColor);
            picPatternColour2.Tag = "color";
            commitPatternDetails("ColorP2", Colours.convertColour(picPatternColour2.BackColor));
            makePatternPreviewThumb();
        }

        private void picPatternColour3_Click(object sender, EventArgs e)
        {
            picPatternColour3.BackColor = showColourDialog(picPatternColour3.BackColor);
            picPatternColour3.Tag = "color";
            commitPatternDetails("ColorP3", Colours.convertColour(picPatternColour3.BackColor));
            makePatternPreviewThumb();
        }

        private void picPatternColour4_Click(object sender, EventArgs e)
        {
            picPatternColour4.BackColor = showColourDialog(picPatternColour4.BackColor);
            picPatternColour4.Tag = "color";
            commitPatternDetails("ColorP4", Colours.convertColour(picPatternColour4.BackColor));
            makePatternPreviewThumb();
        }

        private void cmbPatternSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                showPatternDetails(listView1.SelectedIndices[0]);
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

                    int patternNo = cmbPatternSelect.SelectedIndex;
                    int channelNo = cmbChannelSelect.SelectedIndex;
                    xmlChunkDetails chunk = (xmlChunkDetails)casPartNew.xmlChunk[listView1.SelectedIndices[0]];
                    if (chunk.pattern[patternNo].type != "solidColor")
                    {

                        // Automatically set the details to solidColour1
                        txtPatternAKey.Text = "key:0333406C:00000000:71D5EFB6C391BC17";
                        txtPatternAName.Text = "solidColor_1";
                        txtPatternARGBMask.Text = "";
                        txtPatternASpecular.Text = "";
                        txtPatternATiling.Text = "4.0000,4.0000";
                        txtPatternAFilename.Text = @"Materials\Miscellaneous\solidColor_1";
                    }
                    picPatternThumb.Visible = false;

                    commitPatternDetails("type", "solidColor");

                    // Get the first colour of the previous pattern
                    picPatternSolidColour.BackColor = picPatternColour1.BackColor;
                    commitPatternDetails("Color", Colours.convertColour(picPatternSolidColour.BackColor));

                    refreshDisplay();

                    break;
                case 1: // Coloured
                    groupBox3.Visible = false;
                    groupBox5.Visible = false;
                    groupBox7.Visible = true;
                    picPatternThumb.Visible = true;
                    commitPatternDetails("type", "Coloured");
                    break;
                case 2: // HSV
                    groupBox3.Visible = false;
                    groupBox5.Visible = true;
                    groupBox7.Visible = false;
                    cmbChannelSelect.SelectedIndex = 0;
                    picPatternThumb.Visible = true;
                    commitPatternDetails("type", "HSV");
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
                viewPatternChannelInfo(chunk.pattern[patternNo]);
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

        private void button11_Click(object sender, EventArgs e)
        {

            //pBrowser.curCategory = this.patternBrowserCategory;
            if (pBrowser.ShowDialog() == DialogResult.OK)
            {
                showPatternDetails(pBrowser.selectedPattern, false);
                // Commit pattern here
                if (listView1.SelectedItems.Count == 1)
                {

                    int patternNo = cmbPatternSelect.SelectedIndex;
                    xmlChunkDetails chunk = (xmlChunkDetails)casPartNew.xmlChunk[listView1.SelectedIndices[0]];
                    chunk.pattern[patternNo] = (patternDetails)pBrowser.selectedPattern.Copy();

                    refreshDisplay();

                }
            }

            pBrowser.Hide();
        }
        #endregion

        private void refreshDisplay()
        {
            // Default to only refresh textures
            refreshDisplay(1);
        }

        private void refreshDisplay(int refreshType)
        {
            if (renderWindow1.RenderEnabled && refreshType == 1)
            {
                if (listView1.SelectedItems.Count == 1 && cEnable3DPreview.Checked == true)
        {

                    xmlChunkDetails details = (xmlChunkDetails)casPartNew.xmlChunk[listView1.SelectedIndices[0]];

                    renderWindow1.RenderEnabled = false;

                    DateTime startTime = DateTime.Now;
                    toolStripStatusLabel1.Text = "Reloading 3d view... please wait...";
                    statusStrip1.Refresh();

                    reloadTextures(details);

                    DateTime stopTime = DateTime.Now;
                    TimeSpan duration = stopTime - startTime;

                    toolStripStatusLabel1.Text = "Reloaded 3D in " + duration.Milliseconds + "ms";

                    renderWindow1.RenderEnabled = true;
                }

                
            }
            else
            {
            if (listView1.SelectedItems.Count == 1 && cEnable3DPreview.Checked == true)
            {
                xmlChunkDetails details = (xmlChunkDetails)casPartNew.xmlChunk[listView1.SelectedIndices[0]];

                toolStripStatusLabel1.Text = "Initialising 3d view... please wait...";
                statusStrip1.Refresh();

                DateTime startTime = DateTime.Now;

                startRender(details);

                DateTime stopTime = DateTime.Now;
                TimeSpan duration = stopTime - startTime;
                this.toolStripStatusLabel1.Text = "Loaded 3D in " + duration.Milliseconds + "ms";


            }

        }
        }

        private void btnReloadTextures_Click(object sender, EventArgs e)
        {
            refreshDisplay(1);
        }

        private void btnStart3D_Click(object sender, EventArgs e)
        {
            refreshDisplay(0);
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

        private void btnDesignTypeCommit_Click(object sender, EventArgs e)
        {
            if (chkDesignType.GetItemChecked(0) == true)
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
            if (chkDesignType.GetItemChecked(1) == true)
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
            if (chkDesignType.GetItemChecked(2) == true)
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

        private void addNewBlankToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListViewItem item = new ListViewItem();
            item.Text = "Design #" + (listView1.Items.Count + 1).ToString();

            if (listView1.Items.Count == 0) lastSelected = -1;

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
            
            refreshDisplay();

        }

        private void addNewCopyLastToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListViewItem item = new ListViewItem();
            item.Text = "Design #" + (listView1.Items.Count + 1).ToString();

            if (listView1.Items.Count == 0) lastSelected = -1;

            // Add a new XML chunk to the casPart
            xmlChunkDetails chunk = new xmlChunkDetails();
            // Copy chunk from casPartSrc index 0
            chunk = (xmlChunkDetails)casPartNew.xmlChunk[listView1.Items.Count - 1].Copy();
            casPartNew.xmlChunk.Add(chunk);
            casPartNew.xmlChunkRaw.Add("");

            saveAsToolStripMenuItem.Enabled = true;

            listView1.Items.Add(item);
            listView1.Items[listView1.Items.Count - 1].Selected = true;

            refreshDisplay();
        }

        private void copyDefaultsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < casPartSrc.xmlChunk.Count; i++)
            {
                ListViewItem item = new ListViewItem();
                item.Text = "Design #" + (listView1.Items.Count + 1).ToString();

                if (listView1.Items.Count == 0) lastSelected = -1;

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

            refreshDisplay();
        }

        private void cEnable3DPreview_CheckedChanged(object sender, EventArgs e)
        {

            if (cEnable3DPreview.Checked)
            {
                MadScience.Helpers.saveRegistryValue("show3dRender", "True");
                this.MinimumSize = new Size(1000, 650);
                btnStart3D.Visible = true;
                btnReloadTextures.Visible = true;
                btnResetView.Visible = true;
                renderWindow1.Visible = true;

                refreshDisplay(0);
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
                renderWindow1.DeInit();
            }
        }

        private void btnResetView_Click(object sender, EventArgs e)
        {
            renderWindow1.ResetView();
        }

        private void cmbMeshName_SelectionChangeCommitted(object sender, EventArgs e)
        {
            newToolStripMenuItem_Click(null, null);
        }

        private void lstMeshTGILinks_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstMeshTGILinks.SelectedItems.Count == 1)
            {
                ListViewItem item = lstMeshTGILinks.SelectedItems[0];
                switch (item.Tag.ToString())
                {
                    case "texture":
                        btnMeshTGILinksFind.Enabled = true;
                        break;
                    default:
                        btnMeshTGILinksFind.Enabled = false;
                        break;
                }
            }
        }

        private void btnMeshTGILinksFind_Click(object sender, EventArgs e)
        {
            listFindOrReplace(lstMeshTGILinks, true);
        }

        ListView curListView;
        private void lstTextureDetails_MouseClick(object sender, MouseEventArgs e)
        {
            ListView lView = (ListView)sender;
            if (lView.SelectedItems.Count == 1)
            {
                ListViewItem item = lView.SelectedItems[0];
                switch (item.Tag.ToString())
                {
                    case "texture":
                        if (!String.IsNullOrEmpty(item.Text)) btnListTextureFind.Enabled = true;
                        btnListTextureReplace.Enabled = true;
                        picLstTextureColour.Enabled = false;
                        label14.Enabled = false;
                        editToolStripMenuItem.Enabled = true;
                        editColourToolStripMenuItem.Enabled = false;
                        findImageToolStripMenuItem.Enabled = true;
                        replaceImageToolStripMenuItem.Enabled = true;
                        break;
                    case "color":
                        btnListTextureFind.Enabled = false;
                        btnListTextureReplace.Enabled = false;
                        picLstTextureColour.Enabled = true;
                        label14.Enabled = true;
                        picLstTextureColour.BackColor = Colours.convertColour(item.SubItems[0].Text);
                        editToolStripMenuItem.Enabled = true;
                        editColourToolStripMenuItem.Enabled = true;
                        findImageToolStripMenuItem.Enabled = false;
                        replaceImageToolStripMenuItem.Enabled = false;
                        break;
                    default:
                        btnListTextureFind.Enabled = false;
                        btnListTextureReplace.Enabled = false;
                        picLstTextureColour.Enabled = false;
                        label14.Enabled = false;
                        editToolStripMenuItem.Enabled = true;
                        editColourToolStripMenuItem.Enabled = false;
                        findImageToolStripMenuItem.Enabled = false;
                        replaceImageToolStripMenuItem.Enabled = false;
                        break;
                }

                if (e.Button == MouseButtons.Right)
                {
                    curListView = lView;
                    contextMenuStrip2.Show(lView, new Point(e.X, e.Y));
                }
            }

        }

        private void lstTextureDetails_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListView lView = (ListView)sender;
            if (e.Button == MouseButtons.Left && lView.SelectedItems.Count == 1)
            {
                ListViewItem item = lView.SelectedItems[0];
                switch (item.Tag.ToString())
                {
                    case "truefalse":
                        if (item.Text.ToLower() == "true") item.Text = "False";
                        else item.Text = "True";
                        item.Font = new Font(item.Font.FontFamily, item.Font.Size, FontStyle.Bold); 
                        break;
                    case "texture":
                        if (!String.IsNullOrEmpty(item.Text)) btnListTextureFind_Click(sender, null);
                        break;
                    case "color":
                        if (!String.IsNullOrEmpty(item.Text)) picListTextureColour_Click(sender, null);
                        break;
                }
            }
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            doContextMenuClick("edit");
        }

        private void doContextMenuClick(string clickType)
        {
            if (curListView != null)
            {
                if (curListView.SelectedItems.Count == 1)
                {
                    ListViewItem item = curListView.SelectedItems[0];
                    if (curListView.Name == "lstTextureDetails")
                    {
                        switch (clickType)
                        {
                            case "edit":
                                item.BeginEdit();
                                break;
                            case "find":
                                btnListTextureFind_Click(null, null);
                                break;
                            case "replace":
                                btnListTextureReplace_Click(null, null);
                                break;
                            case "colour":
                                picListTextureColour_Click(null, null);
                                break;

                        }
                    }
                    if (curListView.Name == "lstOtherDetails")
                    {
                        switch (clickType)
                        {
                            case "edit":
                                item.BeginEdit();
                                break;
                            case "find":
                                btnListOtherFind_Click(null, null);
                                break;
                            case "replace":
                                btnLstOtherReplace_Click(null, null);
                                break;
                            case "colour":
                                picLstOtherColour_Click(null, null);
                                break;

                        }
                    }
                }
            }
        }

        private void findImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            doContextMenuClick("find");
        }

        private void replaceImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            doContextMenuClick("replace");
        }

        private void editColourToolStripMenuItem_Click(object sender, EventArgs e)
        {
            doContextMenuClick("colour");
        }

        private void lstOtherDetails_MouseClick(object sender, MouseEventArgs e)
        {
            ListView lView = (ListView)sender;
            if (lView.SelectedItems.Count == 1)
            {
                ListViewItem item = lView.SelectedItems[0];
                switch (item.Tag.ToString())
                {
                    case "texture":
                        if (!String.IsNullOrEmpty(item.Text)) btnListOtherFind.Enabled = true;
                        btnLstOtherReplace.Enabled = true;
                        picLstOtherColour.Enabled = false;

                        editToolStripMenuItem.Enabled = true;
                        editColourToolStripMenuItem.Enabled = false;
                        findImageToolStripMenuItem.Enabled = true;
                        replaceImageToolStripMenuItem.Enabled = true;
                        break;
                    case "color":
                        btnListOtherFind.Enabled = false;
                        btnLstOtherReplace.Enabled = false;
                        picLstOtherColour.Enabled = true;
                        picLstOtherColour.BackColor = Colours.convertColour(item.SubItems[0].Text);

                        editToolStripMenuItem.Enabled = true;
                        editColourToolStripMenuItem.Enabled = true;
                        findImageToolStripMenuItem.Enabled = false;
                        replaceImageToolStripMenuItem.Enabled = false;
                        break;
                    default:
                        btnListOtherFind.Enabled = false;
                        btnLstOtherReplace.Enabled = false;
                        picLstOtherColour.Enabled = false;

                        editToolStripMenuItem.Enabled = true;
                        editColourToolStripMenuItem.Enabled = false;
                        findImageToolStripMenuItem.Enabled = false;
                        replaceImageToolStripMenuItem.Enabled = false;
                        break;
                }

                if (e.Button == MouseButtons.Right)
                {
                    curListView = lView;
                    contextMenuStrip2.Show(lView, new Point(e.X, e.Y));
                }
            }

        }

        private void lstOtherDetails_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListView lView = (ListView)sender;
            if (e.Button == MouseButtons.Left && lView.SelectedItems.Count == 1)
            {
                ListViewItem item = lView.SelectedItems[0];
                switch (item.Tag.ToString())
                {
                    case "truefalse":
                        if (item.Text.ToLower() == "true") item.Text = "False";
                        else item.Text = "True";
                        item.Font = new Font(item.Font.FontFamily, item.Font.Size, FontStyle.Bold);
                        break;
                    case "texture":
                        if (!String.IsNullOrEmpty(item.Text)) btnListOtherFind_Click(sender, null);
                        break;
                    case "color":
                        if (!String.IsNullOrEmpty(item.Text)) picLstOtherColour_Click(sender, null);
                        break;
                }
            }
        }

        private void btnDebugHSVRefresh_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                makePatternPreviewThumb();
                //PatternBrowser.HSVColor basehsv = new PatternBrowser.HSVColor();
                //basehsv.Hue = double.Parse(p.BaseHBg) * 360;
                //basehsv.Saturation = double.Parse(p.BaseSBg);
                //basehsv.Value = double.Parse(p.BaseVBg);
                //PatternBrowser.HSVColor hsv = new PatternBrowser.HSVColor();
                //hsv.Hue = double.Parse(p.HBg)*360;
                //hsv.Saturation = double.Parse(p.SBg);
                //hsv.Value = double.Parse(p.VBg);
                //picHSVColorBG.BackColor = (hsv+basehsv).Color;
            }
        }

        private void picHSVColorBG_Click(object sender, EventArgs e)
        {
            picHSVColorBG.BackColor = showColourDialog(picHSVColorBG.BackColor);

            Colours.HSVColor hsv = new Colours.HSVColor(picHSVColorBG.BackColor);
            hsv.Hue -= double.Parse(txtPatternABaseHBg.Text) * 360;
            hsv.Saturation -= double.Parse(txtPatternABaseSBg.Text);
            hsv.Value -= double.Parse(txtPatternABaseVBg.Text);
            
            txtPatternAHBg.Text = MadScience.Helpers.numberToString(hsv.Hue / 360);
            txtPatternASBg.Text = MadScience.Helpers.numberToString(hsv.Saturation);
            txtPatternAVBg.Text = MadScience.Helpers.numberToString(hsv.Value);

            makePatternPreviewThumb();
        }

        private void Form1_ResizeBegin(object sender, EventArgs e)
        {
            Console.WriteLine("Resize Begin");
        }

        private void chkPatternAEnabled_CheckedChanged(object sender, EventArgs e)
        {
            commitPatternDetails("enabled", chkPatternAEnabled.Checked);
        }

        private void chkPatternALinked_CheckedChanged(object sender, EventArgs e)
        {
            commitPatternDetails("linked", chkPatternALinked.Checked);
        }

        private void txtPatternAKey_TextChanged(object sender, EventArgs e)
        {
            commitPatternDetails("key", txtPatternAKey.Text);
        }

        private void txtPatternAName_TextChanged(object sender, EventArgs e)
        {
            commitPatternDetails("name", txtPatternAName.Text);
        }

        private void txtPatternAFilename_TextChanged(object sender, EventArgs e)
        {
            commitPatternDetails("filename", txtPatternAFilename.Text);
        }

        private void txtPatternATiling_TextChanged(object sender, EventArgs e)
        {
            commitPatternDetails("tiling", txtPatternATiling.Text);
        }

        private void txtPatternARGBMask_TextChanged(object sender, EventArgs e)
        {
            commitPatternDetails("rgbmask", txtPatternARGBMask.Text);
        }

        private void txtPatternASpecular_TextChanged(object sender, EventArgs e)
        {
            commitPatternDetails("specular", txtPatternASpecular.Text);
        }

        private void txtPatternBGImage_TextChanged(object sender, EventArgs e)
        {
            commitPatternDetails("bgimage", txtPatternBGImage.Text);
        }

        private void renderWindow1_RequireNewTextures(object sender, EventArgs e)
        {
            reloadTextures();
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
