using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private int maxSlots = 12;
    public List<ItemData> items = new List<ItemData>();
    public static Inventory Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // transform.root Ç≈ÇÕÇ»Ç≠é©ï™é©êgÇæÇØéùÇøâzÇ∑
        DontDestroyOnLoad(gameObject);
    }

    public ItemData GetItem(int index)
    {
        if (index < 0 || index >= items.Count)
        {
            return null;
        }

        return items[index];
    }

    public void Remove(ItemData item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
            Debug.Log($"Removed item: {item.itemName}");
        }
    }

    public void RemoveAt(int index)
    {
        if (index < 0 || index >= items.Count) return;

        items.RemoveAt(index);
    }

    public bool Add(ItemData item)
    {
        if (items.Count >= maxSlots)
        {
            Debug.Log("Inventory is full");
            return false;
        }

        items.Add(item);
        return true;
    }
    public void ForceAdd(ItemData item)
    {
        items.Add(item);
        Debug.Log($"[Inventory] ã≠êßí«â¡: {item.itemName}");
    }
}