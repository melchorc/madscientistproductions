using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace PatchLogViewer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Figure out Temp path
            string tempPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Temp\\";
            bool validDir = false;

            validDir = Directory.Exists(tempPath);
            if (!validDir)
            {
                tempPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\..\\Temp\\";
                validDir = Directory.Exists(tempPath);
            }

            string lastPatchLog = "";
            DateTime lastPatchDate = new DateTime();

            if (validDir)
            {
                // Check each of the folders here to see if there is a {C05D8CDB-417D-4335-A38C-A0659EDFD6B8} folder underneath
                DirectoryInfo tempDir = new DirectoryInfo(tempPath);
                DirectoryInfo[] tempDirFiles = tempDir.GetDirectories();
                foreach (DirectoryInfo d in tempDirFiles)
                {
                    //ListViewItem item = new ListViewItem();
                    //item.Text = "Checking inside " + d.Name;

                    if (File.Exists(d.FullName + "\\{C05D8CDB-417D-4335-A38C-A0659EDFD6B8}\\rtpatch.log"))
                    {
                        FileInfo patchLog = new FileInfo(d.FullName + "\\{C05D8CDB-417D-4335-A38C-A0659EDFD6B8}\\rtpatch.log");
                        if (patchLog.LastWriteTime > lastPatchDate)
                        {
                            lastPatchDate = patchLog.LastWriteTime;
                            lastPatchLog = patchLog.FullName;
                        }
                        //item.SubItems.Add("Patch log");
                    }
                    //listView1.Items.Add(item);

                }

                Console.WriteLine(lastPatchLog);
                Console.WriteLine(lastPatchDate.ToString());
                Console.WriteLine(tempPath);
            }

            //lastPatchLog = @"J:\Users\Stuart\AppData\Local\Temp\{139939F5-6E60-4895-8E81-A75E8A342F72}\{C05D8CDB-417D-4335-A38C-A0659EDFD6B8}\rtpatch.log";
            if (lastPatchLog == "")
            {
                ListViewItem item = new ListViewItem();
                item.Text = "Checking " + tempPath;
                listView1.Items.Add(item);
                item = new ListViewItem();
                item.Text = "Sorry, we couldn't find the last patch log on your system!";
                item.SubItems.Add("Error!");
                listView1.Items.Add(item);
            }
            else
            {
                StreamReader patchLogStream = new StreamReader(lastPatchLog);
                ListViewItem item = new ListViewItem();
                item.Text = lastPatchLog;
                item.SubItems.Add("Patch log found");
                listView1.Items.Add(item);
                item = new ListViewItem();
                int count = 0;
                int errors = 0;
                do
                {
                    count++;
                    string line = patchLogStream.ReadLine().Trim();
                    if (count == 1)
                    {
                        item.Text = line;
                    }
                    if (count == 2)
                    {
                        item.SubItems.Add(line);
                        if (line.StartsWith("Error"))
                        {
                            item.BackColor = Color.LightSalmon;
                            //item.ForeColor = Color.Firebrick;
                            errors++;
                        }
                        listView1.Items.Add(item);
                        item = new ListViewItem();
                        count = 0;
                    }
                } while (patchLogStream.Peek() != -1);

                if (errors == 0)
                {
                    item.Text = "No errors found!  This is good, and means the patch INSTALLED correctly.";
                    listView1.Items.Add(item);
                }
                else
                {
                    item.Text = errors.ToString() + " errors found.  You should restore this file from your original CD, or from a backup!";
                    listView1.Items.Add(item);
                }
                patchLogStream.Close();

            }

        }
    }
}
