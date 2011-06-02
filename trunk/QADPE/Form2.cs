using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Gibbed.Sims3.FileFormats;
using System.IO;

namespace qadpe
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        public metaEntry.typesToMeta lookupList;

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Sims 3 Package|*.package";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = openFileDialog1.FileName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Sims 3 Package|*.package";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox3.Text = openFileDialog1.FileName;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            progressBar1.Minimum = 0;
            progressBar1.Value = 0;

            textBox1.Text = "";

            string firstPath = textBox2.Text;
            FileInfo f1 = new FileInfo(firstPath);
            Stream firstStream = File.OpenRead(firstPath);
            Database firstDb = new Database(firstStream);

            string secondPath = textBox3.Text;
            FileInfo f2 = new FileInfo(secondPath);
            Stream secondStream = File.OpenRead(secondPath);
            Database secondDb = new Database(secondStream);

            textBox1.Text += firstStream.Length.ToString() + " bytes vs " + secondStream.Length.ToString() + " bytes" + Environment.NewLine;
            textBox1.Text += "Comparing " + f1.Name + " to " + f2.Name + Environment.NewLine;

            textBox1.Text += f1.Name + " has " + firstDb.Entries.Count.ToString() + Environment.NewLine;
            textBox1.Text += f2.Name + " has " + secondDb.Entries.Count.ToString() + Environment.NewLine;

            StringBuilder sb = new StringBuilder();

            progressBar1.Maximum = firstDb.Entries.Count;

            int numChanged = 0;
            int identical = 0;

            foreach (KeyValuePair<ResourceKey, Database.Entry> entry in firstDb.Entries)
            {
                progressBar1.Value++;
                if (secondDb.Entries.ContainsKey(entry.Key) == false)
                {
                    numChanged++;
                    sb.AppendLine("Only in " + f1.Name + ": " + entry.Key + " (Size: " + entry.Value.CompressedSize + ")" );
                }
            }

            progressBar1.Value = 0;
            progressBar1.Maximum = secondDb.Entries.Count;

            foreach (KeyValuePair<ResourceKey, Database.Entry> entry in secondDb.Entries)
            {
                progressBar1.Value++;
                if (firstDb.Entries.ContainsKey(entry.Key) == false)
                {
                    numChanged++;
                    sb.AppendLine("Only in " + f2.Name + ": " + entry.Key + " (Size: " + entry.Value.CompressedSize + ")");
                }
            }

            numChanged = 0;

            progressBar1.Value = 0;
            progressBar1.Maximum = firstDb.Entries.Count;

            System.Security.Cryptography.MD5CryptoServiceProvider cryptHandler;
            cryptHandler = new System.Security.Cryptography.MD5CryptoServiceProvider();

            foreach (KeyValuePair<ResourceKey, Database.Entry> entry in firstDb.Entries)
            {
                progressBar1.Value++;
                bool different = false;

                if (secondDb.Entries.ContainsKey(entry.Key) == false)
                {
                    // Check the second file to see if it has a chunk with the same type and group, but a different instance,
                    // but also has the same size and hash etc
                    foreach (KeyValuePair<ResourceKey, Database.Entry> entry2 in secondDb.Entries)
                    {
                        if (entry.Key.TypeId == entry2.Key.TypeId && entry.Key.GroupId == entry2.Key.GroupId && entry.Value.CompressedSize == entry2.Value.CompressedSize && entry.Value.DecompressedSize == entry2.Value.DecompressedSize)
                        {
                            identical++;
                            sb.AppendLine("Chunk " + progressBar1.Value.ToString() + ": Identical chunks: " + entry.Key + " and " + entry2.Key);
                            different = true;
                            break;
                        }
                    }
                    if (!different)
                    {
                        sb.AppendLine("Chunk " + progressBar1.Value.ToString() + ": Does not appear in second package " + entry.Key);
                    }
                    continue;
                }

                ResourceKey key = entry.Key;

                string diffReason = "";

                if (firstDb.Entries[key].DecompressedSize != secondDb.Entries[key].DecompressedSize)
                {
                    diffReason = "Size";
                    different = true;
                }
                else
                {

					//if (firstDb.Entries[key].


                    byte[] firstData = firstDb.GetResource(key);
                    byte[] secondData = secondDb.GetResource(key);

                    if (firstData.Length != secondData.Length)
                    {
                        diffReason = "Size2";
                        different = true;
                    }
                    else
                    {
                        byte[] hash1 = cryptHandler.ComputeHash(firstData);
                        byte[] hash2 = cryptHandler.ComputeHash(secondData);

                        if (hash1.ToString() != hash2.ToString())
                        {
                            diffReason = "Hash of contents";
                            different = true;
                        }
                    }
                }

                if (different == true)
                {
                    numChanged++;
                    sb.AppendLine("Chunk " + progressBar1.Value.ToString() + ": Differs in " + diffReason + ": " + key);
                }
                else
                {
                    identical++;
                    sb.AppendLine("Chunk " + progressBar1.Value.ToString() + ": Identical chunks: " + key);
                }
            }

            textBox1.Text += sb.ToString();
            textBox1.Text += identical.ToString() + " identical entries" + Environment.NewLine;
            textBox1.Text += numChanged.ToString() + " changed entries" + Environment.NewLine;

            progressBar1.Value = 0;
            secondStream.Close();
            firstStream.Close();

        }
    }
}
