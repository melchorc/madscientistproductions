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
		public FormWindowState PreviouseWindowState { get; private set; }
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
			PreviouseWindowState = WindowState;
        }

		protected override void OnLayout(LayoutEventArgs levent)
		{
			base.OnLayout(levent);
			PreviouseWindowState = WindowState;
		}

		public void Activate(bool restoreIfMinimized)
		{
			if (restoreIfMinimized && WindowState == WindowState)
			{
				WindowState = PreviouseWindowState == FormWindowState.Normal
										? FormWindowState.Normal : FormWindowState.Maximized;
			}
			Activate();
		}

		private void addFile(string filename)
		{
			FileInfo f = new FileInfo(filename);
			ListViewItem item = new ListViewItem();

			item.Text = f.Name;
			item.SubItems.Add(f.FullName);

			detective.isCorrupt = false;
			detective.isDisabled = false;
			MadScience.Detective.PackageType pType = detective.getType(f.FullName);
			item.SubItems.Add(detective.pType.ToString());
			item.SubItems.Add("");

			item.BackColor = pType.ToColor();

			listView1.Items.Add(item);

		}

		private void displayTotal()
		{
			string numWaiting = "";
			if (listView1.Items.Count == 0)
			{
				numWaiting = "No files";
			}
			else
			{
				if (listView1.Items.Count == 1)
				{
					numWaiting = "One file";
				}
				else
				{
					numWaiting = listView1.Items.Count.ToString() + " files";
				}
			}
			lblHeader.Text = numWaiting + " waiting for install";

		}

		public void AppendArgs(string[] args)
		{
			if (args == null) return;

			for (int i = 1; i < args.Length; i++)
			{
				addFile(args[i].ToString());
			}

			displayTotal();

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

			if (MadScience.Helpers.gamesInstalled.gameCheck == 0)
			{
				lblHeader.Text = "Error: Cannot find Sims 3!";
				showMessage("Error: Cannot find Sims 3 game.", Color.Salmon);
				showMessage("The Helper Monkey WILL NOT work correctly!", Color.Salmon);
				//showMessage("Please use the Framework Installer to install the correct framework.", Color.Salmon);
			}
			else
			{
				for (int i = 0; i < MadScience.Helpers.gamesInstalled.Items.Count; i++)
				{
					if (MadScience.Helpers.gamesInstalled.Items[i].isInstalled)
					{
						comboBox1.Items.Add(MadScience.Helpers.gamesInstalled.Items[i].gameName);
					}
				}

				if (Environment.GetCommandLineArgs().Length > 1)
				{
					for (int i = 1; i < Environment.GetCommandLineArgs().Length; i++)
					{
						addFile(Environment.GetCommandLineArgs().GetValue(i).ToString());
					}
					displayTotal();
				}
				else
				{

					checkAndLoadTemp();

					if (listView1.Items.Count == 0)
					{
						label1.Text = "Right click and choose 'Send To -> Sims 3 as Custom Content', or double click a package file to use this program correctly.";
						//showMessage("Right click and choose 'Send To -> Sims 3 as Custom Content'");
						//showMessage("or double click a package file to use this program correctly.");
						studioShieldButton1.Enabled = false;
						comboBox1.Enabled = false;
						treeView1.Enabled = false;
						label2.Enabled = false;
					}
				}

				
			}

			//label1.Text = "Select the game folder to install these packages to, using the dropdown 1) Install To.";

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
				case "High-End Loft Stuff":
					baseFolder = MadScience.Helpers.gamesInstalled.Items[2].path;
					break;
			}
			if (String.IsNullOrEmpty(baseFolder)) return "";

			baseFolder = Path.Combine(baseFolder, Path.Combine("Mods", "Packages"));
			if (treeView1.SelectedNode != null)
			{
				baseFolder = Path.Combine(baseFolder, treeView1.SelectedNode.Tag.ToString());
			}

			return baseFolder;
		}

		private string copyFileToFolder(string filename)
		{
			string retVal = "";
			toolStripStatusLabel1.Text = "Copying " + filename;
			try
			{
				string copyTo = getInstallFolder();
				if (!String.IsNullOrEmpty(copyTo))
				{
					FileInfo f = new FileInfo(filename);
					copyTo = Path.Combine(copyTo, f.Name);

					//MessageBox.Show(filename + " -- " + copyTo);

					// Are we trying to copy to the same place as the original?
					if (filename == copyTo)
					{
						retVal = "Copy failed: You can't copy a file to itself. :)";
						return retVal;
					}

					try
					{

						// First, check if any existing backups exist...
						if (File.Exists(copyTo + ".backup"))
						{
							File.Delete(copyTo + ".backup");
						}

						// If an existing file was found, move that to backup...
						if (File.Exists(copyTo))
						{
							File.Move(copyTo, copyTo + ".backup");
						}

						// Now finally copy the file in...
						File.Copy(filename, copyTo);

						// Check if it copied...
						if (File.Exists(copyTo))
						{
							// Copied ok!
							retVal = "";
						}
						else
						{
							// Copy failed
							if (File.Exists(copyTo + ".backup"))
							{
								// Move backup back
								File.Move(copyTo + ".backup", copyTo);
							}
						}

					}
					catch (Exception ex)
					{
						// Copy failed
						if (File.Exists(copyTo + ".backup"))
						{
							// Move backup back
							File.Move(copyTo + ".backup", copyTo);
						}
						retVal = "Copy failed: " + ex.Message;
					}
				}
				else
				{
					retVal = "Copy folder is blank!";
				}
			}
			catch (Exception ex)
			{
				retVal = ex.Message;
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
				case "High-End Loft Stuff":
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
				studioShieldButton1.Enabled = false;
				treeView1.Enabled = false;
				label3.Enabled = false;
				linkLabel1.Visible = true;
				lblHeader.Text = "No Framework found!";
				treeView1.Nodes.Add("No Framework found!");
				return;
			}

			picAccept.Visible = true;
			picRemove.Visible = false;
			studioShieldButton1.Enabled = true;
			treeView1.Enabled = true;
			label3.Enabled = true;
			displayTotal();
			linkLabel1.Visible = false;

			label1.Text = "Now, select the sub-folder where you want to install the packages to, or choose '<Base Mods\\Packages>'.  When done, click 3) Install to install the files to your game.";

			treeView1.Nodes.Clear();

			string basePath = Path.Combine(MadScience.Helpers.findSims3Root(epNumber), Path.Combine("Mods", "Packages"));
			string gameName = Helpers.getGameName(epNumber);

			TreeNode rootNode = new TreeNode("<" + gameName + "\\Mods\\Packages>");
			rootNode.Tag = "";
			treeView1.Nodes.Add(rootNode);

			foreach (string d in Directory.GetDirectories(basePath))
			{
				DirectoryInfo dir = new DirectoryInfo(d);
				TreeNode dNode = new TreeNode(dir.Name);
				dNode.Tag = dir.Name;
				rootNode.Nodes.Add(dNode);

				foreach (string e in Directory.GetDirectories(d))
				{
					DirectoryInfo dirE = new DirectoryInfo(e);
					TreeNode eNode = new TreeNode(dirE.Name);
					eNode.Tag = Path.Combine(dNode.Tag.ToString(), dirE.Name);
					dNode.Nodes.Add(eNode);

					foreach (string f in Directory.GetDirectories(e))
					{
						DirectoryInfo dirF = new DirectoryInfo(f);
						TreeNode fNode = new TreeNode(dirF.Name);
						fNode.Tag = Path.Combine(eNode.Tag.ToString(), dirF.Name);
						eNode.Nodes.Add(fNode);

						foreach (string g in Directory.GetDirectories(f))
						{
							DirectoryInfo dirG = new DirectoryInfo(g);
							TreeNode gNode = new TreeNode(dirG.Name);
							gNode.Tag = Path.Combine(fNode.Tag.ToString(), dirG.Name);
							fNode.Nodes.Add(gNode);
						}
					}
				}
			}
			rootNode.Expand();
			treeView1.SelectedNode = rootNode;
			treeView1.Select();
			

		}

		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start("http://www.modthesims.info/d/384014");
		}

		private void studioShieldButton1_EscalationSuccessful(StudioControls.Controls.EscalationGoal escalationGoal, object sender, EventArgs e)
		{
		}

		private void FindByTag(string tagSearch)
		{
			TreeNodeCollection nodes = treeView1.Nodes;
			foreach (TreeNode n in nodes)
			{
				if (n.Tag.ToString() == tagSearch)
				{
					treeView1.SelectedNode = n;
					treeView1.SelectedNode.EnsureVisible();
					treeView1.Select();
					break;
				}
				else
				{
					if (FindRecursiveTag(tagSearch, n)) break;
				}
			}
		}

		private bool FindRecursiveTag(string tagSearch, TreeNode treeNode)
		{
			bool retVal = false;
			foreach (TreeNode tn in treeNode.Nodes)
			{
				if (tn.Tag.ToString() == tagSearch)
				{
					treeView1.SelectedNode = tn;
					treeView1.SelectedNode.EnsureVisible();
					treeView1.Select();

					retVal = true;
					break;
				}
				else
				{
					if (FindRecursiveTag(tagSearch, tn)) break;
				}
			}
			return retVal;
		}

		private bool checkAndLoadTemp()
		{
			//File.Create(Path.Combine(Path.GetTempPath(), "monkeyfiles.tmp"));

			try
			{
				string filename = Path.Combine(Path.GetTempPath(), "monkeyfiles.tmp");
				if (File.Exists(filename))
				{
					//MessageBox.Show("Loading temporary file...");
					FileInfo f = new FileInfo(filename);
					DateTime dtnow = DateTime.Now;
					DateTime f5m = f.CreationTime.AddMinutes(2.0d);
					int blah = f5m.CompareTo(dtnow);
					if (blah == 1)
					{

						listView1.Items.Clear();

						Stream input = File.OpenRead(filename);
						StreamReader reader = new StreamReader(input);

						int numItems = Convert.ToInt32(reader.ReadLine());
						comboBox1.SelectedIndex = Convert.ToInt32(reader.ReadLine());
						string selectedNode = reader.ReadLine();
						FindByTag(selectedNode);

						//listBox1.SelectedIndex = Convert.ToInt32(reader.ReadLine());
						for (int i = 0; i < numItems; i++)
						{
							addFile(reader.ReadLine());
						}
						reader.Close();
						input.Close();
						File.Delete(filename);

						installAllFiles();

						return true;
					}
					else
					{
						File.Delete(filename);
						return false;
					}
					return false;
				}
				else
				{
					//MessageBox.Show("No temporary file found...");
					return false;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			return false;
		}

		private void installAllFiles()
		{
			displayTotal();

			label1.Text = "Now, select the sub-folder where you want to install the packages to, or choose '<Base Mods\\Packages>'.  When done, click 3) Install to install the files to your game.";

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
							listView1.Items[i].SubItems[3].Text = "Not a package file";
							listView1.Items[i].BackColor = Color.Salmon;
							break;
						default:
							string copyResult = copyFileToFolder(filePath);
							if (copyResult == "")
							{
								listView1.Items[i].SubItems[3].Text = "File copied OK";
								numInstalled++;
							}
							else
							{
								listView1.Items[i].SubItems[3].Text = copyResult;
								listView1.Items[i].BackColor = Color.Salmon;
							}
							break;
					}
				}
				else
				{
					listView1.Items[i].SubItems[3].Text = pType.ToString();
					listView1.Items[i].BackColor = Color.Salmon;
				}
			}
			lblHeader.Text = numInstalled.ToString() + " of " + listView1.Items.Count.ToString() + " files installed";

			if (numInstalled < listView1.Items.Count)
			{
				picAccept.Visible = false;
				picRemove.Visible = true;
			}
			else
			{
				picRemove.Visible = false;
				picAccept.Visible = true;
			}

			toolStripProgressBar1.Value = toolStripProgressBar1.Minimum;
			toolStripStatusLabel1.Text = "Any files NOT in white in the list are not installed!";

		}

		private void studioShieldButton1_EscalationStarting(StudioControls.Controls.EscalationGoal escalationGoal, object sender, EventArgs e)
		{
			// Save the contents of the listView to a temporary file
			this.PreviouseWindowState = WindowState;

			string filename = Path.Combine(Path.GetTempPath(), "monkeyfiles.tmp");

			if (File.Exists(filename))
			{
				File.Delete(filename);
			}

			FileStream stream = new FileStream(filename, FileMode.Create, FileAccess.Write);
			StreamWriter writer = new StreamWriter(stream);

			writer.WriteLine(listView1.Items.Count.ToString());
			writer.WriteLine(comboBox1.SelectedIndex.ToString());
			//writer.WriteLine(listBox1.SelectedIndex.ToString());
			//MessageBox.Show(treeView1.SelectedNode.Tag.ToString());
			writer.WriteLine(treeView1.SelectedNode.Tag.ToString());
			for (int i = 0; i < listView1.Items.Count; i++)
			{
				writer.WriteLine(listView1.Items[i].SubItems[1].Text);
			}

			writer.Close();
			stream.Close();
		}

		private void studioShieldButton1_Click(object sender, EventArgs e)
		{
			if (StudioControls.Controls.UACUtilities.HasAdminPrivileges())
			{
				installAllFiles();
			}
		}

		private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start("http://www.modthesims.info/d/387006");
		}

		private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			button1.Enabled = true;
		}

		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			button1.Enabled = true;
		}

	}
}
