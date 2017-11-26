static float PI = 3.14159274f;
static float PI2 = 6.28318548f;

float4 lerp_rgba(float4 x, float4 y, float s)
{
    float a = lerp(x.a, y.a, s);
    float3 rgb = lerp(x.rgb, y.rgb, s) * a;
    return float4(rgb.r, rgb.g, rgb.b, a);
}

float interpolate(float min, float max, float value)
{
    if (min == max)
    {
        return 0.5;
    }

    if (min < max)
    {
        return clamp((value - min) / (max - min), 0, 1);
    }

    return clamp((value - max) / (min - max), 0, 1);
}


float clamp_angle_positive(float a)
{
    if (a < 0)
    {
        return a + PI2;
    }

    return a;
}

float clamp_angle_negative(float a)
{
    if (a > 0)
    {
        return a - PI2;
    }

    return a;
}

float angle_from_start(float2 uv, float2 center_point, float start_angle, float central_angle)
{
    float2 v = uv - center_point;
    return central_angle > 0
        ? clamp_angle_positive(clamp_angle_positive(atan2(v.x, -v.y)) - clamp_angle_positive(start_angle))
        : abs(clamp_angle_negative(clamp_angle_negative(atan2(v.x, -v.y)) - clamp_angle_negative(start_angle)));
}
