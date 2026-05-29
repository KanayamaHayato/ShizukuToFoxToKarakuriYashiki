using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    public ItemData itemData;
    private Inventory inventory;

    [SerializeField] private float pickupDistance = 2.0f;
    [SerializeField] private InteractPanel interactPanel;

    private Transform player;

    void Start()
    {
        inventory = FindObjectOfType<Inventory>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        if (interactPanel == null)
        {
            interactPanel = FindObjectOfType<InteractPanel>(true);
        }
    }

    void Update()
    {
        if (player == null || inventory == null || itemData == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= pickupDistance)
        {
            interactPanel.Show("Eキーで拾う");

            if (Input.GetKeyDown(KeyCode.E))
            {
                bool added = inventory.Add(itemData);

                if (added)
                {
                    interactPanel.Hide();
                    Destroy(gameObject);
                }
                else
                {
                    interactPanel.Show("インベントリがいっぱいです");
                }
            }
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