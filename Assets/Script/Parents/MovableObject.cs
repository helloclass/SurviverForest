using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableObject : Object
{
    // TargetObject
    public GameObject PlayerObject;
    public Animator PlayerAnimator;

    protected float HealthPoint;
    protected float PowerPoint;

    public float WalkSpeed;
    public float RunSpeed;

    public bool isDead;

    public virtual void Start()
    {
        HealthPoint = 10.0f;
        PowerPoint = 5.0f;

        WalkSpeed = 10.0f;
        RunSpeed = 30.0f;

        isDead = false;
    }
}
