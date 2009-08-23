using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MadScience.Wrappers
{

    public enum FieldTypes : uint
    {
        AlphaMap = 0xc3faac4f,
        AlphaMaskThreshold = 0xe77a2b60,
        Ambient = 0x04a5daa3,
        AmbientOcclusionMap = 0xb01cba60,
        AmbientUVSelector = 0x797f8e81,
        AnimDir = 0x3f89c2ef,
        AnimSpeed = 0xd600cb63,
        AutoRainbow = 0x5f7800ea,
        BackFaceDiffuseContribution = 0xd641a1b1,
        bHasDetailmap = 0xe9008abe,
        bHasNormalMap = 0x5e99ee74,
        bIsTerrainRoad = 0xa4a17516,
        BloomFactor = 0x4168508b,
        BounceAmountMeters = 0xd8542d8b,
        ContourSmoothing = 0x1e27dccd,
        CounterMatrixRow1 = 0x1ef8655d,
        CounterMatrixRow2 = 0x1ef8655e,
        CutoutValidHeights = 0x6d43d7b7,
        DetailMap = 0x9205daa8,
        DetailUVScale = 0xcd985a0b,
        Diffuse = 0x637daa05,
        DiffuseMap = 0x6cc0fd85,
        DiffuseUVScale = 0x2d4e507e,
        DiffuseUVSelector = 0x91eebaff,
        DimmingCenterHeight = 0x01adace0,
        DimmingRadius = 0x32dfa298,
        DirtOverlay = 0x48372e62,
        DropShadowAtlas = 0x22ad8507,
        DropShadowStrength = 0x1b1ab4d5,
        EdgeDarkening = 0x8c27d8c9,
        Emission = 0x3bd441a0,
        EmissionMap = 0xf303d152,
        EmissiveBloomMultiplier = 0x490e6eb4,
        EmissiveLightMultiplier = 0x8ef71c85,
        FadeDistance = 0x957210ea,
        FresnelOffset = 0xfb66a8cb,
        ImposterTexture = 0xbdcf71c5,
        ImposterTextureAOandSI = 0x15c9d298,
        ImposterTextureWater = 0xbf3fb9fa,
        ImpostorDetailTexture = 0x56e1c6b2,
        ImpostorWater = 0x277cf8eb,
        Layer2Shift = 0x92692cb2,
        LayerOffset = 0x80d9bfe1,
        LightMapScale = 0x4f7dcb9b,
        MaskHeight = 0x849cdadc,
        MaskWidth = 0x707f712f,
        NoiseMap = 0xe19fd579,
        NoiseMapScale = 0x5e86dea1,
        NormalMap = 0x6e56548a,
        NormalMapScale = 0x3c45e334,
        NormalMapUVSelector = 0x415368b4,
        NormalUVScale = 0xba2d1ab9,
        PosOffset = 0x790ebf2c,
        PosScale = 0x487648e5,
        Reflective = 0x73c9923e,
        RefractionDistortionScale = 0xc3c472a1,
        RevealMap = 0xf3f22ac4,
        RippleDistanceScale = 0xccb35b98,
        RippleHeights = 0x6a07d7e1,
        RippleSpeed = 0x52dec070,
        RoadDetailMap = 0x28392dc6,
        RoadNormalMap = 0xbca022bc,
        RoadTexture = 0x53521204,
        RoomLightMap = 0xe7ca9166,
        RotationSpeed = 0x32003ad4,
        RugSort = 0x906997a9,
        ShadowAlphaTest = 0xfeb1f9cb,
        Shininess = 0xf755f7ff,
        SparkleCube = 0x1d90c086,
        SparkleSpeed = 0xba13921e,
        SpecStyle = 0x9554d40f,
        Specular = 0x2ce11842,
        SpecularMap = 0xad528a60,
        SpecularUVScale = 0xf12e27c3,
        SpecularUVSelector = 0xb63546ac,
        TerrainLightMap = 0x5fd5b006,
        TextureSpeedScale = 0x583df357,
        Transparency = 0x05d22fd3,
        Transparent = 0x988403f9,
        UVOffset = 0x57582869,
        UVScale = 0x159ba53e,
        UVScales = 0x420520e9,
        UVScrollSpeed = 0xf2eea6ec,
        UVTiling = 0x773cab85,
        VertexColorScale = 0xa2fd73ca,
        WaterScrollSpeedLayer1 = 0xafa11436,
        WaterScrollSpeedLayer2 = 0xafa11435,
        WindSpeed = 0x66e9b6bc,
        WindStrength = 0xbc4a2544,
        Unknown = 0x209b1c8e,
        Unknown2 = 0xdaa9532d,
        Unknown3 = 0x29bcdd1f,
        Unknown4 = 0x2eb8e9d4,
        HaloRamp = 0x84f6e0fb,
        HaloBlur = 0xc3ad4f50,
        HaloHighColor = 0xd4043258,
    }

    public enum ShaderType : uint
    {
        additive = 0x5af16731,
        BasinWater = 0x6aad2ad5,
        BrushedMetal = 0x3fd7990d,
        BurntTile = 0x690fdf06,
        CasSilk = 0x0072aa53,
        CasSimEyes = 0xb51ec997,
        CasSimHair = 0xfcf80ce1,
        CasSimHairSimple = 0xa7b368fb,
        CasSkin = 0x01772897,
        Counters = 0xa4172f62,
        DropShadow = 0xc09c7582,
        ExteriorCeiling = 0xd2ac4914,
        ExteriorWalls = 0xcd677552,
        Fence = 0x67107fe8,
        FlatMirror = 0xa68d9e29,
        Floors = 0xbc84d000,
        FloorsVisualizer = 0x2b1f3aec,
        Foliage = 0x4549e22e,
        FullBright = 0x14fa335e,
        Gemstones = 0xa063c1d0,
        Ghost = 0x2b1f3aec,
        GhostEyes = 0x8c88b4a8,
        GhostHair = 0x00c394a6,
        GlassForFences = 0x52986c62,
        GlassForObjects = 0x492eca7c,
        GlassForObjectsTranslucent = 0x849cf021,
        GlassForPortals = 0x81dd204d,
        GlassForRabbitHoles = 0x265ffaa1,
        ImpostorColorDefault = 0xed4fb30e,
        ImpostorColorGlow = 0x9661e300,
        ImposterLightingDefault = 0x5f03f969,
        ImpostorLightingGlow = 0x05954911,
        Instanced = 0x0cb82eb8,
        InstancedImpostorColor = 0xe7abde9c,
        LotImposter = 0x68601de3,
        LotPondWater = 0xe1386384,
        LotTerrain = 0x11d0b721,
        LotTerrainImposterMaker = 0xaee088f0,
        Occluder = 0x071fd3d4,
        OutdoorProp = 0x4d26bec0,
        Painting = 0xaa495821,
        Particle = 0x6da87a9b,
        ParticleAnim = 0x460e93f4,
        ParticleLight = 0xd9a8e549,
        PhongAlpha = 0xfc5fc212,
        Phong = 0xb9105a6d,
        PickCASSim = 0x26d1704a,
        PickCounters = 0xce0c0dc1,
        PickDefault = 0x9017b045,
        PickInstanced = 0xb7178269,
        PickRug = 0x18120028,
        PickSim = 0x301464c3,
        PickTerrain = 0x0f49bea1,
        PickWater = 0xc107590f,
        PickWalls = 0xb81ad379,
        Plumbob = 0xdef16564,
        Ponds = 0x79c38597,
        PreviewWallsAndFloors = 0x213d6300,
        RabbitHoleHighDetail = 0x8d346bbc,
        RabbitHoleMediumDetail = 0xaede7105,
        Roads = 0x5e0ac22e,
        RoadsCompositor = 0x7c8b3791,
        Roofs = 0x4c0628aa,
        RoofImpostorLighting = 0xcb14114c,
        Rug = 0x2a72b9a1,
        ShadowMapMerged = 0xe2918799,
        SimEyes = 0xcf8a70b4,
        SimEyelashes = 0x9d9da161,
        SimHair = 0x84fd7152,
        SimHairVisualizer = 0x109defb6,
        SimSilk = 0x53881019,
        Simple = 0x723aa6e7,
        SimSkin = 0x548394b9,
        SimSkinThumbnail = 0x9eff872b,
        SimSkinVisualizer = 0x969921ad,
        Stairs = 0x4ce2f497,
        StandingWater = 0x70fde012,
        StaticTerrain = 0xe05b91aa,
        StaticTerrainLowLOD = 0x413d7051,
        Subtractive = 0x0b272cc5,
        TerrainLightFog = 0x69eb86e4,
        TerrainVisualization = 0xc589e244,
        TreeBillboard = 0xedd106f2,
        TreeShadowCompositor = 0x974fba48,
        ThumbnailShadowPlane = 0xd32eec7b,
        VertexColor = 0xb39101ac,
        Walls = 0x974fba48,
    }


    public class SimGeomFile
    {
        public RcolHeader rcolHeader = new RcolHeader();
        public SimGeom simgeom = new SimGeom();

        public SimGeomFile()
        {
        }

        public SimGeomFile(Stream input)
        {
            loadFromStream(input);
        }

        public void Load(Stream input)
        {
            loadFromStream(input);
        }

        private void loadFromStream(Stream input)
        {
            this.rcolHeader.Load(input);
            this.simgeom.Load(input);
        }

        public void Save(Stream output)
        {
            if (this.rcolHeader.chunks.Count == 0)
            {
                this.rcolHeader.chunks.Add(new MadScience.Wrappers.OffsetSize());
            }
            if (this.rcolHeader.externalChunks.Count == 0 && this.rcolHeader.internalChunks.Count == 0)
            {
                throw new Exception("You must have a valid RCOL header");
                //return;
            }

            this.rcolHeader.Save(output);
            this.simgeom.Save(output);

            rcolHeader.chunks[0] = simgeom.offSize;

            output.Seek(rcolHeader.chunkListOffset, SeekOrigin.Begin);
            for (int i = 0; i < rcolHeader.chunks.Count; i++)
            {
                rcolHeader.chunks[i].Save(output);
            }
        }

        public Stream Save()
        {
            Stream temp = new MemoryStream();
            Save(temp);
            return temp;
        }
    }
    

    public class SimGeom
    {
        public string magic = "GEOM";
        public uint version = 5;
        public OffsetSize offSize = new OffsetSize();
        public uint embeddedId = 0;
        public MTNFChunk mtnfChunk = new MTNFChunk();
        public uint unk1 = 0;
        public uint unk2 = 0;

        public List<SimGeomVertexFormat> vertexFormats = new List<SimGeomVertexFormat>();

        public List<Vector3> vertices = new List<Vector3>();
        public List<Vector3> normals = new List<Vector3>();
        public List<Vector2> uvs = new List<Vector2>();
        public List<uint> bones = new List<uint>();
        public List<Vector4> weights = new List<Vector4>();
        public List<Vector3> tangentNormals = new List<Vector3>();
        public List<uint> tagVals = new List<uint>();
        public List<uint> vertexIds = new List<uint>();

        public uint faceFormatItemCount = 0;
        public byte faceFormatBytesPerFacePoint = 0;
        public List<UShort3> faces = new List<UShort3>();

        public uint flags = 0;
        public List<uint> boneHashes = new List<uint>();

        public KeyTable keytable = new KeyTable();

        public SimGeom()
        {
        }

        public SimGeom(Stream input)
        {
            loadFromStream(input);
        }

        public void Load(Stream input)
        {
            loadFromStream(input);
        }

        private void loadFromStream(Stream input)
        {
            this.magic = MadScience.StreamHelpers.ReadStringASCII(input, 4);
            this.version = MadScience.StreamHelpers.ReadValueU32(input);
            this.keytable.offset = MadScience.StreamHelpers.ReadValueU32(input);
            this.keytable.size = MadScience.StreamHelpers.ReadValueU32(input);

            this.embeddedId = MadScience.StreamHelpers.ReadValueU32(input);
            if (embeddedId != 0)
            {
                MadScience.StreamHelpers.ReadValueU32(input);
                this.mtnfChunk.Load(input);
            }

            this.unk1 = MadScience.StreamHelpers.ReadValueU32(input);
            this.unk2 = MadScience.StreamHelpers.ReadValueU32(input);

            uint numVerts = MadScience.StreamHelpers.ReadValueU32(input);

            uint vfCount = MadScience.StreamHelpers.ReadValueU32(input);

            vertexFormats.Clear();
            for (int i = 0; i < vfCount; i++)
            {
                vertexFormats.Add(new SimGeomVertexFormat(input));
            }

            for (int i = 0; i < numVerts; i++)
            {
                for (int j = 0; j < vertexFormats.Count; j++)
                {
                    SimGeomVertexFormat vf = (SimGeomVertexFormat)vertexFormats[j];
                    switch (vf.dataType)
                    {
                        case 1:
                            this.vertices.Add(new Vector3(input));
                            break;
                        case 2:
                            this.normals.Add(new Vector3(input));
                            break;
                        case 3:
                            this.uvs.Add(new Vector2(input));
                            break;
                        case 4:
                            this.bones.Add(MadScience.StreamHelpers.ReadValueU32(input));
                            break;
                        case 5:
                            this.weights.Add(new Vector4(input));
                            break;
                        case 6:
                            this.tangentNormals.Add(new Vector3(input));
                            break;
                        case 7:
                            this.tagVals.Add(MadScience.StreamHelpers.ReadValueU32(input));
                            break;
                        case 10:
                            this.vertexIds.Add(MadScience.StreamHelpers.ReadValueU32(input));
                            break;

                    }

                }
            }

            this.faceFormatItemCount = MadScience.StreamHelpers.ReadValueU32(input);
            this.faceFormatBytesPerFacePoint = MadScience.StreamHelpers.ReadValueU8(input);

            uint numFacePoints = MadScience.StreamHelpers.ReadValueU32(input);

            uint numPolygons = numFacePoints / 3;

            for (int i = 0; i < numPolygons; i++)
            {
                switch (faceFormatBytesPerFacePoint)
                {
                    case 2:
                        this.faces.Add(new UShort3(input));
                        break;
                }
            }

            // Tail section
            this.flags = MadScience.StreamHelpers.ReadValueU32(input);
            uint boneHashCount = MadScience.StreamHelpers.ReadValueU32(input);
            for (int i = 0; i < boneHashCount; i++)
            {
                this.boneHashes.Add(MadScience.StreamHelpers.ReadValueU32(input));
            }

            //long seekFrom = input.Position - 4;

            //input.Seek(this.keytable.offset + seekFrom, SeekOrigin.Begin);

            this.keytable.Load(input);

        }

        public void Save(Stream output)
        {
            this.offSize.offset = (uint)output.Position;

            StreamHelpers.WriteStringASCII(output, this.magic);
            StreamHelpers.WriteValueU32(output, this.version);

            StreamHelpers.WriteValueU32(output, 0);
            StreamHelpers.WriteValueU32(output, 0);

            StreamHelpers.WriteValueU32(output, this.embeddedId);
            if (this.embeddedId != 0)
            {
                StreamHelpers.WriteValueU32(output, 0);
                this.mtnfChunk.Save(output);

                output.Seek(this.mtnfChunk.offSize.offset - 4, SeekOrigin.Begin);
                StreamHelpers.WriteValueU32(output, this.mtnfChunk.offSize.size);
                output.Seek(this.mtnfChunk.offSize.offset + this.mtnfChunk.offSize.size, SeekOrigin.Begin);
            }

            StreamHelpers.WriteValueU32(output, this.unk1);
            StreamHelpers.WriteValueU32(output, this.unk2);

            StreamHelpers.WriteValueU32(output, (uint)this.vertices.Count);
            StreamHelpers.WriteValueU32(output, (uint)this.vertexFormats.Count);
            for (int i = 0; i < this.vertexFormats.Count; i++)
            {
                this.vertexFormats[i].Save(output);
            }

            for (int i = 0; i < this.vertices.Count; i++)
            {
                for (int j = 0; j < this.vertexFormats.Count; j++)
                {
                    SimGeomVertexFormat vf = this.vertexFormats[j];
                    switch (vf.dataType)
                    {
                        case 1:
                            this.vertices[i].Save(output);
                            break;
                        case 2:
                            this.normals[i].Save(output);
                            break;
                        case 3:
                            this.uvs[i].Save(output);
                            break;
                        case 4:
                            StreamHelpers.WriteValueU32(output, this.bones[i]);
                            break;
                        case 5:
                            this.weights[i].Save(output);
                            break;
                        case 6:
                            this.tangentNormals[i].Save(output);
                            break;
                        case 7:
                            StreamHelpers.WriteValueU32(output, this.tagVals[i]);
                            break;
                        case 10:
                            StreamHelpers.WriteValueU32(output, this.vertexIds[i]);
                            break;
                    }
                }
            }

            StreamHelpers.WriteValueU32(output, this.faceFormatItemCount);
            StreamHelpers.WriteValueU8(output, this.faceFormatBytesPerFacePoint);

            StreamHelpers.WriteValueU32(output, (uint)(this.faces.Count * 3));
            for (int i = 0; i < this.faces.Count; i++)
            {
                switch (this.faceFormatBytesPerFacePoint)
                {
                    case 2:
                        this.faces[i].Save(output);
                        break;
                }
            }

            StreamHelpers.WriteValueU32(output, this.flags);
            StreamHelpers.WriteValueU32(output, (uint)this.boneHashes.Count);
            for (int i = 0; i < this.boneHashes.Count; i++)
            {
                StreamHelpers.WriteValueU32(output, this.boneHashes[i]);
            }

            this.keytable.offset = (uint)(output.Position - this.offSize.offset) - 12;
            this.keytable.size = 0;
            this.keytable.Save(output);

            this.offSize.size = (uint)(output.Position - this.offSize.offset);

            output.Seek(this.offSize.offset + 8, SeekOrigin.Begin);
            StreamHelpers.WriteValueU32(output, this.keytable.offset);
            StreamHelpers.WriteValueU32(output, this.keytable.size);

            output.Seek(this.offSize.offset + this.offSize.size, SeekOrigin.Begin);

        }

        public Stream Save()
        {
            Stream temp = new MemoryStream();
            Save(temp);
            return temp;
        }
    }

    public class SimGeomVertexFormat
    {
        public uint dataType = 0;
        public uint subType = 0;
        public byte bytesPerElement = 0;

        public SimGeomVertexFormat()
        {
        }

        public SimGeomVertexFormat(Stream input)
        {
            loadFromStream(input);
        }

        public void Load(Stream input)
        {
            loadFromStream(input);
        }

        private void loadFromStream(Stream input)
        {
            this.dataType = StreamHelpers.ReadValueU32(input);
            this.subType = StreamHelpers.ReadValueU32(input);
            this.bytesPerElement = StreamHelpers.ReadValueU8(input);
        }

        public void Save(Stream output)
        {
            StreamHelpers.WriteValueU32(output, this.dataType);
            StreamHelpers.WriteValueU32(output, this.subType);
            StreamHelpers.WriteValueU8(output, this.bytesPerElement);
        }

    }

    public class MTNFChunk
    {    
        public string magic = "MTNF";
        public OffsetSize offSize = new OffsetSize();
        public List<MTNFEntry> entries = new List<MTNFEntry>();
        public uint blockSize = 0;

        public MTNFChunk()
        {
        }

        public MTNFChunk(Stream input)
        {
            loadFromStream(input);
        }

        public void Load(Stream input)
        {
            loadFromStream(input);
        }

        private void loadFromStream(Stream input)
        {
            this.magic = MadScience.StreamHelpers.ReadStringASCII(input, 4);
            MadScience.StreamHelpers.ReadValueU32(input); // Always zero
            this.blockSize = MadScience.StreamHelpers.ReadValueU32(input);

            this.entries.Clear();

            uint entryCount = MadScience.StreamHelpers.ReadValueU32(input);
            for (int i = 0; i < entryCount; i++)
            {
                MTNFEntry entry = new MTNFEntry();
                entry.fieldTypeHash = MadScience.StreamHelpers.ReadValueU32(input);
                entry.dataType = MadScience.StreamHelpers.ReadValueU32(input);
                entry.dataCount = MadScience.StreamHelpers.ReadValueU32(input);
                entry.offset = MadScience.StreamHelpers.ReadValueU32(input);
                entries.Add(entry);
            }
            for (int i = 0; i < entryCount; i++)
            {
                switch (entries[i].dataType)
                {
                    case 1:
                        for (int j = 0; j < entries[i].dataCount; j++)
                        {
                            entries[i].floats.Add(MadScience.StreamHelpers.ReadValueF32(input));
                        }
                        break;
                    case 2:
                    case 4:
                        for (int j = 0; j < entries[i].dataCount; j++)
                        {
                            entries[i].dwords.Add(MadScience.StreamHelpers.ReadValueU32(input));
                        }
                        break;
                }
            }

        }

        public void Save(Stream output)
        {
            this.offSize.offset = (uint)output.Position;

            MadScience.StreamHelpers.WriteStringASCII(output, this.magic);
            MadScience.StreamHelpers.WriteValueU32(output, 0);

            MadScience.StreamHelpers.WriteValueU32(output, 0); // We'll come back here later...

            MadScience.StreamHelpers.WriteValueU32(output, (uint)entries.Count);

            int startTailOffset = 16 + (entries.Count * 16);
            int addOffset = 0;
            for (int i = 0; i < entries.Count; i++)
            {
                MadScience.StreamHelpers.WriteValueU32(output, entries[i].fieldTypeHash);
                MadScience.StreamHelpers.WriteValueU32(output, entries[i].dataType);
                MadScience.StreamHelpers.WriteValueU32(output, entries[i].dataCount);
                MadScience.StreamHelpers.WriteValueU32(output, (uint)(startTailOffset + addOffset));
                Console.WriteLine(startTailOffset + addOffset);
                addOffset += (int)entries[i].dataCount * 4;
            }

            long followOffset = output.Position;

            for (int i = 0; i < entries.Count; i++)
            {
                switch (entries[i].dataType)
                {
                    case 1:
                        for (int j = 0; j < entries[i].dataCount; j++)
                        {
                            MadScience.StreamHelpers.WriteValueF32(output, entries[i].floats[j]);
                        }
                        break;
                    case 2:
                    case 4:
                        for (int j = 0; j < entries[i].dataCount; j++)
                        {
                            MadScience.StreamHelpers.WriteValueU32(output, entries[i].dwords[j]);
                        }
                        break;
                }
            }

            this.offSize.size = (uint)output.Position - this.offSize.offset;

            long followSize = output.Position - followOffset;

            output.Seek(this.offSize.offset + 8, SeekOrigin.Begin);
            StreamHelpers.WriteValueU32(output, (uint)followSize);

            // Seek back to the end
            output.Seek(this.offSize.offset + this.offSize.size, SeekOrigin.Begin);

        }

        public Stream Save()
        {
            Stream temp = new MemoryStream();
            Save(temp);
            return temp;
        }
    }

    public class MTNFEntry
    {
        public uint fieldTypeHash = 0;
        public uint dataType = 0;
        public uint dataCount = 0;
        public uint offset = 0;

        public List<float> floats = new List<float>();
        public List<uint> dwords = new List<uint>();
    }
}
