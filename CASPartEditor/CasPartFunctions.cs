using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;

using MadScience;
using MadScience.Wrappers;
using OX.Copyable;

namespace CASPartEditor
{
    public partial class Form1 : Form
    {
        private void showMeshDetails()
        {
            string meshName = "";

            // Translate selection back into af am etc
            switch (cmbSimAge.Text)
            {
                case "Toddler":
                    meshName += "p";
                    break;
                case "Child":
                    meshName += "c";
                    break;
                case "Teen":
                    meshName += "t";
                    break;
                case "Young Adult":
                    meshName += "y";
                    break;
                case "Adult":
                    meshName += "a";
                    break;
                case "Elder":
                    meshName += "e";
                    break;
                case "All Ages":
                    meshName += "u";
                    break;
            }

            switch (cmbSimGender.Text)
            {
                case "Female":
                    meshName += "f";
                    break;
                case "Male":
                    meshName += "m";
                    break;
                case "Unisex":
                    meshName += "u";
                    break;
            }

            if (cmbMeshName.Text == "* Custom")
            {

                //if (this.isNew == false)
                //{
                //meshName += casPartSrc.meshName;
                //}
                txtCasPartName.Text = casPartSrc.meshName;
                txtMeshName.Text = casPartSrc.meshName;

                txtCasPartInstance.Text = MadScience.StringHelpers.HashFNV64(meshName).ToString("X16");
                picMeshPreview.Image = null;
                picMeshPreview.Invalidate();

            }
            else
            {

                if (cmbPartTypes.Text == "(none)")
                {
                    meshName += cmbMeshName.Text;
                }
                else
                {
                    meshName += cmbPartTypes.Text + cmbMeshName.Text;
                }

                txtCasPartName.Text = meshName;
                txtMeshName.Text = meshName;

                scrollPanelToThumb(meshName);

                for (int i = 0; i < this.lookupList.Items.Length; i++)
                {
                    filesFile f = lookupList.Items[i];
                    if (f.fullCasPartname == meshName)
                    {
                        txtCasPartInstance.Text = f.instanceid;
                        break;
                    }
                }

                picMeshPreview.Image = null;
                picMeshPreview.Invalidate();

                // Find thumbnail
                if (File.Exists(Path.Combine(Application.StartupPath, Path.Combine("cache", meshName + ".png"))))
                {
                    Stream picMeshPreviewStream = File.OpenRead(Path.Combine(Application.StartupPath, Path.Combine("cache", meshName + ".png")));
                    picMeshPreview.Image = Image.FromStream(picMeshPreviewStream);
                    picMeshPreviewStream.Close();
                }
                else
                {
                    extractCASThumbnail(meshName);
                }
            }

            if (this.isNew == true)
            {
                // Attempt to load the existing caspart into memory so we can extract data later.
                Stream casPartFile = File.Open(Path.Combine(Application.StartupPath, Path.Combine("casparts", txtCasPartName.Text + ".caspart")), FileMode.Open, FileAccess.Read, FileShare.Read);
                casPartFile cPartFile = new casPartFile();
                this.casPartSrc = cPartFile.Load(casPartFile);
                casPartFile.Close();
            }

            for (int i = 0; i < checkedListAge.Items.Count; i++)
            {
                checkedListAge.SetItemChecked(i, false);
            }
            for (int i = 0; i < checkedListCategory.Items.Count; i++)
            {
                checkedListCategory.SetItemChecked(i, false);
            }
            for (int i = 0; i < checkedListCategoryExtended.Items.Count; i++)
            {
                checkedListCategoryExtended.SetItemChecked(i, false);
            }
            for (int i = 0; i < checkedListGender.Items.Count; i++)
            {
                checkedListGender.SetItemChecked(i, false);
            }
            for (int i = 0; i < checkedListOther.Items.Count; i++)
            {
                checkedListOther.SetItemChecked(i, false);
            }
            for (int i = 0; i < checkedListType.Items.Count; i++)
            {
                checkedListType.SetItemChecked(i, false);
            }

            // Get the Mesh links for the first LOD
            Stream meshStream = Stream.Null;

            // Use the VPXY to get the mesh lod
            Stream vpxyStream = KeyUtils.findKey(casPartSrc.tgi64list[casPartSrc.tgiIndexVPXY], 0);

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

            if (meshStream != Stream.Null && meshStream != null)
            {
                lstMeshTGILinks.Items.Clear();
                SimGeomFile simgeomfile = new SimGeomFile(meshStream);
                for (int i = 0; i < simgeomfile.simgeom.keytable.keys.Count; i++)
                {
                    ListViewItem item = new ListViewItem();
                    item.SubItems.Add("TGI #" + (i + 1));
                    item.Text = simgeomfile.simgeom.keytable.keys[i].ToString();
                    if (simgeomfile.simgeom.keytable.keys[i].typeId == 0x00B2D882)
                    {
                        item.Tag = "texture";
                    }
                    else
                    {
                        item.Tag = "";
                    }
                    lstMeshTGILinks.Items.Add(item);
                }
            }


            lstCasPartDetails.Items.Clear();
            // Populate the CAS Part Details 
            addCasPartItem("Mesh Name", casPartSrc.meshName);
            addCasPartItem("Clothing Order", casPartSrc.clothingOrder.ToString());
            addCasPartItem("CAS Part Type", casPartSrc.clothingType.ToString() + " (0x" + casPartSrc.clothingType.ToString("X8") + ")");
            addCasPartItem("Type", casPartSrc.typeFlag.ToString() + " (0x" + casPartSrc.typeFlag.ToString("X8") + ")");
            addCasPartItem("Age/Gender", casPartSrc.ageGenderFlag.ToString() + " (0x" + casPartSrc.ageGenderFlag.ToString("X8") + ")");
            addCasPartItem("Clothing Category", casPartSrc.clothingCategory.ToString() + " (0x" + casPartSrc.clothingCategory.ToString("X8") + ")");
            addCasPartItem("Unk String", casPartSrc.unkString);

            addCasPartItem("Unk2", casPartSrc.unk2.ToString());
            addCasPartItem("TGI Index Body Part 1", casPartSrc.tgiIndexBodyPart1.ToString());
            addCasPartItem("TGI Index Body Part 2", casPartSrc.tgiIndexBodyPart2.ToString());
            addCasPartItem("TGI Index Blend Info Fat", casPartSrc.tgiIndexBlendInfoFat.ToString());
            addCasPartItem("TGI Index Blend Info Fit", casPartSrc.tgiIndexBlendInfoFit.ToString());
            addCasPartItem("TGI Index Blend Info Thin", casPartSrc.tgiIndexBlendInfoThin.ToString());
            addCasPartItem("TGI Index Blend Info Special", casPartSrc.tgiIndexBlendInfoSpecial.ToString());
            addCasPartItem("Unk5", casPartSrc.unk5.ToString());
            addCasPartItem("VPXY", casPartSrc.tgiIndexVPXY.ToString());

            //tgi64 tempvpxy = (tgi64)casPartSrc.tgi64list[casPartSrc.tgiIndexVPXY];
            //txtVPXYPrimary.Text = "key:" + tempvpxy.typeid.ToString("X8") + ":" + tempvpxy.groupid.ToString("X8") + ":" + tempvpxy.instanceid.ToString("X16");

            addCasPartItem("Count 2", casPartSrc.count2.ToString());
            for (int i = 0; i < casPartSrc.count2; i++)
            {
                unkRepeat unk = (unkRepeat)casPartSrc.count2repeat[i];
                addCasPartItem("#" + i.ToString() + ": unkNum ", unk.unkNum.ToString());
                addCasPartItem("#" + i.ToString() + ": unk2", unk.unk2.ToString());
                addCasPartItem("#" + i.ToString() + ": unkRepeatInner", unk.unkRepeatInnerCount.ToString());
                for (int j = 0; j < unk.unkRepeatInnerCount; j++)
                {
                    intTriple iT = (intTriple)unk.unkRepeatInnerLoop[j];
                    addCasPartItem("#" + i.ToString() + "." + j.ToString() + ": One", iT.one.ToString());
                    addCasPartItem("#" + i.ToString() + "." + j.ToString() + ": Two", iT.two.ToString());
                    addCasPartItem("#" + i.ToString() + "." + j.ToString() + ": Three", iT.three.ToString());
                }
            }

            addCasPartItem("TGI Index Diffuse", casPartSrc.tgiIndexDiffuse.ToString());
            addCasPartItem("TGI Index Specular", casPartSrc.tgiIndexSpecular.ToString());

            addCasPartItem("Diffuse Links", casPartSrc.count3.ToString());
            for (int i = 0; i < casPartSrc.count3; i++)
            {
                byte cRepeat = (byte)casPartSrc.count3repeat[i];
                addCasPartItem("#" + i.ToString(), cRepeat.ToString());
            }

            addCasPartItem("Specular Links", casPartSrc.count4.ToString());
            for (int i = 0; i < casPartSrc.count4; i++)
            {
                byte cRepeat = (byte)casPartSrc.count4repeat[i];
                addCasPartItem("#" + i.ToString(), cRepeat.ToString());
            }

            addCasPartItem("Count 5", casPartSrc.count5.ToString());
            for (int i = 0; i < casPartSrc.count5; i++)
            {
                byte cRepeat = (byte)casPartSrc.count5repeat[i];
                addCasPartItem("#" + i.ToString(), cRepeat.ToString());
            }

            for (int i = 0; i < chkDesignType.Items.Count; i++)
            {
                chkDesignType.SetItemChecked(i, false);
            }

            for (int i = 0; i < casPartSrc.count6; i++)
            {
                MadScience.Wrappers.ResourceKey tgi = casPartSrc.tgi64list[i];
                string tgiType = MadScience.Helpers.findMetaEntry(tgi.typeId).shortName;
                Console.WriteLine(tgi.typeId.ToString() + " " + tgiType);

                if (tgi.typeId == 0x0333406C)
                {
                    if (tgi.instanceId == 0x52E8BE209C703561)
                    {
                        chkDesignType.SetItemChecked(0, true);
                    }
                    if (tgi.instanceId == 0xE37696463F6B2D6E)
                    {
                        chkDesignType.SetItemChecked(1, true);
                    }
                    if (tgi.instanceId == 0x01625DDC220C08C6)
                    {
                        chkDesignType.SetItemChecked(2, true);
                    }

                }

                addCasPartItem("TGI #" + i.ToString() + " " + tgiType, tgi.ToString());
            }

            // Category flags
            if ((casPartSrc.typeFlag & 0x1) == 0x1) checkedListType.SetItemChecked(0, true); // Hair
            if ((casPartSrc.typeFlag & 0x2) == 0x2) checkedListType.SetItemChecked(1, true); // Scalp
            if ((casPartSrc.typeFlag & 0x4) == 0x4) checkedListType.SetItemChecked(2, true); // Face Overlay
            if ((casPartSrc.typeFlag & 0x8) == 0x8) checkedListType.SetItemChecked(3, true); // Body
            if ((casPartSrc.typeFlag & 0x10) == 0x10) checkedListType.SetItemChecked(4, true); // Accessory

            switch (casPartSrc.clothingType)
            {
                case 1: checkedListClothingType.SetItemChecked(0, true); break;
                case 2: checkedListClothingType.SetItemChecked(1, true); break;
                case 3: checkedListClothingType.SetItemChecked(2, true); break;
                case 4: checkedListClothingType.SetItemChecked(3, true); break;
                case 5: checkedListClothingType.SetItemChecked(4, true); break;
                case 6: checkedListClothingType.SetItemChecked(5, true); break;
                case 7: checkedListClothingType.SetItemChecked(6, true); break;
                case 11: checkedListClothingType.SetItemChecked(7, true); break;
                case 12: checkedListClothingType.SetItemChecked(8, true); break;
                case 13: checkedListClothingType.SetItemChecked(9, true); break;
                case 14: checkedListClothingType.SetItemChecked(10, true); break;
                case 15: checkedListClothingType.SetItemChecked(11, true); break;
                case 16: checkedListClothingType.SetItemChecked(12, true); break;
                case 17: checkedListClothingType.SetItemChecked(13, true); break;
                case 18: checkedListClothingType.SetItemChecked(14, true); break;
                case 19: checkedListClothingType.SetItemChecked(15, true); break;
                case 20: checkedListClothingType.SetItemChecked(16, true); break;
                case 21: checkedListClothingType.SetItemChecked(17, true); break;
                case 22: checkedListClothingType.SetItemChecked(18, true); break;
                case 24: checkedListClothingType.SetItemChecked(19, true); break;
                case 25: checkedListClothingType.SetItemChecked(20, true); break;
                case 26: checkedListClothingType.SetItemChecked(21, true); break;
                case 29: checkedListClothingType.SetItemChecked(22, true); break;
                case 30: checkedListClothingType.SetItemChecked(23, true); break;
                case 31: checkedListClothingType.SetItemChecked(24, true); break;
            }

            if ((casPartSrc.ageGenderFlag & 0x1) == 0x1) checkedListAge.SetItemChecked(0, true); // Baby
            if ((casPartSrc.ageGenderFlag & 0x2) == 0x2) checkedListAge.SetItemChecked(1, true); // Toddler
            if ((casPartSrc.ageGenderFlag & 0x4) == 0x4) checkedListAge.SetItemChecked(2, true); // Child
            if ((casPartSrc.ageGenderFlag & 0x8) == 0x8) checkedListAge.SetItemChecked(3, true); // Teen
            if ((casPartSrc.ageGenderFlag & 0x10) == 0x10) checkedListAge.SetItemChecked(4, true); // YoungAdult
            if ((casPartSrc.ageGenderFlag & 0x20) == 0x20) checkedListAge.SetItemChecked(5, true); // Adult
            if ((casPartSrc.ageGenderFlag & 0x40) == 0x40) checkedListAge.SetItemChecked(6, true); // Elder

            if ((casPartSrc.ageGenderFlag & 0x1000) == 0x1000) checkedListGender.SetItemChecked(0, true); // Male
            if ((casPartSrc.ageGenderFlag & 0x2000) == 0x2000) checkedListGender.SetItemChecked(1, true); // Female

            if ((casPartSrc.ageGenderFlag & 0x100000) == 0x100000) checkedListOther.SetItemChecked(0, true); // LeftHanded
            if ((casPartSrc.ageGenderFlag & 0x200000) == 0x200000) checkedListOther.SetItemChecked(1, true); // RightHanded
            if ((casPartSrc.ageGenderFlag & 0x10000) == 0x10000) checkedListOther.SetItemChecked(2, true); // Human

            if ((casPartSrc.clothingCategory & 0x1) == 0x1) checkedListCategory.SetItemChecked(0, true); // Naked
            if ((casPartSrc.clothingCategory & 0x2) == 0x2) checkedListCategory.SetItemChecked(1, true); // Everyday
            if ((casPartSrc.clothingCategory & 0x4) == 0x4) checkedListCategory.SetItemChecked(2, true); // Formalwear
            if ((casPartSrc.clothingCategory & 0x8) == 0x8) checkedListCategory.SetItemChecked(3, true); // Sleepwear
            if ((casPartSrc.clothingCategory & 0x10) == 0x10) checkedListCategory.SetItemChecked(4, true); // Swimwear
            if ((casPartSrc.clothingCategory & 0x20) == 0x20) checkedListCategory.SetItemChecked(5, true); // Athletic
            if ((casPartSrc.clothingCategory & 0x40) == 0x40) checkedListCategory.SetItemChecked(6, true); // Singed
            if ((casPartSrc.clothingCategory & 0x100) == 0x100) checkedListCategory.SetItemChecked(7, true); // Career
            if ((casPartSrc.clothingCategory & 0xFFFF) == 0xFFFF) checkedListCategory.SetItemChecked(8, true); // All

            if ((casPartSrc.clothingCategory & 0x100000) == 0x100000) checkedListCategoryExtended.SetItemChecked(0, true); // ValidForMaternity
            if ((casPartSrc.clothingCategory & 0x200000) == 0x200000) checkedListCategoryExtended.SetItemChecked(1, true); // ValidForRandom
            if ((casPartSrc.clothingCategory & 0x400000) == 0x400000) checkedListCategoryExtended.SetItemChecked(2, true); // IsHat
            if ((casPartSrc.clothingCategory & 0x800000) == 0x800000) checkedListCategoryExtended.SetItemChecked(3, true); // IsRevealing
            if ((casPartSrc.clothingCategory & 0x1000000) == 0x1000000) checkedListCategoryExtended.SetItemChecked(4, true); // IsHiddenInCas

            saveAsToolStripMenuItem.Enabled = true;
            btnDumpFromFullbuild2.Enabled = true;

            lstStencilPool.Items.Clear();

            // Default all stencil boxes to blank
            for (int i = 1; i <= 15; i++)
            {
                if (stencilPool.Count < i) { stencilPool.Add(new stencilDetails()); }
                //updateStencilBoxes(i, new stencilDetails());
            }


            // Calculate all the stencils so we can build up the stencil pool
            int curStencilNum = 1;
            for (int i = 0; i < casPartSrc.xmlChunk.Count; i++)
            {
                if (debugModeToolStripMenuItem.Checked)
                {
                    Helpers.logMessageToFile(casPartSrc.xmlChunkRaw[i].ToString().Replace("/><", "/>" + Environment.NewLine + "<"));
                }

                xmlChunkDetails chunk = (xmlChunkDetails)casPartSrc.xmlChunk[i];

                if (!inStencilList(chunk.stencil.A))
                {
                    updateStencilBoxes(curStencilNum, chunk.stencil.A);
                    curStencilNum++;

                }
                if (!inStencilList(chunk.stencil.B))
                {
                    updateStencilBoxes(curStencilNum, chunk.stencil.B);
                    curStencilNum++;

                }
                if (!inStencilList(chunk.stencil.C))
                {
                    updateStencilBoxes(curStencilNum, chunk.stencil.C);
                    curStencilNum++;

                }
                if (!inStencilList(chunk.stencil.D))
                {
                    updateStencilBoxes(curStencilNum, chunk.stencil.D);
                    curStencilNum++;

                }
                if (!inStencilList(chunk.stencil.E))
                {
                    updateStencilBoxes(curStencilNum, chunk.stencil.E);
                    curStencilNum++;

                }
                if (!inStencilList(chunk.stencil.F))
                {
                    updateStencilBoxes(curStencilNum, chunk.stencil.F);
                    curStencilNum++;
                }

            }

            for (int i = curStencilNum; i <= 15; i++)
            {
                //if (stencilPool.Count < i) { stencilPool.Add(new stencilDetails()); }
                updateStencilBoxes(i, new stencilDetails());
            }

            if (this.casPartSrc.xmlChunkRaw.Count == 0)
            {
                label8.Visible = true;
                listView1.Enabled = false;
                //btnDumpFromFullbuild2.Enabled = false;
                btnAddNewDesign.Enabled = false;
                btnDeleteDesign.Enabled = false;
            }
            else
            {

                label8.Visible = false;
                listView1.Enabled = true;
                //btnDumpFromFullbuild2.Enabled = true;
                btnAddNewDesign.Enabled = true;
                btnDeleteDesign.Enabled = true;

                this.casPartNew = (casPart)casPartSrc.Copy();
                if (this.isNew)
                {
                    this.casPartNew.xmlChunk.Clear();
                    this.casPartNew.xmlChunkRaw.Clear();
                    lastSelected = -1;
                    listView1.Items.Clear();
                    lstTextureDetails.Items.Clear();
                    lstOtherDetails.Items.Clear();

                    //btnAddNewDesign_Click(this, null);
                }

            }

        }

