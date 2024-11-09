using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour
{
    bool isBroken;

    // Start is called before the first frame update
    void Start()
    {
        isBroken = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            StoneBrake();
        }
    }

    void StoneBrake()
    {
        for (int cIDX = 0; cIDX < transform.childCount; cIDX++)
        {
            transform.GetChild(cIDX).GetComponent<Rigidbody>().isKinematic = false;
        }
    }
}
