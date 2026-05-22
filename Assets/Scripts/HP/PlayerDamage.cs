using UnityEngine;

public class PlayerDamage : MonoBehaviour
{
    public HeartSystem heartSystem;
    public GameOverManager gameOverManager;

    public void WalkDamage()
    {
        if (heartSystem.life <= 0) return;

        heartSystem.TakeDamage(1);

        if (heartSystem.life <= 0)
        {
            gameOverManager.ShowGameOver();
        }
    }

    public void JumpHeal()
    {
        if (heartSystem.life <= 0) return;

        heartSystem.Heal(1);
    }

    public bool IsDead()
    {
        return heartSystem.life <= 0;
    }
}