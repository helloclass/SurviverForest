using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Buki : Object
{
    public GameManager gameManager;

    public Animator BodyAnimator;

    // 무기를 던져서 손이 비였다면 HandPlayer로 변경한다.
    public GameObject HandPlayer;
    // 던져질 무기 오브젝트
    public GameObject HandleThrowBody;
    public ThrowObject throwObject;

    protected bool isAttack1;
    protected bool isAttack2;
    protected bool isAttack3;

    protected float CheckTimeOut;
    protected const float TimeOutLimit = 0.75f;
    protected float TimeOutOfSwing;
    protected float TimeOutOfHit;

    protected bool isAnimationDone1;
    protected bool isAnimationDone2;
    protected bool isAnimationDone3;

    protected bool isHitDone1;
    protected bool isHitDone2;
    protected bool isHitDone3;

    // 총알, 화살 포함 던질 수 있는 것의 개수
    protected int NumberOfThrowHandle = 1;
    // 최대 개수 등 정의
    protected int MaxNumberOfThrowHandle = 1;

    protected bool isThrowAble = false;
    protected bool isThrowHold = false;
    protected bool isThrow = false;

    public virtual void Awake()
    {
        throwObject = HandleThrowBody.GetComponent<ThrowObject>();
    }

    // Start is called before the first frame update
    public virtual void Start()
    {
        isThrowAble = true;

        isAttack1 = false;
        isAttack2 = false;
        isAttack3 = false;

        CheckTimeOut = 0.0f;

        TimeOutOfSwing = 0.3f;
        TimeOutOfHit = 0.7f;

    isAnimationDone1 = false;
        isAnimationDone2 = false;
        isAnimationDone3 = false;

        isHitDone1 = false;
        isHitDone2 = false;
        isHitDone3 = false;

        HandleThrowBody.GetComponent<BoxCollider>().enabled = false;

        StartCoroutine(UpdateAnimator());
        StartCoroutine(CalcNextClip());
    }

    // BukiPlayer가 활성화 되었을 때 코루틴을 다시 시작시킨다.
    public virtual void OnEnable()
    {
        // Run Start..?

        StartCoroutine(UpdateAnimator());
        StartCoroutine(CalcNextClip());
    }

    // BukiPalyer가 비활성화 되었을 때 코루틴을 강제로 종료시킨다.
    public virtual void OnDisable()
    {
        // Run OnDestroy..?

        StopCoroutine(UpdateAnimator());
        StopCoroutine(CalcNextClip());
    }

    public virtual void OnDestroy()
    {
        StopCoroutine(UpdateAnimator());
        StopCoroutine(CalcNextClip());
    }

    // 화살, 총알 등의 소모품들을 전부 다 사용하였을때
    public abstract void ThrowObjectIsEmpty();
    // 화살, 총알 등의 소모품들을 충전했을 때
    public abstract void ChargingThrowObject();

    public abstract IEnumerator UpdateAnimator();
    public abstract IEnumerator CalcNextClip();
}
