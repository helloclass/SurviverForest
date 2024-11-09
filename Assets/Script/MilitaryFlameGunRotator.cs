using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MilitaryFlameGunRotator : MonoBehaviour
{
    Vector3 test;
    Vector3 test1;

    public void Start()
    {
        test1 = new Vector3(-23.25f, -7.368f, -156.465f);
    }

    public void LateUpdate()
    {
        test = transform.rotation.eulerAngles;
        test = test + test1;

        Debug.Log(test);

        transform.rotation = Quaternion.Euler(test);
    }
}
