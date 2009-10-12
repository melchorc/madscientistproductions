using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

using MadScience;
using MadScience.Wrappers;

namespace CASPartEditor
{
    public partial class Form1 : Form
    {

        private MemoryStream makeBlendFile(keyName proxy)
        {
            MemoryStream output = new MemoryStream();
            MadScience.StreamHelpers.WriteValueU32(output, 7);
            MadScience.StreamHelpers.WriteValueU32(output, 0);
            MadScience.StreamHelpers.WriteValueU32(output, 20);
            MadScience.StreamHelpers.WriteValueU8(output, (byte)(proxy.name.Length * 2));
            MadScience.StreamHelpers.WriteStringUTF16(output, false, proxy.name);
            MadScience.StreamHelpers.WriteValueU32(output, 2);
            MadScience.StreamHelpers.WriteValueU32(output, 1);
            MadScience.StreamHelpers.WriteValueU32(output, 1024);
            MadScience.StreamHelpers.WriteValueU32(output, 1);
            MadScience.StreamHelpers.WriteValueU32(output, 77951);
            MadScience.StreamHelpers.WriteValueU16(output, 0);
            MadScience.StreamHelpers.WriteValueU32(output, 16256);
            MadScience.StreamHelpers.WriteValueU32(output, 0);
            MadScience.StreamHelpers.WriteValueU16(output, 0);
            uint position = (uint)output.Position;
            MadScience.StreamHelpers.WriteValueU32(output, 1);
            //MadScience.StreamHelpers.WriteValueU64(output, 6231196913);
            MadScience.StreamHelpers.WriteValueU32(output, proxy.typeId);
            MadScience.StreamHelpers.WriteValueU32(output, proxy.groupId);
            MadScience.StreamHelpers.WriteValueU64(output, proxy.instanceId);

            output.Seek(4, SeekOrigin.Begin);
            MadScience.StreamHelpers.WriteValueU32(output, (position - 8));

            // Should be 101 bytes long at this point
            return output;
        }

        private MemoryStream makeVPXYfile(MadScience.Wrappers.ResourceKey headerKey)
        {
            MemoryStream mem = new MemoryStream();
            MadScience.Wrappers.VPXYFile vpxyFile = new MadScience.Wrappers.VPXYFile();
            vpxyFile.rcolHeader.internalChunks.Add(headerKey);

            vpxyFile.Save(mem);

            return mem;
        }

        private Stream saveGeom(string filename, ResourceKey bumpMapKey)
        {
            int bumpmapPos = -1;

            SimGeomFile simgeomfile = new SimGeomFile();

            Stream blah = File.Open(filename, FileMode.Open);
            simgeomfile.Load(blah);
            blah.Close();

            if (bumpMapKey.ToString() != "key:00000000:00000000:0000000000000000")
            {
                // Figure out bumpmap location
                // To do this we loop through the MTNF
                for (int i = 0; i < simgeomfile.simgeom.mtnfChunk.entries.Count; i++)
                {
                    if (simgeomfile.simgeom.mtnfChunk.entries[i].fieldTypeHash == (uint)FieldTypes.NormalMap)
                    {
                        bumpmapPos = (int)simgeomfile.simgeom.mtnfChunk.entries[i].dwords[0];
                        break;
                    }
                }
                if (bumpmapPos > -1)
                {
                    simgeomfile.simgeom.keytable.keys[bumpmapPos] = bumpMapKey;
                }
            }
            return simgeomfile.Save();
        }

