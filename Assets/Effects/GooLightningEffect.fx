sampler2D uImage0 : register(s0);
float4 uColor;
matrix uTransformMatrix;
float uTime;
float uBackPower;

texture uTexture;
sampler tex = sampler_state
{
    texture = <uTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};
texture uGlow;
sampler glow = sampler_state
{
    texture = <uGlow>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};

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
    VertexShaderOutput output = (VertexShaderOutput) 0;
    output.Color = input.Color;
    output.Coord = input.Coord;
    output.Position = mul(input.Position, uTransformMatrix);
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float4 base = tex2D(tex, input.Coord + float2(frac(uTime * 2), 0)) * tex2D(tex, input.Coord + float2(frac(uTime) + 0.5, 0)) * float4(1, 1, 1, 0);
    float4 baseGlow = tex2D(glow, input.Coord + float2(frac(uTime), 0)) * float4(1, 1, 1, 0);
    float4 back = length(baseGlow + base * 2) * float4(input.Color.rgb * 0.1, 0.2);
    return back * uBackPower + (pow(base * 2 + baseGlow, 2) + baseGlow) * input.Color;
}

technique Technique1
{
    pass ShaderPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
        VertexShader = compile vs_2_0 VertexShaderFunction();
    }
}