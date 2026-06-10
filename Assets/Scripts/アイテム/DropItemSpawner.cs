using UnityEngine;
public class DropItemSpawner : MonoBehaviour
{
    [SerializeField] private float forwardDistance = 1.2f;
    [SerializeField] private float heightOffset = 1.0f;

    private Transform GetPlayer()
    {
        var playerObj = GameObject.FindWithTag("Player");
        return playerObj != null ? playerObj.transform : null;
    }

    public void Drop(ItemData item)
    {
        if (item == null || item.dropPrefab == null) return;

        var player = GetPlayer();
        if (player == null)
        {
            Debug.LogWarning("[DropItemSpawner] PlayerÇ™å©Ç¬Ç©ÇËÇ‹ÇπÇÒÅB");
            return;
        }

        Vector3 spawnPos =
            player.position +
            player.forward * forwardDistance +
            Vector3.up * heightOffset;

        GameObject obj = Instantiate(item.dropPrefab, spawnPos, item.dropPrefab.transform.rotation);
        var dropped = obj.GetComponent<DroppedItem>();
        if (dropped != null)
            dropped.Setup(item);
    }
}