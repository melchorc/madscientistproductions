float4x4 gWvpXf : WorldViewProjection;
 
float3 gColor : DIFFUSE = {1,1,1};
 
struct AppVertexData {
    float4 pos		: POSITION;
};
 
struct VertexOutput {
    float4 hpos		: POSITION;
};
 
VertexOutput wireframe_vs(AppVertexData IN,
	uniform float4x4 WvpXf
) {
    VertexOutput OUT = (VertexOutput)0;
    OUT.hpos = mul(float4(IN.pos.xyz,1.0),WvpXf) - float4(0, 0, 0.0001, 0);
    return OUT;
}
 
/************ PIXEL SHADERS ******************/
 
float4 wireframe_ps(VertexOutput IN,
		    uniform float3 Color
) : COLOR
{
	return float4(Color.rgb, 1.0);
}
 
 
technique wireframe
{
    pass p0
    {
        VertexShader = compile vs_2_0 wireframe_vs(gWvpXf);
		ZEnable = true;
		ZWriteEnable = true;
		ZFunc = LessEqual;
		AlphaBlendEnable = false;
		CullMode = Cw;
		FillMode = Wireframe;
        PixelShader = compile ps_2_0 wireframe_ps(gColor);
    }
}


/****************************************** EOF ***/
