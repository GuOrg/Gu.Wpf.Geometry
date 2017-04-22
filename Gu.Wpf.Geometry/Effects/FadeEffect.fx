/// <class>Fade</class>
/// <description>Fade to a colour by animating the strength.</description>

sampler2D inputSampler : register(S0);

/// <summary>The color used to tint the input.</summary>
/// <minValue>0</minValue>
/// <maxValue>1</maxValue>
/// <defaultValue>0</defaultValue>
float Strength : register(C0);

/// <summary>The colour to fade to.</summary>
/// <defaultValue>Black</defaultValue>
float4 To : register(C2);

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float4 src = tex2D(inputSampler, uv);
    return lerp(src, To, Strength);
}
