using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanternRandomSpawner : MonoBehaviour
{
    [Header("配置する灯籠Prefab")]
    public GameObject lanternPrefab;

    [Header("床")]
    public Transform floor;

    [Header("生成数")]
    public int lanternCount = 5;

    [Header("マスの間隔")]
    public float gridSize = 3.0f;

    [Header("壁から離す距離")]
    public float wallCheckRadius = 1.0f;

    [Header("壁レイヤー")]
    public LayerMask wallLayer;

    [Header("灯籠の高さ")]
    public float lanternY = 0.5f;

    void Start()
    {
        SpawnLanterns();
    }

    void SpawnLanterns()
    {
        List<Vector3> candidatePositions = new List<Vector3>();

        Vector3 floorScale = floor.localScale;
        Vector3 floorPos = floor.position;

        // Cube床の場合、UnityのCubeの基本サイズは1なのでscaleをそのまま広さとして使える
        float floorWidth = floorScale.x;
        float floorDepth = floorScale.z;

        float startX = floorPos.x - floorWidth / 2f;
        float endX = floorPos.x + floorWidth / 2f;
        float startZ = floorPos.z - floorDepth / 2f;
        float endZ = floorPos.z + floorDepth / 2f;

        for (float x = startX; x <= endX; x += gridSize)
        {
            for (float z = startZ; z <= endZ; z += gridSize)
            {
                Vector3 randomOffset = new Vector3(
                    Random.Range(-gridSize * 0.3f, gridSize * 0.3f),
                    0,
                    Random.Range(-gridSize * 0.3f, gridSize * 0.3f)
                );

                Vector3 pos = new Vector3(x, lanternY, z) + randomOffset;

                // 壁に近すぎる場所は除外
                bool nearWall = Physics.CheckSphere(pos, wallCheckRadius, wallLayer);

                if (!nearWall)
                {
                    candidatePositions.Add(pos);
                }
            }
        }

        // 候補をシャッフル
        Shuffle(candidatePositions);

        int spawnCount = Mathf.Min(lanternCount, candidatePositions.Count);

        for (int i = 0; i < spawnCount; i++)
        {
            Instantiate(lanternPrefab, candidatePositions[i], Quaternion.identity);
        }

        Debug.Log("灯籠を " + spawnCount + " 個配置しました");
    }

    void Shuffle(List<Vector3> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);

            Vector3 temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}