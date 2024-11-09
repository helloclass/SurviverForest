using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoltGun : MonoBehaviour
{
    Animator BodyAnimator;

    // For Gun Recoil
    GameObject PlayerBody;

    GameObject PistolObject;
    GameObject PistolObject2;
    GameObject PistolObject3;
    GameObject PistolShellObject;
 
    public Vector3 NormalPosition;
    public Vector3 NormalRotation;
    Quaternion NormalQuat;

    public Vector3 AimmingPosition;
    public Vector3 AimmingRotation;
    Quaternion AimmingQuat;

    // 총기 반동에 대한 양 설정
    public Vector3 MinAmountOfRecoil;
    public Vector3 MinAmountOfFixedRecoil;

    public Vector3 MaxAmountOfRecoil;
    public Vector3 MaxAmountOfFixedRecoil;

    public int PistolNum = 0;
    const int MaxPistolNum = 30;

    float FireDelayTime = 0.0f;
    float MaxFireDelayTime = 0.1f;

    bool isFixed;
    bool isFireDone;
    bool isReload;

    void Awake()
    {
        PlayerBody = GameObject.Find("PlayerBody");

        PistolObject = GameObject.Find("AK47_Particle_System");
        PistolShellObject = GameObject.Find("AK47_Particle_System_2");

        PistolObject2 = GameObject.Find("Muzzleflash_Star");
        PistolObject3 = GameObject.Find("MuzzleFlash");
    }

    // Start is called before the first frame update
    void Start()
    {
        PistolNum = MaxPistolNum;

        isFixed         = false;
        isFireDone      = true;
        isReload        = false;

        BodyAnimator    = GetComponent<Animator>();

        BodyAnimator.SetBool("IsFire", false);
        BodyAnimator.SetBool("IsFixedFire", false);
        BodyAnimator.SetBool("IsReload", false);

        NormalQuat      = Quaternion.Euler(NormalRotation);
        AimmingQuat     = Quaternion.Euler(AimmingRotation);

        StartCoroutine(UpdateAnimator());
        StartCoroutine(CalcNextClip());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.R))
        {
            if (isFixed)
            {
                isFixed = false;
                StartCoroutine(MakeNormalAnim());
            }

            isReload = true;
            BodyAnimator.SetBool("IsReload", true);
            PistolNum = MaxPistolNum;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            if (!isFixed)
            {
                isFixed = true;
                StartCoroutine(MakeFixedAnim());
            }
            else
            {
                isFixed = false;
                StartCoroutine(MakeNormalAnim());
            }
        }

        FireDelayTime += Time.fixedDeltaTime;
    }

    void OnDestroy()
    {
        StopCoroutine(UpdateAnimator());
        StopCoroutine(CalcNextClip());
    }

    IEnumerator MakeFixedAnim()
    {
        float AccumulateTime = 0.0f;

        while (AccumulateTime < 1.0f)
        {
            transform.localPosition = Vector3.Lerp(NormalPosition, AimmingPosition, AccumulateTime);
            transform.localRotation = Quaternion.Lerp(NormalQuat, AimmingQuat, AccumulateTime);

            AccumulateTime += Time.fixedDeltaTime * 2.0f;

            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }

        transform.localPosition = AimmingPosition;
        transform.localRotation = AimmingQuat;
    }

    IEnumerator MakeNormalAnim()
    {
        float AccumulateTime = 0.0f;

        while (AccumulateTime < 1.0f)
        {
            transform.localPosition = Vector3.Lerp(AimmingPosition, NormalPosition, AccumulateTime);
            transform.localRotation = Quaternion.Lerp(AimmingQuat, NormalQuat, AccumulateTime);

            AccumulateTime += Time.fixedDeltaTime * 2.0f;

            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }

        transform.localPosition = NormalPosition;
        transform.localRotation = NormalQuat;
    }

    IEnumerator CalcNextClip()
    {
        AnimatorStateInfo animInfo;
        bool normalTime;

        while (true)
        {
            animInfo = BodyAnimator.GetCurrentAnimatorStateInfo(0);
            normalTime = (1.0f < animInfo.normalizedTime);

            isFireDone =
                (animInfo.IsName("bolt_cycle") &&
                normalTime) || 
                animInfo.IsName("idle");

            if (isReload && animInfo.IsName("reload"))
            {
                isReload = false;
                BodyAnimator.SetBool("IsReload", false);
            }
            if (isFireDone)
            {
                //PistolObject.GetComponent<ParticleSystem>().Stop(true);
                //PistolShellObject.GetComponent<ParticleSystem>().Stop(true);
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator UpdateAnimator()
    {
        while (true)
        {
            BodyAnimator.SetBool("IsFire", false);
            BodyAnimator.SetBool("IsFixedFire", false);

            // Auto Charging
            if (PistolNum <= 0)
            {
                if (isFixed)
                {
                    isFixed = false;
                    StartCoroutine(MakeNormalAnim());
                }

                isReload = true;
                BodyAnimator.SetBool("IsReload", true);
                PistolNum = MaxPistolNum;
            }
            else if (Input.GetMouseButton(0) && (MaxFireDelayTime < FireDelayTime))
            {
                FireDelayTime = 0.0f;

                // Fire
                if (isFireDone)
                {
                    isFireDone = false;
                    PistolNum--;

                    if (!isFixed)
                    {
                        PlayerBody.GetComponent<PlayerController>().GunRecoil(
                            new Vector3(
                                Random.Range(MinAmountOfRecoil.x, MaxAmountOfRecoil.x),
                                Random.Range(MinAmountOfRecoil.y, MaxAmountOfRecoil.y),
                                Random.Range(MinAmountOfRecoil.z, MaxAmountOfRecoil.z)
                        )); 
                        BodyAnimator.SetBool("IsFire", true);
                    }
                    else
                    {
                        PlayerBody.GetComponent<PlayerController>().GunRecoil(
                            new Vector3(
                                Random.Range(MinAmountOfFixedRecoil.x, MaxAmountOfFixedRecoil.x),
                                Random.Range(MinAmountOfFixedRecoil.y, MaxAmountOfFixedRecoil.y),
                                Random.Range(MinAmountOfFixedRecoil.z, MaxAmountOfFixedRecoil.z)
                        ));
                        BodyAnimator.SetBool("IsFixedFire", true);
                    }

                    // Fire Pistol
                    PistolObject.GetComponent<ParticleSystem>().Play(true);
                    PistolShellObject.GetComponent<ParticleSystem>().Play(true);
                    PistolObject2.GetComponent<ParticleSystem>().Play(true);
                    PistolObject3.GetComponent<ParticleSystem>().Play(true);
                }
            }

            yield return null;
        }
    }
}
