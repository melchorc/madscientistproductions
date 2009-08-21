/*********************************************************************NVMH3****
*******************************************************************************
$Revision: #4 $

Copyright NVIDIA Corporation 2008
TO THE MAXIMUM EXTENT PERMITTED BY APPLICABLE LAW, THIS SOFTWARE IS PROVIDED
*AS IS* AND NVIDIA AND ITS SUPPLIERS DISCLAIM ALL WARRANTIES, EITHER EXPRESS
OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, IMPLIED WARRANTIES OF MERCHANTABILITY
AND FITNESS FOR A PARTICULAR PURPOSE.  IN NO EVENT SHALL NVIDIA OR ITS SUPPLIERS
BE LIABLE FOR ANY SPECIAL, INCIDENTAL, INDIRECT, OR CONSEQUENTIAL DAMAGES
WHATSOEVER (INCLUDING, WITHOUT LIMITATION, DAMAGES FOR LOSS OF BUSINESS PROFITS,
BUSINESS INTERRUPTION, LOSS OF BUSINESS INFORMATION, OR ANY OTHER PECUNIARY
LOSS) ARISING OUT OF THE USE OF OR INABILITY TO USE THIS SOFTWARE, EVEN IF
NVIDIA HAS BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGES.

% This material shows and compares results from four popular and
% advanced schemes for emulating displaement mapping.  They are:
% Relief Mapping, Parallax Mapping, Normal Mapping, and Relief
% Mapping with Shadows.  Original File by Fabio Policarpo.

keywords: material bumpmap
date: 071005

Note: Strong discontinuties in the model geometric normal (e.g., very sharp
    differences from the normals in two or more parts of the same triangle)
    can cause unusual overall light-attentuation errors. Re-normalizing the
    rasterized normals in the pixel shader can correct this, but the case
    was considered rare enough that these extra steps were eliminated for
    code efficiency. If you see off lighting near sharp model edges, just
    normalize "IN.normal" in the calculation of the varible "att" (shared
    by all techniques).


keywords: DirectX10
// Note that this version has twin versions of all techniques,
//   so that this single effect file can be used in *either*
//   DirectX9 or DirectX10

To learn more about shading, shaders, and to bounce ideas off other shader
    authors and users, visit the NVIDIA Shader Library Forums at:

    http://developer.nvidia.com/forums/

*******************************************************************************
******************************************************************************/

/*****************************************************************/
/*** HOST APPLICATION IDENTIFIERS ********************************/
/*** Potentially predefined by varying host environments *********/
/*****************************************************************/

int global : SasGlobal
<
	bool SasUiVisible = false;
	int3 SasVersion = {1,0,0};
>;

// #define _XSI_		/* predefined when running in XSI */
// #define TORQUE		/* predefined in TGEA 1.7 and up */
// #define _3DSMAX_		/* predefined in 3DS Max */
#ifdef _3DSMAX_
int ParamID = 0x0003;		/* Used by Max to select the correct parser */
#endif /* _3DSMAX_ */
#ifdef _XSI_
#define Main Static		/* Technique name used for export to XNA */
#endif /* _XSI_ */

#ifndef FXCOMPOSER_VERSION	/* for very old versions */
#define FXCOMPOSER_VERSION 180
#endif /* FXCOMPOSER_VERSION */

#ifndef DIRECT3D_VERSION
#define DIRECT3D_VERSION 0x900
#endif /* DIRECT3D_VERSION */

#define FLIP_TEXTURE_Y	/* Different in OpenGL & DirectX */

/*****************************************************************/
/*** EFFECT-SPECIFIC CODE BEGINS HERE ****************************/
/*****************************************************************/

/******* Lighting Macros *******/
/** To use "Object-Space" lighting definitions, change these two macros: **/
#define LIGHT_COORDS "World"
// #define OBJECT_SPACE_LIGHTS /* Define if LIGHT_COORDS is "Object" */

/**** UNTWEAKABLES: Hidden & Automatically-Tracked Parameters **********/

// transform object vertices to world-space:
float4x4 gWorldXf : World < string UIWidget="None"; >;
// transform object normals, tangents, & binormals to world-space:
float4x4 gWorldITXf : WorldInverseTranspose < string UIWidget="None"; >;
// transform object vertices to view space and project them in perspective:
float4x4 gWvpXf : WorldViewProjection < string UIWidget="None"; >;
// provide tranform from "view" or "eye" coords back to world-space:
float4x4 gViewIXf : ViewInverse < string UIWidget="None"; >;
float4x4 gViewXf : View <string UIWidget="none";>;
float4x4 gWorldViewXf : WorldView <string UIWidget="none";>;

