Shader "Custom/ScanlineSpriteShader"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _Resolution ("Resolution", Float) = 128.0
        _ScanlineGapWidth ("Scanline Gap Width", Float) = 0.1
        _ScanlineHardness ("Scanline Hardness", Float) = 10.0
        _ScanlineOffset ("Scanline Offset", Float) = 0.05
        _AlternativeSize ("Alternative Size", Float) = 2.0
        _ColorMultiplier ("Color Multiplier", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 200

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha // Enable transparency blending
            ZWrite Off // Do not write to the depth buffer

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR; // To use the sprite's color
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR; // Pass the color to the fragment shader
            };

            sampler2D _MainTex;
            float _Resolution;
            float _ScanlineGapWidth;
            float _ScanlineHardness;
            float _ScanlineOffset;
            float _AlternativeSize;
            float _ColorMultiplier;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color; // Pass the sprite's color to the fragment shader
                return o;
            }

            float Scanline(float y, float offset, float multiplier)
            {
                return (tanh(sin(y * 3.14159 * _AlternativeSize) * multiplier + offset) * 0.5 + 0.5);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample the main texture (sprite)
                float4 color = tex2D(_MainTex, i.uv) * i.color; // Use the sprite's color
                
                // Compute the scanline effect for the current y-coordinate
                float scanlineEffect = Scanline(i.uv.y * _Resolution + _ScanlineOffset, -_ScanlineGapWidth, _ScanlineHardness);

                // Blend the scanline effect with the original color
                color.rgb *= scanlineEffect * _ColorMultiplier;

                return color; // Return the final color with scanline effect
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}