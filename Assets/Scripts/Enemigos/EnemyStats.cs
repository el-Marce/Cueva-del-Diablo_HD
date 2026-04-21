using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public float health = 100f;
    public float damage = 10f;
    public float moveSpeed;
    public float animationSpeed = 1f;      // <- velocidad de reproducción del clip
    public float patrolSpeed;              // <- velocidad específica de patrulla

    public System.Action OnHit;
    public void TakeDamage(float amount)
    {
        health -= amount;
        OnHit?.Invoke();

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
}