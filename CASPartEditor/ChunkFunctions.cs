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
        private void saveGeom(Stream input, Stream output, List<keyName> keynames)
        {
            BinaryReader reader = new BinaryReader(input);
            BinaryWriter writer = new BinaryWriter(output);

            uint rcolVersion = reader.ReadUInt32();
            writer.Write(rcolVersion);
            uint rcolDatatype = reader.ReadUInt32();
            writer.Write(rcolDatatype);

            uint rcolIndex3 = reader.ReadUInt32();
            writer.Write(rcolIndex3);
            uint rcolIndex1 = reader.ReadUInt32();
            writer.Write(rcolIndex1);
            uint rcolIndex2 = reader.ReadUInt32();
            writer.Write(rcolIndex2);

            for (int i = 0; i < rcolIndex2; i++)
            {
                ulong instanceId = reader.ReadUInt64();
                writer.Write(instanceId);
                uint typeId = reader.ReadUInt32();
                writer.Write(typeId);
                uint groupId = reader.ReadUInt32();
                writer.Write(groupId);
            }

            for (int i = 0; i < rcolIndex2; i++)
            {
                uint chunkOffset = reader.ReadUInt32();
                writer.Write(chunkOffset);
                uint chunkSize = reader.ReadUInt32();
                writer.Write(chunkSize);
            }

            // Real GEOM chunk
            string geomString = MadScience.StreamHelpers.ReadStringASCII(input, 4);
            MadScience.StreamHelpers.WriteStringASCII(output, geomString);

            uint geomVersion = reader.ReadUInt32();
            writer.Write(geomVersion);

            uint tailOffset = reader.ReadUInt32();
            writer.Write(tailOffset);

            long seekFrom = input.Position;
            //uint tailSize = reader.ReadUInt32();
            //writer.Write(tailSize);

            //int bytesLeft = (int)((tailOffset + startGeom) - input.Position);
            int bytesLeft = (int)(input.Length - input.Position);
            writer.Write(reader.ReadBytes(bytesLeft));

            output.Seek(tailOffset + seekFrom, SeekOrigin.Begin);
            input.Seek(tailOffset + seekFrom, SeekOrigin.Begin);

            // Tail section
            /*
            uint tailStart = reader.ReadUInt32();
            if (tailStart != 4)
            {
                // Looks like we seeked to the wrong place... try going back a bit
                output.Seek(tailOffset + 40, SeekOrigin.Begin);
                input.Seek(tailOffset + 40, SeekOrigin.Begin);
                tailStart = reader.ReadUInt32();

            }
            writer.Write(tailStart);
            uint countTail = reader.ReadUInt32();
            writer.Write(countTail);
            for (int i = 0; i < countTail; i++)
            {
                writer.Write(reader.ReadUInt32());
            }
            */
            // TGI list
            uint numOrig = reader.ReadUInt32();
            writer.Write(numOrig);

            // How many does the original mesh have? - Some meshes (Accessories) have only 3 TGI links.  Some have 4, some have 5.
            // If 3 then chop off first two
            // If 4 then chop off first one
            // If 5 then save all

            int startAt = 0;
            if (numOrig == 3)
            {
                startAt = 2;
            }
            if (numOrig == 4)
            {
                startAt = 1;
            }

            for (int i = startAt; i < keynames.Count; i++)
            {
                if (keynames[i].ToString() != keynames[i].Blank)
                {
                    writer.Write(keynames[i].typeId);
                    writer.Write(keynames[i].groupId);
                    writer.Write(keynames[i].instanceId);
                }
                else
                {
                    // Skip this key
                    writer.Write(reader.ReadUInt32());
                    writer.Write(reader.ReadUInt32());
                    writer.Write(reader.ReadUInt64());
                }
            }

            Console.WriteLine("Done saving mesh...");
        }

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

                    vpxyStream.Seek(0x14, SeekOrigin.Begin);
                    MadScience.StreamHelpers.WriteValueU64(vpxyStream, vpxyKey.instanceId);
                    MadScience.StreamHelpers.WriteValueU32(vpxyStream, vpxyKey.typeId);
                    MadScience.StreamHelpers.WriteValueU32(vpxyStream, vpxyKey.groupId);

                    vpxyStream.Seek(vpxyStream.Length - 48, SeekOrigin.Begin);
                    MadScience.StreamHelpers.WriteValueU32(vpxyStream, meshLod1.typeId);
                    MadScience.StreamHelpers.WriteValueU32(vpxyStream, meshLod1.groupId);
                    MadScience.StreamHelpers.WriteValueU64(vpxyStream, meshLod1.instanceId);
                    MadScience.StreamHelpers.WriteValueU32(vpxyStream, meshLod2.typeId);
                    MadScience.StreamHelpers.WriteValueU32(vpxyStream, meshLod2.groupId);
                    MadScience.StreamHelpers.WriteValueU64(vpxyStream, meshLod2.instanceId);
                    MadScience.StreamHelpers.WriteValueU32(vpxyStream, meshLod3.typeId);
                    MadScience.StreamHelpers.WriteValueU32(vpxyStream, meshLod3.groupId);
                    MadScience.StreamHelpers.WriteValueU64(vpxyStream, meshLod3.instanceId);


                    MemoryStream proxyFitFile = makeVPXYfile(proxyFit.ToResourceKey());
                    MemoryStream proxyFatFile = makeVPXYfile(proxyFat.ToResourceKey());
                    MemoryStream proxyThinFile = makeVPXYfile(proxyThin.ToResourceKey());
                    MemoryStream proxySpecialFile = makeVPXYfile(proxySpecial.ToResourceKey());

                    MemoryStream bodyBlendFitFile = makeBlendFile(proxyFit);
                    MemoryStream bodyBlendFatFile = makeBlendFile(proxyFat);
                    MemoryStream bodyBlendThinFile = makeBlendFile(proxyThin);
                    MemoryStream bodyBlendSpecialFile = makeBlendFile(proxySpecial);

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


                    // Modify the meshes if they need a replacement bump map
                    List<keyName> kNames = new List<keyName>();

                    kNames.Add(new keyName());
                    kNames.Add(new keyName());
                    if (String.IsNullOrEmpty(txtOtherBumpMap.Text) == false)
                    {
                        keyName bumpMapKey = new keyName(txtOtherBumpMap.Text, meshName + "_n");
                        kNames.Add(bumpMapKey);
                        if (txtOtherBumpMap.Text != "" && !txtOtherBumpMap.Text.StartsWith("key:")) db.SetResourceStream(bumpMapKey.ToResourceKey(), File.OpenRead(txtOtherBumpMap.Text));
                    }
                    else
                    {
                        kNames.Add(new keyName());
                    }
                    kNames.Add(new keyName());
                    kNames.Add(new keyName());

                    Stream blah;

                    MemoryStream geomLod1 = new MemoryStream();
                    if (txtMeshLod1.Text.Trim() != "")
                    {
                        blah = File.Open(txtMeshLod1.Text, FileMode.Open);
                        saveGeom(blah, geomLod1, kNames);
                        blah.Close();
                        db.SetResourceStream(meshLod1.ToResourceKey(), geomLod1);
                    }
                    geomLod1.Close();

                    MemoryStream geomLod2 = new MemoryStream();
                    if (txtMeshLod2.Text.Trim() != "")
                    {
                        blah = File.Open(txtMeshLod2.Text, FileMode.Open);
                        saveGeom(blah, geomLod2, kNames);
                        blah.Close();
                        db.SetResourceStream(meshLod2.ToResourceKey(), geomLod2);
                    }
                    geomLod2.Close();

                    MemoryStream geomLod3 = new MemoryStream();
                    if (txtMeshLod3.Text.Trim() != "")
                    {
                        blah = File.Open(txtMeshLod3.Text, FileMode.Open);
                        saveGeom(blah, geomLod3, kNames);
                        blah.Close();
                        db.SetResourceStream(meshLod3.ToResourceKey(), geomLod3);
                    }
                    geomLod3.Close();

                    blah = null;
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