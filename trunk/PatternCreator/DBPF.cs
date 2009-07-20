using System;
using System.Collections;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace QuickPackageMaker
{
    class DBPF
    {
        string filename = "";
        public string errorMessage = "";
        public int indexType = 0;
        private typesToMeta lookupList;

        public ArrayList indexEntries = new ArrayList();
        public headerRecord packageHeader = new headerRecord();

        public void replaceIndex(int destIndex, indexRecord indexEntry)
        {
            indexEntries[destIndex] = indexEntry;
        }

        public void saveIndex()
        {

            FileStream dbpffile = new FileStream(filename, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            BinaryReader readFile = new BinaryReader(dbpffile);
            BinaryWriter writeFile = new BinaryWriter(dbpffile);

            string dbpfString = Encoding.ASCII.GetString(readFile.ReadBytes(4));
            bool longIdx = false;
            if (dbpfString == "DBBF")
            {
                longIdx = true;
            }

            int dbpfVersion = readFile.ReadInt32();
            dbpffile.Seek(7 * 4, SeekOrigin.Current);

            int entryCount = readFile.ReadInt32();
            int indexSize = 0;
            int indexSizeHi = 0;
            int indexStartV2 = 0;
            int indexStartHi = 0;

            if (longIdx == true)
            {
                indexSize = readFile.ReadInt32();
                indexSizeHi = readFile.ReadInt32();
                dbpffile.Seek(4 * 2, SeekOrigin.Current);
                indexStartV2 = readFile.ReadInt32();
                indexStartHi = readFile.ReadInt32();

            }
            else
            {

                // Skip old first entry offset
                readFile.ReadInt32();
                indexSize = readFile.ReadInt32();

                // Skip 4 entries
                dbpffile.Seek(4 * 4, SeekOrigin.Current);

                indexStartV2 = readFile.ReadInt32();
            }
            // Parse the index
            dbpffile.Seek(indexStartV2, SeekOrigin.Begin);

            indexType = readFile.ReadInt32();

            // Save the indexes
            uint globalTypeID = 0;
            uint globalGroupID = 0;
            int indexNull = 0;
            indexRecord indexEntry;

            switch (indexType)
            {
                // Sims 3 uses index types 0 to 3
                case 0:
                    for (int i = 0; i < entryCount; i++)
                    {
                        indexEntry = (indexRecord)indexEntries[i];
                        writeFile.Write(indexEntry.typeID);
                        writeFile.Write(indexEntry.groupID);
                        writeFile.Write(indexEntry.instanceHi);
                        writeFile.Write(indexEntry.instance);
                        writeFile.Write(indexEntry.offset);
                        //writeFile.Write(indexEntry.offsetHi);
                        writeFile.Write(indexEntry.size);
                        writeFile.Write(indexEntry.realsize);
                        writeFile.Write(indexEntry.flags);
                    }
                    break;
                //case 1:
                //    break;
                case 2:

                    indexEntry = (indexRecord)indexEntries[0];
                    writeFile.Write(indexEntry.groupID);

                    for (int i = 0; i < entryCount; i++)
                    {
                        indexEntry = (indexRecord)indexEntries[i];
                        writeFile.Write(indexEntry.typeID);
                        writeFile.Write(indexEntry.instanceHi);
                        writeFile.Write(indexEntry.instance);
                        writeFile.Write(indexEntry.offset);
                        //writeFile.Write(indexEntry.offsetHi);
                        writeFile.Write(indexEntry.size);
                        writeFile.Write(indexEntry.realsize);
                        writeFile.Write(indexEntry.flags);
                    }
                    break;
                case 3:
                    indexEntry = (indexRecord)indexEntries[0];
                    writeFile.Write(indexEntry.typeID);
                    writeFile.Write(indexEntry.groupID);

                    for (int i = 0; i < entryCount; i++)
                    {
                        indexEntry = (indexRecord)indexEntries[i];
                        writeFile.Write(indexEntry.instanceHi);
                        writeFile.Write(indexEntry.instance);
                        writeFile.Write(indexEntry.offset);
                        //writeFile.Write(indexEntry.offsetHi);
                        writeFile.Write(indexEntry.size);
                        writeFile.Write(indexEntry.realsize);
                        writeFile.Write(indexEntry.flags);
                    }
                    break;
                // Case 4 is used by both TS3 and Spore
                case 4:
                    if (longIdx == true)
                    {
                        // Spore?
                        writeFile.Write(indexNull);
                        for (int i = 0; i < entryCount; i++)
                        {
                            indexEntry = new indexRecord();
                            writeFile.Write(indexEntry.typeID);
                            writeFile.Write(indexEntry.groupID);
                            writeFile.Write(indexNull);
                            writeFile.Write(indexEntry.instance);
                            writeFile.Write(indexEntry.offset);
                            writeFile.Write(indexEntry.offsetHi);
                            writeFile.Write(indexEntry.size);
                            writeFile.Write(indexEntry.realsize);
                            writeFile.Write(indexEntry.flags);
                        }
                    }
                    else
                    {
                        // TS3
                        indexNull = readFile.ReadInt32();
                        if (indexNull != 0)
                        {
                            errorMessage = "Invalid index null";
                            break;
                        }
                        for (int i = 0; i < entryCount; i++)
                        {
                            indexEntry = new indexRecord();
                            indexEntry.typeID = readFile.ReadUInt32();
                            indexEntry.groupID = readFile.ReadUInt32();
                            indexEntry.instanceHi = 0;
                            indexEntry.instance = readFile.ReadUInt32();
                            indexEntry.offset = readFile.ReadUInt32();
                            indexEntry.offsetHi = 0;
                            indexEntry.size = readFile.ReadInt32() & 0x7FFFFFFF;
                            indexEntry.realsize = readFile.ReadUInt32();
                            indexEntry.flags = readFile.ReadInt32();
                            if ((indexEntry.flags & 0x0000ffff) != 0) { indexEntry.compressed = true; }
                            indexEntry.metaEntry = lookupList.lookup(indexEntry.typeID);

                            indexEntries.Add(indexEntry);
                        }
                    }
                    break;
                case 5:
                    if (12 + (entryCount * 24) != indexSize)
                    {
                        errorMessage = "Entry count/index size mismatch (type 5)";
                        break;
                    }

                    globalTypeID = readFile.ReadUInt32();

                    indexNull = readFile.ReadInt32();
                    if (indexNull != 0)
                    {
                        errorMessage = "Invalid index null";
                        break;
                    }
                    for (int i = 0; i < entryCount; i++)
                    {
                        indexEntry = new indexRecord();
                        indexEntry.typeID = globalTypeID;
                        indexEntry.groupID = readFile.ReadUInt32();
                        indexEntry.instanceHi = 0;
                        indexEntry.instance = readFile.ReadUInt32();
                        indexEntry.offset = readFile.ReadUInt32();
                        indexEntry.offsetHi = 0;
                        indexEntry.size = readFile.ReadInt32() & 0x7FFFFFFF;
                        indexEntry.realsize = readFile.ReadUInt32();
                        indexEntry.flags = readFile.ReadInt32();
                        if ((indexEntry.flags & 0x0000ffff) != 0) { indexEntry.compressed = true; }
                        indexEntry.metaEntry = lookupList.lookup(indexEntry.typeID);

                        indexEntries.Add(indexEntry);
                    }

                    break;
                case 6:
                    if (longIdx == true)
                    {
                        errorMessage = "Type 6 index is not implemented for DBBF format";
                        break;
                    }
                    if (12 + (entryCount * 24) != indexSize)
                    {
                        errorMessage = "Entry count/index size mismatch (type 6)";
                        break;
                    }
                    globalTypeID = readFile.ReadUInt32();

                    indexNull = readFile.ReadInt32();
                    if (indexNull != 0)
                    {
                        errorMessage = "Invalid index null";
                        break;
                    }
                    for (int i = 0; i < entryCount; i++)
                    {
                        indexEntry = new indexRecord();
                        indexEntry.typeID = globalTypeID;
                        indexEntry.groupID = readFile.ReadUInt32();
                        indexEntry.instanceHi = 0;
                        indexEntry.instance = readFile.ReadUInt32();
                        indexEntry.offset = readFile.ReadUInt32();
                        indexEntry.offsetHi = 0;
                        indexEntry.size = readFile.ReadInt32() & 0x7FFFFFFF;
                        indexEntry.realsize = readFile.ReadUInt32();
                        indexEntry.flags = readFile.ReadInt32();
                        if ((indexEntry.flags & 0x0000ffff) != 0) { indexEntry.compressed = true; }
                        indexEntry.metaEntry = lookupList.lookup(indexEntry.typeID);

                        indexEntries.Add(indexEntry);
                    }
                    break;
                case 7:
                    if (longIdx == true)
                    {
                        errorMessage = "Type 7 index is not implemented for DBBF format";
                        break;
                    }
                    if (16 + (entryCount * 20) != indexSize)
                    {
                        errorMessage = "Entry count/index size mismatch (type 7)";
                        break;
                    }
                    globalTypeID = readFile.ReadUInt32();
                    globalGroupID = readFile.ReadUInt32();

                    indexNull = readFile.ReadInt32();
                    if (indexNull != 0)
                    {
                        errorMessage = "Invalid index null";
                        break;
                    }
                    for (int i = 0; i < entryCount; i++)
                    {
                        indexEntry = new indexRecord();
                        indexEntry.typeID = globalTypeID;
                        indexEntry.groupID = globalGroupID;
                        indexEntry.instanceHi = 0;
                        indexEntry.instance = readFile.ReadUInt32();
                        indexEntry.offset = readFile.ReadUInt32();
                        indexEntry.offsetHi = 0;
                        indexEntry.size = readFile.ReadInt32() & 0x7FFFFFFF;
                        indexEntry.realsize = readFile.ReadUInt32();
                        indexEntry.flags = readFile.ReadInt32();
                        if ((indexEntry.flags & 0x0000ffff) != 0) { indexEntry.compressed = true; }
                        indexEntry.metaEntry = lookupList.lookup(indexEntry.typeID);
                        indexEntries.Add(indexEntry);
                    }
                    break;
                default:
                    errorMessage = "Unknown index type";
                    break;

            }


            writeFile.Close();
            readFile.Close();
            dbpffile.Close();
 

        }

        public bool open(string filename)
        {
            FileStream dbpffile = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
            BinaryReader readFile = new BinaryReader(dbpffile);

            string dbpfString = Encoding.ASCII.GetString(readFile.ReadBytes(4));
            if (dbpfString != "DBPF" && dbpfString != "DBBF")
            {
                errorMessage = "Not a DBPF/DBBF file";
                readFile.Close();
                dbpffile.Close();
                return false;
            }
            bool longIdx = false;
            if (dbpfString == "DBBF")
            {
                longIdx = true;
            }

            packageHeader.MajorVersion = readFile.ReadInt32();
            if (packageHeader.MajorVersion != 2 && packageHeader.MajorVersion != 3)
            {
                errorMessage = "Expecting version 2 or 3!";
                readFile.Close();
                dbpffile.Close();
                return false;
            }

            packageHeader.MinorVersion = readFile.ReadInt32();
            packageHeader.Unknown0C = readFile.ReadUInt32();
            packageHeader.Unknown10 = readFile.ReadUInt32();
            packageHeader.Unknown14 = readFile.ReadUInt32();
            packageHeader.Unknown18 = readFile.ReadUInt32();
            packageHeader.Unknown1C = readFile.ReadUInt32();
            packageHeader.Unknown20 = readFile.ReadUInt32();

            int entryCount = readFile.ReadInt32();
            if (entryCount == 0)
            {
                errorMessage = "Empty Archive.  Nothing to extract!";
                readFile.Close();
                dbpffile.Close();
                return false;
            }

            int indexSize = 0;
            int indexSizeHi = 0;
            int indexStartV2 = 0;
            int indexStartHi = 0;

            if (longIdx == true)
            {
                indexSize = readFile.ReadInt32();
                indexSizeHi = readFile.ReadInt32();
                dbpffile.Seek(4 * 2, SeekOrigin.Current);
                indexStartV2 = readFile.ReadInt32();
                indexStartHi = readFile.ReadInt32();

            }
            else
            {

                // Skip old first entry offset
                readFile.ReadInt32();
                indexSize = readFile.ReadInt32();

                // Skip 4 entries
                dbpffile.Seek(4 * 4, SeekOrigin.Current);

                indexStartV2 = readFile.ReadInt32();
            }
            // Parse the index
            dbpffile.Seek(indexStartV2, SeekOrigin.Begin);

            indexType = readFile.ReadInt32();

            lookupTypes();

            uint globalTypeID = 0;
            uint globalGroupID = 0;
            int indexNull = 0;
            indexRecord indexEntry;

            switch (indexType)
            {
                // Sims 3 uses index types 0 to 3
                case 0:
                    if (4 + (entryCount * 32) != indexSize) {
                        errorMessage = "Entry count/index size mismatch (type 0)";
                        break;
                    }
                    for (int i = 0; i < entryCount; i++)
                    {
                        indexEntry = new indexRecord();
                        indexEntry.typeID = readFile.ReadUInt32();
                        indexEntry.groupID = readFile.ReadUInt32();
                        indexEntry.instanceHi = readFile.ReadUInt32();
                        indexEntry.instance = readFile.ReadUInt32();
                        indexEntry.offset = readFile.ReadUInt32();
                        indexEntry.offsetHi = 0;
                        indexEntry.size = readFile.ReadInt32() & 0x7FFFFFFF;
                        indexEntry.realsize = readFile.ReadUInt32();
                        indexEntry.flags = readFile.ReadInt32();
                        if ((indexEntry.flags & 0x0000ffff) != 0) { indexEntry.compressed = true; }
                        indexEntry.metaEntry = lookupList.lookup(indexEntry.typeID);

                        indexEntries.Add(indexEntry);
                    }
                    break;
                //case 1:
                //    break;
                case 2:
                    if (8 + (entryCount * 28) != indexSize) 
                    {
                        errorMessage = "Entry count/index size mismatch (type 2)";
                        break;
                    }
                    globalGroupID = readFile.ReadUInt32();
                    for (int i = 0; i < entryCount; i++)
                    {
                        indexEntry = new indexRecord();
                        indexEntry.typeID = readFile.ReadUInt32();
                        indexEntry.groupID = globalGroupID;
                        indexEntry.instanceHi = readFile.ReadUInt32();
                        indexEntry.instance = readFile.ReadUInt32();
                        indexEntry.offset = readFile.ReadUInt32();
                        indexEntry.offsetHi = 0;
                        indexEntry.size = readFile.ReadInt32() & 0x7FFFFFFF;
                        indexEntry.realsize = readFile.ReadUInt32();
                        indexEntry.flags = readFile.ReadInt32();
                        if ((indexEntry.flags & 0x0000ffff) != 0) { indexEntry.compressed = true; }
                        indexEntry.metaEntry = lookupList.lookup(indexEntry.typeID);

                        indexEntries.Add(indexEntry);
                    }
                    break;
                case 3:
                    if (12 + (entryCount * 24) != indexSize)
                    {
                        errorMessage = "Entry count/index size mismatch (type 3)";
                        break;
                    }
                    globalTypeID = readFile.ReadUInt32();
                    globalGroupID = readFile.ReadUInt32();
                    for (int i = 0; i < entryCount; i++)
                    {
                        indexEntry = new indexRecord();
                        indexEntry.typeID = globalTypeID;
                        indexEntry.groupID = globalGroupID;
                        indexEntry.instanceHi = readFile.ReadUInt32();
                        indexEntry.instance = readFile.ReadUInt32();
                        indexEntry.offset = readFile.ReadUInt32();
                        indexEntry.offsetHi = 0;
                        indexEntry.size = readFile.ReadInt32() & 0x7FFFFFFF;
                        indexEntry.realsize = readFile.ReadUInt32();
                        indexEntry.flags = readFile.ReadInt32();
                        if ((indexEntry.flags & 0x0000ffff) != 0) { indexEntry.compressed = true; }
                        indexEntry.metaEntry = lookupList.lookup(indexEntry.typeID);

                        indexEntries.Add(indexEntry);
                    }
                    break;
                // Case 4 is used by both TS3 and Spore
                case 4:
                    if (longIdx == true)
                    {
                        // Spore?
                        if (8 + (entryCount * 32) != indexSize)
                        {
                            errorMessage = "Entry count/index size mismatch (type 4)";
                            break;
                        }
                        indexNull = readFile.ReadInt32();
                        if (indexNull != 0)
                        {
                            errorMessage = "Invalid index null";
                            break;
                        }
                        for (int i = 0; i < entryCount; i++)
                        {
                            indexEntry = new indexRecord();
                            indexEntry.typeID = readFile.ReadUInt32();
                            indexEntry.groupID = readFile.ReadUInt32();
                            indexEntry.instanceHi = 0;
                            indexEntry.instance = readFile.ReadUInt32();
                            indexEntry.offset = readFile.ReadUInt32();
                            indexEntry.offsetHi = readFile.ReadUInt32();
                            indexEntry.size = readFile.ReadInt32();
                            indexEntry.realsize = readFile.ReadUInt32();
                            indexEntry.flags = readFile.ReadInt32();
                            if ((indexEntry.flags & 0x0000ffff) != 0) { indexEntry.compressed = true; }
                            indexEntry.metaEntry = lookupList.lookup(indexEntry.typeID);

                            indexEntries.Add(indexEntry);
                        }
                    }
                    else
                    {
                        // TS3
                        if (8 + (entryCount * 28) != indexSize)
                        {
                            errorMessage = "Entry count/index size mismatch (type 4)";
                            break;
                        }
                        indexNull = readFile.ReadInt32();
                        if (indexNull != 0)
                        {
                            errorMessage = "Invalid index null";
                            break;
                        }
                        for (int i = 0; i < entryCount; i++)
                        {
                            indexEntry = new indexRecord();
                            indexEntry.typeID = readFile.ReadUInt32();
                            indexEntry.groupID = readFile.ReadUInt32();
                            indexEntry.instanceHi = 0;
                            indexEntry.instance = readFile.ReadUInt32();
                            indexEntry.offset = readFile.ReadUInt32();
                            indexEntry.offsetHi = 0;
                            indexEntry.size = readFile.ReadInt32() & 0x7FFFFFFF;
                            indexEntry.realsize = readFile.ReadUInt32();
                            indexEntry.flags = readFile.ReadInt32();
                            if ((indexEntry.flags & 0x0000ffff) != 0) { indexEntry.compressed = true; }
                            indexEntry.metaEntry = lookupList.lookup(indexEntry.typeID);

                            indexEntries.Add(indexEntry);
                        }
                    }
                    break;
                case 5:
                    if (longIdx == true)
                    {
                        errorMessage = "Type 5 index is not implemented for DBBF format";
                        break;
                    }
                    if (12 + (entryCount * 24) != indexSize)
                    {
                        errorMessage = "Entry count/index size mismatch (type 5)";
                        break;
                    }

                    globalTypeID = readFile.ReadUInt32();

                    indexNull = readFile.ReadInt32();
                    if (indexNull != 0)
                    {
                        errorMessage = "Invalid index null";
                        break;
                    }
                    for (int i = 0; i < entryCount; i++)
                    {
                        indexEntry = new indexRecord();
                        indexEntry.typeID = globalTypeID;
                        indexEntry.groupID = readFile.ReadUInt32();
                        indexEntry.instanceHi = 0;
                        indexEntry.instance = readFile.ReadUInt32();
                        indexEntry.offset = readFile.ReadUInt32();
                        indexEntry.offsetHi = 0;
                        indexEntry.size = readFile.ReadInt32() & 0x7FFFFFFF;
                        indexEntry.realsize = readFile.ReadUInt32();
                        indexEntry.flags = readFile.ReadInt32();
                        if ((indexEntry.flags & 0x0000ffff) != 0) { indexEntry.compressed = true; }
                        indexEntry.metaEntry = lookupList.lookup(indexEntry.typeID);

                        indexEntries.Add(indexEntry);
                    }

                    break;
                case 6:
                    if (longIdx == true)
                    {
                        errorMessage = "Type 6 index is not implemented for DBBF format";
                        break;
                    }
                    if (12 + (entryCount * 24) != indexSize)
                    {
                        errorMessage = "Entry count/index size mismatch (type 6)";
                        break;
                    }
                    globalTypeID = readFile.ReadUInt32();

                    indexNull = readFile.ReadInt32();
                    if (indexNull != 0)
                    {
                        errorMessage = "Invalid index null";
                        break;
                    }
                    for (int i = 0; i < entryCount; i++)
                    {
                        indexEntry = new indexRecord();
                        indexEntry.typeID = globalTypeID;
                        indexEntry.groupID = readFile.ReadUInt32();
                        indexEntry.instanceHi = 0;
                        indexEntry.instance = readFile.ReadUInt32();
                        indexEntry.offset = readFile.ReadUInt32();
                        indexEntry.offsetHi = 0;
                        indexEntry.size = readFile.ReadInt32() & 0x7FFFFFFF;
                        indexEntry.realsize = readFile.ReadUInt32();
                        indexEntry.flags = readFile.ReadInt32();
                        if ((indexEntry.flags & 0x0000ffff) != 0) { indexEntry.compressed = true; }
                        indexEntry.metaEntry = lookupList.lookup(indexEntry.typeID);

                        indexEntries.Add(indexEntry);
                    }
                    break;
                case 7:
                    if (longIdx == true)
                    {
                        errorMessage = "Type 7 index is not implemented for DBBF format";
                        break;
                    }
                    if (16 + (entryCount * 20) != indexSize)
                    {
                        errorMessage = "Entry count/index size mismatch (type 7)";
                        break;
                    }
                    globalTypeID = readFile.ReadUInt32();
                    globalGroupID = readFile.ReadUInt32();

                    indexNull = readFile.ReadInt32();
                    if (indexNull != 0)
                    {
                        errorMessage = "Invalid index null";
                        break;
                    }
                    for (int i = 0; i < entryCount; i++)
                    {
                        indexEntry = new indexRecord();
                        indexEntry.typeID = globalTypeID;
                        indexEntry.groupID = globalGroupID;
                        indexEntry.instanceHi = 0;
                        indexEntry.instance = readFile.ReadUInt32();
                        indexEntry.offset = readFile.ReadUInt32();
                        indexEntry.offsetHi = 0;
                        indexEntry.size = readFile.ReadInt32() & 0x7FFFFFFF;
                        indexEntry.realsize = readFile.ReadUInt32();
                        indexEntry.flags = readFile.ReadInt32();
                        if ((indexEntry.flags & 0x0000ffff) != 0) { indexEntry.compressed = true; }
                        indexEntry.metaEntry = lookupList.lookup(indexEntry.typeID);
                        indexEntries.Add(indexEntry);
                    }
                    break;
                default:
                    errorMessage = "Unknown index type";
                    break;
                 
            }

            if (errorMessage != "")
            {
                readFile.Close();
                dbpffile.Close();
                return false;
            }

            this.filename = filename;

            readFile.Close();
            dbpffile.Close();
        
            return true;
        }

        public struct indexRecord
        {
            //public int indexType;
            public uint typeID;
            public uint groupID;
            public uint instanceHi;
            public uint instance;
            public uint offset;
            public uint offsetHi;
            public int size;
            public bool compressed;
            public uint realsize;
            public int flags;
            public metaEntry metaEntry;
        }

        public struct headerRecord
        {
            public int MajorVersion;	// 04
            public int MinorVersion;	// 08
            public uint Unknown0C;		// 0C
            public uint Unknown10;		// 10
            public uint Unknown14;		// 14 - always 0?
            public uint Unknown18;		// 18 - always 0?
            public uint Unknown1C;		// 1C - always 0?
            public uint Unknown20;		// 20
            public int IndexCount;		// 24 - Number of index entries in the package.
            public uint Unknown28;		// 28
            public int IndexSize;		// 2C - The total size in bytes of index entries.
            public uint Unknown30;		// 30
            public uint Unknown34;		// 34
            public uint Unknown38;		// 38
            public uint Always3;		// 3C - Always 3?
            public int IndexOffset;		// 40 - Absolute offset in package to the index header.
            public uint Unknown44;		// 44
            public uint Unknown48;		// 48
            public uint Unknown4C;		// 4C
            public uint Unknown50;		// 50
            public uint Unknown54;		// 54
            public uint Unknown58;		// 58
            public uint Unknown5C;		// 5C
        }

        private void lookupTypes()
        {
            TextReader r = new StreamReader("metaTypes.xml");
            XmlSerializer s = new XmlSerializer(typeof(typesToMeta));
            this.lookupList = (typesToMeta)s.Deserialize(r);
            r.Close();
           
        }

    }

    [XmlRoot("typesToMeta")]
    public class typesToMeta
    {

        private ArrayList metaTypes;
        private Hashtable metaTypes2;

        public typesToMeta()
        {
            metaTypes = new ArrayList();
            metaTypes2 = new Hashtable();
        }

        public metaEntry lookup(uint typeID)
        {
            return (metaEntry)metaTypes2[typeID];
        }

        [XmlElement("entry")]
        public metaEntry[] Entries
        {
            get
            {
                metaEntry[] entries = new metaEntry[metaTypes.Count];
                metaTypes.CopyTo(entries);
                return entries;
            }
            set
            {
                if (value == null) return;

                metaEntry[] entries = (metaEntry[])value;
                metaTypes2.Clear();
                foreach (metaEntry entry in entries)
                {
                    metaTypes2.Add(entry.key, entry);
                }


            }
        }
        /*
        public int AddItem(metaEntry entry)
        {
            return metaTypes.Add(entry);
        }
        */
    }

    public class metaEntry
    {
        [XmlAttribute("key")]
        public uint key;

        [XmlElement("shortName")]
        public string shortName;

        [XmlElement("longName")]
        public string longName;

        public metaEntry()
        {
        }

        public metaEntry(string ShortName, string LongName)
        {
            shortName = ShortName;
            longName = LongName;
        }
    }
}
