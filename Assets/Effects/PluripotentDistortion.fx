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
    
    float2 polar = float2(atan2(center.y, center.x) / (3.1415), sqrt(centerLength) * 0.05 * distortSize.y);

    float dist0 = length(tex2D(noise0, polar * 0.5 + float2(sin(polar.y * 6.28) * saturate(2 - polar.y) * 0.3, uProgress)).rgb);
    float dist1 = length((tex2D(noise1, float2(polar.x * 2, polar.y) + float2(sin(polar.y * 6.28 + 0.7) * 0.5 * saturate(2 - polar.y), uProgress) + dist0 * 0.05) * pow(dist0, 1.5)).rgb);
    centerLength -= dist0 * 0.1;
    centerLength += dist1 * 0.1;

    float distortion = pow(dist1, 2) * centerLength * smoothstep(1.5, 0.9, centerLength) * 0.7;
    float subtraction = (pow(dist1, 2) - 0.1) * smoothstep(0.05, 0.3, centerLength) * smoothstep(3, 0.9, centerLength + dist0 * 0.4);
    float addition = dist1 * smoothstep(0.05, 0.3, centerLength) * smoothstep(3, 0.9, centerLength);
    return tex2D(uImage0, coords - distortion * normalize(center) * smoothstep(0.1, 0.2, center) * uOpacity) + (addition * 0.5 - subtraction) * uOpacity;

}

technique Technique1
{
    pass DistortionPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}