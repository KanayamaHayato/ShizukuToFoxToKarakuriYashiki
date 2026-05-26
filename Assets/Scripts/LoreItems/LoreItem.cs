using UnityEngine;

public class LoreItem : MonoBehaviour
{
    [Header("伝記情報")]
    [SerializeField] private int loreNumber;
    [SerializeField] private string loreName;
    [SerializeField] private DialogueData loreText;

    private bool playerNear = false;
    private bool alreadyPicked = false;

    void Update()
    {
        if (playerNear && !alreadyPicked && Input.GetKeyDown(KeyCode.E))
            PickUp();
    }

    private void PickUp()
    {
        alreadyPicked = true;
        InteractUIManager.Instance.Hide(); // ★
        LoreManager.Instance.CollectLore(this);
        Debug.Log($"伝記 No.{loreNumber} 「{loreName}」を入手");
        gameObject.SetActive(false);
    }

    public int LoreNumber => loreNumber;
    public string LoreName => loreName;
    public DialogueData LoreText => loreText;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = true;
            if (!alreadyPicked)
                InteractUIManager.Instance.Show(); // ★
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = false;
            InteractUIManager.Instance.Hide(); // ★
        }
    }
}