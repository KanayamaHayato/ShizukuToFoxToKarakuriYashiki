using UnityEngine;

public class ShrineDialogueTrigger : MonoBehaviour
{
    [Header("セリフ")]
    [SerializeField] private DialogueData openingDialogue;    // シーン開始時
    [SerializeField] private DialogueData findShrineDialogue; // 祠発見時
    [SerializeField] private DialogueData fixShrineDialogue;  // 触った後

    void Start()
    {
        DialogueManager.Instance.StartDialogue(openingDialogue);
    }

    public void OnFindShrine()
    {
        DialogueManager.Instance.StartDialogue(findShrineDialogue);
    }

    public void OnFixShrine()
    {
        DialogueManager.Instance.StartDialogue(fixShrineDialogue);
    }
}