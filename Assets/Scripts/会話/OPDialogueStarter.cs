using UnityEngine;
public class OPDialogueStarter : MonoBehaviour
{
    [SerializeField] private DialogueData dialogueData;
    public void StartDialogue()
    {
        DialogueManager.Instance.StartDialogue(dialogueData);
    }
}