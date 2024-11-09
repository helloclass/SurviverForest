using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Machete : ThrowObject
{
    public override void Start()
    {
        FireSpeed = 100.0f;
        DamageValue = 50.0f;
    }
}
