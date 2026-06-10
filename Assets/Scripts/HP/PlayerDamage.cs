using UnityEngine;

public class PlayerDamage : MonoBehaviour
{
    public HeartSystem heartSystem;
    public GameOverManager gameOverManager;

    private void Damage()
    {
        if (heartSystem.life <= 0) return;

        heartSystem.TakeDamage(1);

        if (heartSystem.life <= 0)
        {
            gameOverManager.ShowGameOver();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyAttack"))
        {
            Damage();
        }
    }

    public bool IsDead()
    {
        return heartSystem.life <= 0;
    }
}