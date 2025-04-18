sampler uImage0 : register(s0);

float2 uSize;

float2 normalize_with_pixelation(float2 coords, float pixel_size, float2 resolution)
{
    return floor(coords / pixel_size) / (resolution / pixel_size);
}

float4 main(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    coords = normalize_with_pixelation(coords * uSize, 2.0f, uSize);

    float4 color = tex2D(uImage0, coords);
    return color; // we could quantize too
}

technique Technique1
{
    pass IckyHand
    {
        PixelShader = compile ps_3_0 main();
    }
}