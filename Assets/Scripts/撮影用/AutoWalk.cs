using System.Collections;
using UnityEngine;

public class AutoWalk : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float walkDuration = 10f;

    [Header("óßÇøé~Ç‹ÇËê›íË")]
    [SerializeField] private float stopAtTime = 5f;   // âΩïbå„Ç…é~Ç‹ÇÈÇ©
    [SerializeField] private float stopDuration = 3f; // âΩïbé~Ç‹ÇÈÇ©

    private float elapsed = 0f;
    private bool walking = true;
    private bool hasStopped = false;
    private Animator animator;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        animator?.SetBool("IsWalking", true);
    }


    void Update()
    {
        if (!walking) return;

        elapsed += Time.deltaTime;

        if (!hasStopped && elapsed >= stopAtTime)
        {
            hasStopped = true;
            StartCoroutine(StopSequence());
            return;
        }

        if (elapsed >= walkDuration)
        {
            walking = false;
            animator?.SetBool("IsWalking", false);
            return;
        }

        transform.position += Vector3.right * walkSpeed * Time.deltaTime;
    }

    private IEnumerator StopSequence()
    {
        walking = false;
        animator?.SetBool("IsWalking", false);
        yield return new WaitForSeconds(stopDuration);
        walking = true;
        animator?.SetBool("IsWalking", true);
    }
}