using System;
using System.Windows.Forms;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Globalization;

namespace BoneDeltaEditor
{
    public partial class Form1 : Form
    {
        private string filename = "";

        private MadScience.Wrappers.BoneDeltaFile bdFile  = new MadScience.Wrappers.BoneDeltaFile();

        private bones boneList;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            TextReader r = new StreamReader(Application.StartupPath + "\\bones.xml");
            XmlSerializer s = new XmlSerializer(typeof(bones));
            this.boneList = (bones)s.Deserialize(r);
            r.Close();

            for (int i = 0; i < this.boneList.Items.Count; i++)
            {
                cmbBoneList.Items.Add(this.boneList.Items[i].name);
            }

            if (Environment.GetCommandLineArgs().Length > 1)
            {
                loadFile(Environment.GetCommandLineArgs()[1].ToString());
            }
            else
            {
                lstEntries.Items.Clear();
                saveToolStripMenuItem.Enabled = false;

                txtVersion.Text = this.bdFile.bonedelta.version.ToString();
                button2.Enabled = false;

                groupBox1.Enabled = false;
            }
        }

        private void loadFile(string filename)
        {

            this.filename = filename;

            toolStripStatusLabel1.Text = this.filename;

            // Deals with RAW chunks here...
            Stream input = File.OpenRead(filename);
            this.bdFile.Load(input);
            input.Close();

            saveToolStripMenuItem.Enabled = true;

            showEntries();

            txtVersion.Text = this.bdFile.bonedelta.version.ToString();

            button2.Enabled = false;
        }

        private void showEntries()
        {
            lstEntries.Items.Clear();

            if (this.bdFile.bonedelta.entries.Count == 0)
            {
                groupBox1.Enabled = false;
            }

            for (int i = 0; i < this.bdFile.bonedelta.entries.Count; i++)
            {
                string boneName = "";
                for (int j = 0; j < this.boneList.Items.Count; j++)
                {
                    if (this.boneList.Items[j].hash == bdFile.bonedelta.entries[i].boneHash.ToString("X8"))
                    {
                        boneName = boneList.Items[j].name;
                        break;
                    }
                }
                        ListViewItem item = new ListViewItem();
                        item.Text = i.ToString();

                if (String.IsNullOrEmpty(boneName)) boneName = "Bone not found";

                item.SubItems.Add(boneName);
                        lstEntries.Items.Add(item);


            }

        }

        private void rCOLHeaderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MadScience.Wrappers.RCOLHeaderEditor rForm = new MadScience.Wrappers.RCOLHeaderEditor();
            rForm.rcolHeader = this.bdFile.rcolHeader;
            if (rForm.ShowDialog() == DialogResult.OK)
            {
                this.bdFile.rcolHeader = rForm.rcolHeader;
            }
            rForm.Close();
            rForm = null;
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bdFile.rcolHeader = new MadScience.Wrappers.RcolHeader();
            bdFile.bonedelta = new MadScience.Wrappers.BoneDelta();

            foreach (Control control in this.Controls)
            {
                MadScienceSmall.Helpers.resetControl(control);
            }
            foreach (Control control in this.groupBox1.Controls)
            {
                MadScienceSmall.Helpers.resetControl(control);
            }
            lstEntries.Items.Clear();
            saveToolStripMenuItem.Enabled = false;

