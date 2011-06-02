using System;
using System.Windows.Forms;
using Microsoft.Win32;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Collections;

namespace FrameworkInstaller
{
    public partial class Form1 : Form
    {
		bool needsUpgrade = false;

        public Form1()
        {
            InitializeComponent();
            //return;
        }

		private void showMessage(string message, Color backColor)
		{
			int priority = 0;
			if (backColor == Color.Salmon) priority = 3;

			MadScience.Helpers.logMessageToFile(message, priority);
			ListViewItem item = new ListViewItem();
			item.Text = message;
			item.BackColor = backColor;
			listView1.Items.Add(item);
			message = null;

			listView1.EnsureVisible(listView1.Items.Count - 1);

		}

		private void showMessage(string message)
		{
			showMessage(message, Color.White);
		}

        private void Form1_Load(object sender, EventArgs e)
        {

			Version vrs = new Version(Application.ProductVersion);
			this.Text = this.Text + " v" + vrs.Major + "." + vrs.Minor + "." + vrs.Build + "." + vrs.Revision;

			this.Show();

			MadScience.Helpers.logPath(Path.Combine(Application.StartupPath, "messages.log"), true);
			showMessage(this.Text);
			checkLocations();
        }

		private void checkLocations()
		{

			MadScience.Helpers.findInstalledGames();

			if (!MadScience.Helpers.gamesInstalled.baseGame.isInstalled)
			{
				showMessage("Error: Cannot find Sims 3 root.");
				showMessage("Custom content will NOT work correctly!");
				showMessage("Click Change to find the correct Sims 3 root folder");

				chkHasTS3.Checked = false;
				chkFrameworkTS3.Checked = false;
				button2.Enabled = false;
				disableFrameworkToolStripMenuItem.Enabled = false;
			}
			else
			{

				if (MadScience.Helpers.gamesInstalled.globalFramework.hasFramework)
				{
					MadScience.Helpers.messages.Clear();
					//textBox4.Text = MadScience.Helpers.gamesInstalled.globalFramework.path;
				}
				//listView1.Show();

				textBox1.Text = MadScience.Helpers.gamesInstalled.baseGame.path;
				textBox5.Text = MadScience.Helpers.gamesInstalled.baseGame.version.ToString(2);
				chkHasTS3.Checked = true;

				//MadScience.Helpers.messages.Clear();

				if (!MadScience.Helpers.gamesInstalled.baseGame.useGlobalFramework)
				{
					showMessage("Checking framework for The Sims 3");

					//if (MadScience.Helpers.hasFramework(sims3root, MadScience.Helpers.GameNumber.baseGame) == false)
					if (MadScience.Helpers.gamesInstalled.baseGame.hasFramework == false)
					{
						chkFrameworkTS3.Checked = false;
					}
					else
					{
						chkFrameworkTS3.Checked = true;
					}
				}
				if (MadScience.Helpers.gamesInstalled.baseGame.useGlobalFramework)
				{
					if (chkFrameworkTS3.Checked == true && !MadScience.Helpers.gamesInstalled.globalFramework.isInstalled)
					{
						this.needsUpgrade = true;
						button2.Text = "Upgrade";
					}
				}
				


				#region World Adventures
				textBox2.Text = MadScience.Helpers.gamesInstalled.worldAdventures.path;
				if (String.IsNullOrEmpty(textBox2.Text))
				{
					chkHasWA.Checked = false;
				}
				else
				{
					textBox6.Text = MadScience.Helpers.gamesInstalled.worldAdventures.version.ToString(2);
					chkHasWA.Checked = true;
				}
				if (chkHasWA.Checked)
				{
					if (!MadScience.Helpers.gamesInstalled.baseGame.useGlobalFramework)
					{
						showMessage("Checking framework for World Adventures");
						//if (MadScience.Helpers.hasFramework(textBox2.Text, MadScience.Helpers.GameNumber.worldAdventures) == false)
						if (MadScience.Helpers.gamesInstalled.worldAdventures.hasFramework)
						{
							chkFrameworkWA.Checked = false;
						}
						else
						{
							chkFrameworkWA.Checked = true;
						}
					}
					if (MadScience.Helpers.gamesInstalled.worldAdventures.useGlobalFramework)
					{
						if (chkFrameworkWA.Checked == true && !MadScience.Helpers.gamesInstalled.globalFramework.isInstalled)
						{
							this.needsUpgrade = true;
							button2.Text = "Upgrade";
						}

					}
				}
				#endregion

				#region High End Loft Stuff
				textBox3.Text = MadScience.Helpers.gamesInstalled.highEndLoftStuff.path;
				if (String.IsNullOrEmpty(textBox3.Text))
				{
					chkHasHELS.Checked = false;
				}
				else
				{
					textBox7.Text = MadScience.Helpers.gamesInstalled.highEndLoftStuff.version.ToString(2);
					chkHasHELS.Checked = true;
				}
				if (chkHasHELS.Checked)
				{
					if (!MadScience.Helpers.gamesInstalled.baseGame.useGlobalFramework)
					{
						showMessage("Checking framework for High End Loft Stuff");
						//if (MadScience.Helpers.hasFramework(textBox3.Text, MadScience.Helpers.GameNumber.highEndLoftStuff) == false)
						if (!MadScience.Helpers.gamesInstalled.highEndLoftStuff.hasFramework)
						{
							chkFrameworkHELS.Checked = false;
						}
						else
						{
							chkFrameworkHELS.Checked = true;
						}
					}

					//if (MadScience.Helpers.gamesInstalled.highEndLoftStuff.version.Major == 3 && MadScience.Helpers.gamesInstalled.highEndLoftStuff.version.Minor >= 3)
					if (MadScience.Helpers.gamesInstalled.highEndLoftStuff.useGlobalFramework)
					{
						if (chkFrameworkHELS.Checked == true && !MadScience.Helpers.gamesInstalled.globalFramework.isInstalled)
						{
							this.needsUpgrade = true;
							button2.Text = "Upgrade";

						}
					}
				}
				#endregion

				#region Ambitions
				textBox4.Text = MadScience.Helpers.gamesInstalled.globalFramework.path;
				//if (String.IsNullOrEmpty(textBox4.Text))
				if (MadScience.Helpers.gamesInstalled.ambitions.isInstalled)
				{
					chkHasAmb.Checked = false;
				}
				else
				{
					//showMessage("Checking framework for Ambitions");
					textBox8.Text = MadScience.Helpers.gamesInstalled.ambitions.version.ToString(2);
					//this.usesGlobalModsPackages = true;
					chkHasAmb.Checked = true;
				}

				//this.usesGlobalModsPackages = true;
				//MadScience.Helpers.gamesInstalled.globalFramework.hasFramework = true;

				//if (this.usesGlobalModsPackages && MadScience.Helpers.hasFramework(Path.Combine(MadScience.Helpers.findMyDocs(), Path.Combine(@"Electronic Arts", MadScience.Helpers.gamesInstalled.baseName)), MadScience.Helpers.GameNumber.ambitions) == false)
				//if (MadScience.Helpers.gamesInstalled.baseGame.useGlobalFramework || MadScience.Helpers.gamesInstalled.worldAdventures.useGlobalFramework || MadScience.Helpers.gamesInstalled.highEndLoftStuff.useGlobalFramework || MadScience.Helpers.gamesInstalled.ambitions.isInstalled)
				if (MadScience.Helpers.gamesInstalled.globalFramework.hasFramework)
				{
					showMessage("Checking My Documents for global framework");

					if (!MadScience.Helpers.gamesInstalled.globalFramework.isInstalled)
					{
						chkFrameworkAmb.Checked = false;
					}
					else
					{
						chkFrameworkAmb.Checked = true;
					}
				}
				#endregion
			}
			if (MadScience.Helpers.messages.Count > 0)
			{
				listView1.Visible = false;
				int numErrors = 0;
				int priority = 0;
				for (int i = 0; i < MadScience.Helpers.messages.Count; i++)
				{
					priority = 0;
					if (MadScience.Helpers.messages[i].BackColor == Color.Salmon)
					{
						priority = 3;
						numErrors++;
					}
					MadScience.Helpers.logMessageToFile(MadScience.Helpers.messages[i].Text, priority);
					listView1.Items.Add(MadScience.Helpers.messages[i]);
				}
				if (numErrors > 0)
				{
					listView1.Columns[0].Text = "Diagnostics - " + numErrors + " errors found";
				}
				else
				{
					listView1.Columns[0].Text = "Diagnostics";
				}
				listView1.Visible = true;
				listView1.EnsureVisible(listView1.Items.Count - 1);
			}



			int gameCheck = 0;
			int flagCheck = 0;

			if (!MadScience.Helpers.gamesInstalled.globalFramework.hasFramework)
			{
				if (chkHasTS3.Checked) gameCheck += 1;
				if (chkHasWA.Checked) gameCheck += 2;
				if (chkHasHELS.Checked) gameCheck += 4;
				if (chkFrameworkTS3.Checked) flagCheck += 1;
				if (chkFrameworkWA.Checked) flagCheck += 2;
				if (chkFrameworkHELS.Checked) flagCheck += 4;
			}
			else
			{
				gameCheck += 8;
				if (MadScience.Helpers.gamesInstalled.globalFramework.isInstalled) flagCheck += 8;

				if (!this.needsUpgrade)
				{
					// Hide the other framework boxes
					chkFrameworkTS3.Visible = false;
					chkFrameworkWA.Visible = false;
					chkFrameworkHELS.Visible = false;
				}
			}

			if (flagCheck == gameCheck)
			{
				// Do we have a global framework?
				if ((MadScience.Helpers.gamesInstalled.globalFramework.hasFramework && !chkFrameworkAmb.Checked) || this.needsUpgrade)
				{
					picAccept.Visible = false;
					picRemove.Visible = true;
					disableFrameworkToolStripMenuItem.Enabled = true;
				}
				else
				{
					picAccept.Visible = true;
					picRemove.Visible = false;
					disableFrameworkToolStripMenuItem.Enabled = true;
				}
			}
			else
			{
				picAccept.Visible = false;
				picRemove.Visible = true;
				disableFrameworkToolStripMenuItem.Enabled = true;
			}

			if (picRemove.Visible)
			{
				if (button2.Text == "Upgrade")
				{
					label1.Text = "You need to upgrade your Framework to the latest version!";
				}
				else
				{
					label1.Text = "You do not have the framework installed in one or more of your Sims 3 locations!";
				}
				button2.Enabled = true;
			}
			if (picAccept.Visible)
			{
				label1.Text = "Framework installation appears OK";
				button2.Enabled = false;

			}

		}

