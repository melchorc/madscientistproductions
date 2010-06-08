using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Win32;

namespace Sims3Dashboard
{
	public partial class Form1 : Form
	{

		public Form1()
		{
			InitializeComponent();

			string fastScan = MadScience.Helpers.getRegistryValue("fastScan");
			if (!String.IsNullOrEmpty(fastScan))
			{
				if (fastScan == "true") alwaysUseFastScanToolStripMenuItem.Checked = true;
			}

			string scanDisabled = MadScience.Helpers.getRegistryValue("scanDisabled");
			if (!String.IsNullOrEmpty(scanDisabled))
			{
				if (scanDisabled == "true") includedisabledInScanToolStripMenuItem.Checked = true;
			}

			string noAutoScan = MadScience.Helpers.getRegistryValue("noAutoScanOnStartup");
			if (!String.IsNullOrEmpty(noAutoScan))
			{
				if (noAutoScan == "true") noAutoScanOnStartupToolStripMenuItem.Checked = true;
			}

		}

		private Stack fileList = new Stack();
		private int filesToProcess;

		private bool inScan = false;

		private string masterDir = "";

		private List<ListViewItem> packageList = new List<ListViewItem>();
		private Dictionary<string, int> conflictList = new Dictionary<string, int>();

		private MadScience.Detective detective = new MadScience.Detective();

		private enum ConflictTypes : uint
		{
			//STBL = 0x220557DA,
			//TXTC = 0x033A1435,
			//MLOD = 0x01D10F34,
			OBJK = 0x02DC343F,
			OBJD = 0x319E4F1D,
			//VPXY = 0x736884F1,
			//_RIG = 0x8EAF13DE,
			S3SA = 0x073FAA07,
			_XML = 0x0333406C,
			CASP = 0x034AEECB,
			ITUN = 0x03B33DDF,
			JAZZ = 0x02D5DF13,
		}

		MadScience.ListViewSorter Sorter = new MadScience.ListViewSorter();

		private void Form1_Load(object sender, EventArgs e)
		{

			Version vrs = new Version(Application.ProductVersion);
			this.Text = this.Text + " v" + vrs.Major + "." + vrs.Minor + "." + vrs.Build + "." + vrs.Revision;

			checkLocations();
			getCacheDetails();

			this.Show();

			if (!noAutoScanOnStartupToolStripMenuItem.Checked)
			{
				showCustomContent();
			}
	
		}

		private void getCacheDetails()
		{
			string TS3MyDocs = MadScience.Helpers.getRegistryValue("cacheFolders");
			if (String.IsNullOrEmpty(TS3MyDocs))
			{

				string myDocs = MadScience.Helpers.findMyDocs();
				if (String.IsNullOrEmpty(myDocs)) return;

				TS3MyDocs = Path.Combine(Path.Combine(myDocs, "Electronic Arts"), "The Sims 3");
			}
			listView2.Items.Clear();

			showCacheInfo(Path.Combine(TS3MyDocs, "CASPartCache.package"), "CASPartCache");
			showCacheInfo(Path.Combine(TS3MyDocs, "compositorCache.package"), "compositorCache");
			showCacheInfo(Path.Combine(TS3MyDocs, "scriptCache.package"), "scriptCache");
			showCacheInfo(Path.Combine(TS3MyDocs, "simCompositorCache.package"), "simCompositorCache");

			DirectoryInfo dir;
			FileInfo[] myFiles;

			if (Directory.Exists(Path.Combine(TS3MyDocs, "WorldCaches")))
			{
				dir = new DirectoryInfo(Path.Combine(TS3MyDocs, "WorldCaches"));
				myFiles = dir.GetFiles("*.package");
				foreach (FileInfo f in myFiles)
				{
					showCacheInfo(f.FullName, "WorldCaches\\" + f.Name);
				}
			}

			if (Directory.Exists(Path.Combine(TS3MyDocs, "Thumbnails")))
			{
				dir = new DirectoryInfo(Path.Combine(TS3MyDocs, "Thumbnails"));
				myFiles = dir.GetFiles("*.package");
				foreach (FileInfo f in myFiles)
				{
					showCacheInfo(f.FullName, "Thumbnails\\" + f.Name);
				}
			}

			//showCacheInfo("", "");

		}

		private void showCacheInfo(string cachePath, string cacheName)
		{
			if (!String.IsNullOrEmpty(cachePath) && !File.Exists(cachePath)) return;


			// CAS Part Cache
			ListViewItem item = new ListViewItem();

			item.Text = cacheName;
			item.SubItems.Add(cachePath);

			if (!String.IsNullOrEmpty(cachePath))
			{
				if (!File.Exists(cachePath))
				{
					item.SubItems.Add("0");
					item.SubItems.Add("0");

				}
				else
				{

					FileInfo f = new FileInfo(cachePath);
					if (f.Length > 0)
					{
						item.SubItems.Add(((f.Length / 1024) + 1).ToString());
					}
					else
					{
						item.SubItems.Add("0");
					}

					Stream casPartCache = File.OpenRead(cachePath);
					MadScience.Wrappers.Database db = new MadScience.Wrappers.Database(casPartCache, true);
					item.SubItems.Add(db.dbpf.Entries.Count.ToString());
					casPartCache.Close();
				}
			}
			else
			{
				item.SubItems.Add("");
				item.SubItems.Add("");
			}

			listView2.Items.Add(item);
		}

