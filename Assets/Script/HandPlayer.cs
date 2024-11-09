using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandPlayer : Buki
{
    Animator HandAnimator;

    // Start is called before the first frame update
    void Start()
    {
        HandAnimator = GetComponent<Animator>();

        HandAnimator.SetBool("isWave", false);
        HandAnimator.SetBool("isVictory", false);
        HandAnimator.SetBool("isClap", false);
        HandAnimator.SetBool("isOk", false);
        HandAnimator.SetBool("isThumbDown", false);
        HandAnimator.SetBool("isThumbUp", false);
        HandAnimator.SetBool("isFriendly", false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    // 화살, 총알 등의 소모품들을 전부 다 사용하였을때
    public override void ThrowObjectIsEmpty()
    {

    }
    // 화살, 총알 등의 소모품들을 충전했을 때
    public override void ChargingThrowObject()
    {

    }

    public override IEnumerator UpdateAnimator()
    {
        yield return null;
    }
    public override IEnumerator CalcNextClip()
    {
        yield return null;
    }
}