        private void saveToDBPF(Database db, ulong instanceId, bool newInstance)
        {
            ResourceKey rkey;

            MemoryStream mem = new MemoryStream();
            casPartFile casPF = new casPartFile();

            // Do we have new meshes?  If so, we need to do some pretty heft modifications. :)

            string meshName = txtMeshName.Text;

            NameMap namemap = new NameMap();
            ResourceKey namemapKey = new ResourceKey(0x0166038C, 0x00000000, instanceId);

            if (!String.IsNullOrEmpty(txtMeshLod1.Text) || !String.IsNullOrEmpty(txtMeshLod0.Text))
            {
                keyName bodyBlendFat = new keyName(0x062C8204, 0x0, meshName + "_fat");
                keyName bodyBlendFit = new keyName(0x062C8204, 0x0, meshName + "_fit");
                keyName bodyBlendThin = new keyName(0x062C8204, 0x0, meshName + "_thin");
                keyName bodyBlendSpecial = new keyName(0x062C8204, 0x0, meshName + "_special");

                namemap.entries.Add(bodyBlendFat.instanceId, bodyBlendFat.name);
                namemap.entries.Add(bodyBlendFit.instanceId, bodyBlendFit.name);
                namemap.entries.Add(bodyBlendThin.instanceId, bodyBlendThin.name);
                namemap.entries.Add(bodyBlendSpecial.instanceId, bodyBlendSpecial.name);

                Stream bodyBlendFatStream = KeyUtils.findKey(casPartSrc.tgi64list[casPartSrc.tgiIndexBlendInfoFat].ToString(), 0);
                Stream bodyBlendFitStream = KeyUtils.findKey(casPartSrc.tgi64list[casPartSrc.tgiIndexBlendInfoFit].ToString(), 0);
                Stream bodyBlendThinStream = KeyUtils.findKey(casPartSrc.tgi64list[casPartSrc.tgiIndexBlendInfoThin].ToString(), 0);
                Stream bodyBlendSpecialStream = KeyUtils.findKey(casPartSrc.tgi64list[casPartSrc.tgiIndexBlendInfoSpecial].ToString(), 0);

                // Load in the blend information
                FacialBlend bodyBlendFatFile = new FacialBlend(bodyBlendFatStream);
                bodyBlendFatFile.partName = meshName + "_fat";
                FacialBlend bodyBlendFitFile = new FacialBlend(bodyBlendFitStream);
                bodyBlendFitFile.partName = meshName + "_fit";
                FacialBlend bodyBlendThinFile = new FacialBlend(bodyBlendThinStream);
                bodyBlendThinFile.partName = meshName + "_thin";
                FacialBlend bodyBlendSpecialFile = new FacialBlend(bodyBlendSpecialStream);
                bodyBlendSpecialFile.partName = meshName + "_special";

                if (debugModeToolStripMenuItem.Checked)
                {
                    Stream bgeoStream = KeyUtils.findKey(bodyBlendFatFile.blendTgi, 0);
                    bgeoStream.Seek(0, SeekOrigin.Begin);
                    bodyBlendFatFile.blendTgi = new keyName(0x067CAA11, 0x0, meshName + "_fat").ToResourceKey();
                    db.SetResourceStream(bodyBlendFatFile.blendTgi, bgeoStream);

                    bgeoStream = KeyUtils.findKey(bodyBlendFitFile.blendTgi, 0);
                    bgeoStream.Seek(0, SeekOrigin.Begin);
                    bodyBlendFitFile.blendTgi = new keyName(0x067CAA11, 0x0, meshName + "_fit").ToResourceKey();
                    db.SetResourceStream(bodyBlendFitFile.blendTgi, bgeoStream);

                    bgeoStream = KeyUtils.findKey(bodyBlendThinFile.blendTgi, 0);
                    bgeoStream.Seek(0, SeekOrigin.Begin);
                    bodyBlendThinFile.blendTgi = new keyName(0x067CAA11, 0x0, meshName + "_thin").ToResourceKey();
                    db.SetResourceStream(bodyBlendThinFile.blendTgi, bgeoStream);

                    bgeoStream = KeyUtils.findKey(bodyBlendSpecialFile.blendTgi, 0);
                    bgeoStream.Seek(0, SeekOrigin.Begin);
                    bodyBlendSpecialFile.blendTgi = new keyName(0x067CAA11, 0x0, meshName + "_special").ToResourceKey();
                    db.SetResourceStream(bodyBlendSpecialFile.blendTgi, bgeoStream);

                }

                db.SetResourceStream(bodyBlendFit.ToResourceKey(), bodyBlendFitFile.Save());
                db.SetResourceStream(bodyBlendFat.ToResourceKey(), bodyBlendFatFile.Save());
                db.SetResourceStream(bodyBlendThin.ToResourceKey(), bodyBlendThinFile.Save());
                db.SetResourceStream(bodyBlendSpecial.ToResourceKey(), bodyBlendSpecialFile.Save());

                // Update the CAS part TGI links with the new VPXY
                casPartNew.tgi64list[casPartNew.tgiIndexBlendInfoFat] = bodyBlendFat.ToResourceKey();
                casPartNew.tgi64list[casPartNew.tgiIndexBlendInfoFit] = bodyBlendFit.ToResourceKey();
                casPartNew.tgi64list[casPartNew.tgiIndexBlendInfoThin] = bodyBlendThin.ToResourceKey();
                casPartNew.tgi64list[casPartNew.tgiIndexBlendInfoSpecial] = bodyBlendSpecial.ToResourceKey();

                keyName proxyFitKey = new keyName(0x736884F1, 0x00000001, meshName + "_fit");
                keyName proxyFatKey = new keyName(0x736884F1, 0x00000001, meshName + "_fat");
                keyName proxyThinKey = new keyName(0x736884F1, 0x00000001, meshName + "_thin");
                keyName proxySpecialKey = new keyName(0x736884F1, 0x00000001, meshName + "_special");

                Stream proxyFatStream = KeyUtils.findKey(new ResourceKey(0x736884F1, 0x00000001, casPartSrc.tgi64list[casPartSrc.tgiIndexBlendInfoFat].instanceId), 0);
                Stream proxyFitStream = KeyUtils.findKey(new ResourceKey(0x736884F1, 0x00000001, casPartSrc.tgi64list[casPartSrc.tgiIndexBlendInfoFit].instanceId), 0);
                Stream proxyThinStream = KeyUtils.findKey(new ResourceKey(0x736884F1, 0x00000001, casPartSrc.tgi64list[casPartSrc.tgiIndexBlendInfoThin].instanceId), 0);
                Stream proxySpecialStream = KeyUtils.findKey(new ResourceKey(0x736884F1, 0x00000001, casPartSrc.tgi64list[casPartSrc.tgiIndexBlendInfoSpecial].instanceId), 0);

                VPXYFile proxyFat = new VPXYFile(proxyFatStream);
                proxyFat.rcolHeader.internalChunks.Clear();
                proxyFat.rcolHeader.internalChunks.Add(proxyFatKey.ToResourceKey());
                VPXYFile proxyFit = new VPXYFile(proxyFitStream);
                proxyFit.rcolHeader.internalChunks.Clear();
                proxyFit.rcolHeader.internalChunks.Add(proxyFitKey.ToResourceKey());
                VPXYFile proxyThin = new VPXYFile(proxyThinStream);
                proxyThin.rcolHeader.internalChunks.Clear();
                proxyThin.rcolHeader.internalChunks.Add(proxyThinKey.ToResourceKey());
                VPXYFile proxySpecial = new VPXYFile(proxySpecialStream);
                proxySpecial.rcolHeader.internalChunks.Clear();
                proxySpecial.rcolHeader.internalChunks.Add(proxySpecialKey.ToResourceKey());

                db.SetResourceStream(proxyFatKey.ToResourceKey(), proxyFat.Save());
                db.SetResourceStream(proxyFitKey.ToResourceKey(), proxyFit.Save());
                db.SetResourceStream(proxyThinKey.ToResourceKey(), proxyThin.Save());
                db.SetResourceStream(proxySpecialKey.ToResourceKey(), proxySpecial.Save());

                uint customGroup = MadScience.StringHelpers.HashFNV24(meshName);
                keyName meshLod0 = new keyName(0x015A1849, customGroup, (ulong)MadScience.StringHelpers.HashFNV32(meshName + "_lod0"), meshName + "_lod0");
                keyName meshLod0_1 = new keyName(0x015A1849, customGroup, (ulong)MadScience.StringHelpers.HashFNV32(meshName + "_lod0_1"), meshName + "_lod0_1");
                keyName meshLod0_2 = new keyName(0x015A1849, customGroup, (ulong)MadScience.StringHelpers.HashFNV32(meshName + "_lod0_2"), meshName + "_lod0_2");
                keyName meshLod0_3 = new keyName(0x015A1849, customGroup, (ulong)MadScience.StringHelpers.HashFNV32(meshName + "_lod0_3"), meshName + "_lod0_3");

                keyName meshLod1 = new keyName(0x015A1849, customGroup, (ulong)MadScience.StringHelpers.HashFNV32(meshName + "_lod1"), meshName + "_lod1");
                keyName meshLod1_1 = new keyName(0x015A1849, customGroup, (ulong)MadScience.StringHelpers.HashFNV32(meshName + "_lod1_1"), meshName + "_lod1_1");
                keyName meshLod1_2 = new keyName(0x015A1849, customGroup, (ulong)MadScience.StringHelpers.HashFNV32(meshName + "_lod1_2"), meshName + "_lod1_2");
                keyName meshLod1_3 = new keyName(0x015A1849, customGroup, (ulong)MadScience.StringHelpers.HashFNV32(meshName + "_lod1_3"), meshName + "_lod1_3");
                keyName meshLod2 = new keyName(0x015A1849, customGroup, (ulong)MadScience.StringHelpers.HashFNV32(meshName + "_lod2"), meshName + "_lod2");
                keyName meshLod2_1 = new keyName(0x015A1849, customGroup, (ulong)MadScience.StringHelpers.HashFNV32(meshName + "_lod2_1"), meshName + "_lod2_1");
                keyName meshLod2_2 = new keyName(0x015A1849, customGroup, (ulong)MadScience.StringHelpers.HashFNV32(meshName + "_lod2_2"), meshName + "_lod2_2");

                keyName meshLod3 = new keyName(0x015A1849, customGroup, (ulong)MadScience.StringHelpers.HashFNV32(meshName + "_lod3"), meshName + "_lod3");

                keyName vpxyKey = new keyName(0x736884F1, 0x00000001, (ulong)customGroup);

                // Load in the VPXY - we need to modify it.
                //keyName oldVpxyKey = new keyName((tgi64)casPartSrc.tgi64list[casPartSrc.tgiIndexVPXY]);
                Stream vpxyStream = KeyUtils.findKey(casPartSrc.tgi64list[casPartSrc.tgiIndexVPXY].ToString(), 0);
                if (Helpers.isValidStream(vpxyStream))
                {

                    namemap.entries.Add(meshLod0.instanceId, meshName + "_lod0");
                    namemap.entries.Add(meshLod0_1.instanceId, meshName + "_lod0_1");
                    namemap.entries.Add(meshLod0_2.instanceId, meshName + "_lod0_2");
                    namemap.entries.Add(meshLod0_3.instanceId, meshName + "_lod0_3");
                    namemap.entries.Add(meshLod1.instanceId, meshName + "_lod1");
                    namemap.entries.Add(meshLod1_1.instanceId, meshName + "_lod1_1");
                    namemap.entries.Add(meshLod1_2.instanceId, meshName + "_lod1_2");
                    namemap.entries.Add(meshLod1_3.instanceId, meshName + "_lod1_3");
                    namemap.entries.Add(meshLod2.instanceId, meshName + "_lod2");
                    namemap.entries.Add(meshLod2_1.instanceId, meshName + "_lod2_1");
                    namemap.entries.Add(meshLod2_2.instanceId, meshName + "_lod2_2");
                    namemap.entries.Add(meshLod3.instanceId, meshName + "_lod3");
                    namemap.entries.Add(vpxyKey.instanceId, meshName);

                    //keyName proxyFit = new keyName(0x736884F1, 0x00000001, meshName + "_fit");
                    //keyName proxyFat = new keyName(0x736884F1, 0x00000001, meshName + "_fat");
                    //keyName proxyThin = new keyName(0x736884F1, 0x00000001, meshName + "_thin");
                    //keyName proxySpecial = new keyName(0x736884F1, 0x00000001, meshName + "_special");

                    VPXYFile vpxyfile = new VPXYFile(vpxyStream);
                    vpxyfile.rcolHeader.internalChunks.Clear();
                    vpxyfile.rcolHeader.internalChunks.Add(vpxyKey.ToResourceKey());

                    vpxyfile.vpxy.linkEntries.Clear();
                    if (!String.IsNullOrEmpty(txtMeshLod0.Text))
                    {
                        // LOD 0
                        VPXYEntry vpxyE = new VPXYEntry();
                        if (!String.IsNullOrEmpty(txtMeshLod0_1.Text)) vpxyE.tgiList.Add(meshLod0_1.ToResourceKey());
                        if (!String.IsNullOrEmpty(txtMeshLod0_2.Text)) vpxyE.tgiList.Add(meshLod0_2.ToResourceKey());
                        if (!String.IsNullOrEmpty(txtMeshLod0_3.Text)) vpxyE.tgiList.Add(meshLod0_3.ToResourceKey());
                        vpxyE.tgiList.Add(meshLod0.ToResourceKey());
                        vpxyfile.vpxy.linkEntries.Add(vpxyE);
                    }
                    if (!String.IsNullOrEmpty(txtMeshLod1.Text))
                    {
                        // LOD 1
                        VPXYEntry vpxyE = new VPXYEntry();
                        if (!String.IsNullOrEmpty(txtMeshLod1_1.Text)) vpxyE.tgiList.Add(meshLod1_1.ToResourceKey());
                        if (!String.IsNullOrEmpty(txtMeshLod1_2.Text)) vpxyE.tgiList.Add(meshLod1_2.ToResourceKey());
                        if (!String.IsNullOrEmpty(txtMeshLod1_3.Text)) vpxyE.tgiList.Add(meshLod1_3.ToResourceKey());
                        vpxyE.tgiList.Add(meshLod1.ToResourceKey());
                        vpxyfile.vpxy.linkEntries.Add(vpxyE);
                    }
                    if (!String.IsNullOrEmpty(txtMeshLod2.Text))
                    {
                        // LOD 2
                        VPXYEntry vpxyE = new VPXYEntry();
                        if (!String.IsNullOrEmpty(txtMeshLod2_1.Text)) vpxyE.tgiList.Add(meshLod2_1.ToResourceKey());
                        if (!String.IsNullOrEmpty(txtMeshLod2_2.Text)) vpxyE.tgiList.Add(meshLod2_2.ToResourceKey());
                        vpxyE.tgiList.Add(meshLod2.ToResourceKey());
                        vpxyfile.vpxy.linkEntries.Add(vpxyE);
                    }
                    if (!String.IsNullOrEmpty(txtMeshLod3.Text))
                    {
                        // LOD 2
                        VPXYEntry vpxyE = new VPXYEntry();
                        vpxyE.tgiList.Add(meshLod3.ToResourceKey());
                        vpxyfile.vpxy.linkEntries.Add(vpxyE);
                    }
                    
                    vpxyfile.vpxy.keytable.keys.Clear();

                    // If a Hair or an Accessory then set the vpxy start to 0 else starts at 1
                    if (checkedListType.GetItemChecked(0) == true || checkedListType.GetItemChecked(4) == true)
                    {
                        vpxyfile.vpxy.numTypeZero = 0;
                    }
                    else
                    {
                        vpxyfile.vpxy.numTypeZero = 1;
                    }
                    vpxyStream = vpxyfile.Save();

                    //vpxyfile.rcolHeader.internalChunks[0] = proxyFit.ToResourceKey();
                    //Stream proxyFitFile = vpxyfile.Save();

                    //vpxyfile.rcolHeader.internalChunks[0] = proxyFat.ToResourceKey();
                    //Stream proxyFatFile = vpxyfile.Save();

                    //vpxyfile.rcolHeader.internalChunks[0] = proxyThin.ToResourceKey();
                    //Stream proxyThinFile = vpxyfile.Save();

                    //vpxyfile.rcolHeader.internalChunks[0] = proxySpecial.ToResourceKey();
                    //Stream proxySpecialFile = vpxyfile.Save();

                    db.SetResourceStream(vpxyKey.ToResourceKey(), vpxyStream);
                    //db.SetResourceStream(proxyFit.ToResourceKey(), proxyFitFile);
                    //db.SetResourceStream(proxyFat.ToResourceKey(), proxyFatFile);
                    //db.SetResourceStream(proxyThin.ToResourceKey(), proxyThinFile);
                    //db.SetResourceStream(proxySpecial.ToResourceKey(), proxySpecialFile);

                    // Update the CAS part TGI links with the new VPXY
                    casPartNew.tgi64list[casPartNew.tgiIndexVPXY] = vpxyKey.ToResourceKey();

                    keyName bumpMapKey = new keyName();

                    if (String.IsNullOrEmpty(txtOtherBumpMap.Text) == false)
                    {
                        bumpMapKey = new keyName(txtOtherBumpMap.Text, meshName + "_n");
                        //kNames.Add(bumpMapKey);
                        Stream bumpMapStream = File.OpenRead(txtOtherBumpMap.Text);
                        if (txtOtherBumpMap.Text != "" && !txtOtherBumpMap.Text.StartsWith("key:")) db.SetResourceStream(bumpMapKey.ToResourceKey(), bumpMapStream);
                        bumpMapStream.Close();
                    }

                    #region Import Mesh LODs
                    if (!String.IsNullOrEmpty(txtMeshLod0.Text.Trim()))
                    {
                        db.SetResourceStream(meshLod0.ToResourceKey(), saveGeom(txtMeshLod0.Text, bumpMapKey.ToResourceKey()));
                    }
                    if (!String.IsNullOrEmpty(txtMeshLod0_1.Text.Trim()))
                    {
                        db.SetResourceStream(meshLod0_1.ToResourceKey(), saveGeom(txtMeshLod0_1.Text, bumpMapKey.ToResourceKey()));
                    }
                    if (!String.IsNullOrEmpty(txtMeshLod0_2.Text.Trim()))
                    {
                        db.SetResourceStream(meshLod0_2.ToResourceKey(), saveGeom(txtMeshLod0_2.Text, bumpMapKey.ToResourceKey()));
                    }
                    if (!String.IsNullOrEmpty(txtMeshLod0_3.Text.Trim()))
                    {
                        db.SetResourceStream(meshLod0_3.ToResourceKey(), saveGeom(txtMeshLod0_3.Text, bumpMapKey.ToResourceKey()));
                    }
                    
                    if (!String.IsNullOrEmpty(txtMeshLod1.Text.Trim()))
                    {
                        db.SetResourceStream(meshLod1.ToResourceKey(), saveGeom(txtMeshLod1.Text, bumpMapKey.ToResourceKey()));
                    }

                    if (!String.IsNullOrEmpty(txtMeshLod1_1.Text.Trim()))
                    {
                        db.SetResourceStream(meshLod1_1.ToResourceKey(), saveGeom(txtMeshLod1_1.Text, bumpMapKey.ToResourceKey()));
                    }

                    if (!String.IsNullOrEmpty(txtMeshLod1_2.Text.Trim()))
                    {
                        db.SetResourceStream(meshLod1_2.ToResourceKey(), saveGeom(txtMeshLod1_2.Text, bumpMapKey.ToResourceKey()));
                    }

                    if (!String.IsNullOrEmpty(txtMeshLod1_3.Text.Trim()))
                    {
                        db.SetResourceStream(meshLod1_3.ToResourceKey(), saveGeom(txtMeshLod1_3.Text, bumpMapKey.ToResourceKey()));
                    }

                    if (!String.IsNullOrEmpty(txtMeshLod2.Text.Trim()))
                    {
                        db.SetResourceStream(meshLod2.ToResourceKey(), saveGeom(txtMeshLod2.Text, bumpMapKey.ToResourceKey()));
                    }
                    if (!String.IsNullOrEmpty(txtMeshLod2_1.Text.Trim()))
                    {
                        db.SetResourceStream(meshLod2_1.ToResourceKey(), saveGeom(txtMeshLod2_1.Text, bumpMapKey.ToResourceKey()));
                    }

                    if (!String.IsNullOrEmpty(txtMeshLod2_2.Text.Trim()))
                    {
                        db.SetResourceStream(meshLod2_2.ToResourceKey(), saveGeom(txtMeshLod2_2.Text, bumpMapKey.ToResourceKey()));
                    }

                    if (!String.IsNullOrEmpty(txtMeshLod3.Text.Trim()))
                    {
                        db.SetResourceStream(meshLod3.ToResourceKey(), saveGeom(txtMeshLod3.Text, bumpMapKey.ToResourceKey()));
                    }
                    #endregion
                }

            }

            db.SetResourceStream(namemapKey, namemap.Save());

            if (casPartNew != null)
            {
                casPF.Save(mem, casPartNew);
            }
            else
            {
                casPF.Save(mem, casPartSrc);
            }
            casPF = null;

            if (this.loadedCasPart.ToString() == "00000000:00000000:0000000000000000")
            {
                rkey = new ResourceKey((uint)0x034AEECB, (uint)0, instanceId, (uint)ResourceKeyOrder.ITG);
            }
            else
            {
                if (!newInstance)
                {
                    rkey = this.loadedCasPart;
                }
                else
                {
                    rkey = new ResourceKey((uint)0x034AEECB, (uint)0, instanceId, (uint)ResourceKeyOrder.ITG);
                }
            }
            db.SetResourceStream(rkey, mem);

            if (casPartNew != null)
            {
                // Go through a list of all the keys and see if they are "local"
                for (int i = 0; i < casPartNew.xmlChunk.Count; i++)
                {
                    xmlChunkDetails chunk = (xmlChunkDetails)casPartNew.xmlChunk[i];

                    for (int j = 0; j < 10; j++)
                    {
                        writeLocalResource(db, stencilPool[j].key);
                    }
                    writeLocalResource(db, chunk.Multiplier);
                    writeLocalResource(db, chunk.Overlay);
                    writeLocalResource(db, chunk.hair.RootColor);
                    writeLocalResource(db, chunk.hair.DiffuseColor);
                    writeLocalResource(db, chunk.hair.HighlightColor);
                    writeLocalResource(db, chunk.hair.TipColor);
                    writeLocalResource(db, chunk.hair.ScalpDiffuseMap);
                    writeLocalResource(db, chunk.hair.ScalpControlMap);
                    writeLocalResource(db, chunk.hair.ScalpSpecularMap);
                    writeLocalResource(db, chunk.hair.ScalpAO);
                    writeLocalResource(db, chunk.hair.FaceDiffuseMap);
                    writeLocalResource(db, chunk.hair.FaceControlMap);
                    writeLocalResource(db, chunk.hair.FaceSpecularMap);
                    writeLocalResource(db, chunk.hair.FaceAO);
                    writeLocalResource(db, chunk.Mask);
                    writeLocalResource(db, chunk.SkinSpecular);
                    writeLocalResource(db, chunk.SkinAmbient);
                    writeLocalResource(db, chunk.ClothingSpecular);
                    writeLocalResource(db, chunk.ClothingAmbient);
                    writeLocalResource(db, chunk.PartMask);
                    writeLocalResource(db, chunk.pattern[0].BackgroundImage);
                    writeLocalResource(db, chunk.pattern[1].BackgroundImage);
                    writeLocalResource(db, chunk.pattern[2].BackgroundImage);
                    writeLocalResource(db, chunk.pattern[3].BackgroundImage);
                    writeLocalResource(db, chunk.pattern[0].rgbmask);
                    writeLocalResource(db, chunk.pattern[1].rgbmask);
                    writeLocalResource(db, chunk.pattern[2].rgbmask);
                    writeLocalResource(db, chunk.pattern[3].rgbmask);
                    writeLocalResource(db, chunk.pattern[0].specmap);
                    writeLocalResource(db, chunk.pattern[1].specmap);
                    writeLocalResource(db, chunk.pattern[2].specmap);
                    writeLocalResource(db, chunk.pattern[3].specmap);
                    writeLocalResource(db, chunk.pattern[0].filename);
                    writeLocalResource(db, chunk.pattern[1].filename);
                    writeLocalResource(db, chunk.pattern[2].filename);
                    writeLocalResource(db, chunk.pattern[3].filename);
                    writeLocalResource(db, chunk.faceOverlay);
                    writeLocalResource(db, chunk.faceSpecular);
                    writeLocalResource(db, chunk.ControlMap);
                    writeLocalResource(db, chunk.DiffuseMap);

                    if (newPNGfiles.ContainsKey(i))
                    {
                        Stream newPNG = File.Open(newPNGfiles[i], FileMode.Open, FileAccess.Read, FileShare.Read);
                        ResourceKey keyPNG = new ResourceKey(0x626F60CE, (uint)(i + 1), instanceId, (uint)ResourceKeyOrder.ITG);
                        db.SetResourceStream(keyPNG, newPNG);
                        newPNG.Close();
                    }
                }
            }

            mem.Close();

        }

        private void writeLocalResource(Database db, string keyName)
        {
            if (String.IsNullOrEmpty(keyName)) return;
            //if (!validateKey(keyName)) return;

            if (Helpers.localFiles.ContainsKey(keyName))
            {
                ResourceKey key = new ResourceKey(keyName);
                Stream newDDS = File.Open((string)Helpers.localFiles[keyName], FileMode.Open, FileAccess.Read, FileShare.Read);
                db.SetResourceStream(key, newDDS);
                newDDS.Close();
            }

        }
    }
}