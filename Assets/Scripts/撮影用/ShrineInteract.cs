using Cinemachine;
using StarterAssets;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShrineInteract : MonoBehaviour
{
    [Header("祠オブジェクト")]
    [SerializeField] private GameObject brokenShrine;
    [SerializeField] private GameObject fixedShrine;

    [Header("暗転")]
    [SerializeField] private float fadeDuration = 1.0f;

    [Header("アニメーション")]
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private RuntimeAnimatorController fixShrineController;
    private RuntimeAnimatorController originalController;

    [Header("座る位置")]
    [SerializeField] private Transform kneelPosition;

    [Header("カメラ")]
    [SerializeField] private CinemachineVirtualCamera shrineCamera;
    [SerializeField] private CinemachineVirtualCamera shrineFixedCamera; // 2つ目の定点カメラ

    [Header("エフェクト")]
    [SerializeField] private ParticleSystem shrineParticle;

    [Header("セリフ")]
    [SerializeField] private ShrineDialogueTrigger dialogueTrigger;
    [SerializeField] private DialogueData warpDialogue; // ワープ直前セリフ

    private bool playerNear = false;
    private bool alreadyTouched = false;

    private void Start()
    {
        fixedShrine.SetActive(false);
        shrineParticle.gameObject.SetActive(false);
    }
    void Update()
    {
        if (playerNear && !alreadyTouched && Input.GetKeyDown(KeyCode.E))
            StartCoroutine(ShrineSequence());
    }

    private IEnumerator ShrineSequence()
    {
        alreadyTouched = true;
        InteractUIManager.Instance.Hide();
        GameObject player = GameObject.FindWithTag("Player");

        // 実行中のセリフを強制終了
        DialogueManager.Instance.ForceStop();
        yield return null; // 1フレーム待つ
        dialogueTrigger.OnFixShrine();

        // 入力を無効化
        var input = player.GetComponent<StarterAssetsInputs>();
        if (input != null) input.move = Vector2.zero;

        // PlayerInputも無効化
        var playerInput = player.GetComponent<UnityEngine.InputSystem.PlayerInput>();
        if (playerInput != null) playerInput.enabled = false;

        // セリフが終わるまで待つ
        bool dialogueEnded = false;
        DialogueManager.Instance.OnDialogueEnd += () => dialogueEnded = true;
        yield return new WaitUntil(() => dialogueEnded);
        
        // プレイヤーを祠の前に移動
        var cc = player.GetComponent<CharacterController>();
        if (cc != null)
        {
            cc.enabled = false;
            player.transform.position = kneelPosition.position;
            player.transform.rotation = kneelPosition.rotation;
            cc.enabled = true;
        }
        else
        {
            player.transform.position = kneelPosition.position;
            player.transform.rotation = kneelPosition.rotation;
        }

        // アニメーション開始
        originalController = playerAnimator.runtimeAnimatorController;
        playerAnimator.runtimeAnimatorController = fixShrineController;

        // カメラ切り替え
        shrineCamera.Priority = 20; // 通常カメラより高くする

        yield return new WaitForSeconds(12f);

        // カメラを2つ目の定点に切り替え
        shrineCamera.Priority = 0;
        shrineFixedCamera.Priority = 20;

        // アニメーションを元に戻す
        playerAnimator.runtimeAnimatorController = originalController;

        yield return StartCoroutine(FadeManager.Instance.FadeOut());

        // 祠差し替え
        brokenShrine.SetActive(false);
        fixedShrine.SetActive(true);

        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(FadeManager.Instance.FadeIn());

        yield return new WaitForSeconds(2f);

        shrineParticle.gameObject.SetActive(true);
        shrineParticle.Play();

        // ワープ直前セリフ開始
        DialogueManager.Instance.StartDialogue(warpDialogue);

        yield return new WaitForSeconds(6f); // 光を見せる時間

        // ホワイトアウト
        yield return StartCoroutine(FadeManager.Instance.WhiteOut());

        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene("Maze");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"TriggerEnter: {other.gameObject.name}");
        if (other.CompareTag("Player"))
        {
            Debug.Log("Playerタグ確認OK");
            playerNear = true;
            if (!alreadyTouched)
                InteractUIManager.Instance.Show();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = false;
            InteractUIManager.Instance.Hide();
        }
    }
}