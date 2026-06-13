using System;
using UnityEngine;

public class LanternManager : MonoBehaviour
{
    public event Action OnAllLit;
    public event Action<int, int> OnLitCountChanged;

    public int TotalCount { get; private set; }
    public int LitCount { get; private set; }

    public void Reset()
    {
        TotalCount = 0;
        LitCount = 0;
    }

    public void RegisterLanternRoom(GameObject room)
    {
        var lanterns = room.GetComponentsInChildren<LanternInteract>(true);
        Debug.Log($"[LanternManager] {room.name} LanternInteract”: {lanterns.Length}");

        TotalCount += lanterns.Length;

        foreach (var l in lanterns)
        {
            l.SetLanternManager(this); // š ’¼Ú“n‚·
            l.OnLit += HandleLit;
            Debug.Log($"[LanternManager] “o˜^Š®—¹: {l.gameObject.name}");
        }
    }

    private void HandleLit()
    {
        LitCount++;
        OnLitCountChanged?.Invoke(LitCount, TotalCount);
        Debug.Log($"“”âÄ {LitCount} / {TotalCount}");

        if (LitCount >= TotalCount)
        {
            Debug.Log("‚·‚×‚Ä‚Ì“”âÄ‚ÉG‚ê‚½IOnAllLit”­‰ÎI");
            OnAllLit?.Invoke();
        }
    }
    public void DebugLightAll()
    {
        int remaining = TotalCount - LitCount;
        for (int i = 0; i < remaining; i++)
            HandleLit();
    }
    public void DebugLightOne()
    {
        if (LitCount < TotalCount)
            HandleLit();
    }
}