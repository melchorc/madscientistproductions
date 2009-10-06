using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Gibbed.Sims3.FileFormats;
using System.Xml;
using System.Xml.Serialization;
using Gibbed.Helpers;
using System.Runtime.InteropServices;

namespace qadpe
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            textBox4.Visible = false;
            panel1.Visible = false;

            chkShowAlpha.Visible = false;
            chkShowBlue.Visible = false;
            chkShowGreen.Visible = false;
            chkShowRed.Visible = false;
        }

        private DdsFileTypePlugin.DdsFile ddsFile = new DdsFileTypePlugin.DdsFile();
        private bool lockImage = false;

        public metaEntry.typesToMeta lookupList;
        private ArrayList indexEntries = new ArrayList();
        private FileInfo currentFile;

        private void lookupTypes()
        {
            TextReader r = new StreamReader(Application.StartupPath + "\\metaTypes.xml");
            XmlSerializer s = new XmlSerializer(typeof(metaEntry.typesToMeta));
            this.lookupList = (metaEntry.typesToMeta)s.Deserialize(r);
            r.Close();

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Sims 3 Package|*.package";
            openFileDialog1.ShowDialog();
            if (openFileDialog1.FileName != "")
            {
                loadFile(openFileDialog1.FileName);
            }
        }

        private void loadFile(string filename)
        {
            toolStripStatusLabel1.Text = filename;
            Stream input = File.OpenRead(filename);

            DatabasePackedFile dbpf = new DatabasePackedFile();
            try
            {
                dbpf.Read(input);
            }
            catch (NotAPackageException)
            {
                MessageBox.Show("bad file: {0}", filename);
                input.Close();
                return;
            }

            this.currentFile = new FileInfo(filename);

            input.Seek(0, SeekOrigin.Begin);

            lookupTypes();
            indexEntries.Clear();
            listView1.Items.Clear();

            txtPkgIndexType.Text = dbpf.IndexType.ToString();
            txtPkgNumChunks.Text = dbpf.Entries.Count.ToString();

            for (int i = 0; i < dbpf.Entries.Count; i++)
            {
                ListViewItem item = new ListViewItem();
                DatabasePackedFile.Entry entry = dbpf.Entries[i];
                item.Text = this.lookupList.lookup(entry.Key.TypeId).shortName;
                item.SubItems.Add(entry.Key.TypeId.ToString("X8"));
                item.SubItems.Add(entry.Key.GroupId.ToString("X8"));
                item.SubItems.Add(entry.Key.InstanceId.ToString("X16"));
                item.SubItems.Add(entry.DecompressedSize.ToString());
                item.SubItems.Add(i.ToString());
                listView1.Items.Add(item);
                indexEntries.Add(entry);

            }

            input.Close();

        }

        private bool isLocalFile(ResourceKey key)
        {
            /*
            Stream input = File.Open(this.currentFile.FullName);

            input.Seek(0, SeekOrigin.Begin);
            DatabasePackedFile dbpf = new DatabasePackedFile();
            try
            {
                dbpf.Read(input);
            }
            catch (Gibbed.Sims3.FileFormats.NotAPackageException)
            {
                MessageBox.Show("bad file: {0}");
                input.Close();
                return false;
            }

            for (int i = 0; i < dbpf.Entries.Count; i++)
            {
                DatabasePackedFile.Entry entry = dbpf.Entries[i];
                if (entry.Key.InstanceId == key.InstanceId && entry.Key.GroupId == key.GroupId && entry.Key.TypeId == key.TypeId)
                {
                    return true;
                }
            }
            input.Close();
            */
            return false;
        }

        private void addListItem(string text, string subitem)
        {
            ListViewItem item = new ListViewItem();
            item.Text = text;
            item.SubItems.Add(subitem);
            listView2.Items.Add(item);

        }

        private void showBlendGeomFile(Stream input)
        {
            BinaryReader reader = new BinaryReader(input);

            string magic = StreamHelpers.ReadStringASCII(input, 4);
            uint version = reader.ReadUInt32();

            uint s1count = reader.ReadUInt32();
            uint s1subcount = reader.ReadUInt32();
            uint s2count = reader.ReadUInt32();
            uint s3count = reader.ReadUInt32();

            addListItem("Section 1 Count", s1count.ToString());
            addListItem("Section 2 Count", s2count.ToString());
            addListItem("Section 3 Count", s3count.ToString());

            uint s1presubentrysize = reader.ReadUInt32();
            uint s1subentryentrysize = reader.ReadUInt32();

            uint s1offset = reader.ReadUInt32();
            uint s2offset = reader.ReadUInt32();
            uint s3offset = reader.ReadUInt32();

            reader.Close();
        }

        private List<uint> showRcolHeader(Stream input)
        {
            List<uint> chunkPositions = new List<uint>();

            addListItem("Start RCOL header", "");
            input.ReadValueU32();
            input.ReadValueU32();
            uint rcolIndex3 = input.ReadValueU32();
            uint rcolIndex1 = input.ReadValueU32();
            uint rcolIndex2 = input.ReadValueU32();
            for (int i = 0; i < rcolIndex2; i++)
            {
                ulong instanceId = input.ReadValueU64();
                uint typeId = input.ReadValueU32();
                uint groupId = input.ReadValueU32();

                addListItem("Internal IGT #" + (i + 1).ToString(), "key:" + typeId.ToString("X8") + ":" + groupId.ToString("X8") + ":" + instanceId.ToString("X16"));                

            }

            for (int i = 0; i < rcolIndex1; i++)
            {
                ulong instanceId = input.ReadValueU64();
                uint typeId = input.ReadValueU32();
                uint groupId = input.ReadValueU32();
                addListItem("External IGT #" + (i + 1).ToString(), "key:" + typeId.ToString("X8") + ":" + groupId.ToString("X8") + ":" + instanceId.ToString("X16"));

            }

            for (int i = 0; i < rcolIndex2; i++)
            {
                uint chunkPos = input.ReadValueU32();
                uint chunkSize = input.ReadValueU32();
                addListItem("Chunk " + (i + 1).ToString(), "Pos: " + chunkPos.ToString() + " Size: " + chunkSize.ToString());
                chunkPositions.Add(chunkPos);
            }
            addListItem("End RCOL header", "");

            return chunkPositions;
        }

        private void readIbufChunk(Stream input)
        {
            BinaryReader reader = new BinaryReader(input);

            string magic = StreamHelpers.ReadStringASCII(input, 4);
            addListItem("Magic", magic);

        }

        private void readMlodChunk(Stream input)
        {
            BinaryReader reader = new BinaryReader(input);

            string magic = StreamHelpers.ReadStringASCII(input, 4);
            addListItem("Magic", magic);
            uint version = reader.ReadUInt32();

            uint count = reader.ReadUInt32();
            addListItem("Count", count.ToString());

            for (int i = 0; i < count; i++)
            {
                uint subsetBytes = reader.ReadUInt32();
                uint unk1 = reader.ReadUInt32();
                uint matd = reader.ReadUInt32();
                uint vrtf = reader.ReadUInt32();
                uint vbuf = reader.ReadUInt32();
                uint ibuf = reader.ReadUInt32();
                uint vbuftype = reader.ReadUInt32();
                ulong vbufoffset = reader.ReadUInt64();
                ulong ibufoffset = reader.ReadUInt64();
                uint vbufcount = reader.ReadUInt32();
                uint ibufcount = reader.ReadUInt32();

                addListItem("Vertex count", vbufcount.ToString());
                addListItem("Polygon count", ibufcount.ToString());

                addListItem("Bounding Box", reader.ReadSingle() + " " + reader.ReadSingle() + " " + reader.ReadSingle() + " " + reader.ReadSingle() + " " + reader.ReadSingle() + " " + reader.ReadSingle());
                
                uint skin = reader.ReadUInt32();
                uint unk2 = reader.ReadUInt32();
                uint unk3 = reader.ReadUInt32();
                uint matd2 = reader.ReadUInt32();

                reader.ReadSingle();
                reader.ReadSingle();
                reader.ReadSingle();
                reader.ReadSingle();
                reader.ReadSingle();
                reader.ReadSingle();
            }
        }

        private void readModlChunk(Stream input)
        {
            BinaryReader reader = new BinaryReader(input);

            string magic = StreamHelpers.ReadStringASCII(input, 4);
            addListItem("Magic", magic);
            uint version = reader.ReadUInt32();

            uint count = reader.ReadUInt32();
            addListItem("Count", count.ToString());

            addListItem("Bounding Box", reader.ReadSingle() + " " + reader.ReadSingle() + " " + reader.ReadSingle() + " " + reader.ReadSingle() + " " + reader.ReadSingle() + " " + reader.ReadSingle());

            if (version >= 258)
            {
                uint unk1 = reader.ReadUInt32();
                uint unk2 = reader.ReadUInt32();
                uint unk3 = reader.ReadUInt32();
                addListItem("Unknown1", unk1.ToString());
                addListItem("Unknown2", unk2.ToString());
                addListItem("Unknown3", unk3.ToString());
            }

            for (int i = 0; i < count; i++)
            {
                uint unk1 = reader.ReadUInt32();
                uint unk2 = reader.ReadUInt32();
                uint unk3 = reader.ReadUInt32();
                float unk4 = reader.ReadSingle();
                float unk5 = reader.ReadSingle();
                addListItem("Unknown #" + i.ToString(), unk1 + " " + unk2 + " " + unk3 + " " + unk4 + " " + unk5);
            }

        }

        private void showModlFile(Stream input)
        {
            BinaryReader reader = new BinaryReader(input);
            List<uint> chunkPositions = showRcolHeader(input);

            for (int i = 0; i < chunkPositions.Count; i++)
            {
                // Seek to position and read magic
                input.Seek(chunkPositions[i], SeekOrigin.Begin);
                string magic = StreamHelpers.ReadStringASCII(input, 4);
                // Seek backwards 4 bytes
                input.Seek(chunkPositions[i], SeekOrigin.Begin);

                switch (magic.ToUpper())
                {
                    case "MODL":
                        readModlChunk(input);
                        break;
                    case "MLOD":
                        readMlodChunk(input);
                        break;
                    case "IBUF":
                        //readIbufChunk(input);
                        break;
                }
            }

            reader.Close();
        }

        private void showFacePartFile(Stream input)
        {
            BinaryReader reader = new BinaryReader(input);
            reader.ReadUInt32();
            uint tgiOffset = reader.ReadUInt32();
            reader.ReadUInt32();

            byte nameLength = reader.ReadByte();
            string partName = Gibbed.Helpers.StreamHelpers.ReadStringUTF16(input, false, (uint)nameLength);

            addListItem("Part Name", partName);

            reader.ReadUInt32();

            ResourceKey blendTgi = input.ReadResourceKeyTGI();

            addListItem("Blend Geometry", "key:" + blendTgi.ToString());

            uint geomCount = reader.ReadUInt32();
            addListItem("Geom/Bone Count", geomCount.ToString());

            for (int i = 0; i < geomCount; i++)
            {
                addListItem("Entry " + (i + 1).ToString(), " ");
                addListItem("  Facial Region", reader.ReadUInt32().ToString());
                uint geomAndBone = reader.ReadUInt32();
                addListItem("  Geom and Bone present?", geomAndBone.ToString());
                if (geomAndBone == 0)
                {
                    addListItem("  Has GeomEntry", reader.ReadUInt32().ToString());
                }
                addListItem("  Age/Gender", reader.ReadUInt32().ToString());
                addListItem("  Amount", reader.ReadSingle().ToString());
                //if (geomAndBone == 0)
                //{
                    addListItem("  GEOM Entry Index", reader.ReadUInt32().ToString());
                //}
                if (geomAndBone == 1)
                {
                    uint hasBoneEntry = reader.ReadUInt32();
                    addListItem("  Has BoneEntry", hasBoneEntry.ToString());
                    if (hasBoneEntry == 1)
                    {
                        addListItem("  Age/Gender2", reader.ReadUInt32().ToString());
                        addListItem("  Amount2", reader.ReadSingle().ToString());
                    }
                    addListItem("  Bone Index", reader.ReadUInt32().ToString());
                }
            }

            uint numTGIs = reader.ReadUInt32();
            addListItem("Num IGTs", numTGIs.ToString());
            for (int i = 0; i < numTGIs; i++)
            {
                uint typeId = reader.ReadUInt32();
                uint groupId = reader.ReadUInt32();
                ulong instanceId = reader.ReadUInt64();

                addListItem("IGT #" + (i + 1).ToString(), "key:" + typeId.ToString("X8") + ":" + groupId.ToString("X8") + ":" + instanceId.ToString("X16"));                
            }

            reader.Close();
        }

        private void showCaspFile(Stream input)
        {
            MadScience.casPartFile cFile = new MadScience.casPartFile();
            MadScience.casPart casPart = cFile.Load(input);
            textBox4.Text = (string)casPart.xmlChunkRaw[0];

        }

        private void showFacialBlendFile(Stream input)
        {
            BinaryReader fblendReader = new BinaryReader(input);


            fblendReader.ReadUInt32();
            uint fblendTgiOffset = fblendReader.ReadUInt32();
            fblendReader.ReadUInt32();
            fblendReader.ReadUInt64();
            uint indexersCount = fblendReader.ReadUInt32();

            for (int i = 0; i < indexersCount; i++)
            {
                fblendReader.ReadUInt32();
            }

            addListItem("BiDirectional", fblendReader.ReadByte().ToString());
            uint casPanelGroup = fblendReader.ReadUInt32();
            string casPanelName = "";
            switch (casPanelGroup)
            {
                case 2:
                    casPanelName = "Head and Ears";
                    break;
                case 8:
                    casPanelName = "Mouth";
                    break;
                case 16:
                    casPanelName = "Nose";
                    break;
                case 64:
                    casPanelName = "Eyelash";
                    break;
                case 128:
                    casPanelName = "Eyes";
                    break;
            }
            addListItem("Cas Panel Group", casPanelGroup.ToString() + " " + casPanelName);
            addListItem("Sort Index", fblendReader.ReadUInt32().ToString());
            fblendReader.ReadUInt32();

            uint tgiCount = fblendReader.ReadUInt32();
            for (int i = 0; i < tgiCount; i++)
            {
                uint typeId = fblendReader.ReadUInt32();
                uint groupId = fblendReader.ReadUInt32();
                ulong instanceId = fblendReader.ReadUInt64();

                addListItem("TGI #" + (i + 1).ToString(), "key:" + typeId.ToString("X8") + ":" + groupId.ToString("X8") + ":" + instanceId.ToString("X16"));
            }

            fblendReader.Close();

        }

        private void showTextureCompositorFile(Stream txtcStream)
        {
            BinaryReader txtcReader = new BinaryReader(txtcStream);
            txtcStream.Seek(4, SeekOrigin.Begin);
            uint txtcTGIOffset = txtcReader.ReadUInt32();
            txtcStream.Seek(txtcTGIOffset, SeekOrigin.Current);

            int txtcNumIGTs = (int)txtcReader.ReadByte();
            for (int p = 0; p < txtcNumIGTs; p++)
            {
                ulong txtcInstanceId = txtcReader.ReadUInt64();
                uint txtcGroupId = txtcReader.ReadUInt32();
                uint txtcTypeId = txtcReader.ReadUInt32();

                ListViewItem txtcItem = new ListViewItem();
                txtcItem.Text = "IGT #" + (p + 1).ToString();
                txtcItem.SubItems.Add(txtcTypeId.ToString("X8") + ":" + txtcGroupId.ToString("X8") + ":" + txtcInstanceId.ToString("X16"));
                listView2.Items.Add(txtcItem);
            }

        }

        private void showVpxyFile(Stream vpxyStream)
        {
            showRcolHeader(vpxyStream);

            string vpxyString = vpxyStream.ReadStringASCII(4);
            uint vpxyVersion = vpxyStream.ReadValueU32();

            uint tailOffset = vpxyStream.ReadValueU32();
            uint tailSize = vpxyStream.ReadValueU32();

            vpxyStream.Seek(tailOffset - 4, SeekOrigin.Current);

            uint numTGIs = vpxyStream.ReadValueU32();

            for (int i = 0; i < numTGIs; i++)
            {
                ListViewItem loditem = new ListViewItem();

                loditem.Text = "TGI #" + i.ToString();

                uint lodTypeId = vpxyStream.ReadValueU32();
                uint lodGroupId = vpxyStream.ReadValueU32();
                ulong lodInstanceId = vpxyStream.ReadValueU64();

                loditem.SubItems.Add("key:" + lodTypeId.ToString("X8") + ":" + lodGroupId.ToString("X8") + ":" + lodInstanceId.ToString("X16"));

                if (isLocalFile(new ResourceKey(lodInstanceId, lodTypeId, lodGroupId)))
                {
                    loditem.SubItems.Add("*");
                }

                listView2.Items.Add(loditem);

            }

        }

        private void showGeomFile(Stream geominput)
        {
            BinaryReader reader = new BinaryReader(geominput);

            uint rcolVersion = reader.ReadUInt32();
            uint rcolDatatype = reader.ReadUInt32();

            uint grcolIndex3 = reader.ReadUInt32();
            uint grcolIndex1 = reader.ReadUInt32();
            uint grcolIndex2 = reader.ReadUInt32();

            for (int i = 0; i < grcolIndex2; i++)
            {
                ulong instanceId = reader.ReadUInt64();
                uint typeId = reader.ReadUInt32();
                uint groupId = reader.ReadUInt32();
            }

            for (int i = 0; i < grcolIndex2; i++)
            {
                uint chunkOffset = reader.ReadUInt32();
                uint chunkSize = reader.ReadUInt32();
            }

            // Real GEOM chunk
            string geomString = StreamHelpers.ReadStringASCII(geominput, 4);

            uint geomVersion = reader.ReadUInt32();

            uint gtailOffset = reader.ReadUInt32();

            long seekFrom = geominput.Position;
            //uint tailSize = reader.ReadUInt32();
            //writer.Write(tailSize);

            geominput.Seek(gtailOffset + seekFrom, SeekOrigin.Begin);

            // TGI list
            uint numTGI = reader.ReadUInt32();

            for (int i = 0; i < numTGI; i++)
            {
                ListViewItem geomitem = new ListViewItem();
                geomitem.Text = "TGI #" + (i + 1).ToString();
                uint lodTypeId = reader.ReadUInt32();
                uint lodGroupId = reader.ReadUInt32();
                ulong lodInstanceId = reader.ReadUInt64();
                geomitem.SubItems.Add(lodTypeId.ToString("X8") + ":" + lodGroupId.ToString("X8") + ":" + lodInstanceId.ToString("X16"));
                listView2.Items.Add(geomitem);
            }

        }

        private void showKeyNameFile(Stream keyNameFile)
        {
            BinaryReader reader = new BinaryReader(keyNameFile);

            reader.ReadUInt32();

            int kCount = reader.ReadInt32();

            for (int i = 0; i < kCount; i++)
            {
                ulong instanceID = reader.ReadUInt64();
                uint nameLength = reader.ReadUInt32();
                string kName = keyNameFile.ReadStringASCII(nameLength);

                ListViewItem litem = new ListViewItem();
                litem.Text = instanceID.ToString("X16");
                litem.SubItems.Add(kName);

                listView2.Items.Add(litem);
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                
                ListViewItem item = listView1.SelectedItems[0];
                DatabasePackedFile.Entry indexEntry = (DatabasePackedFile.Entry)indexEntries[Convert.ToInt32(item.SubItems[5].Text)];

                txtLongName.Text = lookupList.lookup(indexEntry.Key.TypeId).longName;
                txtInstanceID.Text = indexEntry.Key.InstanceId.ToString("X16");
                txtOffset.Text = indexEntry.Offset.ToString();
                txtGroupId.Text = indexEntry.Key.GroupId.ToString("X8");

                textBox25.Text = "key:" + indexEntry.Key.TypeId.ToString("X8") + ":" + txtGroupId.Text + ":" + txtInstanceID.Text;

                Stream input = File.OpenRead(this.currentFile.FullName);
                
                Database db = new Database(input, true);

                textBox4.Visible = false;
                panel1.Visible = false;
                listView2.Visible = false;

                this.lockImage = true;
                chkShowAlpha.Visible = false;
                chkShowBlue.Visible = false;
                chkShowGreen.Visible = false;
                chkShowRed.Visible = false;
                this.lockImage = false;

                switch (indexEntry.Key.TypeId)
                {
                    case 0x034AEECB: // Cas Part
                        textBox4.Visible = true;
                        textBox4.Text = "";
                        Stream caspStream = db.GetResourceStream(indexEntry.Key);
                        showCaspFile(caspStream);
                        caspStream.Close();
                        break;

                    case 0x01661233: // MODL 
                        listView2.Visible = true;
                        listView2.Items.Clear();
                        Stream modlStream = db.GetResourceStream(indexEntry.Key);
                        showModlFile(modlStream);
                        modlStream.Close();
                        break;
                    case 0x067CAA11: // Blend Geometry
                        listView2.Visible = true;
                        listView2.Items.Clear();

                        Stream bgeoStream = db.GetResourceStream(indexEntry.Key);
                        showBlendGeomFile(bgeoStream);
                        bgeoStream.Close();

                        break;

                    case 0x0358B08A: // Face Part File
                        listView2.Visible = true;
                        listView2.Items.Clear();

                        Stream fpartStream = db.GetResourceStream(indexEntry.Key);
                        showFacePartFile(fpartStream);
                        fpartStream.Close();

                        break;

                    case 0xb52f5055: // Facial Blend

                        listView2.Visible = true;
                        listView2.Items.Clear();

                        Stream fblendStream = db.GetResourceStream(indexEntry.Key);
                        showFacialBlendFile(fblendStream);
                        fblendStream.Close();

                        break;

                    case 0x033A1435: // Texture Compositor

                        listView2.Visible = true;
                        listView2.Items.Clear();

                        Stream txtcStream = db.GetResourceStream(indexEntry.Key);
                        showTextureCompositorFile(txtcStream);
                        txtcStream.Close();
                        break;

                    case 0x736884F1: // VPXY

                        listView2.Visible = true;
                        listView2.Items.Clear();

                        Stream vpxyStream = db.GetResourceStream(indexEntry.Key);
                        showVpxyFile(vpxyStream);
                        vpxyStream.Close();

                        break;

                    case 0x0166038C: // Key name

                        listView2.Visible = true;
                        listView2.Items.Clear();

                        Stream keyNameFile = db.GetResourceStream(indexEntry.Key);
                        showKeyNameFile(keyNameFile);
                        keyNameFile.Close();

                        break;

                    case 0x00b2d882: // DDS

                        panel1.Visible = true;

                        ddsFile.Load(db.GetResourceStream(indexEntry.Key));
                        this.lockImage = true;
                        pictureBox1.Image = ddsFile.Image();

                        chkShowAlpha.Visible = true;
                        chkShowBlue.Visible = true;
                        chkShowGreen.Visible = true;
                        chkShowRed.Visible = true;
                        chkShowAlpha.Checked = false;
                        chkShowBlue.Checked = true;
                        chkShowGreen.Checked = true;
                        chkShowRed.Checked = true;
                        this.lockImage = false;

                        break;
                    // XML
                    case 0x73e93eeb:
                    case 0xd4d9fbe5:
                    case 0x0333406c:
                        textBox4.Visible = true;
                        textBox4.Text = Encoding.ASCII.GetString(db.GetResource(indexEntry.Key));
                        break;
                    case 0x015a1849:
                        // GEOM
                        listView2.Visible = true;
                        listView2.Items.Clear();

                        Stream geominput = db.GetResourceStream(indexEntry.Key);
                        showGeomFile(geominput);
                        geominput.Close();

                        break;
                }

                input.Close();

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                ListViewItem item = listView1.SelectedItems[0];
                int destIndexId = Convert.ToInt32(item.SubItems[5].Text);

                DatabasePackedFile.Entry indexEntry = new DatabasePackedFile.Entry();
                DatabasePackedFile.Entry oldEntry = new DatabasePackedFile.Entry();

                indexEntry = (DatabasePackedFile.Entry)indexEntries[destIndexId];
                oldEntry = (DatabasePackedFile.Entry)indexEntries[destIndexId];

                indexEntry.Key.InstanceId = UInt64.Parse(txtInstanceID.Text, System.Globalization.NumberStyles.AllowHexSpecifier);
                indexEntry.Key.GroupId = UInt32.Parse(txtGroupId.Text, System.Globalization.NumberStyles.AllowHexSpecifier);
                indexEntry.Offset = Convert.ToInt64(txtOffset.Text);
                //indexEntry.Key.InstanceId = 0;

                // Open DBPF
                Stream input = File.Open(this.currentFile.FullName, FileMode.Open, FileAccess.ReadWrite);

                /*
                DatabasePackedFile dbpf = new DatabasePackedFile();
                try
                {
                    dbpf.Read(input);
                }
                catch (NotAPackageException)
                {
                    MessageBox.Show("bad file: {0}", openFileDialog1.FileName);
                    input.Close();
                    return;
                }

                input.Seek(0, SeekOrigin.Begin);
                */

                Database db;

                try
                {
                    db = new Database(input, true);
                }
                catch (NotAPackageException)
                {
                    MessageBox.Show("bad file: {0}", this.currentFile.FullName);
                    input.Close();
                    return;
                }

                db.MoveResource(oldEntry.Key, indexEntry.Key);

                indexEntries[destIndexId] = indexEntry;

                item.SubItems[3].Text = indexEntry.Key.InstanceId.ToString("X16");

                db.Commit(true);
                
                input.Close();
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog2.ShowDialog();
            if (openFileDialog2.FileName != "")
            {
                Stream inputFile = File.OpenRead(openFileDialog2.FileName);

                ListViewItem item = listView1.SelectedItems[0];
                int destIndexId = Convert.ToInt32(item.SubItems[5].Text);
                DatabasePackedFile.Entry indexEntry = (DatabasePackedFile.Entry)indexEntries[destIndexId];

                // Open DBPF
                Stream dbpf = File.Open(openFileDialog1.FileName, FileMode.Open, FileAccess.ReadWrite);
                Database db;

                try
                {
                    db = new Database(dbpf, true);
                }
                catch (NotAPackageException)
                {
                    MessageBox.Show("bad file: {0}", openFileDialog2.FileName);
                    dbpf.Close();
                    return;
                }

                db.SetResourceStream(indexEntry.Key, inputFile);

                db.Commit(true);

                dbpf.Close();
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {

            progressBar1.Minimum = 0;
            progressBar1.Value = 0;

            string openPath = "";
            if (comboBox1.Text.StartsWith("\\"))
            {
                openPath = MadScience.Helpers.findSims3Root() + comboBox1.Text;
            }
            else
            {
                openPath = comboBox1.Text;
            }

            if (openPath == "") return;

            Stream input = File.OpenRead(openPath);

            Database db = new Database(input, true);

            input.Seek(0, SeekOrigin.Begin);

            DatabasePackedFile dbpf = new DatabasePackedFile();
            try
            {
                dbpf.Read(input);
            }
            catch (NotAPackageException)
            {
                MessageBox.Show("bad file: {0}", comboBox1.Text);
                input.Close();
                return;
            }

            this.currentFile = new FileInfo(openPath);


            lookupTypes();
            indexEntries.Clear();
            listView1.Items.Clear();

            txtPkgIndexType.Text = dbpf.IndexType.ToString();
            txtPkgNumChunks.Text = dbpf.Entries.Count.ToString();

            uint searchTypeID = 0;
            if (comboBox2.Text != "") { searchTypeID = Gibbed.Helpers.StringHelpers.ParseHex32(comboBox2.Text); }
            ulong instanceID = 0;
            bool searchInstance = false;
            if (textBox8.Text != "")
            {
                instanceID = Gibbed.Helpers.StringHelpers.ParseHex64(textBox8.Text);
                searchInstance = true;
            }

            string tempChunk = "";
            int count = 0;

            progressBar1.Maximum = dbpf.Entries.Count;


            for (int i = 0; i < dbpf.Entries.Count; i++)
            {
                ListViewItem item = new ListViewItem();
                DatabasePackedFile.Entry entry = dbpf.Entries[i];
                progressBar1.Value++;

                bool searchChunk = false;


                if (entry.Key.TypeId == searchTypeID)
                {
                    if (searchInstance == true)
                    {
                        if (entry.Key.InstanceId == instanceID)
                        {
                            searchChunk = true;
                        }
                    }
                    else
                    {
                        searchChunk = true;
                    }
                }
                else
                {
                    if (searchInstance == true)
                    {
                        if (entry.Key.InstanceId == instanceID)
                        {
                            searchChunk = true;
                        }
                    }
                }
                //if (searchTypeID == 0) { searchChunk = true; }

                if (searchChunk)
                {

                    tempChunk = Encoding.UTF8.GetString(db.GetResource(entry.Key));
                    bool addChunk = false;

                    // If everything is blank we just list... everything
                    if (textBox3.Text == "" && textBox6.Text == "" && textBox7.Text == "")
                    {
                        addChunk = true;
                    }
                    else
                    {

                        if (tempChunk.Contains(textBox3.Text))
                        {
                            if (textBox6.Text != "")
                            {
                                if (tempChunk.Contains(textBox6.Text))
                                {
                                    if (textBox7.Text != "")
                                    {
                                        if (tempChunk.Contains(textBox7.Text))
                                        {
                                            addChunk = true;
                                        }
                                    }
                                    else
                                    {
                                        addChunk = true;
                                    }
                                }
                            }
                            else
                            {
                                addChunk = true;
                            }

                        }

                    }

                    if (addChunk == true)
                    {
                        item.Text = this.lookupList.lookup(entry.Key.TypeId).shortName;
                        item.SubItems.Add(entry.Key.TypeId.ToString("X8"));
                        item.SubItems.Add(entry.Key.GroupId.ToString("X8"));
                        item.SubItems.Add(entry.Key.InstanceId.ToString("X16"));
                        item.SubItems.Add(entry.DecompressedSize.ToString());
                        item.SubItems.Add(count.ToString());
                        listView1.Items.Add(item);
                        indexEntries.Add(entry);
                        count++;
                    }
                }
            }

            input.Close();


        }

        private void button4_Click(object sender, EventArgs e)
        {
            Stream file;
            Database db;
            ResourceKey rkey;
            Stream input;

            file = File.Open("P:\\Games\\Working\\Skins\\afface - dark\\afface-dark.package", FileMode.Create, FileAccess.ReadWrite);
            input = File.Open("P:\\Games\\Working\\Skins\\afface - dark\\afface-dark.dds", FileMode.Open, FileAccess.Read);
            db = new Database(file, false);
            rkey = new ResourceKey((ulong)0x304910BE2CB17463, 11720834, 0);
            db.SetResourceStream(rkey, input);
            db.Commit(true);
            input.Close();
            file.Close();

            file = File.Open("P:\\Games\\Working\\Skins\\afface - light\\afface-light.package", FileMode.Create, FileAccess.ReadWrite);
            input = File.Open("P:\\Games\\Working\\Skins\\afface - dark\\afface-dark.dds", FileMode.Open, FileAccess.Read);
            db = new Database(file, false);
            rkey = new ResourceKey((ulong)0x40E5744B0DBFC323, 11720834, 0);
            db.SetResourceStream(rkey, input);
            db.Commit(true);
            input.Close();
            file.Close();

            file = File.Open("P:\\Games\\Working\\Skins\\af-tf-efbody - dark\\af-tf-efbody fullybarbie-dark.package", FileMode.Create, FileAccess.ReadWrite);
            input = File.Open("P:\\Games\\Working\\Skins\\af-tf-efbody - dark\\fullybarbie-dark.dds", FileMode.Open, FileAccess.Read);
            db = new Database(file, false);
            rkey = new ResourceKey((ulong)0x185D7126C73DC404, 11720834, 0);
            db.SetResourceStream(rkey, input);
            rkey = new ResourceKey((ulong)0x13F78C079D70D7F8, 11720834, 0);
            db.SetResourceStream(rkey, input);
            db.Commit(true);
            input.Close();
            file.Close();

            file = File.Open("P:\\Games\\Working\\Skins\\af-tf-efbody - dark\\af-tf-efbody nipplesandpubes-dark.package", FileMode.Create, FileAccess.ReadWrite);
            input = File.Open("P:\\Games\\Working\\Skins\\af-tf-efbody - dark\\nipplesandpubes-dark.dds", FileMode.Open, FileAccess.Read);
            db = new Database(file, false);
            rkey = new ResourceKey((ulong)0x185D7126C73DC404, 11720834, 0);
            db.SetResourceStream(rkey, input);
            rkey = new ResourceKey((ulong)0x13F78C079D70D7F8, 11720834, 0);
            db.SetResourceStream(rkey, input);
            db.Commit(true);
            input.Close();
            file.Close();

            file = File.Open("P:\\Games\\Working\\Skins\\af-tf-efbody - dark\\af-tf-efbody nipplesnopubes-dark.package", FileMode.Create, FileAccess.ReadWrite);
            input = File.Open("P:\\Games\\Working\\Skins\\af-tf-efbody - dark\\nipplesnopubes-dark.dds", FileMode.Open, FileAccess.Read);
            db = new Database(file, false);
            rkey = new ResourceKey((ulong)0x185D7126C73DC404, 11720834, 0);
            db.SetResourceStream(rkey, input);
            rkey = new ResourceKey((ulong)0x13F78C079D70D7F8, 11720834, 0);
            db.SetResourceStream(rkey, input);
            db.Commit(true);
            input.Close();
            file.Close();

            file = File.Open("P:\\Games\\Working\\Skins\\af-tf-efbody - light\\af-tf-efbody fullybarbie-light.package", FileMode.Create, FileAccess.ReadWrite);
            input = File.Open("P:\\Games\\Working\\Skins\\af-tf-efbody - light\\fullybarbie.dds", FileMode.Open, FileAccess.Read);
            db = new Database(file, false);
            rkey = new ResourceKey((ulong)0x798410349B50700C, 11720834, 0);
            db.SetResourceStream(rkey, input);
            rkey = new ResourceKey((ulong)0xB4CDC208D8D51BF0, 11720834, 0);
            db.SetResourceStream(rkey, input);
            db.Commit(true);
            input.Close();
            file.Close();

            file = File.Open("P:\\Games\\Working\\Skins\\af-tf-efbody - light\\af-tf-efbody nipplesandpubes-light.package", FileMode.Create, FileAccess.ReadWrite);
            input = File.Open("P:\\Games\\Working\\Skins\\af-tf-efbody - light\\nipplesandpubes.dds", FileMode.Open, FileAccess.Read);
            db = new Database(file, false);
            rkey = new ResourceKey((ulong)0x798410349B50700C, 11720834, 0);
            db.SetResourceStream(rkey, input);
            rkey = new ResourceKey((ulong)0xB4CDC208D8D51BF0, 11720834, 0);
            db.SetResourceStream(rkey, input);
            db.Commit(true);
            input.Close();
            file.Close();

            file = File.Open("P:\\Games\\Working\\Skins\\af-tf-efbody - light\\af-tf-efbody nipplesnopubes-light.package", FileMode.Create, FileAccess.ReadWrite);
            input = File.Open("P:\\Games\\Working\\Skins\\af-tf-efbody - light\\nipplesnopubes.dds", FileMode.Open, FileAccess.Read);
            db = new Database(file, false);
            rkey = new ResourceKey((ulong)0x798410349B50700C, 11720834, 0);
            db.SetResourceStream(rkey, input);
            rkey = new ResourceKey((ulong)0xB4CDC208D8D51BF0, 11720834, 0);
            db.SetResourceStream(rkey, input);
            db.Commit(true);
            input.Close();
            file.Close();

            file = File.Open("P:\\Games\\Working\\Skins\\am-embody - dark\\am-embody fullhair-dark.package", FileMode.Create, FileAccess.ReadWrite);
            input = File.Open("P:\\Games\\Working\\Skins\\am-embody - dark\\ambody-fullhair.dds", FileMode.Open, FileAccess.Read);
            db = new Database(file, false);
            rkey = new ResourceKey((ulong)0xB1D30A51A5ED1903, 11720834, 0);
            db.SetResourceStream(rkey, input);
            rkey = new ResourceKey((ulong)0xD4EE1715BCEA0F77, 11720834, 0);
            db.SetResourceStream(rkey, input);
            db.Commit(true);
            input.Close();
            file.Close();

            file = File.Open("P:\\Games\\Working\\Skins\\am-embody - dark\\am-embody hairless-dark.package", FileMode.Create, FileAccess.ReadWrite);
            input = File.Open("P:\\Games\\Working\\Skins\\am-embody - dark\\ambody-hairless.dds", FileMode.Open, FileAccess.Read);
            db = new Database(file, false);
            rkey = new ResourceKey((ulong)0xB1D30A51A5ED1903, 11720834, 0);
            db.SetResourceStream(rkey, input);
            rkey = new ResourceKey((ulong)0xD4EE1715BCEA0F77, 11720834, 0);
            db.SetResourceStream(rkey, input);
            db.Commit(true);
            input.Close();
            file.Close();

            file = File.Open("P:\\Games\\Working\\Skins\\am-embody - dark\\am-embody lighterchest-dark.package", FileMode.Create, FileAccess.ReadWrite);
            input = File.Open("P:\\Games\\Working\\Skins\\am-embody - dark\\ambody-lighterchest.dds", FileMode.Open, FileAccess.Read);
            db = new Database(file, false);
            rkey = new ResourceKey((ulong)0xB1D30A51A5ED1903, 11720834, 0);
            db.SetResourceStream(rkey, input);
            rkey = new ResourceKey((ulong)0xD4EE1715BCEA0F77, 11720834, 0);
            db.SetResourceStream(rkey, input);
            db.Commit(true);
            input.Close();
            file.Close();

            file = File.Open("P:\\Games\\Working\\Skins\\am-embody - dark\\am-embody nopubesnohappytrail-dark.package", FileMode.Create, FileAccess.ReadWrite);
            input = File.Open("P:\\Games\\Working\\Skins\\am-embody - dark\\ambody-nopubesnohappytraillighthair.dds", FileMode.Open, FileAccess.Read);
            db = new Database(file, false);
            rkey = new ResourceKey((ulong)0xB1D30A51A5ED1903, 11720834, 0);
            db.SetResourceStream(rkey, input);
            rkey = new ResourceKey((ulong)0xD4EE1715BCEA0F77, 11720834, 0);
            db.SetResourceStream(rkey, input);
            db.Commit(true);
            input.Close();
            file.Close();

            file = File.Open("P:\\Games\\Working\\Skins\\am-embody - dark\\am-embody lighthairwithpubesnohappytrail-dark.package", FileMode.Create, FileAccess.ReadWrite);
            input = File.Open("P:\\Games\\Working\\Skins\\am-embody - dark\\am-lighthairwithpubesnohappytrail.dds", FileMode.Open, FileAccess.Read);
            db = new Database(file, false);
            rkey = new ResourceKey((ulong)0xB1D30A51A5ED1903, 11720834, 0);
            db.SetResourceStream(rkey, input);
            rkey = new ResourceKey((ulong)0xD4EE1715BCEA0F77, 11720834, 0);
            db.SetResourceStream(rkey, input);
            db.Commit(true);
            input.Close();
            file.Close();

            file = File.Open("P:\\Games\\Working\\Skins\\am-embody - light\\am-embody fullhair-light.package", FileMode.Create, FileAccess.ReadWrite);
            input = File.Open("P:\\Games\\Working\\Skins\\am-embody - light\\ambody-fullhair.dds", FileMode.Open, FileAccess.Read);
            db = new Database(file, false);
            rkey = new ResourceKey((ulong)0x4DB46D1662895FC3, 11720834, 0);
            db.SetResourceStream(rkey, input);
            rkey = new ResourceKey((ulong)0x9121FA3B0B65C88F, 11720834, 0);
            db.SetResourceStream(rkey, input);
            db.Commit(true);
            input.Close();
            file.Close();

            file = File.Open("P:\\Games\\Working\\Skins\\am-embody - light\\am-embody hairless-light.package", FileMode.Create, FileAccess.ReadWrite);
            input = File.Open("P:\\Games\\Working\\Skins\\am-embody - light\\ambodyhairless.dds", FileMode.Open, FileAccess.Read);
            db = new Database(file, false);
            rkey = new ResourceKey((ulong)0x4DB46D1662895FC3, 11720834, 0);
            db.SetResourceStream(rkey, input);
            rkey = new ResourceKey((ulong)0x9121FA3B0B65C88F, 11720834, 0);
            db.SetResourceStream(rkey, input);
            db.Commit(true);
            input.Close();
            file.Close();

            file = File.Open("P:\\Games\\Working\\Skins\\am-embody - light\\am-embody lighterchest-light.package", FileMode.Create, FileAccess.ReadWrite);
            input = File.Open("P:\\Games\\Working\\Skins\\am-embody - light\\ambody-lighterchest.dds", FileMode.Open, FileAccess.Read);
            db = new Database(file, false);
            rkey = new ResourceKey((ulong)0x4DB46D1662895FC3, 11720834, 0);
            db.SetResourceStream(rkey, input);
            rkey = new ResourceKey((ulong)0x9121FA3B0B65C88F, 11720834, 0);
            db.SetResourceStream(rkey, input);
            db.Commit(true);
            input.Close();
            file.Close();

            file = File.Open("P:\\Games\\Working\\Skins\\am-embody - light\\am-embody nopubesnohappytrail-light.package", FileMode.Create, FileAccess.ReadWrite);
            input = File.Open("P:\\Games\\Working\\Skins\\am-embody - light\\ambody-nopubesnohappytraillighthair.dds", FileMode.Open, FileAccess.Read);
            db = new Database(file, false);
            rkey = new ResourceKey((ulong)0x4DB46D1662895FC3, 11720834, 0);
            db.SetResourceStream(rkey, input);
            rkey = new ResourceKey((ulong)0x9121FA3B0B65C88F, 11720834, 0);
            db.SetResourceStream(rkey, input);
            db.Commit(true);
            input.Close();
            file.Close();

            file = File.Open("P:\\Games\\Working\\Skins\\am-embody - light\\am-embody lighthairwithpubesnohappytrail-dark.package", FileMode.Create, FileAccess.ReadWrite);
            input = File.Open("P:\\Games\\Working\\Skins\\am-embody - light\\am-lighthairwithpubesnohappytrail.dds", FileMode.Open, FileAccess.Read);
            db = new Database(file, false);
            rkey = new ResourceKey((ulong)0x4DB46D1662895FC3, 11720834, 0);
            db.SetResourceStream(rkey, input);
            rkey = new ResourceKey((ulong)0x9121FA3B0B65C88F, 11720834, 0);
            db.SetResourceStream(rkey, input);
            db.Commit(true);
            input.Close();
            file.Close();

            file = File.Open("P:\\Games\\Working\\Skins\\amface - dark\\amface-dark.package", FileMode.Create, FileAccess.ReadWrite);
            input = File.Open("P:\\Games\\Working\\Skins\\amface - dark\\amface-dark.dds", FileMode.Open, FileAccess.Read);
            db = new Database(file, false);
            rkey = new ResourceKey((ulong)0x6CA41C919ECE1BE0, 11720834, 0);
            db.SetResourceStream(rkey, input);
            db.Commit(true);
            input.Close();
            file.Close();

            file = File.Open("P:\\Games\\Working\\Skins\\amface - light\\amface-light.package", FileMode.Create, FileAccess.ReadWrite);
            input = File.Open("P:\\Games\\Working\\Skins\\amface - light\\amface.dds", FileMode.Open, FileAccess.Read);
            db = new Database(file, false);
            rkey = new ResourceKey((ulong)0x6F853F0E35157E24, 11720834, 0);
            db.SetResourceStream(rkey, input);
            db.Commit(true);
            input.Close();
            file.Close();

            file = File.Open("P:\\Games\\Working\\Skins\\efface - dark\\efface-dark.package", FileMode.Create, FileAccess.ReadWrite);
            input = File.Open("P:\\Games\\Working\\Skins\\efface - dark\\efface-dark.dds", FileMode.Open, FileAccess.Read);
            db = new Database(file, false);
            rkey = new ResourceKey((ulong)0x629E533C0262E927, 11720834, 0);
            db.SetResourceStream(rkey, input);
            db.Commit(true);
            input.Close();
            file.Close();

            file = File.Open("P:\\Games\\Working\\Skins\\efface - light\\efface-light.package", FileMode.Create, FileAccess.ReadWrite);
            input = File.Open("P:\\Games\\Working\\Skins\\efface - light\\efface.dds", FileMode.Open, FileAccess.Read);
            db = new Database(file, false);
            rkey = new ResourceKey((ulong)0x503C527E546E8C5F, 11720834, 0);
            db.SetResourceStream(rkey, input);
            db.Commit(true);
            input.Close();
            file.Close();

            file = File.Open("P:\\Games\\Working\\Skins\\emface - dark\\emface-dark.package", FileMode.Create, FileAccess.ReadWrite);
            input = File.Open("P:\\Games\\Working\\Skins\\emface - dark\\emface - dark.dds", FileMode.Open, FileAccess.Read);
            db = new Database(file, false);
            rkey = new ResourceKey((ulong)0x21F6BAC9B048B934, 11720834, 0);
            db.SetResourceStream(rkey, input);
            db.Commit(true);
            input.Close();
            file.Close();

            file = File.Open("P:\\Games\\Working\\Skins\\emface - light\\emface-light.package", FileMode.Create, FileAccess.ReadWrite);
            input = File.Open("P:\\Games\\Working\\Skins\\emface - light\\emface.dds", FileMode.Open, FileAccess.Read);
            db = new Database(file, false);
            rkey = new ResourceKey((ulong)0x7EDD5D417BC66680, 11720834, 0);
            db.SetResourceStream(rkey, input);
            db.Commit(true);
            input.Close();
            file.Close();

            file = File.Open("J:\\Users\\Stuart\\AppData\\Roaming\\Miranda\\Received Files\\hystericalparoxysm@hotmail.com\\afBottomBriefs_biki_no.package", FileMode.Create, FileAccess.ReadWrite);
            input = File.Open("J:\\Users\\Stuart\\AppData\\Roaming\\Miranda\\Received Files\\hystericalparoxysm@hotmail.com\\afBottomBriefs_biki_0x849f383fd886bb35_0x00B2D882-0x00000000-0x849F383FD886BB35.dds", FileMode.Open, FileAccess.Read);
            db = new Database(file, false);
            rkey = new ResourceKey((ulong)0x849F383FD886BB35, 11720834, 0);
            db.SetResourceStream(rkey, input);
            db.Commit(true);
            input.Close();
            file.Close();

            file = File.Open("J:\\Users\\Stuart\\AppData\\Roaming\\Miranda\\Received Files\\hystericalparoxysm@hotmail.com\\afTopBra_Strapless_no.package", FileMode.Create, FileAccess.ReadWrite);
            input = File.Open("J:\\Users\\Stuart\\AppData\\Roaming\\Miranda\\Received Files\\hystericalparoxysm@hotmail.com\\afTopBra_Strapless__0x0baf3c417b3d7bd2_0x00B2D882-0x00000000-0x0BAF3C417B3D7BD2.dds", FileMode.Open, FileAccess.Read);
            db = new Database(file, false);
            rkey = new ResourceKey((ulong)0x0BAF3C417B3D7BD2, 11720834, 0);
            db.SetResourceStream(rkey, input);
            db.Commit(true);
            input.Close();
            file.Close();

            file = File.Open("J:\\Users\\Stuart\\AppData\\Roaming\\Miranda\\Received Files\\hystericalparoxysm@hotmail.com\\NoZzzs.package", FileMode.Create, FileAccess.ReadWrite);
            input = File.Open("J:\\Users\\Stuart\\AppData\\Roaming\\Miranda\\Received Files\\hystericalparoxysm@hotmail.com\\z_0xaf63bd4c8601b7a5_0x00B2D882-0x00000000-0xAF63BD4C8601B7A5.dds", FileMode.Open, FileAccess.Read);
            db = new Database(file, false);
            rkey = new ResourceKey((ulong)0xAF63BD4C8601B7A5, 11720834, 0);
            db.SetResourceStream(rkey, input);
            db.Commit(true);
            input.Close();
            file.Close();

            MessageBox.Show("Done");
        }


        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {

                ListViewItem item = listView1.SelectedItems[0];
                DatabasePackedFile.Entry indexEntry = (DatabasePackedFile.Entry)indexEntries[Convert.ToInt32(item.SubItems[5].Text)];

                metaEntry mEntry = lookupList.lookup(indexEntry.Key.TypeId);
                if (mEntry.shortName == "") { mEntry.shortName = "unk"; }
                txtLongName.Text = mEntry.longName;
                txtInstanceID.Text = indexEntry.Key.InstanceId.ToString("X16");
                txtOffset.Text = indexEntry.Offset.ToString();

                Stream input = File.OpenRead(this.currentFile.FullName);

                Database db = new Database(input, true);

                Stream output = db.GetResourceStream(indexEntry.Key);
                FileStream saveFile = new FileStream(this.currentFile.DirectoryName + "\\" + txtLongName.Text + "_" + indexEntry.Key.TypeId.ToString("X8") + "_" + indexEntry.Key.GroupId.ToString("X8") + "_" + txtInstanceID.Text + "." + mEntry.shortName, FileMode.Create, FileAccess.Write);

                ReadWriteStream(output, saveFile);

                saveFile.Close();
                output.Close();
                input.Close();

            }
        }

        private void ReadWriteStream(Stream readStream, Stream writeStream)
        {
            int Length = 256;
            Byte[] buffer = new Byte[Length];
            int bytesRead = readStream.Read(buffer, 0, Length);
            // write the required bytes
            while (bytesRead > 0)
            {
                writeStream.Write(buffer, 0, bytesRead);
                bytesRead = readStream.Read(buffer, 0, Length);
            }
            readStream.Close();
            writeStream.Close();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            textBox2.Text = Gibbed.Helpers.StringHelpers.HashFNV32(textBox1.Text).ToString("X8");
            textBox10.Text = Gibbed.Helpers.StringHelpers.HashFNV64(textBox1.Text).ToString("X16");
            textBox24.Text = Gibbed.Helpers.StringHelpers.HashFNV24(textBox1.Text).ToString("X8");
        }

        private void chkShowRed_CheckedChanged(object sender, EventArgs e)
        {
            showDDSChannels();
        }

        private void chkShowGreen_CheckedChanged(object sender, EventArgs e)
        {
            showDDSChannels();
        }

        private void chkShowBlue_CheckedChanged(object sender, EventArgs e)
        {
            showDDSChannels();
        }

        private void chkShowAlpha_CheckedChanged(object sender, EventArgs e)
        {
            showDDSChannels();
        }

        private void showDDSChannels()
        {
            if (!this.lockImage)
            {
                pictureBox1.Image = ddsFile.Image(chkShowRed.Checked, chkShowGreen.Checked, chkShowBlue.Checked, chkShowAlpha.Checked);
            }
        }


        MadScience.ListViewSorter Sorter = new MadScience.ListViewSorter();

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            Sorter.Sort(listView1, e);
        }

        private void compareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.ShowDialog();
            form2 = null;
        }

        private void button12_Click(object sender, EventArgs e)
        {
            const int printStatus = 4;
            string letters = "abcdefghijklmnopqrstuvwxyz0123456789.";
            
            List<uint> hash32 = new List<uint>(new uint[] { Gibbed.Helpers.StringHelpers.ParseHex32(textBox2.Text) });

            string prefix = "";

            for (int length = 1; length <= 6; length++)
            {
                Console.WriteLine("Welcome to round " + length.ToString() + "!");

                int[] state = new int[length];

                bool go = true;
                if (length >= printStatus)
                {
                    Console.Write(letters[0]);
                }
                while (go)
                {
                    string text = prefix;
                    for (int i = 0; i < length; i++)
                    {
                        text += letters[state[i]];
                    }

                    //Console.WriteLine(text);
                    if (hash32.Contains(text.HashFNV32()))
                    {
                        textBox1.Text = text;
                    }

                    state[0]++;
                    for (int i = 0; i < length; i++)
                    {
                        if (state[i] >= letters.Length)
                        {
                            if (i + 1 >= length)
                            {
                                go = false;
                                break;
                            }

                            state[i] = 0;
                            state[i + 1]++;

                            if (length >= printStatus && i + 2 >= length && state[i + 1] < letters.Length)
                            {
                                Console.Write(letters[state[i + 1]]);
                            }
                        }
                    }
                }

                if (length >= printStatus)
                {
                    Console.WriteLine();
                }
            }
            
        }

        private void button12_Click_1(object sender, EventArgs e)
        {
            CountFiles(@"P:\Stuart\My Documents\Electronic Arts\The Sims 3\DCBackup\");
            ProcessFiles();
        }

        public void ProcessFiles()
        {

            DateTime start = new DateTime();
            start = DateTime.Now;

            int numFiles = this.fileList.Count;
            string filename = "";

            for (int i = 0; i < numFiles; i++)
            {
                try
                {
                    filename = this.fileList.Pop().ToString();
                    FileInfo f = new FileInfo(filename);
                    Console.WriteLine("Processing " + f.Name);
                    Stream input = File.OpenRead(filename);
                    Database db = new Database(input);

                    foreach (ResourceKey entry in db.Entries.Keys)
                    {
                        if (entry.GroupId == 0x00DCE592 || entry.GroupId == 0x01DCE592)
                        //if (entry.GroupId == 0x00FACADE || entry.GroupId == 0x01FACADE)
                        {
                            ListViewItem item = new ListViewItem();
                            item.Text = "";
                            item.SubItems.Add(entry.TypeId.ToString("X8"));
                            item.SubItems.Add(entry.GroupId.ToString("X8"));
                            item.SubItems.Add(entry.InstanceId.ToString("X16"));
                            item.SubItems.Add(f.Name);
                            item.SubItems.Add("");
                            listView1.Items.Add(item);
                            
                        }
                    }
                    
                    input.Close();

                }
                catch (System.Exception excpt)
                {
                    MessageBox.Show(excpt.Message + " " + excpt.StackTrace);
                }
            }

            DateTime stop = new DateTime();
            stop = DateTime.Now;
            int timeTaken = (stop.Second - start.Second);
            if (timeTaken == 0) timeTaken++;

        }


        private Stack fileList = new Stack();
        public void CountFiles(string sDir)
        {
            try
            {

                DirectoryInfo dir = new DirectoryInfo(sDir);
                FileInfo[] myFiles = dir.GetFiles("*.package");
                //filesToProcess += myFiles.Length;
                foreach (FileInfo f in myFiles)
                {
                    this.fileList.Push(f.FullName);
                }

                Application.DoEvents();

            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void makeNewPackageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form3 form3 = new Form3();
            form3.ShowDialog();
            form3 = null;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (Environment.GetCommandLineArgs().Length > 1)
            {
                string filename = Environment.GetCommandLineArgs()[1].ToString();
                loadFile(filename);
            }
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            Stream boneList = File.OpenRead(@"C:\Programming\Projects\Sims 3\BoneDeltaEditor\bin\Debug\bones.txt");
            TextReader reader = new StreamReader(boneList);

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sb.AppendLine("<bones>");

            string input = null;
            while ((input = reader.ReadLine()) != null)
            {
                sb.AppendLine("  <bone name=\"" + input + "\" hash=\"" + Gibbed.Helpers.StringHelpers.HashFNV32(input).ToString("X8") + "\" />");
            }
            sb.AppendLine("</bones>");

            Stream boneXml = File.OpenWrite(@"C:\Programming\Projects\Sims 3\BoneDeltaEditor\bin\Debug\bones.xml");
            Gibbed.Helpers.StreamHelpers.WriteStringASCII(boneXml, sb.ToString());
            boneXml.Close();

            reader = null;
            boneList.Close();
        }




    }


    public class metaEntry
    {
        [XmlAttribute("key")]
        public uint key;

        [XmlElement("shortName")]
        public string shortName;

        [XmlElement("longName")]
        public string longName;

        public metaEntry()
        {
        }

        [XmlRoot("typesToMeta")]
        public class typesToMeta
        {

            private ArrayList metaTypes;
            private Hashtable metaTypes2;

            public typesToMeta()
            {
                metaTypes = new ArrayList();
                metaTypes2 = new Hashtable();
            }

            public metaEntry lookup(uint typeID)
            {
                if (metaTypes2[typeID] != null)
                {
                    return (metaEntry)metaTypes2[typeID];
                }
                else
                {
                    metaEntry empty = new metaEntry();
                    return empty;
                }
            }

            [XmlElement("entry")]
            public metaEntry[] Entries
            {
                get
                {
                    metaEntry[] entries = new metaEntry[metaTypes.Count];
                    metaTypes.CopyTo(entries);
                    return entries;
                }
                set
                {
                    if (value == null) return;

                    metaEntry[] entries = (metaEntry[])value;
                    metaTypes2.Clear();
                    foreach (metaEntry entry in entries)
                    {
                        metaTypes2.Add(entry.key, entry);
                    }


                }
            }

        }

        public metaEntry(string ShortName, string LongName)
        {
            shortName = ShortName;
            longName = LongName;
        }
    }

}