        private List<bool> getHiddenSections()
        {
            List<bool> ret = new List<bool>();
            ret.Add(true); // Texture Details
            ret.Add(true); // Clothing Details
            ret.Add(true); // Skin Details
            ret.Add(false); // Face Overlay Details

            if (checkedListClothingType.GetItemChecked(13) || checkedListClothingType.GetItemChecked(14) || checkedListClothingType.GetItemChecked(15) || checkedListClothingType.GetItemChecked(16) || checkedListClothingType.GetItemChecked(18)) 
            {
                ret[0] = false;
                ret[1] = false;
                ret[2] = false;
                ret[3] = true;
            }
            if (checkedListClothingType.GetItemChecked(17))
            {
                ret[3] = true;
            }


            return ret;
        }

        private void showCasPart(int chunkNo)
        {
            // Just takes the first XML chunk and uses that
            xmlChunkDetails chunk = (xmlChunkDetails)casPartNew.xmlChunk[chunkNo];

            Console.WriteLine("Showing design " + chunkNo.ToString());
            /*
            chkStencilAEnabled.Enabled = true;
            chkStencilBEnabled.Enabled = true;
            chkStencilCEnabled.Enabled = true;
            chkStencilDEnabled.Enabled = true;
            chkStencilEEnabled.Enabled = true;
            chkStencilFEnabled.Enabled = true;
             */

            chkPatternAEnabled.Enabled = true;
            cmbPatternSelect.SelectedIndex = 0;
            showPatternDetails(chunkNo);

            //chkPatternBEnabled.Enabled = true;
            //chkPatternCEnabled.Enabled = true;
            chkLogoEnabled.Enabled = true;

            btnPatternStencil1Preview.Enabled = false;
            btnPatternStencil2Preview.Enabled = false;
            btnPatternStencil3Preview.Enabled = false;
            btnPatternStencil4Preview.Enabled = false;
            btnPatternStencil5Preview.Enabled = false;
            btnPatternStencil6Preview.Enabled = false;
            cmbPatternStencil1.Enabled = true;
            cmbPatternStencil2.Enabled = true;
            cmbPatternStencil3.Enabled = true;
            cmbPatternStencil4.Enabled = true;
            cmbPatternStencil5.Enabled = true;
            cmbPatternStencil6.Enabled = true;

            if (chunk.hasCustomThumbnail)
            {
                chkUseCustomThumbnail.Checked = true;
                txtCustomThumbnailPath.Text = newPNGfiles[chunkNo];
            }

            //ListViewItem item;
            // Stencil Details

            cmbPatternStencil1.SelectedIndex = 0;
            if (chunk.stencil.A.key != "" && chunk.stencil.A.Enabled.ToLower() == "true")
            {
                if (inStencilList(chunk.stencil.A)) cmbPatternStencil1.SelectedIndex = getStencilFromPool(chunk.stencil.A) + 1;
            }
            cmbPatternStencil2.SelectedIndex = 0;
            if (chunk.stencil.B.key != "" && chunk.stencil.B.Enabled.ToLower() == "true")
            {
                if (inStencilList(chunk.stencil.B)) cmbPatternStencil2.SelectedIndex = getStencilFromPool(chunk.stencil.B) + 1;
            }
            cmbPatternStencil3.SelectedIndex = 0;
            if (chunk.stencil.C.key != "" && chunk.stencil.C.Enabled.ToLower() == "true")
            {
                if (inStencilList(chunk.stencil.C)) cmbPatternStencil3.SelectedIndex = getStencilFromPool(chunk.stencil.C) + 1;
            }
            cmbPatternStencil4.SelectedIndex = 0;
            if (chunk.stencil.D.key != "" && chunk.stencil.D.Enabled.ToLower() == "true")
            {
                if (inStencilList(chunk.stencil.D)) cmbPatternStencil4.SelectedIndex = getStencilFromPool(chunk.stencil.D) + 1;
            }
            cmbPatternStencil5.SelectedIndex = 0;
            if (chunk.stencil.E.key != "" && chunk.stencil.E.Enabled.ToLower() == "true")
            {
                if (inStencilList(chunk.stencil.E)) cmbPatternStencil5.SelectedIndex = getStencilFromPool(chunk.stencil.E) + 1;
            }
            cmbPatternStencil6.SelectedIndex = 0;
            if (chunk.stencil.F.key != "" && chunk.stencil.F.Enabled.ToLower() == "true")
            {
                if (inStencilList(chunk.stencil.F)) cmbPatternStencil6.SelectedIndex = getStencilFromPool(chunk.stencil.F) + 1;
            }

            // Pattern Details

            // Other Details
            lstOtherDetails.Items.Clear();
            lstTextureDetails.Items.Clear();

            List<bool> showSection = getHiddenSections();

            if (showSection[0])
            {
                addListHeader(lstTextureDetails, "Texture Details");
                addListItem(lstTextureDetails, chunk.Multiplier, "Base Texture", "texture");
                addListBlank(lstTextureDetails);
            }
            if (showSection[1])
            {
                addListHeader(lstTextureDetails, "Clothing Details");
                addListItem(lstTextureDetails, chunk.ClothingSpecular, "Clothing Specular", "texture");
                addListItem(lstTextureDetails, chunk.ClothingAmbient, "Clothing Ambient", "texture");
                addListBlank(lstTextureDetails);
            }

            if (showSection[3])
            {
                addListHeader(lstTextureDetails, "Face Overlay Details");

                if (checkedListClothingType.GetItemChecked(15) || checkedListClothingType.GetItemChecked(14) || checkedListClothingType.GetItemChecked(18)) // Eyeliner, Eyebrow, Eyeshadow
                {
                    addListItem(lstTextureDetails, chunk.TintColor, "Tint Color", "color");
                    addListBlank(lstTextureDetails);
                }
                else
                {

                    addListItem(lstTextureDetails, chunk.tint.A.color, "Tint Color A", "color");
                    addListItem(lstTextureDetails, chunk.tint.B.color, "Tint Color B", "color");
                    addListItem(lstTextureDetails, chunk.tint.C.color, "Tint Color C", "color");
                    addListItem(lstTextureDetails, chunk.tint.D.color, "Tint Color D", "color");
                    addListItem(lstTextureDetails, chunk.tint.A.enabled, "Tint Color A Enabled", "truefalse");
                    addListItem(lstTextureDetails, chunk.tint.B.enabled, "Tint Color B Enabled", "truefalse");
                    addListItem(lstTextureDetails, chunk.tint.C.enabled, "Tint Color C Enabled", "truefalse");
                    addListItem(lstTextureDetails, chunk.tint.D.enabled, "Tint Color D Enabled", "truefalse");
                }
                addListBlank(lstTextureDetails);
                
                addListItem(lstTextureDetails, chunk.faceOverlay, "Face Overlay", "texture");
                addListItem(lstTextureDetails, chunk.faceSpecular, "Face Specular", "texture");
                addListBlank(lstTextureDetails);
            }

            addListHeader(lstTextureDetails, "Mask & Overlay Details");
            addListItem(lstTextureDetails, chunk.Overlay, "Overlay", "texture");
            addListItem(lstTextureDetails, chunk.ControlMap, "Control Map", "texture");
            addListItem(lstTextureDetails, chunk.DiffuseMap, "Diffuse Map", "texture");
            addListItem(lstTextureDetails, chunk.Mask, "Mask", "texture");
            addListItem(lstTextureDetails, chunk.PartMask, "Part Mask", "texture");
            addListBlank(lstTextureDetails);

            addListHeader(lstOtherDetails, "Mask & Overlay Details");
            addListItem(lstOtherDetails, chunk.MaskHeight, "MaskHeight", "");
            addListItem(lstOtherDetails, chunk.MaskWidth, "MaskWidth", "");
            addListBlank(lstOtherDetails);

            // Hair details
            if (checkedListType.GetItemChecked(0))
            {

                addListHeader(lstTextureDetails, "Hair Details");
                addListItem(lstTextureDetails, chunk.hair.RootColor, "Root Color", "color");
                addListItem(lstTextureDetails, chunk.hair.DiffuseColor, "Diffuse Color", "color");
                addListItem(lstTextureDetails, chunk.hair.HighlightColor, "Highlight Color", "color");
                addListItem(lstTextureDetails, chunk.hair.TipColor, "Tip Color", "color");
                addListBlank(lstTextureDetails);
                addListItem(lstTextureDetails, chunk.hair.ScalpSpecularMap, "Scalp Specular Map", "texture");
                addListItem(lstTextureDetails, chunk.hair.ScalpDiffuseMap, "Scalp Diffuse Map", "texture");
                addListItem(lstTextureDetails, chunk.hair.ScalpSpecularMap, "Scalp Control Map", "texture");
                addListItem(lstTextureDetails, chunk.hair.FaceSpecularMap, "Face Specular Map", "texture");
                addListItem(lstTextureDetails, chunk.hair.FaceDiffuseMap, "Face Diffuse Map", "texture");
                addListItem(lstTextureDetails, chunk.hair.FaceSpecularMap, "Face Control Map", "texture");
                addListBlank(lstTextureDetails);

                addListHeader(lstOtherDetails, "Hair Details");
                addListItem(lstOtherDetails, chunk.IsHat, "IsHat", "truefalse");
                addListItem(lstOtherDetails, chunk.DrawsOnFace, "DrawsOnFace", "truefalse");
                addListItem(lstOtherDetails, chunk.DrawsOnScalp, "DrawsOnScalp", "truefalse");
                addListItem(lstOtherDetails, chunk.hair.ScalpAO, "Scalp AO", "texture");
                addListItem(lstOtherDetails, chunk.hair.FaceAO, "Face AO", "texture");
                addListBlank(lstTextureDetails);

            }

            if (showSection[2])
            {
                addListHeader(lstTextureDetails, "Skin Details");
                addListItem(lstTextureDetails, chunk.SkinAmbient, "Skin Ambient", "texture");
                addListItem(lstTextureDetails, chunk.SkinSpecular, "Skin Specular", "texture");
            }
            if (chunk.logo.enabled.ToLower() == "true") { chkLogoEnabled.Checked = true; }
            else { chkLogoEnabled.Checked = false; }

            txtLogoUpperLeft.Text = chunk.logo.upperLeft;
            txtLogoLowerRight.Text = chunk.logo.lowerRight;
            txtLogoFilename.Text = chunk.logo.filename;
            txtLogoKey.Text = chunk.logo.key;
            txtLogoName.Text = chunk.logo.name;

            addListHeader(lstOtherDetails, "CAS Details");
            addListItem(lstOtherDetails, chunk.IsNaked, "IsNaked", "truefalse");
            addListItem(lstOtherDetails, chunk.IsNotNaked, "IsNotNaked", "truefalse");
            addListItem(lstOtherDetails, chunk.partType, "partType", "");
            addListItem(lstOtherDetails, chunk.age, "age", "");
            addListItem(lstOtherDetails, chunk.gender, "gender", "");
            addListItem(lstOtherDetails, chunk.bodyType, "bodyType", "");
            addListBlank(lstOtherDetails);

            addListHeader(lstOtherDetails, "Misc. Details");
            addListItem(lstOtherDetails, chunk.name, "Name", "");
            addListItem(lstOtherDetails, chunk.reskey, "reskey", "");
            addListItem(lstOtherDetails, chunk.daeFileName, "daeFileName", "");
            addListItem(lstOtherDetails, chunk.filename, "filename", "");
            addListItem(lstOtherDetails, chunk.assetRoot, "assetRoot", "");
            addListItem(lstOtherDetails, chunk.Rotation, "Rotation", "");

            toolStripStatusLabel1.Text = "Initialising 3d view... please wait...";
            statusStrip1.Refresh();

            if (renderWindow1.RenderEnabled)
                btnReloadTextures_Click(null, null);
            else
                btnStart3D_Click(null, null);

            toolStripStatusLabel1.Text = "";

        }


