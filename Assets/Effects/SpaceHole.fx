sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);
sampler uImage3 : register(s3);
float3 uColor;
float3 uSecondaryColor;
float2 uScreenResolution;
float2 uScreenPosition;
float2 uTargetPosition;
float2 uDirection;
float uOpacity;
float uTime;
float uIntensity;
float uProgress;
float2 uImageSize1;
float2 uImageSize2;
float2 uImageSize3;
float2 uImageOffset;
float uSaturation;
float4 uSourceRect;
float2 uZoom;

texture distortionSample;
sampler2D distortTex = sampler_state
{
    texture = <distortionSample>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};
float2 distortSize;

float inEdge;
float outEdge;
float2 uSize;

float4 PixelShaderFunction(float4 baseColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float2 targetCoords = (uTargetPosition - uScreenPosition) / uScreenResolution;
    float2 center = ((coords - targetCoords)) * (uScreenResolution / uScreenResolution.y) / uZoom / uSize;
    float2 polar = float2(atan2(center.x, center.y) / 6.28, length(center) * 0.6);
    float4 distort = tex2D(distortTex, polar + float2(0, uProgress)) * (smoothstep(1.1 * uIntensity, 0, length(center))) * length(center);
    return tex2D(uImage0, coords + length(distort) * center - center * (smoothstep(0.8 * uIntensity, 0, length(center)))) * (smoothstep(0.25 * uIntensity, 0.4 * uIntensity, length(center) - length(distort)));

}

technique Technique1
{
    pass BlackHolePass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}