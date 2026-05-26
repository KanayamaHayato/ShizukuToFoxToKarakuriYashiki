using System.Collections;
using TMPro;
using UnityEngine;

public class LoreNotifyUI : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private TextMeshProUGUI notifyText;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float displayTime = 2.5f;
    [SerializeField] private float fadeTime = 0.4f;

    void Start()
    {
        // SetActive(false)ではなくalphaで隠す
        if (canvasGroup != null)
            canvasGroup.alpha = 0f;
    }

    public void Show(int number, string name)
    {
        StopAllCoroutines();
        notifyText.text = $"伝記 No.{number} 入手\n「{name}」";
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

        yield return new WaitForSeconds(displayTime);

        // フェードアウト
        while (canvasGroup.alpha > 0f)
        {
            canvasGroup.alpha -= Time.deltaTime / fadeTime;
            yield return null;
        }

        root.SetActive(false);
    }
}