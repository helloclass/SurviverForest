using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneSwingBuki : Buki
{
    public override IEnumerator CalcNextClip()
    {
        yield return null;
    }
    public override IEnumerator UpdateAnimator()
    {
        while (true)
        {
            BodyAnimator.SetBool("IsAttack", false);
            BodyAnimator.SetBool("IsHit", false);

            //isAnimationDone1 =
            //    (BodyAnimator.GetCurrentAnimatorStateInfo(0).IsName("attack") &&
            //    (0.85f <= BodyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime));

            isAnimationDone1 =
                (BodyAnimator.GetCurrentAnimatorStateInfo(0).IsName("attack"));

            if (isAnimationDone1)
            {
                isAnimationDone1 = false;

                HandleThrowBody.GetComponent<BoxCollider>().enabled = false;

                if (HandleThrowBody.GetComponent<ThrowObject>().isHit)
                {
                    HandleThrowBody.GetComponent<ThrowObject>().isHit = false;
                    BodyAnimator.SetBool("IsHit", true);
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (isThrowHold)
                {
                    isThrowHold = false;
                    BodyAnimator.SetBool("IsThrowHold", false);
                    BodyAnimator.SetBool("IsThrowCancle", true);
                }
                else
                {
                    BodyAnimator.SetBool("IsAttack", true);
                    HandleThrowBody.GetComponent<BoxCollider>().enabled = true;
                }
            }

            if (isThrowAble)
            {
                if (Input.GetMouseButtonDown(1))
                {
                    isThrowHold = true;

                    gameManager.postProcessMaterial.SetFloat("MinDepthOfField", 0.001f);
                    gameManager.postProcessMaterial.SetFloat("MaxDepthOfField", -0.01f);
                    if (gameManager.postProcessMaterial.HasProperty("_IsUsedDynamicDoF"))
                    {
                        gameManager.postProcessMaterial.SetFloat("_IsUsedDynamicDoF", 1.0f);
                    }

                    BodyAnimator.SetBool("IsThrow", false);
                    BodyAnimator.SetBool("IsThrowCancle", false);

                    BodyAnimator.SetBool("IsThrowHold", true);
                }
                else if (Input.GetMouseButtonUp(1))
                {
                    if (isThrowHold)
                    {
                        isThrowHold = false;

                        gameManager.postProcessMaterial.SetFloat("MinDepthOfField", 0.001f);
                        gameManager.postProcessMaterial.SetFloat("MaxDepthOfField", 0.01f);
                        if (gameManager.postProcessMaterial.HasProperty("_IsUsedDynamicDoF"))
                        {
                            gameManager.postProcessMaterial.SetFloat("_IsUsedDynamicDoF", 0.0f);
                        }

                        BodyAnimator.SetBool("IsThrowHold", false);
                        BodyAnimator.SetBool("IsThrow", true);

                        GameObject ThrowObject = Instantiate(HandleThrowBody, HandleThrowBody.transform);
                        ThrowObject.transform.parent = null;
                        ThrowObject.GetComponent<BoxCollider>().enabled = true;
                        ThrowObject.GetComponent<ThrowObject>().Fire(transform.forward);

                        // 무기를 던졌으므로 BukiPlayer에서 HandPlayer로 변환 시킨다. 
                        ThrowObjectIsEmpty();
                    }
                }
            }

            yield return null;
        }
    }

    // 화살, 총알 등의 소모품들을 전부 다 사용하였을때
    public override void ThrowObjectIsEmpty()
    {
        HandPlayer.GetComponent<PlayerController>().ThrowedHandleObject();
    }

    public override void ChargingThrowObject()
    {

    }
}
