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

float uSize;
float uAngle;
float uPower;

float2 RotatedBy(float2 coords, float theta)
{
    float s = sin(theta);
    float c = cos(theta);
    return float2(coords.x * c - coords.y * s, coords.x * s + coords.y * c);
}

float4 PixelShaderFunction(float4 baseColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float2 targetCoords = (uTargetPosition - uScreenPosition) / uScreenResolution;
    float2 targetCenter = ((coords - targetCoords) * (uScreenResolution / uScreenResolution.y)) / uZoom;
    float totalPower = uIntensity * uPower;
    
    float distanceToTarget = length(targetCenter);
    float angle = uAngle * totalPower * exp(-distanceToTarget / uSize * 1.9) * smoothstep(1.5, -0.1, distanceToTarget);
    float2 rotatedCoords = RotatedBy(coords - 0.5, angle) + 0.5;
    
    float4 color = float4(lerp(uColor, 1, smoothstep(0.3, 0.27, distanceToTarget + 0.8 - uIntensity * 0.85)), 1) * smoothstep(0.45, 0.28, distanceToTarget + 0.55 - uIntensity * 0.6);
    
    return (tex2D(uImage0, rotatedCoords) - smoothstep(uSize + 0.2, uSize - 0.2, distanceToTarget) * 2 * totalPower + color) * clamp(1 - smoothstep(1, 2, 1 - distanceToTarget / 2) * totalPower, 0, 1);

}

technique Technique1
{
    pass BlackHolePass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}