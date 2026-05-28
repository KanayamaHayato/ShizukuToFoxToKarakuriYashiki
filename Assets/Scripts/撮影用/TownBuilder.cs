using System.Collections.Generic;
using UnityEngine;

public class TownBuilder : MonoBehaviour
{
    [SerializeField] private GameObject[] housePrefabs; // Spriteを持つプレハブ
    [SerializeField] private int rowCount = 4;          // 列数（Z方向）
    [SerializeField] private float rowSpacing = 3f;     // 列間隔（Z方向）
    [SerializeField] private float streetLength = 100f; // 街の長さ（X方向）
    [SerializeField] private float randomGap = 1f;      // 家と家の隙間のランダム幅
    [SerializeField] private int seed = 0;

    void Start()
    {
        BuildTown();
    }

    public void BuildTown()
    {
        Debug.Log($"[TownBuilder] BuildTown開始 prefab数:{housePrefabs.Length}");

        var random = new System.Random(seed);

        for (int row = 0; row < rowCount; row++)
        {
            float z = row * rowSpacing;
            float x = 0f;

            while (x < streetLength)
            {
                // ランダムに家を選ぶ
                int index = random.Next(housePrefabs.Length);
                GameObject prefab = housePrefabs[index];

                // Spriteのサイズを取得
                SpriteRenderer sr = prefab.GetComponent<SpriteRenderer>();
                float width = sr != null ? sr.sprite.bounds.size.x : 1f;

                // ★追加
                Debug.Log($"prefab:{prefab.name} sr:{sr != null} width:{width} x:{x}");

                // 配置
                Vector3 pos = transform.position + new Vector3(x + width / 2f, 0f, z);
                Instantiate(prefab, pos, Quaternion.identity, transform);

                // 次の家の位置（隙間をランダムに）
                float gap = (float)random.NextDouble() * randomGap;
                x += width + gap;
            }
        }
    }
}