using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RabbitAnimationTree
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

    public bool isDead;
    public bool isAttack;
    public bool isRecognize;
    public float TurnAxis;
    public RabbitAnimationTree(Animator animator)
    {
        this.animator = animator;

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
        if(isTrigger)
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

                Debug.Log("Action: " + anim);

                animator.CrossFade(anim, transitionDuration);
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
                if (!(isRecognize | isAttack | isDead))
                    continue;
            }

            //Debug.Log("Action: ");
            //foreach (string name in animName)
            //{
            //    Debug.Log(name);
            //}

            selector = Random.Range(0, 5);
            if (selector == 0 || selector == 1)
            {
                loopCount = Random.Range(1, 3);
                for (int i = 0; i < loopCount; i++)
                {
                    animName.Enqueue("Be_Idle 01");
                }

                selector = Random.Range(0, 4);

                if (selector == 0)
                {
                    loopCount = Random.Range(1, 1);
                    for (int i = 0; i < loopCount; i++)
                    {
                        animName.Enqueue("Be_Idle Look Left");
                    }

                }
                else if (selector == 1)
                {
                    loopCount = Random.Range(1, 1);
                    for (int i = 0; i < loopCount; i++)
                    {
                        animName.Enqueue("Be_Dig");
                    }

                }
                else if (selector == 2)
                {
                    loopCount = Random.Range(1, 1);
                    for (int i = 0; i < loopCount; i++)
                    {
                        animName.Enqueue("Be_Idle Smell");
                    }

                }
                else if (selector == 3)
                {
                    loopCount = Random.Range(1, 1);
                    for (int i = 0; i < loopCount; i++)
                    {
                        animName.Enqueue("Be_Idle to Stand");
                    }
                    loopCount = Random.Range(1, 3);
                    for (int i = 0; i < loopCount; i++)
                    {
                        animName.Enqueue("Be_Stand Idle01");
                    }

                    selector = Random.Range(0, 2);
                    if (selector == 0)
                    {
                        loopCount = Random.Range(1, 1);
                        for (int i = 0; i < loopCount; i++)
                        {
                            animName.Enqueue("Be_Stand Idle Scratch Neck");
                        }

                    }
                    else if (selector == 1)
                    {
                        loopCount = Random.Range(1, 1);
                        for (int i = 0; i < loopCount; i++)
                        {
                            animName.Enqueue("Be_Stand Roar");
                        }

                    }

                    loopCount = Random.Range(1, 1);
                    for (int i = 0; i < loopCount; i++)
                    {
                        animName.Enqueue("Be_Stand to Idle");
                    }

                }

            }
            else if (selector == 2 || selector == 3)
            {
                loopCount = Random.Range(2, 4);
                for (int i = 0; i < loopCount; i++)
                {
                    animName.Enqueue("Be_Walk");
                }

                loopCount = Random.Range(2, 4);
                for (int i = 0; i < loopCount; i++)
                {
                    animName.Enqueue("Be_Walk Left");
                }

                loopCount = Random.Range(2, 4);
                for (int i = 0; i < loopCount; i++)
                {
                    animName.Enqueue("Be_Walk Right");
                }
            }
            else if (selector == 4)
            {
                loopCount = Random.Range(1, 3);
                for (int i = 0; i < loopCount; i++)
                {
                    animName.Enqueue("Be_Idle 01");
                }

                loopCount = Random.Range(1, 1);
                for (int i = 0; i < loopCount; i++)
                {
                    animName.Enqueue("Be_Idle to Seat");
                }

                loopCount = Random.Range(2, 4);
                for (int i = 0; i < loopCount; i++)
                {
                    animName.Enqueue("Be_Seat Idle 01");
                }

                selector = Random.Range(0, 2);
                if (selector == 0)
                {
                    loopCount = Random.Range(1, 1);
                    for (int i = 0; i < loopCount; i++)
                    {
                        animName.Enqueue("Be_Seat Look Around");
                    }

                }
                else if (selector == 1)
                {
                    loopCount = Random.Range(1, 1);
                    for (int i = 0; i < loopCount; i++)
                    {
                        animName.Enqueue("Be_Seat to Sleep");
                    }
                    loopCount = Random.Range(2, 4);
                    for (int i = 0; i < loopCount; i++)
                    {
                        animName.Enqueue("Be_Sleep_Idle");
                    }
                    loopCount = Random.Range(1, 1);
                    for (int i = 0; i < loopCount; i++)
                    {
                        animName.Enqueue("Be_Sleep to Seat");
                    }

                }

                loopCount = Random.Range(1, 1);
                for (int i = 0; i < loopCount; i++)
                {
                    animName.Enqueue("Be_Seat to Idle");
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
                        animName.Enqueue("Be_Run");
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
                //        animName.Enqueue("Be_Run Left");
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
                //        animName.Enqueue("Be_Run Right");
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

                selector = Random.Range(0, 5);
                if (selector == 0)
                {
                    loopCount = Random.Range(1, 1);
                    for (int i = 0; i < loopCount; i++)
                    {
                        animName.Enqueue("Be_Attack Bite Left");
                    }

                }
                else if (selector == 1)
                {
                    loopCount = Random.Range(1, 1);
                    for (int i = 0; i < loopCount; i++)
                    {
                        animName.Enqueue("Be_Attack Paw Left");
                    }

                }
                else if (selector == 2)
                {
                    loopCount = Random.Range(1, 1);
                    for (int i = 0; i < loopCount; i++)
                    {
                        animName.Enqueue("Be_Attack Paws");
                    }

                }
                else if (selector == 3)
                {
                    loopCount = Random.Range(1, 1);
                    for (int i = 0; i < loopCount; i++)
                    {
                        animName.Enqueue("Be_Attack Bite Right");
                    }

                }
                else if (selector == 4)
                {
                    loopCount = Random.Range(1, 1);
                    for (int i = 0; i < loopCount; i++)
                    {
                        animName.Enqueue("Be_Attack Paw Right");
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
            if (isDead == true)
            {
                animName.Clear();

                selector = Random.Range(0, 4);
                if (selector == 0)
                {
                    loopCount = Random.Range(1, 1);
                    for (int i = 0; i < loopCount; i++)
                    {
                        animName.Enqueue("Be_Death1");
                    }

                }
                else if (selector == 1)
                {
                    loopCount = Random.Range(1, 1);
                    for (int i = 0; i < loopCount; i++)
                    {
                        animName.Enqueue("Be_Death2");
                    }

                }
                else if (selector == 2)
                {
                    loopCount = Random.Range(1, 1);
                    for (int i = 0; i < loopCount; i++)
                    {
                        animName.Enqueue("Be_Death1 Mirror");
                    }

                }
                else if (selector == 3)
                {
                    loopCount = Random.Range(1, 1);
                    for (int i = 0; i < loopCount; i++)
                    {
                        animName.Enqueue("Be_Death2 Mirror");
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
        }

    }

}
