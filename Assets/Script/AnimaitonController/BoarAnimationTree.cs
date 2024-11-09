using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoarAnimationTree
{
    public Animator animator;
    float prevAnimTime, curAnimTime;

    const float transitionDuration = 0.25f;
    const float boundAnimTime = 0.9f;

    int selector = new int();
    int loopCount = new int();
    Queue<string> animName = new Queue<string>();

    bool isTrigger;
    int CurrentPhaseIndex;

    bool isPass, isPass1;

    public bool isDead;
    public bool isHurt;
    public bool isAttack;
    public bool isRecognize;
    public float TurnAxis;
    public BoarAnimationTree(Animator animator)
    {
        this.animator = animator;

        isPass = false;
        isPass1 = false;

        isTrigger = false;
        CurrentPhaseIndex = 0;

        prevAnimTime = 0;
        curAnimTime = 0;
    }

    public void AnimationStart()
    {
        animator.Play(animName.Dequeue());
    }

    public void AnimationUpdate()
    {
        if (isPass)
        {
            animator.speed = 0.0f;
            return;
        }

        if (isTrigger)
        {
            isTrigger = false;

            string anim;

            if (0 < animName.Count)
            {
                anim = animName.Dequeue();
                animator.CrossFade(anim, transitionDuration);
            }
        }

        curAnimTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        if (prevAnimTime < boundAnimTime && boundAnimTime <= curAnimTime)
        {
            string anim;

            if (0 < animName.Count)
            {
                anim = animName.Dequeue();

                animator.CrossFade(anim, transitionDuration);
            }
            else if (isPass1)
            {
                isPass = true;
            }
        }

        prevAnimTime = curAnimTime;
    }

    public IEnumerator AnimUpdate()
    {
        while (true)
        {
            if (0 < animName.Count)
            {
                yield return new WaitForSeconds(0.1f);
            }

            if (30 < animName.Count)
            {
                // If the state is IDLE.
                if (!(isRecognize | isAttack | isHurt | isDead))
                    continue;
            }

            selector = Random.Range(0, 5);
            if (selector == 0 || selector == 1)
            {
                loopCount = Random.Range(1, 3);
                for (int i = 0; i < loopCount; i++)
                {
                    animName.Enqueue("BIdle");
                }

                selector = Random.Range(0, 4);

                if (selector == 0)
                {
                    loopCount = Random.Range(1, 1);
                    for (int i = 0; i < loopCount; i++)
                    {
                        animName.Enqueue("BIdle Big Shake");
                    }

                }
                else if (selector == 1)
                {
                    loopCount = Random.Range(1, 1);
                    for (int i = 0; i < loopCount; i++)
                    {
                        animName.Enqueue("BIdle Look");
                    }

                }
                else if (selector == 2)
                {
                    loopCount = Random.Range(1, 1);
                    for (int i = 0; i < loopCount; i++)
                    {
                        animName.Enqueue("BIdle SmallShake");
                    }

                }
                else if (selector == 3)
                {
                    selector = Random.Range(0, 2);
                    if (selector == 0)
                    {
                        loopCount = Random.Range(1, 1);
                        for (int i = 0; i < loopCount; i++)
                        {
                            animName.Enqueue("BDrink");
                        }

                    }
                    else if (selector == 1)
                    {
                        loopCount = Random.Range(1, 1);
                        for (int i = 0; i < loopCount; i++)
                        {
                            animName.Enqueue("BEat");
                        }

                    }

                }

            }
            else if (selector == 2 || selector == 3)
            {
                loopCount = Random.Range(2, 4);
                for (int i = 0; i < loopCount; i++)
                {
                    animName.Enqueue("BWalk");
                }

                loopCount = Random.Range(2, 4);
                for (int i = 0; i < loopCount; i++)
                {
                    animName.Enqueue("BWalk L");
                }

                loopCount = Random.Range(2, 4);
                for (int i = 0; i < loopCount; i++)
                {
                    animName.Enqueue("BWalk R");
                }
            }
            else if (selector == 4)
            {
                loopCount = Random.Range(1, 3);
                for (int i = 0; i < loopCount; i++)
                {
                    animName.Enqueue("BIdle");
                }

                loopCount = Random.Range(1, 1);
                for (int i = 0; i < loopCount; i++)
                {
                    animName.Enqueue("BIdle to Sit");
                }

                loopCount = Random.Range(2, 4);
                for (int i = 0; i < loopCount; i++)
                {
                    animName.Enqueue("BSit");
                }

                loopCount = Random.Range(1, 1);
                for (int i = 0; i < loopCount; i++)
                {
                    animName.Enqueue("BSit to Sleep");
                }
                loopCount = Random.Range(2, 4);
                for (int i = 0; i < loopCount; i++)
                {
                    animName.Enqueue("BSleep");
                }
                loopCount = Random.Range(1, 1);
                for (int i = 0; i < loopCount; i++)
                {
                    animName.Enqueue("BSleep to Idle");
                }

            }
            if (isRecognize == true)
            {
                animName.Clear();

                //if (-5.0f <= TurnAxis && TurnAxis < 5.0f)
                //{
                    loopCount = Random.Range(1, 1);
                    for (int i = 0; i < loopCount; i++)
                    {
                        animName.Enqueue("BRun");
                    }
                //}
                //else
                //{
                //    continue;

                //}
                //if (TurnAxis < -5.0f)
                //{
                //    loopCount = Random.Range(1, 1);
                //    for (int i = 0; i < loopCount; i++)
                //    {
                //        animName.Enqueue("B Run L");
                //    }
                //}
                //else
                //{
                //    continue;

                //}
                //if (5.0f <= TurnAxis)
                //{
                //    loopCount = Random.Range(1, 1);
                //    for (int i = 0; i < loopCount; i++)
                //    {
                //        animName.Enqueue("B Run R");
                //    }
                //}
                //else
                //{
                //    continue;

                //}

            }
            else
            {
                if (CurrentPhaseIndex != 0)
                {
                    isTrigger = true;
                    CurrentPhaseIndex = 0;
                }

                continue;

            }
            if (isAttack == true)
            {
                animName.Clear();

                if (CurrentPhaseIndex == 1)
                {
                    loopCount = Random.Range(1, 1);
                    for (int i = 0; i < loopCount; i++)
                    {
                        animName.Enqueue("BAttack Run");
                    }
                }
                else
                {
                    selector = Random.Range(0, 4);
                    if (selector == 0)
                    {
                        loopCount = Random.Range(1, 1);
                        for (int i = 0; i < loopCount; i++)
                        {
                            animName.Enqueue("BAttack Bite");
                        }

                    }
                    else if (selector == 1)
                    {
                        loopCount = Random.Range(1, 1);
                        for (int i = 0; i < loopCount; i++)
                        {
                            animName.Enqueue("BAttack Fangs 1");
                        }

                    }
                    else if (selector == 2)
                    {
                        loopCount = Random.Range(1, 1);
                        for (int i = 0; i < loopCount; i++)
                        {
                            animName.Enqueue("BAttack Fangs 2");
                        }

                    }
                    else if (selector == 3)
                    {
                        loopCount = Random.Range(1, 1);
                        for (int i = 0; i < loopCount; i++)
                        {
                            animName.Enqueue("BAttack Fangs Front");
                        }

                    }
                }
            }
            else
            {
                if (CurrentPhaseIndex != 1)
                {
                    isTrigger = true;
                    CurrentPhaseIndex = 1;
                }

                continue;

            }

            if (isHurt == true)
            {
                selector = Random.Range(0, 2);
                if (selector == 0)
                {
                    loopCount = Random.Range(1, 1);
                    for (int i = 0; i < loopCount; i++)
                    {
                        animName.Enqueue("BGetHit Front L");
                    }

                }
                else if (selector == 1)
                {
                    loopCount = Random.Range(1, 1);
                    for (int i = 0; i < loopCount; i++)
                    {
                        animName.Enqueue("BGetHit Front R");
                    }

                }
            }
            else
            {
                if (CurrentPhaseIndex != 2)
                {
                    isTrigger = true;
                    CurrentPhaseIndex = 2;
                }

                continue;

            }

            if (isDead == true)
            {
                animName.Clear();

                selector = Random.Range(0, 4);
                if (selector == 0)
                {
                    loopCount = Random.Range(1, 1);
                    for (int i = 0; i < loopCount; i++)
                    {
                        animName.Enqueue("BDeath 2");
                    }

                }
                else if (selector == 1)
                {
                    loopCount = Random.Range(1, 1);
                    for (int i = 0; i < loopCount; i++)
                    {
                        animName.Enqueue("BDeath Side");
                    }

                }
                else if (selector == 2)
                {
                    loopCount = Random.Range(1, 1);
                    for (int i = 0; i < loopCount; i++)
                    {
                        animName.Enqueue("BDeath 2 Opp");
                    }

                }
                else if (selector == 3)
                {
                    loopCount = Random.Range(1, 1);
                    for (int i = 0; i < loopCount; i++)
                    {
                        animName.Enqueue("BDeath Side Opp");
                    }

                }

                isPass1 = true;
                break;
            }
            else
            {
                if (CurrentPhaseIndex != 3)
                {
                    isTrigger = true;
                    CurrentPhaseIndex = 3;
                }

                continue;

            }
        }

    }

}
