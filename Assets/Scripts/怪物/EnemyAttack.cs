using System.Collections;
using UnityEngine;
using Cinemachine;

public class EnemyAttack : MonoBehaviour
{
    [Header("演出")]
    [SerializeField] private string facePointName = "FacePoint";
    [SerializeField] private float closeUpDuration = 2.0f;
    [SerializeField] private float fadeDuration = 1.0f;

    [Header("霧エフェクト")]
    [SerializeField] private ParticleSystem fogEffect;

    [Header("ワープ設定")]
    [SerializeField] private float warpDistance = 2f;   // 怪物との距離
    [SerializeField] private float warpHeightOffset = 0f; // 雫の高さ調整

    private bool hasAttacked = false;
    private Transform facePoint;

    void Start()
    {
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
        // ★ ワープ前に怪物のColliderを無効化
        // ★ 怪物のRigidbodyをKinematicにして物理演算を止める
        var rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        // ★ 雫を怪物の正面にワープ
        if (facePoint != null)
        {
            Vector3 warpPos = facePoint.position + transform.forward * warpDistance;
            warpPos.y += warpHeightOffset;
            player.transform.position = warpPos;
            player.transform.rotation = Quaternion.LookRotation(-transform.forward);
        }
        // ★ 雫のアニメーションを止める
        var playerAnimator = player.GetComponentInChildren<Animator>();
        if (playerAnimator != null)
            playerAnimator.SetFloat("Speed", 0f);

        // 怪物の動きを止める
        var enemyMove = GetComponent<EnemyMove>();
        var animator = GetComponentInChildren<Animator>();
        if (animator != null)
            animator.SetTrigger("Attack");
        if (enemyMove != null) enemyMove.enabled = false;

        // ズーム無効化
        var cameraZoom = FindObjectOfType<CameraZoom>();
        if (cameraZoom != null) cameraZoom.isEnabled = false;

        // ② CloseUpCameraをPriorityで切り替え
        var closeUpCamObj = GameObject.Find("CloseUpCamera");
        CinemachineVirtualCamera closeUpCam = closeUpCamObj
            ?.GetComponent<CinemachineVirtualCamera>();

        if (closeUpCam != null && facePoint != null)
        {
            // ★ 怪物の正面にカメラターゲットを配置（雫と同じ方式）
            GameObject camTarget = new GameObject("CamTarget");
            camTarget.transform.position = facePoint.position + transform.forward * 3f;
            camTarget.transform.rotation = Quaternion.LookRotation(-transform.forward);

            closeUpCam.Follow = camTarget.transform;
            closeUpCam.LookAt = facePoint;
            closeUpCam.Priority = 20;

            Destroy(camTarget, closeUpDuration + fadeDuration + 1f);
        }
        else
        {
            Debug.LogWarning("[EnemyAttack] CloseUpCameraが見つかりません。");
        }

        // ③ 赤いライトを生成
        GameObject lightObj = new GameObject("AttackLight");
        Light attackLight = lightObj.AddComponent<Light>();
        attackLight.color = Color.red;
        attackLight.intensity = 3f;
        attackLight.range = 10f;
        lightObj.transform.position = facePoint.position;
        lightObj.transform.SetParent(facePoint);

        // ④ ドアップを一定時間維持
        yield return new WaitForSeconds(closeUpDuration);
        Destroy(lightObj);

        // ⑤ お札を1枚減らす
        playerDamage.heartSystem.TakeDamage(1);
        if (playerDamage.heartSystem.life <= 0)
        {
            playerDamage.heartSystem.UpdateHearts();
            if (closeUpCam != null) closeUpCam.Priority = -1;
            if (cameraZoom != null) cameraZoom.isEnabled = true;
            FindObjectOfType<GameOverManager>()?.ShowGameOver();
            yield break;
        }

        // ⑥ CloseUpCameraを戻す・操作再開
        if (closeUpCam != null) closeUpCam.Priority = -1;
        if (cameraZoom != null) cameraZoom.isEnabled = true;
        if (controller != null) controller.enabled = true;

        // ⑦ 霧パーティクルを怪物から切り離して再生
        if (fogEffect != null)
        {
            Debug.Log("[EnemyAttack] 霧再生");
            fogEffect.transform.SetParent(null);
            fogEffect.Play();
            Destroy(fogEffect.gameObject, fogEffect.main.duration);
        }
        else
        {
            Debug.Log("[EnemyAttack] fogEffectがnull");
        }

        // ⑧ 怪物本体のRendererとColliderを無効化（霧は切り離し済みなので影響なし）
        foreach (var r in GetComponentsInChildren<Renderer>())
            r.enabled = false;
        foreach (var col in GetComponentsInChildren<Collider>())
            col.enabled = false;

        // ⑨ フェードアウト待ち後に怪物消滅
        yield return new WaitForSeconds(fadeDuration);
        Destroy(gameObject);
    }

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