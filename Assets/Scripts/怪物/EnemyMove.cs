using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    public float speed = 1.5f;
    public float stopDistance = 1.5f;

    [Header("視線チェック")]
    [SerializeField] private float eyeHeight = 1.0f;        // 目の高さ
    [SerializeField] private LayerMask obstacleLayer;       // 壁レイヤー
    public bool CanSeePlayer { get; private set; } = false;

    private Transform target;
    private Rigidbody rb;
    private bool canSeePlayer = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Debug.Log($"[EnemyMove] rb={rb}");
    }

    public void OnDetectObject(Collider collider)
    {
        if (collider.CompareTag("Player"))
            target = collider.transform;
    }

    public void OnLoseObject(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            target = null;
            canSeePlayer = false;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    void FixedUpdate()
    {
        if (target == null) return;

        // 視線チェック
        Vector3 eyePos = transform.position + Vector3.up * eyeHeight;
        Vector3 targetPos = target.position + Vector3.up * eyeHeight;
        Vector3 dir = targetPos - eyePos;
        float dist = dir.magnitude;

        if (Physics.Raycast(eyePos, dir.normalized, dist, obstacleLayer))
        {
            // 壁に遮られている → 止まる
            CanSeePlayer = false;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            return;
        }

        CanSeePlayer = true;

        // 追いかける
        Vector3 flatTargetPos = target.position;
        flatTargetPos.y = transform.position.y;
        float flatDist = Vector3.Distance(transform.position, flatTargetPos);

        if (flatDist > stopDistance)
        {
            Vector3 nextPos = Vector3.MoveTowards(
                transform.position,
                flatTargetPos,
                speed * Time.fixedDeltaTime
            );
            rb.MovePosition(nextPos);
        }

        Vector3 direction = flatTargetPos - transform.position;
        if (direction != Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(direction);
            rb.MoveRotation(rotation);
        }
    }
}