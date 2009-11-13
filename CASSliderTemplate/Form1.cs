using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

using MadScience.Wrappers;

namespace CASSliderTemplate
{
    public partial class Form1 : Form
    {
        uint allFlags = (uint)AgeGenderFlags.Toddler + (uint)AgeGenderFlags.Child + (uint)AgeGenderFlags.Teen + (uint)AgeGenderFlags.YoungAdult + (uint)AgeGenderFlags.Adult + (uint)AgeGenderFlags.Elder + (uint)AgeGenderFlags.Human;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            folderBrowserDialog1.Description = "Please locate the directory where you want to save this package";
            folderBrowserDialog1.ShowDialog();

            //txtSliderName.Text = txtSliderName.Text;

            // Make the instance id
            ulong instanceid = MadScience.StringHelpers.HashFNV64(txtSliderName.Text);
            ulong instance32 = (ulong)MadScience.StringHelpers.HashFNV32(txtSliderName.Text);

            ulong instanceLeft = MadScience.StringHelpers.HashFNV64(txtSliderName.Text + "Left");
            ulong instanceRight = MadScience.StringHelpers.HashFNV64(txtSliderName.Text + "Right");

            // Keys
            ResourceKey nameMapKey = new ResourceKey(0x0166038C, 0, instanceid);
            ResourceKey stblKey = new ResourceKey(0x220557DA, 0, instance32);
            ResourceKey blendunitKey = new ResourceKey(0xB52F5055, 0, instanceid);

            ResourceKey facialblendLeft = new MadScience.Wrappers.ResourceKey(0x0358B08A, 0x0, instanceLeft);
            ResourceKey facialblendRight = new MadScience.Wrappers.ResourceKey(0x0358B08A, 0x0, instanceRight);

            ResourceKey blendGeomLeft = new ResourceKey(0x067CAA11, 0, instanceLeft);
            ResourceKey blendGeomRight = new ResourceKey(0x067CAA11, 0, instanceRight);

            ResourceKey vpxyRightMale = new ResourceKey(0x736884F1, 0x0, MadScience.StringHelpers.HashFNV64(txtSliderName.Text + "RightMale"));
            ResourceKey vpxyLeftMale = new ResourceKey(0x736884F1, 0x0, MadScience.StringHelpers.HashFNV64(txtSliderName.Text + "LeftMale"));
            ResourceKey boneDeltaRightMale = new ResourceKey(0x0355E0A6, 0, MadScience.StringHelpers.HashFNV64(txtSliderName.Text + "RightMale"));
            ResourceKey boneDeltaLeftMale = new ResourceKey(0x0355E0A6, 0, MadScience.StringHelpers.HashFNV64(txtSliderName.Text + "LeftMale"));
            ResourceKey vpxyRightFemale = new ResourceKey(0x736884F1, 0x0, MadScience.StringHelpers.HashFNV64(txtSliderName.Text + "RightFemale"));
            ResourceKey vpxyLeftFemale = new ResourceKey(0x736884F1, 0x0, MadScience.StringHelpers.HashFNV64(txtSliderName.Text + "LeftFemale"));
            ResourceKey boneDeltaRightFemale = new ResourceKey(0x0355E0A6, 0, MadScience.StringHelpers.HashFNV64(txtSliderName.Text + "RightFemale"));
            ResourceKey boneDeltaLeftFemale = new ResourceKey(0x0355E0A6, 0, MadScience.StringHelpers.HashFNV64(txtSliderName.Text + "LeftFemale"));
            


            #region Name Map
            NameMap namemap = new NameMap();
            namemap.entries.Add(instanceid, txtSliderName.Text);
            namemap.entries.Add(instanceLeft, txtSliderName.Text + "Left");
            namemap.entries.Add(instanceRight, txtSliderName.Text + "Right");
            if (chkMFLink.Checked)
            {
                namemap.entries.Add(vpxyRightFemale.instanceId, txtSliderName.Text + "RightFemale");
                namemap.entries.Add(vpxyLeftFemale.instanceId, txtSliderName.Text + "LeftFemale");
            }
            else
            {
                if (chkMale.Checked)
                {
                    namemap.entries.Add(vpxyRightMale.instanceId, txtSliderName.Text + "RightMale");
                    namemap.entries.Add(vpxyLeftMale.instanceId, txtSliderName.Text + "LeftMale");
                }
                if (chkFemale.Checked)
                {
                    namemap.entries.Add(vpxyRightFemale.instanceId, txtSliderName.Text + "RightFemale");
                    namemap.entries.Add(vpxyLeftFemale.instanceId, txtSliderName.Text + "LeftFemale");
                }
            }
            Stream nameMapFile = namemap.Save();
            namemap = null;
            #endregion

