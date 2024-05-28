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

texture distortionSample0;
sampler2D dSample0 = sampler_state
{
    texture = <distortionSample0>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};
texture distortionSample1;
sampler2D dSample1 = sampler_state
{
    texture = <distortionSample1>;
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

float donut(float2 coord)
{
    float mid = (outEdge + inEdge) / 2.0;
    float d = 1.0 - length(coord);
    return smoothstep(inEdge, outEdge, d) * smoothstep(outEdge, inEdge, d);
}
float4 PixelShaderFunction(float4 baseColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float2 targetCoords = (uTargetPosition - uScreenPosition) / uScreenResolution;
    float2 center = ((coords - targetCoords)) * (uScreenResolution / uScreenResolution.y) / uZoom / uSize;

    float4 dist0 = tex2D(dSample0, (coords + uScreenPosition * 0.0005 + float2(-uProgress * 2, uProgress * 4)) * distortSize);
    float4 dist1 = tex2D(dSample1, (coords * 0.5 + uScreenPosition * 0.0005 + float2(0, uProgress * 3)) * distortSize);
    float distPower = length(dist0 * dist1) / 4;
    float wave = sin((length(center) + uProgress) * 6.28 * uIntensity * distPower);
    float2 distort = center * wave * uOpacity * donut(center);
    float4 image = tex2D(uImage0, coords + distort);
    return image * (1 - clamp(length(center + distort) * 0.4 * uOpacity, 0, 0.6));

}

technique Technique1
{
    pass DistortionPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}