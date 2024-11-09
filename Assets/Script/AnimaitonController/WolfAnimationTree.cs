using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfAnimationTree
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
    public WolfAnimationTree(Animator animator)
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

            selector = Random.Range(0, 5);
            if (selector == 0 || selector == 1)
            {
                loopCount = Random.Range(1, 3);
                for (int i = 0; i < loopCount; i++)
                {
                    animName.Enqueue("F_Idle01");
                }

                selector = Random.Range(0, 3);

                if (selector == 0)
                {
                    loopCount = Random.Range(1, 1);
                    for (int i = 0; i < loopCount; i++)
                    {
                        animName.Enqueue("F_Idle02");
                    }

                }
                else if (selector == 1)
                {
                    loopCount = Random.Range(1, 1);
                    for (int i = 0; i < loopCount; i++)
                    {
                        animName.Enqueue("F_Idle03");
                    }

                }
                else if (selector == 2)
                {
                    selector = Random.Range(0, 3);
                    if (selector == 0)
                    {
                        loopCount = Random.Range(1, 1);
                        for (int i = 0; i < loopCount; i++)
                        {
                            animName.Enqueue("F_Drink");
                        }

                    }
                    else if (selector == 1)
                    {
                        loopCount = Random.Range(1, 1);
                        for (int i = 0; i < loopCount; i++)
                        {
                            animName.Enqueue("F_Eat");
                        }

                    }
                    else if (selector == 2)
                    {
                        loopCount = Random.Range(1, 1);
                        for (int i = 0; i < loopCount; i++)
                        {
                            animName.Enqueue("F_Crawl");
                        }

                    }

                }

            }
            else if (selector == 2 || selector == 3)
            {
                loopCount = Random.Range(2, 4);
                for (int i = 0; i < loopCount; i++)
                {
                    animName.Enqueue("F_Walk");
                }

                loopCount = Random.Range(2, 4);
                for (int i = 0; i < loopCount; i++)
                {
                    animName.Enqueue("F_Walk_Left");
                }

                loopCount = Random.Range(2, 4);
                for (int i = 0; i < loopCount; i++)
                {
                    animName.Enqueue("F_Walk_Right");
                }
            }
            else if (selector == 4)
            {
                loopCount = Random.Range(1, 3);
                for (int i = 0; i < loopCount; i++)
                {
                    animName.Enqueue("F_Idle01");
                }

                loopCount = Random.Range(1, 1);
                for (int i = 0; i < loopCount; i++)
                {
                    animName.Enqueue("F_Idle to Sit");
                }

                loopCount = Random.Range(2, 4);
                for (int i = 0; i < loopCount; i++)
                {
                    animName.Enqueue("F_Sit");
                }

                selector = Random.Range(0, 3);
                if (selector == 0)
                {

                }
                if (selector == 1)
                {
                    loopCount = Random.Range(1, 1);
                    for (int i = 0; i < loopCount; i++)
                    {
                        animName.Enqueue("F_Sit to Lie");
                    }

                    loopCount = Random.Range(2, 4);
                    for (int i = 0; i < loopCount; i++)
                    {
                        animName.Enqueue("F_Lie01");
                    }

                    loopCount = Random.Range(1, 1);
                    for (int i = 0; i < loopCount; i++)
                    {
                        animName.Enqueue("F_Lie02");
                    }

                    loopCount = Random.Range(1, 1);
                    for (int i = 0; i < loopCount; i++)
                    {
                        animName.Enqueue("F_Lie to Sit");
                    }
                }
                if (selector == 2)
                {
                    loopCount = Random.Range(1, 1);
                    for (int i = 0; i < loopCount; i++)
                    {
                        animName.Enqueue("F_Sit to Lie");
                    }

                    loopCount = Random.Range(2, 4);
                    for (int i = 0; i < loopCount; i++)
                    {
                        animName.Enqueue("F_Lie01");
                    }

                    loopCount = Random.Range(1, 1);
                    for (int i = 0; i < loopCount; i++)
                    {
                        animName.Enqueue("F_Lie02");
                    }

                    loopCount = Random.Range(1, 1);
                    for (int i = 0; i < loopCount; i++)
                    {
                        animName.Enqueue("F_Lie to Sleep");
                    }
                    loopCount = Random.Range(2, 4);
                    for (int i = 0; i < loopCount; i++)
                    {
                        animName.Enqueue("F_Sleep");
                    }

                    loopCount = Random.Range(1, 1);
                    for (int i = 0; i < loopCount; i++)
                    {
                        animName.Enqueue("F_Sleep to Sit");
                    }
                }

                loopCount = Random.Range(1, 1);
                for (int i = 0; i < loopCount; i++)
                {
                    animName.Enqueue("F_Sit to Idle");
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
                    animName.Enqueue("F_Run");
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
                //        animName.Enqueue("F_Run_Left");
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
                //        animName.Enqueue("F_Run_Right");
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

                selector = Random.Range(0, 4);
                if (selector == 0)
                {
                    loopCount = Random.Range(1, 1);
                    for (int i = 0; i < loopCount; i++)
                    {
                        animName.Enqueue("F_Attack_Bite_Left");
                    }

                }
                else if (selector == 1)
                {
                    loopCount = Random.Range(1, 1);
                    for (int i = 0; i < loopCount; i++)
                    {
                        animName.Enqueue("F_Attack_Bite_Right");
                    }

                }
                else if (selector == 2)
                {
                    loopCount = Random.Range(1, 1);
                    for (int i = 0; i < loopCount; i++)
                    {
                        animName.Enqueue("F_Attack_Claws");
                    }

                }
                else if (selector == 3)
                {
                    loopCount = Random.Range(1, 1);
                    for (int i = 0; i < loopCount; i++)
                    {
                        animName.Enqueue("F_Attack_Jump");
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

                selector = Random.Range(0, 2);
                if (selector == 0)
                {
                    loopCount = Random.Range(1, 1);
                    for (int i = 0; i < loopCount; i++)
                    {
                        animName.Enqueue("F_Death1");
                    }

                }
                else if (selector == 1)
                {
                    loopCount = Random.Range(1, 1);
                    for (int i = 0; i < loopCount; i++)
                    {
                        animName.Enqueue("F_Death2");
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
