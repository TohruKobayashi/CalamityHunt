sampler2D uImage0 : register(s0);
texture uBaseTexture;
sampler tex0 = sampler_state
{
    texture = <uBaseTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};
texture uGlowTexture;
sampler tex1 = sampler_state
{
    texture = <uGlowTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};
float2 uPosition;
float4 uBaseColor;
float uTime;
float2 uSize;
float2 uScreenSize;

float4 PixelShaderFunction(float4 baseColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 screen = tex2D(uImage0, coords);
    
    float2 offsetCoords = (coords + uPosition / uScreenSize) * (uScreenSize / uScreenSize.y);
    float4 baseTexture = tex2D(tex0, float2(frac(offsetCoords.x - sin((offsetCoords.y + uTime) * 80 + uTime * 6.28) * 0.015), frac(offsetCoords.y - uTime * 5) - sin((offsetCoords.x * 0.5 + uTime) * 40 + uTime * 6.28) * 0.013) * uSize);
    float4 glowTexture = tex2D(tex1, float2(frac(offsetCoords.x - sin((offsetCoords.y + uTime) * 80 + uTime * 6.28) * 0.015), frac(offsetCoords.y - uTime * 5) - sin((offsetCoords.x * 0.5 + uTime) * 40 + uTime * 6.28) * 0.013) * uSize);

    float pixel = baseTexture * smoothstep(0.5, 1, screen.a);
    float4 color = float4(pixel, pixel, pixel, 1) * (length(screen) > 0 ? 1 : 0);

    return color * (float4(screen.rgb, 1) + float4(glowTexture.rgb, 0) * pow(length(screen.rgb) * 2, 2));

}

technique Technique1
{
    pass GelPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}