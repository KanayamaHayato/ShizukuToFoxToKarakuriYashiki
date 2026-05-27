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
}