        private void button1_Click(object sender, EventArgs e)
        {
            MadScience.Helpers.setSims3Root();
            textBox1.Text = MadScience.Helpers.findSims3Root();

			if (String.IsNullOrEmpty(textBox1.Text))
			{
				MessageBox.Show("No valid Sims 3 install could be found at that path");
			}

			checkLocations();
        }

		public void CountFiles(string sDir)
		{
			try
			{

				foreach (string d in Directory.GetDirectories(sDir))
				{
					CountFiles(d);
				}


				DirectoryInfo dir = new DirectoryInfo(sDir);
				FileInfo[] myFiles = dir.GetFiles("*.package");
				filesToProcess += myFiles.Length;
				foreach (FileInfo f in myFiles)
				{
					fileInfo fi = new fileInfo();
					fi.filename = f.FullName;
					fi.foldername = this.masterDir;

					this.fileList.Push(fi);

					fi = null;
				}

				Application.DoEvents();

			}
			catch (System.Exception excpt)
			{
				Console.WriteLine(excpt.Message);
			}
		}

		private Stack fileList = new Stack();
		private int filesToProcess;
		private string masterDir;
		class fileInfo
		{
			public string filename = "";
			public string foldername = "";

		}


        private void button2_Click(object sender, EventArgs e)
        {
			listView1.Items.Clear();

			if (this.needsUpgrade)
			{
				if (!chkFrameworkAmb.Checked)
				{
					showMessage("Since you have a patch that supports a Global My Documents path, we need to move everything from the old folders to the new");
					showMessage("First though, lets disable the old framework since it's not needed anymore...");

					if (chkFrameworkTS3.Checked)
					{
						disableFramework(textBox1.Text);
					}
					if (chkFrameworkWA.Checked)
					{
						disableFramework(textBox2.Text);
					}
					if (chkFrameworkHELS.Checked)
					{
						disableFramework(textBox3.Text);
					}

					showMessage("Now attempt to install framework in My Documents...");
					string globalModPath = Path.Combine(MadScience.Helpers.findMyDocs(), Path.Combine(@"Electronic Arts", MadScience.Helpers.gamesInstalled.baseName));
					createFramework(globalModPath, false);
					globalModPath = Path.Combine(globalModPath, Path.Combine("Mods", "Packages"));

					checkLocations();

					// TODO:REMOVE
					chkFrameworkAmb.Checked = true;

					if (chkFrameworkAmb.Checked)
					{
						// Check for explicit Mods\Packages folder
						if (Directory.Exists(Path.Combine(MadScience.Helpers.gamesInstalled.baseGame.path, Path.Combine("Mods", "Packages"))))
						{
							this.masterDir = Path.Combine(MadScience.Helpers.gamesInstalled.baseGame.path, Path.Combine("Mods", "Packages"));
							CountFiles(this.masterDir);
						}
						// Check for explicit Mods\Packages folder
						if (Directory.Exists(Path.Combine(MadScience.Helpers.gamesInstalled.worldAdventures.path, Path.Combine("Mods", "Packages"))))
						{
							this.masterDir = Path.Combine(MadScience.Helpers.gamesInstalled.worldAdventures.path, Path.Combine("Mods", "Packages"));
							CountFiles(this.masterDir);
						}
						// Check for explicit Mods\Packages folder
						if (Directory.Exists(Path.Combine(MadScience.Helpers.gamesInstalled.highEndLoftStuff.path, Path.Combine("Mods", "Packages"))))
						{
							this.masterDir = Path.Combine(MadScience.Helpers.gamesInstalled.highEndLoftStuff.path, Path.Combine("Mods", "Packages"));
							CountFiles(this.masterDir);
						}


						if (this.fileList.Count > 0)
						{
							label1.Text = "Copying old files to new mods folder, please wait...";
							label1.Refresh();

							// Start the process....
							progressBar1.Maximum = this.fileList.Count;
							//string myDocs = MadScience.Helpers.findMyDocs();
							try
							{
								for (int i = 0; i < progressBar1.Maximum; i++)
								{
									fileInfo fi = (fileInfo)this.fileList.Pop();
									FileInfo f = new FileInfo(fi.filename);

									string folder = f.DirectoryName.Replace(fi.foldername, "");
									if (!String.IsNullOrEmpty(folder))
									{
										// create the directory
										if (!Directory.Exists(globalModPath + folder))
										{
											Directory.CreateDirectory(globalModPath + folder);
										}
									}
									//File.Move(f.filename, Path.Combine(globalModPath, fi.
									if (!File.Exists(Path.Combine(globalModPath + folder, f.Name)))
									{
										File.Copy(fi.filename, Path.Combine(globalModPath + folder, f.Name));
										//File.Move(fi.filename, Path.Combine(globalModPath + folder, f.Name));
									}
									progressBar1.Value++;
									progressBar1.Refresh();
								}
							}
							catch (Exception ex)
							{
								showMessage("An error occured while trying to copy files to your My Documents folder!", Color.Salmon);
								showMessage(ex.Message, Color.Salmon);
							}
						}
					}
				}
			}
			else
			{
				if (!MadScience.Helpers.gamesInstalled.globalFramework.hasFramework)
				{
					// Copy the DLL to each of the folders Game\Bin locations
					if (!chkFrameworkTS3.Checked)
					{
						createFramework(textBox1.Text);
					}
					if (!chkFrameworkWA.Checked)
					{
						createFramework(textBox2.Text);
					}
					if (!chkFrameworkHELS.Checked)
					{
						createFramework(textBox3.Text);
					}
				}
				else
				{
					if (!chkFrameworkAmb.Checked)
					{
						createFramework(Path.Combine(MadScience.Helpers.findMyDocs(), Path.Combine(@"Electronic Arts", MadScience.Helpers.gamesInstalled.baseName)), false);
					}
				}
			}
			checkLocations();
        }

