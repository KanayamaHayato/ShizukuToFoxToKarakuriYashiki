using UnityEngine;

public class LoreSpawner : MonoBehaviour
{
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.25f);
    }
#endif
}