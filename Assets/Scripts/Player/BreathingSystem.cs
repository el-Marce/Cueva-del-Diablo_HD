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

        if (timer <= 0 && timer > -breathWindow)
        {
            // Zona crítica
            Debug.Log("Zona crítica: Respira ahora");
        }

        if (timer <= -breathWindow)
        {
            healthSystem.TakeDamage(damagePerSecond * Time.deltaTime);
            Debug.Log("Perdiendo vida por no respirar");
            OnBreathMissed?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            Breathe();
        }
    }

    public void Breathe()
    {
        Debug.Log("Respirando, temporizador reiniciado");
        timer = breathInterval;
        hasBreathed = true;
    }
}