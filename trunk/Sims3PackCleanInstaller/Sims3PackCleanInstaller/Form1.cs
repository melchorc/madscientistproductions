using System;
using System.Windows.Forms;
using System.IO;
using System.Drawing;

namespace Sims3PackCleanInstaller
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			Version vrs = new Version(Application.ProductVersion);
			this.Text = this.Text + " v" + vrs.Major + "." + vrs.Minor + "." + vrs.Build + "." + vrs.Revision;

			if (Environment.GetCommandLineArgs().Length > 1)
			{
				for (int i = 1; i < Environment.GetCommandLineArgs().Length; i++)
				{
					loadFile(Environment.GetCommandLineArgs().GetValue(i).ToString());
				}
			}

		}

		MadScience.Detective detective = new MadScience.Detective();
		MadScience.Sims3Pack Sims3Pack = new MadScience.Sims3Pack();
		MadScience.Sims3Pack.Sims3PackFile Sims3PackFile = new MadScience.Sims3Pack.Sims3PackFile();

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				toolStripStatusLabel1.Text = openFileDialog1.FileName;
				loadFile(openFileDialog1.FileName);
				saveToolStripMenuItem.Enabled = true;
				saveAsToolStripMenuItem.Enabled = true;
				button1.Enabled = true;
				dumpXMLToolStripMenuItem.Enabled = true;
				corruptedToolStripMenuItem.Enabled = true;
				extractToolStripMenuItem.Enabled = true;
			}
		}

		private void loadFile(string filename)
		{
			Sims3Pack = new MadScience.Sims3Pack();
			Sims3PackFile = Sims3Pack.Load(filename);

			listView1.Items.Clear();

			for (int i = 0; i < Sims3PackFile.PackagedFiles.Count; i++)
			{
				addListItem(Sims3PackFile.PackagedFiles[i]);
			}

			txtName.Text = Sims3PackFile.DisplayName;
			txtDesc.Text = Sims3PackFile.Description;
			txtType.Text = Sims3PackFile.Type;
			txtSubType.Text = Sims3PackFile.SubType;
			txtMinReqV.Text = Sims3PackFile.MinReqVersion;

		}

		private void addListItem(MadScience.Sims3Pack.Sims3PackFile.PackagedFile packagedFile)
		{
			detective.isCorrupt = false;
			detective.isDisabled = false;

			ListViewItem item = new ListViewItem();
			item.Checked = true;
			item.Text = packagedFile.Name;
			item.SubItems.Add(""); // 1 - Size
			item.SubItems.Add(""); // 2 - Description
			item.SubItems.Add(""); // 3 - Type
			item.SubItems.Add(""); // 4 - Sub-Type

			item.SubItems[1].Text = packagedFile.Length.ToString();

			if (packagedFile.Length == 0)
			{
				item.BackColor = detective.pType.ToColor(MadScience.Detective.PackageTypes.emptyPackage);
				item.SubItems[3].Text = detective.pType.ToString(MadScience.Detective.PackageTypes.emptyPackage);
			}
			else
			{
				MadScience.Detective.PackageType packageType = detective.getType(packagedFile.DBPF);
				item.BackColor = detective.pType.ToColor();
			}

			if (!String.IsNullOrEmpty(packagedFile.MetaTags.description))
			{
				item.SubItems[2].Text = packagedFile.MetaTags.description;
			}

			if (detective.isCorrupt)
			{
				item.SubItems[3].Text = detective.pType.ToString();
				item.SubItems[4].Text = detective.pType.SubType;
				if (!detective.isDisabled) item.BackColor = detective.pType.ToColor(MadScience.Detective.PackageTypes.corruptTXTC);
			}
			else
			{
				item.SubItems[3].Text = detective.pType.ToString();
				if (String.IsNullOrEmpty(item.SubItems[2].Text))
				{
					item.SubItems[2].Text = detective.pType.SubType;
				}
				else
				{
					item.SubItems[4].Text = detective.pType.SubType;
				}
				
			}

			// Do some sanity checks...

			// If the description so far is empty, use the metatags name
			if (String.IsNullOrEmpty(item.SubItems[2].Text) && !String.IsNullOrEmpty(packagedFile.MetaTags.name))
			{
				item.SubItems[2].Text = packagedFile.MetaTags.name;
			}

			listView1.Items.Add(item);
		}

		private void listView1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listView1.SelectedItems.Count == 0) return;

			ListViewItem item = listView1.SelectedItems[0];
			Stream fileinput = Sims3PackFile.PackagedFiles[item.Index].DBPF;
			fileinput.Position = 0;
			Stream input = Stream.Null;
			switch (this.detective.pType.ToType(item.SubItems[3].Text))
			{
				case MadScience.Detective.PackageTypes.pngThumbnail:
					picThumb.Image = Image.FromStream(fileinput);
					picThumb.Visible = true;
					break;

				case MadScience.Detective.PackageTypes.objectGeneric:
					MadScience.Wrappers.Database db = new MadScience.Wrappers.Database(fileinput);
					input = MadScience.Package.Search.getStream(db, 0x2E75C764, -1, -1);
					if (!MadScience.StreamHelpers.isValidStream(input))
					{
						input = MadScience.Package.Search.getStream(db, 0x2E75C765, -1, -1);
						if (!MadScience.StreamHelpers.isValidStream(input))
						{
							input = MadScience.Package.Search.getStream(db, 0x2E75C766, -1, -1);
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
					picThumb.Image = MadScience.Patterns.makePatternThumb(fileinput);
					picThumb.Visible = true;
					break;
				default:
					picThumb.Visible = false;
					break;
			}

		}

		private void listView1_ItemChecked(object sender, ItemCheckedEventArgs e)
		{
		}

		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			FileInfo f = new FileInfo(toolStripStatusLabel1.Text);
			saveFileDialog1.InitialDirectory = f.DirectoryName;
			saveFileDialog1.FileName = "Copy of " + f.Name;
			saveFileDialog1.Filter = "Sims3Pack File|*.Sims3Pack";
			if (saveFileDialog1.ShowDialog() == DialogResult.OK)
			{
				for (int i = 0; i < listView1.Items.Count; i++)
				{
					if (!listView1.Items[i].Checked) Sims3PackFile.PackagedFiles[i].isDisabled = true;
				}

				Sims3Pack.Save(saveFileDialog1.FileName, Sims3PackFile);
			}
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < listView1.Items.Count; i++)
			{
				if (!listView1.Items[i].Checked) Sims3PackFile.PackagedFiles[i].isDisabled = true;
			}
			Sims3Pack.Save(toolStripStatusLabel1.Text, Sims3PackFile);
		}

		private void corruptedToolStripMenuItem_Click(object sender, EventArgs e)
		{
			fixCorruptedFiles();
		}

		private void fixCorruptedFiles()
		{
			if (MessageBox.Show("This action will FIX all the selected packages (if possible).  It is not reversible.  Do you want to continue?", "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				int numFixed = 0;
				for (int i = 0; i < listView1.Items.Count; i++)
				{
					if (listView1.Items[i].SubItems[3].Text.StartsWith("Corrupt"))
					{
						Stream dbpf = Sims3PackFile.PackagedFiles[i].DBPF;
						dbpf.Position = 0;
						//toolStripStatusLabel1.Text = "Fixing " + filename;

						MadScience.Detective.PackageTypes pType = detective.pType.ToType(listView1.CheckedItems[i].SubItems[3].Text);

						try
						{
							switch (pType)
							{
								case MadScience.Detective.PackageTypes.corruptTXTC:
									if (MadScience.Fixers.fixTXTR(dbpf, false) == true) numFixed++;
									dbpf.Position = 0;
									checkPackage(dbpf, listView1.Items[i]);
									break;
								case MadScience.Detective.PackageTypes.corruptIndex:
									if (MadScience.Fixers.fixBadIndex(dbpf, false) == true) numFixed++;
									dbpf.Position = 0;
									checkPackage(dbpf, listView1.Items[i]);
									break;
								case MadScience.Detective.PackageTypes.corruptRecursive:
									if (MadScience.Fixers.fixRecursive(dbpf, false) == true) numFixed++;
									dbpf.Position = 0;
									checkPackage(dbpf, listView1.Items[i]);
									break;
							}

							//checkPackage(filename, listView1.CheckedItems[i]);
						}
						catch (Exception ex)
						{
							MessageBox.Show(listView1.Items[i].Text + ": " + ex.Message);
						}
					}
				}
				//toolStripStatusLabel1.Text = numFixed + " fixed.";
			}

		}

		private void checkPackage(Stream input, ListViewItem item)
		{
			detective.isCorrupt = false;
			detective.isDisabled = false;
			
			if (input.Length == 0)
			{
				item.BackColor = detective.pType.ToColor(MadScience.Detective.PackageTypes.emptyPackage);
				item.SubItems[3].Text = detective.pType.ToString(MadScience.Detective.PackageTypes.emptyPackage);
			}
			else
			{

				MadScience.Detective.PackageType packageType = detective.getType(input);


				item.BackColor = detective.pType.ToColor();

				if (detective.isCorrupt)
				{
					item.SubItems[3].Text = detective.pType.ToString();
					item.SubItems[4].Text = detective.pType.SubType;
					if (!detective.isDisabled) item.BackColor = detective.pType.ToColor(MadScience.Detective.PackageTypes.corruptTXTC);
				}
				else
				{
					item.SubItems[3].Text = detective.pType.ToString();
					item.SubItems[4].Text = detective.pType.SubType;
				}
			}
		}

		private void dumpXMLToolStripMenuItem_Click(object sender, EventArgs e)
		{
			saveFileDialog1.Filter = "XML file|*.xml";
			if (saveFileDialog1.ShowDialog() == DialogResult.OK)
			{
				Stream output = File.OpenWrite(saveFileDialog1.FileName);
				MadScience.StreamHelpers.WriteStringUTF8(output, "Raw XML:" + Environment.NewLine);
				MadScience.StreamHelpers.WriteStringUTF8(output, Sims3PackFile.rawXml + Environment.NewLine);
				MadScience.StreamHelpers.WriteStringUTF8(output, "Generated XML:" + Environment.NewLine);
				MadScience.StreamHelpers.WriteStringUTF8(output, Sims3Pack.WriteSims3PackXml(Sims3PackFile));
				output.Close();
			}
		}

		private void listView1_ItemCheck(object sender, ItemCheckEventArgs e)
		{
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void button1_Click(object sender, EventArgs e)
		{

		}

		private void button1_Click_1(object sender, EventArgs e)
		{
			for (int i = 0; i < listView1.Items.Count; i++)
			{
				if (listView1.Items[i].SubItems[3].Text == "Thumbnail")
				{
					listView1.Items[i].Checked = false;
				}
			}
		}

		private void extractToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DialogResult result = folderBrowserDialog1.ShowDialog();
			if (result == DialogResult.OK)
			{
				for (int i = 0; i < Sims3PackFile.PackagedFiles.Count; i++)
				{
					string filename = Sims3PackFile.PackagedFiles[i].Name;
					Sims3PackFile.PackagedFiles[i].DBPF.Seek(0, SeekOrigin.Begin);
					Stream output = File.Create(Path.Combine(folderBrowserDialog1.SelectedPath, filename));
					MadScience.StreamHelpers.CopyStream(Sims3PackFile.PackagedFiles[i].DBPF, output);
					output.Close();
				}

			}

		}


	}
}
