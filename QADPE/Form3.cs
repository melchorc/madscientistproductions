using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Gibbed.Sims3.FileFormats;

namespace qadpe
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            openFileDialog3.Filter = "DDS Files|*.dds|All Files|*.*";
            if (openFileDialog3.ShowDialog() == DialogResult.OK && openFileDialog3.FileName != "")
            {
                label9.Text = openFileDialog3.FileName;
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            openFileDialog3.Filter = "DDS Files|*.dds|All Files|*.*";
            if (openFileDialog3.ShowDialog() == DialogResult.OK && openFileDialog3.FileName != "")
            {
                label13.Text = openFileDialog3.FileName;
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            openFileDialog3.Filter = "DDS Files|*.dds|All Files|*.*";
            if (openFileDialog3.ShowDialog() == DialogResult.OK && openFileDialog3.FileName != "")
            {
                label14.Text = openFileDialog3.FileName;
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            openFileDialog3.Filter = "DDS Files|*.dds|All Files|*.*";
            if (openFileDialog3.ShowDialog() == DialogResult.OK && openFileDialog3.FileName != "")
            {
                label15.Text = openFileDialog3.FileName;
            }
        }

        private void addToDBPF(string filename, string typeID, string groupID, string instanceID, Database db)
        {
            Stream input = File.Open(filename, FileMode.Open, FileAccess.Read);
            ResourceKey rkey = new ResourceKey((ulong)Gibbed.Helpers.StringHelpers.ParseHex64(instanceID), (uint)Gibbed.Helpers.StringHelpers.ParseHex32(typeID), (uint)Gibbed.Helpers.StringHelpers.ParseHex32(groupID));
            db.SetResourceStream(rkey, input);
            input.Close();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Sims 3 Package|*.package";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK && saveFileDialog1.FileName != "")
            {

                Stream file = File.Open(saveFileDialog1.FileName, FileMode.Create, FileAccess.ReadWrite);
                Database db = new Database(file, false);

                if (label9.Text != "") addToDBPF(label9.Text, textBox9.Text, textBox11.Text, textBox5.Text, db);
                if (label13.Text != "") addToDBPF(label13.Text, textBox13.Text, textBox12.Text, textBox14.Text, db);
                if (label14.Text != "") addToDBPF(label14.Text, textBox16.Text, textBox15.Text, textBox17.Text, db);
                if (label15.Text != "") addToDBPF(label15.Text, textBox19.Text, textBox18.Text, textBox20.Text, db);
                if (label16.Text != "") addToDBPF(label16.Text, textBox22.Text, textBox21.Text, textBox23.Text, db);

                db.Commit(true);
                file.Close();

            }
        }

    }
}
