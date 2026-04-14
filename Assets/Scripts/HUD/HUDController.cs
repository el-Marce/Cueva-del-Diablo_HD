using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HUDController : MonoBehaviour
{
    [Header("Health")]
    public Image healthBar;
    public Image healthBarDelayed;

    [Header("Sanity")]
    public Image sanityBar;
    public Image sanityBarDelayed;

    [Header("Configuración")]
    public float shadowDelay = 0.5f;
    public float shadowSpeed = 3f;

    HealthSystem healthSystem;
    SanitySystem sanitySystem;

    Queue<(float value, float time)> healthHistory = new Queue<(float, float)>();
    Queue<(float value, float time)> sanityHistory = new Queue<(float, float)>();

    void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        healthSystem = player.GetComponentInChildren<HealthSystem>();
        sanitySystem = player.GetComponentInChildren<SanitySystem>();

        float h = healthSystem.GetHealthNormalized();
        float s = sanitySystem.GetSanityNormalized();

        healthBar.fillAmount = h;
        healthBarDelayed.fillAmount = h;
        sanityBar.fillAmount = s;
        sanityBarDelayed.fillAmount = s;

        healthSystem.OnHealthChanged += OnHealthChanged;
        sanitySystem.OnSanityChanged += OnSanityChanged;
    }

    void OnDestroy()
    {
        if (healthSystem != null) healthSystem.OnHealthChanged -= OnHealthChanged;
        if (sanitySystem != null) sanitySystem.OnSanityChanged -= OnSanityChanged;
    }

    void OnHealthChanged(float current)
    {
        float normalized = current / healthSystem.maxHealth;
        healthBar.fillAmount = normalized;
        healthHistory.Enqueue((normalized, Time.time));
    }

    void OnSanityChanged(float current)
    {
        float normalized = current / sanitySystem.maxSanity;
        sanityBar.fillAmount = normalized;
        sanityHistory.Enqueue((normalized, Time.time));
    }

    void Update()
    {
        ApplyDelay(healthHistory, healthBarDelayed);
        ApplyDelay(sanityHistory, sanityBarDelayed);
    }

    void ApplyDelay(Queue<(float value, float time)> history, Image bar)
    {
        float target = bar.fillAmount;

        while (history.Count > 0 && Time.time - history.Peek().time >= shadowDelay)
            target = history.Dequeue().value;

        bar.fillAmount = Mathf.MoveTowards(bar.fillAmount, target, shadowSpeed * Time.deltaTime);
    }
}