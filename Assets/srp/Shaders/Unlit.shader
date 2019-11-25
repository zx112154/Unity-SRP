Shader "MyPipeline/Unlit"
{
	Properties
	{
		_Color("Color", Color) = (1, 1, 1, 1)
	}
	SubShader
	{
		Pass
		{
			HLSLPROGRAM
			#include "../ShaderLibrary/Unlit.hlsl"
			#pragma target 3.5
			#pragma multi_compile_instancing
			#pragma instancing_options assumeuniformscaling
			#pragma vertex UnlitPassVertex
			#pragma fragment UnlitPassFragment
			ENDHLSL
		}
	}
}
