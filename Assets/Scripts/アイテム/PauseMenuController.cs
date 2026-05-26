using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private GameObject itemMenuCanvas;
    [SerializeField] private MonoBehaviour[] disableWhenOpen;

    private bool isOpen = false;

    void Start()
    {
        itemMenuCanvas.SetActive(false);
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (isOpen)
                CloseMenu();
            else
                OpenMenu();
        }
    }

    void OpenMenu()
    {
        isOpen = true;
        itemMenuCanvas.SetActive(true);
        Time.timeScale = 0f;

        foreach (MonoBehaviour script in disableWhenOpen)
        {
            if (script != null) script.enabled = false;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void CloseMenu()
    {
        isOpen = false;
        itemMenuCanvas.SetActive(false);
        Time.timeScale = 1f;

        foreach (MonoBehaviour script in disableWhenOpen)
        {
            if (script != null) script.enabled = true;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}