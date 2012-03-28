#include "util.fxh"

float4x4 pvw;
float4x4 textureMatrix;
Texture2D tex;

struct VS_IN
{
	float4 model : POSITION;
	float4 color : COLOR;
};

struct PS_IN
{
	float4 clip : SV_POSITION;
	float4 world : TEXCOORD0;
	float4 color : COLOR;
};


PS_IN vs( VS_IN input )
{
	PS_IN output = ( PS_IN )0;
	
	output.clip = mul( pvw, input.model );
	output.world = input.model; // TODO: separate into projection, world, and model matrices
	output.color = input.color;
	
	return output;
}

float4 ps( PS_IN input ) : SV_Target
{
	return input.color;
}

technique10 renderOpaque
{
	pass p0
	{
		SetRasterizerState( solidRS );
		
		SetVertexShader( CompileShader( vs_4_1, vs() ) );
		SetGeometryShader( NULL );
		SetPixelShader( CompileShader( ps_4_1, ps() ) );
		
		SetDepthStencilState( depthTestEnabled, 0 );
		SetBlendState( alphaBlendDisabled, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );
	}
}

technique10 renderAlpha
{
	pass p0
	{
		SetRasterizerState( solidRS );
		
		SetVertexShader( CompileShader( vs_4_1, vs() ) );
		SetGeometryShader( NULL );
		SetPixelShader( CompileShader( ps_4_1, ps() ) );
		
		SetDepthStencilState( depthTestEnabledWritesDisabled, 0 );
		SetBlendState( alphaBlendOver, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );
	}
}

technique10 renderAdditive
{
	pass p0
	{
		SetRasterizerState( solidRS );
		
		SetVertexShader( CompileShader( vs_4_1, vs() ) );
		SetGeometryShader( NULL );
		SetPixelShader( CompileShader( ps_4_1, ps() ) );
		
		SetDepthStencilState( depthTestEnabledWritesDisabled, 0 );
		SetBlendState( alphaBlendOneOne, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );
	}
}

struct VS_TEX_IN
{
	float4 model : POSITION;
	float4 texcoord : TEXCOORD0;
};

struct PS_TEX_IN
{
	float4 clip : SV_POSITION;
	float4 world : TEXCOORD0;
	float4 texcoord : TEXCOORD1;
};

PS_TEX_IN vsTex( VS_TEX_IN input )
{
	PS_TEX_IN output = ( PS_TEX_IN )0;
	
	output.clip = mul( pvw, input.model );
	output.world = input.model; // TODO: separate into projection, world, and model matrices
	output.texcoord = mul( textureMatrix, input.texcoord );
	
	return output;
}

float4 psTex( PS_TEX_IN input ) : SV_TARGET
{
	return tex.Sample( ssPoint, input.texcoord.xy );
}

technique10 renderTexOpaque
{
	pass p0
	{
		SetRasterizerState( solidRS );
		
		SetVertexShader( CompileShader( vs_4_1, vsTex() ) );
		SetGeometryShader( NULL );
		SetPixelShader( CompileShader( ps_4_1, psTex() ) );
		
		SetDepthStencilState( depthTestDisabled, 0 );
		SetBlendState( alphaBlendDisabled, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );
	}
}

technique10 renderTexAlpha
{
	pass p0
	{
		SetRasterizerState( solidRS );
		
		SetVertexShader( CompileShader( vs_4_1, vsTex() ) );
		SetGeometryShader( NULL );
		SetPixelShader( CompileShader( ps_4_1, psTex() ) );
		
		SetDepthStencilState( depthTestDisabled, 0 );
		SetBlendState( alphaBlendOver, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );
	}
}
