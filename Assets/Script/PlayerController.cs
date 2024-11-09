using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject MainCamera;

    public float Speed = 0.05f;
    public float JumpSpeed = 1.0f;

    private bool isJump = false;
    public bool isMouseLock = true;

    private bool isRide = false;
    private GameObject RideSeatObject;

    public float Sensitivity = 1.0f;

    Rigidbody PlayerRigidBody = null;

    // 총기에 의한 반동 구현
    Vector3 LeftRecoilDegree;

    // Inventory
    public int MaxItemSlotNum = 36;
    public string[] ItemNameList;
    public int[] ItemNumList;

    // 무기 리스트
    int HandleObjectIndex = 0;
    public GameObject[] HandleObjectList;

    // 체력 State
    public const float HP_State_Max_Value = 1000.0f;
    public const float Thirsty_State_Max_Value = 1000.0f;
    public const float Hungry_State_Max_Value = 1000.0f;

    public float HP_State_Value;
    public float Thirsty_State_Value;
    public float Hungry_State_Value;

    // Start is called before the first frame update
    void Start()
    {
        HandleObjectIndex = 0;
        //HandleObjectList[0].SetActive(true);

        PlayerRigidBody         = GetComponent<Rigidbody>();

        LeftRecoilDegree        = new Vector3(0.0f, 0.0f, 0.0f);

        ItemNameList            = new string[MaxItemSlotNum];
        ItemNumList             = new int[MaxItemSlotNum];

        HP_State_Value = HP_State_Max_Value;
        Thirsty_State_Value = Thirsty_State_Max_Value;
        Hungry_State_Value = Hungry_State_Max_Value;

    StartCoroutine(ClampPlayerRotation());
    }

    private void Update()
    {
        //HP_State_Value = HP_State_Value - Time.deltaTime * 30.0f;

        if (isRide)
        {
            transform.position = RideSeatObject.transform.position;
            transform.rotation = RideSeatObject.transform.rotation;

            //transform.GetChild(0).transform.LookAt(RideSeatObject.transform.forward);

            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Physics.Raycast(transform.position, -Vector3.up * 0.1f))
            {
                isJump = false;

                PlayerRigidBody.velocity =
                    new Vector3(0.0f, JumpSpeed, 0.0f);
            }
        }

        if (Input.GetKey(KeyCode.W))
        {
            PlayerRigidBody.MovePosition(PlayerRigidBody.position + Speed * MainCamera.transform.forward * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            PlayerRigidBody.MovePosition(PlayerRigidBody.position - Speed * MainCamera.transform.forward * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.A))
        {
            PlayerRigidBody.MovePosition(PlayerRigidBody.position - Speed * MainCamera.transform.right * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            PlayerRigidBody.MovePosition(PlayerRigidBody.position + Speed * MainCamera.transform.right * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            HandleObjectList[HandleObjectIndex].SetActive(false);

            HandleObjectIndex = (HandleObjectIndex + 1) < HandleObjectList.Length ?
                (HandleObjectIndex + 1) : 0;

            if (HandleObjectList[HandleObjectIndex] != null)
                HandleObjectList[HandleObjectIndex].SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            HandleObjectList[HandleObjectIndex].SetActive(false);

            HandleObjectIndex = 0 <= (HandleObjectIndex - 1) ?
                (HandleObjectIndex - 1) : HandleObjectList.Length - 1;

            if (HandleObjectList[HandleObjectIndex] != null)
                HandleObjectList[HandleObjectIndex].SetActive(true);
        }

        //// For Gun Recoil
        //if (0.01f < LeftRecoilDegree.magnitude)
        //{
        //    PlayerRigidBody.rotation = Quaternion.Euler(PlayerRigidBody.rotation.eulerAngles + LeftRecoilDegree);

        //    LeftRecoilDegree = LeftRecoilDegree * 0.25f;
        //}

        Vector3 CameraRotation = MainCamera.transform.rotation.eulerAngles;
        MainCamera.transform.rotation = Quaternion.Euler(
            CameraRotation.x, CameraRotation.y, 0.0f
        );
    }

    private void OnDestroy()
    {
        StopCoroutine(ClampPlayerRotation());
    }

    IEnumerator ClampPlayerRotation()
    {
        while (true)
        {
            Vector3 DeltaRotation = Vector3.zero;

            //MousePositionDelta = Input.mousePosition - PreviousMousePositionDelta;
            //PreviousMousePositionDelta = Input.mousePosition;

            DeltaRotation.y = Input.GetAxis("Mouse X") * Sensitivity;
            DeltaRotation.x = -Input.GetAxis("Mouse Y") * Sensitivity;

            if (0.01f < LeftRecoilDegree.magnitude)
            {
                DeltaRotation += LeftRecoilDegree;
                LeftRecoilDegree = LeftRecoilDegree * 0.2f;
            }

            //transform.rotation = Quaternion.Euler(0.0f, DeltaRotation.y, 0.0f);
            //MainCamera.transform.rotation = Quaternion.Euler(DeltaRotation);

            transform.Rotate(new Vector3(0.0f, DeltaRotation.y, 0.0f));
            MainCamera.transform.Rotate(new Vector3(DeltaRotation.x, DeltaRotation.y, 0.0f));

            yield return null;
        }
    }

    public void GunRecoil(Vector3 RecoilDegree)
    {
        LeftRecoilDegree += RecoilDegree;
    }

    public void ThrowedHandleObject()
    {
        HandleObjectList[HandleObjectIndex].SetActive(false);

        HandleObjectIndex = 0;

        HandleObjectList[HandleObjectIndex].SetActive(true);

    }

    // 아이템 습득
    public bool AppendItem(string ItemName)
    {
        int EmptyIndex = -1;

        // 같은 아이템을 가지고 있다면 가지고 있는 개수(Item Num)을 1 더한다.
        for (int iter = 0; iter < MaxItemSlotNum; iter++)
        {
            if (ItemNameList[iter] == ItemName)
            {
                ItemNumList[iter] += 1;
                return true;
            }
        }
        // 가지고 있지 않은 아이템이면 비어있는 슬롯에 아이템을 추가한다
        if (0 <= EmptyIndex)
        {
            ItemNameList[EmptyIndex] = ItemName;
            ItemNumList[EmptyIndex] = 1;

            return true;
        }
        // 인벤이 가득 찼으면 추가를 못한다.
        return false;
    }

    // 아이템 사용
    public void UseItem(int SlotIndex)
    {
        ItemNumList[SlotIndex] = ItemNumList[SlotIndex] - 1;

        if (ItemNumList[SlotIndex] == 0)
        {
            ItemNameList[SlotIndex] = "";
            ItemNumList[SlotIndex] = 0;
        }
    }

    // 아이템 드랍
    public void DropItem(int SlotIndex)
    {
        if (SlotIndex < MaxItemSlotNum)
        {
            ItemNameList[SlotIndex] = "";
            ItemNumList[SlotIndex] = 0;
        }
    }

    public void RideToVehicle(GameObject vehicle)
    {
        isRide = true;

        GetComponent<CapsuleCollider>().isTrigger = true;
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Rigidbody>().mass = 0.0f;

        RideSeatObject = vehicle;
    }

    public void ExitToVehicle(GameObject exit)
    {
        isRide = false;

        GetComponent<CapsuleCollider>().isTrigger = false;
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().mass = 70.0f;

        transform.position = exit.transform.position;
        transform.parent = null;
    }
}
