using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using StarterAssets;
using System.Collections;

public class ShrineFindTrigger : MonoBehaviour
{
    [SerializeField] private ShrineDialogueTrigger dialogueTrigger;
    [SerializeField] private CinemachineVirtualCamera findShrineCamera;
    [SerializeField] private float lookDuration = 3f;
    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !triggered)
        {
            triggered = true;

            // 入力無効化
            var input = other.GetComponent<StarterAssetsInputs>();
            if (input != null) input.move = Vector2.zero;
            var playerInput = other.GetComponent<PlayerInput>();
            if (playerInput != null) playerInput.enabled = false;

            // 実行中のセリフを強制終了
            DialogueManager.Instance.ForceStop();

            dialogueTrigger.OnFindShrine();
            StartCoroutine(ShowShrineCamera(other.gameObject));
        }
    }

    private IEnumerator ShowShrineCamera(GameObject player)
    {
        yield return new WaitForSeconds(9f);
        findShrineCamera.Priority = 20;
        yield return new WaitForSeconds(lookDuration);
        findShrineCamera.Priority = 0;

        // 入力再有効化
        var playerInput = player.GetComponent<PlayerInput>();
        if (playerInput != null) playerInput.enabled = true;
    }
}