            txtVersion.Text = this.bdFile.bonedelta.version.ToString();
            button2.Enabled = false;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Bone Delta File|*.bonedelta";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                loadFile(openFileDialog1.FileName);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stream saveFile = File.Open(this.filename, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            saveBoneDelta(saveFile);
            saveFile.Close();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Bone Delta File|*.bonedelta";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.filename = saveFileDialog1.FileName;
                toolStripStatusLabel1.Text = this.filename;
                Stream saveFile = File.Open(this.filename, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                saveBoneDelta(saveFile);
                saveFile.Close();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void saveBoneDelta(Stream output)
        {
            this.bdFile.bonedelta.version = Convert.ToUInt32(txtVersion.Text);

            this.bdFile.Save(output);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.bdFile.bonedelta.entries.Add(new MadScience.Wrappers.BoneDeltaEntry());
            ListViewItem item = new ListViewItem();
            item.Text = (this.bdFile.bonedelta.entries.Count - 1).ToString();
            item.SubItems.Add("(None)");
            lstEntries.Items.Add(item);

            lstEntries.Items[lstEntries.Items.Count - 1].Selected = true;
            lstEntries.Items[lstEntries.Items.Count - 1].EnsureVisible();

            groupBox1.Enabled = true;

        }

        private void showEntry(int entryNo)
        {
            button2.Enabled = true;
            groupBox1.Enabled = true;

            string boneHash = this.bdFile.bonedelta.entries[entryNo].boneHash.ToString("X8");
            for (int i = 0; i < this.boneList.Items.Count; i++)
            {
                if (this.boneList.Items[i].hash == boneHash)
                {
                    cmbBoneList.SelectedIndex = i;
                    break;
                }
            }
            txtBoneHash.Text = boneHash;

            txtMinX.Text = this.bdFile.bonedelta.entries[entryNo].offset.x.ToString(CultureInfo.InvariantCulture);
            txtMinY.Text = this.bdFile.bonedelta.entries[entryNo].offset.y.ToString(CultureInfo.InvariantCulture);
            txtMinZ.Text = this.bdFile.bonedelta.entries[entryNo].offset.z.ToString(CultureInfo.InvariantCulture);
            txtMaxX.Text = this.bdFile.bonedelta.entries[entryNo].scale.x.ToString(CultureInfo.InvariantCulture);
            txtMaxY.Text = this.bdFile.bonedelta.entries[entryNo].scale.y.ToString(CultureInfo.InvariantCulture);
            txtMaxZ.Text = this.bdFile.bonedelta.entries[entryNo].scale.z.ToString(CultureInfo.InvariantCulture);
            txtQuatX.Text = this.bdFile.bonedelta.entries[entryNo].quat.x.ToString(CultureInfo.InvariantCulture);
            txtQuatY.Text = this.bdFile.bonedelta.entries[entryNo].quat.y.ToString(CultureInfo.InvariantCulture);
            txtQuatZ.Text = this.bdFile.bonedelta.entries[entryNo].quat.z.ToString(CultureInfo.InvariantCulture);
            txtQuatW.Text = this.bdFile.bonedelta.entries[entryNo].quat.w.ToString(CultureInfo.InvariantCulture);
        }

        private void btnEntryCommit_Click(object sender, EventArgs e)
        {
            if (lstEntries.SelectedItems.Count > 0)
            {
                MadScience.Wrappers.BoneDeltaEntry entry = this.bdFile.bonedelta.entries[lstEntries.SelectedIndices[0]];
                try
                {
                    lstEntries.SelectedItems[0].SubItems[1].Text = cmbBoneList.Text;

                    entry.boneHash = MadScience.StringHelpers.ParseHex32("0x" + txtBoneHash.Text);
                    entry.offset.x = Convert.ToSingle(txtMinX.Text, CultureInfo.InvariantCulture);
                    entry.offset.y = Convert.ToSingle(txtMinY.Text, CultureInfo.InvariantCulture);
                    entry.offset.z = Convert.ToSingle(txtMinZ.Text, CultureInfo.InvariantCulture);
                    entry.scale.x = Convert.ToSingle(txtMaxX.Text, CultureInfo.InvariantCulture);
                    entry.scale.y = Convert.ToSingle(txtMaxY.Text, CultureInfo.InvariantCulture);
                    entry.scale.z = Convert.ToSingle(txtMaxZ.Text, CultureInfo.InvariantCulture);
                    entry.quat.x = Convert.ToSingle(txtQuatX.Text, CultureInfo.InvariantCulture);
                    entry.quat.y = Convert.ToSingle(txtQuatY.Text, CultureInfo.InvariantCulture);
                    entry.quat.z = Convert.ToSingle(txtQuatZ.Text, CultureInfo.InvariantCulture);
                    entry.quat.w = Convert.ToSingle(txtQuatW.Text, CultureInfo.InvariantCulture);
                    this.bdFile.bonedelta.entries[lstEntries.SelectedIndices[0]] = entry;
                    entry = null;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in numbers " + ex.Message);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (lstEntries.SelectedItems.Count > 0)
            {
                int selected = lstEntries.SelectedIndices[0];
                this.bdFile.bonedelta.entries.RemoveAt(selected);

                showEntries();

                int toSelect = selected;
                if (selected > (lstEntries.Items.Count - 1))
                {
                    toSelect -= 1;
                }
                if (toSelect > -1)
                {
                    lstEntries.Items[toSelect].Selected = true;
                    lstEntries.Items[toSelect].EnsureVisible();
                }

            }
        }

        private void cmbBoneList_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtBoneHash.Text = this.boneList.Items[cmbBoneList.SelectedIndex].hash;
        }

        private void lstEntries_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstEntries.SelectedItems.Count  > 0)
            {
                showEntry(lstEntries.SelectedIndices[0]);
            }
        }

