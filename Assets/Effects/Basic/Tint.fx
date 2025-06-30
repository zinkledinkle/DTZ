sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float3 uColor;
float uIntensity;
bool UVWarp = false;

float4 Tint(float2 coords : TEXCOORD0) : COLOR0
{
    if (UVWarp)
        coords += tex2D(uImage1, coords).r * uIntensity / 100;
    float4 color = tex2D(uImage0, coords);
    color.rgb += uColor * uIntensity;
    return color;
}

technique Technique1
{
    pass TintPass
    {
        PixelShader = compile ps_2_0 Tint();
    }
}