            #region String Table
            // Start with the STBL
            STBL stbl = new STBL();
            stbl.Items.Add(new STBLEntry(instanceid, txtSliderString.Text));
            Stream stblFile = stbl.Save();
            stbl = null;
            #endregion

            #region BlendUnit
            // Now the BlendUnit (CAS Slider)
            BlendUnit blendunit = new BlendUnit();
            blendunit.localeHash = instanceid;
            blendunit.bidirectional = 1;
            blendunit.casPanelSubGroup = Convert.ToUInt32(txtSubgroup.Text);
            if (chkListCasPanelGroup.GetItemChecked(0) == true) blendunit.casPanelGroup = (uint)casPanelGroup.HeadAndEars;
            if (chkListCasPanelGroup.GetItemChecked(1) == true) blendunit.casPanelGroup = (uint)casPanelGroup.Mouth;
            if (chkListCasPanelGroup.GetItemChecked(2) == true) blendunit.casPanelGroup = (uint)casPanelGroup.Nose;
            if (chkListCasPanelGroup.GetItemChecked(3) == true) blendunit.casPanelGroup = (uint)casPanelGroup.Eyelash;
            if (chkListCasPanelGroup.GetItemChecked(4) == true) blendunit.casPanelGroup = (uint)casPanelGroup.Eyes;
            blendunit.blendLinks.Add(facialblendRight);
            blendunit.blendLinks.Add(facialblendLeft);

            Stream blendunitFile = blendunit.Save();
            blendunit = null;
            #endregion

            #region Facial Blends
            // Now the Facial Blends - need 2 of these, one per slider
            FacialBlend faceblend = new FacialBlend();
            faceblend.partName = txtSliderName.Text + "Left";
            faceblend.blendTgi = blendGeomLeft;
            faceblend.blendType = 2;
            faceblend.keytable.keys.Add(new ResourceKey());
            if (chkMFLink.Checked)
            {
                faceblend.keytable.keys.Add(vpxyLeftFemale);
                if (chkMale.Checked && chkFemale.Checked)
                {
                    faceblend.geomBoneEntries.Add(makeFBEntry((uint)AgeGenderFlags.Male, 1));
                    faceblend.geomBoneEntries.Add(makeFBEntry((uint)AgeGenderFlags.Female, 1));
                }
                else
                {
                    if (chkMale.Checked) { faceblend.geomBoneEntries.Add(makeFBEntry((uint)AgeGenderFlags.Male, 1)); }
                    if (chkFemale.Checked) { faceblend.geomBoneEntries.Add(makeFBEntry((uint)AgeGenderFlags.Female, 1)); }
                }
            }
            else
            {
                if (chkMale.Checked) { faceblend.keytable.keys.Add(vpxyLeftMale); }
                if (chkFemale.Checked) { faceblend.keytable.keys.Add(vpxyLeftFemale); }
                if (chkMale.Checked && chkFemale.Checked)
                {
                    faceblend.geomBoneEntries.Add(makeFBEntry((uint)AgeGenderFlags.Male, 1));
                    faceblend.geomBoneEntries.Add(makeFBEntry((uint)AgeGenderFlags.Female, 2));
                }
                else
                {
                    if (chkMale.Checked) { faceblend.geomBoneEntries.Add(makeFBEntry((uint)AgeGenderFlags.Male, 1)); }
                    if (chkFemale.Checked) { faceblend.geomBoneEntries.Add(makeFBEntry((uint)AgeGenderFlags.Female, 1)); }
                }
            }
            Stream faceBlendFileLeft = faceblend.Save();
            faceblend = null;

