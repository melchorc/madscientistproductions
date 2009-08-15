using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Xml.XPath;

namespace Sims3Pack_Multi_Installer
{
    class Sims3Pack
    {
        public string rawXML = "";
        public string errorMessage = "";
        public ArrayList packagedFiles = new ArrayList();

        private string filename = "";

        public bool load(string filename)
        {
            FileStream s3pfile = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
            BinaryReader readFile = new BinaryReader(s3pfile);

            uint version = readFile.ReadUInt32();
            string s3pack = Encoding.ASCII.GetString(readFile.ReadBytes(7));
            if (s3pack != "TS3Pack")
            {
                errorMessage = "Not a Sims3Pack file";
                readFile.Close();
                s3pfile.Close();
                return false;
            }
            this.filename = filename;

            uint unknown = readFile.ReadUInt16();

            int xmlLength = readFile.ReadInt32();

            byte[] xmlBytes = readFile.ReadBytes(xmlLength);

            MemoryStream mem = new MemoryStream(xmlBytes);
            XPathDocument doc = new XPathDocument(mem);

            Stack nameStack = getXmlFields(doc, "//PackagedFile/Name");
            Stack offsetStack = getXmlFields(doc, "//PackagedFile/Offset");
            Stack lengthStack = getXmlFields(doc, "//PackagedFile/Length");

            int sCount = nameStack.Count;
            for (int i = 0; i < sCount; i++)
            {
                string tempName = nameStack.Pop().ToString();
                int tempOffset = Convert.ToInt32(offsetStack.Pop().ToString());
                int length = Convert.ToInt32(lengthStack.Pop().ToString());

                Sims3PackFileInfo s3fileInfo = new Sims3PackFileInfo();
                s3fileInfo.name = tempName;
                s3fileInfo.cleanName = sanitiseString(tempName.Replace(".package", "")) + ".package";
                s3fileInfo.offset = tempOffset;
                s3fileInfo.length = length;

                packagedFiles.Add(s3fileInfo);
                s3fileInfo = null;
            }


            //string xmlStuff = Encoding.ASCII.GetString(xmlBytes);

            // Close the files since we'll be working purely with the XML here.
            readFile.Close();
            s3pfile.Close();

            //rawXML = xmlStuff;

            return true;
        }

        private Stack getXmlFields(XPathDocument doc, string xpath)
        {
            Stack myStack = new Stack();

            XPathNavigator nav = doc.CreateNavigator();
            XPathExpression expr;

            expr = nav.Compile(xpath);
            XPathNodeIterator iterator = nav.Select(expr);

            try
            {
                while (iterator.MoveNext())
                {
                    XPathNavigator nav2 = iterator.Current.Clone();
                    myStack.Push(nav2.Value);                                        
                    //Console.WriteLine(nav2.Value);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return myStack;
        }

        public bool extractFile(Sims3PackFileInfo file, string destinationFolder)
        {
            return extractFile(file, destinationFolder, true);
        }

        private string sanitiseString(string input)
        {
            string temp = Regex.Replace(input, "[^a-zA-Z0-9]", "");
            return temp;
            //var s = from ch in input where char.IsLetterOrDigit(ch) select ch;
            //return UnicodeEncoding.ASCII.GetString(UnicodeEncoding.ASCII.GetBytes(s.ToArray()));
        }

        public bool extractFile(Sims3PackFileInfo file, string destinationFolder, bool cleanNames )
        {
            FileStream s3pfile = new FileStream(this.filename, FileMode.Open, FileAccess.Read, FileShare.Read);
            BinaryReader readFile = new BinaryReader(s3pfile);

            uint version = readFile.ReadUInt32();
            string s3pack = Encoding.ASCII.GetString(readFile.ReadBytes(7));
            if (s3pack != "TS3Pack")
            {
                errorMessage = "Not a Sims3Pack file";
                readFile.Close();
                s3pfile.Close();
                return false;
            }

            uint unknown = readFile.ReadUInt16();

            int xmlLength = readFile.ReadInt32();
            s3pfile.Seek(xmlLength + file.offset, SeekOrigin.Current);

            string outputName = "";
            if (cleanNames)
            {
                outputName = file.cleanName;
            }
            else
            {
                outputName = file.name;
            }

            FileStream destFile = new FileStream(destinationFolder + "\\" + outputName, FileMode.Create, FileAccess.Write, FileShare.Write);
            BinaryWriter writeFile = new BinaryWriter(destFile);

            writeFile.Write(readFile.ReadBytes(file.length));

            writeFile.Close();
            destFile.Close();

            readFile.Close();
            s3pfile.Close();

            return true;
        }
    }

    class Sims3PackFileInfo
    {
        public string name = "";
        public string cleanName = "";
        public int offset = 0;
        public int length = 0;
    }
}
