sampler uImage0 : register(s0);
texture uTex0;
sampler2D tex0 = sampler_state
{
    texture = <uTex0>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};

texture uTex1;
sampler2D tex1 = sampler_state
{
    texture = <uTex1>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};

texture uMap;
sampler2D map = sampler_state
{
    texture = <uMap>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};

float uTime;
float uBrightness;
float uStrength;
float2 uWorldPos;
float4 uColorBase;

float4 PixelShaderFunction(float4 base : COLOR0, float2 input : TEXCOORD0) : COLOR0
{
    float4 img1 = tex2D(tex1, input + float2(uWorldPos.x + uTime * 3, uWorldPos.y + sin((uTime + input.x) * 6.28) * 0.1));
    float4 img0 = tex2D(tex0, input * float2(0.5, 0.75) + float2(uWorldPos.x + uTime * 2, uWorldPos.y * 2 + sin((uTime + input.x) * 6.28) * 0.05 + length(img1) * 0.1f + sin(uTime * 6.28) * 0.4)) * uStrength;
    float cloud = clamp(lerp(0.1, 0.6, input.y + length(img1) * 0.2), 0, 1) * uStrength;
    float4 final = img0 * uStrength * (0.5 + img1 * cloud);
    return final * uColorBase * lerp(0.5, 2, length(final.rgb) + uStrength * 0.1);
}

technique Technique1
{
    pass PixelShaderPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}