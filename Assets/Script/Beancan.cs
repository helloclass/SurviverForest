using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beancan : Object
{
    bool isFire = false;

    public GameObject FireDirectionObject;
    public GameObject ExplosionObject;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Fire(Vector3 FireForward)
    {
        isFire = true;
        StartCoroutine("DeleteTimer");

        GetComponent<Rigidbody>().transform.rotation = Quaternion.Euler(FireForward);
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().velocity = FireDirectionObject.transform.forward * 15.0f;

        GetComponent<BoxCollider>().enabled = true;
    }

    IEnumerator DeleteTimer()
    {
        yield return new WaitForSeconds(3.0f);

        ExplosionObject.transform.parent = null;

        ExplosionObject.GetComponent<ParticleSystem>().Play();
        ExplosionObject.GetComponent<SelfDestruct>().StartDestoryTimer();

        Destroy(gameObject);
    }
}
