using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using MadScience.Wrappers;

namespace MadScience.Package
{
	public class Search
	{
		public static ResourceKey getKey(string filename, int typeID, int groupID, long instanceID)
		{
			ResourceKey matchChunk = new ResourceKey();
			

			if (String.IsNullOrEmpty(filename)) { return matchChunk; }

			// Open the package file and search
			Stream package = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
			MadScience.Wrappers.Database db = new MadScience.Wrappers.Database(package, true);

			int searchType = 0;
			if (typeID != -1) { searchType += 1; }
			if (groupID != -1) { searchType += 2; }
			if (instanceID != -1) { searchType += 4; }

			bool foundMatch = false;

			foreach (MadScience.Wrappers.ResourceKey entry in db._Entries.Keys)
			{
				//ResourceKey key = db.Entries.Keys[i];
				//DatabasePackedFile.Entry entry = db.Entries.Keys[i];
				//DatabasePackedFile.Entry entry = db.dbpfEntries[i];
				//MadScience.Wrappers.ResourceKey entry = new MadScience.Wrappers.ResourceKey(keyString);

				switch (searchType)
				{
					case 7:
						if (entry.typeId == typeID && entry.groupId == groupID && entry.instanceId == (ulong)instanceID)
						{
							//loadedCasPart = entry;
							//matchChunk = db.GetResourceStream(entry);
							matchChunk = entry;
							foundMatch = true;
						}
						break;
					case 6:
						if (entry.groupId == groupID && entry.instanceId == (ulong)instanceID)
						{
							//loadedCasPart = entry;
							//matchChunk = db.GetResourceStream(entry);
							matchChunk = entry;
							foundMatch = true;
						}
						break;
					case 5:
						if (entry.typeId == typeID && entry.instanceId == (ulong)instanceID)
						{
							//loadedCasPart = entry;
							//matchChunk = db.GetResourceStream(entry);
							matchChunk = entry;
							foundMatch = true;
						}
						break;
					case 4:
						if (entry.instanceId == (ulong)instanceID)
						{
							//loadedCasPart = entry;
							//matchChunk = db.GetResourceStream(entry);
							matchChunk = entry;
							foundMatch = true;
						}
						break;
					case 3:
						if (entry.typeId == typeID && entry.groupId == groupID)
						{
							//loadedCasPart = entry;
							//matchChunk = db.GetResourceStream(entry);
							matchChunk = entry;
							foundMatch = true;
						}
						break;
					case 2:
						if (entry.groupId == groupID)
						{
							//loadedCasPart = entry;
							//matchChunk = db.GetResourceStream(entry);
							matchChunk = entry;
							foundMatch = true;
						}
						break;
					case 1:
						if (entry.typeId == typeID)
						{
							//loadedCasPart = entry;
							//matchChunk = db.GetResourceStream(entry);
							matchChunk = entry;
							foundMatch = true;
						}
						break;
				}
				if (foundMatch)
				{
					break;
				}

			}
			package.Close();

			return matchChunk;

		}

		public static Stream getStream(string filename, int typeID, int groupID, long instanceID)
		{

			Stream matchChunk = null;

			if (String.IsNullOrEmpty(filename)) { return matchChunk; }

			// Open the package file and search
			Stream package = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
			MadScience.Wrappers.Database db = new MadScience.Wrappers.Database(package, true);

			matchChunk = getStream(db, typeID, groupID, instanceID);

			package.Close();

			return matchChunk;
		}


		public static Stream getStream(Database db, int typeID, int groupID, long instanceID)
		{

			Stream matchChunk = null;

			int searchType = 0;
			if (typeID != -1) { searchType += 1; }
			if (groupID != -1) { searchType += 2; }
			if (instanceID != -1) { searchType += 4; }

			bool foundMatch = false;

			foreach (MadScience.Wrappers.ResourceKey entry in db._Entries.Keys)
			{
				//ResourceKey key = db.Entries.Keys[i];
				//DatabasePackedFile.Entry entry = db.Entries.Keys[i];
				//DatabasePackedFile.Entry entry = db.dbpfEntries[i];
				//MadScience.Wrappers.ResourceKey entry = new MadScience.Wrappers.ResourceKey(keyString);

				switch (searchType)
				{
					case 7:
						if (entry.typeId == typeID && entry.groupId == groupID && entry.instanceId == (ulong)instanceID)
						{
							//loadedCasPart = entry;
							matchChunk = db.GetResourceStream(entry);
							foundMatch = true;
						}
						break;
					case 6:
						if (entry.groupId == groupID && entry.instanceId == (ulong)instanceID)
						{
							//loadedCasPart = entry;
							matchChunk = db.GetResourceStream(entry);
							foundMatch = true;
						}
						break;
					case 5:
						if (entry.typeId == typeID && entry.instanceId == (ulong)instanceID)
						{
							//loadedCasPart = entry;
							matchChunk = db.GetResourceStream(entry);
							foundMatch = true;
						}
						break;
					case 4:
						if (entry.instanceId == (ulong)instanceID)
						{
							//loadedCasPart = entry;
							matchChunk = db.GetResourceStream(entry);
							foundMatch = true;
						}
						break;
					case 3:
						if (entry.typeId == typeID && entry.groupId == groupID)
						{
							//loadedCasPart = entry;
							matchChunk = db.GetResourceStream(entry);
							foundMatch = true;
						}
						break;
					case 2:
						if (entry.groupId == groupID)
						{
							//loadedCasPart = entry;
							matchChunk = db.GetResourceStream(entry);
							foundMatch = true;
						}
						break;
					case 1:
						if (entry.typeId == typeID)
						{
							//loadedCasPart = entry;
							matchChunk = db.GetResourceStream(entry);
							foundMatch = true;
						}
						break;
				}
				if (foundMatch)
				{
					break;
				}

			}
			

			return matchChunk;

		}

		public static Stream getStream(string filename, string keyString)
		{
			keyString = keyString.Replace("key:", "");
			string[] temp = keyString.Split(":".ToCharArray());

			int typeID = (int)MadScience.StringHelpers.ParseHex32("0x" + temp[0]);
			int groupID = (int)MadScience.StringHelpers.ParseHex32("0x" + temp[1]);
			long instanceID = (long)MadScience.StringHelpers.ParseHex64("0x" + temp[2]);

			return getStream(filename, typeID, groupID, instanceID);
		}

	}
}
