float4x4 xWorldViewProjection;

float4x4 WorldInverseTranspose;

float3 DiffuseLightDirection = float3(0, 1, 0);
float4 DiffuseColor = float4(1, 1, 1, 1);
float DiffuseIntensity = 0.4;


float4 AmbientColor = float4(1, 1, 1, 1);
float AmbientIntensity = 0.1;

struct VertexShaderDiffuseInput
{
	float4 Position : POSITION0;
	float4 Normal : NORMAL0;
};

struct VertexShaderDiffuseOutput
{
	float4 Position : POSITION0;
	float4 Color : COLOR0;
};

//*********************************************
struct VertexShaderAmbientInput
{
	float4 Position : POSITION0;
};

struct VertexShaderAmbientOutput
{
	float4 Position : POSITION0;
};
//*********************************************

VertexShaderDiffuseOutput VertexShaderDiffuseFunction(VertexShaderDiffuseInput input)
{
	VertexShaderDiffuseOutput output;

	output.Position = mul(input.Position, xWorldViewProjection);

	float4 normal = mul(input.Normal, WorldInverseTranspose);
	float lightIntensity = dot(normal, DiffuseLightDirection);
	output.Color = saturate(DiffuseColor * DiffuseIntensity * lightIntensity);

	return output;
}

float4 PixelShaderDiffuseFunction(VertexShaderDiffuseOutput input) : COLOR0
{
	return saturate(input.Color + AmbientColor * AmbientIntensity);
}
//*************************************************************************
VertexShaderAmbientOutput VertexShaderAmbientFunction(VertexShaderAmbientInput input)
{
	VertexShaderAmbientOutput output;

	output.Position = mul(input.Position, xWorldViewProjection);

	return output;
}

float4 PixelShaderAmbientFunction(VertexShaderAmbientOutput input) : COLOR0
{
	return AmbientColor * AmbientIntensity;
}
//***********************************************************************

technique Diffuse
{
	pass Pass1
	{
		VertexShader = compile vs_2_0 VertexShaderDiffuseFunction();
		PixelShader = compile ps_2_0 PixelShaderDiffuseFunction();
	}
}

technique Ambient
{
	pass Pass1
	{
		VertexShader = compile vs_2_0 VertexShaderAmbientFunction();
		PixelShader = compile ps_2_0 PixelShaderAmbientFunction();
	}
}