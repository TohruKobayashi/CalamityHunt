sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float3 uColor;
float3 uSecondaryColor;
float uOpacity;
float2 uTargetPosition;
float uSaturation;
float uRotation;
float uTime;
float4 uSourceRect;
float2 uWorldPosition;
float uDirection;
float3 uLightSource;
float2 uImageSize0;
float2 uImageSize1;
float4 uLegacyArmorSourceRect;
float2 uLegacyArmorSheetSize;

texture noisemap;
float2 noisemapSize;
sampler2D noisemapSampler = sampler_state
{
    texture = <noisemap>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    
    // add the noisemap layer
    // we want a wavy effect; those are the "gouges"
    // we also want it to account for terrarias pixel ratio;
    float2 frameCoords = (coords * uImageSize0 - uSourceRect.xy) / uSourceRect.zw * 0.2;
    float2 noiseCoordsWithTime = frameCoords + float2(uTime * -0.03, uTime * -0.05);
    
    float4 noise = tex2D(noisemapSampler, noiseCoordsWithTime);
    
    // get the noisemaps luminosity and the color itll be when applied
    float noiseLuminosity = (noise.r + noise.g + noise.b) / 3;
    float3 colorBright = uColor * 4;
    
    // put it all together, with color of course
    color.rgb *= uColor;
    if (noiseLuminosity > 0.4)
    {
        color.rgb *= noiseLuminosity * colorBright;
    }
    
    return color * sampleColor * color.a;
}

technique Technique1
{
    pass DyePass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}