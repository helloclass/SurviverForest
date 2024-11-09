using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hammer : MonoBehaviour
{
    public bool isFire;

    Vector3 HammerFireForward;
    void Awake()
    {
        isFire = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Update()
    {
        if (isFire)
        {
            GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity - HammerFireForward * Time.deltaTime * 25.0f;
            GetComponent<Rigidbody>().transform.Rotate(new Vector3(90.0f, 0.0f, 0.0f) * Time.deltaTime * 15.0f);
        }
    }

    public void Fire(Vector3 FireForward)
    {
        isFire = true;

        StartCoroutine(DeleteTimer());

        HammerFireForward = FireForward;

        GetComponent<Rigidbody>().transform.LookAt(HammerFireForward);
        GetComponent<Rigidbody>().transform.rotation =
            Quaternion.Euler(
                GetComponent<Rigidbody>().transform.rotation.eulerAngles + new Vector3(0.0f, 90.0f, 0.0f)
            );

        GetComponent<Rigidbody>().isKinematic = false;

        GetComponent<Rigidbody>().velocity = HammerFireForward * 50.0f;

    }

    IEnumerator DeleteTimer()
    {
        yield return new WaitForSeconds(5.0f);

        Destroy(gameObject);
    }
}