            faceblend = new FacialBlend();
            faceblend.partName = txtSliderName.Text + "Right";
            faceblend.blendTgi = blendGeomRight;
            faceblend.blendType = 2;
            faceblend.keytable.keys.Add(new ResourceKey());
            if (chkMFLink.Checked)
            {
                faceblend.keytable.keys.Add(vpxyRightFemale);
                if (chkMale.Checked && chkFemale.Checked)
                {
                    faceblend.geomBoneEntries.Add(makeFBEntry((uint)AgeGenderFlags.Male, 1));
                    faceblend.geomBoneEntries.Add(makeFBEntry((uint)AgeGenderFlags.Female, 1));
                }
                else
                {
                    if (chkMale.Checked) { faceblend.geomBoneEntries.Add(makeFBEntry((uint)AgeGenderFlags.Male, 1)); }
                    if (chkFemale.Checked) { faceblend.geomBoneEntries.Add(makeFBEntry((uint)AgeGenderFlags.Female, 1)); }
                }

            }
            else
            {
                if (chkMale.Checked) { faceblend.keytable.keys.Add(vpxyRightMale); }
                if (chkFemale.Checked) { faceblend.keytable.keys.Add(vpxyRightFemale); }
                if (chkMale.Checked && chkFemale.Checked)
                {
                    faceblend.geomBoneEntries.Add(makeFBEntry((uint)AgeGenderFlags.Male, 1));
                    faceblend.geomBoneEntries.Add(makeFBEntry((uint)AgeGenderFlags.Female, 2));
                }
                else
                {
                    if (chkMale.Checked) { faceblend.geomBoneEntries.Add(makeFBEntry((uint)AgeGenderFlags.Male, 1)); }
                    if (chkFemale.Checked) { faceblend.geomBoneEntries.Add(makeFBEntry((uint)AgeGenderFlags.Female, 1)); }
                }
            }
            Stream faceBlendFileRight = faceblend.Save();
            faceblend = null;
            #endregion

            #region Blend Geometry
            BlendGeom blendGeom = new BlendGeom();
            BlendGeomSection1 blendGeomS1 = new BlendGeomSection1();
            if (chkMale.Checked)
            {
                blendGeomS1.ageGenderFlags = (uint)AgeGenderFlags.Male + allFlags;
                blendGeomS1.regionFlags = getRegionFlag();
                blendGeomS1.subentries.Add(new BlendGeomSection1SubEntry());
                blendGeomS1.subentries.Add(new BlendGeomSection1SubEntry());
                blendGeomS1.subentries.Add(new BlendGeomSection1SubEntry());
                blendGeomS1.subentries.Add(new BlendGeomSection1SubEntry());
                blendGeom.section1.Add(blendGeomS1);
            }
            if (chkFemale.Checked)
            {
                blendGeomS1 = new BlendGeomSection1();
                blendGeomS1.ageGenderFlags = (uint)AgeGenderFlags.Female + allFlags;
                blendGeomS1.regionFlags = getRegionFlag();
                blendGeomS1.subentries.Add(new BlendGeomSection1SubEntry());
                blendGeomS1.subentries.Add(new BlendGeomSection1SubEntry());
                blendGeomS1.subentries.Add(new BlendGeomSection1SubEntry());
                blendGeomS1.subentries.Add(new BlendGeomSection1SubEntry());
                blendGeom.section1.Add(blendGeomS1);
            }
            Stream blendGeomFileLeft = blendGeom.Save();
            
            blendGeom = new BlendGeom();
            blendGeomS1 = new BlendGeomSection1();
            if (chkMale.Checked)
            {
                blendGeomS1.ageGenderFlags = (uint)AgeGenderFlags.Male + allFlags;
                blendGeomS1.regionFlags = getRegionFlag();
                blendGeomS1.subentries.Add(new BlendGeomSection1SubEntry());
                blendGeomS1.subentries.Add(new BlendGeomSection1SubEntry());
                blendGeomS1.subentries.Add(new BlendGeomSection1SubEntry());
                blendGeomS1.subentries.Add(new BlendGeomSection1SubEntry());
                blendGeom.section1.Add(blendGeomS1);
            }
            if (chkFemale.Checked)
            {
                blendGeomS1 = new BlendGeomSection1();
                blendGeomS1.ageGenderFlags = (uint)AgeGenderFlags.Female + allFlags;
                blendGeomS1.regionFlags = getRegionFlag();
                blendGeomS1.subentries.Add(new BlendGeomSection1SubEntry());
                blendGeomS1.subentries.Add(new BlendGeomSection1SubEntry());
                blendGeomS1.subentries.Add(new BlendGeomSection1SubEntry());
                blendGeomS1.subentries.Add(new BlendGeomSection1SubEntry());
                blendGeom.section1.Add(blendGeomS1);
            }
            Stream blendGeomFileRight = blendGeom.Save();

            blendGeom = null;

            #endregion

