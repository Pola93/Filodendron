float4x4 xWorldViewProjection;

//for Diffuse
float4x4 WorldInverseTranspose;
float3 DiffuseLightDirection = float3(0, 1, 0);
float4 DiffuseColor = float4(1, 1, 1, 1);
float DiffuseIntensity = 0.5;

//for Ambient
float4 AmbientColor = float4(1, 1, 1, 1);
float AmbientIntensity = 0.1;

//for Specular
float4x4 World;
float Shininess = 200; // Most metallic surfaces have a value between about 100 and 500
float4 SpecularColor = float4(1, 1, 1, 1);
float SpecularIntensity = 1;
float3 ViewVector = float3(0, 1, 0); //  indicates the direction that the camera or "eye" is looking in

texture ModelTexture;
sampler2D textureSampler = sampler_state {
	Texture = (ModelTexture);
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = Clamp;
	AddressV = Clamp;
};

//************Specular***************
struct VertexShaderSpecularInput // the same as struct for diffuse vertex shader input
{
	float4 Position : POSITION0;
	float4 Normal : NORMAL0;
	float2 TextureCoordinate : TEXCOORD0;
};

struct VertexShaderSpecularOutput
{
	float4 Position : POSITION0;
	float4 Color : COLOR0;
	float3 Normal : TEXCOORD0;
	float2 TextureCoordinate : TEXCOORD1;
};

//************Diffuse************
struct VertexShaderDiffuseInput
{
	float4 Position : POSITION0;
	float4 Normal : NORMAL0;
	float2 TextureCoordinate : TEXCOORD0;
};

struct VertexShaderDiffuseOutput
{
	float4 Position : POSITION0;
	float4 Color : COLOR0;
	float2 TextureCoordinate : TEXCOORD0;
};

//*************Ambient********************
struct VertexShaderAmbientInput
{
	float4 Position : POSITION0;
	float2 TextureCoordinate : TEXCOORD0;
};

struct VertexShaderAmbientOutput
{
	float4 Position : POSITION0;
	float2 TextureCoordinate : TEXCOORD0;
};

//************Specular******************
VertexShaderSpecularOutput VertexShaderSpecularFunction(VertexShaderSpecularInput input)
{
	VertexShaderSpecularOutput output;

	output.Position = mul(input.Position, xWorldViewProjection);

	float4 normal = mul(input.Normal, WorldInverseTranspose);
	float lightIntensity = dot(normal, DiffuseLightDirection);
	output.Color = saturate(DiffuseColor * DiffuseIntensity * lightIntensity);

	output.Normal = normal;
	output.TextureCoordinate = input.TextureCoordinate;

	return output;
}

float4 PixelShaderSpecularFunction(VertexShaderSpecularOutput input) : COLOR0
{
	float3 light = normalize(DiffuseLightDirection); // inefficient to normalize here
	float3 normal = normalize(input.Normal);
	float3 r = normalize(2 * dot(light, normal) * normal - light); // reflection vector
	float3 v = normalize(mul(normalize(ViewVector), World)); // view vector

	float dotProduct = dot(r, v);
	float4 specular = SpecularIntensity * SpecularColor * max(pow(dotProduct, Shininess), 0) * length(input.Color);

	float4 textureColor = tex2D(textureSampler, input.TextureCoordinate);
	textureColor.a = 1; // set the alpha channel

	return saturate(textureColor * (input.Color) + AmbientColor * AmbientIntensity + specular);
}

//*************Diffuse*******************
VertexShaderDiffuseOutput VertexShaderDiffuseFunction(VertexShaderDiffuseInput input)
{
	VertexShaderDiffuseOutput output;

	output.Position = mul(input.Position, xWorldViewProjection);

	float4 normal = mul(input.Normal, WorldInverseTranspose);
	float lightIntensity = dot(normal, DiffuseLightDirection);
	output.Color = saturate(DiffuseColor * DiffuseIntensity * lightIntensity);

	output.TextureCoordinate = input.TextureCoordinate;

	return output;
}

float4 PixelShaderDiffuseFunction(VertexShaderDiffuseOutput input) : COLOR0
{
	float4 textureColor = tex2D(textureSampler, input.TextureCoordinate);
	textureColor.a = 1; // set the alpha channel

	return saturate(textureColor * (input.Color) + AmbientColor * AmbientIntensity);
}

//***************Ambient*******************************************
VertexShaderAmbientOutput VertexShaderAmbientFunction(VertexShaderAmbientInput input)
{
	VertexShaderAmbientOutput output;

	output.Position = mul(input.Position, xWorldViewProjection);

	output.TextureCoordinate = input.TextureCoordinate;

	return output;
}

float4 PixelShaderAmbientFunction(VertexShaderAmbientOutput input) : COLOR0
{
	float4 textureColor = tex2D(textureSampler, input.TextureCoordinate);
	textureColor.a = 1; // set the alpha channel

	return textureColor + AmbientColor * AmbientIntensity;
}

//***************Techniques***************************************
technique Specular
{
	pass Pass1
	{
		VertexShader = compile vs_2_0 VertexShaderSpecularFunction();
		PixelShader = compile ps_2_0 PixelShaderSpecularFunction();
	}
}

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