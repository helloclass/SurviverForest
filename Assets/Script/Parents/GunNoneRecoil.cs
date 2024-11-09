using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunNoneRecoil : Object
{
    Animator BodyAnimator;

    // For Gun Recoil
    GameObject PlayerBody;

    GameObject PistolEffectObject;
    GameObject PistolEffectObject2;
    GameObject PistolEffectObject3;
    GameObject PistolShellObject;

    public GameObject PistolObject;

    public int PistolNum = 0;
    const int MaxPistolNum = 30;

    float FireDelayTime = 0.0f;
    float MaxFireDelayTime = 0.1f;

    bool isFireDone;
    bool isReload;

    void Awake()
    {
        PlayerBody = GameObject.Find("PlayerBody");

        PistolEffectObject = GameObject.Find("AK47_Particle_System");
        PistolShellObject = GameObject.Find("AK47_Particle_System_2");

        PistolEffectObject2 = GameObject.Find("Muzzleflash_Star");
        PistolEffectObject3 = GameObject.Find("MuzzleFlash");
    }

    // Start is called before the first frame update
    void Start()
    {
        PistolNum = MaxPistolNum;

        isFireDone = true;
        isReload = false;

        BodyAnimator = GetComponent<Animator>();

        BodyAnimator.SetBool("IsFire", false);
        BodyAnimator.SetBool("IsFixedFire", false);
        BodyAnimator.SetBool("IsReload", false);

        StartCoroutine(UpdateAnimator());
        StartCoroutine(CalcNextClip());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.R))
        {
            isReload = true;
            BodyAnimator.SetBool("IsReload", true);
            PistolNum = MaxPistolNum;
        }

        FireDelayTime += Time.fixedDeltaTime;
    }

    void OnDestroy()
    {
        StopCoroutine(UpdateAnimator());
        StopCoroutine(CalcNextClip());
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
                ((animInfo.IsName("fire-1") ||
                animInfo.IsName("fire_ADS")) &&
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

                    // Fire Pistol
                    PistolEffectObject.GetComponent<ParticleSystem>().Play(true);
                    PistolShellObject.GetComponent<ParticleSystem>().Play(true);
                    PistolEffectObject2.GetComponent<ParticleSystem>().Play(true);
                    PistolEffectObject3.GetComponent<ParticleSystem>().Play(true);

                    GameObject InstPistolBody = Instantiate(PistolObject, PistolObject.transform);
                    InstPistolBody.transform.localScale = new Vector3(5.0f, 5.0f, 5.0f);

                    InstPistolBody.transform.parent = null;
                    InstPistolBody.GetComponent<Pistol>().enabled = true;
                    InstPistolBody.GetComponent<BoxCollider>().enabled = true;
                    InstPistolBody.GetComponent<Pistol>().Fire();
                }
            }

            yield return null;
        }
    }
}
