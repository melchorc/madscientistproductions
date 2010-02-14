using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using MadScience.Wrappers;

namespace MadScience
{
	public class Fixers
	{

		#region Fix bad index pointers
		public static bool fixBadIndex(string filename)
		{
			return fixBadIndex(filename, true);
		}

		public static bool fixBadIndex(string filename, bool throwError)
		{
			Stream input = File.Open(filename, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
			bool retVal = fixBadIndex(input, throwError);
			input.Close();
			return retVal;
		}

		public static bool fixBadIndex(Stream input)
		{
			return fixBadIndex(input, true);
		}

		public static bool fixBadIndex(Stream input, bool throwError)
		{
			Database db = new Database(input, true, throwError);
			return fixBadIndex(db);
		}

		public static bool fixBadIndex(Database db)
		{
			uint numFixed = 0;
			if (db.dbpf.packageType == Detective.PackageTypes.corruptIndex)
			{
				// This means the index is off by 3.  We can just write the file out again
				numFixed++;
			}

			if (numFixed == 0)
			{
				return false;
			}
			else
			{
				db.Commit(true);
			}

			return true;

		}
		#endregion

		#region Fix corrupted TXTR chunks

		// Return TRUE if the file was corrupted and not fixed.  FALSE otherwise.
		public static bool fixTXTR(string filename)
		{
			return fixTXTR(filename, true);
		}

		public static bool fixTXTR(string filename, bool throwError)
		{
			Stream input = File.Open(filename, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
			bool retVal = fixTXTR(input, throwError);
			input.Close();
			return retVal;
		}

		public static bool fixTXTR(Stream input)
		{
			return fixTXTR(input, true);
		}

		public static bool fixTXTR(Stream input, bool throwError)
		{
			Database db = new Database(input, true, throwError);
			return fixTXTR(db);
		}

		public static bool fixTXTR(Database db)
		{
			uint numFixed = 0;

			// textBox1.Text += "Checking for corrupted TXTC entries... " + Environment.NewLine;
			for (int i = 0; i < db.dbpf.Entries.Count; i++)
			{
				DatabasePackedFile.Entry entry = db.dbpf.Entries[i];
				if (entry.Key.typeId == 0x033A1435)
				{
					//textBox1.Text += "  " + entry.Key.ToString() + Environment.NewLine;
					// textBox1.Text += "    Checking for corrupted TGI offset... ";

					// Quick and dirty way
					Stream TXTC = db.GetResourceStream(entry.Key);
					// Read offset, first 4 bytes after ID
					StreamHelpers.ReadValueU32(TXTC);

					uint offset = StreamHelpers.ReadValueU32(TXTC);
					// textBox1.Text += offset.ToString() + "...";

					// Seek to this offset + 8 and read the number there.
					TXTC.Seek(offset + 8, SeekOrigin.Begin);

					uint numTGIs = StreamHelpers.ReadValueU8(TXTC);
					// textBox1.Text += numTGIs.ToString() + " TGIs... ";

					// Since each TGI is 16 bytes we can calculate how many bytes they are.
					uint tgiSize = numTGIs * 16;
					uint tgiOffsetEnd = offset + 8 + 1 + tgiSize;

					//textBox1.Text += "TGI block end is at " + tgiOffsetEnd.ToString() + "...";

					if (tgiOffsetEnd == TXTC.Length)
					{
						return false;
					}
					else
					{
						// Try offset - 1
						offset = offset - 1;

						// Seek to this offset + 8 and read the number there.
						TXTC.Seek(offset + 8, SeekOrigin.Begin);

						numTGIs = StreamHelpers.ReadValueU8(TXTC);
						// textBox1.Text += numTGIs.ToString() + " TGIs... ";

						// Since each TGI is 16 bytes we can calculate how many bytes they are.
						tgiSize = numTGIs * 16;
						tgiOffsetEnd = offset + 8 + 1 + tgiSize;

						//textBox1.Text += "TGI block end is at " + tgiOffsetEnd.ToString() + "...";

						if (tgiOffsetEnd == TXTC.Length)
						{
							//   textBox1.Text += "Correct!";
							// Offset it minus 1

							TXTC.Seek(4, SeekOrigin.Begin);
							StreamHelpers.WriteValueU32(TXTC, offset);

							MemoryStream newTXTC = new MemoryStream();
							StreamHelpers.CopyStream(TXTC, newTXTC, true);

							db.SetResourceStream(entry.Key, newTXTC);

							//    textBox1.Text += Environment.NewLine;
							//     textBox1.Text += "The above resource has been fixed.";

							numFixed++;

							newTXTC.Close();
						}
						else
						{
							//   textBox1.Text += "Incorrect!";
						}
					}

					// textBox1.Text += Environment.NewLine;


					TXTC = null;
				}
			}

			if (numFixed == 0)
			{
				//textBox1.Text += Environment.NewLine;
				//textBox1.Text += "This file appears OK!";
				return false;
			}
			else
			{
				//this.filesFixed++;
				//textBox1.Text += Environment.NewLine;
				//textBox1.Text += "This file had some corrupted TXTCs! They are now fixed.";
				//textBox1.Text += "Fixed " + filename + Environment.NewLine;
				//saveToolStripMenuItem.Enabled = true;
				db.Commit(true);
			}

			return true;

		}
		#endregion

	}
}
