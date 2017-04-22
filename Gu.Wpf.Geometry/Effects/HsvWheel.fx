static float Pi = 3.141592f;
static float2 cp = float2(0.5, 0.5);
static float2 xv = float2(1, 0);

/// <summary>The inner radius.</summary>
/// <minValue>0</minValue>
/// <maxValue>1</maxValue>
/// <defaultValue>0</defaultValue>
float InnerRadius : register(C0);

/// <summary>The inner saturation.</summary>
/// <minValue>0</minValue>
/// <maxValue>1</maxValue>
/// <defaultValue>0</defaultValue>
float InnerSaturation : register(C1);

/// <summary>The value.</summary>
/// <minValue>0</minValue>
/// <maxValue>1</maxValue>
/// <defaultValue>1</defaultValue>
float Value : register(C2);

float angle(in float2 v1, in float2 v2, in bool clockWise)
{
    int sign = clockWise ? -1 : 1;
    float a1 = atan2(v1.y, v1.x) * sign;
    float a2 = atan2(v2.y, v2.x) * sign;
    float a = a2 - a1;
    if (a < 0)
    {
        return 2 * Pi + a;;
    }

    return a;
}

float3 HUEtoRGB(in float H)
{
    float R = abs(H * 6 - 3) - 1;
    float G = 2 - abs(H * 6 - 2);
    float B = 2 - abs(H * 6 - 4);
    return saturate(float3(R, G, B));
}

// http://www.chilliant.com/rgb2hsv.html
float3 HSVtoRGB(in float3 HSV)
{
    float3 RGB = HUEtoRGB(HSV.x);
    return ((RGB - 1) * HSV.y + 1) * HSV.z;
}

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float2 rv = uv - cp;
    float r = length(rv);
    float ir = InnerRadius / 2;
    if (r >= ir && r <= 0.5)
    {
        float h = angle(rv, xv, false) / (2 * Pi);
        float s = lerp(InnerSaturation, 1, smoothstep(ir, 0.5, r));
        float v = Value;
        return float4(HSVtoRGB(float3(h, s, v)), 1);
    }

    return float4(0, 0, 0, 0);
}