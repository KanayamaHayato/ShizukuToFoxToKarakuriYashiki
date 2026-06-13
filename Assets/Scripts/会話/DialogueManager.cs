using System;
using System.Collections;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [SerializeField] private DialogueUI dialogueUI;

    [Header("自動再生設定")]
    [SerializeField] private bool autoPlay = false;      // ★ 自動再生モード
    [SerializeField] private float autoPlayDelay = 3f;  // ★ 1行あたりの表示時間

    private DialogueData currentData;
    private int currentIndex;
    private bool isRunning = false;

    public event Action OnDialogueEnd;
    public bool IsRunning => isRunning;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        dialogueUI.Hide();
    }

    public void StartDialogue(DialogueData data)
    {
        Debug.Log($"[DialogueManager] StartDialogue呼ばれた: {data.name}", data);
        if (isRunning) return;

        currentData = data;
        currentIndex = 0;
        isRunning = true;

        dialogueUI.Show();
        ShowCurrentLine();

        // ★ 自動再生モードならコルーチン開始
        if (autoPlay)
            StartCoroutine(AutoPlayCoroutine());
    }

    void Update()
    {
        if (!isRunning || autoPlay) return; // ★ autoPlay中はEキー無効

        if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
            Next();
    }

    private IEnumerator AutoPlayCoroutine()
    {
        while (isRunning)
        {
            // タイプライターが終わるまで待つ
            yield return new WaitUntil(() => !dialogueUI.IsTyping);

            // 表示時間待つ
            yield return new WaitForSeconds(autoPlayDelay);

            if (!isRunning) break;
            Next();
        }
    }

    private void Next()
    {
        if (dialogueUI.IsTyping)
        {
            dialogueUI.SkipTyping();
            return;
        }

        currentIndex++;

        if (currentIndex >= currentData.lines.Length)
        {
            EndDialogue();
            return;
        }

        ShowCurrentLine();
    }

    private void ShowCurrentLine()
    {
        var line = currentData.lines[currentIndex];
        dialogueUI.ShowLine(line.speakerName, line.text);
    }

    private void EndDialogue()
    {
        isRunning = false;
        dialogueUI.Hide();
        OnDialogueEnd?.Invoke();
    }
    public void ForceStop()
    {
        isRunning = false;
        dialogueUI.Hide();
    }
}