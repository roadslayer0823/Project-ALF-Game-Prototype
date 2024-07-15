Shader "Unlit/DualSideFillAmount"
{
 Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        [HideInInspector] _Color ("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
        _FillAmount ("FillAmount", Range(0, 1)) = 1
        [MaterialToggle] _FocusLeft ("FocusLeft", Range(0, 1)) = 1
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex SpriteVert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile_local _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #include "UnitySprites.cginc"

            float _FillAmount;
            float _FocusLeft;

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color;
                c.rgb *= c.a;

                // Calculate the position relative to the center (0.5, 0.5)
                float2 uv = IN.texcoord - 0.5;
                float angle = atan2(uv.y, uv.x);
                
                // Normalize angle to range [0, 2]
                float normalizedAngle = (angle + UNITY_PI) / UNITY_PI; // Convert to [0, 2] range

                // Adjust normalized angle for the 180-degree fill
                if (normalizedAngle > 1.0)
                {
                    normalizedAngle -= 1.0;
                }

                // Shift the angle range based on the focus direction
                if (_FocusLeft == 1)
                {
                    normalizedAngle = 1.0 - normalizedAngle; // Focus on the left side
                }

                // Calculate the fill amount starting from the center and expanding outwards
                float fillThreshold = _FillAmount * 0.5;

                // Check if the current pixel should be filled
                if (normalizedAngle < 0.5 - fillThreshold || normalizedAngle > 0.5 + fillThreshold)
                {
                    discard;
                }

                return c;
            }
            ENDCG
        }
    }
}
