sampler uImage0 : register(s0);

texture uCutTexture;
sampler2D cutTexture = sampler_state
{
    texture = <uCutTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};

texture uNoise;
sampler2D noiseTexture = sampler_state
{
    texture = <uNoise>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};

float2 uImageSize;
float4 uColor;
float4 uSecondaryColor;
float uTime;
float uDistortionStrength;
float2 uScreenPosition;
float uDistance;
float2 uCenter;

bool diffInner(float4 image, float2 coords)
{
    float4 left = tex2D(uImage0, coords + float2(0, 2) / uImageSize);
    float4 right = tex2D(uImage0, coords + float2(0, -2) / uImageSize);
    float4 up = tex2D(uImage0, coords + float2(2, 0) / uImageSize);
    float4 down = tex2D(uImage0, coords + float2(-2, 0) / uImageSize);
    
    if (image.a > left.a || image.a > right.a || image.a > up.a || image.a > down.a)
        return true;
    else
        return false;
}

bool diffOuter(float4 image, float2 coords)
{
    float4 left = tex2D(uImage0, coords + float2(0, 2) / uImageSize);
    float4 right = tex2D(uImage0, coords + float2(0, -2) / uImageSize);
    float4 up = tex2D(uImage0, coords + float2(2, 0) / uImageSize);
    float4 down = tex2D(uImage0, coords + float2(-2, 0) / uImageSize);
    
    if (image.a < left.a || image.a < right.a || image.a < up.a || image.a < down.a)
        return true;
    else
        return false;
}

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float distance = length(coords - uCenter);
    float4 noise = tex2D(noiseTexture, (coords + uScreenPosition) * float2(1, uImageSize.y / uImageSize.x) + float2(frac(-uTime), frac(-uTime)));
    float4 noise2 = tex2D(noiseTexture, (coords + uScreenPosition) * float2(1, uImageSize.y / uImageSize.x) + float2(frac(uTime + 0.5), frac(-uTime)));
    float2 offset = length(noise.rgb * noise2.rgb) / uImageSize * uDistortionStrength * smoothstep(uDistance - 0.3, uDistance + 0.3, distance) * -normalize((coords * 2 - 1));
    float2 offsetRounded = round(offset * uImageSize.y / 2) / (uImageSize.y / 2);
    float4 image = tex2D(uImage0, coords + offsetRounded);
    
    float edges = diffInner(image, coords + offsetRounded);
    float edgesOuter = diffOuter(image, coords + offsetRounded);
    
    float4 totalEdge = edges * uSecondaryColor * (1 - distance) + edgesOuter * lerp(uColor, uSecondaryColor, smoothstep(uDistance - 0.1, uDistance + 0.2, distance));

    return totalEdge;
}

technique Technique1
{
    pass ShaderPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}