            #region Proxys
            VPXYFile vpxy = new VPXYFile();
            VPXYEntry vpxyEntry = new VPXYEntry();
            Stream vpxyFileLeftMale = Stream.Null;
            Stream vpxyFileRightMale = Stream.Null;
            Stream vpxyFileLeftFemale = Stream.Null;
            Stream vpxyFileRightFemale = Stream.Null;
            if (chkMFLink.Checked)
            {
                if (chkFemale.Checked)
                {
                    // VPXY Female left
                    vpxy = new VPXYFile();
                    vpxy.rcolHeader.internalChunks.Add(vpxyLeftFemale);
                    vpxyEntry = new VPXYEntry();
                    vpxyEntry.tgiList.Add(boneDeltaLeftFemale);
                    vpxy.vpxy.linkEntries.Add(vpxyEntry);
                }
                vpxyFileLeftFemale = vpxy.Save();
                if (chkFemale.Checked)
                {
                    // VPXY Female Right
                    vpxy = new VPXYFile();
                    vpxy.rcolHeader.internalChunks.Add(vpxyRightFemale);
                    vpxyEntry = new VPXYEntry();
                    vpxyEntry.tgiList.Add(boneDeltaRightFemale);
                    vpxy.vpxy.linkEntries.Add(vpxyEntry);
                }
                vpxyFileRightFemale = vpxy.Save();

            }
            else
            {
                if (chkMale.Checked)
                {
                    //VPXY Male Left
                    vpxy.rcolHeader.internalChunks.Add(vpxyLeftMale);
                    vpxyEntry.tgiList.Add(boneDeltaLeftMale);
                    vpxy.vpxy.linkEntries.Add(vpxyEntry);
                }
                vpxyFileLeftMale = vpxy.Save();
                if (chkMale.Checked)
                {
                    //VPXY Male Right
                    vpxy = new VPXYFile();
                    vpxy.rcolHeader.internalChunks.Add(vpxyRightMale);
                    vpxyEntry = new VPXYEntry();
                    vpxyEntry.tgiList.Add(boneDeltaRightMale);
                    vpxy.vpxy.linkEntries.Add(vpxyEntry);
                }
                vpxyFileRightMale = vpxy.Save();
                if (chkFemale.Checked)
                {
                    // VPXY Female left
                    vpxy = new VPXYFile();
                    vpxy.rcolHeader.internalChunks.Add(vpxyLeftFemale);
                    vpxyEntry = new VPXYEntry();
                    vpxyEntry.tgiList.Add(boneDeltaLeftFemale);
                    vpxy.vpxy.linkEntries.Add(vpxyEntry);
                }
                vpxyFileLeftFemale = vpxy.Save();
                if (chkFemale.Checked)
                {
                    // VPXY Female Right
                    vpxy = new VPXYFile();
                    vpxy.rcolHeader.internalChunks.Add(vpxyRightFemale);
                    vpxyEntry = new VPXYEntry();
                    vpxyEntry.tgiList.Add(boneDeltaRightFemale);
                    vpxy.vpxy.linkEntries.Add(vpxyEntry);
                }
                vpxyFileRightFemale = vpxy.Save();

            }
            #endregion

