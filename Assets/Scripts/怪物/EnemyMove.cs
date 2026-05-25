using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    public float speed = 1.5f;
    public float stopDistance = 1.5f;

    private Transform target;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void OnDetectObject(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            target = collider.transform;
        }
    }

    public void OnLoseObject(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            target = null;

            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    void FixedUpdate()
    {
        if (target == null) return;

        Vector3 targetPos = target.position;
        targetPos.y = transform.position.y;

        float distance =
            Vector3.Distance(transform.position, targetPos);

        if (distance > stopDistance)
        {
            Vector3 nextPos = Vector3.MoveTowards(
                transform.position,
                targetPos,
                speed * Time.fixedDeltaTime
            );

            rb.MovePosition(nextPos);
        }

        Vector3 direction = targetPos - transform.position;

        if (direction != Vector3.zero)
        {
            Quaternion rotation =
                Quaternion.LookRotation(direction);

            rb.MoveRotation(rotation);
        }
    }
}