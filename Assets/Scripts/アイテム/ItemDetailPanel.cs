using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemDetailPanel : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Inventory inventory;
    [SerializeField] private ItemMenuController menuController;
    [SerializeField] private DropItemSpawner dropSpawner;
    [SerializeField] private Button useButton;
    [SerializeField] private HeartSystem heartSystem;
    [SerializeField] private TimeStopManager timeStopManager;

    private ItemData currentItem;

    void Awake()
    {
        Hide();
    }

    public void Show(ItemData item)
    {
        if (item == null)
        {
            Hide();
            return;
        }

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        icon.sprite = item.icon;
        icon.enabled = true;
        nameText.text = item.itemName;
        descriptionText.text = item.description;

        currentItem = item;
        useButton.interactable = true;

        if (item.itemType == ItemType.Heal)
        {
            bool canUse = heartSystem.life < heartSystem.maxLife;
            useButton.interactable = canUse;

            if (!canUse)
            {
                descriptionText.text =
                    item.description +
                    "\n<color=#FFD700><b>Ç®éDÇÃóÕÇÕñûÇøÇƒÇ¢Ç‹Ç∑ÅB</b></color>";
            }
        }

        if (item.itemType == ItemType.TimeStop)
        {
            bool canUse = !timeStopManager.IsStopping;

            useButton.interactable = canUse;

            if (!canUse)
            {
                descriptionText.text =
                    item.description +
                    "\n<color=#FFD700><b>éûÇÃó¨ÇÍÇÕÇ‹Çæê√Ç‹Ç¡ÇƒÇ¢Ç‹ÇπÇÒÅB</b></color>";
            }
        }
    }

    public void Hide()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        icon.enabled = false;
        nameText.text = "";
        descriptionText.text = "";

        currentItem = null;
    }

    public void OnClickDrop()
    {
        if (currentItem == null) return;

        dropSpawner.Drop(currentItem);
        inventory.Remove(currentItem);
        menuController.Refresh();
        Hide();
    }

    public void OnClickUse()
    {
        if (currentItem == null) return;

        if (currentItem.itemType == ItemType.Heal)
        {
            heartSystem.Heal(currentItem.healAmount);
            menuController.DropSelected();
            Hide();
        }
        else if (currentItem.itemType == ItemType.TimeStop)
        {
            timeStopManager.StopEnemies(currentItem.timeStopSeconds);
            menuController.DropSelected();
            Hide();
        }
    }
}