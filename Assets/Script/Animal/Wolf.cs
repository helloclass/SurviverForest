using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf : Enemy
{
    public WolfAnimationTree bearAnimation;

    public override void Start()
    {
        base.Start();

        HealthPoint = 50.0f;
        PowerPoint = 5.0f;

        WalkSpeed = 5.0f;
        RunSpeed = 10.0f;

        PlayerAnimator = GetComponent<Animator>();

        if (PlayerAnimator != null)
        {
            bearAnimation = new WolfAnimationTree(PlayerAnimator);

            StartCoroutine(bearAnimation.AnimUpdate());
            bearAnimation.AnimationStart();
        }
        else
        {
            Debug.Log("Error");
        }
    }

    // Update is called once per frame
    public void Update()
    {
        bearAnimation.AnimationUpdate();

        if (!isDead)
        {
            float PlayerDist = (transform.position - PlayerObject.transform.position).magnitude;

            // 일정 거리 이내로 다가오면 공격을 시작한다
            if (PlayerDist < PlayerAttackRange)
            {
                Vector3 LookForward =
                    (PlayerObject.transform.position - transform.position).normalized;

                float Roll = Vector3.Dot(transform.right, LookForward) * Mathf.Rad2Deg;

                transform.Rotate(new Vector3(0, Roll, 0) * Time.deltaTime * 30.0f);

                bearAnimation.isAttack = true;
                bearAnimation.TurnAxis = 0.0f;
            }
            else if (PlayerDist < PlayerRecognizeRange)
            {
                Vector3 LookForward =
                    (PlayerObject.transform.position - transform.position).normalized;

                float Roll = Vector3.Dot(transform.right, LookForward) * Mathf.Rad2Deg;

                transform.Rotate(new Vector3(0, Roll, 0) * Time.deltaTime * 30.0f);

                bearAnimation.isAttack = false;
                bearAnimation.isRecognize = true;
                //bearAnimation.TurnAxis = Roll;
            }
            else
            {
                bearAnimation.isAttack = false;
                bearAnimation.isRecognize = false;
                bearAnimation.TurnAxis = 0.0f;
            }
        }
        else
        {

        }
    }
}
