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

float4 PixelShaderFunction(float4 base : COLOR0, float2 input : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, input);
    
    // get nosemap. we get it a little smaller than it usually is, so you can see the cysts better
    // we also set diff time offsets for both axis' so you can see different patterns over time
    float2 noiseCoords = (input * uImageSize0 - uSourceRect.xy) / noisemapSize * 4;
    float2 noiseCoordsWithTime = noiseCoords + uTime * 0.05;
    noiseCoordsWithTime.x = noiseCoords + uTime * 0.03;
    float4 noise = tex2D(noisemapSampler, noiseCoordsWithTime);
    
    // get the noisemaps luminosity, and adjust that to look nicer
    float noiseLuminosity = (noise.r + noise.g + noise.b) / 3;
    noiseLuminosity = pow(noiseLuminosity, 1.5);
    noiseLuminosity = clamp(noiseLuminosity + 0.2, 0.33, 1);
    
    // add color
    float colorLuminosity = (color.r + color.g + color.b) / 3;
    color.rgb = colorLuminosity * noiseLuminosity * uColor;
    
    // hueshift the lowlights of the noise towards blue
    float noiseLuminosityInverted = 1 - noiseLuminosity + 1;
    color.b *= clamp(noiseLuminosityInverted - 0.3, 1, 22);
    
    return color * base;

}

technique Technique1
{
    pass DyePass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}