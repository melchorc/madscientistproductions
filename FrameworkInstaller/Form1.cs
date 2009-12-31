using System;
using System.Windows.Forms;
using Microsoft.Win32;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;

namespace FrameworkInstaller
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //return;
        }

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

        private bool checkFileLocations(string checkPath, string fullBuild)
        {

			if (String.IsNullOrEmpty(checkPath)) return false;

			ListViewItem item = new ListViewItem();



			showMessage("Checking path " + checkPath);

            // Check for resource.cfg presence
            bool hasFoundResource = false;
            if (File.Exists(checkPath + "\\resource.cfg"))
            {
                hasFoundResource = true;
				showMessage("Found resource.cfg: Yes");
            }
            else
            {
				showMessage("Found resource.cfg: No", Color.Salmon);
			}

            // Check for Mods\\Packages folder
            bool hasFoundModsPackages = false;
            if (Directory.Exists(checkPath + "\\Mods\\Packages\\"))
            {
                hasFoundModsPackages = true;
				showMessage(@"Found Mods\Packages folder: Yes");
			}
            else
            {
				showMessage(@"Found Mods\Packages folder: No", Color.Salmon);
            }

            if (File.Exists(Path.Combine(checkPath, MadScience.Helpers.getGameSubPath("\\GameData\\Shared\\Packages\\" + fullBuild + ".package"))))
            {
				showMessage(@"Found " + fullBuild + ": Yes");
			}
            else
            {
				showMessage(@"Found " + fullBuild + ": No", Color.Salmon);
            }

			bool hasFoundDLLFramework = false;
			if (File.Exists(Path.Combine(checkPath, MadScience.Helpers.getGameSubPath("\\Game\\Bin\\d3dx9_31.dll"))))
			{
				showMessage(@"Found custom framework d3dx9_31.dll: Yes");
				hasFoundDLLFramework = true;
			}
			else
			{
				showMessage(@"Found custom framework d3dx9_31.dll: No", Color.Salmon);
			}

            //textBox1.Text += Environment.NewLine;

            if (!hasFoundModsPackages && !hasFoundResource)
            {
				showMessage(@"The Installer could not find either a Resource.cfg or a Mods\Packages folder! This means that your game WILL NOT currently accept custom content and installation via the Helper Monkey", Color.Salmon);
				showMessage(@"Please install the Framework by clicking the Install button.");
            }
            if (!hasFoundModsPackages && hasFoundResource)
            {
				showMessage(@"The Installer found a Resource.cfg but not the Mods\Packages folder!  This means that your game WILL NOT currently accept custom content, and installation via the Helper Monkey will fail.", Color.Salmon);
				showMessage(@"Please install the Framework by clicking the Install button.");
            }
            if (hasFoundModsPackages && !hasFoundResource)
            {
				showMessage(@"The Installer found a Mods\Packages folder but not Resource.cfg!  This means that your game WILL NOT currently accept custom content, and any custom packages will not show up in game.", Color.Salmon);
				showMessage(@"Please install the Framework by clicking the Install button.");
            }
            if (hasFoundModsPackages && hasFoundResource && !hasFoundDLLFramework)
            {
				showMessage(@"The Installer found both a Mods\Packages folder and the Resource.cfg, but NOT a custom DLL.  This means that your game SHOULD accept basic custom content, but NOT core mods.", Color.Salmon);
            }
			if (hasFoundModsPackages && hasFoundResource && hasFoundDLLFramework)
			{
				showMessage(@"The Installer found all Framework components.  This means that your game SHOULD accept basic custom content, and also core mods.");
				return true;
			}

			return false;

        }
 

        private void Form1_Load(object sender, EventArgs e)
        {
			this.Show();

			checkLocations();
        }

		private void checkLocations()
		{
			string sims3root = MadScience.Helpers.findSims3Root();
			if (sims3root == "")
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
				textBox2.Text = sims3root;
				chkHasTS3.Checked = true;

				if (checkFileLocations(sims3root, "FullBuild0") == false)
				{
					chkFrameworkTS3.Checked = false;
				}
				else
				{
					chkFrameworkTS3.Checked = true;
				}

				textBox3.Text = MadScience.Helpers.findSims3Root(" World Adventures", "FullBuildEP1", false);
				if (String.IsNullOrEmpty(textBox3.Text))
				{
					chkHasWA.Checked = false;
				}
				else
				{
					chkHasWA.Checked = true;
				}
				if (checkFileLocations(textBox3.Text, "FullBuildEP1") == false)
				{
					chkFrameworkWA.Checked = false;
				}
				else
				{
					chkFrameworkWA.Checked = true;
				}

			}

			if (chkHasTS3.Checked && chkHasWA.Checked)
			{
				if (chkFrameworkTS3.Checked && chkFrameworkWA.Checked)
				{
					picAccept.Visible = true;
					picRemove.Visible = false;
					disableFrameworkToolStripMenuItem.Enabled = true;
				}
				if (chkFrameworkTS3.Checked && !chkFrameworkWA.Checked)
				{
					picAccept.Visible = false;
					picRemove.Visible = true;
					disableFrameworkToolStripMenuItem.Enabled = false;
				}

			}
			if (chkHasTS3.Checked && !chkHasWA.Checked)
			{
				if (chkFrameworkTS3.Checked)
				{
					picAccept.Visible = true;
					picRemove.Visible = false;
					disableFrameworkToolStripMenuItem.Enabled = true;
				}
				if (!chkFrameworkTS3.Checked)
				{
					picAccept.Visible = false;
					picRemove.Visible = true;
					disableFrameworkToolStripMenuItem.Enabled = false;
				}
			}

			if (picRemove.Visible)
			{
				label1.Text = "You do not have the framework installed in one or more of your Sims 3 locations!";
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
            textBox2.Text = MadScience.Helpers.findSims3Root();

			if (String.IsNullOrEmpty(textBox2.Text))
			{
				MessageBox.Show("No valid Sims 3 install could be found at that path");
			}

			checkLocations();
        }

        private void button2_Click(object sender, EventArgs e)
        {
			listView1.Items.Clear();
			// Copy the DLL to each of the folders Game\Bin locations
				if (!chkFrameworkTS3.Checked)
				{
					createFramework(textBox2.Text);
				}
				if (!chkFrameworkWA.Checked)
				{
					createFramework(textBox3.Text);
				}

				checkLocations();
        }

		private bool createFramework(string path)
		{

			bool retValue = true;

			if (String.IsNullOrEmpty(path)) return false;
			if (!Directory.Exists(path)) return false;

			showMessage("Installing framework to " + path);

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
			return retValue;
		}

        private void button3_Click(object sender, EventArgs e)
        {
            MadScience.Helpers.setSims3Root(" World Adventures", "FullBuildEp1");
            textBox3.Text = MadScience.Helpers.findSims3Root(" World Adventures", "FullBuildEp1", true);

			if (String.IsNullOrEmpty(textBox3.Text))
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
				disableFramework(textBox2.Text);
			}
			if (chkFrameworkWA.Checked)
			{
				disableFramework(textBox3.Text);
			}

			checkLocations();
		}

		private bool disableFramework(string path)
		{

			bool retValue = true;

			if (File.Exists(Path.Combine(path, "resource.cfg")))
			{

				if (File.Exists(Path.Combine(path, "resource.cfg.old")))
				{
					File.Delete(Path.Combine(path, "resource.cfg.old"));
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
				
			}
			return retValue;
		}

    }
}
