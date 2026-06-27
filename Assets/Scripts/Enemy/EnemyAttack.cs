using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public void PerformAoEAttack(Vector2 attackPoint, float radius, int damage, LayerMask playerLayer)
    {
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackPoint, radius, playerLayer);

        foreach (Collider2D player in hitPlayers)
        {
            if (player.TryGetComponent<PlayerStats>(out var health))
            {
                health.TakeDamage(damage);
                Debug.Log(health.CurrentHealth);
            }
        }
    }
}
