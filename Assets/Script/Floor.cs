using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : Tatemono
{
    // Start is called before the first frame update
    protected override void Start()
    {
        PlayerDirection = new float[5];

        MaxPillarHeight = 5.0f;
    }
    void FixedUpdate()
    {
        //PlayerObject.transform.forward;
        PlayerDirection[0] = Vector3.Dot(PlayerObject.transform.forward, transform.forward);
        PlayerDirection[1] = -PlayerDirection[0];
        PlayerDirection[2] = Vector3.Dot(PlayerObject.transform.forward, transform.right);
        PlayerDirection[3] = -PlayerDirection[2];
        PlayerDirection[4] = Vector3.Dot(PlayerObject.transform.forward, transform.up);

        FindMaxDummy = float.MinValue;
        FindMaxIndex = int.MinValue;

        for(FindMaxIter = 0; FindMaxIter < 5; FindMaxIter++)
        {
            if (FindMaxDummy < PlayerDirection[FindMaxIter])
            {
                FindMaxDummy = PlayerDirection[FindMaxIter];
                FindMaxIndex = FindMaxIter;
            }
        }

        // Forward
        if (FindMaxIndex == 0)
        {
            transform.position = new Vector3(
                PlayerObject.transform.position.x - PlayerObject.transform.position.x % GapSize,
                PlayerObject.transform.position.y - PlayerObject.transform.position.y % GapSize,
                PlayerObject.transform.position.z - PlayerObject.transform.position.z % GapSize + GapSize
            );
        }
        // Back
        else if (FindMaxIndex == 1)
        {
            transform.position = new Vector3(
                PlayerObject.transform.position.x - PlayerObject.transform.position.x % GapSize,
                PlayerObject.transform.position.y - PlayerObject.transform.position.y % GapSize,
                PlayerObject.transform.position.z - PlayerObject.transform.position.z % GapSize - GapSize
            );
        }
        // Right
        else if (FindMaxIndex == 2)
        {
            transform.position = new Vector3(
                PlayerObject.transform.position.x - PlayerObject.transform.position.x % GapSize + GapSize,
                PlayerObject.transform.position.y - PlayerObject.transform.position.y % GapSize,
                PlayerObject.transform.position.z - PlayerObject.transform.position.z % GapSize
            );
        }
        // Left
        else if (FindMaxIndex == 3)
        {
            transform.position = new Vector3(
                PlayerObject.transform.position.x - PlayerObject.transform.position.x % GapSize - GapSize,
                PlayerObject.transform.position.y - PlayerObject.transform.position.y % GapSize,
                PlayerObject.transform.position.z - PlayerObject.transform.position.z % GapSize
            );
        }
        else
        {
            transform.position = new Vector3(
                PlayerObject.transform.position.x - PlayerObject.transform.position.x % GapSize,
                PlayerObject.transform.position.y - PlayerObject.transform.position.y % GapSize + GapSize,
                PlayerObject.transform.position.z - PlayerObject.transform.position.z % GapSize
            );
        }

        if (0.0f < PlayerObject.transform.position.z)
        {
            transform.position += new Vector3(0.0f, 0.0f, GapSize);
        }

            transform.rotation = Quaternion.identity;
    }


    protected override void CreateInstance()
    {
        GameObject instance = Instantiate(gameObject, transform.position, transform.rotation);

        instance.transform.parent = null;
        instance.GetComponent<BoxCollider>().isTrigger = false;
        instance.GetComponent<Floor>().enabled = false;

        for (int FloorIter = 0; FloorIter < TatemonoObject.Length; FloorIter++)
        {
            instance.GetComponent<Floor>().TatemonoObject[FloorIter].GetComponent<Renderer>().material.color = Color.white;
            instance.GetComponent<Floor>().TatemonoObject[FloorIter].GetComponent<Renderer>().material.mainTexture = PostCreationMaterial;
        }
    }
}
