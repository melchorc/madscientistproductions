using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using MadScience.Wrappers;

namespace SimGeomEditor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private string filename = "";
        private SimGeomFile simgeomfile = new SimGeomFile();

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Add(" ");
            foreach (uint fieldHash in Enum.GetValues(typeof(ShaderType)))
            {
                Console.WriteLine(Enum.GetName(typeof(ShaderType), fieldHash));
                    comboBox1.Items.Add(Enum.GetName(typeof(ShaderType), fieldHash));
            }

            if (Environment.GetCommandLineArgs().Length > 1)
            {
                loadFile(Environment.GetCommandLineArgs()[1].ToString());
            }
            else
            {
                lstEntries.Items.Clear();
                saveToolStripMenuItem.Enabled = false;
            }
        }

        private void loadFile(string filename)
        {

            this.filename = filename;

            this.simgeomfile = new SimGeomFile();

            toolStripStatusLabel1.Text = this.filename;

            // Deals with RAW chunks here...
            Stream input = File.OpenRead(filename);
            this.simgeomfile.Load(input);
            input.Close();

            saveToolStripMenuItem.Enabled = true;

            int num = 0;
            comboBox1.SelectedIndex = num;
            foreach (uint fieldHash in Enum.GetValues(typeof(ShaderType)))
            {
                num++;
                if (fieldHash == simgeomfile.simgeom.embeddedId)
                {
                    comboBox1.SelectedIndex = num;
                    break;
                }
            }

            showEntries();

            importMTNFToolStripMenuItem.Enabled = true;
            exportMTNFToolStripMenuItem.Enabled = true;


        }
        private void showEntries()
        {
            lstEntries.Items.Clear();

            for (int i = 0; i < simgeomfile.simgeom.mtnfChunk.entries.Count; i++)
            {
                MTNFEntry mtnf = simgeomfile.simgeom.mtnfChunk.entries[i];
                ListViewItem item = new ListViewItem();
                item.Text = i.ToString();
                foreach (uint fieldHash in Enum.GetValues(typeof(FieldTypes)))
                {
                    if (fieldHash == mtnf.fieldTypeHash)
                    {
                        item.SubItems.Add(Enum.GetName(typeof(FieldTypes), fieldHash));
                        break;                        
                    }
                }
                if (item.SubItems.Count == 1) item.SubItems.Add(mtnf.fieldTypeHash.ToString("X8"));
                lstEntries.Items.Add(item);

            }

            listView1.Items.Clear();
            for (int i = 0; i < simgeomfile.simgeom.keytable.keys.Count; i++)
            {
                ListViewItem item = new ListViewItem();
                item.Text = "TGI #" + i.ToString();
                item.SubItems.Add(simgeomfile.simgeom.keytable.keys[i].ToString());
                listView1.Items.Add(item);
                item = null;
            }

            lstBones.Items.Clear();
            for (int i = 0; i < simgeomfile.simgeom.boneHashes.Count; i++)
            {
                ListViewItem item = new ListViewItem();
                item.Text = "#" + i.ToString();
                item.SubItems.Add(simgeomfile.simgeom.boneHashes[i].ToString());
                lstBones.Items.Add(item);
                item = null;
            }

            lstVertices.Visible = false;
            lstVertices.Items.Clear();

            for (int i = 0; i < simgeomfile.simgeom.vertices.Count; i++)
            {
                ListViewItem item = new ListViewItem();
                item.Text = "#" + i.ToString();
                for (int j = 0; j < simgeomfile.simgeom.vertexFormats.Count; j++)
                {
                    switch (simgeomfile.simgeom.vertexFormats[j].dataType)
                    {
                        case 1:
                            item.SubItems.Add(simgeomfile.simgeom.vertices[i].x.ToString() + " " + simgeomfile.simgeom.vertices[i].y.ToString() + " " + simgeomfile.simgeom.vertices[i].z.ToString());
                            break;
                        case 2:
                            item.SubItems.Add(simgeomfile.simgeom.normals[i].x.ToString() + " " + simgeomfile.simgeom.normals[i].y.ToString() + " " + simgeomfile.simgeom.normals[i].z.ToString());
                            break;
                        case 3:
                            item.SubItems.Add(simgeomfile.simgeom.uvs[i].u.ToString() + " " + simgeomfile.simgeom.uvs[i].v.ToString());
                            break;
                        case 4:
                            item.SubItems.Add(simgeomfile.simgeom.bones[i].ToString() + " " + simgeomfile.simgeom.bones[i].ToString("X4"));
                            break;
                        case 5:
                            item.SubItems.Add(simgeomfile.simgeom.weights[i].x.ToString() + " " + simgeomfile.simgeom.weights[i].y.ToString() + " " + simgeomfile.simgeom.weights[i].z.ToString() + " " + simgeomfile.simgeom.weights[i].w.ToString());
                            break;
                        case 6:
                            item.SubItems.Add(simgeomfile.simgeom.tangentNormals[i].x.ToString() + " " + simgeomfile.simgeom.tangentNormals[i].y.ToString() + " " + simgeomfile.simgeom.tangentNormals[i].z.ToString());
                            break;
                        case 7:
                            item.SubItems.Add(simgeomfile.simgeom.tagVals[i].ToString());
                            break;
                        case 10:
                            item.SubItems.Add(simgeomfile.simgeom.vertexIds[i].ToString());
                            break;

                    }
                }

                lstVertices.Items.Add(item);
                item = null;
            }
            lstVertices.Visible = true;
        }



        private void rCOLHeaderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MadScience.Wrappers.RCOLHeaderEditor rForm = new MadScience.Wrappers.RCOLHeaderEditor();
            rForm.rcolHeader = this.simgeomfile.rcolHeader;
            if (rForm.ShowDialog() == DialogResult.OK)
            {
                this.simgeomfile.rcolHeader = rForm.rcolHeader;
            }
            rForm.Close();
            rForm = null;
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            simgeomfile.rcolHeader = new MadScience.Wrappers.RcolHeader();
            simgeomfile.simgeom = new MadScience.Wrappers.SimGeom();

            foreach (Control control in this.Controls)
            {
                MadScienceSmall.Helpers.resetControl(control);
            }
            //foreach (Control control in this.groupBox1.Controls)
            //{
                //MadScienceSmall.Helpers.resetControl(control);
            //}
            lstEntries.Items.Clear();
            saveToolStripMenuItem.Enabled = false;
            importMTNFToolStripMenuItem.Enabled = true;
            exportMTNFToolStripMenuItem.Enabled = false;

            //txtVersion.Text = this.bdFile.bonedelta.version.ToString();
            //button2.Enabled = false;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Sim Geom File|*.simgeom";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                loadFile(openFileDialog1.FileName);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stream saveFile = File.Open(this.filename, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            saveSimGeom(saveFile);
            saveFile.Close();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Sim Geometry File|*.simgeom";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.filename = saveFileDialog1.FileName;
                toolStripStatusLabel1.Text = this.filename;
                Stream saveFile = File.Open(this.filename, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                saveSimGeom(saveFile);
                saveFile.Close();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        
        private void saveSimGeom(Stream output)
        {
            this.simgeomfile.Save(output);
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                txtNewTGI.Text = listView1.SelectedItems[0].SubItems[1].Text;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                simgeomfile.simgeom.keytable.keys.RemoveAt(listView1.SelectedIndices[0]);
                listView1.Items.Clear();
                for (int i = 0; i < simgeomfile.simgeom.keytable.keys.Count; i++)
                {
                    ListViewItem item = new ListViewItem();
                    item.Text = "TGI #" + i.ToString();
                    item.SubItems.Add(simgeomfile.simgeom.keytable.keys[i].ToString());
                    listView1.Items.Add(item);
                    item = null;
                }
                txtNewTGI.Text = "";
                //showGbEntry(cmbChooseGeomEntry.SelectedIndex);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                // Check if anything uses this TGI
                simgeomfile.simgeom.keytable.keys[listView1.SelectedIndices[0]] = new MadScience.Wrappers.ResourceKey(txtNewTGI.Text);
                listView1.Items.Clear();
                for (int i = 0; i < simgeomfile.simgeom.keytable.keys.Count; i++)
                {
                    ListViewItem item = new ListViewItem();
                    item.Text = "TGI #" + i.ToString();
                    item.SubItems.Add(simgeomfile.simgeom.keytable.keys[i].ToString());
                    listView1.Items.Add(item);
                    item = null;
                }
                //showGbEntry(cmbChooseGeomEntry.SelectedIndex);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtNewTGI.Text) == false && MadScienceSmall.Helpers.validateKey(txtNewTGI.Text))
            {
                MadScience.Wrappers.ResourceKey rKey = new MadScience.Wrappers.ResourceKey(txtNewTGI.Text);
                simgeomfile.simgeom.keytable.keys.Add(rKey);
                ListViewItem item = new ListViewItem();
                item.Text = "TGI #" + listView1.Items.Count.ToString();
                item.SubItems.Add(rKey.ToString());
                listView1.Items.Add(item);
                //rKey = null;
            }
        }

        private void lstEntries_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstEntries.SelectedItems.Count == 1)
            {
                showMtnfEntry(lstEntries.SelectedIndices[0]);
            }
        }

        private void showMtnfEntry(int entryNo)
        {
            MTNFEntry entry = this.simgeomfile.simgeom.mtnfChunk.entries[entryNo];
            txtMtnfDataType.Text = entry.dataType.ToString();
            txtMtnfCount.Text = entry.dataCount.ToString();

            txtMtnfBox1.Text = "";
            txtMtnfBox2.Text = "";
            txtMtnfBox3.Text = "";
            txtMtnfBox4.Text = "";

            if (entry.dataCount >= 1)
            {
                switch (entry.dataType)
                {
                    case 4:
                    case 2:
                        txtMtnfBox1.Text = entry.dwords[0].ToString();
                        break;
                    case 1:
                        txtMtnfBox1.Text = entry.floats[0].ToString();
                        break;
                }
            }
            if (entry.dataCount >= 2)
            {
                switch (entry.dataType)
                {
                    case 4:
                    case 2:
                        txtMtnfBox2.Text = entry.dwords[1].ToString();
                        break;
                    case 1:
                        txtMtnfBox2.Text = entry.floats[1].ToString();
                        break;
                }
            }
            if (entry.dataCount >= 3)
            {
                switch (entry.dataType)
                {
                    case 4:
                    case 2:
                        txtMtnfBox3.Text = entry.dwords[2].ToString();
                        break;
                    case 1:
                        txtMtnfBox3.Text = entry.floats[2].ToString();
                        break;
                }
            }
            if (entry.dataCount >= 4)
            {
                switch (entry.dataType)
                {
                    case 4:
                    case 2:
                        txtMtnfBox4.Text = entry.dwords[3].ToString();
                        break;
                    case 1:
                        txtMtnfBox4.Text = entry.floats[3].ToString();
                        break;
                }
            }

        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (lstEntries.SelectedItems.Count == 1)
            {
                MTNFEntry entry = this.simgeomfile.simgeom.mtnfChunk.entries[lstEntries.SelectedIndices[0]];

                switch (entry.dataType)
                {
                    case 4:
                    case 2:
                        if (entry.dataCount >= 1 && !String.IsNullOrEmpty(txtMtnfBox1.Text)) { entry.dwords[0] = Convert.ToUInt32(txtMtnfBox1.Text); }
                        if (entry.dataCount >= 2 && !String.IsNullOrEmpty(txtMtnfBox2.Text)) { entry.dwords[1] = Convert.ToUInt32(txtMtnfBox2.Text); }
                        if (entry.dataCount >= 3 && !String.IsNullOrEmpty(txtMtnfBox3.Text)) { entry.dwords[2] = Convert.ToUInt32(txtMtnfBox3.Text); }
                        if (entry.dataCount >= 4 && !String.IsNullOrEmpty(txtMtnfBox4.Text)) { entry.dwords[3] = Convert.ToUInt32(txtMtnfBox4.Text); }
                        break;
                    case 1:
                        if (entry.dataCount >= 1 && !String.IsNullOrEmpty(txtMtnfBox1.Text)) { entry.floats[0] = Convert.ToSingle(txtMtnfBox1.Text); }
                        if (entry.dataCount >= 2 && !String.IsNullOrEmpty(txtMtnfBox2.Text)) { entry.floats[1] = Convert.ToSingle(txtMtnfBox2.Text); }
                        if (entry.dataCount >= 3 && !String.IsNullOrEmpty(txtMtnfBox3.Text)) { entry.floats[2] = Convert.ToSingle(txtMtnfBox3.Text); }
                        if (entry.dataCount >= 4 && !String.IsNullOrEmpty(txtMtnfBox4.Text)) { entry.floats[3] = Convert.ToSingle(txtMtnfBox4.Text); }
                        break;
                }

                this.simgeomfile.simgeom.mtnfChunk.entries[lstEntries.SelectedIndices[0]] = entry;
            }
        }

        private void exportMTNFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "MTNF File|*.mtnf";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //this.filename = saveFileDialog1.FileName;
                //toolStripStatusLabel1.Text = this.filename;
                Stream saveFile = File.Open(saveFileDialog1.FileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                this.simgeomfile.simgeom.mtnfChunk.Save(saveFile);
                saveFile.Close();
            }
        }

        private void importMTNFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "MTNF File|*.mtnf";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Stream input = File.OpenRead(openFileDialog1.FileName);
                this.simgeomfile.simgeom.mtnfChunk.Load(input);
                input.Close();

                exportMTNFToolStripMenuItem.Enabled = true;

                showEntries();
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            uint newBoneId = Convert.ToUInt32(txtBoneSet.Text);
            for (int i = 0; i < simgeomfile.simgeom.bones.Count; i++)
            {
                simgeomfile.simgeom.bones[i] = newBoneId;
            }

            showEntries();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Sim GEOM File|*.simgeom";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Stream input = File.OpenRead(openFileDialog1.FileName);
                SimGeomFile tempGeom = new SimGeomFile(input);
                this.simgeomfile.simgeom.boneHashes.Clear();
                for (int i = 0; i < tempGeom.simgeom.boneHashes.Count; i++)
                {
                    this.simgeomfile.simgeom.boneHashes.Add(tempGeom.simgeom.boneHashes[i]);
                }
                input.Close();
                tempGeom = null;

                showEntries();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {
            Console.WriteLine(this.simgeomfile.simgeom.embeddedId.ToString("X4"));
            int num = 0;
            foreach (uint fieldHash in Enum.GetValues(typeof(ShaderType)))
            {
                num++;
                if (num == comboBox1.SelectedIndex)
                {
                    simgeomfile.simgeom.embeddedId = fieldHash;
                    break;
                }
            }
            Console.WriteLine(this.simgeomfile.simgeom.embeddedId.ToString("X4"));
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}
