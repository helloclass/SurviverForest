using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachetePlayer : ThreeSwingBuki
{
    public override void Start()
    {
        base.Start();

        TimeOutOfSwing = 0.6f;
        TimeOutOfHit = 0.6f;
    }
}
