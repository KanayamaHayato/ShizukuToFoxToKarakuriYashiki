using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 各配置ポイントにアタッチ。データ保持と生成ロジックのみ。
/// 自分では起動しない → ItemSpawnerManagerから呼ばれる。
/// </summary>
public class ItemSpawner : MonoBehaviour
{
    [System.Serializable]
    public class ItemSpawnEntry
    {
        public GameObject itemPrefab;
        [Min(0)]
        public float weight = 1f;
    }

    [Header("抽選アイテムリスト")]
    [SerializeField] private List<ItemSpawnEntry> spawnEntries = new();

    [Header("空当選を許可する")]
    [SerializeField] private bool allowEmpty = false;
    [Range(0f, 100f)]
    [SerializeField] private float emptyWeight = 0f;

    public bool HasSpawned { get; private set; } = false;

    /// <summary>
    /// ItemSpawnerManagerから呼ぶ。
    /// </summary>
    public void Spawn()
    {
        if (HasSpawned) return;

        GameObject selected = WeightedRandom();
        if (selected != null)
            Instantiate(selected, transform.position, transform.rotation, transform.parent);

        HasSpawned = true;
    }

    private GameObject WeightedRandom()
    {
        float totalWeight = 0f;
        foreach (var entry in spawnEntries)
            totalWeight += Mathf.Max(0f, entry.weight);
        if (allowEmpty)
            totalWeight += emptyWeight;

        if (totalWeight <= 0f)
        {
            Debug.LogWarning($"[ItemSpawner] {gameObject.name}: 有効な重みがありません。");
            return null;
        }

        float rand = Random.Range(0f, totalWeight);
        float cursor = 0f;
        foreach (var entry in spawnEntries)
        {
            cursor += Mathf.Max(0f, entry.weight);
            if (rand < cursor) return entry.itemPrefab;
        }
        return null;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 0.25f);
    }
#endif
}