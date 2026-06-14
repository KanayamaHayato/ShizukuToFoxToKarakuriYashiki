using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance { get; private set; }

    [SerializeField] private CanvasGroup fadePanel;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float fadeOutDelay = 10f; // ★ 何秒後にフェードアウトするか
    [SerializeField] private string nextSceneName = "ShrineScene"; // 次のシーン名
    [SerializeField] private Image fadePanelImage;
    [SerializeField] private bool autoTransition = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        if (autoTransition) // ★追加
            StartCoroutine(FadeSequence());
    }

    private IEnumerator FadeSequence()
    {
        // フェードイン
        yield return StartCoroutine(FadeIn());

        // ★ 一定時間待ってからフェードアウト
        yield return new WaitForSeconds(fadeOutDelay);

        yield return StartCoroutine(FadeOut());

        // ★ フェードアウト完了後にシーン遷移
        UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
    }

    public IEnumerator FadeIn()
    {
        fadePanelImage.color = Color.black;
        fadePanel.alpha = 1f;
        while (fadePanel.alpha > 0f)
        {
            fadePanel.alpha -= Time.deltaTime / fadeDuration;
            yield return null;
        }
        fadePanel.alpha = 0f;
    }

    public IEnumerator FadeOut()
    {
        fadePanelImage.color = Color.black;
        fadePanel.alpha = 0f;
        while (fadePanel.alpha < 1f)
        {
            fadePanel.alpha += Time.deltaTime / fadeDuration;
            yield return null;
        }
        fadePanel.alpha = 1f;
    }

    public IEnumerator WhiteOut()
    {
        fadePanelImage.color = Color.white;
        fadePanel.alpha = 0f;
        while (fadePanel.alpha < 1f)
        {
            fadePanel.alpha += Time.deltaTime / fadeDuration;
            yield return null;
        }
        fadePanel.alpha = 1f;
    }
}