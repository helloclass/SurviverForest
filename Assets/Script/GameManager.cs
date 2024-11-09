using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


public class GameManager : MonoBehaviour
{
    bool mIsInventoryVisible;

    public GameObject mPlayerObject;

    public RectTransform mPlayerHPStateObject;
    public RectTransform mPlayerThirstyStateObject;
    public RectTransform mPlayerHungryStateObject;

    public GameObject mInventoryObject;
    public GameObject mHotkeyObject;

    PlayerController mPlayerController;
    Inventory mInventory;
    Hotkey mHotkey;

    // -1000, -700
    public GameObject TatemonoControllerUI;

    float ScrollSpeed = 1000.0f;

    public Material postProcessMaterial;

    public Color HealVignetteColor;
    public Color HurtVignetteColor;

    public static List<ScriptableRendererFeature> GetRendererFeatures()
    {
        var renderer = (GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset).scriptableRenderer;
        return typeof(ScriptableRenderer)
            .GetField("m_RendererFeatures")
            .GetValue(renderer) as List<ScriptableRendererFeature>;
    }

    // Start is called before the first frame update
    void Start()
    {
        // fixed mouse position
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        mPlayerController = mPlayerObject.GetComponent<PlayerController>();
        mInventory = mPlayerObject.GetComponent<Inventory>();

        // turn off Inventory
        mIsInventoryVisible = false;
        mInventoryObject.SetActive(false);

        var urpAsset = (GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset).scriptableRenderer;

        //List<ScriptableRendererFeature> RendererFeatures = 
        //    typeof(ScriptableRenderer).GetProperty("rendererFeatures").GetValue(urpAsset) as List<ScriptableRendererFeature>;

        var property = typeof(ScriptableRenderer).GetProperty("rendererFeatures", BindingFlags.NonPublic | BindingFlags.Instance);

        List<ScriptableRendererFeature> RendererFeatures =
            property.GetValue(urpAsset) as List<ScriptableRendererFeature>;

        if (urpAsset != null)
        {
            // 모든 ScriptableRendererFeature를 순회하여 원하는 Feature를 찾음
            foreach (var rendererFeature in RendererFeatures)
            {
                if (rendererFeature is BlitRenderPassFeature customFeature)
                {
                    // 해당 Feature의 Material을 새 Material로 변경
                    postProcessMaterial = customFeature.settings.material;

                    if (postProcessMaterial.HasProperty("_Color"))
                    {
                        // _Color 변수를 빨간색으로 변경
                        postProcessMaterial.SetColor("_Color", Color.black);
                    }

                    if (postProcessMaterial.HasProperty("_Power"))
                    {
                        // _Color 변수를 빨간색으로 변경
                        postProcessMaterial.SetFloat("_Power", 3.0f);
                    }

                    if (postProcessMaterial.HasProperty("_IsWarning"))
                    {
                        postProcessMaterial.SetFloat("_IsWarning", 0.0f);
                    }
                    if (postProcessMaterial.HasProperty("_IsDistort"))
                    {
                        postProcessMaterial.SetFloat("_IsDistort", 0.0f);
                    }
                    if (postProcessMaterial.HasProperty("_IsUsedDynamicDoF"))
                    {
                        postProcessMaterial.SetFloat("_IsUsedDynamicDoF", 0.0f);
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Show Mouse Mode
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        // Show Inventory Mode
        if (Input.GetKeyDown(KeyCode.I))
        {
            // open or not
            mIsInventoryVisible = !mIsInventoryVisible;

            if (mIsInventoryVisible)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Confined;
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }

            mInventoryObject.SetActive(mIsInventoryVisible);
        }

        // Input for Hotkey
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            mHotkey.SelectHotkey(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            mHotkey.SelectHotkey(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            mHotkey.SelectHotkey(3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            mHotkey.SelectHotkey(4);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            mHotkey.SelectHotkey(5);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            mHotkey.SelectHotkey(6);
        }

        if (postProcessMaterial.HasProperty("_Color"))
        {
            // _Color 변수를 빨간색으로 변경
            postProcessMaterial.SetColor("_Color", HurtVignetteColor);
        }

        if (postProcessMaterial.HasProperty("_Power"))
        {
            // _Color 변수를 빨간색으로 변경
            float power = Mathf.Sin(Time.time) + 1.0f;
            postProcessMaterial.SetFloat("_Power", power);
        }

        if (postProcessMaterial.HasProperty("_Timer"))
        {
            // _Color 변수를 빨간색으로 변경
            float timer = Mathf.Sin(Time.time) * 0.1f;
            postProcessMaterial.SetFloat("_Timer", timer);
        }

        // Update HP State
        // 1%, 5

        mPlayerHPStateObject.localScale = new Vector3(
                mPlayerController.HP_State_Value * 0.01f,
                5.0f, 
                1.0f
            );
        mPlayerHPStateObject.localPosition = new Vector3(
                -162.0f - (100.0f - mPlayerController.HP_State_Value * 0.1f) * 5.0f,
                1174.0f,
                0.0f
            );

        mPlayerThirstyStateObject.localScale = new Vector3(
                mPlayerController.Thirsty_State_Value * 0.01f,
                5.0f,
                1.0f
            );
        mPlayerThirstyStateObject.localPosition = new Vector3(
                -162.0f - (100.0f - mPlayerController.Thirsty_State_Value * 0.1f) * 5.0f,
                587.0f,
                0.0f
            );

        mPlayerHungryStateObject.localScale = new Vector3(
                mPlayerController.Hungry_State_Value * 0.01f,
                5.0f,
                1.0f
            );
        mPlayerHungryStateObject.localPosition = new Vector3(
                -162.0f - (100.0f - mPlayerController.Hungry_State_Value * 0.1f) * 5.0f,
                0.0f,
                0.0f
            );

        if (mPlayerController.HP_State_Value < 100.0f)
        {
            if (postProcessMaterial.HasProperty("_IsWarning"))
            {
                postProcessMaterial.SetFloat("_IsWarning", 1.0f);
            }
        }

        if (mPlayerController.Thirsty_State_Value < 100.0f)
        {
            if (postProcessMaterial.HasProperty("_IsDistort"))
            {
                postProcessMaterial.SetFloat("_IsDistort", 1.0f);
            }
        }

        if (mPlayerController.Hungry_State_Value < 100.0f)
        {
            if (postProcessMaterial.HasProperty("_IsDistort"))
            {
                postProcessMaterial.SetFloat("_IsDistort", 1.0f);
            }
        }

    }

    IEnumerator OnTatemonoUICoroutine()
    {
        RectTransform rt = TatemonoControllerUI.GetComponent<RectTransform>();
        Vector3 dummy = Vector3.zero;

        while (true)
        {
            dummy = rt.localPosition;
            dummy.x = dummy.x + ScrollSpeed * Time.deltaTime;
            if (-600 < dummy.x)
                break;

            rt.localPosition = dummy;

            yield return new WaitForSeconds(0.01f);
        }

        dummy.x = -600;
        rt.localPosition = dummy;

        yield return null;
    }

    IEnumerator OffTatemonoUICoroutine()
    {
        RectTransform rt = TatemonoControllerUI.GetComponent<RectTransform>();
        Vector3 dummy = Vector3.zero;

        while (true)
        {
            dummy = rt.localPosition;
            dummy.x = dummy.x - ScrollSpeed * Time.deltaTime;
            if (dummy.x < -1000)
                break;

            rt.localPosition = dummy;

            yield return new WaitForSeconds(0.01f);
        }

        dummy.x = -1000;
        rt.localPosition = dummy;

        yield return null;
    }

    public void OnTatemonoUI()
    {
        TatemonoControllerUI.SetActive(true);
        StartCoroutine(OnTatemonoUICoroutine());
    }
    public void OffTatemonoUI()
    {
        StartCoroutine(OffTatemonoUICoroutine());
        TatemonoControllerUI.SetActive(false);
    }
}
