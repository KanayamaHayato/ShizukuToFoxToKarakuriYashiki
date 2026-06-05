using System;
using System.Collections;
using UnityEngine;

public class LanternInteract : MonoBehaviour
{
    [SerializeField] private Renderer lanternRenderer;

    [Header("Œõ‰‰o")]
    [SerializeField] private float litIntensity = 2.0f;
    [SerializeField] private float fadeTime = 1.5f;

    public event Action OnLit;

    private bool playerNear = false;
    private bool alreadyTouched = false;

    private const int LightMaterialIndex = 4;
    private Material lightMaterial;
    private bool isReady = false; // š’Ç‰Á


    // š LanternManager‚©‚ç’¼Ú“n‚µ‚Ä‚à‚ç‚¤
    private LanternManager lanternManager;

    public void SetLanternManager(LanternManager lm)
    {
        lanternManager = lm;
    }

    void Start()
    {
        if (lanternRenderer == null)
        {
            Debug.LogError($"[LanternInteract] lanternRenderer ‚ª–¢İ’è‚Å‚·: {gameObject.name}");
            return;
        }

        var mats = lanternRenderer.materials;

        if (mats.Length <= LightMaterialIndex)
        {
            Debug.LogError($"[LanternInteract] ƒ}ƒeƒŠƒAƒ‹”‚ª‘«‚è‚Ü‚¹‚ñ: {mats.Length}");
            return;
        }

        lightMaterial = mats[LightMaterialIndex];
        lightMaterial.EnableKeyword("_EMISSION");
        lightMaterial.SetColor("_EmissionColor", Color.black);
        mats[LightMaterialIndex] = lightMaterial;
        lanternRenderer.materials = mats;
        // š’Ç‰Á: ¶¬’¼Œã‚ÌŒë”­‰Î‚ğ–h‚®
        Invoke(nameof(SetReady), 0.5f);
    }

    private void SetReady() => isReady = true;

    void Update()
    {
        if (playerNear && !alreadyTouched && Input.GetKeyDown(KeyCode.E))
            TouchLantern();
    }

    private void TouchLantern()
    {
        alreadyTouched = true;
        InteractUIManager.Instance.Hide();
        StartCoroutine(LightUpCoroutine());
        Debug.Log($"[LanternInteract] OnLitw“Ç”: {OnLit?.GetInvocationList().Length}");
        OnLit?.Invoke();
        Debug.Log("“”âÄ‚ÉG‚ê‚½");
    }

    private IEnumerator LightUpCoroutine()
    {
        float elapsed = 0f;
        Color baseColor = lightMaterial.GetColor("_Color");

        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeTime);
            Color emissionColor = baseColor * Mathf.Pow(t * litIntensity, 2f);
            lightMaterial.SetColor("_EmissionColor", emissionColor);
            yield return null;
        }

        Color finalColor = baseColor * litIntensity;
        lightMaterial.SetColor("_EmissionColor", finalColor);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isReady) return; // š’Ç‰Á

        if (other.CompareTag("Player"))
        {
            playerNear = true;
            if (!alreadyTouched)
                InteractUIManager.Instance.Show();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = false;
            InteractUIManager.Instance.Hide();
        }
    }
}