using UnityEngine;
using Cinemachine;
using System.Collections;

public class ShrineFindTrigger : MonoBehaviour
{
    [SerializeField] private ShrineDialogueTrigger dialogueTrigger;
    [SerializeField] private CinemachineVirtualCamera findShrineCamera;
    [SerializeField] private float lookDuration = 3f;
    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !triggered)
        {
            triggered = true;
            dialogueTrigger.OnFindShrine();
            StartCoroutine(ShowShrineCamera());
        }
    }

    private IEnumerator ShowShrineCamera()
    {
        findShrineCamera.Priority = 20;
        yield return new WaitForSeconds(lookDuration);
        findShrineCamera.Priority = 0;
    }
}