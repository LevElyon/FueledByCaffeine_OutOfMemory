Shader "Custom/UI_FadeRight"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Tint ("Tint", Color) = (1,1,1,1)
        _FadeStart ("Fade Start (0..1)", Range(0,1)) = 0.7
        _FadeWidth ("Fade Width (0..1)", Range(0,1)) = 0.3
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
            "RenderPipeline"="UniversalPipeline"
        }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _Tint;
                float4 _MainTex_ST;
                float _FadeStart;
                float _FadeWidth;
            CBUFFER_END

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                // Sample texture
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv) * _Tint;

                // Smooth horizontal fade (based on UV.x)
                float edge0 = _FadeStart;
                float edge1 = _FadeStart + _FadeWidth;
                float t = saturate((IN.uv.x - edge0) / max(edge1 - edge0, 0.0001));

                // Softer fade
                t = t * t * t * (t * (6*t - 15) + 10); // smoother S-curve

                // Invert so left=1, right=0
                float alphaMul = 1.0 - t;

                col.a *= alphaMul;
                return col;
            }

            ENDHLSL
        }
    }
}
