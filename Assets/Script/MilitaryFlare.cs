using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 

public class MilitaryFlare : Object
{
    bool isFire = false;

    public GameObject FireDirectionObject;
    public GameObject ExplosionObject;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnTriggerEnter(Collider collision)
    {
        if (isFire && collision.gameObject.tag == "Enemy")
        {
            // Hurt
            Enemy enemyObject = collision.gameObject.transform.root.GetComponent<Enemy>();

            if (enemyObject)
            {
                enemyObject.GetFired();
                enemyObject.Damaging(5);
            }
        }
    }

    public void Fire(Vector3 FireForward)
    {
        isFire = true;
        StartCoroutine("DeleteTimer");

        ExplosionObject.GetComponent<ParticleSystem>().Play();

        //GetComponent<Rigidbody>().transform.rotation = Quaternion.Euler(FireForward);
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().velocity = transform.forward * 50.0f;

        GetComponent<BoxCollider>().enabled = true;
    }

    IEnumerator DeleteTimer()
    {
        yield return new WaitForSeconds(3.0f);

        //ExplosionObject.transform.parent = null;
        //ExplosionObject.GetComponent<SelfDestruct>().StartDestoryTimer();

        Destroy(gameObject);
    }
}
