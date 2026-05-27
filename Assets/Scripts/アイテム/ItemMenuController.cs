using UnityEngine;

public class ItemMenuController : MonoBehaviour
{
    [SerializeField] private Inventory inventory;

    [SerializeField] private ItemSlot[] slots;
    private int selectedIndex = -1;

    [SerializeField] private ItemDetailPanel detailPanel;

    public void SelectSlot(int index)
    {
        if (!slots[index].HasItem()) return;

        if (selectedIndex >= 0)
            slots[selectedIndex].SetSelected(false);

        selectedIndex = index;
        slots[selectedIndex].SetSelected(true);

        var item = slots[index].GetItem();
        detailPanel.Show(item);
    }

    void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            ItemData item = inventory.GetItem(i);
            slots[i].SetItem(item);
            slots[i].SetSelected(false);
        }

        selectedIndex = -1;
        detailPanel.Show(null);
    }

    public void DropSelected()
    {
        if (selectedIndex < 0) return;

        inventory.RemoveAt(selectedIndex);
        Refresh();
    }
}