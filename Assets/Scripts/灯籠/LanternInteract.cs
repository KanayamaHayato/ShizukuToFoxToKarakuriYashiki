// LanternInteract.cs
using System;
using UnityEngine;

public class LanternInteract : MonoBehaviour
{
    [SerializeField] private Renderer lanternRenderer;
    [SerializeField] private GameObject interactUI;

    // LanternManager が購読するイベント（直接参照をなくす）
    public event Action OnLit;

    private bool playerNear = false;
    private bool alreadyTouched = false;

    void Start()
    {
        if (interactUI != null)
            interactUI.SetActive(false);
    }

    void Update()
    {
        if (playerNear && !alreadyTouched && Input.GetKeyDown(KeyCode.E))
            TouchLantern();
    }

    private void TouchLantern()
    {
        alreadyTouched = true;

        if (lanternRenderer != null)
            lanternRenderer.material.color = Color.yellow;

        if (interactUI != null)
            interactUI.SetActive(false);

        OnLit?.Invoke();
        Debug.Log("灯籠に触れた");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = true;
            if (!alreadyTouched && interactUI != null)
                interactUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = false;
            if (interactUI != null)
                interactUI.SetActive(false);
        }
    }
}