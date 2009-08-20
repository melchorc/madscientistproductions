﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Drawing;
using System.Xml.Serialization;
using System.Windows.Forms;

using MadScience;
using MadScience.Wrappers;


namespace CASPartEditor
{
    public partial class Form1 : Form
    {

        public void reloadTextures()
        {
            if (listView1.SelectedItems.Count == 1 && cEnable3DPreview.Checked == true)
            {

                xmlChunkDetails details = (xmlChunkDetails)casPartNew.xmlChunk[listView1.SelectedIndices[0]];

                renderWindow1.RenderEnabled = false;

                DateTime startTime = DateTime.Now;
                toolStripStatusLabel1.Text = "Reloading 3d view... please wait...";
                statusStrip1.Refresh();

                reloadTextures(details);


                DateTime stopTime = DateTime.Now;
                TimeSpan duration = stopTime - startTime;

                toolStripStatusLabel1.Text = "Reloaded 3D in " + duration.Milliseconds + "ms";

                renderWindow1.RenderEnabled = true;
            }
        }

        public void reloadTextures(xmlChunkDetails details)
        {
            List<string> textures = findDefaultTextures(casPartNew.ageGenderFlag, casPartNew.typeFlag);

            renderWindow1.loadTexture(KeyUtils.findKey(textures[0]), "skinTexture");
            renderWindow1.loadTexture(KeyUtils.findKey(textures[1]), "skinSpecular");
            renderWindow1.loadTexture(KeyUtils.findKey(textures[2]), "normalMap");

            renderWindow1.loadTexture(KeyUtils.findKey(details.ClothingAmbient), "ambientTexture");
            renderWindow1.loadTexture(KeyUtils.findKey(details.ClothingSpecular), "specularTexture");
            renderWindow1.loadTexture(KeyUtils.findKey(details.Multiplier), "baseTexture");
            renderWindow1.loadTexture(null, "stencilA");

            if ((casPartNew.typeFlag & 0x4) == 0x4)
            {
                renderWindow1.shaderMode = 1;
            }
            else
            {
                renderWindow1.shaderMode = 0;
            }

            generate3DTexture(details);
            
            renderWindow1.resetDevice();

        }

        public void startRender(xmlChunkDetails details)
        {

            Stream meshStream = Stream.Null;

            // Use the VPXY to get the mesh lod
            Stream vpxyStream = KeyUtils.findKey(casPartNew.tgi64list[casPartNew.tgiIndexVPXY], 0);

            if (vpxyStream != null)
            {
                VPXYFile vpxyFile = new VPXYFile(vpxyStream);
                // Get the first VPXY internal link
                if (vpxyFile.vpxy.linkEntries.Count >= 1 && vpxyFile.vpxy.linkEntries[0].tgiList.Count >= 1)
                {
                    meshStream = KeyUtils.findKey(vpxyFile.vpxy.linkEntries[0].tgiList[0], 0);
                }
                vpxyStream.Close();
            }

            if (meshStream == Stream.Null)
            {
                ResourceKey findKey = findDefaultMeshes(casPartNew.ageGenderFlag, casPartNew.typeFlag);
                
                if (findKey.groupId != 0 && findKey.instanceId != 0)
                {
                    meshStream = KeyUtils.findKey(findKey, 0);
                }
            }

            if (meshStream != Stream.Null)
            {

                MadScience.Render.modelInfo newModel = MadScience.Render.Helpers.geomToModel(meshStream);
                newModel.name = txtMeshName.Text;
                renderWindow1.BackgroundColour = MadScience.Colours.convertColour(MadScience.Helpers.getRegistryValue("renderBackgroundColour"));
                //renderWindow1.loadDefaultTextures();
                renderWindow1.setModel(newModel);

                reloadTextures(details);

                renderWindow1.RenderEnabled = true;


            }
            else
            {
                renderWindow1.statusLabel.Text = "Sorry, we could not find a mesh!";
            }

            meshStream.Close();
        }

        private void generate3DTexture(xmlChunkDetails details)
        {
            renderWindow1.lblGeneratingTexture.Visible = true;
            if (!bwGenTexture.IsBusy)
                bwGenTexture.RunWorkerAsync(details);

        }

        private void bwGenTexture_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            xmlChunkDetails details = (xmlChunkDetails)e.Argument;
            Bitmap b;

