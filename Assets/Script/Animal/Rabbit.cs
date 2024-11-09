using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rabbit : Enemy
{
    public override void Start()
    {
        base.Start();

        HealthPoint = 10.0f;
        PowerPoint = 5.0f;

        WalkSpeed = 5.0f;
        RunSpeed = 10.0f;

        PlayerAnimator = GetComponent<Animator>();
    }
}