		private bool createFramework(string path)
		{
			return createFramework(path, true);
		}

		private bool createFramework(string path, bool oldStyle)
		{

			bool retValue = true;

			if (String.IsNullOrEmpty(path)) return false;
			if (!Directory.Exists(path)) return false;

			showMessage("Installing framework to " + path);

			if (oldStyle)
			{
				if (File.Exists(Path.Combine(path, "Game\\Bin\\d3dx9_31.dll")))
				{
					showMessage("Custom Framework DLL already exists in Game\\Bin... Skipping install");
				}
				else
				{
					if (File.Exists(Path.Combine(Application.StartupPath, "d3dx9_31.dll")))
					{
						if (Directory.Exists(Path.Combine(path, "Game\\Bin")))
						{
							File.Copy(Path.Combine(Application.StartupPath, "d3dx9_31.dll"), Path.Combine(path, "Game\\Bin\\d3dx9_31.dll"), true);
							if (File.Exists(Path.Combine(path, "Game\\Bin\\d3dx9_31.dll")))
							{
								showMessage("Attempting to copy DLL file to " + path + "\\Game\\Bin... Succeeded!");
							}
							else
							{
								showMessage("Attempting to copy DLL file to " + path + "\\Game\\Bin... Failed!", Color.Salmon);
								retValue = false;
							}
						}
						else
						{
							showMessage("Could not find " + path + "\\Game\\Bin!", Color.Salmon);
							retValue = false;
						}
					}
				}
			}

			if (Directory.Exists(Path.Combine(path, "Mods\\DCCache")))
			{
				showMessage("Mods\\DCCache directory already exists... skipping");
			}
			else
			{
				try
				{
					Directory.CreateDirectory(Path.Combine(path, "Mods"));
					Directory.CreateDirectory(Path.Combine(path, "Mods\\DCCache"));
				}
				catch (Exception ex)
				{
				}
				if (Directory.Exists(Path.Combine(path, "Mods\\DCCache")))
				{
					showMessage("Attempting to create Mods\\DCCache folder... Succeeded!");
				}
				else
				{
					showMessage("Attempting to create Mods\\DCCache folder... Failed!", Color.Salmon);
					retValue = false;
				}
			}

			if (Directory.Exists(Path.Combine(path, "Mods\\Packages")))
			{
				showMessage("Mods\\Packages directory already exists... skipping");
			}
			else
			{
				try
				{
					Directory.CreateDirectory(Path.Combine(path, "Mods"));
					Directory.CreateDirectory(Path.Combine(path, "Mods\\Packages"));
				}
				catch (Exception ex)
				{
				}
				if (Directory.Exists(Path.Combine(path, "Mods\\Packages")))
				{
					showMessage("Attempting to create Mods\\Packages folder... Succeeded!");
				}
				else
				{
					showMessage("Attempting to create Mods\\Packages folder... Failed!", Color.Salmon);
					retValue = false;
				}
			}

			if (oldStyle)
			{
				if (File.Exists(Path.Combine(path, "resource.cfg")))
				{
					showMessage("Resource.cfg already exists... skipping");
				}
				else
				{
					if (File.Exists(Path.Combine(Application.StartupPath, "resource.cfg.original")))
					{
						showMessage("Attempting to copy resource.cfg file to " + path + "...");
						File.Copy(Path.Combine(Application.StartupPath, "resource.cfg.original"), Path.Combine(path, "resource.cfg"), true);
						if (File.Exists(Path.Combine(path, "resource.cfg")))
						{
							showMessage("Succeeded!");
						}
						else
						{
							showMessage("Failed!", Color.Salmon);
							retValue = false;
						}
					}
				}
			}
			else
			{
				if (File.Exists(Path.Combine(path, "Mods\\resource.cfg")))
				{
					showMessage("Resource.cfg already exists... skipping");
				}
				else
				{
					if (File.Exists(Path.Combine(Application.StartupPath, "resource.cfg.new")))
					{
						showMessage("Attempting to copy resource.cfg file to " + path + "\\Mods...");
						File.Copy(Path.Combine(Application.StartupPath, "resource.cfg.new"), Path.Combine(path, "Mods\\resource.cfg"), true);
						if (File.Exists(Path.Combine(path, "Mods\\resource.cfg")))
						{
							showMessage("Succeeded!");
						}
						else
						{
							showMessage("Failed!", Color.Salmon);
							retValue = false;
						}
					}
				}
			}
			return retValue;
		}

