using UnityEngine;
public class End2SceneInit : MonoBehaviour
{
    void Awake()
    {
        if (EndingManager.Instance != null)
            EndingManager.Instance.IsInRitualRoom = true;
    }
}