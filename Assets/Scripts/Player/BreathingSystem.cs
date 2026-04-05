using UnityEngine;
using System;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class BreathingSystem : MonoBehaviour
{
    [Header("Timing")]
    public float breathInterval = 10f;
    public float breathWindow = 5f;
    public float damagePerSecond = 10f;

    private float timer;

    private bool inWarningWindow = false;
    private bool inDamageState = false;

    private bool canBreathe = true;
    public float breatheCooldown = 1f;

    public event Action<float> OnBreathTimerChanged;
    public event Action OnBreathMissed;

    private HealthSystem healthSystem;
    private NoiseEmitter noiseEmitter;

    public TextMeshProUGUI breathWarningText;
    private Coroutine blinkCoroutine;

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
        StopAllCoroutines();  // cancela BlinkText que usa la referencia vieja
        blinkCoroutine = null;

        GameObject ui = GameObject.Find("UI");
        breathWarningText = ui.transform.Find("Canvas/BreathWarningText").GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
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
            if (!inWarningWindow)
            {
                inWarningWindow = true;

                if (blinkCoroutine == null)
                {
                    blinkCoroutine = StartCoroutine(BlinkText());
                }
            }
        }
        else
        {
            inWarningWindow = false;
        }

        if (timer <= -breathWindow)
        {
            if (!inDamageState)
            {
                inDamageState = true;

                if (blinkCoroutine == null)
                {
                    blinkCoroutine = StartCoroutine(BlinkText());
                }
            }

            healthSystem.TakeDamage(damagePerSecond * Time.deltaTime);
            OnBreathMissed?.Invoke();
        }
        else
        {
            inDamageState = false;
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

        if (!(inWarningWindow || inDamageState))
        {
            return;
        }

        Debug.Log("Respiración correcta");

        timer = breathInterval;
        inWarningWindow = false;
        inDamageState = false;

        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
        }

        breathWarningText.gameObject.SetActive(false);

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