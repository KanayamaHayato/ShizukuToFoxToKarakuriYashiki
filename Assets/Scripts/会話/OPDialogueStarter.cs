using UnityEngine;
public class OPDialogueStarter : MonoBehaviour
{
    [SerializeField] private DialogueData dialogueData;
    void Start()
    {
        DialogueManager.Instance.StartDialogue(dialogueData);
    }
}