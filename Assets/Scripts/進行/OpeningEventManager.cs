using System.Collections;
using UnityEngine;

public class OpeningEventManager : MonoBehaviour
{
    [SerializeField] private DialogueData openingDialogue; // セリフデータ
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private string nextSceneName = "Maze";

    // Signal_StartDialogueから呼ぶ
    public void StartOpeningDialogue()
    {
        if (openingDialogue != null)
            DialogueManager.Instance.StartDialogue(openingDialogue);
    }

    // Signal_FadeOutから呼ぶ
    public void TriggerFadeOut()
    {
        Debug.Log("TriggerFadeOut called!"); // ←追加
        StartCoroutine(FadeOutAndLoad());
    }

    private IEnumerator FadeOutAndLoad()
    {
        // 暗転
        fadeCanvasGroup.alpha = 0f;
        while (fadeCanvasGroup.alpha < 1f)
        {
            fadeCanvasGroup.alpha += Time.deltaTime / fadeDuration;
            yield return null;
        }

        // 次のシーンへ
        UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
    }
}