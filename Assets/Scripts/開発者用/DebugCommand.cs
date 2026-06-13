using UnityEngine;

public class DebugCommand : MonoBehaviour
{
    [SerializeField] private LanternManager lanternManager;

    void Update()
    {
        // F1で全灯籠点灯
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Debug.Log("[Debug] 全灯籠点灯！");
            lanternManager.DebugLightAll();
        }
        // F2で一個点灯
        if (Input.GetKeyDown(KeyCode.F2))
        {
            Debug.Log("[Debug] 灯籠1個点灯！");
            lanternManager.DebugLightOne();
        }
        // F3キーで一番近い伝記を入手
        if (Input.GetKeyDown(KeyCode.F3))
        {
            var player = GameObject.FindWithTag("Player");
            if (player == null) return;

            var lores = FindObjectsOfType<LoreItem>();
            LoreItem nearest = null;
            float minDist = float.MaxValue;

            foreach (var lore in lores)
            {
                float dist = Vector3.Distance(player.transform.position, lore.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = lore;
                }
            }

            if (nearest != null)
            {
                LoreManager.Instance.CollectLore(nearest);
                nearest.gameObject.SetActive(false);
                Debug.Log($"[Debug] 最近傍の伝記を入手: {nearest.LoreName}");
            }
            else
            {
                Debug.Log("[Debug] 伝記が見つかりません");
            }
        }
    }
}