        private void updateChunkFromLists(ListView lView, xmlChunkDetails chunk)
        {
            for (int i = 0; i < lView.Items.Count; i++)
            {
                ListViewItem item = lView.Items[i];

                switch (item.SubItems[1].Text)
                {
                    case "Name":
                        chunk.name = item.Text;
                        break;
                    case "reskey":
                        chunk.reskey = item.Text;
                        break;
                    case "assetRoot":
                        chunk.assetRoot = item.Text;
                        break;

                    case "Overlay":
                        chunk.Overlay = item.Text;
                        break;

                    case "Control Map":
                        chunk.ControlMap = item.Text;
                        break;
                    case "Diffuse Map":
                        chunk.DiffuseMap = item.Text;
                        break;

                    case "Mask":
                        chunk.Mask = item.Text;
                        break;

                    case "MaskHeight":
                        chunk.MaskHeight = item.Text;
                        break;

                    case "MaskWidth":
                        chunk.MaskWidth = item.Text;
                        break;

                    case "Base Texture":
                    case "Multiplier":
                        chunk.Multiplier = item.Text;
                        break;

                    case "IsHat":
                        chunk.IsHat = item.Text;
                        break;
                    case "DrawsOnFace":
                        chunk.DrawsOnFace = item.Text;
                        break;
                    case "DrawsOnScalp":
                        chunk.DrawsOnScalp = item.Text;
                        break;
                    case "IsNaked":
                        chunk.IsNaked = item.Text;
                        break;

                    case "IsNotNaked":
                        chunk.IsNotNaked = item.Text;
                        break;

                    case "Skin Specular":
                        chunk.SkinSpecular = item.Text;
                        break;

                    case "Skin Ambient":
                        chunk.SkinAmbient = item.Text;
                        break;

                    case "Clothing Specular":
                        chunk.ClothingSpecular = item.Text;
                        break;

                    case "Clothing Ambient":
                        chunk.ClothingAmbient = item.Text;
                        break;

                    case "Rotation":
                        chunk.Rotation = item.Text;
                        break;

                    case "Root Color":
                        chunk.hair.RootColor = item.Text;
                        break;
                    case "Diffuse Color":
                        chunk.hair.DiffuseColor = item.Text;
                        break;
                    case "Highlight Color":
                        chunk.hair.HighlightColor = item.Text;
                        break;
                    case "Tip Color":
                        chunk.hair.TipColor = item.Text;
                        break;

                    case "Face Overlay":
                        chunk.faceOverlay = item.Text;
                        break;
                    case "Face Specular":
                        chunk.faceSpecular = item.Text;
                        break;

                    case "Tint Color":
                        chunk.TintColor = item.Text;
                        break;
                    case "Tint Color A":
                        chunk.tint.A.color = item.Text;
                        break;
                    case "Tint Color B":
                        chunk.tint.B.color = item.Text;
                        break;
                    case "Tint Color C":
                        chunk.tint.C.color = item.Text;
                        break;
                    case "Tint Color D":
                        chunk.tint.D.color = item.Text;
                        break;
                    case "Tint Color A Enabled":
                        chunk.tint.A.enabled = item.Text;
                        break;
                    case "Tint Color B Enabled":
                        chunk.tint.B.enabled = item.Text;
                        break;
                    case "Tint Color C Enabled":
                        chunk.tint.C.enabled = item.Text;
                        break;
                    case "Tint Color D Enabled":
                        chunk.tint.D.enabled = item.Text;
                        break;

                    case "Scalp Diffuse Map":
                        chunk.hair.ScalpDiffuseMap = item.Text;
                        break;
                    case "Scalp Control Map":
                        chunk.hair.ScalpControlMap = item.Text;
                        break;
                    case "Scalp Specular Map":
                        chunk.hair.ScalpSpecularMap = item.Text;
                        break;
                    case "Scalp AO":
                        chunk.hair.ScalpAO = item.Text;
                        break;
                    case "Face Diffuse Map":
                        chunk.hair.FaceDiffuseMap = item.Text;
                        break;
                    case "Face Control Map":
                        chunk.hair.FaceControlMap = item.Text;
                        break;
                    case "Face Specular Map":
                        chunk.hair.FaceSpecularMap = item.Text;
                        break;
                    case "Face AO":
                        chunk.hair.FaceAO = item.Text;
                        break;

                    case "Part Mask":
                        chunk.PartMask = item.Text;
                        break;

                    case "partType":
                        chunk.partType = item.Text;
                        break;

                    case "age":
                        chunk.age = item.Text;
                        break;

                    case "gender":
                        chunk.gender = item.Text;
                        break;

                    case "bodyType":
                        chunk.bodyType = item.Text;
                        break;

                    case "daeFileName":
                        chunk.daeFileName = item.Text;
                        break;

                    case "filename":
                        chunk.filename = item.Text;
                        break;

                }

            }

        }


    }
}
