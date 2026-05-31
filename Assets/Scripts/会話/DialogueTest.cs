using UnityEngine;

public class DialogueTest : MonoBehaviour
{
    [SerializeField] private DialogueData testData;

    void Start()
    {
        DialogueManager.Instance.StartDialogue(testData);
    }
}