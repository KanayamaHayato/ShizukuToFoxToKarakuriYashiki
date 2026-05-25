using System;
using System.Collections;
using UnityEngine;

public class LanternInteract : MonoBehaviour
{
    [SerializeField] private Renderer lanternRenderer;
    [SerializeField] private GameObject interactUI;

    [Header("光演出")]
    [SerializeField] private float litIntensity = 2.0f;   // 最終的な光の強さ
    [SerializeField] private float fadeTime = 1.5f;   // ふわっと光るまでの秒数

    public event Action OnLit;

    private bool playerNear = false;
    private bool alreadyTouched = false;

    // 光部分マテリアルのインデックス
    private const int LightMaterialIndex = 4;

    private Material lightMaterial; // インスタンスマテリアル

    void Start()
    {
        if (interactUI != null)
            interactUI.SetActive(false);

        // マテリアルをインスタンス化（他の灯籠に影響しないように）
        lightMaterial = lanternRenderer.materials[LightMaterialIndex];
        lightMaterial.EnableKeyword("_EMISSION");
        lightMaterial.SetColor("_EmissionColor", Color.black); // 最初は消灯

        // インスタンスマテリアルをRendererに反映
        var mats = lanternRenderer.materials;
        mats[LightMaterialIndex] = lightMaterial;
        lanternRenderer.materials = mats;
    }

    void Update()
    {
        if (playerNear && !alreadyTouched && Input.GetKeyDown(KeyCode.E))
            TouchLantern();
    }

    private void TouchLantern()
    {
        alreadyTouched = true;

        if (interactUI != null)
            interactUI.SetActive(false);

        StartCoroutine(LightUpCoroutine());
        OnLit?.Invoke();
        Debug.Log("灯籠に触れた");
    }

    private IEnumerator LightUpCoroutine()
    {
        float elapsed = 0f;

        // 光部分の元の色を取得
        Color baseColor = lightMaterial.GetColor("_Color");

        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeTime);

            // Emissionをだんだん強くする
            Color emissionColor = baseColor * Mathf.Pow(t * litIntensity, 2f);
            lightMaterial.SetColor("_EmissionColor", emissionColor);

            yield return null;
        }

        // 最終値で固定
        Color finalColor = baseColor * litIntensity;
        lightMaterial.SetColor("_EmissionColor", finalColor);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = true;
            if (!alreadyTouched && interactUI != null)
                interactUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = false;
            if (interactUI != null)
                interactUI.SetActive(false);
        }
    }
}