using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeancanGrenadePlayer : Buki
{
    public GameObject ThrowPositionObject;
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isThrowHold = true;
            BodyAnimator.SetBool("IsThrow", false);

            BodyAnimator.SetBool("IsThrowHold", true);
        }
    }

    // 화살, 총알 등의 소모품들을 전부 다 사용하였을때
    public override void ThrowObjectIsEmpty()
    {
        HandPlayer.GetComponent<PlayerController>().ThrowedHandleObject();
    }

    // 화살, 총알 등의 소모품들을 충전했을 때
    public override void ChargingThrowObject()
    {

    }

    public override IEnumerator CalcNextClip()
    {
        yield return new WaitForSeconds(0.1f);
    }
    public override IEnumerator UpdateAnimator()
    {
        while (true)
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (isThrowHold)
                {
                    isThrowHold = false;
                    isThrow = true;
                    BodyAnimator.SetBool("IsThrowHold", false);
                    BodyAnimator.SetBool("IsThrow", true);
                }
            }

            yield return null;
        }
    }

    public void ThrowGrenade()
    {
        GameObject ThrowObject = Instantiate(HandleThrowBody, ThrowPositionObject.transform);
        ThrowObject.SetActive(true);
        ThrowObject.transform.parent = null;

        ThrowObject.GetComponent<Beancan>().Fire(transform.forward);
    }

    public void ThrowEnd()
    {
        ThrowObjectIsEmpty();
    }
}
