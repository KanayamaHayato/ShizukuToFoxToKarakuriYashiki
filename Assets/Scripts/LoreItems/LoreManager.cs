using System;
using System.Collections.Generic;
using UnityEngine;

public class LoreManager : MonoBehaviour
{
    public static LoreManager Instance { get; private set; }

    [Header("収集設定")]
    [SerializeField] private int requiredLoreCount = 5; // 全部で何個か（インスペクターで設定）

    [Header("写真表示UI")]
    [SerializeField] private LoreUI loreUI;

    [Header("入手通知UI")]
    [SerializeField] private LoreNotifyUI notifyUI;

    // 収集済み伝記リスト（番号順）
    private List<LoreItem> collectedLores = new List<LoreItem>();

    public int CollectedCount => collectedLores.Count;
    public int RequiredCount => requiredLoreCount;
    public bool HasAllLores => collectedLores.Count >= requiredLoreCount;

    // 全部集めたときのイベント
    public event Action OnAllLoresCollected;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void CollectLore(LoreItem item)
    {
        // 重複チェック
        foreach (var l in collectedLores)
            if (l.LoreNumber == item.LoreNumber) return;

        collectedLores.Add(item);

        // 入手通知を出す
        if (notifyUI != null)
            notifyUI.Show(item.LoreNumber, item.LoreName);

        Debug.Log($"[LoreManager] 伝記収集 {collectedLores.Count}/{requiredLoreCount}");

        // 全部集まったら写真表示
        if (HasAllLores)
        {
            Debug.Log("[LoreManager] 全伝記収集！写真を表示");
            OnAllLoresCollected?.Invoke();
            if (loreUI != null)
                loreUI.ShowPhoto();
        }
    }

    // 収集済みの伝記を番号順で取得（メニュー表示用）
    public List<LoreItem> GetCollectedLoresSorted()
    {
        var sorted = new List<LoreItem>(collectedLores);
        sorted.Sort((a, b) => a.LoreNumber.CompareTo(b.LoreNumber));
        return sorted;
    }
}