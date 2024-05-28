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


texture uNoiseTexture0;
sampler2D noise0 = sampler_state
{
    texture = <uNoiseTexture0>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};
texture uNoiseTexture1;
sampler2D noise1 = sampler_state
{
    texture = <uNoiseTexture1>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};

float2 distortSize;

float4 PixelShaderFunction(float4 baseColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float2 targetCoords = (uTargetPosition - uScreenPosition) / uScreenResolution;
    float2 center = ((coords - targetCoords)) * (uScreenResolution / uScreenResolution.y) / uZoom;
    float centerLength = length(center);

    float2 polar = float2(atan2(center.y, center.x) / 3.14 * 2 + uProgress * 3.14, pow(smoothstep(0, 1.5, centerLength) * distortSize.y, 3) * 0.3);

    float dist0 = length(tex2D(noise0, float2(polar.x * 0.5 + polar.y + uProgress * 3, polar.y + uProgress * 10)).rgb);
    float dist1 = length((tex2D(noise1, float2(polar.x * 2 + polar.y * 2 * saturate(2 - polar.y), polar.y + uProgress * 10) + dist0 * center * 0.1)).rgb) / 2;
    centerLength -= dist0 * 0.1;
    centerLength += dist1 * 0.1;
    
    float distortion = dist1 * smoothstep(0.5, 1.2, centerLength);
    float subtraction = (pow(dist1, 2) - 0.1) * smoothstep(0.5, 1.0, centerLength);
    float addition = dist1 * smoothstep(0.5, 1.0, centerLength);
    return tex2D(uImage0, coords - distortion * normalize(center) * uOpacity) + (addition - subtraction * 1.5) * uOpacity;
}

technique Technique1
{
    pass DistortionPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}