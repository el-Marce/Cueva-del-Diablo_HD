using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using System.Collections;

public class HealthSystem : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    public event Action<float> OnHealthChanged;
    public event Action OnPlayerDeath;

    void Awake()
    {
        if (FindObjectsOfType<HealthSystem>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(transform.root.gameObject);
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (transform.position.y < -2f && currentHealth > 0)
            TakeDamage(currentHealth);

        if (Input.GetKeyDown(KeyCode.N))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        OnHealthChanged?.Invoke(currentHealth);

        if (currentHealth <= 0)
            Die();
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        OnHealthChanged?.Invoke(currentHealth);
    }

    public void Die()
    {
        Debug.Log("Jugador muerto");
        OnPlayerDeath?.Invoke();
        StartCoroutine(RestartLevel());
    }

    IEnumerator RestartLevel()
    {
        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public float GetHealthNormalized()
    {
        return currentHealth / maxHealth;
    }
}