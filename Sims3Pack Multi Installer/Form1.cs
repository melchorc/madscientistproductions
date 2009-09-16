using System;
using System.Windows.Forms;

using System.IO;
using System.Collections;
using Microsoft.Win32;

namespace Sims3Pack_Multi_Installer
{
    public partial class Form1 : Form
    {
        private Stack fileList = new Stack();
        private int filesToProcess;
        
        public Form1()
        {
            InitializeComponent();

            // Check for registry key
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\Mad Scientist Productions\\Sims3PackMultiInstaller", true);
            if (key == null)
            {
                key = Registry.CurrentUser.CreateSubKey("Software\\Mad Scientist Productions\\Sims3PackMultiInstaller");
            }

            if (key.GetValue("sourceFolder") != null)
            {
                txtSourceFolder.Text = key.GetValue("sourceFolder").ToString();
            }
            else
            {
                key.SetValue("sourceFolder", "");
            }
            if (key.GetValue("destFolder") != null)
            {
                txtDestinationFolder.Text = key.GetValue("destFolder").ToString();
            }
            else
            {
                key.SetValue("destFolder", "");
            }

            if (key.GetValue("cleanNames") != null)
            {
                if (key.GetValue("cleanNames").ToString() == "True") checkBox1.Checked = true;
                else checkBox1.Checked = false;
            }

            key.Close();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog1.Description = "Select the folder that contains your downloaded Sims3Pack files...";
            if (txtSourceFolder.Text != "") { FolderBrowserDialog1.SelectedPath = txtSourceFolder.Text; }
            FolderBrowserDialog1.ShowDialog();
            if (FolderBrowserDialog1.SelectedPath != "") {
                txtSourceFolder.Text = FolderBrowserDialog1.SelectedPath;
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog1.Description = "Select the folder where you want to place the files extracted from the Sims3Packs...";
            if (txtDestinationFolder.Text != "") { FolderBrowserDialog1.SelectedPath = txtDestinationFolder.Text; }
            FolderBrowserDialog1.ShowDialog();
            if (FolderBrowserDialog1.SelectedPath != "")
            {
                txtDestinationFolder.Text = FolderBrowserDialog1.SelectedPath;
            }

        }

        public void ProcessFiles()
        {
            ProgressBar1.Minimum = 0;

            DateTime start = new DateTime();
            start = DateTime.Now;

            int numFiles = this.fileList.Count;
            string filename = "";

            ProgressBar1.Maximum = numFiles + 1;

            for (int i = 0; i < numFiles; i++)
            {
                try
                {
                    filename = this.fileList.Pop().ToString();

                    Sims3Pack s3pack = new Sims3Pack();
                    if (s3pack.load(filename) == true)
                    {
                        // Okay so now we should have a list of Sims3PackFileInfo stuff in the packagedFiles Array
                            //Sims3PackFileInfo s3fileinfo = new Sims3PackFileInfo();

                        FileInfo f = new FileInfo(filename);

                        toolStripStatusLabel1.Text = "Extracting " + f.Name;
                        statusStrip1.Refresh();

                        for (int j = 0; j < s3pack.packagedFiles.Count; j++)
                        {
                            Sims3PackFileInfo s3fileinfo = (Sims3PackFileInfo)s3pack.packagedFiles[j];
                            // Only install .package files
                            string extension = s3fileinfo.name.Substring(s3fileinfo.name.LastIndexOf("."));
                            //Console.WriteLine(s3fileinfo.name + " " + extension);
                            if (extension.ToLower() == ".package")
                            {
                                if (s3pack.extractFile(s3fileinfo, txtDestinationFolder.Text, checkBox1.Checked) == true)
                                {

                                }
                            }
                        }
                       
                        ProgressBar1.Value++;

                        
                    }
                    s3pack = null;

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

            double filesPerSecond = Math.Round((double)(numFiles / timeTaken), 2);

            toolStripStatusLabel1.Text = "Done " + numFiles + " files in " + timeTaken + " seconds with " + filesPerSecond + " files per second.";
            ProgressBar1.Value = 0;

        }

        public void CountFiles(string sDir)
        {
            try
            {
                /*
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    CountFiles(d);
                }
                */

                DirectoryInfo dir = new DirectoryInfo(sDir);
                FileInfo[] myFiles = dir.GetFiles("*.Sims3Pack");
                filesToProcess += myFiles.Length;
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

        private void Button3_Click(object sender, EventArgs e)
        {
            if ((txtSourceFolder.Text != "") && (txtDestinationFolder.Text != ""))
            {
                // Save folders in registry
                RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\Mad Scientist Productions\\Sims3PackMultiInstaller", true);
                key.SetValue("sourceFolder", txtSourceFolder.Text);
                key.SetValue("destFolder", txtDestinationFolder.Text);
                if (checkBox1.Checked) key.SetValue("cleanNames", "True");
                else key.SetValue("cleanNames", "False");

                key.Close();

                CountFiles(txtSourceFolder.Text);
                if (this.fileList.Count > 0)
                {
                    ProcessFiles();
                }
                else
                {
                    toolStripStatusLabel1.Text = "No Sims3Packs found in folder.";
                }
            }
        }

    }
}
