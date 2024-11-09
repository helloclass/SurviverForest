using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// HackSawPlayer.cs (Can't Throw / Warning!!!)

public class TwoSwingBuki : Buki
{
    public override IEnumerator CalcNextClip()
    {
        AnimatorStateInfo animInfo;
        bool normalTime;
        bool hitNormalTime;

        while (true)
        {
            animInfo = BodyAnimator.GetCurrentAnimatorStateInfo(0);
            hitNormalTime = (TimeOutOfHit <= animInfo.normalizedTime);

            isAnimationDone1 =
                (animInfo.IsName("attack-1"));

            isAnimationDone2 =
                (animInfo.IsName("attack-2"));

            isHitDone1 =
                animInfo.IsName("attack-1_hit") &&
                hitNormalTime;

            isHitDone2 =
                animInfo.IsName("attack-2_hit") &&
                hitNormalTime;

            if (throwObject.isHit)
            {
                if (isAnimationDone1 || isAnimationDone2)
                {
                    BodyAnimator.SetBool("IsAttack1", false);
                    BodyAnimator.SetBool("IsAttack2", false);
                    BodyAnimator.SetBool("IsHit", true);
                }
            }

            yield return null;
        }
    }

    public override IEnumerator UpdateAnimator()
    {
        while (true)
        {
            BodyAnimator.SetBool("IsTimeOut", false);
            //BodyAnimator.SetBool("IsHit", false);

            if (Input.GetMouseButtonDown(0))
            {
                if (isThrowHold)
                {
                    isThrowHold = false;
                    BodyAnimator.SetBool("IsThrowHold", false);
                    BodyAnimator.SetBool("IsThrowCancle", true);
                    HandleThrowBody.GetComponent<BoxCollider>().enabled = false;
                }

                if (!isAttack1)
                {
                    CheckTimeOut = Time.time;

                    isAttack1 = true;

                    BodyAnimator.SetBool("IsAttack1", true);
                    BodyAnimator.SetBool("IsAttack2", false);
                    BodyAnimator.SetBool("IsHit", false);

                    HandleThrowBody.GetComponent<BoxCollider>().enabled = true;
                }
                else if (!isAttack2)
                {
                    if (throwObject.isHit)
                    {
                        //HandleThrowBody.GetComponent<BoxCollider>().enabled = false;

                        if (isHitDone1)
                        {
                            throwObject.isHit = false;
                            isAttack2 = true;

                            BodyAnimator.SetBool("IsAttack1", false);
                            BodyAnimator.SetBool("IsAttack2", true);
                            BodyAnimator.SetBool("IsHit", false);

                            HandleThrowBody.GetComponent<BoxCollider>().enabled = true;
                        }
                    }
                }
                else
                {
                    if (throwObject.isHit)
                    {
                        //HandleThrowBody.GetComponent<BoxCollider>().enabled = false;

                        if (isHitDone2)
                        {
                            throwObject.isHit = false;

                            isAttack1 = false;
                            isAttack2 = false;

                            BodyAnimator.SetBool("IsAttack1", false);
                            BodyAnimator.SetBool("IsAttack2", false);
                            BodyAnimator.SetBool("IsHit", false);
                        }
                    }
                    else if (isHitDone2)
                    {
                        throwObject.isHit = false;

                        isAttack1 = false;
                        isAttack2 = false;

                        BodyAnimator.SetBool("IsAttack1", false);
                        BodyAnimator.SetBool("IsAttack2", false);
                        BodyAnimator.SetBool("IsHit", false);
                    }
                }
            }
            else
            {
                if (TimeOutLimit < (Time.time - CheckTimeOut))
                {
                    throwObject.isHit = false;

                    isAttack1 = false;
                    isAttack2 = false;

                    HandleThrowBody.GetComponent<BoxCollider>().enabled = false;

                    BodyAnimator.SetBool("IsAttack1", false);
                    BodyAnimator.SetBool("IsAttack2", false);
                    BodyAnimator.SetBool("IsHit", false);

                    BodyAnimator.SetBool("IsTimeOut", true);
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
