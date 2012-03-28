#include "util.fxh"

cbuffer cbPerLight
{
	matrix pvw;
	float3 lightPosition;
};

void renderVarianceShadowMapVS( float4 world : POSITION,
							    out float4 clip : SV_POSITION,
								out float4 worldOut : TEXCOORD0 )
{
	clip = mul( pvw, world );
	worldOut = world;
}

float2 computeMoments( float depth )
{ 
	float2 moments;
		
	// first moment is the depth itself.   
	moments.x = depth;
	
	// Compute partial derivatives of depth.   
	float dx = ddx( depth );
	float dy = ddy( depth );
	
	// Compute second moment over the pixel extents
	moments.y = depth * depth + 0.25 * ( dx * dx + dy * dy );
	
	//moments.x = 1;
	moments.y = depth * depth;
	
	return moments;
}

float4 renderVarianceShadowMapPS( float4 clip : SV_POSITION,
								  float4 world : TEXCOORD0 ) : SV_Target
{	
	float depth = length( world.xyz - lightPosition );
	float2 moments = computeMoments( depth );
	return float4( moments, 0, 0 );
}

technique10 renderVarianceShadowMap
{
	pass p0
	{
		SetRasterizerState( solidRS );
		
		SetVertexShader( CompileShader( vs_4_1, renderVarianceShadowMapVS() ) );
		SetGeometryShader( NULL );
		SetPixelShader( CompileShader( ps_4_1, renderVarianceShadowMapPS() ) );
		
		SetDepthStencilState( depthTestEnabled, 0 );
		SetBlendState( alphaBlendDisabled, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );
	}	
}
