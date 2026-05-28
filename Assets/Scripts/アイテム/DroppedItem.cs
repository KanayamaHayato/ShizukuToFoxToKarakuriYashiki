using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    public ItemData itemData;
    private Inventory inventory;

    [SerializeField] private float pickupDistance = 2.0f;
    private Transform player;

    void Start()
    {
        inventory = FindObjectOfType<Inventory>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            Debug.Log("Player found");
        }
        else
        {
            Debug.Log("Player not found");
        }
    }

    void Update()
    {
        if (player == null || inventory == null || itemData == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= pickupDistance)
        {
            Debug.Log("EƒL[‚ÅE‚¦‚é");

            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("E‰Ÿ‚µ‚½");

                if (itemData == null)
                {
                    Debug.Log("ItemData ‚ª“ü‚Á‚Ä‚È‚¢");
                    return;
                }

                bool added = inventory.Add(itemData);

                if (added)
                {
                    Debug.Log(itemData.itemName + " ‚ðE‚Á‚½");
                    Destroy(gameObject);
                }
                else
                {
                    Debug.Log("Inventory ‚ª–ž”t");
                }
            }
        }
    }

    public void Setup(ItemData data)
    {
        itemData = data;
    }
}