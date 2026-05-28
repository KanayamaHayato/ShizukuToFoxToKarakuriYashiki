using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private GameObject selectedFrame;

    private ItemData currentItem;

    public bool HasItem()
    {
        return currentItem != null;
    }

    void Start()
    {
        if (selectedFrame != null)
        {
            selectedFrame.SetActive(false);
        }
    }

    public void SetItem(ItemData item)
    {
        currentItem = item;

        if (item == null)
        {
            icon.enabled = false;
            GetComponent<Button>().interactable = false;
            SetSelected(false);
            return;
        }

        icon.enabled = true;
        icon.sprite = item.icon;
        GetComponent<Button>().interactable = true;
    }

    public void SetSelected(bool selected)
    {
        selectedFrame.SetActive(selected);
    }

    public void OnClick()
    {
        Debug.Log("Slot clicked");

        if (selectedFrame != null)
        {
            selectedFrame.SetActive(true);
        }
    }

    public ItemData GetItem()
    {
        return currentItem;
    }
}