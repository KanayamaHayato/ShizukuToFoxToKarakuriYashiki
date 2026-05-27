using UnityEngine;

public class DropItemSpawner : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float forwardDistance = 1.2f;
    [SerializeField] private float heightOffset = 0.5f;

    public void Drop(ItemData item)
    {
        if (item == null || item.dropPrefab == null) return;

        Vector3 spawnPos =
            player.position +
            player.forward * forwardDistance +
            Vector3.up * heightOffset;

        GameObject obj = Instantiate(item.dropPrefab, spawnPos, Quaternion.identity);

        var dropped = obj.GetComponent<DroppedItem>();
        if (dropped != null)
        {
            dropped.Setup(item);
        }
    }
}