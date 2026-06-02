using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TimeStopManager : MonoBehaviour
{
    private bool isStopping = false;
    public bool IsStopping => isStopping;

    [SerializeField] private GameObject timeStopUI;
    [SerializeField] private Image grayMask;

    void Start()
    {
        timeStopUI.SetActive(false);
    }

    public void StopEnemies(float seconds)
    {
        if (isStopping) return;

        StartCoroutine(StopRoutine(seconds));
    }

    private IEnumerator StopRoutine(float seconds)
    {
        isStopping = true;

        MonoBehaviour[] enemies = FindObjectsOfType<MonoBehaviour>();

        foreach (MonoBehaviour script in enemies)
        {
            if (script.CompareTag("Enemy"))
                script.enabled = false;
        }

        timeStopUI.SetActive(true);
        grayMask.fillAmount = 0f;

        float timer = 0f;

        while (timer < seconds)
        {
            timer += Time.deltaTime;

            grayMask.fillAmount = timer / seconds;

            yield return null;
        }

        foreach (MonoBehaviour script in enemies)
        {
            if (script != null && script.CompareTag("Enemy"))
                script.enabled = true;
        }

        timeStopUI.SetActive(false);
        isStopping = false;
    }
}