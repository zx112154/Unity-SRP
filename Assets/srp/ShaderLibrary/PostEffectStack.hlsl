#ifndef MYRP_POST_EFFECT_STACK_INCLUDED
#define MYRP_POST_EFFECT_STACK_INCLUDED

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"

TEXTURE2D(_MainTex);
SAMPLER(sampler_MainTex);
TEXTURE2D(_DepthTex);
SAMPLER(sampler_DepthTex);

float4 _ProjectionParams;

float4 _ZBufferParams;

float _ReinhardModifier;

struct VertexInput {
	float4 pos : POSITION;
};

struct VertexOutput {
	float4 clipPos : SV_POSITION;
	float2 uv : TEXCOORD0;
};

//模糊采样
float4 BlurSample(float2 uv, float uOffset = 0.0, float vOffset = 0.0)
{
	//ddx(uv.x) 相邻像素的UV的X的差值
	uv += float2(uOffset * ddx(uv.x), vOffset * ddy(uv.y));
	return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
}

VertexOutput DefaultPassVertex(VertexInput input)
{
	VertexOutput output;
	output.clipPos = float4(input.pos.xy, 0.0, 1.0);
	output.uv = input.pos.xy * 0.5 + 0.5;
	if (_ProjectionParams.x < 0.0)
		output.uv.y = 1.0 - output.uv.y;

	return output;
}

float4 CopyPassFragment(VertexOutput input) : SV_TARGET
{
	return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
}

float4 DepthStripesPassFragment(VertexOutput input) : SV_TARGET
{
	float rawDepth = SAMPLE_DEPTH_TEXTURE(_DepthTex, sampler_DepthTex, input.uv);
//转换得到深度到近平面的坐标
float depth = LinearEyeDepth(rawDepth, _ZBufferParams);
float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);

//处理天空盒
#if UNITY_REVERSED_Z
	bool hasDepth = rawDepth != 0;
#else
	bool hasDepth = rawDepth != 1;
#endif

	if (hasDepth)
		color *= pow(sin(3.14 * depth), 2.0);

	return color;
}

float4 BlurPassFragment(VertexOutput input) : SV_TARGET
{
	float4 color =
	BlurSample(input.uv,  1,  1) +
	BlurSample(input.uv, -1,  1) +
	BlurSample(input.uv,  1, -1) +
	BlurSample(input.uv, -1, -1);

	return float4(color.rgb * 0.25, 1);
}

float4 ToneMappingPassFragment(VertexOutput input) : SV_TARGET
{
	float3 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv).rgb;
	color *= (1 + color * _ReinhardModifier) / (1 + color);
	return float4(saturate(color), 1);
}


#endif //MYRP_POST_EFFECT_STACK_INCLUDED