float2 getShadowMapTexCoords( float4 world, float3 lightPosition, matrix pvwLight, out float distanceToLight )
{
	// transform the point into the clip space of the light
	float4 lightClip = mul( pvwLight, world );
	distanceToLight = length( world.xyz - lightPosition );	

	// take light's clip space coordinates
	// and turn it into texture space
	// 0. homogenize the coordinates (gives [-1,1])
	// 1. scale and offset to [0,1]
	// 2. flip it upside down for DirectX idiocy
	float2 shadowMapUV = lightClip.xy / lightClip.w;
	shadowMapUV = 0.5 * ( shadowMapUV + float2( 1, 1 ) );
	return flipUV( shadowMapUV );
}

// One-tailed Chebyshev's Inequality
// given a distribution on x
// let u = mean( x ) and s^2 = variance( x )
// if t > u
// then P( x >= t ) <= p_max( t )
//   where p_max( t ) = s^2 / ( s^2 + ( t - u )^2 )
// else p_max = 1
float chebyshevUpperBound( float2 moments, float t )
{
	const float MIN_VARIANCE = 0.001;

	// standard shadow map test		
	float p = ( t <= moments.x );
	
	// compute variance
	float variance = moments.y - ( moments.x * moments.x );
	variance = max( variance, MIN_VARIANCE );
	
	// compute probabilistic upper bound
	float d = t - moments.x;
	float p_max = variance / ( variance + d * d );
	
	return max( p, p_max );
}

float computeVarianceShadowMapFraction( float4 world, float3 lightPosition, matrix pvwLight, Texture2D varianceShadowMap )
{
	float distanceToLight;
	float2 shadowMapUV = getShadowMapTexCoords( world, lightPosition, pvwLight, distanceToLight );	
	
	float2 moments = varianceShadowMap.Sample( ssLinear, shadowMapUV );	
	return chebyshevUpperBound( moments, distanceToLight );
}

float computeVarianceShadowMapFraction( float4 world, float3 lightPosition, matrix pvwLight, int lightIndex, Texture2DArray varianceShadowMaps )
{
	float distanceToLight;
	float2 shadowMapUV = getShadowMapTexCoords( world, lightPosition, pvwLight, distanceToLight );
	
	float2 moments = varianceShadowMaps.Sample( ssLinear, float3( shadowMapUV, lightIndex ) );
	return chebyshevUpperBound( moments, distanceToLight );
}