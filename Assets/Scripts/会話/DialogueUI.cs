using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private GameObject root;           // UI全体の親
    [SerializeField] private CanvasGroup canvasGroup;   // フェード用
    [SerializeField] private TextMeshProUGUI nameText;  // 話者名
    [SerializeField] private TextMeshProUGUI bodyText;  // セリフ本文
    [SerializeField] private GameObject nameBox;        // 名前欄（空白のとき非表示）

    [SerializeField] private float typeSpeed = 0.05f; // 1文字あたりの秒数
    [SerializeField] private float fadeSpeed = 0.3f;  // フェイン秒数

    public bool IsTyping { get; private set; } = false;

    private Coroutine typingCoroutine;

    public void Show()
    {
        root.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(FadeIn());
    }

    public void Hide()
    {
        StartCoroutine(FadeOutAndHide());
    }

    public void ShowLine(string speaker, string text)
    {
        // 名前欄
        bool hasSpeaker = !string.IsNullOrEmpty(speaker);
        nameBox.SetActive(hasSpeaker);
        if (hasSpeaker) nameText.text = speaker;

        // タイプライター開始
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeText(text));
    }

    public void SkipTyping()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        // 全文即表示
        bodyText.maxVisibleCharacters = bodyText.text.Length;
        IsTyping = false;
    }

    private IEnumerator TypeText(string text)
    {
        IsTyping = true;
        bodyText.text = text;
        bodyText.maxVisibleCharacters = 0;

        foreach (char _ in text)
        {
            bodyText.maxVisibleCharacters++;
            yield return new WaitForSeconds(typeSpeed);
        }

        IsTyping = false;
    }

    private IEnumerator FadeIn()
    {
        canvasGroup.alpha = 0f;
        while (canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha += Time.deltaTime / fadeSpeed;
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    private IEnumerator FadeOutAndHide()
    {
        while (canvasGroup.alpha > 0f)
        {
            canvasGroup.alpha -= Time.deltaTime / fadeSpeed;
            yield return null;
        }
        root.SetActive(false);
    }
}