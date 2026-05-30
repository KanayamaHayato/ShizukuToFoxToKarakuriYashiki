using UnityEngine;

public class AutoWalk : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float walkDuration = 10f; // ‰½•b•à‚­‚©

    private float elapsed = 0f;
    private bool walking = true;

    void Update()
    {
        if (!walking) return;

        elapsed += Time.deltaTime;

        if (elapsed >= walkDuration)
        {
            walking = false;
            return;
        }

        transform.position += Vector3.right * walkSpeed * Time.deltaTime;
    }
}