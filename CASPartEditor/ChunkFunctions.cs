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

            return simgeomfile.Save();
        }

        private void saveToDBPF(Database db, ulong instanceId, bool newInstance)
        {
            ResourceKey rkey;

            MemoryStream mem = new MemoryStream();
            casPartFile casPF = new casPartFile();

            // Do we have new meshes?  If so, we need to do some pretty heft modifications. :)

            string meshName = txtMeshName.Text;

            if (!String.IsNullOrEmpty(txtMeshLod1.Text))
            {
                uint customGroup = MadScience.StringHelpers.HashFNV24(meshName);
                keyName meshLod1 = new keyName(0x015A1849, customGroup, (ulong)MadScience.StringHelpers.HashFNV32(meshName + "_lod1"), meshName + "_lod1");
                keyName meshLod1_1 = new keyName(0x015A1849, customGroup, (ulong)MadScience.StringHelpers.HashFNV32(meshName + "_lod1_1"), meshName + "_lod1_1");
                keyName meshLod1_2 = new keyName(0x015A1849, customGroup, (ulong)MadScience.StringHelpers.HashFNV32(meshName + "_lod1_2"), meshName + "_lod1_2");
                keyName meshLod1_3 = new keyName(0x015A1849, customGroup, (ulong)MadScience.StringHelpers.HashFNV32(meshName + "_lod1_3"), meshName + "_lod1_3");
                keyName meshLod2 = new keyName(0x015A1849, customGroup, (ulong)MadScience.StringHelpers.HashFNV32(meshName + "_lod2"), meshName + "_lod2");
                keyName meshLod3 = new keyName(0x015A1849, customGroup, (ulong)MadScience.StringHelpers.HashFNV32(meshName + "_lod3"), meshName + "_lod3");

                keyName vpxyKey = new keyName(0x736884F1, 0x00000001, (ulong)customGroup);

                // Load in the VPXY - we need to modify it.
                //keyName oldVpxyKey = new keyName((tgi64)casPartSrc.tgi64list[casPartSrc.tgiIndexVPXY]);
                Stream vpxyStream = KeyUtils.findKey(casPartSrc.tgi64list[casPartSrc.tgiIndexVPXY].ToString(), 0);
                if (vpxyStream != null)
                {

                    keyName proxyFit = new keyName(0x736884F1, 0x00000001, meshName + "_fit");
                    keyName proxyFat = new keyName(0x736884F1, 0x00000001, meshName + "_fat");
                    keyName proxyThin = new keyName(0x736884F1, 0x00000001, meshName + "_thin");
                    keyName proxySpecial = new keyName(0x736884F1, 0x00000001, meshName + "_special");
                    keyName bodyBlendFat = new keyName(0x062C8204, 0x0, meshName + "_fat");
                    keyName bodyBlendFit = new keyName(0x062C8204, 0x0, meshName + "_fit");
                    keyName bodyBlendThin = new keyName(0x062C8204, 0x0, meshName + "_thin");
                    keyName bodyBlendSpecial = new keyName(0x062C8204, 0x0, meshName + "_special");

                    VPXYFile vpxyfile = new VPXYFile(vpxyStream);
                    vpxyfile.rcolHeader.internalChunks.Clear();
                    vpxyfile.rcolHeader.internalChunks.Add(vpxyKey.ToResourceKey());

                    vpxyfile.vpxy.linkEntries.Clear();
                    if (!String.IsNullOrEmpty(txtMeshLod1.Text))
                    {
                        // LOD 1
                        VPXYEntry vpxyE = new VPXYEntry();
                        vpxyE.tgiList.Add(meshLod1.ToResourceKey());
                        if (!String.IsNullOrEmpty(txtMeshLod1_1.Text)) vpxyE.tgiList.Add(meshLod1_1.ToResourceKey());
                        if (!String.IsNullOrEmpty(txtMeshLod1_2.Text)) vpxyE.tgiList.Add(meshLod1_2.ToResourceKey());
                        if (!String.IsNullOrEmpty(txtMeshLod1_3.Text)) vpxyE.tgiList.Add(meshLod1_3.ToResourceKey());
                        vpxyfile.vpxy.linkEntries.Add(vpxyE);
                    }
                    if (!String.IsNullOrEmpty(txtMeshLod2.Text))
                    {
                        // LOD 2
                        VPXYEntry vpxyE = new VPXYEntry();
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

                    vpxyStream = vpxyfile.Save();

                    vpxyfile.rcolHeader.internalChunks[0] = proxyFit.ToResourceKey();
                    Stream proxyFitFile = vpxyfile.Save();

                    vpxyfile.rcolHeader.internalChunks[0] = proxyFat.ToResourceKey();
                    Stream proxyFatFile = vpxyfile.Save();

                    vpxyfile.rcolHeader.internalChunks[0] = proxyThin.ToResourceKey();
                    Stream proxyThinFile = vpxyfile.Save();

                    vpxyfile.rcolHeader.internalChunks[0] = proxySpecial.ToResourceKey();
                    Stream proxySpecialFile = vpxyfile.Save();

                    //MemoryStream proxyFitFile = makeVPXYfile(proxyFit.ToResourceKey());
                    //MemoryStream proxyFatFile = makeVPXYfile(proxyFat.ToResourceKey());
                    //MemoryStream proxyThinFile = makeVPXYfile(proxyThin.ToResourceKey());
                    //MemoryStream proxySpecialFile = makeVPXYfile(proxySpecial.ToResourceKey());



                    //MemoryStream bodyBlendFitFile = makeBlendFile(proxyFit);
                    //MemoryStream bodyBlendFatFile = makeBlendFile(proxyFat);
                    //MemoryStream bodyBlendThinFile = makeBlendFile(proxyThin);
                    //MemoryStream bodyBlendSpecialFile = makeBlendFile(proxySpecial);

                    Stream bodyBlendFitFile = KeyUtils.findKey(casPartSrc.tgi64list[casPartSrc.tgiIndexBlendInfoFit].ToString(), 0);
                    Stream bodyBlendFatFile = KeyUtils.findKey(casPartSrc.tgi64list[casPartSrc.tgiIndexBlendInfoFat].ToString(), 0);
                    Stream bodyBlendThinFile = KeyUtils.findKey(casPartSrc.tgi64list[casPartSrc.tgiIndexBlendInfoThin].ToString(), 0);
                    Stream bodyBlendSpecialFile = KeyUtils.findKey(casPartSrc.tgi64list[casPartSrc.tgiIndexBlendInfoSpecial].ToString(), 0);

                    db.SetResourceStream(vpxyKey.ToResourceKey(), vpxyStream);
                    db.SetResourceStream(proxyFit.ToResourceKey(), proxyFitFile);
                    db.SetResourceStream(proxyFat.ToResourceKey(), proxyFatFile);
                    db.SetResourceStream(proxyThin.ToResourceKey(), proxyThinFile);
                    db.SetResourceStream(proxySpecial.ToResourceKey(), proxySpecialFile);
                    db.SetResourceStream(bodyBlendFit.ToResourceKey(), bodyBlendFitFile);
                    db.SetResourceStream(bodyBlendFat.ToResourceKey(), bodyBlendFatFile);
                    db.SetResourceStream(bodyBlendThin.ToResourceKey(), bodyBlendThinFile);
                    db.SetResourceStream(bodyBlendSpecial.ToResourceKey(), bodyBlendSpecialFile);

                    // Update the CAS part TGI links with the new VPXY
                    casPartNew.tgi64list[casPartNew.tgiIndexVPXY] = vpxyKey.ToResourceKey();
                    casPartNew.tgi64list[casPartNew.tgiIndexBlendInfoFat] = bodyBlendFat.ToResourceKey();
                    casPartNew.tgi64list[casPartNew.tgiIndexBlendInfoFit] = bodyBlendFit.ToResourceKey();
                    casPartNew.tgi64list[casPartNew.tgiIndexBlendInfoThin] = bodyBlendThin.ToResourceKey();
                    casPartNew.tgi64list[casPartNew.tgiIndexBlendInfoSpecial] = bodyBlendSpecial.ToResourceKey();

                    keyName bumpMapKey = new keyName();

                    if (String.IsNullOrEmpty(txtOtherBumpMap.Text) == false)
                    {
                        bumpMapKey = new keyName(txtOtherBumpMap.Text, meshName + "_n");
                        //kNames.Add(bumpMapKey);
                        Stream bumpMapStream = File.OpenRead(txtOtherBumpMap.Text);
                        if (txtOtherBumpMap.Text != "" && !txtOtherBumpMap.Text.StartsWith("key:")) db.SetResourceStream(bumpMapKey.ToResourceKey(), bumpMapStream);
                        bumpMapStream.Close();
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

                    if (!String.IsNullOrEmpty(txtMeshLod3.Text.Trim()))
                    {
                        db.SetResourceStream(meshLod3.ToResourceKey(), saveGeom(txtMeshLod3.Text, bumpMapKey.ToResourceKey()));
                    }

                }

            }

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