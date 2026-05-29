using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private GameObject itemMenuCanvas;
    [SerializeField] private MonoBehaviour[] disableWhenOpen;
    [SerializeField] private ItemMenuController itemMenu;
    [SerializeField] private GameObject interactCanvas;

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

        if (itemMenu != null)
            itemMenu.Refresh();

        itemMenuCanvas.SetActive(true);
        Time.timeScale = 0f;

        foreach (MonoBehaviour script in disableWhenOpen)
        {
            if (script != null)
                script.enabled = false;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (interactCanvas != null)
            interactCanvas.SetActive(false);
    }

    void CloseMenu()
    {
        isOpen = false;

        itemMenuCanvas.SetActive(false);
        Time.timeScale = 1f;

        foreach (MonoBehaviour script in disableWhenOpen)
        {
            if (script != null)
                script.enabled = true;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (interactCanvas != null)
            interactCanvas.SetActive(true);
    }
}