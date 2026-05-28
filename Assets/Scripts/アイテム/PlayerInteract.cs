using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera playerCamera;

    [Header("Interact")]
    [SerializeField] private float interactDistance = 10;
    [SerializeField] private InteractPanel panel;

    private IInteractable currentInteractable;

    void Update()
    {
        CheckInteractable();

        if (currentInteractable != null && Input.GetKeyDown(KeyCode.E))
        {
            currentInteractable.Interact();
        }
    }

    void CheckInteractable()
    {
        currentInteractable = null;

        // TPSå¸ÇØÇ…è≠Çµâ∫å¸Ç´
        Vector3 dir = playerCamera.transform.forward;
        dir.y -= 0.1f;
        dir.Normalize();

        Ray ray = new Ray(playerCamera.transform.position, dir);
        Debug.DrawRay(ray.origin, ray.direction * interactDistance, Color.red);

        if (currentInteractable != null)
        {
            Debug.Log("Interact Found");

            panel.Show(currentInteractable.GetActionText());
            return;
        }

        panel.Hide();
    }
}