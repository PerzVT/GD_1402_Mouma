Shader "Custom/GlowingRingShader" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1, 0, 0, 1)
        _GlowIntensity ("Glow Intensity", Range(0, 10)) = 2
        _GlowWidth ("Glow Width", Range(0, 1)) = 0.1
    }

    SubShader {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        LOD 100

        CGPROGRAM
        #pragma surface surf Lambert

        sampler2D _MainTex;
        float4 _Color;
        float _GlowIntensity;
        float _GlowWidth;

        struct Input {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutput o) {
            float distToCenter = distance(IN.uv_MainTex, float2(0.5, 0.5));
            float glow = 1 - smoothstep(0.5 - _GlowWidth, 0.5 + _GlowWidth, distToCenter);
            o.Albedo = _Color.rgb * glow * _GlowIntensity;
            o.Alpha = glow;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
