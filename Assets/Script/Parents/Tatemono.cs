using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    주의 : Parents가 NULL 이어야 함.
*/

public abstract class Tatemono : MonoBehaviour
{
    protected float GapSize = 3.0f;

    public GameObject PlayerObject;

    public GameObject CenterPivot;
    public GameObject[] TatemonoObject;
    public GameObject[] TatemonoCorner;

    public Texture PostCreationMaterial;

    Ray CornerRay;
    RaycastHit CornerRaycastHit;

    protected float MaxPillarHeight;
    protected bool isBuildAbleCauseHeight;
    protected bool isBuildAbleCauseOverlap;

    public float[] PlayerDirection;

    protected float FindMaxDummy;
    protected int FindMaxIndex;
    protected int FindMaxIter;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        CornerRay = new Ray();

        isBuildAbleCauseHeight = true;
        isBuildAbleCauseOverlap = true;
    }

    // Update is called once per frame
    void Update()
    {
        //// Check Is Build Able Height
        //for (int CornerIter = 0; CornerIter < TatemonoCorner.Length; CornerIter++)
        //{
        //    CornerRay.origin = TatemonoCorner[CornerIter].transform.position;
        //    CornerRay.direction = -Vector3.up;

        //    if (Physics.Raycast(CornerRay.origin, CornerRay.direction, out CornerRaycastHit, MaxPillarHeight))
        //    {
        //        isBuildAbleCauseHeight = true;
        //    }
        //    else
        //    {
        //        isBuildAbleCauseHeight = false;
        //    }
        //}

        for (int FloorIter = 0; FloorIter < TatemonoObject.Length; FloorIter++)
        {
            TatemonoObject[FloorIter].GetComponent<Renderer>().material.color = Color.blue;
        }

        // Create When Pushed Return
        //if (Input.GetKeyDown(KeyCode.Return))
        if (Input.GetMouseButtonDown(0))
        {
            CreateInstance();
        }
       

    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Tatemono")
        {
            isBuildAbleCauseOverlap = false;
        }
        else
        {
            isBuildAbleCauseOverlap = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        isBuildAbleCauseOverlap = true;
    }

    protected abstract void CreateInstance();
}
