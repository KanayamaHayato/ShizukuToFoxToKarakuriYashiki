using UnityEngine;
using TMPro;

public class LanternInteract : MonoBehaviour
{
    public LanternManager lanternManager;

    public Renderer lanternRenderer;

    // UIテキスト
    public GameObject interactUI;

    private bool playerNear = false;
    private bool alreadyTouched = false;

    void Start()
    {
        interactUI.SetActive(false);
    }

    void Update()
    {
        if (playerNear && !alreadyTouched)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                TouchLantern();
            }
        }
    }

    void TouchLantern()
    {
        alreadyTouched = true;

        lanternManager.AddLantern();

        lanternRenderer.material.color = Color.yellow;

        interactUI.SetActive(false);

        Debug.Log("灯籠に触れた");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = true;

            if (!alreadyTouched)
            {
                interactUI.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = false;

            interactUI.SetActive(false);
        }
    }
}