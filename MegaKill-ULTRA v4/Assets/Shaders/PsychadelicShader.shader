Shader "Custom/PsychedelicShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _TimeSpeed ("Time Speed", Range(0, 10)) = 1.0
        _Frequency ("Frequency", Range(0.1, 10)) = 1.0
        _Amplitude ("Amplitude", Range(0.1, 5)) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            fixed4 _Color;
            float _TimeSpeed;
            float _Frequency;
            float _Amplitude;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float time = _Time.y * _TimeSpeed;
                float x = sin(i.uv.x * _Frequency + time) * _Amplitude;
                float y = cos(i.uv.y * _Frequency + time) * _Amplitude;

                float2 uv = i.uv + float2(x, y);
                fixed4 col = tex2D(_MainTex, uv) * _Color;

                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
