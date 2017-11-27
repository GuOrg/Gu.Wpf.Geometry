#include <shared.hlsli>
static float2 cp = float2(0.5, 0.5);

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

/// <summary>The value in Hue, Saturation, Value.</summary>
/// <minValue>0</minValue>
/// <maxValue>1</maxValue>
/// <defaultValue>0.5</defaultValue>
float Lightness : register(C2);

/// <summary>The starting angle of the gradient, clockwise from X-axis</summary>
/// <minValue>-360</minValue>
/// <maxValue>360</maxValue>
/// <defaultValue>90</defaultValue>
float StartAngle : register(C3);

/// <summary>The central angle of the gradient, positive value for clockwise.</summary>
/// <minValue>-360</minValue>
/// <maxValue>360</maxValue>
/// <defaultValue>-360</defaultValue>
float CentralAngle : register(C4);

float3 HUEtoRGB(in float H)
{
    float R = abs(H * 6 - 3) - 1;
    float G = 2 - abs(H * 6 - 2);
    float B = 2 - abs(H * 6 - 4);
    return saturate(float3(R, G, B));
}

// http://www.chilliant.com/rgb2hsv.html
float3 HSLtoRGB(in float3 HSL)
{
    float3 RGB = HUEtoRGB(HSL.x);
    float C = (1 - abs(2 * HSL.z - 1)) * HSL.y;
    return (RGB - 0.5) * C + HSL.z;
}

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float2 rv = uv - cp;
    float r = length(rv);
    float ir = InnerRadius / 2;
    if (r >= ir && r <= 0.5)
    {
        float sa = radians(StartAngle);
        float ca = radians(CentralAngle);
        float h = interpolate(
            0,
            abs(ca),
            angle_from_start(uv, cp, sa, ca));
        float s = lerp(InnerSaturation, 1, interpolate(ir, 0.5, r));
        float l = Lightness;
        return float4(HSLtoRGB(float3(h, s, l)), 1);
    }

    return float4(0, 0, 0, 0);
}