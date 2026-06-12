using System.Collections;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private GameObject tutorialUI;  // チュートリアルのUI
    [SerializeField] private float delayAfterDialogue = 2f; // セリフ後の待機時間

    private bool isShowing = false;

    void Start()
    {
        tutorialUI.SetActive(false);
        StartCoroutine(WaitAndShow());
    }

    private IEnumerator WaitAndShow()
    {
        // セリフが終わるまで待つ
        yield return new WaitUntil(() => !DialogueManager.Instance.IsRunning);

        // 一定時間待つ
        yield return new WaitForSeconds(delayAfterDialogue);

        // チュートリアル表示
        tutorialUI.SetActive(true);
        isShowing = true;
    }

    void Update()
    {
        if (!isShowing) return;

        if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
        {
            tutorialUI.SetActive(false);
            isShowing = false;
        }
    }
}