/////////////// Tweakables //////////

bool gUseStencil = false;

float3 gLamp0Pos : POSITION <
    string Object = "PointLight0";
    string UIName =  "Lamp 0 Position";
    string Space = (LIGHT_COORDS);
> = {-0.5f,2.0f,1.25f};

float gTileCount <
    string UIName = "Tile Repeat";
    string UIWidget = "slider";
    float UIMin = 1.0;
    float UIStep = 1.0;
    float UIMax = 32.0;
> = 8;

// Ambient Light
float3 gAmbiColor : AMBIENT <
    string UIName =  "Ambient Light";
    string UIWidget = "Color";
> = {0.07f,0.07f,0.07f};

// surface color
float3 gSurfaceColor : DIFFUSE <
    string UIName =  "Surface";
    string UIWidget = "Color";
> = {1,1,1};

float3 gSpecColor <
    string UIName = "Specular";
    string UIWidget = "color";
> = {0.75,0.75,0.75};

float gPhongExp <
    string UIName = "Phong Exponent";
    string UIWidget = "slider";
    float UIMin = 8.0f;
    float UIStep = 8;
    float UIMax = 256.0f;
> = 128.0;

/*********** TEXTURES ***************/

texture gStencilTexture : DIFFUSE;

sampler2D gStencilSampler = sampler_state {
    Texture = <gStencilTexture>;
    MinFilter = Linear;
    MipFilter = Linear;
    MagFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};

texture gSkinTexture : DIFFUSE;

sampler2D gSkinSampler = sampler_state {
    Texture = <gSkinTexture>;
    MinFilter = Linear;
    MipFilter = Linear;
    MagFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};

texture gSkinSpecular : DIFFUSE;

sampler2D gSkinSpecularSampler = sampler_state {
    Texture = <gSkinSpecular>;
    MinFilter = Linear;
    MipFilter = Linear;
    MagFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};

texture gReliefTexture : NORMAL;

sampler2D gReliefSampler = sampler_state {
    Texture = <gReliefTexture>;
    MinFilter = Linear;
    MipFilter = Linear;
    MagFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};

/********** CONNECTOR STRUCTURES *****************/

struct AppVertexData {
    float4 pos		: POSITION;
    float3 normal	: NORMAL; // expected to be normalized
    float2 txcoord	: TEXCOORD0;
    float3 tangent	: TANGENT0; // pre-normalized
    float3 binormal	: BINORMAL0; // pre-normalized
};

struct VertexOutput {
    float4 hpos		: POSITION;
    float2 UV		: TEXCOORD0;
    float3 vpos		: TEXCOORD1;
    float3 tangent	: TEXCOORD2;
    float3 binormal	: TEXCOORD3;
    float3 normal	: TEXCOORD4;
    float4 lightpos	: TEXCOORD5;
    float4 color	: COLOR0;
};

/*** SHADER FUNCTIONS **********************************************/

VertexOutput view_spaceVS(AppVertexData IN,
    uniform float4x4 WorldITXf, // our four standard "untweakable" xforms
	uniform float4x4 WorldXf,
	uniform float4x4 ViewIXf,
	uniform float4x4 WvpXf,
    uniform float4x4 ViewXf,
    uniform float4x4 WorldViewXf,
    uniform float TileCount,
    uniform float3 LampPos
) {
    VertexOutput OUT = (VertexOutput)0;
    // isolate WorldViewXf rotation-only part
    float3x3 modelViewRotXf;
    modelViewRotXf[0] = WorldViewXf[0].xyz;
    modelViewRotXf[1] = WorldViewXf[1].xyz;
    modelViewRotXf[2] = WorldViewXf[2].xyz;
    float4 Po = float4(IN.pos.xyz,1.0);
    OUT.hpos = mul(Po,WvpXf);
    // vertex position in view space (with model transformations)
    OUT.vpos = mul(Po,WorldViewXf).xyz;
    // light position in view space
    float4 Lw = float4(LampPos.xyz,1); // this point in world space
    OUT.lightpos = mul(Lw,ViewXf); // this point in view space
    // tangent space vectors in view space (with model transformations)
    OUT.tangent = mul(IN.tangent,modelViewRotXf);
    OUT.binormal = mul(IN.binormal,modelViewRotXf);
    OUT.normal = mul(IN.normal,modelViewRotXf);
    // copy color and texture coordinates
    OUT.color = float4(1, 1, 1, 1);
    OUT.UV = TileCount * IN.txcoord.xy;
    return OUT;
}

