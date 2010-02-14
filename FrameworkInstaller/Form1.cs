﻿using System;
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

        private void Form1_Load(object sender, EventArgs e)
        {

			Version vrs = new Version(Application.ProductVersion);
			this.Text = this.Text + " v" + vrs.Major + "." + vrs.Minor + "." + vrs.Build + "." + vrs.Revision;

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

				MadScience.Helpers.messages.Clear();

				if (MadScience.Helpers.hasFramework(sims3root, MadScience.Helpers.GameNumber.baseGame) == false)
				{
					chkFrameworkTS3.Checked = false;
				}
				else
				{
					chkFrameworkTS3.Checked = true;
				}

				textBox3.Text = MadScience.Helpers.findSims3Root(MadScience.Helpers.GameNumber.worldAdventures, false);
				if (String.IsNullOrEmpty(textBox3.Text))
				{
					chkHasWA.Checked = false;
				}
				else
				{
					chkHasWA.Checked = true;
				}
				if (MadScience.Helpers.hasFramework(textBox3.Text, MadScience.Helpers.GameNumber.worldAdventures) == false)
				{
					chkFrameworkWA.Checked = false;
				}
				else
				{
					chkFrameworkWA.Checked = true;
				}

				textBox1.Text = MadScience.Helpers.findSims3Root(MadScience.Helpers.GameNumber.highEndLoftStuff, false);
				if (String.IsNullOrEmpty(textBox1.Text))
				{
					chkHasHELS.Checked = false;
				}
				else
				{
					chkHasHELS.Checked = true;
				}
				if (MadScience.Helpers.hasFramework(textBox1.Text, MadScience.Helpers.GameNumber.highEndLoftStuff) == false)
				{
					chkFrameworkHELS.Checked = false;
				}
				else
				{
					chkFrameworkHELS.Checked = true;
				}

				if (MadScience.Helpers.messages.Count > 0)
				{
					listView1.Visible = false;
					int numErrors = 0;
					for (int i = 0; i < MadScience.Helpers.messages.Count; i++)
					{
						if (MadScience.Helpers.messages[i].BackColor == Color.Salmon)
						{
							numErrors++;
						}
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
				}

			}

			int gameCheck = 0;
			if (chkHasTS3.Checked) gameCheck += 1;
			if (chkHasWA.Checked) gameCheck += 2;
			if (chkHasHELS.Checked) gameCheck += 4;

			int flagCheck = 0;
			if (chkFrameworkTS3.Checked) flagCheck += 1;
			if (chkFrameworkWA.Checked) flagCheck += 2;
			if (chkFrameworkHELS.Checked) flagCheck += 4;

			if (flagCheck == gameCheck)
			{
				picAccept.Visible = true;
				picRemove.Visible = false;
				disableFrameworkToolStripMenuItem.Enabled = true;
			}
			else
			{
				picAccept.Visible = false;
				picRemove.Visible = true;
				disableFrameworkToolStripMenuItem.Enabled = true;
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
			if (!chkFrameworkHELS.Checked)
			{
				createFramework(textBox1.Text);
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
            MadScience.Helpers.setSims3Root(MadScience.Helpers.GameNumber.worldAdventures);
			textBox3.Text = MadScience.Helpers.findSims3Root(MadScience.Helpers.GameNumber.worldAdventures, true);

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
			if (chkFrameworkHELS.Checked)
			{
				disableFramework(textBox1.Text);
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

    }
}