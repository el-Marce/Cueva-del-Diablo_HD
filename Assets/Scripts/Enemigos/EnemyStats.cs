using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public float health = 100f;
    public float damage = 10f;
    public float moveSpeed;
    public float animationSpeed = 1f;      // <- velocidad de reproducción del clip
    public float patrolSpeed;              // <- velocidad específica de patrulla

    public System.Action OnHit;
    public bool IsDead { get; private set; }
    public void TakeDamage(float amount)
    {
        if (IsDead) return;

        health -= amount;
        OnHit?.Invoke();

        if (health <= 0)
        {
            IsDead = true;
            Die();
        }
    }

    void Die()
    {
        //Destroy(gameObject);
    }
}