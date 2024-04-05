Shader "Unlit/GradientShaderwithGlowEffect"
{
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _GradientDirection ("Gradient Direction", Range(0, 360)) = 0
        _Color1 ("Color 1", Color) = (1,1,1,1)
        _Color2 ("Color 2", Color) = (0,0,0,1)
        _Slide ("Slide", Range(0, 1)) = 0.5

        // New properties for controlling the fill percentage of each color
        _Color1_R_Percentage ("Color 1 R Percentage", Range(0, 1)) = 1
        _Color1_G_Percentage ("Color 1 G Percentage", Range(0, 1)) = 1
        _Color1_B_Percentage ("Color 1 B Percentage", Range(0, 1)) = 1

        _Color2_R_Percentage ("Color 2 R Percentage", Range(0, 1)) = 1
        _Color2_G_Percentage ("Color 2 G Percentage", Range(0, 1)) = 1
        _Color2_B_Percentage ("Color 2 B Percentage", Range(0, 1)) = 1

        _Color_A_Percentage ("Color Alpha Percentage", Range(1, 2)) = 1
        
    }
 
    SubShader {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
 
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
 
            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
 
            struct v2f {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };
 
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _GradientDirection;
            float4 _Color1;
            float4 _Color2;

            // New variables for fill amount of each color
            float _Color1_R_Percentage;
            float _Color1_G_Percentage;
            float _Color1_B_Percentage;
            float _Color2_R_Percentage;
            float _Color2_G_Percentage;
            float _Color2_B_Percentage;
            float _Slide;
            float _Color_A_Percentage;
 
            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }
 
            fixed4 frag (v2f i) : SV_Target {
                float2 uv = i.uv;
                float4 tex = tex2D(_MainTex, uv);
                float angle = (_GradientDirection + 45) / 180 * 3.14159265;
                float2 direction = float2(cos(angle), sin(angle));
                float2 gradientUV = uv - (_Slide - 0.1);
                float gradient = dot(gradientUV, direction) + 0.5;

                // Apply fill amount to each color individually
                fixed4 color;
                color.r = lerp(_Color1.r * _Color1_R_Percentage, _Color2.r * _Color2_R_Percentage, gradient);
                color.g = lerp(_Color1.g * _Color1_G_Percentage, _Color2.g * _Color2_G_Percentage, gradient);
                color.b = lerp(_Color1.b * _Color1_B_Percentage, _Color2.b * _Color2_B_Percentage, gradient);
                color.a = lerp(_Color1.a * _Color_A_Percentage, _Color2.a * _Color_A_Percentage, gradient);
                
                return tex * color;
            }
            ENDCG
        }
    }
}
