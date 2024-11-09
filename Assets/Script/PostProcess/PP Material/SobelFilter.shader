Shader "Unlit/SobelFilter"
{
	Properties 
	{
	    [HideInInspector]_MainTex ("Base (RGB)", 2D) = "white" {}

        _BloodShotTex ("BloodShotTex (RGB)", 2D) = "white" {}

        _Color ("VignetteColor", Color) = (1, 1, 1, 1)
        _Power ("VignettePower", float) = 1.0

        _IsWarning ("Warning", Float) = 0
        _IsDistort ("Distort", Float) = 0
        _IsUsedDynamicDoF ("IsUsedDynamicDoF", Float) = 0

        // ÀÜ»ó°£ÀÇ °£°Ý
        _FocusDistance ("FocusDistance", Range(0.0, 0.01)) = 0.005

        _MinDepthOfField ("MinDepthOfField", Range(0.0, 0.0075)) = 0.0005
        _MaxDepthOfField ("MaxDepthOfField", Range(-0.01, 0.015)) = -0.01

        _Timer ("Timer", float) = 0.0

	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		Pass
		{
            Name "Sobel Filter"
            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            
            #pragma shader_feature RAW_OUTLINE
            #pragma shader_feature POSTERIZE
            
            TEXTURE2D(_CameraDepthTexture);
            SAMPLER(sampler_CameraDepthTexture);
            
#ifndef RAW_OUTLINE
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
#endif
            TEXTURE2D(_BloodShotTex);
            SAMPLER(sampler_BloodShotTex);         

            float4 _Color;
            float _Power;

            float _IsWarning;
            float _IsDistort;
            float _IsUsedDynamicDoF;

            float _FocusDistance;

            float _MinDepthOfField;
            float _MaxDepthOfField;

            float _Timer;

            struct Attributes
            {
                float4 positionOS       : POSITION;
                float2 uv               : TEXCOORD0;
            };

            struct Varyings
            {
                float4 vertex           : SV_POSITION;
                float2 uv               : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            // ³ëÀÌÁî ÇÔ¼ö Æ÷ÇÔ
            float Hash21(float2 p)
            {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 34.23);
                return frac(p.x * p.y);
            }

            float Noise(float2 uv)
            {
                float2 i = floor(uv);
                float2 f = frac(uv);

                float a = Hash21(i);
                float b = Hash21(i + float2(1.0, 0.0));
                float c = Hash21(i + float2(0.0, 1.0));
                float d = Hash21(i + float2(1.0, 1.0));

                float2 u = f * f * (3.0 - 2.0 * f);

                return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y);
            }
            
            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.vertex = vertexInput.positionCS;
                output.uv = input.uv;
                
                return output;
            }

            half4 frag (Varyings input) : SV_Target 
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float noiseValue = Noise((input.uv + _Timer) * 10.0);

                float2 uv_distorted = 
                    float2(
                        input.uv.x + (noiseValue * 0.1) - 0.05,
                        input.uv.y + (noiseValue * 0.1) - 0.05
                    );

                half4 col = half4(0, 0, 0, 0);
                half4 blood_col = half4(0, 0, 0, 0);
                float uv_length = 0;

                blood_col = SAMPLE_TEXTURE2D(_BloodShotTex, sampler_BloodShotTex, uv_distorted);

                float2 uv_center = uv_distorted;
                uv_center.x = uv_center.x - 0.5;
                uv_center.y = uv_center.y - 0.5;

                uv_length = length(uv_center);
                uv_length = pow(uv_length, _Power);

                blood_col = blood_col * _IsWarning;
                uv_length = uv_length * _IsWarning;

////////////////////////////////////////////////////////////////////////
// USE BOKEH
////////////////////////////////////////////////////////////////////////

                // depth.x => [0, 850]
                
                half4 depth = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, input.uv);
                half4 center_depth = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, float2(0.5f, 0.5f));
                //depth = depth * 850.0f;

                float X1 = _FocusDistance;
                float Y1 = _FocusDistance;

                float X2 = -_FocusDistance;
                float Y2 = -_FocusDistance;

                float MaxDof = _MaxDepthOfField + (center_depth * 2.0f) * _IsUsedDynamicDoF;
                float MinDof = _MinDepthOfField + (center_depth * 1.0f) * _IsUsedDynamicDoF;

                // ÐÎª¤ªËíÞùê
                if (MaxDof < depth.x)
                {
                    X1 = X1 * ((MaxDof - depth.x) * 200);
                    Y1 = Y1 * ((MaxDof - depth.x) * 200);

                    X2 = X2  * ((MaxDof - depth.x) * 200);
                    Y2 = Y2  * ((MaxDof - depth.x) * 200);
                }
                // êÀª¤ªÎíÞùê
                else if (depth.x < MinDof)
                {
                    X1 = X1 * ((MinDof - depth.x) * 300);
                    Y1 = Y1 * ((MinDof - depth.x) * 300);

                    X2 = X2  * ((MinDof - depth.x) * 300);
                    Y2 = Y2  * ((MinDof - depth.x) * 300);
                }
                else
                {
                    X1 = 0.0f;
                    Y1 = 0.0f;

                    X2 = 0.0f;
                    Y2 = 0.0f;
                }

                float2 finalUV = uv_distorted * _IsDistort + input.uv * (1.0 - _IsDistort);
                col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, finalUV) * 0.025 + 0.01;

                for (int i = 1; i <= 10; i++)
                {
                    float2 _finalUV = float2(finalUV.x + X1 * i * 0.1f, finalUV.y + Y1 * i * 0.1f);
                    col = col + SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, _finalUV) * 0.02;

                    _finalUV = float2(finalUV.x + X1 * -i * 0.1f, finalUV.y + Y1 * i * 0.1f);
                    col = col + SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, _finalUV) * 0.02;

                    _finalUV = float2(finalUV.x + X1 * -i * 0.1f, finalUV.y + Y1 * -i * 0.1f);
                    col = col + SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, _finalUV) * 0.02;

                    _finalUV = float2(finalUV.x + X1 * i * 0.1f, finalUV.y + Y1 * -i * 0.1f);
                    col = col + SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, _finalUV) * 0.02;
                }

                for (int i = 1; i <= 10; i++)
                {
                    float2 _finalUV = float2(finalUV.x + X2 * i * 0.1f, finalUV.y + Y2 * i * 0.1f);
                    col = col + SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, _finalUV) * 0.02;

                    _finalUV = float2(finalUV.x + X2 * -i * 0.1f, finalUV.y + Y2 * i * 0.1f);
                    col = col + SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, _finalUV) * 0.02;

                    _finalUV = float2(finalUV.x + X2 * -i * 0.1f, finalUV.y + Y2 * -i * 0.1f);
                    col = col + SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, _finalUV) * 0.02;

                    _finalUV = float2(finalUV.x + X2 * i * 0.1f, finalUV.y + Y2 * -i * 0.1f);
                    col = col + SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, _finalUV) * 0.02;
                }

                col = col + pow(col, 5.0);

////////////////////////////////////////////////////////////////////////

                return blood_col * blood_col.w + col * (1.0 - blood_col.w) + uv_length * _Color;
            }
            
			#pragma vertex vert
			#pragma fragment frag
			
			ENDHLSL
		}
	} 
	FallBack "Diffuse"
}
