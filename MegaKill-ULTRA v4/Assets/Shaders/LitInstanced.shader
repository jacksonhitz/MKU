Shader "Custom/Instanced Lit (Combined)"
{
    Properties
    {
        // Hidden properties for rendering setup
        [HideInInspector] _SrcBlend("Source Blend", Float) = 1.0
        [HideInInspector] _DstBlend("Destination Blend", Float) = 0.0
        [HideInInspector] _ZWrite("ZWrite", Float) = 1.0

        [MainTexture] _MainTex ("Albedo (RGB) Alpha (A)", 2D) = "white" {}
        [Normal] _BumpMap ("Normal Map", 2D) = "bump" {}
        _BumpScale ("Normal Scale", Float) = 1.0
        [MainColor] _Color ("Color", Color) = (1,1,1,1)

        [Space(10)]
        [Header(Transparency Settings)]
        [Space(5)]
        [Enum(Rendering.TransparencyMode)] _TransparencyMode ("Transparency Mode", Float) = 0
        _Cutoff ("Alpha Cutoff", Range(0.0, 1.0)) = 0.5
        _DitherThreshold ("Dither Threshold", Range(0.0, 1.0)) = 0.75

        // Advanced options
        [Space(10)]
        [Header(Advanced Settings)]
        [Space(5)]
        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Culling", Float) = 2
    }

    CustomEditor "Rendering.InstancedLitShaderGUI"

    SubShader
    {
        // Base pass for all modes
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            Blend [_SrcBlend] [_DstBlend]
            ZWrite [_ZWrite]
            Cull [_Cull]

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma target 3.5

            // Define shader feature for different transparency modes
            #pragma shader_feature_local _TRANSPARENCY_OFF _TRANSPARENCY_CUTOUT _TRANSPARENCY_DITHER


            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"

            // Properties
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_ST;
            float _BumpScale;
            float _Cutoff;
            float _DitherThreshold;

            // Instanced properties
            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
            UNITY_INSTANCING_BUFFER_END(Props)

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 bumpUV : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
                float3 worldTangent : TEXCOORD3;
                float3 worldBitangent : TEXCOORD4;
                float3 worldPos : TEXCOORD5;
                float4 screenPosition : TEXCOORD6;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            // Dithering function for the dithered transparency mode
            float DitherPattern(float2 screenPos)
            {
                // 4x4 Bayer matrix dither pattern
                const float4x4 thresholdMatrix = {
                    1.0 / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0,
                    13.0 / 17.0,  5.0 / 17.0, 15.0 / 17.0,  7.0 / 17.0,
                    4.0 / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0,
                    16.0 / 17.0,  8.0 / 17.0, 14.0 / 17.0,  6.0 / 17.0
                };

                uint index_x = screenPos.x % 4;
                uint index_y = screenPos.y % 4;

                return thresholdMatrix[index_y][index_x];
            }

            v2f vert (appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);

                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.screenPosition = ComputeScreenPos(o.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.bumpUV = o.uv;

                o.worldNormal = TransformObjectToWorldNormal(v.normal);
                o.worldTangent = normalize(TransformObjectToWorldDir(v.tangent.xyz));
                o.worldBitangent = normalize(cross(o.worldNormal, o.worldTangent) * v.tangent.w);
                o.worldPos = TransformObjectToWorld(v.vertex.xyz);

                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);

                // Normal mapping
                half4 normalSample = SAMPLE_TEXTURE2D(_BumpMap, sampler_MainTex, i.bumpUV);
                half3 normalMap = UnpackNormal(normalSample);
                normalMap.xy *= _BumpScale;

                float3x3 TBN = float3x3(i.worldTangent, i.worldBitangent, i.worldNormal);
                float3 worldNormal = normalize(mul(normalMap, TBN));

                // Sample albedo texture and apply color
                float4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                float4 color = UNITY_ACCESS_INSTANCED_PROP(Props, _Color);
                col.rgb *= color.rgb;
                col.a *= color.a;

                // Simple directional lighting
                float3 lightDir = normalize(float3(1, 1, -1));
                float ndotl = max(0, dot(worldNormal, lightDir)) * 0.5 + 0.5;
                col.rgb *= ndotl;

                // Handle different transparency modes
                #if defined(_TRANSPARENCY_CUTOUT)
                    // Alpha cutout mode
                    clip(col.a - _Cutoff);
                #elif defined(_TRANSPARENCY_DITHER)
                    // Dithered transparency
                    float2 screenPos = i.screenPosition.xy / i.screenPosition.w * _ScreenParams.xy;
                    float dither = DitherPattern(screenPos);
                    clip(col.a - dither * _DitherThreshold);
                #endif

                return col;
            }
            ENDHLSL
        }

        // Shadow casting pass
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }

            ZWrite On
            ZTest LEqual
            Cull[_Cull]

            HLSLPROGRAM
            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment
            #pragma multi_compile_instancing

            // Define shader feature for different transparency modes
            #pragma shader_feature_local _TRANSPARENCY_OFF _TRANSPARENCY_CUTOUT _TRANSPARENCY_DITHER

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

            // These are needed for the shadow caster pass
            float3 _LightDirection;

            // Properties
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_ST;
            float _Cutoff;
            float _DitherThreshold;

            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
            UNITY_INSTANCING_BUFFER_END(Props)

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float4 positionCS : SV_POSITION;
                float4 screenPosition : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            float DitherPattern(float2 screenPos)
            {
                const float4x4 thresholdMatrix = {
                    1.0 / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0,
                    13.0 / 17.0,  5.0 / 17.0, 15.0 / 17.0,  7.0 / 17.0,
                    4.0 / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0,
                    16.0 / 17.0,  8.0 / 17.0, 14.0 / 17.0,  6.0 / 17.0
                };

                uint index_x = screenPos.x % 4;
                uint index_y = screenPos.y % 4;

                return thresholdMatrix[index_y][index_x];
            }

            Varyings ShadowPassVertex(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);

                // Convert to world space
                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                float3 normalWS = TransformObjectToWorldNormal(input.normalOS);

                // Calculate shadow position directly
                float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, _LightDirection));

                #if UNITY_REVERSED_Z
                    positionCS.z = min(positionCS.z, UNITY_NEAR_CLIP_VALUE);
                #else
                    positionCS.z = max(positionCS.z, UNITY_NEAR_CLIP_VALUE);
                #endif

                output.positionCS = positionCS;
                output.uv = TRANSFORM_TEX(input.texcoord, _MainTex);
                output.screenPosition = ComputeScreenPos(output.positionCS);

                return output;
            }

            half4 ShadowPassFragment(Varyings input) : SV_TARGET
            {
                UNITY_SETUP_INSTANCE_ID(input);

                float4 albedoAlpha = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                float4 color = UNITY_ACCESS_INSTANCED_PROP(Props, _Color);
                albedoAlpha.a *= color.a;

                #if defined(_TRANSPARENCY_CUTOUT)
                    clip(albedoAlpha.a - _Cutoff);
                #elif defined(_TRANSPARENCY_DITHER)
                    float2 screenPos = input.screenPosition.xy / input.screenPosition.w * _ScreenParams.xy;
                    float dither = DitherPattern(screenPos);
                    clip(albedoAlpha.a - dither * _DitherThreshold);
                #endif

                return 0;
            }
            ENDHLSL
        }

        // DepthOnly pass for depth prepass
        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode" = "DepthOnly" }

            ZWrite On
            ColorMask 0
            Cull[_Cull]

            HLSLPROGRAM
            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment
            #pragma multi_compile_instancing

            #pragma shader_feature_local _TRANSPARENCY_OFF _TRANSPARENCY_CUTOUT _TRANSPARENCY_DITHER

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_ST;
            float _Cutoff;
            float _DitherThreshold;

            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
            UNITY_INSTANCING_BUFFER_END(Props)

            struct Attributes
            {
                float4 position : POSITION;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float4 positionCS : SV_POSITION;
                float4 screenPosition : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            float DitherPattern(float2 screenPos)
            {
                const float4x4 thresholdMatrix = {
                    1.0 / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0,
                    13.0 / 17.0,  5.0 / 17.0, 15.0 / 17.0,  7.0 / 17.0,
                    4.0 / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0,
                    16.0 / 17.0,  8.0 / 17.0, 14.0 / 17.0,  6.0 / 17.0
                };

                uint index_x = screenPos.x % 4;
                uint index_y = screenPos.y % 4;

                return thresholdMatrix[index_y][index_x];
            }

            Varyings DepthOnlyVertex(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);

                output.positionCS = TransformObjectToHClip(input.position.xyz);
                output.uv = TRANSFORM_TEX(input.texcoord, _MainTex);
                output.screenPosition = ComputeScreenPos(output.positionCS);

                return output;
            }

            half4 DepthOnlyFragment(Varyings input) : SV_TARGET
            {
                UNITY_SETUP_INSTANCE_ID(input);

                float4 albedoAlpha = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                float4 color = UNITY_ACCESS_INSTANCED_PROP(Props, _Color);
                albedoAlpha.a *= color.a;

                #if defined(_TRANSPARENCY_CUTOUT)
                    clip(albedoAlpha.a - _Cutoff);
                #elif defined(_TRANSPARENCY_DITHER)
                    float2 screenPos = input.screenPosition.xy / input.screenPosition.w * _ScreenParams.xy;
                    float dither = DitherPattern(screenPos);
                    clip(albedoAlpha.a - dither * _DitherThreshold);
                #endif

                return 0;
            }
            ENDHLSL
        }
    }
}
