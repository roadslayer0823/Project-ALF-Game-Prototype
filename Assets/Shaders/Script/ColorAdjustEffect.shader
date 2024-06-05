Shader "Unlit/ColorAdjustEffect"
{
    Properties
	{
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Brightness("Brightness", Float) = 1	
		_Saturation("Saturation", Float) = 1	
		_Contrast("Contrast", Float) = 1		
	}

		SubShader
		{
			Pass
				{
			ZTest Always
			Cull Off 
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			sampler2D _MainTex;
			half _Brightness;
			half _Saturation;
			half _Contrast;

			#pragma vertex vert
			#pragma fragment frag
			#include "Lighting.cginc"


			struct appdata_t
			{
				float4 vertex : POSITION;
				half4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};
			struct v2f
			{
				float4 pos : SV_POSITION; 
				half2  uv : TEXCOORD0;	  
				half4 color : COLOR;
			};

			//vertex shader
			v2f vert(appdata_t v)
			{
				v2f o;
				
				o.pos = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				
				o.uv = v.texcoord;
				return o;
			}

			//fragment shader
			fixed4 frag(v2f i) : COLOR
			{
			fixed4 renderTex = tex2D(_MainTex, i.uv)*i.color;
			fixed3 finalColor = renderTex * _Brightness;
			fixed gray = 0.2125 * renderTex.r + 0.7154 * renderTex.g + 0.0721 * renderTex.b;
			fixed3 grayColor = fixed3(gray, gray, gray);
			
			finalColor = lerp(grayColor, finalColor, _Saturation);
			
			fixed3 avgColor = fixed3(0.5, 0.5, 0.5);
			finalColor = lerp(avgColor, finalColor, _Contrast);
			return fixed4(finalColor, renderTex.a);
				}
			ENDCG
			}
		}
	FallBack Off
}
