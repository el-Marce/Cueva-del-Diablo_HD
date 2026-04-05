using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using System.Collections;

public class HealthSystem : MonoBehaviour
{
    static HealthSystem Instance;

    public float maxHealth = 100f;
    public float currentHealth;

    public event Action<float> OnHealthChanged;
    public event Action OnPlayerDeath;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject spawn = GameObject.Find("PlayerSpawn");
        if (spawn != null)
        {
            transform.root.position = spawn.transform.position;
            transform.root.rotation = spawn.transform.rotation;
        }
    }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(transform.root.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(transform.root.gameObject);
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (transform.position.y < -5f && currentHealth > 0)
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
        Instance = null;  // <- permite que el nuevo Player de la escena tome el control
        Destroy(transform.root.gameObject); // <- destruye el Player muerto
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public float GetHealthNormalized()
    {
        return currentHealth / maxHealth;
    }
}