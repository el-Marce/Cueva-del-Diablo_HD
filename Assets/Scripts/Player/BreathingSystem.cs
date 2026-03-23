using UnityEngine;
using System;
using System.Collections;
using TMPro;
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
    NoiseEmitter noiseEmitter;

    public TextMeshProUGUI breathWarningText;
    private Coroutine blinkCoroutine;

    void Start()
    {
        microphoneInput = GetComponent<MicrophoneInput>();
        healthSystem = GetComponent<HealthSystem>();
        noiseEmitter = GetComponent<NoiseEmitter>();

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
                //Debug.Log("Zona crítica: Respira ahora");

                if (blinkCoroutine == null)
                {
                    blinkCoroutine = StartCoroutine(BlinkText());
                }
            }
        }
        else
        {
            inCriticalWindow = false;

            if (blinkCoroutine != null)
            {
                StopCoroutine(blinkCoroutine);
                blinkCoroutine = null;
                breathWarningText.gameObject.SetActive(false);
            }
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
        noiseEmitter.EmitNoise(0.1f);

        if (!canBreathe) return;

        if (!inCriticalWindow)
        {
            //Debug.Log("Respiración fuera de tiempo");
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

    IEnumerator BlinkText()
    {
        while (true)
        {
            breathWarningText.gameObject.SetActive(!breathWarningText.gameObject.activeSelf);
            yield return new WaitForSeconds(0.5f);
        }
    }
}