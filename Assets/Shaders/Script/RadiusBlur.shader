Shader "Custom/RadiusBlur"
{
    Properties
    {
        [HideInInspector]_MainTex("Texture",2D) = "white"{}

        _RadiusData("Radius Data", Vector) = (0.5, 0.5, 0.0, 1.0)
        _RadiusIterationData("Radius Iteration", Vector) = (1.0, 1.0, 0.0, 0.0)
        _RadiusCenterRange("Radius Center Range", Range(0, 0.5)) = 0
    }

    SubShader
    {
        CGINCLUDE
            #include "UnityCG.cginc"
            #pragma shader_feature _ _USE_CIRCLE_CENTER
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            half3 _RadiusData;
            half2 _RadiusIterationData;
            half _RadiusCenterRange;

            struct a2f_rb
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f_rb
            {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
            };

            v2f_rb RadiusBlurVertex(a2f_rb v)
            {
                v2f_rb o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.uv, _MainTex);

                return o;
            }

            half4 RadiusBlurFragment(v2f_rb i): SV_Target
            {
                half2 radiusRange = _RadiusData.xy - i.texcoord.xy;

                half4 color = 0.0;
                half range_amount = 1;
                #if defined(_USE_CIRCLE_CENTER)
                    half square_dis = dot(radiusRange, radiusRange);
                    half square_range = _RadiusCenterRange * _RadiusCenterRange;
                    range_amount = saturate((square_dis - square_range) / square_range);
                #endif
                [unroll(30)]
                for(int j = 0; j < _RadiusIterationData.x; j++)
                {
                    color += tex2D(_MainTex, i.texcoord);
                    i.texcoord += radiusRange * _RadiusData.z * range_amount;
                }

                return color * _RadiusIterationData.y;
            }
        ENDCG
        
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            CGPROGRAM
            #pragma vertex RadiusBlurVertex
            #pragma fragment RadiusBlurFragment
            ENDCG
        }
    }
}
