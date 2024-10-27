Shader "Custom/Simple3DWater"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {} // Water surface texture
        _WaveSpeed ("Wave Speed", Float) = 0.2 // Speed of waves
        _Frequency ("Wave Frequency", Float) = 8.0 // Frequency of waves
        _Amplitude ("Wave Amplitude", Float) = 0.1 // Height of waves
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" } // Basic opaque rendering
        LOD 200 // Level of detail, for performance optimization

        Pass
        {
            CGPROGRAM
            #pragma vertex vert // Use the 'vert' function as the vertex shader
            #pragma fragment frag // Use the 'frag' function as the fragment shader

            #include "UnityCG.cginc" // Unity's built-in utilities

            // Properties for the shader
            sampler2D _MainTex; // Water texture
            float _WaveSpeed; // Wave speed
            float _Frequency; // Wave frequency
            float _Amplitude; // Wave height/amplitude

            struct appdata // Vertex data coming from Unity
            {
                float4 vertex : POSITION; // Vertex position
                float3 normal : NORMAL; // Normal data for lighting
                float2 uv : TEXCOORD0; // UV texture coordinates
            };

            struct v2f // Data sent from vertex to fragment shader
            {
                float2 uv : TEXCOORD0; // UV texture coordinates
                float4 vertex : SV_POSITION; // Screen-space position
                float3 worldPos : TEXCOORD1; // World position
            };

            // Vertex shader: transforms vertices and passes world position and UVs to the fragment shader
            v2f vert(appdata v)
            {
                v2f o;
                o.uv = v.uv;
                o.vertex = UnityObjectToClipPos(v.vertex); // Convert object space to clip space
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz; // Get world position of vertex
                return o;
            }

            // Water wave distortion function based on world position
            float2 WaterWaveDistortion(float3 worldPos)
            {
                float timeFactor = _Time.y * _WaveSpeed; // Time-based factor for wave movement
                float3 pos = worldPos * _Frequency + timeFactor; // Modify position by frequency and time

                // Calculate wave distortion using sine and cosine functions
                float waveX = cos(pos.x - pos.z) * cos(pos.z);
                float waveY = sin(pos.x + pos.z) * sin(pos.z);

                return float2(waveX, waveY); // Return UV distortion based on calculated wave
            }

            // Fragment shader: apply water effect and sample texture
            fixed4 frag(v2f i) : SV_Target
            {
                // Apply water wave distortion to the UVs based on world position
                float2 waveDistortion = WaterWaveDistortion(i.worldPos);
                float2 distortedUV = i.uv + waveDistortion * _Amplitude; // Apply amplitude to distortion

                // Sample the water texture with the distorted UVs
                fixed4 col = tex2D(_MainTex, distortedUV);

                return col; // Output the final color
            }
            ENDCG
        }
    }
    FallBack "Diffuse" // Fallback to a basic diffuse shader if unsupported
}
