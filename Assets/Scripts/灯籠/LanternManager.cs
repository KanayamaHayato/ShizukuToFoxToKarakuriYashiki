using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanternManager : MonoBehaviour
{
    public int requiredLanternCount = 3; // ’Eo‚É•K—v‚È“”âÄ”

    private int touchedLanternCount = 0;

    public void AddLantern()
    {
        touchedLanternCount++;

        Debug.Log("G‚Á‚½“”âÄ‚Ì”: " + touchedLanternCount + " / " + requiredLanternCount);

        if (touchedLanternCount >= requiredLanternCount)
        {
            Debug.Log("‚·‚×‚Ä‚Ì“”âÄ‚ÉG‚ê‚½I’Eo‚Å‚«‚éI");
        }
    }

    public int GetTouchedCount()
    {
        return touchedLanternCount;
    }
}