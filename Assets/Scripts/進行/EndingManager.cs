using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class EndingManager : MonoBehaviour
{
    public static EndingManager Instance { get; private set; }

    [Header("セリフ")]
    [SerializeField] private DialogueData ritualDialogue; // ワープ直後のやじろセリフ
    [Header("エンディングセリフ")]
    [SerializeField] private DialogueData endingDialogue; // 灯籠が降るときのセリフ
    [SerializeField] private DialogueData end2Dialogue;
    [Header("エンディングテキスト")]
    [SerializeField] private GameObject endingTextObject; // "END\n最後の灯籠を灯す" を表示するUI
    [SerializeField] private GameObject end2TextObject;

    [Header("灯籠")]
    [SerializeField] private GameObject lanternPrefab;   // 降らせる灯籠プレハブ
    [SerializeField] private int lanternCount = 30;
    [SerializeField] private float lanternSpawnRadius = 10f;
    [SerializeField] private float lanternFallSpeed = 2f;
    [SerializeField] private float lanternSpawnHeight = 15f;
    [SerializeField] private Color lanternLightColor = Color.red;

    [Header("カメラ")]
    [SerializeField] private CinemachineVirtualCamera camFPS;
    [SerializeField] private Transform yajiroFaceTarget; // やじろの顔あたりのTransform

    [Header("暗転")]
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private float fadeDuration = 2f;

    [Header("やじろ")]
    [SerializeField] private GameObject yajiroObject;
    [SerializeField] private Transform yajiroFinalTransform; // ← 追加

    public bool IsInRitualRoom { get; set; } = false;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // 暗転CanvasGroupを最初は非表示に
        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 0f;
            
        }
    }

    // RitualRoomManagerから呼ぶ（ワープ直後）
    public void StartRitualSequence()
    {
        StartCoroutine(RitualSequenceCoroutine());
    }

    // 最後の灯籠を灯したとき
    public void OnRitualLanternLit()
    {
        StartCoroutine(EndingCoroutine());
    }

    private IEnumerator RitualSequenceCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

        if (yajiroObject != null)
        {
            yajiroObject.SetActive(true);

            // Animatorはオフのまま
            var anim = yajiroObject.GetComponent<Animator>();
            if (anim != null) anim.enabled = false;

            yajiroObject.transform.position = yajiroFinalTransform.position;
            yajiroObject.transform.rotation = yajiroFinalTransform.rotation;
        }

        if (ritualDialogue != null)
        {
            DialogueManager.Instance.StartDialogue(ritualDialogue);
            yield return new WaitUntil(() => !DialogueManager.Instance.IsRunning);
        }
    }

    private IEnumerator EndingCoroutine()
    {
        // プレイヤー操作停止
        var player = GameObject.FindWithTag("Player");
        var playerController = player?.GetComponent<StarterAssets.ThirdPersonController>();
        var playerInput = player?.GetComponent<StarterAssets.StarterAssetsInputs>();
        if (playerController != null) playerController.enabled = false;
        if (playerInput != null) playerInput.enabled = false;

        // 灯籠を降らせる
        StartCoroutine(SpawnFallingLanterns());

        // やじろのアップに切り替え
        if (camFPS != null && yajiroFaceTarget != null)
        {
            camFPS.transform.position = yajiroFaceTarget.position + Vector3.forward * 2f + Vector3.up * 0.5f;
            camFPS.transform.LookAt(yajiroFaceTarget);
            camFPS.Priority = 20;
        }

        // 灯籠が降りながら会話
        if (endingDialogue != null)
        {
            DialogueManager.Instance.StartDialogue(endingDialogue);
            yield return new WaitUntil(() => !DialogueManager.Instance.IsRunning);
        }

        yield return new WaitForSeconds(2f);

        // 暗転
        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.gameObject.SetActive(true);
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                fadeCanvasGroup.alpha = elapsed / fadeDuration;
                yield return null;
            }
        }
        yield return new WaitForSeconds(3f);
        // ENDテキスト表示
        if (endingTextObject != null)
            endingTextObject.SetActive(true);

        // EキーかクリックでタイトルへBrand
        yield return new WaitUntil(() =>
            Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0));


        // タイトルへ
        SceneManager.LoadScene("MainMenu");
    }

    private IEnumerator SpawnFallingLanterns()
    {
        GameObject player = GameObject.FindWithTag("Player");
        Vector3 center = player != null ? player.transform.position : Vector3.zero;

        for (int i = 0; i < lanternCount; i++)
        {
            Vector3 spawnPos = center + new Vector3(
                Random.Range(-lanternSpawnRadius, lanternSpawnRadius),
                lanternSpawnHeight,
                Random.Range(-lanternSpawnRadius, lanternSpawnRadius)
            );

            GameObject lantern = Instantiate(lanternPrefab, spawnPos, Quaternion.identity);

            // 赤いライトをつける
            Light light = lantern.GetComponentInChildren<Light>();
            if (light != null)
            {
                light.color = lanternLightColor;
            }
            else
            {
                // ライトがなければ追加
                Light newLight = lantern.AddComponent<Light>();
                newLight.color = lanternLightColor;
                newLight.intensity = 10f;
                newLight.range = 10f;
            }

            // 落下させる
            StartCoroutine(FallLantern(lantern));

            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator FallLantern(GameObject lantern)
    {
        while (lantern != null)
        {
            lantern.transform.position += Vector3.down * lanternFallSpeed * Time.deltaTime;
            yield return null;
        }
    }

    public void StartRitualSequenceEnd2()
    {
        IsInRitualRoom = false; // 灯籠を触れないようにする
        StartCoroutine(RitualSequenceCoroutineEnd2());
    }

    private IEnumerator RitualSequenceCoroutineEnd2()
    {
        yield return new WaitForSeconds(0.5f);
        // エンド2のシーケンスをここに実装
        Debug.Log("[EndingManager] エンド2開始");
        yield return new WaitForSeconds(0.5f);
        // プレイヤー操作停止
        var player = GameObject.FindWithTag("Player");
        var playerController = player?.GetComponent<StarterAssets.ThirdPersonController>();
        var playerInput = player?.GetComponent<StarterAssets.StarterAssetsInputs>();
        if (playerController != null) playerController.enabled = false;
        if (playerInput != null) playerInput.enabled = false;
        // アニメーションをアイドルに戻す
        var animator = player?.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetFloat("Speed", 0f);
            animator.SetFloat("MotionSpeed", 0f);
        }

        // 暗転
        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.gameObject.SetActive(true);
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                fadeCanvasGroup.alpha = elapsed / fadeDuration;
                yield return null;
            }
        }

        // やじろ表示
        if (yajiroObject != null)
        {
            yajiroObject.SetActive(true);
            var anim = yajiroObject.GetComponent<Animator>();
            if (anim != null) anim.enabled = false;
            yajiroObject.transform.position = yajiroFinalTransform.position;
            yajiroObject.transform.rotation = yajiroFinalTransform.rotation;
        }
        // プレイヤーをやじろの前に移動
        if (player != null && yajiroFinalTransform != null)
            player.transform.position = yajiroFinalTransform.position
                        + yajiroFinalTransform.forward * 2.5f;  // 前後の距離

       // Y軸回転だけやじろの方を向かせる
        Vector3 dir = yajiroFinalTransform.position - player.transform.position;
        dir.y = 0f;
        player.transform.rotation = Quaternion.LookRotation(dir);

        yield return new WaitForSeconds(0.5f);
        // 明転
        if (fadeCanvasGroup != null)
        {
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                fadeCanvasGroup.alpha = 1f - (elapsed / fadeDuration);
                yield return null;
            }
            fadeCanvasGroup.alpha = 0f;
            fadeCanvasGroup.gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(0.5f);

        // エンド2会話
        if (end2Dialogue != null)
        {
            DialogueManager.Instance.StartDialogue(end2Dialogue);
            yield return new WaitUntil(() => !DialogueManager.Instance.IsRunning);
        }
        if (end2TextObject != null)
            end2TextObject.SetActive(true);

        // 暗転
        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.gameObject.SetActive(true);
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                fadeCanvasGroup.alpha = elapsed / fadeDuration;
                yield return null;
            }
        }

        yield return new WaitForSeconds(1f);

        // End2シーンへ
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}