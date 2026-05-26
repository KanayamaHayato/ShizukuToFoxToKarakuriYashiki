using System.Collections;
using UnityEngine;

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

        if (ritualRoomObject != null)
            ritualRoomObject.SetActive(false);

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

        // ★ 儀式の間が完成したらコメントを外す
        // if (ritualRoomObject != null)
        //     ritualRoomObject.SetActive(true);

        GameObject player = GameObject.FindWithTag("Player");
        // if (player != null && ritualRoomSpawnPoint != null)
        //     player.transform.position = ritualRoomSpawnPoint.position;

        yield return new WaitForSeconds(0.5f);

        // 白からフェードイン
        while (whiteoutCanvasGroup.alpha > 0f)
        {
            whiteoutCanvasGroup.alpha -= Time.deltaTime * whiteoutSpeed;
            yield return null;
        }

        // ★ 屋代のセリフも後で
        // DialogueManager.Instance.StartDialogue(yasiroDialogue);
    }
}