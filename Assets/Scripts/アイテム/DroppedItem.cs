using UnityEngine;
using System.Collections;

public class DroppedItem : MonoBehaviour
{
    public ItemData itemData;
    private Inventory inventory;
    private bool isShowing = false;
    private bool isShowingFullMessage = false;

    [SerializeField] private float pickupDistance = 2.0f;
    [SerializeField] private InteractPanel interactPanel;

    private Transform player;

    void Start()
    {
        inventory = FindObjectOfType<Inventory>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        if (interactPanel == null)
            interactPanel = FindObjectOfType<InteractPanel>(true);
    }

    void Update()
    {
        if (player == null || inventory == null || itemData == null || interactPanel == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= pickupDistance)
        {
            if (!isShowingFullMessage)
                interactPanel.Show("EƒL[‚ÅE‚¤");

            isShowing = true;

            if (!isShowingFullMessage && Input.GetKeyDown(KeyCode.E))
            {
                bool added = inventory.Add(itemData);

                if (added)
                {
                    interactPanel.Hide();
                    Destroy(gameObject);
                    return;
                }
                else
                {
                    StartCoroutine(ShowFullMessage());
                }
            }
        }
        else
        {
            if (isShowing)
            {
                interactPanel.Hide();
                isShowing = false;
            }
        }
    }

    private IEnumerator ShowFullMessage()
    {
        isShowingFullMessage = true;

        interactPanel.Show("Ž‚¿•¨‚ª‚¢‚Á‚Ï‚¢‚Å‚·I");

        yield return new WaitForSeconds(2f);

        isShowingFullMessage = false;
    }

    public void Setup(ItemData data)
    {
        itemData = data;
    }
}