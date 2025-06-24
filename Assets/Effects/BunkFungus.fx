sampler uImage0 : register(s0);
sampler uImage1 : register(s1); // Automatically Images/Misc/Perlin via Force Shader testing option
sampler uImage2 : register(s2); // Automatically Images/Misc/noise via Force Shader testing option
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

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float2 center = float2(0.5f, 0.5f);
    
    float2 warpyCoords = coords - tex2D(uImage1, coords + float2(uTime / 100, uTime / 40)).rb / 50 * uIntensity;
    
    float2 uv = warpyCoords - center;

    int segments = 6;
    float angle = atan2(uv.y, uv.x);
    float radius = length(uv);

    angle += uTime / 100;
    
    float segmentAngle = 2 * 3.14159265f / segments;
    angle = fmod(angle, segmentAngle);
    if (angle < 0)
        angle += segmentAngle;
    if (fmod(floor((atan2(uv.y, uv.x) / segmentAngle)), 2) == 1)
        angle = segmentAngle - angle;
    float2 kaleidoUV = float2(cos(angle), sin(angle)) * radius + center;
    
    float4 kaleido = tex2D(uImage0, kaleidoUV);
    
    float4 color = tex2D(uImage0, warpyCoords);
    return color + (kaleido * uIntensity);
}

technique Technique1
{
    pass fungus
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}