/************ PIXEL SHADERS ******************/

float4 normal_mapPS(VertexOutput IN,
		    uniform float3 SurfaceColor,
		    uniform sampler2D SkinSampler,
		    uniform sampler2D SkinSpecularSampler,
		    uniform sampler2D StencilSampler,
		    uniform sampler2D ReliefSampler,
		    uniform float PhongExp,
		    uniform float3 SpecColor,
		    uniform float3 AmbiColor,
			uniform bool UseStencil			
) : COLOR
{
	////////////////// Texture compositing
	
	// Diffuse color
	// Load in the diffuse textures
	float4 texCol2 = float4(1, 1, 1, 1);
	if (UseStencil)
	{
	  texCol2 = tex2D(StencilSampler,IN.UV);
	}	
	float3 texCol3 = tex2D(SkinSampler,IN.UV).rgb;
	
	// Start with white
	float4 diffuseColor = float4(1, 1, 1, 1);
	// Mask off by the multiplier (Should also apply masks at this point)
	//diffuseColor.a = texCol1.a;
	// Perform color blending srcblend = destcolor and destblend=srccolor
	//diffuseColor.rgb = float3(diffuseColor.r * texCol1.r, diffuseColor.g * texCol1.g, diffuseColor.b * texCol1.b) * 2;
	// Alpha blending of the stencil
	diffuseColor = texCol2 * texCol2.a + diffuseColor * (1 - texCol2.a);
	// Alpha blend the shirt onto the skin texture
    float3 texCol = texCol3 * (1 - diffuseColor.a) + diffuseColor.xyz * diffuseColor.a;
    
    // Specular map influence - alpha blend the shirt specular onto the skin specular
    float specular = tex2D(SkinSpecularSampler,IN.UV).b * (1 - diffuseColor.a) * texCol3.b * 2 * diffuseColor.a;

	////////////////// Pixel rendering

	// Scale the normal map data from 0..1 to -1..1 - also alpha blend the normal map against straight up normal using the shirt layer alpha so the shirt doesn't get perturbed
    float3 tNorm = float3(0,0,0);
    //float3 tNorm = (tex2D(ReliefSampler,IN.UV).agg * (1 - texCol1.a) + texCol1.a * 0.5) * 2.0 - float3(1.0,1.0,1.0);
    // Calculate the missing component of the normal map, care of Pythagorean theorem
    // If the answer to the sqrt should be negative, the surface will have been culled anyway
    //tNorm = normalize(float3(tNorm.x, tNorm.y, sqrt(1.0 - (tNorm.x * tNorm.x + tNorm.y * tNorm.y))));
    
    // transform tNorm to world space
    //tNorm = normalize(tNorm.x*IN.tangent -
	//	      tNorm.y*IN.binormal + 
	//	      tNorm.z*IN.normal);
    
    // view and light directions
    float3 Vn = normalize(IN.vpos);
    float3 Ln = normalize(IN.lightpos.xyz-IN.vpos);
    
    // compute diffuse and specular terms
    float att = saturate(dot(Ln,IN.normal));
    float diff = saturate(dot(Ln,tNorm));
    float spec = saturate(normalize(Ln-Vn));
    spec = pow(spec,PhongExp);
    
    // compute final color
    float3 finalcolor = AmbiColor*texCol +
	    att*(texCol*SurfaceColor.xyz*diff+SpecColor*spec*specular);
    return float4(finalcolor.rgb,1.0);
}

///////////////////////////////////////
/// TECHNIQUES ////////////////////////
///////////////////////////////////////


technique normal_mapping <
	string Script = "Pass=p0;";
> {
    pass p0 <
	string Script = "Draw=geometry;";
    > {
        VertexShader = compile vs_2_0 view_spaceVS(gWorldITXf,gWorldXf,
				gViewIXf,gWvpXf,
					    gViewXf,gWorldViewXf,
					    gTileCount,
					    gLamp0Pos);
		ZEnable = true;
		ZWriteEnable = true;
		ZFunc = LessEqual;
		AlphaBlendEnable = true;
		CullMode = None;
        PixelShader = compile ps_2_0 normal_mapPS(
						gSurfaceColor,
						gSkinSampler,
						gSkinSpecularSampler,
						gStencilSampler,
						gReliefSampler,
						gPhongExp,
						gSpecColor,
						gAmbiColor,
						gUseStencil);
    }
}


/****************************************** EOF ***/