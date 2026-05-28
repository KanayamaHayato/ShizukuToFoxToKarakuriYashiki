using TMPro;
using UnityEngine;

public class InteractPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text actionText;

    void Awake()
    {
        Hide();
    }

    public void Show(string text)
    {
        actionText.text = text;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}