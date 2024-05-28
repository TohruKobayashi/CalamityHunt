sampler2D uImage0 : register(s0);
matrix uTransformMatrix;
float uTime;

texture uTexture0;
sampler tex0 = sampler_state
{
    texture = <uTexture0>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};
texture uTexture1;
sampler tex1 = sampler_state
{
    texture = <uTexture1>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};
texture uNoiseTexture;
sampler texNoise = sampler_state
{
    texture = <uNoiseTexture>;
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
    float endFadeOuts = smoothstep(0.0, 0.1, input.Coord.x) * smoothstep(1.0, 0.9, input.Coord.x);
    float noise = (length(tex2D(texNoise, float2(input.Coord.x - uTime, input.Coord.y / 3 - uTime) / 2).rgb) / 3 - 0.25) * endFadeOuts;
    float4 base = tex2D(tex0, float2(input.Coord.x - uTime, input.Coord.y) + noise * 0.2);
    float4 glow = tex2D(tex1, float2(input.Coord.x - uTime, input.Coord.y) + noise * 0.2);
    return (pow(base, 3) * (input.Color + 0.1) + glow * input.Color) * 2 * (length(base.rgb) / 3);
}

technique Technique1
{
    pass ShaderPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
        VertexShader = compile vs_2_0 VertexShaderFunction();
    }
}