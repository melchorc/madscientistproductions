using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace QuickPackageMaker
{
    class Class1
    {
    }

            private typesToMeta lookupList;

        private void lookupTypes()
        {
            TextReader r = new StreamReader("metaTypes.xml");
            XmlSerializer s = new XmlSerializer(typeof(typesToMeta));
            this.lookupList = (typesToMeta)s.Deserialize(r);
            r.Close();
           
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

        public metaEntry(string ShortName, string LongName)
        {
            shortName = ShortName;
            longName = LongName;
        }
    }
}
