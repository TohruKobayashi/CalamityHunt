sampler2D uImage0 : register(s0);
float4 uColor;
float4 uInnerColor;
matrix uTransformMatrix;
texture uTexture;
texture uTexture2;
float uTime;
sampler2D tex1 = sampler_state
{
    texture = <uTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};
sampler2D tex2 = sampler_state
{
    texture = <uTexture2>;
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
    float4 trail = tex2D(tex1, float2(frac(input.Coord.x * 1.5 + uTime), input.Coord.y));
    float4 trail2 = tex2D(tex2, float2(frac(input.Coord.x * 1.5 + uTime), input.Coord.y)) * (tex2D(tex2, float2(frac(input.Coord.x * 1.5 + uTime * 2), input.Coord.y)) + trail);
    
    float innerPower = length((trail2 + trail) * (1 - input.Coord.x)) / 3 + smoothstep(0.4, 0.5, sin(input.Coord.y * 3.14));
    if (length(trail.rgb + trail2.rgb) > 0.2 + input.Coord.x * 1.5)
        return input.Color + (uInnerColor * innerPower);
    
    return (trail + trail2) * uColor;
}

technique Technique1
{
    pass ShaderPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
        VertexShader = compile vs_2_0 VertexShaderFunction();
    }
}