Shader "Unlit/BeeShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        _Timer ("Timer", float) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            #pragma instancing_options procedural:setup

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _Timer;

            StructuredBuffer<float4> positionBuffer;
            StructuredBuffer<float4> rotationBuffer;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert (
                    appdata v, 
                    uint instanceID : SV_InstanceID
                )
            {
                float4x4 localMat = 
                    float4x4(
                        1.0f, 0.0f, 0.0f, 0.0f,
                        0.0f, 1.0f, 0.0f, 0.0f,
                        0.0f, 0.0f, 1.0f, 0.0f,
                        0.0f, 0.0f, 0.0f, 1.0f
                        );

                float rx = radians(-90.0f);
                float ry = radians(0.0f);
                float rz = radians(0.0f);

                float4x4 localRollMat =
                    float4x4(
                        cos(rz), sin(rz), 0.0f, 0.0f,
                        -sin(rz), cos(rz), 0.0f, 0.0f,
                        0.0f, 0.0f, 1.0f, 0.0f,
                        0.0f, 0.0f, 0.0f, 1.0f
                        );
                float4x4 localYawMat =
                    float4x4(
                        cos(ry), 0.0f, -sin(ry), 0.0f,
                        0.0f, 1.0f, 0.0f, 0.0f,
                        sin(ry), 0.0f, cos(ry), 0.0f,
                        0.0f, 0.0f, 0.0f, 1.0f
                        );
                float4x4 localPitchMat =
                    float4x4(
                        1.0f, 0.0f, 0.0f, 0.0f,
                        0.0f, cos(rx), sin(rx), 0.0f,
                        0.0f, -sin(rx), cos(rx), 0.0f,
                        0.0f, 0.0f, 0.0f, 1.0f
                        );

                localMat = mul(localMat, localRollMat);
                localMat = mul(localMat, localYawMat);
                localMat = mul(localMat, localPitchMat);

                float4 worldPosition = mul(v.vertex, localMat); // 위치 적용



                localMat = 
                    float4x4(
                        3.0f, 0.0f, 0.0f, 0.0f,
                        0.0f, 3.0f, 0.0f, 0.0f,
                        0.0f, 0.0f, 3.0f, 0.0f,
                        0.0f, 0.0f, 0.0f, 1.0f
                        );

                rx = radians(rotationBuffer[instanceID].x);
                ry = radians(rotationBuffer[instanceID].y);
                rz = radians(rotationBuffer[instanceID].z);

                localRollMat =
                    float4x4(
                        cos(rz), sin(rz), 0.0f, 0.0f,
                        -sin(rz), cos(rz), 0.0f, 0.0f,
                        0.0f, 0.0f, 1.0f, 0.0f,
                        0.0f, 0.0f, 0.0f, 1.0f
                        );
                localYawMat =
                    float4x4(
                        cos(ry), 0.0f, -sin(ry), 0.0f,
                        0.0f, 1.0f, 0.0f, 0.0f,
                        sin(ry), 0.0f, cos(ry), 0.0f,
                        0.0f, 0.0f, 0.0f, 1.0f
                        );
                localPitchMat =
                    float4x4(
                        1.0f, 0.0f, 0.0f, 0.0f,
                        0.0f, cos(rx), sin(rx), 0.0f,
                        0.0f, -sin(rx), cos(rx), 0.0f,
                        0.0f, 0.0f, 0.0f, 1.0f
                        );

                localMat = mul(localMat, localRollMat);
                localMat = mul(localMat, localYawMat);
                localMat = mul(localMat, localPitchMat);

                localMat[3][0] = positionBuffer[instanceID].x;
                localMat[3][1] = positionBuffer[instanceID].y;
                localMat[3][2] = positionBuffer[instanceID].z;

                worldPosition = mul(worldPosition, localMat); // 위치 적용

                v2f o;
                o.vertex = UnityObjectToClipPos(worldPosition);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);

                //clip(col.w - 0.2);

                return col;
            }
            ENDCG
        }
    }
}
