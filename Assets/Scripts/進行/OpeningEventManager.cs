using System.Collections;
using UnityEngine;

public class OpeningEventManager : MonoBehaviour
{
    [Header("QÆ")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private CanvasGroup fadeCanvasGroup; // •‚¢CanvasGroup

    [Header("İ’è")]
    [SerializeField] private float waitBeforeEnemy = 1f;
    [SerializeField] private float waitBeforeFade = 2f;
    [SerializeField] private float fadeDuration = 1f;

    private bool hasPlayed = false;

    public void PlayOpeningEvent(GameObject startRoom, GameObject[] neighborRooms)
    {
        if (hasPlayed) return;
        hasPlayed = true;
        StartCoroutine(OpeningSequence(startRoom, neighborRooms));
    }

    private IEnumerator OpeningSequence(GameObject startRoom, GameObject[] neighborRooms)
    {
        var player = GameObject.FindWithTag("Player");
        if (player == null) yield break;

        // ‡@ ‘€ì•s”\‚É‚·‚é
        var controller = player.GetComponent<StarterAssets.ThirdPersonController>();
        if (controller != null) controller.enabled = false;

        // ‡A ´‚ğ—×‚Ì•”‰®‚Éƒ[ƒv
        if (neighborRooms != null && neighborRooms.Length > 0)
        {
            var neighborRoom = neighborRooms[Random.Range(0, neighborRooms.Length)];
            var spawnPoint = neighborRoom.transform.Find("SpawnPoint");
            Vector3 warpPos = spawnPoint != null
                ? spawnPoint.position
                : neighborRoom.transform.position + Vector3.up;

            var cc = player.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;
            player.transform.position = warpPos;
            if (cc != null) cc.enabled = true;
        }

        yield return new WaitForSeconds(waitBeforeEnemy);

        // ‡B ‰ö•¨‚ğƒXƒ|[ƒ“
        Vector3 enemyPos = player.transform.position + player.transform.forward * 3f;
        GameObject enemy = Instantiate(enemyPrefab, enemyPos, Quaternion.identity);

        // ‡C EnemyAttack‚ÌƒI[ƒvƒjƒ“ƒO‰‰o
        var enemyAttack = enemy.GetComponent<EnemyAttack>();
        if (enemyAttack != null)
            enemyAttack.TriggerOpening(player);

        yield return new WaitForSeconds(waitBeforeFade);

        // ‡D ˆÃ“]
        yield return StartCoroutine(FadeOut());

        // ‡E ‰ö•¨‚ğÁ‚·
        if (enemy != null) Destroy(enemy);

        // ‡F ´‚ğ‰Šú•”‰®‚Éƒ[ƒv
        var startSpawnPoint = startRoom.transform.Find("SpawnPoint");
        Vector3 startPos = startSpawnPoint != null
            ? startSpawnPoint.position
            : startRoom.transform.position + Vector3.up;

        var cc2 = player.GetComponent<CharacterController>();
        if (cc2 != null) cc2.enabled = false;
        player.transform.position = startPos;
        if (cc2 != null) cc2.enabled = true;

        // ‡G ˆÃ“]‰ğœ
        yield return StartCoroutine(FadeIn());

        // ‡H ‘€ìÄŠJ
        if (controller != null) controller.enabled = true;
    }

    private IEnumerator FadeOut()
    {
        fadeCanvasGroup.alpha = 0f;
        while (fadeCanvasGroup.alpha < 1f)
        {
            fadeCanvasGroup.alpha += Time.deltaTime / fadeDuration;
            yield return null;
        }
        fadeCanvasGroup.alpha = 1f;
    }

    private IEnumerator FadeIn()
    {
        fadeCanvasGroup.alpha = 1f;
        while (fadeCanvasGroup.alpha > 0f)
        {
            fadeCanvasGroup.alpha -= Time.deltaTime / fadeDuration;
            yield return null;
        }
        fadeCanvasGroup.alpha = 0f;
    }
}