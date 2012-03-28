#define PI 3.141592653589793

RasterizerState solidRS
{
	FrontCounterClockwise = TRUE;
	CullMode = NONE;
	FillMode = SOLID;
};

RasterizerState solidCullBackRS
{
	FrontCounterClockwise = TRUE;
	CullMode = BACK;
	FillMode = SOLID;
};

RasterizerState solidBiasedRS
{
	FrontCounterClockwise = TRUE;
	CullMode = NONE;
	FillMode = SOLID;
	DepthBias = -10;
	SlopeScaledDepthBias = -0.5;
};

RasterizerState wireframeRS
{
	FrontCounterClockwise = TRUE;
	CullMode = NONE;
	FillMode = WIREFRAME;	
};

RasterizerState wireframeBiasedRS
{
	FrontCounterClockwise = TRUE;
	CullMode = NONE;
	FillMode = WIREFRAME;
	//DepthBias = -128;
};

SamplerState ssAnisotropic4
{
	Filter = ANISOTROPIC;
	AddressU = CLAMP;
	AddressV = CLAMP;
	MaxAnisotropy = 4;
};

SamplerState ssAnisotropic8
{
	Filter = ANISOTROPIC;
	AddressU = CLAMP;
	AddressV = CLAMP;
	MaxAnisotropy = 8;
};

SamplerState ssAnisotropic16
{
	Filter = ANISOTROPIC;
	AddressU = CLAMP;
	AddressV = CLAMP;
	MaxAnisotropy = 16;
};

SamplerState ssLinear
{
	Filter = MIN_MAG_LINEAR_MIP_POINT;
	AddressU = CLAMP;
	AddressV = CLAMP;
};

SamplerState ssPoint
{
	Filter = MIN_MAG_MIP_POINT;
	AddressU = CLAMP;
	AddressV = CLAMP;
};

SamplerState ssMinMagMipLinear
{
	Filter = MIN_MAG_MIP_LINEAR;
	AddressU = CLAMP;
	AddressV = CLAMP;
};

SamplerComparisonState ssPointCmpGT
{
	Filter = COMPARISON_MIN_MAG_MIP_POINT;
	AddressU = CLAMP;
	AddressV = CLAMP;
	ComparisonFunc = GREATER;
};

SamplerComparisonState ssPointCmpLT
{
	Filter = COMPARISON_MIN_MAG_MIP_POINT;
	AddressU = CLAMP;
	AddressV = CLAMP;
	ComparisonFunc = LESS;
};

SamplerComparisonState ssLinearCmpGT
{
	Filter = COMPARISON_MIN_MAG_MIP_LINEAR;
	AddressU = CLAMP;
	AddressV = CLAMP;
	ComparisonFunc = GREATER;
};

SamplerComparisonState ssLinearCmpLT
{
	Filter = COMPARISON_MIN_MAG_MIP_LINEAR;
	AddressU = CLAMP;
	AddressV = CLAMP;
	ComparisonFunc = LESS;
};

BlendState alphaBlendDisabled
{
	BlendEnable[ 0 ] = FALSE;
};

BlendState alphaBlendOver
{
	BlendEnable[ 0 ] = TRUE;
	BlendOp = ADD;
	SrcBlend = SRC_ALPHA;
	DestBlend = INV_SRC_ALPHA;

	BlendOpAlpha = ADD;
	SrcBlendAlpha = ONE;
	DestBlendAlpha = INV_SRC_ALPHA;
};

BlendState alphaBlendOneOne
{
	BlendEnable[ 0 ] = TRUE;
	BlendOp = ADD;
	SrcBlend = ONE;
	DestBlend = ONE;

	BlendOpAlpha = ADD;
	SrcBlendAlpha = ONE;
	DestBlendAlpha = ONE;
};

DepthStencilState depthTestDisabled
{
	DepthEnable = FALSE;
	StencilEnable = FALSE;
};

DepthStencilState depthTestEnabled
{
	DepthEnable = TRUE;
	StencilEnable = FALSE;
	DepthFunc = LESS;
	DepthWriteMask = ALL;
};

DepthStencilState depthTestEnabledWritesDisabled
{
	DepthEnable = TRUE;
	StencilEnable = FALSE;
	DepthFunc = LESS;
	DepthWriteMask = ZERO;
};

float linstep( float min, float max, float v )
{
	return saturate( ( v - min ) / ( max - min ) );
}

float cosineInterpolate( float x, float y, float t )
{
    float cosValue = 0.5 * ( 1 + cos( PI * t ) );
    float t2 = 1 - cosValue;
    return lerp( x, y, t2 );
}

float2 flipUV( float2 uv )
{
	return float2( uv.x, 1 - uv.y );
}

bool insideUnitSquare( float2 xy )
{
	return( xy.x >= 0 && xy.y >= 0 && xy.x <= 1 && xy.y <= 1 );
}

// rect is specified as:
// ( left, bottom, right, top )
bool insideRectangle( float2 xy, float4 rect )
{
	return( xy.x >= rect.x && xy.y >= rect.y && xy.x <= rect.z && xy.y <= rect.w );
}

float4 premultiplyAlpha( float4 colorIn )
{
	return float4( colorIn.rgb * colorIn.a, colorIn.a );
}

float4 unpremultiplyAlpha( float4 colorIn )
{
	return float4( colorIn.rgb / colorIn.a, colorIn.a );
}

// UV is flipped for you
float2 worldToProjectedUV( float4 world, matrix projectorPVW )
{
	// world --> clip
	float4 projectorClip = mul( projectorPVW, world );
	// homogenize: clip --> ndc
	float2 projectorUV = projectorClip.xy / projectorClip.w;
	
	// scale and bias from ndc to [0,1]
	projectorUV = 0.5 * ( projectorUV + float2( 1, 1 ) );
	
	// flip up-down because Direct3D is retarded
	return flipUV( projectorUV );
}

// Returns the angle between unit vectors u and v
// The range is [-pi,pi]
float angleBetween( float3 u, float3 v )
{
	float ct = dot( u, v );
    float st = length( cross( u, v ) );
    return atan2( st, ct );
}

bool withinFieldOfView( float3 scenePoint, matrix pvwCamera )
{
	float4 clip = mul( pvwCamera, float4( scenePoint, 1 ) );
	float3 ndc = clip.xyz / clip.w;
 
    float nx = ndc.x;
    float ny = ndc.y;
    float nz = ndc.z;

	// within field of view
	return( nx > -1 && nx < 1 && ny > -1 && ny < 1 && nz > 0 );
}

bool withinViewFrustum( float3 scenePoint, matrix pvwCamera )
{
	float4 clip = mul( pvwCamera, float4( scenePoint, 1 ) );
	float3 ndc = clip.xyz / clip.w;
 
    float nx = ndc.x;
    float ny = ndc.y;
    float nz = ndc.z;

	// within field of view
	return( nx > -1 && nx < 1 && ny > -1 && ny < 1 && nz > 0 && nz < 1 );
}