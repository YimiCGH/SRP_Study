Shader "My Pipeline/Lit"
{
    Properties
    {
       _Color("Color",Color) = (1,1,1,1)
       _Cutoff ("Alpha Cutoff", Range(0.0, 1.0)) = 0.5
       [Toggle(_CLIPPING)] _Clipping ("Alpha Clipping", Float) = 0
       [Toggle(_PREMULTIPLY_ALPHA)] _PremulAlpha ("Premultiply Alpha", Float) = 0

       _Metallic ("Metallic", Range(0, 1)) = 0 //1表示完全金属，0表示完全绝缘
	   _Smoothness ("Smoothness", Range(0, 1)) = 0.5 // 1表示完全光滑的，0表示完全粗糙的
       _BaseMap("Texture", 2D) = "white" {}
       [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("Src Blend", Float) = 1
	   [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("Dst Blend", Float) = 0
       [Enum(Off, 0, On, 1)] _ZWrite ("Z Write", Float) = 1
       
    }
    SubShader
    {        
        Pass
        {
            Tags {
				"LightMode" = "CustomLit"
			}

            Blend [_SrcBlend] [_DstBlend]
            ZWrite [_ZWrite]
            HLSLPROGRAM

            #pragma target 3.5

            #pragma multi_compile_instancing
            #pragma shader_feature _CLIPPING
            #pragma shader_feature _PREMULTIPLY_ALPHA

            #pragma instancing_options assumeuniformscaling
            #pragma vertex LitPassVertex
            #pragma fragment LitPassFragment

            #include "../ShaderLibrary/Lit.hlsl"

            

            ENDHLSL
        }
    }
}
