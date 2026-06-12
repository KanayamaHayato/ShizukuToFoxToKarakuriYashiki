using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SceneIntroManager : MonoBehaviour
{
    [Header("暗転UI")]
    [SerializeField] private Image fadeImage; // 黒いImage

    [Header("チャイム")]
    [SerializeField] private AudioSource chimeAudio; // 後で設定

    [Header("設定")]
    [SerializeField] private float chimeDuration = 3f;  // チャイムの長さ
    [SerializeField] private float fadeInDuration = 2f; // フェードインの時間
    [SerializeField] private GameObject autoWalkTarget; // 自動歩きのオブジェクト

    [Header("セリフマネージャー")]
    [SerializeField] private OPDialogueStarter dialogueStarter; // ★追加


    void Start()
    {
        // 最初は真っ黒
        var c = fadeImage.color;
        c.a = 1f;
        fadeImage.color = c;

        // 自動歩きを最初は無効化
        if (autoWalkTarget != null)
            autoWalkTarget.SetActive(false);

        StartCoroutine(IntroSequence());
    }

    private IEnumerator IntroSequence()
    {
        // ① チャイムを鳴らす
        if (chimeAudio != null)
            chimeAudio.Play();

        // ② チャイムが鳴り終わるまで待つ
        yield return new WaitForSeconds(chimeDuration);

        // ③ フェードイン
        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = 1f - Mathf.Clamp01(elapsed / fadeInDuration);
            var c = fadeImage.color;
            c.a = alpha;
            fadeImage.color = c;
            yield return null;
        }

        // ④ セリフ開始
        if (dialogueStarter != null)
            dialogueStarter.StartDialogue();

        // ⑤ 自動歩き開始
        if (autoWalkTarget != null)
            autoWalkTarget.SetActive(true);
    }
}