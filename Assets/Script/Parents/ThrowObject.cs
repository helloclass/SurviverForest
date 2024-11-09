using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowObject : Object, ItemInterface
{
    bool isFire = false;
    bool isGetStuck = false;

    public bool isHit = false;

    public float FireSpeed = 20.0f;
    public float DamageValue = 5.0f;

    Vector3 MaceFireForward;

    // Start is called before the first frame update
    public virtual void Start()
    {
        isHit = false;

        ObjectName = gameObject.name;
    }

    public virtual void FixedUpdate()
    {
        if (isFire && !isGetStuck)
        {
            GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity - MaceFireForward * Time.deltaTime * 15.0f;
            GetComponent<Rigidbody>().transform.Rotate(new Vector3(90.0f, 0.0f, 0.0f) * Time.deltaTime * 15.0f);

        }
    }

    public virtual void OnTriggerEnter(Collider collision)
    {
        if (!isGetStuck && collision.gameObject.tag == "Enemy")
        {
            // Hurt
            Enemy enemyObject = collision.gameObject.transform.root.GetComponent<Enemy>();

            if (enemyObject)
            {
                if (isFire)
                {
                    isGetStuck = true;

                    GetComponent<Rigidbody>().useGravity = false;
                    GetComponent<Rigidbody>().velocity = Vector3.zero;
                    transform.parent = collision.gameObject.transform;
                }

                isHit = true;
                enemyObject.Damaging(DamageValue);
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

    public void Fire(Vector3 FireForward)
    {
        isFire = true;

        StartCoroutine(DeleteTimer());

        MaceFireForward = FireForward;

        GetComponent<Rigidbody>().transform.localScale = new Vector3(150.0f, 150.0f, 150.0f);

        //GetComponent<Rigidbody>().transform.LookAt(MaceFireForward);
        GetComponent<Rigidbody>().transform.rotation =
            Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f));

        GetComponent<Rigidbody>().transform.localRotation =
            Quaternion.Euler(new Vector3(0.0f, 90.0f, 0.0f));

        GetComponent<Rigidbody>().isKinematic = false;

        GetComponent<Rigidbody>().velocity = MaceFireForward * 30.0f;

    }

    IEnumerator DeleteTimer()
    {
        yield return new WaitForSeconds(5.0f);

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
