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

            foreach (uint fieldHash in Enum.GetValues(typeof(ShaderType)))
            {
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

            int num = -1;
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
            txtMtnfData.Text = "";
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < entry.dataCount; i++)
            {
                switch (entry.dataType)
                {
                    case 4:
                    case 2:
                        sb.AppendLine(entry.dwords[i].ToString());
                        break;
                    case 1:
                        sb.AppendLine(entry.floats[i].ToString());
                        break;
                }
                
            }
            txtMtnfData.Text = sb.ToString();
            sb = null;
        }
    }
}
