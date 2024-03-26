sampler uImage0 : register(s0);

texture uTexture0;
sampler2D tex0 = sampler_state
{
    texture = <uTexture0>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};
texture uTexture1;
sampler2D tex1 = sampler_state
{
    texture = <uTexture1>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = clamp;
    AddressV = clamp;
};
float4 uColor;
float4 uBloomColor;
float uTime;
float uNoiseThickness;
float uNoiseSize;
float uLength;
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
    float endFadeOuts = smoothstep(0.0, 0.1, input.Coord.x / uLength) * smoothstep(1.0, 0.9, input.Coord.x / uLength);
    float noise = (length(tex2D(tex0, float2(input.Coord.x / uNoiseSize - uTime, input.Coord.y / (uNoiseSize * 2) - uTime)).rgb) / 3 - 0.33) * uNoiseThickness * endFadeOuts;
    float beam = length(tex2D(tex1, float2((input.Coord.x) * uLength * 0.2, input.Coord.y + noise)).rgb) / 2;
    
    float innerLine = smoothstep(0.75, 0.85, beam);
    float bloom = smoothstep(0.05, 0.6, beam);
    return (innerLine * uColor + bloom * uBloomColor) * (0.6f + noise * 0.4f) * input.Color;
}

technique Technique1
{
    pass EnergyPass
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}