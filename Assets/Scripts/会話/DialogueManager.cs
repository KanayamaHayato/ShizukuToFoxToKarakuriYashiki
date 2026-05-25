using System;
using System.Collections;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [SerializeField] private DialogueUI dialogueUI;

    private DialogueData currentData;
    private int currentIndex;
    private bool isRunning = false;

    // 会話終了時のコールバック（Timelineから使う）
    public event Action OnDialogueEnd;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // 外から呼ぶ開始メソッド
    public void StartDialogue(DialogueData data)
    {
        if (isRunning) return;

        currentData = data;
        currentIndex = 0;
        isRunning = true;

        dialogueUI.Show();
        ShowCurrentLine();
    }

    // Eキーまたはクリックで次へ
    void Update()
    {
        if (!isRunning) return;

        if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
            Next();
    }

    private void Next()
    {
        // タイプライター中なら即全表示
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
}