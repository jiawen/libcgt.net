#include "util.fxh"

Texture2D image;
Texture2DArray imageArray;
matrix pvw;
float2 delta;
float sigmaSquared;
float radius;

// un-normalized
float gaussian1D( float dx, float sigmaSquared )
{
	float x2 = dx * dx;
	return exp( -0.5 * x2 / sigmaSquared );
}

// un-normalized
float gaussian2D( float dx, float dy, float sigmaSquared )
{
	float r2 = dx * dx + dy * dy;
	return exp( -0.5 * r2 / sigmaSquared );
}

void gaussianBlurVS( float4 world : POSITION,
					 float2 uvIn : TEXCOORD0,
					 out float4 clip : SV_POSITION,
					 out float2 uvOut : TEXCOORD0 )
{
	clip = mul( pvw, world );
	uvOut = uvIn;
}

float4 gaussianBlurPS( float4 clip : SV_POSITION,
					   float2 uv : TEXCOORD0 ) : SV_Target
{
	float4 sum = 0;
	float weight = 0;	

	for( int i = -radius; i <= radius; ++i )
	{
		float g = gaussian1D( i, sigmaSquared );
		sum += g * image.Sample( ssPoint, uv + i * delta );
		weight += g;
	}
	
	return sum / weight;
}

void gaussianBlurMRTPS( float4 clip : SV_POSITION,
					    float2 uv : TEXCOORD0,
					    
					    out float4 color0 : SV_Target0,
					    out float4 color1 : SV_Target1,
					    out float4 color2 : SV_Target2,
					    out float4 color3 : SV_Target3,
					    out float4 color4 : SV_Target4,
					    out float4 color5 : SV_Target5,
					    out float4 color6 : SV_Target6,
					    out float4 color7 : SV_Target7 )
{
	float4 sum[ 8 ];
	float weight = 0;
	
	for( int i = 0; i < 7; ++i )
	{
		sum[ i ] = float4( 0, 0, 0, 0 );		
	}
		
	for( int x = -radius; x <= radius; ++x )
	{
		float g = gaussian1D( x, sigmaSquared );
		float2 uvdx = uv + x * delta;
		
		for( int i = 0; i < 7; ++i )
		{			
			sum[ i ] += g * imageArray.Sample( ssPoint, float3( uvdx, i ) );			
		}
		weight += g;
	}
	
	color0 = sum[ 0 ] / weight;
	color1 = sum[ 1 ] / weight;
	color2 = sum[ 2 ] / weight;
	color3 = sum[ 3 ] / weight;
	color4 = sum[ 4 ] / weight;
	color5 = sum[ 5 ] / weight;
	color6 = sum[ 6 ] / weight;
	color7 = sum[ 7 ] / weight;
}


technique10 GaussianBlur
{	
	pass p0
	{
		SetRasterizerState( solidRS );
		
		SetVertexShader( CompileShader( vs_4_1, gaussianBlurVS() ) );
		SetGeometryShader( NULL );
		SetPixelShader( CompileShader( ps_4_1, gaussianBlurPS() ) );
		
		SetDepthStencilState( depthTestDisabled, 0 );
		SetBlendState( alphaBlendDisabled, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );
	}
}

technique10 GaussianBlurMRT
{	
	pass p0
	{
		SetRasterizerState( solidRS );
		
		SetVertexShader( CompileShader( vs_4_1, gaussianBlurVS() ) );
		SetGeometryShader( NULL );
		SetPixelShader( CompileShader( ps_4_1, gaussianBlurMRTPS() ) );
		
		SetDepthStencilState( depthTestDisabled, 0 );
		SetBlendState( alphaBlendDisabled, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );
	}
}
