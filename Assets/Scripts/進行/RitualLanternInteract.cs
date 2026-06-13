using UnityEngine;

public class RitualLanternInteract : MonoBehaviour
{
    private bool playerNear = false;
    private bool isLit = false;

    void Update()
    {
        if (playerNear && !isLit && Input.GetKeyDown(KeyCode.E))
        {
            // ƒGƒ“ƒh2‚ªn‚Ü‚Á‚Ä‚¢‚½‚çG‚ê‚È‚¢
            if (EndingManager.Instance != null && !EndingManager.Instance.IsInRitualRoom)
                return;

            isLit = true;
            InteractUIManager.Instance.Hide();
            EndingManager.Instance.OnRitualLanternLit();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isLit && other.CompareTag("Player"))
        {
            playerNear = true;
            InteractUIManager.Instance.Show();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = false;
            InteractUIManager.Instance.Hide();
        }
    }
}