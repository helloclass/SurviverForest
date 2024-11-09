using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : MonoBehaviour
{
    bool isFire = false;

    private void OnTriggerStay(Collider collision)
    {
        if (isFire && collision.gameObject.tag == "Enemy")
        {
            // Hurt
            Enemy enemy = collision.gameObject.transform.root.GetComponent<Enemy>();

            if (enemy)
            {
                enemy.Damaging(50);
            }
        }
    }

    public void Fire()
    {
        isFire = true;
        StartCoroutine("DeleteTimer");

        GetComponent<Rigidbody>().isKinematic = false;

        Vector3 PistolVelocity = transform.forward * 100.0f;
        GetComponent<Rigidbody>().velocity = PistolVelocity;
    }

    IEnumerator DeleteTimer()
    {
        yield return new WaitForSeconds(3.0f);

        Destroy(gameObject);
    }
}
