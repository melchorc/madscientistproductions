using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Drawing;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.Globalization;

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
                if (!renderWindow1.RenderEnabled)
                {
                    reload3D();
                    return;
                }

                xmlChunkDetails details = (xmlChunkDetails)casPartNew.xmlChunk[listView1.SelectedIndices[0]];

                renderWindow1.RenderEnabled = false;

                DateTime startTime = DateTime.Now;
                toolStripStatusLabel1.Text = "Reloading Textures...";
                statusStrip1.Refresh();

                reloadTextures(details);


                DateTime stopTime = DateTime.Now;
                TimeSpan duration = stopTime - startTime;

                toolStripStatusLabel1.Text = "Reloaded Textures in " + duration.Milliseconds + "ms";

                renderWindow1.RenderEnabled = true;
            }
        }

        public void reloadTextures(xmlChunkDetails details)
        {
            if (listView1.SelectedItems.Count == 1 && cEnable3DPreview.Checked == true)
            {

                List<string> textures = findDefaultTextures(casPartNew.ageGenderFlag, casPartNew.typeFlag);

                renderWindow1.loadTexture(KeyUtils.findKey(textures[0]), "skinTexture");
                renderWindow1.loadTexture(KeyUtils.findKey(textures[1]), "skinSpecular");
                //renderWindow1.loadTexture(KeyUtils.findKey(textures[2]), "normalMap");

                //Don't use the default skin bumpmap texture, but load the bumpmap from the current mesh.
                keyName bumpMapKey = new keyName("foobar", txtMeshName.Text + "_n");
                renderWindow1.loadTexture(KeyUtils.findKey(bumpMapKey.ToResourceKey()), "normalMap");

                if ((casPartNew.typeFlag & 0x1) == 0x1)
                {
                    renderWindow1.loadTexture(KeyUtils.findKey(details.ClothingAmbient), "ambientTexture");
                    renderWindow1.loadTexture(KeyUtils.findKey(details.ClothingSpecular), "specularTexture");
                }
                else if ((casPartNew.typeFlag & 0x4) == 0x4)
                {
                    renderWindow1.loadTexture(KeyUtils.findKey(details.faceOverlay), "ambientTexture");
                    renderWindow1.loadTexture(KeyUtils.findKey(details.faceSpecular), "specularTexture");
                }
                else
                {
                    renderWindow1.loadTexture(KeyUtils.findKey(details.ClothingAmbient), "ambientTexture");
                    renderWindow1.loadTexture(KeyUtils.findKey(details.ClothingSpecular), "specularTexture");
                }
                renderWindow1.loadTexture(KeyUtils.findKey(details.Multiplier), "baseTexture");
                renderWindow1.loadTexture(null, "stencilA");

                //if ((casPartNew.typeFlag & 0x4) == 0x4)
                //{
                //renderWindow1.shaderMode = 1;
                //}
                //else
                //{
                //renderWindow1.shaderMode = 0;
                //}

                generate3DTexture(details);

                // We don't need this here since generate3DTexture resets the device anyway
                //renderWindow1.resetDevice();
            }
        }

        public void reload3D()
        {
            if (listView1.SelectedItems.Count == 1 && cEnable3DPreview.Checked == true)
            {
                xmlChunkDetails details = (xmlChunkDetails)casPartNew.xmlChunk[listView1.SelectedIndices[0]];

                toolStripStatusLabel1.Text = "Initialising 3d view... please wait...";
                statusStrip1.Refresh();

                DateTime startTime = DateTime.Now;

                reload3D(details);

                DateTime stopTime = DateTime.Now;
                TimeSpan duration = stopTime - startTime;
                this.toolStripStatusLabel1.Text = "Loaded 3D in " + duration.Milliseconds + "ms";
            }
        }

        public void reload3D(xmlChunkDetails details)
        {

            if (listView1.SelectedItems.Count == 1 && cEnable3DPreview.Checked == true)
            {

                // Get the Mesh links for the first LOD
                List<Stream> meshStreams = new List<Stream>();

                // Use the VPXY to get the mesh lod
                Stream vpxyStream = KeyUtils.findKey(casPartSrc.tgi64list[casPartSrc.tgiIndexVPXY], 0);

                if (Helpers.isValidStream(vpxyStream))
                {
                    VPXYFile vpxyFile = new VPXYFile(vpxyStream);
                    // Get the first VPXY internal link
                    if (vpxyFile.vpxy.linkEntries.Count >= 1 && vpxyFile.vpxy.linkEntries[0].tgiList.Count >= 1)
                    {
                        for (int i = 0; i < vpxyFile.vpxy.linkEntries[0].tgiList.Count; i++)
                        {
                            meshStreams.Add(KeyUtils.findKey(vpxyFile.vpxy.linkEntries[0].tgiList[i], 0));
                        }
                    }
                    vpxyStream.Close();
                }

                if (meshStreams.Count == 0) // || ((casPartSrc.typeFlag & 0x1) == 0x1))
                {
                    ResourceKey findKey = findDefaultMeshes(casPartNew.ageGenderFlag, casPartNew.typeFlag);

                    if (findKey.groupId != 0 && findKey.instanceId != 0)
                    {
                        meshStreams.Add(KeyUtils.findKey(findKey, 0));
                    }
                }

                if (meshStreams.Count > 0)
                {
                    renderWindow1.BackgroundColour = MadScience.Colours.convertColour(MadScience.Helpers.getRegistryValue("renderBackgroundColour"));

                    // For each model, go through, get the model info and send it to the render window
                    for (int i = 0; i < meshStreams.Count; i++)
                    {
                        MadScience.Render.modelInfo newModel = MadScience.Render.Helpers.geomToModel(meshStreams[i]);
                        newModel.name = txtMeshName.Text;
                        //renderWindow1.loadDefaultTextures();
                        renderWindow1.setModel(newModel, i);

                    }

                    reloadTextures(details);
                    renderWindow1.RenderEnabled = true;


                }
                else
                {
                    renderWindow1.statusLabel.Text = "Sorry, we could not find a mesh!";
                }
            }
        }

        private void generate3DTexture()
        {
            if (listView1.SelectedItems.Count == 1 && cEnable3DPreview.Checked == true)
            {
                xmlChunkDetails details = (xmlChunkDetails)casPartNew.xmlChunk[listView1.SelectedIndices[0]];
                generate3DTexture(details);
            }
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
                //makeup
                b = composeMakeup(details, casPartNew.clothingType);
            }
            else if ((casPartNew.typeFlag & 0x1) == 0x1)
            {
                //hair
                b = composeHair(details);
            }
            else
            {
                b = composeMultiplier(details);
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
                    if ((casPartNew.typeFlag & 0x1) == 0x1)
                    {
                        renderWindow1.shaderMode = 2;
                    }
                    else
                    {
                        renderWindow1.shaderMode = 0;
                    }
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
            if (details.Overlay != null && ! MadScience.Patterns.isEmptyTexture(details.Overlay))
                stencils.Add(KeyUtils.findKey(details.Overlay));
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
                renderWindow1.loadTextureFromBitmap(MadScience.Patterns.mergeStencils(stencils), "stencilA");
        }

        private Bitmap composeMakeup(xmlChunkDetails details, uint casPartType)
        {
            DateTime startTime2 = DateTime.Now;
            List<MadScience.Wrappers.ResourceKey> tempList = new List<MadScience.Wrappers.ResourceKey>();
            tempList.Add(new MadScience.Wrappers.ResourceKey(details.faceOverlay));
            tempList.Add(new MadScience.Wrappers.ResourceKey(details.Mask));

            List<Stream> textures = KeyUtils.findKey(tempList, 2);
            DateTime stopTime2 = DateTime.Now;
            TimeSpan duration2 = stopTime2 - startTime2;
            Console.WriteLine("Key search time: " + duration2.TotalMilliseconds);

            if (MadScience.Patterns.isEmptyTexture(details.Multiplier))
                textures[0] = null;

            DateTime startTime = DateTime.Now;
            Bitmap output = null;
            if (String.IsNullOrEmpty(details.TintColor))
            {
                if (MadScience.Patterns.isEmptyMask(details.Mask) && details.tint.A.enabled != null && details.tint.A.enabled.ToLower() == "true")
                {
                    output = PatternProcessor.ProcessMakeupTexture(textures, MadScience.Colours.convertColour(details.tint.A.color));
                }
                else if (MadScience.Patterns.isEmptyMask(details.Mask) || String.IsNullOrEmpty(details.Mask))
                {
//                    output = new Bitmap(16, 16);
                    output = PatternProcessor.ProcessMakeupTexture(textures, Color.White);
                }
                else
                {
                    output = PatternProcessor.ProcessMakeupTexture(textures,
                    details.tint.A,
                    details.tint.B,
                    details.tint.C,
                    details.tint.D);
                }
            }
            else if (details.TintColor != null)
            {
                output = PatternProcessor.ProcessMakeupTexture(textures, MadScience.Colours.convertColour(details.TintColor));
            }

            DateTime stopTime = DateTime.Now;
            TimeSpan duration = stopTime - startTime;
            Console.WriteLine("Total Makeup Texture generation time: " + duration.TotalMilliseconds);
            return output;
        }

        private Bitmap composeMultiplier(xmlChunkDetails details)
        {
            Bitmap[] myPatterns = new Bitmap[1];
            DateTime startTime2 = DateTime.Now;
            List<MadScience.Wrappers.ResourceKey> tempList = new List<MadScience.Wrappers.ResourceKey>();
            tempList.Add(new MadScience.Wrappers.ResourceKey(details.Multiplier));
            tempList.Add(new MadScience.Wrappers.ResourceKey(details.Mask));

            List<Stream> textures = KeyUtils.findKey(tempList, 2);

            if (details.pattern[3].Enabled.ToLower() == "true" && details.filename == "CasRgbaMask")
            {
                myPatterns = new Bitmap[4];
            }
            else if (details.pattern[2].Enabled.ToLower() == "true")
            {
                myPatterns = new Bitmap[3];
            }
            else if (details.pattern[1].Enabled.ToLower() == "true")
            {
                myPatterns = new Bitmap[2];
            }

            PointF[] tilings = new PointF[myPatterns.Length];

            for(int i = 0; i < myPatterns.Length;i++)
                if (details.pattern[i].Enabled.ToLower() == "true")
                {
                    myPatterns[i] = (Bitmap)Patterns.makePatternThumb(details.pattern[i]); ;
                    tilings[i] = new PointF(1f, 1f);
                    if (details.pattern[i].Tiling != null)
                    {
                        String[] s = details.pattern[i].Tiling.Split(',');
                        if (s.Length == 2)
                        {
                            try
                            {
                                tilings[i].X = Convert.ToSingle(s[0], CultureInfo.InvariantCulture);
                                tilings[i].Y = Convert.ToSingle(s[1], CultureInfo.InvariantCulture);
                            }
                            catch (Exception)
                            {
                                tilings[i] = new PointF(1f, 1f);
                            }
                        }
                    }
                }

            DateTime stopTime2 = DateTime.Now;
            TimeSpan duration2 = stopTime2 - startTime2;
            Console.WriteLine("Key search time: " + duration2.TotalMilliseconds);

            DateTime startTime = DateTime.Now;
            if (MadScience.Patterns.isEmptyTexture(details.Multiplier))
                textures[0] = null;

            Bitmap output;
            if (MadScience.Patterns.isEmptyMask(details.Mask))
               output = PatternProcessor.ProcessSingleChannelTexture(
               textures,
               myPatterns[0],
               tilings[0]);           
            else 
                output = PatternProcessor.ProcessClothingTexture(
                textures,
                myPatterns,
                tilings);

            DateTime stopTime = DateTime.Now;
            TimeSpan duration = stopTime - startTime;
            Console.WriteLine("Total Multiplier Texture generation time: " + duration.TotalMilliseconds);
            return output;
        }

        private Bitmap composeHair(xmlChunkDetails details)
        {
            Bitmap patterns = composeMultiplier(details);
 
            List<MadScience.Wrappers.ResourceKey> tempList = new List<MadScience.Wrappers.ResourceKey>();
            tempList.Add(new MadScience.Wrappers.ResourceKey(details.DiffuseMap));
            tempList.Add(new MadScience.Wrappers.ResourceKey(details.ControlMap));

            List<Stream> textures = KeyUtils.findKey(tempList, 2);

            if (MadScience.Patterns.isEmptyTexture(details.DiffuseMap))
                textures[0] = null;

            DateTime startTime = DateTime.Now;
            Bitmap hair = PatternProcessor.ProcessHairTexture(textures,
                MadScience.Colours.convertColour(details.hair.DiffuseColor),
                MadScience.Colours.convertColour(details.hair.RootColor),
                MadScience.Colours.convertColour(details.hair.HighlightColor),
                MadScience.Colours.convertColour(details.hair.TipColor));

            if (patterns.Width < hair.Width)
            {
                using (Graphics g = Graphics.FromImage(hair))
                {
                    g.DrawImage(patterns, 0, 0, patterns.Width, patterns.Height);
                }
                return hair;
            }
            else
            {
                using (Graphics g = Graphics.FromImage(patterns))
                {
                    g.DrawImage(hair, 0, 0, patterns.Width, patterns.Height);
                }
                return patterns;
            }

        }

        public static Dictionary<string, string> defaultMeshes = new Dictionary<string, string>();
        public static ResourceKey findDefaultMeshes(uint ageGenderFlag, uint typeFlag)
        {
            ResourceKey ret = new ResourceKey();

            // Load in XML
            if (defaultMeshes.Count == 0)
            {
                // Load in XML
                TextReader r = new StreamReader(Path.Combine(Application.StartupPath, Path.Combine("xml", "defaultMeshes.xml")));
                XmlSerializer s = new XmlSerializer(typeof(meshesFile));
                DeserializeMeshes(r, defaultMeshes);
                r.Close();
            }

            string flags = "";
            string highestAge = "";

            if ((ageGenderFlag & (uint)AgeGenderFlags.Baby) == (uint)AgeGenderFlags.Baby) highestAge = "b";
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
            if ((typeFlag & 0x1) == 0x1) flags += "Face";

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
                TextReader r = new StreamReader(Path.Combine(Application.StartupPath, Path.Combine("xml", "defaultTextures.xml")));
                Deserialize(r, defaultTextures);
                r.Close();
            }

            string flags = "";
            string highestAge = "";

            if ((ageGenderFlag & (uint)AgeGenderFlags.Baby) == (uint)AgeGenderFlags.Baby) highestAge = "b";
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
            if ((typeFlag & 0x1) == 0x1) flags += "Face";

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