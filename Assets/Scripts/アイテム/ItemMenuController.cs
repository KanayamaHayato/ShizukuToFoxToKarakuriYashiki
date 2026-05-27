using UnityEngine;

public class ItemMenuController : MonoBehaviour
{
    [SerializeField] private Inventory inventory;

    [SerializeField] private ItemSlot[] slots;
    private int selectedIndex = -1;

    public void SelectSlot(int index)
    {
        if (!slots[index].HasItem()) return;

        if (selectedIndex >= 0)
            slots[selectedIndex].SetSelected(false);

        selectedIndex = index;
        slots[selectedIndex].SetSelected(true);
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
        }
    }
}