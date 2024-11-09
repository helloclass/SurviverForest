using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HIveController : MonoBehaviour
{
    public GameObject BeeClusterObject;

    // Update is called once per frame
    public void BeeThrow()
    {
        StartCoroutine(ThrowBeeTimer());
    }

    IEnumerator ThrowBeeTimer()
    {
        yield return new WaitForSeconds(1.0f);

        BeeClusterObject.SetActive(true);
        BeeClusterObject.transform.parent = null;
    }

    IEnumerator DeleteTimer()
    {
        yield return new WaitForSeconds(5.0f);

        Destroy(gameObject);
    }
}
