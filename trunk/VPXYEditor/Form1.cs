using System;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace VPXYEditor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private string filename = "";

        private MadScience.Wrappers.VPXYFile vpxyFile = new MadScience.Wrappers.VPXYFile();

        private void Form1_Load(object sender, EventArgs e)
        {
            if (Environment.GetCommandLineArgs().Length > 1)
            {
                loadFile(Environment.GetCommandLineArgs()[1].ToString());
            }
        }

        private void showLinkList()
        {
            listView2.Items.Clear();
            for (int i = 0; i < vpxyFile.vpxy.linkEntries.Count; i++)
            {
                MadScience.Wrappers.VPXYEntry entry = vpxyFile.vpxy.linkEntries[i];
                ListViewItem item = new ListViewItem();
                item.Text = i.ToString();
                item.SubItems.Add("List with " + entry.tgiList.Count.ToString() + " items");
                //item.SubItems.Add(entry.tgiIndex[0].ToString() + " " + vpxy.keytable.keys[(int)entry.tgiIndex[0]]);
                listView2.Items.Add(item);
                entry = null;
            }

            txtTypeZeroStart.Text = vpxyFile.vpxy.numTypeZero.ToString();
        }

        private void showSeprList()
        {
            listView3.Items.Clear();
            for (int i = 0; i < vpxyFile.vpxy.seprEntries.Count; i++)
            {
                MadScience.Wrappers.VPXYEntry entry = vpxyFile.vpxy.seprEntries[i];
                ListViewItem item = new ListViewItem();
                item.Text = i.ToString();
                item.SubItems.Add(entry.tgiList[0].ToString());
                //item.Text = vpxy.keytable.keys[(int)entry.tgiIndex[0]].ToString();
                listView3.Items.Add(item);
                entry = null;
            }

        }

        private void showBoundingBox()
        {
            txtMinX.Text = this.vpxyFile.vpxy.boundingbox.min.x.ToString();
            txtMinY.Text = this.vpxyFile.vpxy.boundingbox.min.y.ToString();
            txtMinZ.Text = this.vpxyFile.vpxy.boundingbox.min.z.ToString();
            txtMaxX.Text = this.vpxyFile.vpxy.boundingbox.max.x.ToString();
            txtMaxY.Text = this.vpxyFile.vpxy.boundingbox.max.y.ToString();
            txtMaxZ.Text = this.vpxyFile.vpxy.boundingbox.max.z.ToString();
        }

        private void loadFile(string filename)
        {

            this.filename = filename;
            this.vpxyFile.vpxy = new MadScience.Wrappers.VPXY();

            toolStripStatusLabel1.Text = this.filename;

            // Deals with RAW chunks here...
            Stream input = File.OpenRead(filename);
            this.vpxyFile.Load(input);
            input.Close();

            showLinkList();
            showSeprList();

            showBoundingBox();

            saveToolStripMenuItem.Enabled = true;

            Console.WriteLine("Done loading");
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.vpxyFile = new MadScience.Wrappers.VPXYFile();

            foreach (Control control in this.Controls)
            {
                MadScienceSmall.Helpers.resetControl(control);
            }
            foreach (Control control in this.groupBox1.Controls)
            {
                MadScienceSmall.Helpers.resetControl(control);
            }
            foreach (Control control in this.groupBox2.Controls)
            {
                MadScienceSmall.Helpers.resetControl(control);
            }
            foreach (Control control in this.groupBox3.Controls)
            {
                MadScienceSmall.Helpers.resetControl(control);
            }
            showBoundingBox();
            textBox1.Enabled = false;
            button5.Enabled = false;
            button4.Enabled = false;
            button2.Enabled = false;
            textBox2.Enabled = false;
            button7.Enabled = false;
            saveToolStripMenuItem.Enabled = false;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stream saveFile = File.Open(this.filename, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            saveVPXY(saveFile);
            saveFile.Close();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Visual Proxy File|*.proxy|Visual Proxy File|*.vpxy";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.filename = saveFileDialog1.FileName;
                toolStripStatusLabel1.Text = this.filename;
                Stream saveFile = File.Open(this.filename, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                saveVPXY(saveFile);
                saveFile.Close();
            }
        }

        private void saveVPXY(Stream output)
        {
            // Populate bounding box
            try
            {
                this.vpxyFile.vpxy.boundingbox.min.x = Convert.ToSingle(txtMinX.Text);
                this.vpxyFile.vpxy.boundingbox.min.y = Convert.ToSingle(txtMinY.Text);
                this.vpxyFile.vpxy.boundingbox.min.z = Convert.ToSingle(txtMinZ.Text);
                this.vpxyFile.vpxy.boundingbox.max.x = Convert.ToSingle(txtMaxX.Text);
                this.vpxyFile.vpxy.boundingbox.max.y = Convert.ToSingle(txtMaxY.Text);
                this.vpxyFile.vpxy.boundingbox.max.z = Convert.ToSingle(txtMaxZ.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            this.vpxyFile.Save(output);

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Visual Proxy File|*.proxy|Visual Proxy File|*.vpxy";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                loadFile(openFileDialog1.FileName);
            }

        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (listView3.SelectedItems.Count > 0)
            {
                // Get the VPXYEntry
                vpxyFile.vpxy.seprEntries.RemoveAt(listView3.SelectedIndices[0]);
                showSeprList();

                textBox1.Text = "";
                //showGbEntry(cmbChooseGeomEntry.SelectedIndex);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (listView3.SelectedItems.Count > 0)
            {
                if (MadScienceSmall.Helpers.validateKey(textBox1.Text))
                {
                    vpxyFile.vpxy.seprEntries[listView3.SelectedIndices[0]].tgiList[0] = new MadScience.Wrappers.ResourceKey(textBox1.Text);
                    listView3.SelectedItems[0].Text = vpxyFile.vpxy.seprEntries[listView3.SelectedIndices[0]].tgiList[0].ToString();
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox1.Text))
            {
                textBox1.Text = "key:00000000:00000000:0000000000000000";
            }
            if (MadScienceSmall.Helpers.validateKey(textBox1.Text))
            {
                MadScience.Wrappers.ResourceKey rKey = new MadScience.Wrappers.ResourceKey(textBox1.Text);
                MadScience.Wrappers.VPXYEntry entry = new MadScience.Wrappers.VPXYEntry();
                entry.type = 1;
                entry.tgiList.Add(rKey);

                vpxyFile.vpxy.seprEntries.Add(entry);
                
                
                ListViewItem item = new ListViewItem();
                item.Text = (listView3.Items.Count).ToString();
                item.SubItems.Add(rKey.ToString());
                listView3.Items.Add(item);
                item = null;

                entry = null;
                //rKey = null;
            }
        }

        private void listView3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView3.SelectedItems.Count > 0)
            {
                textBox1.Text = listView3.SelectedItems[0].SubItems[1].Text;
                textBox1.Enabled = true;
                button5.Enabled = true;
                button4.Enabled = true;
            }
        }

        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView2.SelectedItems.Count > 0)
            {
                textBox2.Text = "";
                StringBuilder sb = new StringBuilder();

                MadScience.Wrappers.VPXYEntry entry = this.vpxyFile.vpxy.linkEntries[listView2.SelectedIndices[0]];
                for (int i = 0; i < entry.tgiList.Count; i++)
                {
                    sb.AppendLine(entry.tgiList[i].ToString());
                }

                textBox2.Text = sb.ToString();

                button2.Enabled = true;
                textBox2.Enabled = true;
                button7.Enabled = true;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (listView2.SelectedItems.Count > 0)
            {
                //textBox2.Text = "";
                //StringBuilder sb = new StringBuilder();

                MadScience.Wrappers.VPXYEntry entry = this.vpxyFile.vpxy.linkEntries[listView2.SelectedIndices[0]];
                entry.tgiList.Clear();

                foreach (string line in textBox2.Lines)
                {
                    if (!String.IsNullOrEmpty(line.Trim()))
                    {
                        if (MadScienceSmall.Helpers.validateKey(line))
                        {
                            entry.tgiList.Add(new MadScience.Wrappers.ResourceKey(line));
                        }
                    }
                }

                listView2.SelectedItems[0].SubItems[1].Text = "List with " + entry.tgiList.Count.ToString() + " items";
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (listView2.SelectedItems.Count > 0)
            {
                this.vpxyFile.vpxy.linkEntries.RemoveAt(listView2.SelectedIndices[0]);
                showLinkList();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MadScience.Wrappers.VPXYEntry entry = new MadScience.Wrappers.VPXYEntry();
            this.vpxyFile.vpxy.linkEntries.Add(entry);

            showLinkList();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void rCOLHeaderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MadScience.Wrappers.RCOLHeaderEditor rForm = new MadScience.Wrappers.RCOLHeaderEditor();
            rForm.rcolHeader = this.vpxyFile.rcolHeader;
            if (rForm.ShowDialog() == DialogResult.OK)
            {
                this.vpxyFile.rcolHeader = rForm.rcolHeader;
            }
            rForm.Close();
            rForm = null;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
