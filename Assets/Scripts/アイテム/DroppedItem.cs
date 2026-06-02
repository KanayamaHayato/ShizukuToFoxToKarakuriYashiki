using UnityEngine;
using System.Collections;

public class DroppedItem : MonoBehaviour
{
    public ItemData itemData;

    private Inventory inventory;
    private InteractPanel interactPanel;
    private Transform player;

    private bool isShowing = false;
    private bool isShowingFullMessage = false;

    [SerializeField] private float pickupDistance = 2.0f;

    void Awake()
    {
        interactPanel = FindObjectOfType<InteractPanel>(true);
    }

    void Start()
    {
        inventory = FindObjectOfType<Inventory>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        if (player == null || inventory == null || itemData == null || interactPanel == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= pickupDistance)
        {
            if (!isShowingFullMessage)
            {
                interactPanel.Show("Eキーで拾う");
            }

            isShowing = true;

            if (!isShowingFullMessage && Input.GetKeyDown(KeyCode.E))
            {
                bool added = inventory.Add(itemData);

                if (added)
                {
                    interactPanel.Hide();
                    Destroy(gameObject);
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

        interactPanel.Show("持ち物がいっぱいです！");

        yield return new WaitForSeconds(2f);

        isShowingFullMessage = false;

        if (isShowing)
        {
            interactPanel.Show("Eキーで拾う");
        }
        else
        {
            interactPanel.Hide();
        }
    }

    public void Setup(ItemData data)
    {
        itemData = data;
    }
}