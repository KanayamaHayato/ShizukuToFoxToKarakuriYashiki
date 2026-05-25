// DialogueTest.cs（確認後に削除してOK）
using UnityEngine;

public class DialogueTest : MonoBehaviour
{
    [SerializeField] private DialogueData testData;

    void Start()
    {
        DialogueManager.Instance.StartDialogue(testData);
    }
}