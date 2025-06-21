sampler uImage0 : register(s0);
float3 uColor;
float2 uScreenResolution;
bool horizontal = true;
float uOpacity;
float uIntensity;

float4 Blur(float2 coords : TEXCOORD0) : COLOR0
{
    float2 txSize = 1 / uScreenResolution;
    float weights[5] = { 0.06136f, 0.24477f, 0.38774f, 0.24477f, 0.06136f }; 
    float4 result = tex2D(uImage0, coords.xy) * weights[0];

    if (horizontal)
    {
        for (int i = 1; i < 5; i++)
        {
            float offset = txSize.x * i * uIntensity;
            result += tex2D(uImage0, coords + float2(txSize.x * i, 0)) * weights[i];
            result += tex2D(uImage0, coords - float2(txSize.x * i, 0)) * weights[i];
        }
    }
    else
    {
        for (int i = 1; i < 5; i++)
        {
            float offset = txSize.x * i * uIntensity;
            result += tex2D(uImage0, coords + float2(0, txSize.y * i)) * weights[i];
            result += tex2D(uImage0, coords - float2(0, txSize.y * i)) * weights[i];
        }
    }
    return float4(result.rgb, result.a * uOpacity);
}
technique Technique1
{
    pass BlurPass
    {
        PixelShader = compile ps_2_0 Blur();
    }
}