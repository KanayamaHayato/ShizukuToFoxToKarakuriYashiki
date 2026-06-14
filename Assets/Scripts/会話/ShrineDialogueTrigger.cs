using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShrineDialogueTrigger : MonoBehaviour
{
    [Header("セリフ")]
    [SerializeField] private DialogueData openingDialogue;    // シーン開始時
    [SerializeField] private DialogueData findShrineDialogue; // 祠発見時
    [SerializeField] private DialogueData fixShrineDialogue;  // 触った後


    private PlayerInput playerInput;

    void Start()
    {
        var player = FindObjectOfType<ThirdPersonController>();
        if (player != null)
            playerInput = player.GetComponent<PlayerInput>();

        SetPlayerInput(false);

        DialogueManager.Instance.OnDialogueEnd += OnOpeningEnd;
        DialogueManager.Instance.StartDialogue(openingDialogue);
    }

    private void OnOpeningEnd()
    {
        DialogueManager.Instance.OnDialogueEnd -= OnOpeningEnd;
        SetPlayerInput(true);
    }

    public void OnFindShrine()
    {
        DialogueManager.Instance.StartDialogue(findShrineDialogue);
    }

    public void OnFixShrine()
    {
        DialogueManager.Instance.StartDialogue(fixShrineDialogue);
    }

    private void SetPlayerInput(bool enabled)
    {
        if (playerInput != null)
            playerInput.enabled = enabled;
    }
}