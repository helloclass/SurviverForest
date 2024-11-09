using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerPlayer : TwoSwingBuki
{
    public GameObject GameManagerObject;

    public override void Start()
    {
        base.Start();
        isThrowAble = false;

        TimeOutOfSwing = 0.6f;
        TimeOutOfHit = 0.6f;
    }

    public override void OnEnable()
    {
        base.OnEnable();

        GameManagerObject.GetComponent<GameManager>().OnTatemonoUI();
    }

    public override void OnDisable()
    {
        base.OnDisable();

        GameManagerObject.GetComponent<GameManager>().OffTatemonoUI();
    }
}
