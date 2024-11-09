using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackSawPlayer : TwoSwingBuki
{
    public override void Start()
    {
        base.Start();
        isThrowAble = false;

        TimeOutOfSwing = 0.6f;
        TimeOutOfHit = 0.6f;

    }
}
