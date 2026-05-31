using UnityEngine;

public class FaceController : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (animator != null)
            animator.SetFloat("FaceTime", animator.GetFloat("FaceTime") + Time.deltaTime);
    }
}