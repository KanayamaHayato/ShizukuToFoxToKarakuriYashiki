using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// シーン内の全ItemSpawnerをFindして一括でSpawnを呼ぶマネージャー。
/// 迷路生成完了後に SpawnAll() を呼ぶこと。
/// </summary>
public class ItemSpawnerManager : MonoBehaviour
{
    [Header("設定")]
    [SerializeField] private bool spawnOnStart = false; // 迷路生成後に手動で呼ぶ場合はfalse

    private List<ItemSpawner> _spawners = new();

    private void Start()
    {
        if (spawnOnStart) SpawnAll();
    }

    /// <summary>
    /// 迷路生成完了後など、外部から呼び出す。
    /// </summary>
    public void SpawnAll()
    {
        // シーン内の全ItemSpawnerを収集
        _spawners = new List<ItemSpawner>(FindObjectsOfType<ItemSpawner>());

        if (_spawners.Count == 0)
        {
            Debug.LogWarning("[ItemSpawnerManager] ItemSpawnerが1つも見つかりませんでした。");
            return;
        }

        Debug.Log($"[ItemSpawnerManager] {_spawners.Count}個のItemSpawnerを検出。Spawn開始。");

        foreach (var spawner in _spawners)
            spawner.Spawn();
    }

    /// <summary>
    /// 特定の部屋だけ再スポーンしたい場合など。
    /// </summary>
    public void SpawnSingle(ItemSpawner spawner)
    {
        spawner.Spawn();
    }
}