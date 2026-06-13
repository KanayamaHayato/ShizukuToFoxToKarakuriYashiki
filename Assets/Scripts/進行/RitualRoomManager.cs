using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RitualRoomManager : MonoBehaviour
{
    public static RitualRoomManager Instance { get; private set; }

    [Header("儀式の間")]
    [SerializeField] private Transform ritualRoomSpawnPoint;
    [SerializeField] private GameObject ritualRoomObject;

    [Header("ワープ演出")]
    [SerializeField] private CanvasGroup whiteoutCanvasGroup;
    [SerializeField] private float whiteoutSpeed = 0.5f;

    // 参照は自動取得
    private LanternManager lanternManager;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // RitualRoomManager.cs の Start() を変更
    void Start()
    {
        if (whiteoutCanvasGroup != null)
        {
            whiteoutCanvasGroup.alpha = 0f;
            whiteoutCanvasGroup.gameObject.SetActive(true);
        }

        // まずSpawnPointを取得してから
        if (ritualRoomObject == null)
            ritualRoomObject = GameObject.FindWithTag("RitualRoom");

        if (ritualRoomSpawnPoint == null && ritualRoomObject != null)
            ritualRoomSpawnPoint = ritualRoomObject.transform.Find("SpawnPoint");

        
        MazeGenerator mazeGenerator = FindObjectOfType<MazeGenerator>();
        if (mazeGenerator == null)
        {
            Debug.LogError("[RitualRoomManager] MazeGeneratorが見つかりません");
            return;
        }
        Debug.Log("[RitualRoomManager] MazeGenerator発見");

        lanternManager = mazeGenerator.GetLanternManager();
        Debug.Log($"[RitualRoomManager] 取得したLanternManagerのID: {lanternManager.GetInstanceID()}");
        if (lanternManager == null)
        {
            Debug.LogError("[RitualRoomManager] LanternManagerが見つかりません");
            return;
        }
        Debug.Log("[RitualRoomManager] LanternManager発見");

        lanternManager.OnAllLit += StartWarpSequence;
        Debug.Log("[RitualRoomManager] OnAllLitに登録完了");


    }

    void OnDestroy()
    {
        if (lanternManager != null)
            lanternManager.OnAllLit -= StartWarpSequence;
    }

    private void StartWarpSequence()
    {
        Debug.Log("[RitualRoomManager] ワープシーケンス開始！");
        StartCoroutine(WarpCoroutine());
    }

    private IEnumerator WarpCoroutine()
    {
        Debug.Log("[RitualRoomManager] ホワイトアウト開始");
        // 白くフェードアウト
        whiteoutCanvasGroup.alpha = 0f;
        while (whiteoutCanvasGroup.alpha < 1f)
        {
            whiteoutCanvasGroup.alpha += Time.deltaTime * whiteoutSpeed;
            yield return null;
        }

        yield return new WaitForSeconds(1f);


        GameObject player = GameObject.FindWithTag("Player");
        if (player != null && ritualRoomSpawnPoint != null)
        {
            // ★ CharacterControllerを一時無効化してから移動
            var cc = player.GetComponent<CharacterController>();
            cc.enabled = false;
            player.transform.position = ritualRoomSpawnPoint.position;
            cc.enabled = true;
        }

        yield return new WaitForSeconds(0.5f);

        // 白からフェードイン
        while (whiteoutCanvasGroup.alpha > 0f)
        {
            whiteoutCanvasGroup.alpha -= Time.deltaTime * whiteoutSpeed;
            yield return null;
        }
        // 儀式の間に移動後にFogを弱める
        RenderSettings.fogDensity = 0.01f;
        EndingManager.Instance.IsInRitualRoom = true;

        if (LoreManager.Instance != null && LoreManager.Instance.HasAllLores)
        {
            SceneManager.LoadScene("End2");
        }
        else
        {
            EndingManager.Instance.StartRitualSequence();
        }
    }
}