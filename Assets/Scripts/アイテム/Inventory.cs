using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<ItemData> items = new List<ItemData>();

    public int MaxSlot => 10;

    public ItemData GetItem(int index)
    {
        if (index < 0 || index >= items.Count) return null;
        return items[index];
    }
}