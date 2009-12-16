using System;
using System.Windows.Forms;
using Microsoft.Win32;
using System.IO;
using System.Diagnostics;

namespace FrameworkInstaller
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //return;
        }

        private bool checkFileLocations(string checkPath, string fullBuild)
        {

			if (String.IsNullOrEmpty(checkPath)) return false;

            textBox1.Text = "";

			textBox1.Text += "Checking path " + checkPath + "..." + Environment.NewLine;

            // Check for resource.cfg presence
            bool hasFoundResource = false;
            textBox1.Text += "Found resource.cfg: ";
            if (File.Exists(checkPath + "\\resource.cfg"))
            {
                hasFoundResource = true;
                textBox1.Text += "Yes";
            }
            else
            {
                textBox1.Text += "No";
            }
            textBox1.Text += Environment.NewLine;

            // Check for Mods\\Packages folder
            bool hasFoundModsPackages = false;
            textBox1.Text += @"Found Mods\Packages folder: ";
            if (Directory.Exists(checkPath + "\\Mods\\Packages\\"))
            {
                hasFoundModsPackages = true;
                textBox1.Text += "Yes";
            }
            else
            {
                textBox1.Text += "No";
            }
            textBox1.Text += Environment.NewLine;

            textBox1.Text += @"Found FullBuild0: ";
            
            if (File.Exists(Path.Combine(checkPath, MadScience.Helpers.getGameSubPath("\\GameData\\Shared\\Packages\\" + fullBuild + ".package"))))
            {
                textBox1.Text += "Yes";
            }
            else
            {
                textBox1.Text += "No";
            }
            textBox1.Text += Environment.NewLine;

            //textBox1.Text += Environment.NewLine;

            if (!hasFoundModsPackages && !hasFoundResource)
            {
                textBox1.Text += @"The Installer could not find either a Resource.cfg or a Mods\Packages folder! This means that your game WILL NOT currently accept custom content and installation via the Helper Monkey";
				textBox1.Text += Environment.NewLine;
				textBox1.Text += @"Please install the Framework by clicking the Go button.";
            }
            if (!hasFoundModsPackages && hasFoundResource)
            {
                textBox1.Text += @"The Installer found a Resource.cfg but not the Mods\Packages folder!  This means that your game WILL NOT currently accept custom content, and installation via the Helper Monkey will fail.";
				textBox1.Text += Environment.NewLine;
				textBox1.Text += @"Please install the Framework by clicking the Go button.";
            }
            if (hasFoundModsPackages && !hasFoundResource)
            {
                textBox1.Text += @"The Installer found a Mods\Packages folder but not Resource.cfg!  This means that your game WILL NOT currently accept custom content, and any custom packages will not show up in game.";
				textBox1.Text += Environment.NewLine;
				textBox1.Text += @"Please install the Framework by clicking the Go button.";
            }
            if (hasFoundModsPackages && hasFoundResource)
            {
                textBox1.Text += @"The Installer found both a Mods\Packages folder and the Resource.cfg.  This means that your game SHOULD accept custom content, and the Helper Monkey should work correctly.";
				textBox1.Text += Environment.NewLine;
				return true;
            }

			return false;

        }
 

        private void Form1_Load(object sender, EventArgs e)
        {
			this.Show();

			string sims3root = MadScience.Helpers.findSims3Root();
			if (sims3root == "")
			{
				textBox1.Text += "Error: Cannot find Sims 3 root." + Environment.NewLine;
				textBox1.Text += "The Helper Monkey WILL NOT work correctly!" + Environment.NewLine;
				textBox1.Text += "Click Change to find the correct Sims 3 root folder" + Environment.NewLine;

				button2.Enabled = false;
			}
			else
			{
				textBox2.Text = sims3root;

				checkFileLocations(sims3root, "FullBuild0");

				textBox3.Text = MadScience.Helpers.findSims3Root(" World Adventures", "FullBuildEP1", false);
				checkFileLocations(textBox3.Text, "FullBuildEP1");

			}

			if (String.IsNullOrEmpty(textBox2.Text) && String.IsNullOrEmpty(textBox3.Text))
			{
				button2.Enabled = false;
			}

        }

        private void button1_Click(object sender, EventArgs e)
        {
            MadScience.Helpers.setSims3Root();
            textBox2.Text = MadScience.Helpers.findSims3Root();
			button2.Enabled = !checkFileLocations(textBox2.Text, "FullBuild0");
        }

        private void button2_Click(object sender, EventArgs e)
        {
			textBox1.Text = "";

			// Copy the DLL to each of the folders Game\Bin locations
				if (!String.IsNullOrEmpty(textBox2.Text))
				{
					createFramework(textBox2.Text);
				}
				if (!String.IsNullOrEmpty(textBox3.Text))
				{
					createFramework(textBox3.Text);
				}
        }

		private bool createFramework(string path)
		{

			bool retValue = true;

			if (File.Exists(Path.Combine(Application.StartupPath, "d3dx9_31.dll")))
			{
				textBox1.Text += "Attempting to copy DLL file to " + path + "\\Game\\Bin...";
				File.Copy(Path.Combine(Application.StartupPath, "d3dx9_31.dll"), Path.Combine(path, "Game\\Bin\\d3dx9_31.dll"), true);
				if (File.Exists(Path.Combine(path, "Game\\Bin\\d3dx9_31.dll")))
				{
					textBox1.Text += "Succeeded!" + Environment.NewLine;
				}
				else
				{
					textBox1.Text += "Failed!" + Environment.NewLine;
					retValue = false;
				}
			}
			textBox1.Text += "Attempting to create Mods\\Packages folder...";
			Directory.CreateDirectory(Path.Combine(path, "Mods"));
			Directory.CreateDirectory(Path.Combine(path, "Mods\\Packages"));

			if (Directory.Exists(Path.Combine(path, "Mods\\Packages"))) 
			{
				textBox1.Text += "Succeeded!" + Environment.NewLine;
			}
			else
			{
				textBox1.Text += "Failed!" + Environment.NewLine;
				retValue = false;
			}

			if (File.Exists(Path.Combine(Application.StartupPath, "resource.cfg")))
			{

				textBox1.Text += "Attempting to copy resource.cfg file to " + path + "...";
				File.Copy(Path.Combine(Application.StartupPath, "resource.cfg"), Path.Combine(path, "resource.cfg"), true);
				if (File.Exists(Path.Combine(path, "resource.cfg")))
				{
					textBox1.Text += "Succeeded!" + Environment.NewLine;
				}
				else
				{
					textBox1.Text += "Failed!" + Environment.NewLine;
					retValue = false;
				}
			}
			return retValue;
		}

        private void button3_Click(object sender, EventArgs e)
        {
            MadScience.Helpers.setSims3Root(" World Adventures", "FullBuildEp1");
            textBox2.Text = MadScience.Helpers.findSims3Root(" World Adventures", "FullBuildEp1", false);

			button2.Enabled = !checkFileLocations(textBox2.Text, "FullBuildEP1");

        }
    }
}
