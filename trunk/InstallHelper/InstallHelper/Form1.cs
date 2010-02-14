using System;
using System.Windows.Forms;
using Microsoft.Win32;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Collections.Generic;
using MadScience;

namespace InstallHelper
{
    public partial class Form1 : Form
    {

		MadScience.Detective detective = new Detective();	

		private void showMessage(string message, Color backColor)
		{
			ListViewItem item = new ListViewItem();
			item.Text = message;
			item.BackColor = backColor;
			listView1.Items.Add(item);
			message = null;

		}

		private void showMessage(string message)
		{
			showMessage(message, Color.White);
		}

        public Form1()
        {
            InitializeComponent();
        }

        private bool copyFileToSims3(string sims3root)
        {
            string filename = "";
            string dirToInstall = "";
            string packagesDir = "\\Mods\\Packages\\";

            if (Environment.GetCommandLineArgs().Length <= 1)
            {
                return false;
            }
            if (Environment.GetCommandLineArgs().Length == 2)
            {
                filename = Environment.GetCommandLineArgs()[1].ToString();
                if (filename == "-uninstall")
                {
                    // We are running the uninstall routine
                    Registry.CurrentUser.DeleteSubKeyTree("Software\\Classes\\Sims3.Package\\shell\\Install to Sims 3");
                    Registry.CurrentUser.DeleteSubKeyTree("Software\\Classes\\Sims3.Package\\shell\\Install to Sims 3 Hacks");
                    Registry.CurrentUser.DeleteSubKeyTree("Software\\Classes\\Sims3.Package\\shell\\Install to Sims 3 Skins");
                    Registry.CurrentUser.DeleteSubKeyTree("Software\\Classes\\Sims3.Package\\shell\\Install to Sims 3 Misc");
                    Registry.CurrentUser.DeleteSubKeyTree("Software\\Classes\\Sims3.Package\\shell\\Install to Sims 3 Patterns");
                    Registry.CurrentUser.DeleteSubKeyTree("Software\\Classes\\Sims3.Package\\shell\\Open Packages folder");

                    return true;

                }
            }
            if (Environment.GetCommandLineArgs().Length == 3)
            {
                filename = Environment.GetCommandLineArgs()[1].ToString();
                dirToInstall = Environment.GetCommandLineArgs()[2].ToString();
            }

            /*
            string cliArgs = "";
            for (int i = 0; i < Environment.GetCommandLineArgs().Length; i++)
            {
                cliArgs += Environment.GetCommandLineArgs()[i].ToString() + " ++ ";
            }
            */

            bool noInstall = false;
            switch (dirToInstall)
            {
                case "-hacks":
                    dirToInstall = "Hacks\\";
                    break;
                case "-pattern":
                    dirToInstall = "Patterns\\";
                    break;
                case "-misc":
                    dirToInstall = "Misc\\";
                    break;
                case "-skins":
                    dirToInstall = "Skins\\";
                    break;
                case "-sim":
                    sims3root = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\Electronic Arts\\The Sims 3";
                    packagesDir = "";
                    dirToInstall = "SavedSims\\";
                    break;
                case "-openfolder":
                    Process p = new Process();
                    p.StartInfo.FileName = "explorer.exe";
                    p.StartInfo.Arguments = sims3root + packagesDir;
                    p.Start();

                    break;
                default:
                    dirToInstall = "";
                    break;
            }

            //MessageBox.Show(Environment.GetCommandLineArgs().Length + " -- " + cliArgs + " -- " + sims3root + " -- " + filename + " -- " + dirToInstall);

            // Get only filename
            string fName = filename.Substring(filename.LastIndexOf("\\") + 1);
            try
            {
                // Check if folder exists (user might have deleted it)
                if (Directory.Exists(sims3root + packagesDir + dirToInstall) == true)
                {
                    if (File.Exists(sims3root + packagesDir + dirToInstall + fName))
                    {
                        if (MessageBox.Show("The file " + fName + " already exists.  Do you want to overwrite?", "Overwrite file?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            File.Copy(filename, sims3root + packagesDir + dirToInstall + fName, true);
                        }
                    }
                    else
                    {
                        File.Copy(filename, sims3root + packagesDir + dirToInstall + fName);
                    }
                }
                else
                {
                    // Check normal mods/packages
                    if (Directory.Exists(sims3root + packagesDir) == true)
                    {
                        if (File.Exists(sims3root + packagesDir + fName))
                        {
                            if (MessageBox.Show("The file " + fName + " already exists.  Do you want to overwrite?", "Overwrite file?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                File.Copy(filename, sims3root + packagesDir + fName, true);
                            }
                        }
                        else
                        {
                            File.Copy(filename, sims3root + packagesDir + fName);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Could not find a valid directory to install to!", "TS3 Helper Monkey");
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "TS3 Helper Monkey");
                return false;
            }

            return true;
        }

		private void checkSendToShortcut()
		{
			string sendToPath = Environment.GetFolderPath(Environment.SpecialFolder.SendTo);

			DirectoryInfo dir = new DirectoryInfo(sendToPath);
			FileInfo[] myFiles = dir.GetFiles("*.lnk");
			bool hasFound = false;
			foreach (FileInfo f in myFiles)
			{
				if (f.Name == "Sims 3 as Custom Content.lnk")
				{
					hasFound = true;
					MadScience.Shortcut link = new MadScience.Shortcut();
					link.Load(f.FullName);
					if (link.Path != Application.ExecutablePath)
					{
						hasFound = false;
					}
					break;
				}
			}

			if (hasFound == false)
			{
				if (File.Exists(Path.Combine(Application.StartupPath, "Sims 3 as Custom Content.lnk")))
				{
					MadScience.Shortcut link = new MadScience.Shortcut(Path.Combine(Application.StartupPath, "Sims 3 as Custom Content.lnk"));
					link.Path = Application.ExecutablePath;
					link.Save(Path.Combine(sendToPath, "Sims 3 as Custom Content.lnk"));
				}
			}
		}

        private void Form1_Load(object sender, EventArgs e)
        {

			Version vrs = new Version(Application.ProductVersion);
			this.Text = this.Text + " v" + vrs.Major + "." + vrs.Minor + "." + vrs.Build + "." + vrs.Revision;

			checkSendToShortcut();

			MadScience.Helpers.findInstalledGames();

			if (!MadScience.Helpers.gamesInstalled.Items[0].isInstalled)
			{
				lblHeader.Text = "Error: Cannot find Sims 3 root";
				showMessage("Error: Cannot find Sims 3 root.", Color.Salmon);
				showMessage("The Helper Monkey WILL NOT work correctly!", Color.Salmon);
				//showMessage("Please use the Framework Installer to install the correct framework.", Color.Salmon);
			}
			else
			{

				comboBox1.Items.Add("Sims 3");

				if (MadScience.Helpers.gamesInstalled.Items[1].isInstalled)
				{
					comboBox1.Items.Add("World Adventures");
				}

				if (MadScience.Helpers.gamesInstalled.Items[2].isInstalled)
				{
					comboBox1.Items.Add("High End Loft Stuff");
				}

				if (Environment.GetCommandLineArgs().Length > 1)
				{
					for (int i = 1; i < Environment.GetCommandLineArgs().Length; i++)
					{
						FileInfo f = new FileInfo(Environment.GetCommandLineArgs().GetValue(i).ToString());
						ListViewItem item = new ListViewItem();

						item.Text = f.Name;
						item.SubItems.Add(f.FullName);

						detective.isCorrupt = false;
						detective.isDisabled = false;
						MadScience.Detective.PackageType pType = detective.getType(f.FullName);
						item.SubItems.Add(detective.pType.ToString());

						item.BackColor = pType.ToColor();

						listView1.Items.Add(item);
					}
					lblHeader.Text = listView1.Items.Count.ToString() + " file(s) waiting for install";
				}
				else
				{
					showMessage("Right click and choose 'Send To -> Sims 3 as Custom Content'");
					showMessage("or double click a package file to use this program correctly.");
					button2.Enabled = false;
					listBox1.Enabled = false;
					comboBox1.Enabled = false;
				}
			}

        }

		private string getInstallFolder()
		{
			//MadScience.Helpers.OpenWindow(Environment.GetFolderPath(Environment.SpecialFolder.SendTo));
			string baseFolder = "";
			switch (comboBox1.SelectedItem.ToString())
			{
				case "Sims 3":
					baseFolder = MadScience.Helpers.gamesInstalled.Items[0].path;
					break;
				case "World Adventures":
					baseFolder = MadScience.Helpers.gamesInstalled.Items[1].path;
					break;
				case "High End Loft Stuff":
					baseFolder = MadScience.Helpers.gamesInstalled.Items[2].path;
					break;
			}
			if (String.IsNullOrEmpty(baseFolder)) return "";

			baseFolder = Path.Combine(baseFolder, Path.Combine("Mods", "Packages"));
			if (listBox1.SelectedIndex > 0)
			{
				baseFolder = Path.Combine(baseFolder, listBox1.SelectedItem.ToString());
			}

			return baseFolder;
		}

		private bool copyFileToFolder(string filename)
		{
			bool retVal = false;
			toolStripStatusLabel1.Text = "Copying " + filename;
			try
			{
				string copyTo = getInstallFolder();
				if (!String.IsNullOrEmpty(copyTo))
				{
					FileInfo f = new FileInfo(filename);
					copyTo = Path.Combine(copyTo, f.Name);
					File.Copy(filename, copyTo, true);
					if (File.Exists(copyTo))
					{
						retVal = true;
					}
				}
			}
			catch (Exception ex)
			{

			}

			return retVal;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			MadScience.Helpers.OpenWindow(getInstallFolder());
		}

		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			switch (comboBox1.SelectedItem.ToString())
			{
				case "Sims 3":
					getRootFolders(Helpers.GameNumber.baseGame);
					break;
				case "World Adventures":
					getRootFolders(Helpers.GameNumber.worldAdventures);
					break;
				case "High End Loft Stuff":
					getRootFolders(Helpers.GameNumber.highEndLoftStuff);
					break;
			}
		}

		private void getRootFolders(Helpers.GameNumber epNumber)
		{

			if (!MadScience.Helpers.hasFramework(MadScience.Helpers.findSims3Root(epNumber), epNumber))
			{
				picAccept.Visible = false;
				picRemove.Visible = true;
				button2.Enabled = false;
				listBox1.Enabled = false;
				linkLabel1.Visible = true;
				lblHeader.Text = "There is no framework installed in that game!";
				return;
			}

			picAccept.Visible = true;
			picRemove.Visible = false;
			button2.Enabled = true;
			listBox1.Enabled = true;
			lblHeader.Text = listView1.Items.Count.ToString() + " file(s) waiting for install";
			linkLabel1.Visible = false;

			listBox1.Items.Clear();

			string basePath = Path.Combine(MadScience.Helpers.findSims3Root(epNumber), Path.Combine("Mods", "Packages"));

			listBox1.Items.Add("<Base Mods\\Packages>");

			foreach (string d in Directory.GetDirectories(basePath))
			{
				DirectoryInfo dir = new DirectoryInfo(d);
				listBox1.Items.Add(dir.Name);
			}

			listBox1.SelectedIndex = 0;

		}

		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start("http://www.modthesims.info/d/384014");
		}

		private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			button1.Enabled = true;
		}

		private void button2_Click(object sender, EventArgs e)
		{
			int numInstalled = 0;
			toolStripProgressBar1.Maximum = listView1.Items.Count;
			for (int i = 0; i < listView1.Items.Count; i++)
			{
				detective.isCorrupt = false;
				detective.isDisabled = false;

				toolStripProgressBar1.Value++;

				string filePath = listView1.Items[i].SubItems[1].Text;
				toolStripStatusLabel1.Text = "Checking " + filePath;


				MadScience.Detective.PackageType pType = detective.getType(filePath);

				if (!detective.isCorrupt)
				{
					switch (pType.MainType)
					{
						case Detective.PackageTypes.sims3Store:
						case Detective.PackageTypes.sims2Package:
						case Detective.PackageTypes.pngThumbnail:
							listView1.Items[i].BackColor = Color.Salmon;
							break;
						default:
							if (copyFileToFolder(filePath))
							{
								numInstalled++;
							}
							else
							{
								listView1.Items[i].BackColor = Color.Salmon;
							}
							break;
					}
				}
				else
				{
					listView1.Items[i].BackColor = Color.Salmon;
				}
			}
			lblHeader.Text = numInstalled.ToString() + " of " + listView1.Items.Count.ToString() + " files installed";
			toolStripProgressBar1.Value = toolStripProgressBar1.Minimum;
			toolStripStatusLabel1.Text = "Any files NOT in white in the list are not installed!";

		}

	}
}
