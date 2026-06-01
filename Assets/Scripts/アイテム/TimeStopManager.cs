using UnityEngine;
using System.Collections;

public class TimeStopManager : MonoBehaviour
{
    private bool isStopping = false;
    public bool IsStopping => isStopping;

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

        yield return new WaitForSecondsRealtime(seconds);

        foreach (MonoBehaviour script in enemies)
        {
            if (script != null && script.CompareTag("Enemy"))
                script.enabled = true;
        }

        isStopping = false;
    }
}