            if ((casPartNew.typeFlag & 0x4) == 0x4)
            {
                b = composeMakeup(details, casPartNew.clothingType);
            }
            else
            {
                b = composeMultiplier(details, details.filename != "CasRgbMask");
            }
            Object[] a = new Object[2];
            a[0] = details;
            a[1] = b;
            e.Result = a;
        }

        private void bwGenTexture_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            Object[] a = (Object[])e.Result;
            if (renderWindow1.RenderEnabled)
            {
                //
                //renderWindow1.RenderEnabled = true;
                if ((casPartNew.typeFlag & 0x4) == 0x4)
                {
                    renderWindow1.loadTextureFromBitmap((Bitmap)a[1], "stencilA");
                    renderWindow1.shaderMode = 1;
                }
                else
                {
                    renderWindow1.loadTextureFromBitmap((Bitmap)a[1], "baseTexture");
                    loadStencils((xmlChunkDetails)a[0]);
                    renderWindow1.shaderMode = 0;
                }
                renderWindow1.resetDevice();
                renderWindow1.lblGeneratingTexture.Visible = false;
            }
            //if the user changed the selection while processing, we need to restart
            if (listView1.SelectedIndices.Count > 0)
            {
                if (a[0] != (xmlChunkDetails)casPartNew.xmlChunk[listView1.SelectedIndices[0]])
                    generate3DTexture((xmlChunkDetails)casPartNew.xmlChunk[listView1.SelectedIndices[0]]);
            }
        }

        private void loadStencils(xmlChunkDetails details)
        {
            List<Stream> stencils = new List<Stream>();

            //Stream[] stencils = new Stream[6];
            if (details.stencil.A.Enabled == "True") stencils.Add(KeyUtils.findKey(details.stencil.A.key));
            if (details.stencil.B.Enabled == "True") stencils.Add(KeyUtils.findKey(details.stencil.B.key));
            if (details.stencil.C.Enabled == "True") stencils.Add(KeyUtils.findKey(details.stencil.C.key));
            if (details.stencil.D.Enabled == "True") stencils.Add(KeyUtils.findKey(details.stencil.D.key));
            if (details.stencil.E.Enabled == "True") stencils.Add(KeyUtils.findKey(details.stencil.E.key));
            if (details.stencil.F.Enabled == "True") stencils.Add(KeyUtils.findKey(details.stencil.F.key));

            if (stencils.Count == 0)
                return;
            if (stencils.Count == 1)
                renderWindow1.loadTexture(stencils[0],"stencilA");
            if (stencils.Count > 1)
                renderWindow1.loadTextureFromBitmap(PatternProcessor.mergeStencils(stencils), "stencilA");
        }

        private Bitmap composeMakeup(xmlChunkDetails details, uint casPartType)
        {
            List<string> skinTextures = findDefaultTextures(casPartNew.ageGenderFlag, casPartNew.typeFlag);
            DateTime startTime2 = DateTime.Now;
            List<MadScience.Wrappers.ResourceKey> tempList = new List<MadScience.Wrappers.ResourceKey>();
            tempList.Add(new MadScience.Wrappers.ResourceKey(details.faceOverlay));
            tempList.Add(new MadScience.Wrappers.ResourceKey(details.Mask));

            List<Stream> textures = KeyUtils.findKey(tempList, 2);
            DateTime stopTime2 = DateTime.Now;
            TimeSpan duration2 = stopTime2 - startTime2;
            Console.WriteLine("Key search time: " + duration2.TotalMilliseconds);

            DateTime startTime = DateTime.Now;
            Bitmap output = null;
            if (details.tint.A.enabled.ToLower() == "true")
            {
                output = PatternProcessor.ProcessMakeupTexture(textures,
                casPartNew.clothingType,
                details.tint.A,
                details.tint.B,
                details.tint.C,
                details.tint.D,
                    true);
            }
            else if (details.TintColor != null)
            {
                tintDetail t = new tintDetail();
                tintDetail d = new tintDetail();
                t.enabled = "True";
                t.color = details.TintColor;
                output = PatternProcessor.ProcessMakeupTexture(textures,
                    casPartNew.clothingType,
                    t,
                    d,
                    d,
                    d,
                    false);
            }

            DateTime stopTime = DateTime.Now;
            TimeSpan duration = stopTime - startTime;
            Console.WriteLine("Total Makeup Texture generation time: " + duration.TotalMilliseconds);
            return output;
        }

        private Bitmap composeMultiplier(xmlChunkDetails details, bool RGBA)
        {
            Color[] pattern1colors = createColorArray(details.pattern[0]);
            Color[] pattern2colors = createColorArray(details.pattern[1]);
            Color[] pattern3colors = createColorArray(details.pattern[2]);
            Color[] pattern4colors = createColorArray(details.pattern[3]);
            Bitmap[] hsvPatterns = new Bitmap[4];

            DateTime startTime2 = DateTime.Now;
            List<MadScience.Wrappers.ResourceKey> tempList = new List<MadScience.Wrappers.ResourceKey>();
            tempList.Add(new MadScience.Wrappers.ResourceKey(details.Multiplier));
            tempList.Add(new MadScience.Wrappers.ResourceKey(details.Mask));
            tempList.Add(new MadScience.Wrappers.ResourceKey(details.Overlay));

            tempList.Add(new MadScience.Wrappers.ResourceKey(details.pattern[0].rgbmask));
            tempList.Add(new MadScience.Wrappers.ResourceKey(details.pattern[1].rgbmask));
            tempList.Add(new MadScience.Wrappers.ResourceKey(details.pattern[2].rgbmask));
            tempList.Add(new MadScience.Wrappers.ResourceKey(details.pattern[3].rgbmask));

            List<Stream> textures = KeyUtils.findKey(tempList, 2);

            // If any of the patterns can't be found, then check if they are a custom pattern and load them
            if (textures[3].Length == 0 && details.pattern[0].Enabled == "True")
            {
                textures[3] = Patterns.findPattern(details.pattern[0]);
            }
            if (textures[4].Length == 0 && details.pattern[1].Enabled == "True")
            {
                //textures[4] = pBrowser.findPattern(pBrowser.findPattern(details.pattern[1].key));
                textures[4] = Patterns.findPattern(details.pattern[1]);
            }
            if (textures[5].Length == 0 && details.pattern[2].Enabled == "True")
            {
                //textures[5] = pBrowser.findPattern(pBrowser.findPattern(details.pattern[2].key));
                textures[5] = Patterns.findPattern(details.pattern[2]);
            }
            if (textures[6].Length == 0 && details.pattern[3].Enabled == "True")
            {
                //textures[6] = pBrowser.findPattern(pBrowser.findPattern(details.pattern[3].key));
                textures[6] = Patterns.findPattern(details.pattern[3]);
            }

            //process hsv Patterns

            if (details.pattern[0].type == "HSV")
            {
                //hsvPatterns[0] = (Bitmap)this.pBrowser.makePatternThumb(pBrowser.findPattern(details.pattern[0].key), pBrowser.pDetailsTopFile(details.pattern[0]));
                hsvPatterns[0] = (Bitmap)Patterns.makePatternThumb(details.pattern[0]);
            }
            if (details.pattern[1].type == "HSV")
            {
                //hsvPatterns[1] = (Bitmap)this.pBrowser.makePatternThumb(pBrowser.findPattern(details.pattern[1].key), pBrowser.pDetailsTopFile(details.pattern[1]));
                hsvPatterns[1] = (Bitmap)Patterns.makePatternThumb(details.pattern[1]);
            }
            if (details.pattern[2].type == "HSV")
            {
                //hsvPatterns[2] = (Bitmap)this.pBrowser.makePatternThumb(pBrowser.findPattern(details.pattern[2].key), pBrowser.pDetailsTopFile(details.pattern[2]));
                hsvPatterns[2] = (Bitmap)Patterns.makePatternThumb(details.pattern[2]);
            }
            if (details.pattern[3].type == "HSV")
            {
                //hsvPatterns[3] = (Bitmap)this.pBrowser.makePatternThumb(pBrowser.findPattern(details.pattern[3].key), pBrowser.pDetailsTopFile(details.pattern[3]));
                hsvPatterns[3] = (Bitmap)Patterns.makePatternThumb(details.pattern[3]);
            }

            DateTime stopTime2 = DateTime.Now;
            TimeSpan duration2 = stopTime2 - startTime2;
            Console.WriteLine("Key search time: " + duration2.TotalMilliseconds);

            DateTime startTime = DateTime.Now;
            Bitmap output = PatternProcessor.ProcessTexture(
                textures,
                hsvPatterns,
                pattern1colors, pattern2colors, pattern3colors, pattern4colors, RGBA);

            DateTime stopTime = DateTime.Now;
            TimeSpan duration = stopTime - startTime;
            Console.WriteLine("Total Multiplier Texture generation time: " + duration.TotalMilliseconds);
            return output;
        }

        private Color[] createColorArray(patternDetails pDetail)
        {
            Color[] colors = new Color[1];

            if (pDetail.Enabled.ToLower() == "false")
            {
                colors[0] = Color.Empty;
                return colors;
            }

            if (pDetail.type == "solidColor")
            {
                colors[0] = MadScience.Colours.convertColour(pDetail.Color);
            }
            if (pDetail.type == "HSV")
            {
                //we process the pattern in the pattern preview code, so we multiplicate it with white so that there is no change
                colors[0] = Color.White;
            }
            if (pDetail.type == "Coloured")
            {
                // Always copy directly - we check individual colours in the pattern processor
                colors = new Color[5];
                colors[0] = MadScience.Colours.convertColour(pDetail.ColorP[0], true);
                colors[1] = MadScience.Colours.convertColour(pDetail.ColorP[1], true);
                colors[2] = MadScience.Colours.convertColour(pDetail.ColorP[2], true);
                colors[3] = MadScience.Colours.convertColour(pDetail.ColorP[3], true);
                colors[4] = MadScience.Colours.convertColour(pDetail.ColorP[4], true);

            }
            return colors;
        }

        public static Dictionary<string, string> defaultMeshes = new Dictionary<string, string>();
        public static ResourceKey findDefaultMeshes(uint ageGenderFlag, uint typeFlag)
        {
            ResourceKey ret = new ResourceKey();

            // Load in XML
            if (defaultMeshes.Count == 0)
            {
                // Load in XML
                TextReader r = new StreamReader(Path.Combine(Application.StartupPath, "xml\\defaultMeshes.xml"));
                XmlSerializer s = new XmlSerializer(typeof(meshesFile));
                DeserializeMeshes(r, defaultMeshes);
                r.Close();
            }

            string flags = "";
            string highestAge = "";

            if ((ageGenderFlag & (uint)AgeGenderFlags.Toddler) == (uint)AgeGenderFlags.Toddler) highestAge = "p";
            if ((ageGenderFlag & (uint)AgeGenderFlags.Child) == (uint)AgeGenderFlags.Child) highestAge = "c";
            if ((ageGenderFlag & (uint)AgeGenderFlags.Teen) == (uint)AgeGenderFlags.Teen) highestAge = "t";
            if ((ageGenderFlag & (uint)AgeGenderFlags.YoungAdult) == (uint)AgeGenderFlags.YoungAdult) highestAge = "y";
            if ((ageGenderFlag & (uint)AgeGenderFlags.Adult) == (uint)AgeGenderFlags.Adult) highestAge = "a";
            if ((ageGenderFlag & (uint)AgeGenderFlags.Elder) == (uint)AgeGenderFlags.Elder) highestAge = "e";

            flags = highestAge;

            //just default to male for now
            if ((ageGenderFlag & (uint)AgeGenderFlags.Male) == (uint)AgeGenderFlags.Male) flags += "m";
            else if ((ageGenderFlag & (uint)AgeGenderFlags.Female) == (uint)AgeGenderFlags.Female) flags += "f";

            // Face Overlay (ie Makeup)
            if ((typeFlag & 0x4) == 0x4) flags += "Face";

            // Body
            if ((typeFlag & 0x8) == 0x8) flags += "Body";

            // Accessory
            if ((typeFlag & 0x10) == 0x10) flags += "Accessory";

            if (defaultMeshes.ContainsKey(flags))
            {
                ret = new ResourceKey(defaultMeshes[flags]);
            }

            return ret;
        }

        public static Dictionary<string, string> defaultTextures = new Dictionary<string, string>();
        public static List<string> findDefaultTextures(uint ageGenderFlag, uint typeFlag)
        {

            // textures[0] = skinTexture
            // textures[1] = skinSpecular
            // textures[2] = normalMap
            List<string> returnTextures = new List<string>(3);
            for (int i = 0; i < 3; i++)
            {
                returnTextures.Add("");
            }

            if (defaultTextures.Count == 0)
            {
                // Load in XML
                TextReader r = new StreamReader(Path.Combine(Application.StartupPath, "xml\\defaultTextures.xml"));
                Deserialize(r, defaultTextures);
                r.Close();
            }

            string flags = "";
            string highestAge = "";

            if ((ageGenderFlag & (uint)AgeGenderFlags.Toddler) == (uint)AgeGenderFlags.Toddler) highestAge = "p";
            if ((ageGenderFlag & (uint)AgeGenderFlags.Child) == (uint)AgeGenderFlags.Child) highestAge = "c";
            if ((ageGenderFlag & (uint)AgeGenderFlags.Teen) == (uint)AgeGenderFlags.Teen) highestAge = "t";
            if ((ageGenderFlag & (uint)AgeGenderFlags.YoungAdult) == (uint)AgeGenderFlags.YoungAdult) highestAge = "y";
            if ((ageGenderFlag & (uint)AgeGenderFlags.Adult) == (uint)AgeGenderFlags.Adult) highestAge = "a";
            if ((ageGenderFlag & (uint)AgeGenderFlags.Elder) == (uint)AgeGenderFlags.Elder) highestAge = "e";

            flags = highestAge;

            //just default to male for now
            if ((ageGenderFlag & (uint)AgeGenderFlags.Male) == (uint)AgeGenderFlags.Male) flags += "m";
            else if ((ageGenderFlag & (uint)AgeGenderFlags.Female) == (uint)AgeGenderFlags.Female) flags += "f";

            /*
            if ((casPartSrc.typeFlag & 0x1) == 0x1) checkedListType.SetItemChecked(0, true); // Hair
            if ((casPartSrc.typeFlag & 0x2) == 0x2) checkedListType.SetItemChecked(1, true); // Scalp
            if ((casPartSrc.typeFlag & 0x4) == 0x4) checkedListType.SetItemChecked(2, true); // Face Overlay
            if ((casPartSrc.typeFlag & 0x8) == 0x8) checkedListType.SetItemChecked(3, true); // Body
            if ((casPartSrc.typeFlag & 0x10) == 0x10) checkedListType.SetItemChecked(4, true); // Accessory
            */

            // Face Overlay (ie Makeup)
            if ((typeFlag & 0x4) == 0x4) flags += "Face";

            // Body
            if ((typeFlag & 0x8) == 0x8) flags += "Body";

            // Accessory
            if ((typeFlag & 0x10) == 0x10) flags += "Accessory";

            // Check in Dictionary - _m = multiplier, _s = specular, _n = normal map
            if (defaultTextures.ContainsKey(flags + "_m")) returnTextures[0] = "key:00B2D882:00000000:" + defaultTextures[flags + "_m"];
            if (defaultTextures.ContainsKey(flags + "_s")) returnTextures[1] = "key:00B2D882:00000000:" + defaultTextures[flags + "_s"];
            if (defaultTextures.ContainsKey(flags + "_n")) returnTextures[2] = "key:00B2D882:00000000:" + defaultTextures[flags + "_n"];

            return returnTextures;
        }

        static void Deserialize(TextReader reader, IDictionary dictionary)
        {
            dictionary.Clear();
            XmlSerializer serializer = new XmlSerializer(typeof(texturesFile));
            texturesFile tFile = (texturesFile)serializer.Deserialize(reader);

            foreach (textureEntry entry in tFile.Items)
            {
                dictionary[entry.flags] = entry.instanceid;
            }
        }

        static void DeserializeMeshes(TextReader reader, IDictionary dictionary)
        {
            dictionary.Clear();
            XmlSerializer serializer = new XmlSerializer(typeof(meshesFile));
            meshesFile tFile = (meshesFile)serializer.Deserialize(reader);

            foreach (meshEntry entry in tFile.Items)
            {
                dictionary[entry.flags] = entry.value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlRootAttribute()]
        public class texturesFile
        {

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute("texture", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = true)]
            public List<textureEntry> Items = new List<textureEntry>();

        }

        public class textureEntry
        {
            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string flags;

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string instanceid;

            public textureEntry()
            {
            }

            public textureEntry(string flags, string instanceid)
            {
                this.flags = flags;
                this.instanceid = instanceid;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlRootAttribute()]
        public class meshesFile
        {

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute("mesh", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = true)]
            public List<meshEntry> Items = new List<meshEntry>();

        }

        public class meshEntry
        {
            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string flags;

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string value;

            public meshEntry()
            {
            }

            public meshEntry(string flags, string value)
            {
                this.flags = flags;
                this.value = value;
            }
        }

    }
}