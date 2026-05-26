using UnityEngine;
using TMPro;

public class InteractUIManager : MonoBehaviour
{
    public static InteractUIManager Instance { get; private set; }

    [SerializeField] private GameObject interactUI;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        interactUI.SetActive(false);
    }

    public void Show() => interactUI.SetActive(true);
    public void Hide() => interactUI.SetActive(false);
}