		private void clearCache(string cachePath, string cacheName)
		{
			if (String.IsNullOrEmpty(cacheName))
			{
				// Clear ALL caches
				if (MessageBox.Show("You are about to clear all caches.  Are you sure?", "Clear All Caches", MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
				{
					string myDocs = MadScience.Helpers.findMyDocs();
					if (String.IsNullOrEmpty(myDocs)) return;

					string TS3MyDocs = Path.Combine(Path.Combine(myDocs, "Electronic Arts"), "The Sims 3");

					DirectoryInfo dir;
					FileInfo[] myFiles;

					if (Directory.Exists(Path.Combine(TS3MyDocs, "WorldCaches")))
					{
						dir = new DirectoryInfo(Path.Combine(TS3MyDocs, "WorldCaches"));
						myFiles = dir.GetFiles("*.package");
						foreach (FileInfo f in myFiles)
						{
							File.Delete(f.FullName);
						}
					}

					if (Directory.Exists(Path.Combine(TS3MyDocs, "Thumbnails")))
					{
						dir = new DirectoryInfo(Path.Combine(TS3MyDocs, "Thumbnails"));
						myFiles = dir.GetFiles("*.package");
						foreach (FileInfo f in myFiles)
						{
							File.Delete(f.FullName);
						}
					}

					try
					{
						File.Delete(Path.Combine(TS3MyDocs, "CASPartCache.package"));
						File.Delete(Path.Combine(TS3MyDocs, "compositorCache.package"));
						File.Delete(Path.Combine(TS3MyDocs, "scriptCache.package"));
						File.Delete(Path.Combine(TS3MyDocs, "simCompositorCache.package"));
					}
					catch (Exception ex)
					{
					}
				}

			}
			else
			{
				if (MessageBox.Show("You are about to delete the " + cacheName + " cache.  Are you sure?", "Clear " + cacheName, MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
				{
					File.Delete(cachePath);
				}
			}

			
		}

		public void ProcessFiles()
		{
			toolStripProgressBar1.Minimum = 0;

			DateTime start = new DateTime();
			start = DateTime.Now;

			int numFiles = this.fileList.Count;
			double bytesScanned = 0;

			//string filename = "";

			toolStripProgressBar1.Maximum = numFiles + 1;
			this.inScan = true;

			for (int i = 0; i < numFiles; i++)
			{
				try
				{
					fileInfo fi = (fileInfo)this.fileList.Pop();
					FileInfo f = new FileInfo(fi.filename);
					bytesScanned += f.Length;

					ListViewItem item = new ListViewItem();

					item.Text = f.Name;
					item.SubItems.Add(fi.filename);

					item.ToolTipText = fi.filename;

					item.SubItems.Add("");

					item.SubItems.Add(f.Length.ToString());
					item.SubItems.Add(f.LastWriteTime.ToString());
					string folder = f.DirectoryName.Replace(fi.foldername, "");
					item.SubItems.Add(folder);
					item.SubItems.Add("");
					item.SubItems.Add("");
					item.Tag = i;

					checkPackage(fi.filename, item);

					listView1.Items.Add(item);
					packageList.Add(item);

					toolStripProgressBar1.Value++;

					//Application.DoEvents();

				}
				catch (System.Exception excpt)
				{
					MessageBox.Show(excpt.Message + " " + excpt.StackTrace);
				}
			}

			this.inScan = false;
			DateTime stop = new DateTime();
			stop = DateTime.Now;

			TimeSpan timeSpan = new TimeSpan();
			timeSpan = stop - start;

			double timeTaken = timeSpan.TotalSeconds;
			if (timeTaken == 0) timeTaken++;

			double filesPerSecond = Math.Round((double)(numFiles / timeTaken), 2);

			toolStripStatusLabel1.Text = "Scanned " + numFiles + " files in " + Math.Round(timeTaken, 3) + " seconds with " + Math.Round(filesPerSecond, 0) + " files per second.";
			statusStrip1.Refresh();
			toolStripProgressBar1.Value = 0;

			if (bytesScanned == 0) bytesScanned++;

			groupBox3.Text = "Installed .package files: (" + numFiles + " files, " + Math.Round(bytesScanned / ( 1024 * 1024), 2) + "MB used)";

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
				if (includedisabledInScanToolStripMenuItem.Checked)
				{
					myFiles = dir.GetFiles("*.package.disabled");
					filesToProcess += myFiles.Length;
					foreach (FileInfo f in myFiles)
					{
						fileInfo fi = new fileInfo();
						fi.filename = f.FullName;
						fi.foldername = this.masterDir;

						this.fileList.Push(fi);

						fi = null;
					}
				}
				myFiles = dir.GetFiles("*.dbc");
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
		private void populateList(string prefix, Color selectColor)
		{

			listView1.Visible = false;
			listView1.Items.Clear();

			for (int i = 0; i < packageList.Count; i++)
			{
				ListViewItem item = packageList[i];
				if (String.IsNullOrEmpty(prefix))
				{
					if (selectColor.IsEmpty)
					{
						// Add everything
						listView1.Items.Add(item);
					}
					else if (item.BackColor == selectColor)
					{
						listView1.Items.Add(item);
					}

				}
				else
				{
					if (item.SubItems[2].Text.StartsWith(prefix))
					{
						listView1.Items.Add(item);
					}
					else if (prefix == "All")
					{
						if (item.BackColor == detective.pType.ToColor(MadScience.Detective.PackageTypes.emptyPackage)) listView1.Items.Add(item); // Corrupt / Empty
						if (item.BackColor == detective.pType.ToColor(MadScience.Detective.PackageTypes.conflictPackage)) listView1.Items.Add(item); // Conflict
						//if (item.BackColor == Color.Yellow) listView1.Items.Add(item); // Duplicate
						if (item.SubItems[2].Text.StartsWith("Duplicate of ")) listView1.Items.Add(item); // Duplicate
						if (item.BackColor == detective.pType.ToColor(MadScience.Detective.PackageTypes.sims2Package)) listView1.Items.Add(item); // Sims 2
					}

				}
			}

			listView1.Visible = true;

			if (String.IsNullOrEmpty(prefix))
			{
				if (selectColor.IsEmpty)
				{
					lblFilterActive.Text = "";
					toolStripStatusLabel1.Text = listView1.Items.Count.ToString() + " custom content files found.";
				}
				else
				{
					toolStripStatusLabel1.Text = "Found " + listView1.Items.Count.ToString() + " filtered items";
					lblFilterActive.Text = "Filter active.";
				}
			}
			else
			{
				toolStripStatusLabel1.Text = "Found " + listView1.Items.Count.ToString() + " filtered items";
				lblFilterActive.Text = "Filter active.";
			}
		}

		private void checkLocations()
		{

			MadScience.Helpers.findInstalledGames();

			for (int i = 0; i < MadScience.Helpers.gamesInstalled.Items.Count; i++)
			{
				ListViewItem item = new ListViewItem();
				MadScience.Helpers.GameInfo gameInfo = MadScience.Helpers.gamesInstalled.Items[i];
				item.Text = gameInfo.gameName;
				if (gameInfo.isInstalled)
				{
					item.SubItems.Add("Yes");
					if (gameInfo.hasFramework)
					{
						item.SubItems.Add("Yes");
					}
					else
					{
						item.SubItems.Add("No");
					}
				}
				else
				{
					item.SubItems.Add("No");
					item.SubItems.Add("");
				}

				if (gameInfo.version.Major > 0)
				{
					item.SubItems.Add(gameInfo.version.ToString());
				}
				else
				{
					item.SubItems.Add("");
				}
				item.SubItems.Add(gameInfo.path);

				listView3.Items.Add(item);
			}

			/*
			string sims3root = MadScience.Helpers.findSims3Root();
			if (sims3root == "")
			{
				//showMessage("Error: Cannot find Sims 3 root.");
				//showMessage("Custom content will NOT work correctly!");
				//showMessage("Click Change to find the correct Sims 3 root folder");

				chkHasTS3.Checked = false;
				chkFrameworkTS3.Checked = false;
				//button2.Enabled = false;
				//disableFrameworkToolStripMenuItem.Enabled = false;
			}
			else
			{
				//textBox2.Text = sims3root;
				chkHasTS3.Checked = true;

				if (MadScience.Helpers.hasFramework(sims3root, MadScience.Helpers.GameNumber.baseGame) == false)
				{
					chkFrameworkTS3.Checked = false;
				}
				else
				{
					chkFrameworkTS3.Checked = true;
				}

				//textBox3.Text = MadScience.Helpers.findSims3Root(" World Adventures", "FullBuildEP1", false);
				//string waLocation = MadScience.Helpers.findSims3Root(" World Adventures", "FullBuildEP1", false);
				string waLocation = MadScience.Helpers.findSims3Root(MadScience.Helpers.GameNumber.worldAdventures, false);
				if (String.IsNullOrEmpty(waLocation))
				{
					chkHasWA.Checked = false;
					button3.Enabled = false;
				}
				else
				{
					chkHasWA.Checked = true;
					button3.Enabled = true;
				}
				if (MadScience.Helpers.hasFramework(waLocation, MadScience.Helpers.GameNumber.worldAdventures) == false)
				{
					chkFrameworkWA.Checked = false;
				}
				else
				{
					chkFrameworkWA.Checked = true;
				}

				string helsLocation = MadScience.Helpers.findSims3Root(MadScience.Helpers.GameNumber.highEndLoftStuff, false);
				if (String.IsNullOrEmpty(helsLocation))
				{
					chkHasHELS.Checked = false;
					button10.Enabled = false;
				}
				else
				{
					chkHasHELS.Checked = true;
					button10.Enabled = true;
				}
				if (MadScience.Helpers.hasFramework(helsLocation, MadScience.Helpers.GameNumber.highEndLoftStuff) == false)
				{
					chkFrameworkHELS.Checked = false;
				}
				else
				{
					chkFrameworkHELS.Checked = true;
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
			 */
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (listView3.SelectedItems.Count > 0)
			{
				string path = listView3.SelectedItems[0].SubItems[4].Text;
				MadScience.Helpers.OpenWindow(path);
			}
		}

		private void button2_Click(object sender, EventArgs e)
		{
			showCustomContent();
		}

		private void showCustomContent()
		{

			packageList.Clear();

			listView1.Items.Clear();
			listView1.Visible = false;

			conflictList.Clear();

			toolStripProgressBar1.Visible = true;
			button4.Visible = false;
			button5.Visible = false;
			button8.Visible = false;
			//label1.Visible = false;

			string lastScanDir = MadScience.Helpers.getRegistryValue("lastScanDir");
			if (!String.IsNullOrEmpty(lastScanDir) && String.IsNullOrEmpty(folderBrowserDialog1.SelectedPath))
			{
				folderBrowserDialog1.SelectedPath = lastScanDir;
			}
			if (String.IsNullOrEmpty(lastScanDir))
			{
				resetScanFolderToolStripMenuItem.Enabled = false;
			}
			else
			{
				resetScanFolderToolStripMenuItem.Enabled = true;
			}

			if (String.IsNullOrEmpty(folderBrowserDialog1.SelectedPath))
			{
				for (int i = 0; i < MadScience.Helpers.gamesInstalled.Items.Count; i++)
				{
					MadScience.Helpers.GameInfo gameInfo = MadScience.Helpers.gamesInstalled.Items[i];
					if (gameInfo.isInstalled)
					{
						if (gameInfo.hasFramework)
						{
							this.masterDir = Path.Combine(gameInfo.path, Path.Combine("Mods", "Packages"));
							CountFiles(this.masterDir);
						}
						else
						{
							// Check for explicit Mods\Packages folder
							if (Directory.Exists(Path.Combine(gameInfo.path, Path.Combine("Mods", "Packages"))))
							{
								this.masterDir = Path.Combine(gameInfo.path, Path.Combine("Mods", "Packages"));
								CountFiles(this.masterDir);
							}
						}
					}

				}

			}
			else
			{
				this.masterDir = folderBrowserDialog1.SelectedPath;
				CountFiles(folderBrowserDialog1.SelectedPath);
			}

			if (this.fileList.Count > 0)
			{
				ProcessFiles();
			}
			else
			{
				toolStripStatusLabel1.Text = "No packages found in any searched folder.";
			}

			//label1.Visible = true;
			toolStripProgressBar1.Visible = false;
			button4.Visible = true;
			button5.Visible = true;
			button8.Visible = true;

			button4.Enabled = false;

			listView1.Visible = true;

			//toolStripStatusLabel1.Text = "";
		}

		private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
		{
			Sorter.Sort(listView1, e);
		}

		private void button4_Click(object sender, EventArgs e)
		{
			if (listView1.CheckedItems.Count > 0)
			{
				contextMenuStrip2.Show(PointToScreen(new Point(button4.Left + button4.Width, button4.Top)));
			}
		}

		private void fixCorruptedFiles()
		{
			if (MessageBox.Show("This action will FIX all the selected packages (if possible).  It is not reversible.  Do you want to continue?", "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				int numFixed = 0;
				for (int i = 0; i < listView1.CheckedItems.Count; i++)
				{
					if (listView1.CheckedItems[i].SubItems[2].Text.StartsWith("Corrupt"))
					{
						string filename = listView1.CheckedItems[i].SubItems[1].Text;
						toolStripStatusLabel1.Text = "Fixing " + filename;

						MadScience.Detective.PackageTypes pType = detective.pType.ToType(listView1.CheckedItems[i].SubItems[2].Text);

						try
						{
							switch (pType)
							{
								case MadScience.Detective.PackageTypes.corruptTXTC:
									if (MadScience.Fixers.fixTXTR(filename, false) == true) numFixed++;
									checkPackage(filename, listView1.CheckedItems[i]);
									break;
								case MadScience.Detective.PackageTypes.corruptIndex:
									if (MadScience.Fixers.fixBadIndex(filename, false) == true) numFixed++;
									checkPackage(filename, listView1.CheckedItems[i]);
									break;
							}

							//checkPackage(filename, listView1.CheckedItems[i]);
						}
						catch (Exception ex)
						{
							MessageBox.Show(filename + ": " + ex.Message);
						}
					}
				}
				toolStripStatusLabel1.Text = numFixed + " fixed.";
			}

		}

		private void disableSelected()
		{
			if (MessageBox.Show("This action will DISABLE all the selected packages.  It is not reversible.  Do you want to continue?", "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				for (int i = 0; i < listView1.CheckedItems.Count; i++)
				{
					string filename = listView1.CheckedItems[i].SubItems[1].Text;
					try
					{
						File.Move(filename, filename + ".disabled");
						listView1.CheckedItems[i].SubItems[1].Text = listView1.CheckedItems[i].SubItems[1].Text + ".disabled";
						listView1.CheckedItems[i].BackColor = detective.pType.ToColor(MadScience.Detective.PackageTypes.disabledPackage);
					}
					catch (Exception ex)
					{
						MessageBox.Show(filename + ": " + ex.Message);
					}
				}
			}
		}

		private void button5_Click(object sender, EventArgs e)
		{
			contextMenuStrip1.Show(PointToScreen(new Point(button5.Left + button5.Width, button5.Top)));
		}

		private void listView1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listView1.SelectedItems.Count == 0) return;

			ListViewItem item = listView1.SelectedItems[0];
			string filename = item.SubItems[1].Text;
			Stream input = Stream.Null;
			switch (this.detective.pType.ToType(item.SubItems[2].Text))
			{
				case MadScience.Detective.PackageTypes.objectGeneric:
					input = MadScience.Package.Search.getStream(filename, 0x2E75C764, -1, -1);
					if (!MadScience.StreamHelpers.isValidStream(input))
					{
						input = MadScience.Package.Search.getStream(filename, 0x2E75C765, -1, -1);
						if (!MadScience.StreamHelpers.isValidStream(input))
						{
							input = MadScience.Package.Search.getStream(filename, 0x2E75C766, -1, -1);
						}
					}
					if (MadScience.StreamHelpers.isValidStream(input))
					{
						picThumb.Image = Image.FromStream(input);
						picThumb.Visible = true;
					}
					else
					{
						picThumb.Visible = false;
					}
					break;
				case MadScience.Detective.PackageTypes.patternGeneric:
					picThumb.Image = MadScience.Patterns.makePatternThumb(filename);
					picThumb.Visible = true;
					break;
				default:
					picThumb.Visible = false;
					break;
			}

		}

		private void listView1_ItemChecked(object sender, ItemCheckedEventArgs e)
		{
			if (this.inScan) return;

			int numChecked = listView1.CheckedItems.Count;
			toolStripStatusLabel1.Text = numChecked.ToString() + " selected";
			if (numChecked > 0)
			{
				button4.Enabled = true;
			}
			else
			{
				button4.Enabled = false;
			}
		}

		private void button3_Click(object sender, EventArgs e)
		{
			MadScience.Helpers.OpenWindow(MadScience.Helpers.findSims3Root(MadScience.Helpers.GameNumber.worldAdventures));
		}

		private void listView2_ColumnClick(object sender, ColumnClickEventArgs e)
		{
			Sorter.Sort(listView2, e);
		}

		private void button7_Click(object sender, EventArgs e)
		{
			clearCache("", "");
			getCacheDetails();
		}

		private void button6_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < listView2.Items.Count; i++)
			{
				if (listView2.Items[i].Checked) clearCache(listView2.Items[i].SubItems[1].Text, listView2.Items[i].Text);
			}
			getCacheDetails();
		}

		private void listView2_ItemChecked(object sender, ItemCheckedEventArgs e)
		{
			int numChecked = listView2.CheckedItems.Count;
			if (numChecked > 0)
			{
				button6.Enabled = true;
			}
			else
			{
				button6.Enabled = false;
			}
		}

		private void disableSelectedToolStripMenuItem_Click(object sender, EventArgs e)
		{
			disableSelected();
		}

		private void fixCorruptedTXTCsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			fixCorruptedFiles();
		}

		private void checkPackage(string filename, ListViewItem item)
		{

			// Check 0 byte file
			FileInfo f = new FileInfo(filename);

			if (!alwaysUseFastScanToolStripMenuItem.Checked)
			{
				toolStripStatusLabel1.Text = "Checking " + f.Name;
				statusStrip1.Refresh();
			}

			detective.isCorrupt = false;
			detective.isDisabled = false;

			if (f.Length == 0)
			{
				item.BackColor = detective.pType.ToColor(MadScience.Detective.PackageTypes.emptyPackage);
				item.SubItems[2].Text = detective.pType.ToString(MadScience.Detective.PackageTypes.emptyPackage);
				return;
			}

			if (filename.EndsWith(".disabled"))
			{
				detective.isDisabled = true;
				item.BackColor = detective.pType.ToColor(MadScience.Detective.PackageTypes.disabledPackage);
			}

			Stream input = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
			MadScience.Wrappers.Database db = new MadScience.Wrappers.Database(input, true, false);

			try
			{
				if (filename.Contains(".dbc"))
				{
					MadScience.Detective.PackageType packageType = detective.getType(db, true);
				}
				else
				{
					MadScience.Detective.PackageType packageType = detective.getType(db, false);
				}
			}
			catch (System.Exception excpt)
			{
				MessageBox.Show(excpt.Message + " " + excpt.StackTrace, f.Name);
				detective.pType.MainType = MadScience.Detective.PackageTypes.corruptBadDownload;
				detective.isCorrupt = true;
				item.BackColor = detective.pType.ToColor();
				item.SubItems[2].Text = detective.pType.ToString();
				input.Close();
				return;
			}

			if (detective.pType.MainType == MadScience.Detective.PackageTypes.sims2Package)
			{
				item.SubItems[2].Text = detective.pType.ToString();
				if (!detective.isDisabled) item.BackColor = detective.pType.ToColor(MadScience.Detective.PackageTypes.sims2Package);
				input.Close();
				return;
			}

			// Game number
			switch (db.dbpf.gameNumber)
			{
				case MadScience.Helpers.GameNumber.baseGame:
					item.SubItems[7].Text = "Base";
					break;
				case MadScience.Helpers.GameNumber.worldAdventures:
					item.SubItems[7].Text = "WA";
					break;
				case MadScience.Helpers.GameNumber.highEndLoftStuff:
					item.SubItems[7].Text = "HELS";
					break;
				case MadScience.Helpers.GameNumber.ambitions:
					item.SubItems[7].Text = "AMB";
					break;
			}

			if (detective.isCorrupt)
			{
				item.SubItems[2].Text = detective.pType.ToString();
				item.SubItems[6].Text = detective.pType.SubType;
				if (!detective.isDisabled) item.BackColor = detective.pType.ToColor(MadScience.Detective.PackageTypes.corruptTXTC);
				input.Close();
				return;
			}

			if (!detective.isDisabled) item.BackColor = listView1.BackColor;
			string pType = detective.pType.ToString();

			// Check if any of this packages types exist in the entriesdb
			for (int i = 0; i < db.dbpf.Entries.Count; i++)
			{
				MadScience.Wrappers.DatabasePackedFile.Entry entry = db.dbpf.Entries[i];
				bool dealtWith = false;
				string entryString = entry.Key.ToString();

				if (Enum.IsDefined(typeof(ConflictTypes), entry.Key.typeId))
				//foreach (uint typeHash in Enum.GetValues(typeof(ResourceTypes)))
				//{
				//	if (typeHash == entry.Key.typeId)
					{
						if (conflictList.ContainsKey(entryString))
						{
							// Something already has this ResourceKey - mark file as conflicted.
							// But wait!  Check if it's pointing to itself first. :)
							if (conflictList[entryString] == (int)item.Tag) break;

							// Check if it's an entire duplicate, or merely part
							FileInfo f2 = new FileInfo(packageList[conflictList[entryString]].SubItems[1].Text);
							//FileInfo f2 = new FileInfo(listView1.Items[conflictList[entryString]].SubItems[1].Text);
							if (f.Length == f2.Length)
							{
								pType = "Duplicate of " + Path.Combine(packageList[conflictList[entryString]].SubItems[5].Text, packageList[conflictList[entryString]].Text);
								if (!detective.isDisabled)
								{
									item.BackColor = detective.pType.ToColor(MadScience.Detective.PackageTypes.duplicatePackage);
									packageList[conflictList[entryString]].BackColor = detective.pType.ToColor(MadScience.Detective.PackageTypes.duplicatePackage);
								}
								dealtWith = true;
								break;
							}
							else
							{
								pType = "Conflicts with " + Path.Combine(packageList[conflictList[entryString]].SubItems[5].Text, packageList[conflictList[entryString]].Text);
								if (!detective.isDisabled)
								{
									item.BackColor = detective.pType.ToColor(MadScience.Detective.PackageTypes.conflictPackage);

									if (packageList[conflictList[entryString]].BackColor != detective.pType.ToColor(MadScience.Detective.PackageTypes.corruptTXTC))
									{
										packageList[conflictList[entryString]].BackColor = detective.pType.ToColor(MadScience.Detective.PackageTypes.conflictPackage);
									}
								}
								dealtWith = true;
								break;
							}
						}
						else
						{
							conflictList.Add(entryString, (int)item.Tag);
						}
				//	}
				}

				if (dealtWith == true) break;
			}

			input.Close();

			//if (packageType == "unknown")
			//{
				//packageType = "Sims 3 Package";
			//}

			item.SubItems[2].Text = pType;
			item.SubItems[6].Text = detective.pType.SubType;

		}

		private void selectListItems(Color colorType)
		{
			if (colorType.IsEmpty) return;

			this.inScan = true;
			for (int i = 0; i < listView1.Items.Count; i++)
			{
				if (listView1.Items[i].BackColor == colorType)
				{
					listView1.Items[i].Checked = true;
				}
			}
			this.inScan = false;

			if (listView1.CheckedItems.Count > 0)
			{
				button4.Enabled = true;
			}
			else
			{
				button4.Enabled = false;
			}

			toolStripStatusLabel1.Text = listView1.CheckedItems.Count.ToString() + " items selected.";
			statusStrip1.Refresh();

		}

		private void selectListItems(string prefix)
		{
			if (String.IsNullOrEmpty(prefix)) return;

			this.inScan = true;
			for (int i = 0; i < listView1.Items.Count; i++)
			{
				if (listView1.Items[i].SubItems[2].Text.StartsWith(prefix))
				{
					listView1.Items[i].Checked = true;
				}
			}
			this.inScan = false;

			if (listView1.CheckedItems.Count > 0)
			{
				button4.Enabled = true;
			}
			else
			{
				button4.Enabled = false;
			}

			toolStripStatusLabel1.Text = listView1.CheckedItems.Count.ToString() + " " + prefix + " items selected.";
			statusStrip1.Refresh();

		}

		private void disableToolStripMenuItem_Click(object sender, EventArgs e)
		{
			selectListItems("Corrupt");
		}

		private void moveToToolStripMenuItem_Click(object sender, EventArgs e)
		{
			selectListItems("Duplicate");
		}

		private void emptyFilesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			selectListItems("Empty");
		}

		private void sims2PackageToolStripMenuItem_Click(object sender, EventArgs e)
		{
			selectListItems("Sims 2");
		}

		private void allcorruptFilesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < listView1.Items.Count; i++)
			{
				if (listView1.Items[i].SubItems[2].Text.StartsWith("Empty"))
				{
					listView1.Items[i].Checked = true;
				}
				if (listView1.Items[i].SubItems[2].Text.StartsWith("Duplicate"))
				{
					listView1.Items[i].Checked = true;
				}
				if (listView1.Items[i].SubItems[2].Text.StartsWith("Conflict"))
				{
					listView1.Items[i].Checked = true;
				}
				if (listView1.Items[i].SubItems[2].Text.StartsWith("Sims 2"))
				{
					listView1.Items[i].Checked = true;
				}
				if (listView1.Items[i].SubItems[2].Text.StartsWith("Corrupt"))
				{
					listView1.Items[i].Checked = true;
				}
			}
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void alwaysUseFastScanToolStripMenuItem_Click(object sender, EventArgs e)
		{
			alwaysUseFastScanToolStripMenuItem.Checked = !alwaysUseFastScanToolStripMenuItem.Checked;
			if (alwaysUseFastScanToolStripMenuItem.Checked)
			{
				MadScience.Helpers.saveRegistryValue("fastScan", "true");
			}
			else
			{
				MadScience.Helpers.saveRegistryValue("fastScan", "false");
			}
		}

