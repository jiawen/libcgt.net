#include "util.fxh"

float4x4 worldMatrix; // obj -> world
float4x4 normalMatrix; // obj normal -> world
float4x4 cameraMatrix; // world -> clip

const float3 lightColor = float3( 1, 1, 1 );
float3 eye;
float4 lightPosition;

void directVS( float4 opIn : POSITION, // object space positions and normals
			   float3 onIn : NORMAL,
			   out float4 clip : SV_POSITION,
			   out float4 wOut : TEXCOORD0,
			   out float3 nOut : TEXCOORD1 )
{
	float4 world = mul( worldMatrix, opIn );
	clip = mul( cameraMatrix, world );
	
	wOut = world;	
	nOut = mul( ( float3x3 )normalMatrix, onIn );	
}

void directPS( float4 clip : SV_POSITION,
			   float4 wIn : TEXCOORD0,
			   float3 nIn : TEXCOORD1,
			   out float4 cOut : SV_TARGET )			  
{
	float3 p = wIn.xyz;
	float3 n = normalize( nIn );
	float3 l = normalize( lightPosition.xyz - lightPosition.w * p );
	
	// compute diffuse color
	float ndl = max( 0, dot( n, l ) );
	float diffuseBRDF = ndl; // * materialColor
	// direct
	float3 directDiffuse = diffuseBRDF * lightColor;
		
	/*
	// compute specular color
	float3 v = normalize( eye - p );
	float3 h = normalize( l + v );
	float ndh = max( 0, dot( n, h ) );	
	float specularBRDF = ndl * pow( ndh, shininess );
	float3 directSpecular = lightColor * ks * lightIntensity * specularBRDF;
	
	float3 direct_and_indirect = visibility * ( directdiffuse + directspecular ) + max( cIn.xyz, float3( 0, 0, 0 ) ) * diffuseTexture;
	
	cOut.xyz = direct_and_indirect;
	*/
	
	cOut.xyz = directDiffuse;
	cOut.w = 1;
}

technique10 directLighting
{	
	pass p0
	{
		SetRasterizerState( solidRS );
		
		SetVertexShader( CompileShader( vs_4_1, directVS() ) );
		SetGeometryShader( NULL );
		SetPixelShader( CompileShader( ps_4_1, directPS() ) );
		
		SetDepthStencilState( depthTestEnabled, 0 );
		SetBlendState( alphaBlendDisabled, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );
	}
}