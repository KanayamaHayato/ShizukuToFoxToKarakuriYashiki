using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    private GameObject currentPlayer;

    // PlayerSpawner.cs を修正
    public void Spawn(GameObject startRoom)
    {
        if (currentPlayer != null)
        {
            // ★ 編集モード対応
            if (Application.isPlaying) Destroy(currentPlayer);
            else DestroyImmediate(currentPlayer);
        }

        Transform spawnPoint = startRoom.transform.Find("SpawnPoint");

        Vector3 pos = spawnPoint != null
            ? spawnPoint.position
            : startRoom.transform.position + Vector3.up;

        if (spawnPoint == null)
            Debug.LogWarning("[PlayerSpawner] SpawnPointが見つかりません。部屋の中心にスポーンします。");

        currentPlayer = Instantiate(playerPrefab, pos, Quaternion.identity);
    }

    public void DestroyExisting()
    {
        if (currentPlayer != null)
        {
            // ★ 即時削除（同フレーム内で確実に消す）
            DestroyImmediate(currentPlayer);
            currentPlayer = null;
            return;
        }

        var existing = GameObject.FindWithTag("Player");
        if (existing != null)
            DestroyImmediate(existing);
    }
}