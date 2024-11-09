using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmeBeeInstancing : Enemy
{
    [Range(1, 1000)]
    public int instanceCount = 1000;
    public Mesh[] mesh;
    public Material[] material;
    int meshCount = 0;
    Bounds renderBounds = new Bounds(Vector3.zero, Vector3.one);

    private ComputeBuffer[] argsBuffer;     // 메시 데이터 버퍼
    private ComputeBuffer positionBuffer; // 위치&스케일 버퍼
    private ComputeBuffer rotationBuffer; // 위치&스케일 버퍼
    private uint[] argsData;

    // assign instnace position
    Vector4[] positions;
    Vector4[] rotations;

    float[] velocities;
    float[] HPs;

    // V1, V2, V3, V4, PlanePosition, PlaneRotation
    Vector4[] up4 = new Vector4[1];

    Vector3 InitBeePosition;

    public override void Start()
    {
        base.Start();

        meshCount = mesh.Length;

        renderBounds.center = gameObject.transform.position;
        renderBounds.size = new Vector3(5.0f, 5.0f, 5.0f);

        InitBeePosition = transform.position;

        // assign argument
        argsBuffer = new ComputeBuffer[meshCount];
        argsData = new uint[5];

        for (int i = 0; i < meshCount; i++)
        {
            argsBuffer[i] = new ComputeBuffer(1, sizeof(uint) * 5, ComputeBufferType.IndirectArguments);

            argsData[0] = (uint)mesh[i].GetIndexCount(0);
            argsData[1] = (uint)instanceCount;
            argsData[2] = (uint)mesh[i].GetIndexStart(0);
            argsData[3] = (uint)mesh[i].GetBaseVertex(0);
            argsData[4] = 0;

            argsBuffer[i].SetData(argsData);
        }

        // assign instnace position
        positions = new Vector4[instanceCount];
        rotations = new Vector4[instanceCount];

        velocities = new float[instanceCount];
        HPs = new float[instanceCount];

        // XYZ : 위치, W : 스케일
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                for (int k = 0; k < 10; k++)
                {
                    ref Vector4 pos = ref positions[i * 100 + j * 10 + k] ;
                    ref Vector4 rot = ref rotations[i * 100 + j * 10 + k];

                    pos.x = 0.0f;
                    pos.y = 0.0f;
                    pos.z = 0.0f;
                    pos.w = 1.0f; // Scale

                    rot.x = -90.0f;
                    rot.y = 0.0f;
                    rot.z = 0.0f;
                    rot.w = 1.0f; // Scale

                    velocities[i * 100 + j * 10 + k] = Random.Range(0.3f, 0.6f);
                    HPs[i * 100 + j * 10 + k]        = Random.Range(10.0f, 50.0f);
                }
            }
        }

        positionBuffer = new ComputeBuffer(instanceCount, sizeof(float) * 4);
        positionBuffer.SetData(positions);

        rotationBuffer = new ComputeBuffer(instanceCount, sizeof(float) * 4);
        rotationBuffer.SetData(rotations);

        for (int i = 0; i < meshCount; i++)
        {
            material[i].SetBuffer("positionBuffer", positionBuffer);
            material[i].SetBuffer("rotationBuffer", rotationBuffer);
        }
    }

    private void Update()
    {
        transform.LookAt(PlayerObject.transform);

        int index = 0;
        float sine = 0.0f;
        float cosine = 0.0f;

        Vector3 EulerRotation = transform.rotation.eulerAngles;
        Vector3 PlayerPosition = new Vector3(
                PlayerObject.transform.position.x,
                PlayerObject.transform.position.y,
                PlayerObject.transform.position.z
            );

        // プレイヤーを伴う
        transform.position = Vector3.Lerp(transform.position, PlayerPosition, Time.deltaTime);

        renderBounds.center = transform.position;

        bool IsAllDead = true;

        // XYZ : 위치, W : 스케일
        for (int i = -5; i < 5; i++)
        {
            for (int j = -5; j < 5; j++)
            {
                for (int k = -5; k < 5; k++)
                {
                    if (0.0f < HPs[index])
                    {
                        IsAllDead = false;

                        sine = Mathf.Sin(Time.time * velocities[index] * 10.0f) * 0.025f;
                        cosine = Mathf.Cos(Time.time * velocities[index] * 10.0f) * 0.025f;

                        ref Vector4 pos = ref positions[index];
                        ref Vector4 rot = ref rotations[index];

                        if (index % 2 == 1)
                        {
                            pos.x += i * velocities[index] * sine + j * velocities[index] * cosine;
                            pos.y += j * velocities[index] * sine + k * velocities[index] * cosine;
                            pos.z += k * velocities[index] * sine + i * velocities[index] * cosine;
                            pos.w = 1.0f; // Scale
                        }
                        else
                        {
                            pos.x += i * velocities[index] * cosine + j * velocities[index] * sine;
                            pos.y += j * velocities[index] * cosine + k * velocities[index] * sine;
                            pos.z += k * velocities[index] * cosine + i * velocities[index] * sine;
                            pos.w = 1.0f; // Scale
                        }

                        rot.x = EulerRotation.x;
                        rot.y = EulerRotation.y;
                        rot.z = EulerRotation.z;
                        rot.w = 1.0f; // Scale
                    }
                    else
                    {
                        ref Vector4 pos = ref positions[index];

                        positions[index].y -= 1.5f * Time.deltaTime;
                    }

                    index = index + 1;
                }
            }
        }

        positionBuffer = new ComputeBuffer(instanceCount, sizeof(float) * 4);
        positionBuffer.SetData(positions);

        rotationBuffer = new ComputeBuffer(instanceCount, sizeof(float) * 4);
        rotationBuffer.SetData(rotations);

        for (int i = 0; i < meshCount; i++)
        {
            material[i].SetFloat("_Timer", Time.time * 10.0f);

            material[i].SetBuffer("positionBuffer", positionBuffer);
            material[i].SetBuffer("rotationBuffer", rotationBuffer);

            Graphics.DrawMeshInstancedIndirect(
                mesh[i],         // 그려낼 메시
                0,               // 서브메시 인덱스
                material[i],     // 그려낼 마테리얼
                renderBounds,    // 렌더링 영역
                argsBuffer[i]    // 메시 데이터 버퍼
            );
        }

        if (IsAllDead)
        {
            Dead();
        }
    }

    private void OnDestroy()
    {
        for (int i = 0; i < meshCount; i++)
        {
            if (argsBuffer[i] != null)
                argsBuffer[i].Release();
        }

        if (positionBuffer != null)
            positionBuffer.Release();
    }

    public override void Attacking(PlayerController player)
    {
        // PlayerObject의 HP를 깎는다.
        //PowerPoint;
        //PlayerObject;
    }

    public override void Damaging(float DamageHP, float RecoilPower = 100.0f)
    {
        //base.Damaging(DamageHP, RecoilPower);
    }

    public override void GetFired()
    {
        if (!isDead)
        {
            for (int i = 0; i < instanceCount; i++)
            {
                HPs[i] -= 0.1f;
            }
        }
    }

    public override void Dead()
    {
        base.Dead();

        isDead = true;
    }
}
