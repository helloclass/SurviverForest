using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovableObject
{
    bool AttackType = false;

    protected bool isAttacked = false;

    protected float PlayerRecognizeRange = 10.0f;
    protected float PlayerAttackRange = 4.0f;

    public GameObject FireEffectObject;

    // animation Variable

    public virtual void Start()
    {
        isDead = false;
    }

    public virtual void FixedUpdate()
    {
        Vector3 rotation = transform.rotation.eulerAngles;
        rotation.x = 0.0f;
        rotation.z = 0.0f;

        transform.rotation = Quaternion.Euler(rotation);
    }

    public virtual void Attacking(PlayerController player)
    {
        // PlayerObject의 HP를 깎는다.
        //PowerPoint;
        //PlayerObject;
    }

    public virtual void Damaging(float DamageHP, float RecoilPower = 100.0f)
    {
        if (!isDead)
        {
            isAttacked = true;

            HealthPoint -= DamageHP;

            // 피격시 뒤로 물러난다.
            Vector3 RecoilVector = PlayerObject.transform.forward * RecoilPower;
            RecoilVector.y = 0.0f;
            GetComponent<Rigidbody>().velocity += RecoilVector;

            if (HealthPoint <= 0)
            {
                Dead();
            }
        }
    }

    public virtual void GetFired()
    {
        if (FireEffectObject)
            FireEffectObject.GetComponent<ParticleSystem>().Play();
    }

    public virtual void Dead()
    {
        isDead = true;
    }
}
