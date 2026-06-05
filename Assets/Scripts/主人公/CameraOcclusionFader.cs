using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// カメラが壁の外に出たとき画面を暗転させる
/// </summary>
public class CameraOcclusionFader : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private Transform player;
    [SerializeField] private Image fadeImage; // 黒いUIImage

    [Header("設定")]
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float fadeSpeed = 10f;

    private float targetAlpha = 0f;

    void Update()
    {
        // カメラ→雫の間に壁があるか判定
        Vector3 camPos = transform.position;
        Vector3 playerPos = player.position + Vector3.up;
        Vector3 dir = playerPos - camPos;
        float dist = dir.magnitude;

        if (Physics.Raycast(camPos, dir.normalized, dist, wallLayer))
        {
            targetAlpha = 1f;
            Debug.Log("壁に当たった"); // ★追加
        }
        else
        {
            targetAlpha = 0f;
            Debug.Log("当たってない"); // ★追加
        }

        // なめらかにフェード
        Color c = fadeImage.color;
        c.a = Mathf.Lerp(c.a, targetAlpha, Time.deltaTime * fadeSpeed);
        fadeImage.color = c;
    }
}