        private void btnEntryCopy_Click(object sender, EventArgs e)
        {
            if (lstEntries.SelectedItems.Count > 0)
            {
                // Applys the settings only to all bones
                for (int i = 0; i < this.bdFile.bonedelta.entries.Count; i++)
                {
                    if (i != lstEntries.SelectedItems[0].Index)
                    {
                        MadScience.Wrappers.BoneDeltaEntry entry = this.bdFile.bonedelta.entries[i];
                        entry.offset.x = Convert.ToSingle(txtMinX.Text, CultureInfo.InvariantCulture);
                        entry.offset.y = Convert.ToSingle(txtMinY.Text, CultureInfo.InvariantCulture);
                        entry.offset.z = Convert.ToSingle(txtMinZ.Text, CultureInfo.InvariantCulture);
                        entry.scale.x = Convert.ToSingle(txtMaxX.Text, CultureInfo.InvariantCulture);
                        entry.scale.y = Convert.ToSingle(txtMaxY.Text, CultureInfo.InvariantCulture);
                        entry.scale.z = Convert.ToSingle(txtMaxZ.Text, CultureInfo.InvariantCulture);
                        entry.quat.x = Convert.ToSingle(txtQuatX.Text, CultureInfo.InvariantCulture);
                        entry.quat.y = Convert.ToSingle(txtQuatY.Text, CultureInfo.InvariantCulture);
                        entry.quat.z = Convert.ToSingle(txtQuatZ.Text, CultureInfo.InvariantCulture);
                        entry.quat.w = Convert.ToSingle(txtQuatW.Text, CultureInfo.InvariantCulture);
                    }
                }
            }
        }

        private void txtBoneHash_TextChanged(object sender, EventArgs e)
        {
            if (lstEntries.SelectedItems.Count > 0)
            {
                MadScience.Wrappers.BoneDeltaEntry entry = this.bdFile.bonedelta.entries[lstEntries.SelectedIndices[0]];
                entry.boneHash = MadScience.StringHelpers.ParseHex32("0x" + txtBoneHash.Text);
                lstEntries.SelectedItems[0].SubItems[1].Text = cmbBoneList.Text;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            txtMinX.Text = (-Convert.ToSingle(txtMinX.Text)).ToString();
            txtMinY.Text = (-Convert.ToSingle(txtMinY.Text)).ToString();
            txtMinZ.Text = (-Convert.ToSingle(txtMinZ.Text)).ToString();
            txtMaxX.Text = (-Convert.ToSingle(txtMaxX.Text)).ToString();
            txtMaxY.Text = (-Convert.ToSingle(txtMaxY.Text)).ToString();
            txtMaxZ.Text = (-Convert.ToSingle(txtMaxZ.Text)).ToString();
            txtQuatX.Text = (-Convert.ToSingle(txtQuatX.Text)).ToString();
            txtQuatY.Text = (-Convert.ToSingle(txtQuatY.Text)).ToString();
            txtQuatZ.Text = (-Convert.ToSingle(txtQuatZ.Text)).ToString();
            txtQuatW.Text = (-Convert.ToSingle(txtQuatW.Text)).ToString();
        }

    }

    [System.Xml.Serialization.XmlRootAttribute()]
    public class bones
    {
        [System.Xml.Serialization.XmlElementAttribute("bone", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = true)]
        public List<bone> Items = new List<bone>();

    }

    public class bone
    {
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name;

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string hash;
    }

}
