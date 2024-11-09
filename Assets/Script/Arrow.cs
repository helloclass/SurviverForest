using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : Object, ItemInterface
{
    bool isFire = false;
    bool isGetStuck = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void FixedUpdate()
    {

    }

    private void OnTriggerStay(Collider collision)
    {
        if (isFire && collision.gameObject.tag == "Enemy")
        {
            // Hurt
            Enemy enemyObject = collision.gameObject.transform.root.GetComponent<Enemy>();

            if (enemyObject)
            {
                isGetStuck = true;
                // 속도를 죽이고
                GetComponent<Rigidbody>().useGravity = false;
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                // 몬스터 몸에 박히도록
                transform.parent = collision.gameObject.transform;

                enemyObject.Damaging(5);
            }
        }
        // reusing arrow
        else if (isGetStuck && collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<Inventory>().AppendItem(this);

            // Get Arrow
            Destroy(gameObject);
        }
    }

    public void Fire()
    {
        isFire = true;
        StartCoroutine("DeleteTimer");

        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<BoxCollider>().enabled = true;

        Vector3 ArrowVelocity = transform.forward * 30.0f;
        GetComponent<Rigidbody>().velocity = ArrowVelocity;
    }

    IEnumerator DeleteTimer()
    {
        yield return new WaitForSeconds(3.0f);

        if (!isGetStuck)
            Destroy(gameObject);
    }

    public int GetIndexInfo()
    {
        return 0;
    }

    public string GetObjectInfo()
    {
        Debug.Log(gameObject.name + " GYAATTO!!!");

        return gameObject.name;
    }

    public string GetTagInfo()
    {
        Debug.Log("tag: " + gameObject.tag);

        return gameObject.tag;
    }
}
