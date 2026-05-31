using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShrineInteract : MonoBehaviour
{
    [Header("âKƒIƒuƒWƒFƒNƒg")]
    [SerializeField] private GameObject brokenShrine;   // ”pšĞ‚ÌâK
    [SerializeField] private GameObject fixedShrine;    // ³í‚ÈâK

    [Header("ˆÃ“]")]
    [SerializeField] private float fadeDuration = 1.0f;

    private bool playerNear = false;
    private bool alreadyTouched = false;

    void Update()
    {
        if (playerNear && !alreadyTouched && Input.GetKeyDown(KeyCode.E))
            StartCoroutine(ShrineSequence());
    }

    private IEnumerator ShrineSequence()
    {
        alreadyTouched = true;
        InteractUIManager.Instance.Hide();

        // TODO: è‚ğ‚©‚¯‚éƒ‚[ƒVƒ‡ƒ“Ä¶

        // ˆÃ“]
        // TODO: FadeManager‚ğŒã‚Åì‚é
        yield return new WaitForSeconds(fadeDuration);

        // ”pšĞ¨³í‚É·‚µ‘Ö‚¦
        brokenShrine.SetActive(false);
        fixedShrine.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        // ƒ[ƒv
        SceneManager.LoadScene("Maze");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = true;
            if (!alreadyTouched)
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