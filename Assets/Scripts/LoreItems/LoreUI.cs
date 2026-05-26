using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoreUI : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image photoImage;       // ツーショット写真
    [SerializeField] private Sprite photoSprite;     // 後から差し替え可能
    [SerializeField] private float fadeTime = 1.0f;
    [SerializeField] private float displayTime = 3.0f;

    void Start()
    {
        root.SetActive(false);
    }

    public void ShowPhoto()
    {
        if (photoSprite != null)
            photoImage.sprite = photoSprite;

        StartCoroutine(ShowCoroutine());
    }

    private IEnumerator ShowCoroutine()
    {
        root.SetActive(true);
        canvasGroup.alpha = 0f;

        // フェードイン
        while (canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha += Time.deltaTime / fadeTime;
            yield return null;
        }

        // 一定時間表示（Eキーでも閉じられる）
        float elapsed = 0f;
        while (elapsed < displayTime && !Input.GetKeyDown(KeyCode.E))
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        // フェードアウト
        while (canvasGroup.alpha > 0f)
        {
            canvasGroup.alpha -= Time.deltaTime / fadeTime;
            yield return null;
        }

        root.SetActive(false);
    }
}