		private void conflictsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			selectListItems("Conflict");
		}

		private void toolStripMenuItem3_Click(object sender, EventArgs e)
		{
			populateList("Corrupt", Color.Empty);
		}

		private void button8_Click(object sender, EventArgs e)
		{
			if (lblFilterActive.Text == "Filter active.")
			{
				clearFilterToolStripMenuItem.Enabled = true;
			}
			else
			{
				clearFilterToolStripMenuItem.Enabled = false;
			}
			contextMenuStrip3.Show(PointToScreen(new Point(button8.Left + button8.Width, button8.Top)));
		}

		private void toolStripMenuItem4_Click(object sender, EventArgs e)
		{
			populateList("", detective.pType.ToColor(MadScience.Detective.PackageTypes.duplicatePackage));
		}

		private void toolStripMenuItem5_Click(object sender, EventArgs e)
		{
			populateList("", detective.pType.ToColor(MadScience.Detective.PackageTypes.conflictPackage));
		}

		private void toolStripMenuItem6_Click(object sender, EventArgs e)
		{
			populateList("Empty", Color.Empty);
		}

		private void toolStripMenuItem7_Click(object sender, EventArgs e)
		{
			populateList("Sims 2", Color.Empty);
		}

		private void toolStripMenuItem8_Click(object sender, EventArgs e)
		{
			populateList("All", Color.Empty);
		}

