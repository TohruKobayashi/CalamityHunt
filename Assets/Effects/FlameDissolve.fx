sampler uImage0 : register(s0);
texture uTexture0;
sampler tex0 = sampler_state
{
    texture = <uTexture0>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};
float2 uTextureScale;
float uFrameCount;
float uProgress;
float uPower;
float uNoiseStrength;

float4 PixelShaderFunction(float4 baseColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 original = tex2D(uImage0, coords);
    float noiseCoord = length(tex2D(tex0, float2(coords.x * uTextureScale.x - uProgress / 2, coords.y * uTextureScale.y * uFrameCount) * 0.45).rgb) / 1.5 - 1;
    float2 movingCoords = (float2(coords.x, coords.y - uProgress * 0.1 / uFrameCount) - 0.5) * (1 - uProgress * 0.1) + 0.5;
    float4 noise = tex2D(tex0, float2(movingCoords.x * uTextureScale.x, movingCoords.y * uTextureScale.y * uFrameCount) + noiseCoord * uProgress * 0.1) * smoothstep(0.1, 0.3, original + noiseCoord * 0.3);
    float power = pow(original * (1 + noise * uNoiseStrength * (0.9 - uPower * 0.001f) - uProgress * uNoiseStrength * 0.8), uPower * (0.5 + uProgress * 0.5) * 0.5);
    return clamp(power, 0, 1) * smoothstep(0, 0.1, original) * baseColor;

}
technique Technique1
{
    pass PixelShaderPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}