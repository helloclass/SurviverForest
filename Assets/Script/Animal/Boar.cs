using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boar : Enemy
{
    public BoarAnimationTree bearAnimation;

    public override void Start()    {
        base.Start();

        HealthPoint = 100.0f;
        PowerPoint = 5.0f;

        WalkSpeed = 5.0f;
        RunSpeed = 10.0f;

        PlayerAnimator = GetComponent<Animator>();

        if (PlayerAnimator != null)
        {
            bearAnimation = new BoarAnimationTree(PlayerAnimator);

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
        float PlayerDist = (transform.position - PlayerObject.transform.position).magnitude;

        if (80.0f < PlayerDist)
            return;

        bearAnimation.AnimationUpdate();

        if (!isDead)
        {
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
            else if (PlayerDist < PlayerRecognizeRange || isAttacked)
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

    public override void Damaging(float DamageHP, float RecoilPower = 100)
    {
        base.Damaging(DamageHP, RecoilPower);

        bearAnimation.isRecognize = true;
        bearAnimation.isAttack = true;
        bearAnimation.isHurt = true;
    }

    public override void Dead()
    {
        base.Dead();

        bearAnimation.isRecognize = true;
        bearAnimation.isAttack = true;
        bearAnimation.isHurt = true;
        bearAnimation.isDead = true;
    }
}