        private void button3_Click(object sender, EventArgs e)
        {
            MadScience.Helpers.setSims3Root(MadScience.Helpers.GameNumber.worldAdventures);
			textBox2.Text = MadScience.Helpers.findSims3Root(MadScience.Helpers.GameNumber.worldAdventures, true);

			if (String.IsNullOrEmpty(textBox2.Text))
			{
				MessageBox.Show("No valid World Adventures install could be found at that path");
			}
			checkLocations();

        }

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void disableFrameworkToolStripMenuItem_Click(object sender, EventArgs e)
		{
			listView1.Items.Clear();

			if (chkFrameworkTS3.Checked)
			{
				disableFramework(textBox1.Text);
			}
			if (chkFrameworkWA.Checked)
			{
				disableFramework(textBox2.Text);
			}
			if (chkFrameworkHELS.Checked)
			{
				disableFramework(textBox3.Text);
			}
			if (chkFrameworkAmb.Checked)
			{
				disableFramework(Path.Combine(MadScience.Helpers.findMyDocs(), @"Electronic Arts\" + MadScience.Helpers.gamesInstalled.baseName + "\\Mods"));
			}

			checkLocations();
		}

		private bool disableFramework(string path)
		{

			bool retValue = true;

			showMessage("Attempting to disable framework in " + path + "...");

			if (File.Exists(Path.Combine(path, "resource.cfg")))
			{

				if (File.Exists(Path.Combine(path, "resource.cfg.old")))
				{
					showMessage("Old resource.cfg found... attempting to delete....");
					try
					{
						File.Delete(Path.Combine(path, "resource.cfg.old"));
						if (!File.Exists(Path.Combine(path, "resource.cfg.old")))
						{
							showMessage("Old resource.cfg deleted successfully", Color.Green);
						}
						else
						{
							showMessage("Old resource.cfg not deleted!", Color.Salmon);
						}
					}
					catch (Exception ex)
					{
						showMessage("An error occurred deleting the old resource.cfg.  Do you have permissions?", Color.Salmon);
					}
				}


				File.Move(Path.Combine(path, "resource.cfg"), Path.Combine(path, "resource.cfg.old"));
				if (File.Exists(Path.Combine(path, "resource.cfg.old")))
				{
					showMessage("Attempting to rename resource.cfg file to resource.cfg.old...Succeeded");
				}
				else
				{
					showMessage("Attempting to rename resource.cfg file to resource.cfg.old...Failed!", Color.Salmon);
					retValue = false;
				}

				if (File.Exists(Path.Combine(path, Path.Combine("Game", Path.Combine("Bin", "d3dx9_31.dll")))))
				{
					showMessage("Attempting to remove core mod dll file");
					try
					{
						File.Delete(Path.Combine(path, Path.Combine("Game", Path.Combine("Bin", "d3dx9_31.dll"))));
						if (!File.Exists(Path.Combine(path, Path.Combine("Game", Path.Combine("Bin", "d3dx9_31.dll")))))
						{
							showMessage("Core mod DLL removed successfully");
						}
						else
						{
							showMessage("An error occured while attempting to delete the core mod dll file", Color.Salmon);
						}
					}
					catch (Exception ex)
					{
						showMessage("An error occured while attempting to delete the core mod dll file", Color.Salmon);
					}
				}


			}
			else
			{
				showMessage("No resource.cfg found in this location!", Color.Salmon);
			}
			return retValue;
		}

		private void button4_Click(object sender, EventArgs e)
		{
			MadScience.Helpers.setSims3Root(MadScience.Helpers.GameNumber.highEndLoftStuff);
			textBox3.Text = MadScience.Helpers.findSims3Root(MadScience.Helpers.GameNumber.highEndLoftStuff);

			if (String.IsNullOrEmpty(textBox3.Text))
			{
				MessageBox.Show("No valid High End Loft Stuff install could be found at that path");
			}
			checkLocations();
		}

		private void button5_Click(object sender, EventArgs e)
		{
			MadScience.Helpers.setSims3Root(MadScience.Helpers.GameNumber.ambitions);
			textBox4.Text = MadScience.Helpers.findSims3Root(MadScience.Helpers.GameNumber.ambitions);

			if (String.IsNullOrEmpty(textBox4.Text))
			{
				MessageBox.Show("No valid Ambitions install could be found at that path");
			}
			checkLocations();
		}

		private void rescanFrameworkToolStripMenuItem_Click(object sender, EventArgs e)
		{
			checkLocations();
		}

    }
}
