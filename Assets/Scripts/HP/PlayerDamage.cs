using UnityEngine;

public class PlayerDamage : MonoBehaviour
{
    public HeartSystem heartSystem;

    public void WalkDamage()
    {
        if (heartSystem.life <= 0) return;

        heartSystem.TakeDamage(1);
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