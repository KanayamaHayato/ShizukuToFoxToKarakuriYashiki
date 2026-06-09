using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 怪物を1体ずつランダムにスポーンする。
/// 雫から見えない部屋に出現し、一定時間見失ったら消滅して再スポーン。
/// </summary>
public class EnemySpawnManager : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private GameObject enemyPrefab;

    [Header("設定")]
    [SerializeField] private float spawnInterval = 10f;      // スポーンまでの間隔
    [SerializeField] private float despawnTime = 8f;         // 見失ってから消滅までの時間
    [SerializeField] private float minSpawnDistance = 20f;   // 雫から最低限離れた距離
    [SerializeField] private LayerMask wallLayer;            // 壁レイヤー

    private GameObject currentEnemy;
    private EnemyMove currentEnemyMove;
    private List<GameObject> roomList = new List<GameObject>();

    // 見失いタイマー
    private float lostTimer = 0f;
    private bool isLost = false;

    // ----------------------------------------

    public void RegisterRooms(List<GameObject> rooms)
    {
        roomList = rooms;
    }

    void Update()
    {
        var player = GetPlayer();

        if (player == null || currentEnemy == null) return;

        // 視線チェック（EnemyMoveのCanSeePlayerを参照）
        if (currentEnemyMove != null && !currentEnemyMove.CanSeePlayer)
        {
            lostTimer += Time.deltaTime;
            if (lostTimer >= despawnTime)
            {
                Despawn();
                StartCoroutine(SpawnAfterDelay());
            }
        }
        else
        {
            lostTimer = 0f;
        }
    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnAfterDelay());
    }

    private IEnumerator SpawnAfterDelay()
    {
        yield return new WaitForSeconds(spawnInterval);
        Spawn();
    }

    private void Spawn()
    {
        var player = GetPlayer();
        if (player == null)
        {
            Debug.LogWarning("[EnemySpawnManager] Playerが見つかりません。");
            StartCoroutine(SpawnAfterDelay());
            return;
        }
        // シーン内の全EnemySpawnPointを収集
        var allSpawnPoints = new List<Transform>();
        foreach (var room in roomList)
        {
            var point = room.transform.Find("EnemySpawnPoint");
            if (point != null)
                allSpawnPoints.Add(point);
        }

        // 雫から見えないスポーンポイントを候補にする
        List<Transform> candidates = new List<Transform>();
        foreach (var point in allSpawnPoints)
        {
            if (Vector3.Distance(player.position, point.position) < minSpawnDistance)
                continue;

            Vector3 dir = player.position - point.position;
            if (Physics.Raycast(point.position, dir.normalized, dir.magnitude, wallLayer))
                candidates.Add(point);
        }

        // 候補がなければ距離だけで選ぶ
        if (candidates.Count == 0)
        {
            foreach (var point in allSpawnPoints)
            {
                if (Vector3.Distance(player.position, point.position) >= minSpawnDistance)
                    candidates.Add(point);
            }
        }

        if (candidates.Count == 0)
        {
            Debug.LogWarning("[EnemySpawnManager] スポーン候補がありません。");
            StartCoroutine(SpawnAfterDelay());
            return;
        }

        Transform spawnPoint = candidates[Random.Range(0, candidates.Count)];
        currentEnemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        currentEnemyMove = currentEnemy.GetComponent<EnemyMove>();
        lostTimer = 0f;

        Debug.Log($"[EnemySpawnManager] 怪物スポーン: {spawnPoint.parent.name}");
    }

    private void Despawn()
    {
        if (currentEnemy != null)
        {
            Destroy(currentEnemy);
            currentEnemy = null;
            currentEnemyMove = null;
            lostTimer = 0f;
        }
    }

    private Transform GetPlayer()
    {
        var playerObj = GameObject.FindWithTag("Player");
        return playerObj != null ? playerObj.transform : null;
    }
}