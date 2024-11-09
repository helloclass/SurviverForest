Shader "Unlit/CutTreeLeaf"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        _Timer ("Timer", float) = 0.0
        _PlayerDirection ("_PlayerDirection", Vector) =(0, 0, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque"}
        LOD 100

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

            StructuredBuffer<float4> positionBuffer;
            StructuredBuffer<float4> rotationBuffer;

            StructuredBuffer<float4> cutterUpBuffer;
            StructuredBuffer<float4> cutterPositionBuffer;

            float _Timer;
            float4 _PlayerDirection;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 test : TEXCOORD1;
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

                float rx = radians(rotationBuffer[instanceID].x + cutterPositionBuffer[5].x - 360.0f);
                float ry = radians(rotationBuffer[instanceID].y + cutterPositionBuffer[5].y - 360.0f);
                float rz = radians(rotationBuffer[instanceID].z + cutterPositionBuffer[5].z - 360.0f);

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

                localMat[3][0] = -cutterPositionBuffer[4].x;
                localMat[3][1] = -cutterPositionBuffer[4].y;
                localMat[3][2] = -cutterPositionBuffer[4].z;

                float3 worldPosition = mul(v.vertex, localMat); // 위치 적용

                localMat = 
                    float4x4(
                        1.0f, 0.0f, 0.0f, 0.0f,
                        0.0f, 1.0f, 0.0f, 0.0f,
                        0.0f, 0.0f, 1.0f, 0.0f,
                        0.0f, 0.0f, 0.0f, 1.0f
                     );

                v2f o;
                float degree = dot(normalize(cutterUpBuffer[0].xyz), normalize(worldPosition));

                rz = radians(_PlayerDirection.z) * _Timer;
                rx = radians(_PlayerDirection.x) * _Timer;

                localRollMat =
                    float4x4(
                        cos(rz), sin(rz), 0.0f, 0.0f,
                        -sin(rz), cos(rz), 0.0f, 0.0f,
                        0.0f, 0.0f, 1.0f, 0.0f,
                        0.0f, 0.0f, 0.0f, 1.0f
                        );
                localPitchMat =
                    float4x4(
                        1.0f, 0.0f, 0.0f, 0.0f,
                        0.0f, cos(rx), sin(rx), 0.0f,
                        0.0f, -sin(rx), cos(rx), 0.0f,
                        0.0f, 0.0f, 0.0f, 1.0f
                        );
                
                // 回転中心を変更
                localMat[3][1] = -cutterPositionBuffer[4].y + 0.3f;

                localMat = mul(localMat, localRollMat);
                localMat = mul(localMat, localPitchMat);

                localMat[3][1] += (cutterPositionBuffer[4].y + 0.3f);

                if (instanceID == 0)
                {
                    localMat = 
                        float4x4(
                            1.0f, 0.0f, 0.0f, 0.0f,
                            0.0f, 1.0f, 0.0f, 0.0f,
                            0.0f, 0.0f, 1.0f, 0.0f,
                            0.0f, 0.0f, 0.0f, 1.0f
                            );
                }

                o.test.x = (degree < 0.0) ? (instanceID & 1) : (1 - instanceID & 1);

                worldPosition = mul(v.vertex, localMat); // 위치 적용

                o.vertex = UnityObjectToClipPos(float4(worldPosition, 1.0));
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                clip(i.test.x - 0.2);

                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);

                clip(col.w - 0.2);

                return col;
            }
            ENDCG
        }
    }
}