            #region BoneDeltas
            BoneDeltaFile bonedeltaFile = new BoneDeltaFile();
            BoneDeltaEntry bdEntry = new BoneDeltaEntry();
            Stream bonedeltaLeftMaleFile;
            Stream bonedeltaRightMaleFile;
            Stream bonedeltaLeftFemaleFile ;
            Stream bonedeltaRightFemaleFile ;
            if (chkMFLink.Checked)
            {
                if (chkMale.Checked)
                {
                    bonedeltaFile.rcolHeader.internalChunks.Add(boneDeltaLeftFemale);
                    bdEntry.boneHash = 0x0F97B21B;
                    bdEntry.scale.x = -2f;
                    bdEntry.scale.y = -2f;
                    bdEntry.scale.z = -2f;
                    bonedeltaFile.bonedelta.entries.Add(bdEntry);
                }
                bonedeltaLeftMaleFile = bonedeltaFile.Save();
                if (chkMale.Checked)
                {
                    bonedeltaFile = new BoneDeltaFile();
                    bonedeltaFile.rcolHeader.internalChunks.Add(boneDeltaRightFemale);
                    bdEntry = new BoneDeltaEntry();
                    bdEntry.boneHash = 0x0F97B21B;
                    bdEntry.scale.x = 2f;
                    bdEntry.scale.y = 2f;
                    bdEntry.scale.z = 2f;
                    bonedeltaFile.bonedelta.entries.Add(bdEntry);
                }
                bonedeltaRightMaleFile = bonedeltaFile.Save();

                if (chkFemale.Checked)
                {
                    bonedeltaFile = new BoneDeltaFile();
                    bonedeltaFile.rcolHeader.internalChunks.Add(boneDeltaLeftFemale);
                    bdEntry = new BoneDeltaEntry();
                    bdEntry.boneHash = 0x0F97B21B;
                    bdEntry.scale.x = -2f;
                    bdEntry.scale.y = -2f;
                    bdEntry.scale.z = -2f;
                    bonedeltaFile.bonedelta.entries.Add(bdEntry);
                }
                bonedeltaLeftFemaleFile = bonedeltaFile.Save();

                if (chkFemale.Checked)
                {
                    bonedeltaFile = new BoneDeltaFile();
                    bonedeltaFile.rcolHeader.internalChunks.Add(boneDeltaRightFemale);
                    bdEntry = new BoneDeltaEntry();
                    bdEntry.boneHash = 0x0F97B21B;
                    bdEntry.scale.x = 2f;
                    bdEntry.scale.y = 2f;
                    bdEntry.scale.z = 2f;
                    bonedeltaFile.bonedelta.entries.Add(bdEntry);
                }
                bonedeltaRightFemaleFile = bonedeltaFile.Save();

            }
            else
            {
                if (chkMale.Checked)
                {
                    bonedeltaFile.rcolHeader.internalChunks.Add(boneDeltaLeftMale);
                    bdEntry.boneHash = 0x0F97B21B;
                    bdEntry.scale.x = -2f;
                    bdEntry.scale.y = -2f;
                    bdEntry.scale.z = -2f;
                    bonedeltaFile.bonedelta.entries.Add(bdEntry);
                }
                bonedeltaLeftMaleFile = bonedeltaFile.Save();
                if (chkMale.Checked)
                {
                    bonedeltaFile = new BoneDeltaFile();
                    bonedeltaFile.rcolHeader.internalChunks.Add(boneDeltaRightMale);
                    bdEntry = new BoneDeltaEntry();
                    bdEntry.boneHash = 0x0F97B21B;
                    bdEntry.scale.x = 2f;
                    bdEntry.scale.y = 2f;
                    bdEntry.scale.z = 2f;
                    bonedeltaFile.bonedelta.entries.Add(bdEntry);
                }
                bonedeltaRightMaleFile = bonedeltaFile.Save();

                if (chkFemale.Checked)
                {
                    bonedeltaFile = new BoneDeltaFile();
                    bonedeltaFile.rcolHeader.internalChunks.Add(boneDeltaLeftFemale);
                    bdEntry = new BoneDeltaEntry();
                    bdEntry.boneHash = 0x0F97B21B;
                    bdEntry.scale.x = -2f;
                    bdEntry.scale.y = -2f;
                    bdEntry.scale.z = -2f;
                    bonedeltaFile.bonedelta.entries.Add(bdEntry);
                }
                bonedeltaLeftFemaleFile = bonedeltaFile.Save();

                if (chkFemale.Checked)
                {
                    bonedeltaFile = new BoneDeltaFile();
                    bonedeltaFile.rcolHeader.internalChunks.Add(boneDeltaRightFemale);
                    bdEntry = new BoneDeltaEntry();
                    bdEntry.boneHash = 0x0F97B21B;
                    bdEntry.scale.x = 2f;
                    bdEntry.scale.y = 2f;
                    bdEntry.scale.z = 2f;
                    bonedeltaFile.bonedelta.entries.Add(bdEntry);
                }
                bonedeltaRightFemaleFile = bonedeltaFile.Save();
            }
            #endregion

            Stream packageFile = File.Create(folderBrowserDialog1.SelectedPath + "\\" + txtSliderName.Text + ".package");
            Database db = new Database(packageFile, false);

            for (int i = 0; i < 22; i++)
            {
                ulong actualKey = (ulong)(i * 72057594037927936) + instance32;
                switch (i)
                {
                    case 1:
                    case 2:
                    case 12:
                    case 13:
                    case 20:
                        break;
                    default:
                        db.SetResourceStream(new ResourceKey(0x220557DA, 0, actualKey), stblFile);
                        break;
                }
            }

