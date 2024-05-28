sampler2D uImage0 : register(s0);
texture uTextureNoise;
sampler texN = sampler_state
{
    texture = <uTextureNoise>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};
texture uTextureClose;
sampler texC = sampler_state
{
    texture = <uTextureClose>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};
texture uTextureFar;
sampler texF = sampler_state
{
    texture = <uTextureFar>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};
float uTime;
float2 uPosition;
float2 uScrollClose;
float2 uScrollFar;
float4 uCloseColor;
float4 uFarColor;
float4 uOutlineColor;
float2 uImageSize;
float uNoiseRepeats;
float uZoom;

float2 resize(float2 coords, float2 offset)
{
    return ((coords * uImageSize) + offset) / uImageSize;
}

float EdgeColor(float4 screen, float2 coords)
{
    float edges[8];
    
    float imageRatio = uImageSize.x / uImageSize.y;

    const float distance = 2;
    edges[0] = length(tex2D(uImage0, resize(coords, float2(distance, 0))));
    edges[1] = length(tex2D(uImage0, resize(coords, float2(0, distance))));
    edges[2] = length(tex2D(uImage0, resize(coords, float2(-distance, 0))));
    edges[3] = length(tex2D(uImage0, resize(coords, float2(0, -distance))));
    
    return smoothstep(0, 0.015, edges[0] + edges[1] + edges[2] + edges[3]) * smoothstep(0.01, -0.01, length(screen.rgba));
}

float4 PixelShaderFunction(float4 baseColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{  
    float imageRatio = uImageSize.x / uImageSize.y;
    float2 center = (coords - 0.5) * imageRatio / uZoom;
    
    float edgeNoiseX = length(tex2D(texN, uPosition * 1.5 + uScrollFar + float2(center.x * 0.4 * imageRatio, frac(center.y * 0.4 + uTime)) * 2 / uZoom).rgb) - 0.5;
    float edgeNoiseY = length(tex2D(texN, uPosition * 1.5 + uScrollClose + float2(center.x * 0.3 * imageRatio, frac(center.y * 0.3 + uTime)) / uZoom).rgb) - 0.5;
    float2 coordDistortion = float2(edgeNoiseX * 0.02, edgeNoiseY * 0.02 + 0.003) * uZoom;

    float4 screen = tex2D(uImage0, coords - coordDistortion * 0.1);

    float noise = length(tex2D(texN, uPosition * 0.025 + uScrollFar + float2(center.x * imageRatio, center.y) * uNoiseRepeats).rgb) / 2 * normalize(center);
    float noise2 = length(tex2D(texN, uPosition * 0.05 + uScrollClose + float2(center.x * imageRatio, center.y - uTime * 2) * uNoiseRepeats + noise * 0.4).rgb) / 2 * normalize(center);
    
    float spaceFar = length(tex2D(texF, uPosition * 0.05 + uScrollFar + float2(center.x * imageRatio, center.y) + noise2 * 0.8).rgb) / 1.5;
    float spaceMiddle = length(tex2D(texC, uPosition * 0.2 + uScrollClose + float2(center.x * imageRatio, center.y) + noise2 * 0.5).rgb) / 1.5;
    float spaceClose = length(tex2D(texC, uPosition * 0.3 + uScrollClose * 2 + float2(center.x * imageRatio, center.y) * 0.5 + noise2 * 0.2).rgb) / 1.5;
    
    float4 distortedScreen = tex2D(uImage0, coords + coordDistortion * 0.2);
    float4 edge = EdgeColor(distortedScreen, coords + coordDistortion * 0.5) * uOutlineColor;
    float4 result = float4(((spaceFar + spaceMiddle + spaceClose * 2) * uFarColor + pow(spaceMiddle, 1.66) * lerp(uFarColor, uCloseColor, 0.1) * 5 + pow(spaceClose, 3) * uCloseColor).rgb, 1);
    
    return result * smoothstep(0, 0.0001, length(screen.rgba)) + edge;
}

technique Technique1
{
    pass PixelShaderPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}