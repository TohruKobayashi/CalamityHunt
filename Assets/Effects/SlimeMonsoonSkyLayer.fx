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
float uHeight;

float4 ColorMap(float x, float y)
{
    return tex2D(map, float2(clamp(x, 0.001, 0.99), y));
}

float4 PixelShaderFunction(float4 base : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float img0 = tex2D(tex0, coords + float2(uWorldPos.x + uTime, sin((uTime * 2 + coords.x) * 6.28 + uWorldPos.x) * 0.1));
    float img1 = tex2D(tex1, coords + float2(uWorldPos.x + uTime * 3, uWorldPos.y + sin((uTime + coords.x) * 6.28 + uWorldPos.x) * 0.05) + (img0 - 0.5) * 0.1);
    
    float cloud = smoothstep(-1, uHeight, coords.y - img0 * 0.4 + sin((uTime * 2 + coords.x) * 6.28 + uWorldPos.x) * 0.1) * smoothstep(uHeight, 1, (1 - coords.y - img1 * 0.6 - sin((uTime * 2 + coords.x) * 6.28 + uWorldPos.x) * 0.2));
    return ColorMap(cloud * uBrightness, 0.5) * smoothstep(0, 0.15, cloud) * base;
}

technique Technique1
{
    pass PixelShaderPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}