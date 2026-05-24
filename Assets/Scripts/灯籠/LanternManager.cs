// LanternManager.cs
using System;
using System.Collections.Generic;
using UnityEngine;

public class LanternManager : MonoBehaviour
{
    // 全灯籠が灯ったときのイベント（ゲームクリア処理などを外から購読）
    public event Action OnAllLit;
    // 1個灯るたびのイベント（UI更新に使う）
    public event Action<int, int> OnLitCountChanged; // (現在数, 総数)

    public int TotalCount { get; private set; }
    public int LitCount { get; private set; }

    // 迷路再生成時にリセット
    public void Reset()
    {
        TotalCount = 0;
        LitCount = 0;
    }

    // MazeGenerator から部屋生成時に呼ぶ
    public void RegisterLanternRoom(GameObject room)
    {
        // 部屋の中にある LanternInteract を全部登録
        var lanterns = room.GetComponentsInChildren<LanternInteract>();
        TotalCount += lanterns.Length;

        foreach (var l in lanterns)
            l.OnLit += HandleLit;
    }

    private void HandleLit()
    {
        LitCount++;
        OnLitCountChanged?.Invoke(LitCount, TotalCount);
        Debug.Log($"灯籠 {LitCount} / {TotalCount}");

        if (LitCount >= TotalCount)
        {
            Debug.Log("すべての灯籠に触れた！");
            OnAllLit?.Invoke();
        }
    }
}