            //db.SetResourceStream(stblKey, stblFile);
            db.SetResourceStream(nameMapKey, nameMapFile);
            db.SetResourceStream(blendunitKey, blendunitFile);
            db.SetResourceStream(blendGeomLeft, blendGeomFileLeft);
            db.SetResourceStream(blendGeomRight, blendGeomFileRight);
            db.SetResourceStream(facialblendLeft, faceBlendFileLeft);
            db.SetResourceStream(facialblendRight, faceBlendFileRight);

            if (chkMFLink.Checked)
            {
                db.SetResourceStream(vpxyLeftFemale, vpxyFileLeftFemale);
                db.SetResourceStream(vpxyRightFemale, vpxyFileRightFemale);
                db.SetResourceStream(boneDeltaLeftFemale, bonedeltaLeftFemaleFile);
                db.SetResourceStream(boneDeltaRightFemale, bonedeltaRightFemaleFile);
            }
            else
            {
                if (chkMale.Checked)
                {
                    db.SetResourceStream(vpxyLeftMale, vpxyFileLeftMale);
                    db.SetResourceStream(vpxyRightMale, vpxyFileRightMale);
                    db.SetResourceStream(boneDeltaLeftMale, bonedeltaLeftMaleFile);
                    db.SetResourceStream(boneDeltaRightMale, bonedeltaRightMaleFile);
                }
                if (chkFemale.Checked)
                {
                    db.SetResourceStream(vpxyLeftFemale, vpxyFileLeftFemale);
                    db.SetResourceStream(vpxyRightFemale, vpxyFileRightFemale);
                    db.SetResourceStream(boneDeltaLeftFemale, bonedeltaLeftFemaleFile);
                    db.SetResourceStream(boneDeltaRightFemale, bonedeltaRightFemaleFile);
                }
            }
            db.Commit(true);

            packageFile.Close();

            MessageBox.Show("Package saved");

        }

        private FacialBlendGeomBoneEntry makeFBEntry(uint gender, uint boneIndex)
        {
            FacialBlendGeomBoneEntry gbEntry = new FacialBlendGeomBoneEntry();
            gbEntry.ageGenderFlags = gender + allFlags;
            gbEntry.ageGenderFlags2 = gbEntry.ageGenderFlags;
            gbEntry.amount = 1;
            gbEntry.amount2 = 1;
            gbEntry.boneIndex = boneIndex;
            gbEntry.geomEntryIndex = 0;
            gbEntry.hasGeomAndBone = 1;
            gbEntry.hasGeomEntry = 0;
            gbEntry.hasBoneEntry = 1;
            gbEntry.regionFlag = getRegionFlag();
            return gbEntry;
        }

        private uint getRegionFlag()
        {
            uint regionFlag = 0;
            switch (cmbRegionType.SelectedIndex)
            {
                case 0:
                    regionFlag = (uint)FacialRegions.Body;
                    break;
                case 1:
                    regionFlag = (uint)FacialRegions.Brow;
                    break;
                case 2:
                    regionFlag = (uint)FacialRegions.Ears;
                    break;
                case 3:
                    regionFlag = (uint)FacialRegions.Eyelashes;
                    break;
                case 4:
                    regionFlag = (uint)FacialRegions.Eyes;
                    break;
                case 5:
                    regionFlag = (uint)FacialRegions.Face;
                    break;
                case 6:
                    regionFlag = (uint)FacialRegions.Head;
                    break;
                case 7:
                    regionFlag = (uint)FacialRegions.Jaw;
                    break;
                case 8:
                    regionFlag = (uint)FacialRegions.Mouth;
                    break;
                case 9:
                    regionFlag = (uint)FacialRegions.Nose;
                    break;
                case 10:
                    regionFlag = (uint)FacialRegions.TranslateEyes;
                    break;
                case 11:
                    regionFlag = (uint)FacialRegions.TranslateMouth;
                    break;

            }

            return regionFlag;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Stream blah = File.OpenRead(@"P:\Stuart\Desktop\FullBuild0\ui\stbl\root\#0A0F16B00BA8342F.stbl");

            MadScience.Wrappers.STBL stbl = new MadScience.Wrappers.STBL(blah);

            blah.Close();

            Stream blahw = File.Open(Application.StartupPath + "\\test.stbl", FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            stbl.Save(blahw);
            blahw.Close();
        }

        private void chkListCasPanelGroup_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // Uncheck all other boxes
            CheckedListBox lView = (CheckedListBox)sender;
            for (int i = 0; i < lView.Items.Count; i++)
            {
                if (i != e.Index)
                {
                    lView.SetItemChecked(i, false);
                }
            }
        }
    }
}
