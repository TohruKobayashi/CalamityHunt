sampler uImage0 : register(s0);

texture uTexture0;
sampler2D a1 = sampler_state
{
    texture = <uTexture0>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};
texture uTexture1;
sampler2D a2 = sampler_state
{
    texture = <uTexture1>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};
float4 uColor;
float uTime;
matrix transformMatrix;

struct VertexShaderInput
{
    float2 Coord : TEXCOORD0;
    float4 Position : POSITION0;
    float4 Color : COLOR0;
};

struct VertexShaderOutput
{
    float2 Coord : TEXCOORD0;
    float4 Position : POSITION0;
    float4 Color : COLOR0;
};

VertexShaderOutput VertexShaderFunction(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;
    output.Color = input.Color;
    output.Coord = input.Coord;
    output.Position = mul(input.Position, transformMatrix);
    return output;
}

float4 PixelShaderFunction(in VertexShaderOutput input) : COLOR0
{
    float4 n1 = tex2D(a1, input.Coord + float2(-uTime, 0)) + tex2D(a2, input.Coord + float2(-uTime * 2, 0));
    return n1 * uColor;

}

technique Technique1
{
    pass EnergyPass
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}