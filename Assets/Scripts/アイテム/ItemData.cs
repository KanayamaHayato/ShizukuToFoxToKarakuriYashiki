using UnityEngine;

[CreateAssetMenu(menuName = "Game/Item")]
public class ItemData : ScriptableObject
{
    [Header("Basic")]
    public string itemName;
    [TextArea]
    public string description;
    public Sprite icon;
}