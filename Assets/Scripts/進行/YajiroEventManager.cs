using System.Collections;
using UnityEngine;
using Cinemachine;

public class YajiroEventManager : MonoBehaviour
{
    [Header("やじろ")]
    [SerializeField] private GameObject yajiroObject;
    [Header("アニメーション")]
    [SerializeField] private Animator yajiroAnimator;


    [Header("カメラ")]
    [SerializeField] private CinemachineVirtualCamera camFPS;
    [SerializeField] private float lookDownAngle = 40f;
    [SerializeField] private float lookDownDuration = 1.5f;

    [Header("セリフ")]
    [SerializeField] private DialogueData dialoguePart1; // 「やあ」
    [SerializeField] private DialogueData dialoguePart2; // 「下を見ろ！」
    [SerializeField] private DialogueData dialoguePart3; // 目が合った後

    [Header("最終部屋やじろ位置")]
    [SerializeField] private Transform yajiroFinalTransform;

    private GameObject player;
    private MonoBehaviour playerController;
    private MonoBehaviour playerInput;

    void Start()
    {
        if (SaveManager.Instance != null && SaveManager.Instance.loadPending)
        {
            SaveManager.Instance.ConsumePending();
            return;
        }

        Debug.Log("[YajiroEventManager] Start()呼ばれた");
        StartCoroutine(WaitForPlayerAndStart());
    }

    private IEnumerator WaitForPlayerAndStart()
    {
        var spawner = FindObjectOfType<PlayerSpawner>();
        Debug.Log($"[YajiroEventManager] Spawner取得: {(spawner == null ? "NULL" : spawner.name)}");

        int frame = 0;
        while (spawner == null || spawner.GetCurrentPlayer() == null)
        {
            if (frame % 60 == 0)
                Debug.Log($"[YajiroEventManager] 待機中... GetCurrentPlayer={spawner?.GetCurrentPlayer()?.name ?? "NULL"}");
            frame++;
            yield return null;
        }
        Debug.Log("[YajiroEventManager] Player検出、イベント開始");
        StartCoroutine(YajiroEventCoroutine());
    }

    private IEnumerator YajiroEventCoroutine()
    {
        // やじろをタグで動的取得
        yajiroObject = GameObject.FindWithTag("Yajiro");

        // 雫をタグで動的取得
        player = GameObject.FindWithTag("Player");
        Debug.Log($"[YajiroEventManager] Player取得: {player.name} 位置: {player.transform.position}");
        if (player == null)
        {
            Debug.LogError("[YajiroEventManager] Playerが見つかりません");
            yield break;
        }

        // プレイヤー操作を無効化
        playerController = player.GetComponent<StarterAssets.ThirdPersonController>();
        playerInput = player.GetComponent<StarterAssets.StarterAssetsInputs>();
        if (playerController != null) playerController.enabled = false;
        if (playerInput != null) playerInput.enabled = false;

        // やじろをプレイヤーの正面に配置
        if (yajiroObject != null)
        {
            Vector3 yajiroPos = player.transform.position + player.transform.forward * 1.5f;
            yajiroPos.y = player.transform.position.y - 0.3f;
            yajiroObject.transform.position = yajiroPos;
            yajiroObject.SetActive(false);
        }

        // Cam_FPSをプレイヤー頭部にセット
        Transform face = player.transform.Find("CinemachineCameraTarget");
        if (face != null)
        {
            camFPS.transform.position = face.position + Vector3.up * 0.3f;
            camFPS.transform.rotation = face.rotation;
        }

        // Cam_FPSを優先
        camFPS.Priority = 20;

        yield return new WaitForSeconds(0.5f);

        // Part1：「やあ」
        DialogueManager.Instance.StartDialogue(dialoguePart1);
        yield return new WaitUntil(() => !DialogueManager.Instance.IsRunning);

        // やじろアクティブ化
        if (yajiroObject != null)
        {
            yajiroObject.SetActive(true);
            yajiroAnimator = yajiroObject.GetComponent<Animator>();
            yajiroAnimator.SetBool("IsUppereye", true);
        }

        yield return new WaitForSeconds(0.5f);

        // Part2：「下を見ろ！」
        DialogueManager.Instance.StartDialogue(dialoguePart2);
        yield return new WaitUntil(() => !DialogueManager.Instance.IsRunning);

        // カメラを下に向ける
        yield return StartCoroutine(LookDownCoroutine());

        yield return new WaitForSeconds(0.5f);

        // Part3：目が合った後
        if (dialoguePart3 != null)
        {
            DialogueManager.Instance.StartDialogue(dialoguePart3);
            yield return new WaitUntil(() => !DialogueManager.Instance.IsRunning);
        }

        // 屋代がすっと消える
        if (yajiroObject != null)
        {
            yajiroAnimator.SetBool("IsUppereye", false);
            yajiroObject.SetActive(false);
        }

        // やじろを最終部屋の位置に移動して非表示
        if (yajiroObject != null && yajiroFinalTransform != null)
        {
            Debug.Log($"[YajiroEventManager] やじろを最終部屋に移動: {yajiroFinalTransform.position}");
            yajiroObject.transform.position = yajiroFinalTransform.position;
            yajiroObject.transform.rotation = yajiroFinalTransform.rotation;
            yajiroObject.SetActive(false);
        }
        else
        {
            Debug.LogError($"[YajiroEventManager] 移動失敗 yajiroObject={yajiroObject?.name ?? "NULL"} yajiroFinalTransform={yajiroFinalTransform?.name ?? "NULL"}");
        }

        yield return new WaitForSeconds(0.3f);

        // カメラを元に戻す
        camFPS.Priority = 0;

        // プレイヤー操作を有効化
        if (playerController != null) playerController.enabled = true;
        if (playerInput != null) playerInput.enabled = true;
    }

    private IEnumerator LookDownCoroutine()
    {
        Quaternion startRot = camFPS.transform.rotation;
        Quaternion endRot = startRot * Quaternion.Euler(lookDownAngle, 0f, 0f);

        float elapsed = 0f;
        while (elapsed < lookDownDuration)
        {
            elapsed += Time.deltaTime;
            camFPS.transform.rotation = Quaternion.Lerp(startRot, endRot, elapsed / lookDownDuration);
            yield return null;
        }

        camFPS.transform.rotation = endRot;
    }
}