using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public float health = 100f;
    public float damage = 10f;
    public float moveSpeed = 3.5f;

    public void TakeDamage(float amount)
    {
        health -= amount;

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