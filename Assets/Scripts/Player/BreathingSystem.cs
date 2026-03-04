using UnityEngine;
using System;

public class BreathingSystem : MonoBehaviour
{
    public float breathInterval = 50f;
    public float breathWindow = 5f;
    public float damagePerSecond = 10f;

    private float timer;
    private bool hasBreathed;

    public event Action<float> OnBreathTimerChanged;
    public event Action OnBreathMissed;

    private HealthSystem healthSystem;

    void Start()
    {
        healthSystem = GetComponent<HealthSystem>();
        timer = breathInterval;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        OnBreathTimerChanged?.Invoke(timer);

        if (timer <= -breathWindow)
        {
            healthSystem.TakeDamage(damagePerSecond * Time.deltaTime);
            OnBreathMissed?.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            Breathe();
        }
    }

    public void Breathe()
    {
        timer = breathInterval;
        hasBreathed = true;
    }
}