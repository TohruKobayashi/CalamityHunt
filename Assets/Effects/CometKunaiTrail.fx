sampler2D uImage0 : register(s0);
float4 uColor;
matrix uTransformMatrix;
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
texture uTextureNoise0;
sampler2D texNoise0 = sampler_state
{
    texture = <uTextureNoise0>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};
texture uTextureNoise1;
sampler2D texNoise1 = sampler_state
{
    texture = <uTextureNoise1>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};
float uTime;

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
    float trailNoise0 = length(tex2D(texNoise0, float2(frac(input.Coord.x + uTime), input.Coord.y + uTime)).rgb) / 1.5 - 0.33;
    float trailNoise1 = length(tex2D(texNoise1, float2(frac(input.Coord.x + uTime), input.Coord.y - uTime * 2 - trailNoise0 * 0.2)).rgb) / 1.5 - 1;

    float4 trail = tex2D(tex0, float2(frac(input.Coord.x * 1.5 + uTime), input.Coord.y + trailNoise0 * 0.2));
    
    return smoothstep(0.5, 0.52, length(trail.rgb * (1 - input.Coord.x))) * 2 + smoothstep(0, 0.2, pow(trail, 2)) * uColor * 1.5;
}

technique Technique1
{
    pass ShaderPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
        VertexShader = compile vs_3_0 VertexShaderFunction();
    }
}