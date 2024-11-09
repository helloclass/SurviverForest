using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cleaver : ThrowObject
{
    public override void Start()
    {
        FireSpeed = 100.0f;
        DamageValue = 50.0f;
    }

    public override void OnTriggerEnter(Collider collision)
    {
        base.OnTriggerEnter(collision);

        if (collision.gameObject.tag == "Tree")
        {
            Vector4[] CutterPannel = new Vector4[7];

            float Height = 0.0f;

            RaycastHit hit;
            Vector3 rayPos = transform.position;

            if (Physics.Raycast(rayPos, Vector3.down, out hit, Mathf.Infinity))
            {
                if (hit.transform.tag == "Tile")
                {
                    Height = rayPos.y - hit.point.y;
                    Height *= 0.5f;
                }
            }

            CutterPannel[0] = new Vector4(300.0f, 0.0f, 300.0f, 1.0f);
            CutterPannel[1] = new Vector4(-300.0f, 0.0f, 300.0f, 1.0f);
            CutterPannel[2] = new Vector4(-300.0f, 0.0f, -300.0f, 1.0f);
            CutterPannel[3] = new Vector4(300.0f, 0.0f, -300.0f, 1.0f);

            CutterPannel[4] = new Vector4(transform.position.x, Height, transform.position.z, 1.0f);
            CutterPannel[5] = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);

            CutterPannel[6] = new Vector4(transform.right.x, transform.right.y, transform.right.z, 1.0f);

            collision.gameObject.GetComponent<Test_ComputeShader>().Chopping(CutterPannel);
        }
    }
}
