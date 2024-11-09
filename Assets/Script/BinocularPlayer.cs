using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BinocularPlayer : Buki
{
    Animator HandAnimator;

    public GameObject PivotUI;
    public GameObject UseBinocularUI;

    // Start is called before the first frame update
    public override void Start()
    {
        HandAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PivotUI.SetActive(false);
            UseBinocularUI.GetComponent<UIBinocularController>().ChangeSight();
        }
        if (Input.GetMouseButtonUp(0))
        {
            PivotUI.SetActive(true);
            UseBinocularUI.GetComponent<UIBinocularController>().ChangeSight();
        }
    }

    // 화살, 총알 등의 소모품들을 전부 다 사용하였을때
    public override void ThrowObjectIsEmpty()
    {

    }

    // 화살, 총알 등의 소모품들을 충전했을 때
    public override void ChargingThrowObject()
    {

    }

    public override IEnumerator CalcNextClip()
    {
        yield return null; 
    }
    public override IEnumerator UpdateAnimator()
    {
        yield return null;
    }
}
