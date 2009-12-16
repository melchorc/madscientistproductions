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
        private string cleanName = "";

        public bool load(string filename)
        {
            FileStream s3pfile = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
            BinaryReader readFile = new BinaryReader(s3pfile);

            FileInfo f = new FileInfo(filename);
            this.cleanName = f.Name.Replace(f.Extension, "");

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

                string extension = tempName.Substring(tempName.LastIndexOf("."));
                if (extension == ".package")
                {
                    Sims3PackFileInfo s3fileInfo = new Sims3PackFileInfo();
                    s3fileInfo.name = tempName;
                    //s3fileInfo.cleanName = sanitiseString(tempName.Replace(".package", "")) + ".package";
                    s3fileInfo.offset = tempOffset;
                    s3fileInfo.length = length;

                    packagedFiles.Add(s3fileInfo);
                    s3fileInfo = null;
                }
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
            if (cleanNames && this.packagedFiles.Count == 1)
            {
                outputName = this.cleanName + ".package";
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

        public Stream create(Stream packageFile, string packageTitle)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.Append("<Sims3Package SubType=\"0x00000000\">");
            sb.Append("<ArchiveVersion>1.4</ArchiveVersion>");
            sb.Append("<CodeVersion>0.0.0.34</CodeVersion>");
            sb.Append("<GameVersion>0.0.0.0</GameVersion>");
            sb.Append("<PackageId>" + packageTitle + "</PackageId>");
            //sb.Append("<Date>" + dateTimePicker1.Value.ToString() + "</Date>");
            sb.Append("<AssetVersion>0</AssetVersion>");
            sb.Append("<MinReqVersion>1.0.0.0</MinReqVersion>");
            sb.Append("<DisplayName><![CDATA[" + packageTitle + "]]></DisplayName>");
            sb.Append("<Description><![CDATA[" + packageTitle + "]]></Description>");
            //sb.Append("<DisplayName>" + txtPatternTitle.Text + "</DisplayName>");
            //sb.Append("<Description>" + txtPatternDesc.Text + "</Description>");
            sb.Append("<Dependencies>");
            sb.Append("<Dependency>0x050cffe800000000050cffe800000000</Dependency>");
            //sb.Append("<Dependency>" + pName + "</Dependency>");
            sb.Append("</Dependencies>");
            sb.Append("<LocalizedNames>");
            sb.Append("<LocalizedName Language=\"en-US\"><![CDATA[" + packageTitle + "]]></LocalizedName>");
            sb.Append("</LocalizedNames>");
            sb.Append("<LocalizedDescriptions>");
            sb.Append("<LocalizedDescription Language=\"en-US\"><![CDATA[" + packageTitle + "]]></LocalizedDescription>");
            sb.Append("</LocalizedDescriptions>");
            sb.Append("<PackagedFile>");
            sb.Append("<Name>" + packageTitle + ".package</Name>");
            sb.Append("<Length>" + packageFile.Length + "</Length>");
            sb.Append("<Offset>0</Offset>");
            //sb.Append("    <Crc>317211BAD0B3E0F5</Crc>");
            sb.Append("<Guid>" + packageTitle + "</Guid>");
            //sb.Append("<ContentType>pattern</ContentType>");
            sb.Append("<MetaTags />");
            sb.Append("</PackagedFile>");
            sb.Append("</Sims3Package>");


            Stream sims3pack = new MemoryStream();

            String s3p_xml = sb.ToString();

            MadScience.StreamHelpers.WriteValueU32(sims3pack, 7);
            MadScience.StreamHelpers.WriteStringASCII(sims3pack, "TS3Pack");
            MadScience.StreamHelpers.WriteValueU16(sims3pack, 257);
            MadScience.StreamHelpers.WriteValueU32(sims3pack, (uint)s3p_xml.Length);
            MadScience.StreamHelpers.WriteStringUTF8(sims3pack, s3p_xml);

            MadScience.Helpers.CopyStream(packageFile, sims3pack);

            return sims3pack;
        }
    }

    class Sims3PackFileInfo
    {
        public string name = "";
        public int offset = 0;
        public int length = 0;
    }
}
