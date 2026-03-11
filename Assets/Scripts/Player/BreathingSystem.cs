using UnityEngine;
using System;
using System.Collections;

public class BreathingSystem : MonoBehaviour
{
    public float breathInterval = 10f;
    public float breathWindow = 5f;
    public float damagePerSecond = 10f;

    private float timer;
    private bool hasBreathed;

    private bool canBreathe = true;
    public float breatheCooldown = 1f;

    public event Action<float> OnBreathTimerChanged;
    public event Action OnBreathMissed;

    private HealthSystem healthSystem;
    private MicrophoneInput microphoneInput;

    private bool inCriticalWindow = false;

    void Start()
    {
        microphoneInput = GetComponent<MicrophoneInput>();
        healthSystem = GetComponent<HealthSystem>();
        timer = breathInterval;
    }

    void Update()
    {
        if (GameState.InMenu) return;

        timer -= Time.deltaTime;
        OnBreathTimerChanged?.Invoke(timer);

        if (timer <= 0 && timer > -breathWindow)
        {
            if (!inCriticalWindow)
            {
                inCriticalWindow = true;
                Debug.Log("Zona crítica: Respira ahora");
            }
        }
        else
        {
            inCriticalWindow = false;
        }

        if (timer <= -breathWindow)
        {
            healthSystem.TakeDamage(damagePerSecond * Time.deltaTime);
            //Debug.Log("Perdiendo vida por no respirar");
            OnBreathMissed?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            Breathe();
        }
    }

    public void Breathe()
    {
        if (!canBreathe) return;

        if (!inCriticalWindow)
        {
            Debug.Log("Respiración fuera de tiempo");
            return;
        }

        Debug.Log("Respiración correcta");

        timer = breathInterval;
        hasBreathed = true;

        microphoneInput.PrintDebugs();

        StartCoroutine(BreatheCooldown());
    }

    IEnumerator BreatheCooldown()
    {
        canBreathe = false;
        yield return new WaitForSeconds(breatheCooldown);
        canBreathe = true;
    }
}