using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_ComputeShader : MonoBehaviour
{
    [Range(1, 2)]
    public int instanceCount = 2;
    public Mesh mesh;
    public Material[] material;
    int subMeshCount = 0;
    Bounds renderBounds = new Bounds(Vector3.zero, Vector3.one);

    public GameObject PlayerObject;
    public GameObject HiveObejct;
    public bool isCut = false;

    private Material[] PrivateMat;

    public int HP = 5;

    private ComputeBuffer[] argsBuffer;     // 메시 데이터 버퍼
    private ComputeBuffer positionBuffer; // 위치&스케일 버퍼
    private ComputeBuffer rotationBuffer; // 위치&스케일 버퍼
    private ComputeBuffer cutterUpBuffer; // 오브젝트가 잘리는 단면 버퍼
    private ComputeBuffer cutterPositionBuffer; // 오브젝트가 잘리는 단면 버퍼
    private uint[] argsData;

    // assign instnace position
    Vector4[] positions;
    Vector4[] rotations;

    // V1, V2, V3, V4, PlanePosition, PlaneRotation
    Vector4[] up4 = new Vector4[1];

    public void Start()
    {
        subMeshCount = mesh.subMeshCount;

        renderBounds.center = gameObject.transform.position;
        renderBounds.size = new Vector3(5.0f, 5.0f, 5.0f);

        // assign argument
        argsBuffer = new ComputeBuffer[subMeshCount];
        argsData = new uint[5];

        for (int i = 0; i < subMeshCount; i++)
        {
            argsBuffer[i] = new ComputeBuffer(1, sizeof(uint) * 5, ComputeBufferType.IndirectArguments);

            argsData[0] = (uint)mesh.GetIndexCount(i);
            argsData[1] = (uint)instanceCount;
            argsData[2] = (uint)mesh.GetIndexStart(i);
            argsData[3] = (uint)mesh.GetBaseVertex(i);
            argsData[4] = 0;

            argsBuffer[i].SetData(argsData);
        }

        // assign instnace position
        positions = new Vector4[instanceCount];
        rotations = new Vector4[instanceCount];

        PrivateMat = new Material[material.Length];
        for (int i = 0; i < material.Length; i++)
        {
            PrivateMat[i] = new Material(material[i]);
        }
    }

    public void Chopping(Vector4[] cutterPlane)
    {
        if (cutterPlane.Length != 7)
        {
            Debug.Log("cutterPlane.Length is must 6!");
            return;
        }

        if (0 < HP)
        {
            HP = HP - 1;

            StartCoroutine(TreeVibration());
            return;
        }

        isCut = true;

        if (Random.Range(0.0f, 1.0f) < 0.5f)
        {
            HiveObejct.SetActive(true);
            HiveObejct.GetComponent<BoxCollider>().enabled = true;
            HiveObejct.GetComponent<Rigidbody>().isKinematic = false;

            HiveObejct.GetComponent<HIveController>().BeeThrow();
        }

        //GetComponent<MeshRenderer>().enabled = false;

        // float4 baseNormal = normalize(cross(p2 - p1, p4 - p1));
        Vector4 v1 = cutterPlane[1] - cutterPlane[0];
        Vector4 v2 = cutterPlane[3] - cutterPlane[0];

        Vector3 up = Vector3.Cross(
            new Vector3(v1.x, v1.y, v1.z), 
            new Vector3(v2.x, v2.y, v2.z)
        );
        up = Vector3.Normalize(up);

        // assign instnace position
        Vector4[] new_positions = new Vector4[instanceCount];
        Vector4[] new_rotations = new Vector4[instanceCount];

        // Vector4 up4 = new Vector4(up.x, up.y, up.z, 1.0f);
        up4[0].x = up.x;
        up4[0].y = up.y;
        up4[0].z = up.z;
        up4[0].w = 1.0f;

        positionBuffer = new ComputeBuffer(instanceCount, sizeof(float) * 4);
        positionBuffer.SetData(new_positions);

        rotationBuffer = new ComputeBuffer(instanceCount, sizeof(float) * 4);
        rotationBuffer.SetData(new_rotations);

        cutterUpBuffer = new ComputeBuffer(1, sizeof(float) * 4);
        cutterUpBuffer.SetData(up4);

        cutterPlane[0] = Vector4.Normalize(cutterPlane[0]);

        cutterPositionBuffer = new ComputeBuffer(7, sizeof(float) * 4);
        cutterPositionBuffer.SetData(cutterPlane);

        for (int i = 0; i < subMeshCount; i++)
        {
            PrivateMat[i].SetBuffer("positionBuffer", positionBuffer);
            PrivateMat[i].SetBuffer("rotationBuffer", rotationBuffer);
            PrivateMat[i].SetBuffer("cutterUpBuffer", cutterUpBuffer);
            PrivateMat[i].SetBuffer("cutterPositionBuffer", cutterPositionBuffer);
        }
    }

    float SquareTimer = 0.0f;

    private void Update()
    {
        if (isCut)
        {
            GetComponent<MeshRenderer>().enabled = false;

            for (int i = 0; i < subMeshCount; i++)
            {
                if (SquareTimer < 120.0f)
                {
                    // material.SetInt("hello", 5);
                    PrivateMat[i].SetFloat("_Timer", SquareTimer);
                    PrivateMat[i].SetVector("_PlayerDirection", PlayerObject.transform.forward);
                }

                Graphics.DrawMeshInstancedIndirect(
                    mesh,            // 그려낼 메시
                    0,               // 서브메시 인덱스
                    PrivateMat[i],     // 그려낼 마테리얼
                    renderBounds,    // 렌더링 영역
                    argsBuffer[i]    // 메시 데이터 버퍼
                );
            }

            SquareTimer += Time.time;
        }
    }

    private void OnDestroy()
    {
        for (int i = 0; i < subMeshCount; i++)
        {
            if (argsBuffer[i] != null)
                argsBuffer[i].Release();
        }

        if (positionBuffer != null)
            positionBuffer.Release();

        if (cutterUpBuffer != null)
            cutterUpBuffer.Release();

        if (cutterPositionBuffer != null)
            cutterPositionBuffer.Release();
    }

    IEnumerator TreeVibration()
    {
        Vector3 OriginPower = transform.position;

        float VibrationPower = 0.0f;
        float VibrationTime = 1.0f;
        while (0.01f < VibrationTime)
        {
            VibrationPower = Mathf.Sin(Time.time * 100.0f) * VibrationTime * 0.01f;
            VibrationTime = VibrationTime - Time.deltaTime;

            transform.position = OriginPower + PlayerObject.transform.right * VibrationPower;

            yield return new WaitForSeconds(0.01f);
        }

        transform.position = OriginPower;
    }
}
