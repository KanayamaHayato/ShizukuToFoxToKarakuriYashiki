using System.Collections;
using UnityEngine;
using Cinemachine;

public class EnemyAttack : MonoBehaviour
{
    [Header("演出")]
    [SerializeField] private string facePointName = "FacePoint";
    [SerializeField] private float closeUpDistance = 1.5f;  // 顔からカメラの距離
    [SerializeField] private float closeUpDuration = 2.0f;  // ドアップの時間
    [SerializeField] private float fadeDuration = 1.0f;     // 霧消えの時間

    [Header("霧エフェクト")]
    [SerializeField] private ParticleSystem fogEffect;      // 消滅時のパーティクル

    private bool hasAttacked = false;
    private Transform facePoint;

    void Start()
    {
        // FacePointを再帰的に探す
        facePoint = FindDeepChild(transform, facePointName);
        if (facePoint == null)
            Debug.LogWarning("[EnemyAttack] FacePointが見つかりません。");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasAttacked) return;
        if (!collision.gameObject.CompareTag("Player")) return;

        var playerDamage = collision.gameObject.GetComponent<PlayerDamage>();
        if (playerDamage == null) return;

        hasAttacked = true;
        StartCoroutine(AttackSequence(playerDamage));
    }

    private IEnumerator AttackSequence(PlayerDamage playerDamage)
    {
        var player = playerDamage.gameObject;

        // ① 操作不能にする
        var controller = player.GetComponent<StarterAssets.ThirdPersonController>();
        if (controller != null) controller.enabled = false;

        // ② カメラを怪物のドアップに切り替え
        var brain = FindObjectOfType<CinemachineBrain>();
        var virtualCam = FindObjectOfType<CinemachineVirtualCamera>();
        Transform originalFollow = null;
        Transform originalLookAt = null;

        if (virtualCam != null && facePoint != null)
        {
            originalFollow = virtualCam.Follow;
            originalLookAt = virtualCam.LookAt;

            // カメラを顔の前に移動
            virtualCam.Follow = facePoint;
            virtualCam.LookAt = facePoint;
        }

        // ③ ドアップを一定時間維持
        yield return new WaitForSeconds(closeUpDuration);

        // ④ お札を1枚減らす
        playerDamage.heartSystem.TakeDamage(1);
        if (playerDamage.heartSystem.life <= 0)
        {
            playerDamage.heartSystem.UpdateHearts();
            // ゲームオーバー処理はGameOverManagerに委譲
            FindObjectOfType<GameOverManager>()?.ShowGameOver();
            yield break;
        }

        // ⑤ 霧のように消えていく
        if (fogEffect != null) fogEffect.Play();

        // メッシュを徐々にフェードアウト
        var renderers = GetComponentsInChildren<Renderer>();
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = 1f - Mathf.Clamp01(elapsed / fadeDuration);
            foreach (var r in renderers)
            {
                foreach (var mat in r.materials)
                {
                    Color c = mat.color;
                    c.a = alpha;
                    mat.color = c;
                }
            }
            yield return null;
        }

        // ⑥ カメラを元に戻す
        if (virtualCam != null)
        {
            virtualCam.Follow = originalFollow;
            virtualCam.LookAt = originalLookAt;
        }

        // ⑦ 操作再開
        if (controller != null) controller.enabled = true;

        // ⑧ 怪物を消滅
        Destroy(gameObject);
    }

    // 再帰的に子オブジェクトを探す
    private Transform FindDeepChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name) return child;
            var result = FindDeepChild(child, name);
            if (result != null) return result;
        }
        return null;
    }
}