using System.Collections;
using UnityEngine;

public class YajiroCommentManager : MonoBehaviour
{
    [Header("やじろのセリフ（灯籠1個目〜6個目）")]
    [SerializeField] private DialogueData[] lanternDialogues; // 最大6個

    private LanternManager lanternManager;
    private MonoBehaviour playerController;
    private MonoBehaviour playerInput;
    private GameObject player;
    [SerializeField] private MazeGenerator mazeGenerator;

    void Start()
    {
        Debug.Log("[YajiroCommentManager] Start()呼ばれた");
        StartCoroutine(WaitForLanternManager());
    }

    void OnDestroy()
    {
        if (lanternManager != null)
            lanternManager.OnLitCountChanged -= OnLanternLit;
    }

    private void OnLanternLit(int litCount, int totalCount)
    {
        // 最後の1個は反応しない
        if (litCount >= totalCount) return;

        int index = litCount - 1;
        if (index < 0 || index >= lanternDialogues.Length) return;
        if (lanternDialogues[index] == null) return;

        StartCoroutine(LanternEventCoroutine(index));
    }

    private IEnumerator LanternEventCoroutine(int index)
    {
        // プレイヤー取得
        player = GameObject.FindWithTag("Player");
        if (player == null) yield break;

        // 操作無効化
        playerController = player.GetComponent<StarterAssets.ThirdPersonController>();
        playerInput = player.GetComponent<StarterAssets.StarterAssetsInputs>();
        if (playerController != null) playerController.enabled = false;
        if (playerInput != null) playerInput.enabled = false;

        // 30m以内の怪物を消滅
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var enemy in enemies)
        {
            if (Vector3.Distance(player.transform.position, enemy.transform.position) <= 30f)
            {
                Destroy(enemy);
            }
        }

        yield return new WaitForSeconds(0.5f);

        // セリフ表示
        DialogueManager.Instance.StartDialogue(lanternDialogues[index]);
        yield return new WaitUntil(() => !DialogueManager.Instance.IsRunning);

        // 操作有効化
        if (playerController != null) playerController.enabled = true;
        if (playerInput != null) playerInput.enabled = true;
    }
    private IEnumerator WaitForLanternManager()
    {
        if (mazeGenerator == null)
            mazeGenerator = FindObjectOfType<MazeGenerator>();

        while (mazeGenerator == null || mazeGenerator.GetLanternManager() == null)
            yield return null;

        lanternManager = mazeGenerator.GetLanternManager();
        lanternManager.OnLitCountChanged += OnLanternLit;
        Debug.Log("[YajiroCommentManager] LanternManager登録完了");
    }
}