		private void scanFolderToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
			{
				MadScience.Helpers.saveRegistryValue("lastScanDir", folderBrowserDialog1.SelectedPath);
				showCustomContent();
			}
		}

		private void openFileLocationToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (listView1.SelectedItems.Count == 1)
			{
				string filename = listView1.SelectedItems[0].SubItems[1].Text;
				string folder = Path.GetDirectoryName(filename);
				MadScience.Helpers.OpenWindow(folder);
			}
		}

		private void deselectAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < listView1.Items.Count; i++)
			{
				if (listView1.Items[i].Checked) listView1.Items[i].Checked = false;
			}
		}

		private void findGameCacheFolderToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (folderBrowserDialog2.ShowDialog() == DialogResult.OK)
			{
				MadScience.Helpers.saveRegistryValue("cacheFolders", folderBrowserDialog2.SelectedPath);
				getCacheDetails();
			}
		}

		private void resetScanFolderToolStripMenuItem_Click(object sender, EventArgs e)
		{
			folderBrowserDialog1.SelectedPath = "";
			MadScience.Helpers.saveRegistryValue("lastScanDir", "");
			resetScanFolderToolStripMenuItem.Enabled = false;
			showCustomContent();
		}

		private void clearFilterToolStripMenuItem_Click(object sender, EventArgs e)
		{
			populateList("", Color.Empty);
		}

		private void pictureBox1_Click(object sender, EventArgs e)
		{
			populateList("Sims 2", Color.Empty);
		}

		private void pictureBox2_Click(object sender, EventArgs e)
		{
			populateList("", detective.pType.ToColor(MadScience.Detective.PackageTypes.duplicatePackage));
		}

		private void label3_Click(object sender, EventArgs e)
		{

		}

		private void pictureBox3_Click(object sender, EventArgs e)
		{
			populateList("", detective.pType.ToColor(MadScience.Detective.PackageTypes.conflictPackage));
		}

		private void pictureBox4_Click(object sender, EventArgs e)
		{
			populateList("", detective.pType.ToColor(MadScience.Detective.PackageTypes.emptyPackage));
		}

		private void button9_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("Warning, this will do the following: Disable Sims 2 and Empty packages, and also Fix Corrupt files (where possible). If a corrupted file cannot be fixed, it will be Disabled. This is NOT reversible. Note: This does NOT fix 'conflicted' files. You have to sort these out yourself. Do you want to continue?", "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				int numFixed = 0;
				int numDisabled = 0;

				for (int i = 0; i < listView1.Items.Count; i++)
				{
					string filename = listView1.Items[i].SubItems[1].Text;

					if (listView1.Items[i].SubItems[2].Text.StartsWith("Corrupt"))
					{
						toolStripStatusLabel1.Text = "Fixing " + filename;

						try
						{
							switch (detective.pType.ToType(listView1.Items[i].SubItems[2].Text))
							{
								case MadScience.Detective.PackageTypes.corruptTXTC:
									if (MadScience.Fixers.fixTXTR(filename, false) == true) numFixed++;
									break;
								case MadScience.Detective.PackageTypes.corruptIndex:
									if (MadScience.Fixers.fixBadIndex(filename, false) == true) numFixed++;
									break;
								case MadScience.Detective.PackageTypes.corruptNotADBPF:
								case MadScience.Detective.PackageTypes.corruptBadDownload:
								case MadScience.Detective.PackageTypes.corruptChaavik:
								case MadScience.Detective.PackageTypes.corruptPeggy:
								case MadScience.Detective.PackageTypes.neighbourhood:
									// We can't fix these, so just disable them
									File.Move(filename, filename + ".disabled");
									numDisabled++;
									break;
							}

							//checkPackage(filename, listView1.Items[i]);
						}
						catch (Exception ex)
						{
							MessageBox.Show(filename + ": " + ex.Message);
						}

					}

					if (listView1.Items[i].SubItems[2].Text.StartsWith("Sims 2") || listView1.Items[i].SubItems[2].Text.StartsWith("Empty"))
					{
						try
						{
							File.Move(filename, filename + ".disabled");
							numDisabled++;
						}
						catch (Exception ex)
						{
							MessageBox.Show(filename + ": " + ex.Message);
						}
					}

					if (listView1.Items[i].SubItems[2].Text.StartsWith("Duplicate of "))
					{
						try
						{
							File.Move(filename, filename + ".disabled");
							numDisabled++;
						}
						catch (Exception ex)
						{
							MessageBox.Show(filename + ": " + ex.Message);
						}
					}

				}
				MessageBox.Show("Scan complete. Fixed " + numFixed + " packages and disabled " + numDisabled + ".");

				showCustomContent();
			}
		}

		private void includedisabledInScanToolStripMenuItem_Click(object sender, EventArgs e)
		{
			includedisabledInScanToolStripMenuItem.Checked = !includedisabledInScanToolStripMenuItem.Checked;
			if (includedisabledInScanToolStripMenuItem.Checked)
			{
				MadScience.Helpers.saveRegistryValue("scanDisabled", "true");
			}
			else
			{
				MadScience.Helpers.saveRegistryValue("scanDisabled", "false");
			}

		}

		private void disabledFilesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			populateList("", detective.pType.ToColor(MadScience.Detective.PackageTypes.disabledPackage));
		}

		private void pictureBox5_Click(object sender, EventArgs e)
		{
			populateList("", detective.pType.ToColor(MadScience.Detective.PackageTypes.disabledPackage));
		}

		private void disabledFilesToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			selectListItems(detective.pType.ToColor(MadScience.Detective.PackageTypes.disabledPackage));
		}

		private void deletePackagesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (listView1.CheckedItems.Count > 0) {

				if (MessageBox.Show("Warning! This action will PERMENANTLY DELETE the selected packages.  Use this action at your own risk.  Do NOT complain to Delphy, MTS, or anywhere else if you use this and lose something important!  Are you SURE you want to delete these " + listView1.CheckedItems.Count + " packages?", "WARNING", MessageBoxButtons.YesNo) == DialogResult.Yes)
				{
					if (MessageBox.Show("Are you REALLY sure? This will totally nuke those files!", "Confirm", MessageBoxButtons.OKCancel) == DialogResult.OK)
					{
						for (int i = 0; i < listView1.CheckedItems.Count; i++)
						{
							string filename = listView1.CheckedItems[i].SubItems[1].Text;
							try
							{
								File.Delete(filename);
								listView1.CheckedItems[i].Remove();
							}
							catch (Exception ex)
							{
								MessageBox.Show(filename + ": " + ex.Message);
							}
						}
					}
				}
			}
		}

		private void openWithS3PEToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (listView1.SelectedItems.Count == 1)
			{
				System.Diagnostics.Process.Start(@"C:\Program Files (x86)\s3pe\s3pe.exe",  '"' + listView1.SelectedItems[0].SubItems[1].Text + '"');
			}
		}

		private void button10_Click(object sender, EventArgs e)
		{
			MadScience.Helpers.OpenWindow(MadScience.Helpers.findSims3Root(MadScience.Helpers.GameNumber.highEndLoftStuff));
		}

		private void listView3_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listView3.SelectedItems.Count > 0)
			{
				if (listView3.SelectedItems[0].SubItems[1].Text == "Yes")
				{
					button1.Enabled = true;
				}
				else
				{
					button1.Enabled = false;
				}
				if (listView3.SelectedItems[0].SubItems[2].Text == "Yes")
				{
					linkLabel1.Visible = false;
				}
				else
				{
					linkLabel1.Visible = true;
				}
			}
		}

		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start("http://www.modthesims.info/d/384014");
		}

		private void noAutoScanOnStartupToolStripMenuItem_Click(object sender, EventArgs e)
		{
			noAutoScanOnStartupToolStripMenuItem.Checked = !noAutoScanOnStartupToolStripMenuItem.Checked;
			if (noAutoScanOnStartupToolStripMenuItem.Checked)
			{
				MadScience.Helpers.saveRegistryValue("noAutoScanOnStartup", "true");
			}
			else
			{
				MadScience.Helpers.saveRegistryValue("NoAutoScanOnStartup", "false");
			}

		}


	}

	class fileInfo
	{
		public string filename = "";
		public string foldername = "";

	}
}
