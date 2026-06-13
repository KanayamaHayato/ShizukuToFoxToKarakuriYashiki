using System.Collections.Generic;
using UnityEngine;

public class LoreSpawnerManager : MonoBehaviour
{
    [Header("設定")]
    [SerializeField] private List<GameObject> lorePrefabs; // 伝記プレハブ（番号順）
    [SerializeField] private int spawnCount = 7;

    public void SpawnAll()
    {
        // シーン内の全LoreSpawnerを収集
        var spawners = new List<LoreSpawner>(FindObjectsOfType<LoreSpawner>());

        if (spawners.Count < spawnCount)
        {
            Debug.LogWarning($"[LoreSpawnerManager] スポーンポイントが足りません: {spawners.Count}/{spawnCount}");
            spawnCount = spawners.Count;
        }

        // シャッフル
        for (int i = 0; i < spawners.Count; i++)
        {
            int j = Random.Range(i, spawners.Count);
            (spawners[i], spawners[j]) = (spawners[j], spawners[i]);
        }

        for (int i = 0; i < spawnCount; i++)
        {
            GameObject prefab = lorePrefabs[i % lorePrefabs.Count];
            var go = Instantiate(prefab, spawners[i].transform.position, Quaternion.identity);
            go.transform.SetParent(null); // ← 追加
            go.isStatic = false;
            foreach (Transform child in go.transform)
                child.gameObject.isStatic = false;
        }

        Debug.Log($"[LoreSpawnerManager] 伝記{spawnCount}個配置完了");

    }
}