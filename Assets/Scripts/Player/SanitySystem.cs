using UnityEngine;
using System;

public class SanitySystem : MonoBehaviour
{
    public float maxSanity = 100f;
    public float currentSanity;

    public event Action<float> OnSanityChanged;
    public event Action OnSanityLow;
    public event Action OnSanityBreak;
    HealthSystem healthSystem;
    public float lowSanityThreshold = 30f;

    void Awake()
    {
        currentSanity = maxSanity;
    }
    private void Start()
    {
        healthSystem = GetComponent<HealthSystem>();
    }
    public void DecreaseSanity(float amount)
    {
        currentSanity -= amount;
        currentSanity = Mathf.Clamp(currentSanity, 0, maxSanity);

        OnSanityChanged?.Invoke(currentSanity);

        if (currentSanity <= lowSanityThreshold)
            OnSanityLow?.Invoke();

        if (currentSanity <= 0)
        {
            healthSystem.Die();
            OnSanityBreak?.Invoke();
        }
    }
    public void RestoreSanity(float amount)
    {
        currentSanity += amount;
        currentSanity = Mathf.Clamp(currentSanity, 0, maxSanity);

        OnSanityChanged?.Invoke(currentSanity);
    }

    public float GetSanityNormalized()
    {
        return currentSanity / maxSanity;
    }
}