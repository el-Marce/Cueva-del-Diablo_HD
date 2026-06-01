using UnityEngine;
using UnityEngine.UI;
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
    public float breatheCooldown = 1f;

    [Header("UI - Referencias")]
    public CanvasGroup breathRoot;
    public TextMeshProUGUI breathText;
    public Image breathImage;

    [Header("UI - Fade")]
    public float fadeInDuration = 0.5f;
    public float delayFadeOut = 1.5f;
    public float fadeOutDuration = 0.8f;

    [Header("UI - Parpadeo rojo")]
    public float blinkSpeed = 2f;
    public float blinkMinAlpha = 0.2f;

    [Header("UI - Pulso verde")]
    public float pulseSpeed = 1.5f;
    public float pulseMinScale = 0.9f;
    public float pulseMaxScale = 1.1f;

    [Header("Contenido")]
    public string mensajeAdvertencia = "NIVEL DE OXÍGENO BAJO ¡¡RESPIRA AHORA!!";
    public string mensajeCorrecto = "Respiración correcta";
    public Color colorAdvertencia = Color.red;
    public Color colorCorrecto = Color.green;

    public event Action<float> OnBreathTimerChanged;
    public event Action OnBreathMissed;

    // Estado de la UI
    enum UIState { Oculto, FadeIn, Rojo, Confirmacion, FadeOut }
    UIState uiState = UIState.Oculto;
    float uiTimer = 0f; // usado para fade in/out y delay verde

    float timer;
    bool inWarningWindow = false;
    bool inDamageState = false;
    bool canBreathe = true;
    bool primeraVez = true;

    HealthSystem healthSystem;
    NoiseEmitter noiseEmitter;

    void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StopAllCoroutines();
        Transform canvas = GameObject.Find("UI").transform.Find("Canvas");
        breathRoot = canvas.Find("BreathWarningRoot").GetComponent<CanvasGroup>();
        breathText = canvas.Find("BreathWarningRoot/BreathWarningText").GetComponent<TextMeshProUGUI>();
        breathImage = canvas.Find("BreathWarningRoot/BreathWarningImage").GetComponent<Image>();
        OcultarInmediato();
    }

    void Start()
    {
        healthSystem = GetComponent<HealthSystem>();
        noiseEmitter = GetComponent<NoiseEmitter>();
        timer = breathInterval;
        OcultarInmediato();
    }

    void Update()
    {
        if (GameState.InMenu) return;

        // ── Timer de respiración ──────────────────────────────────────────
        timer -= Time.deltaTime;
        OnBreathTimerChanged?.Invoke(timer);

        if (timer <= 0 && timer > -breathWindow)
        {
            if (!inWarningWindow)
            {
                inWarningWindow = true;
                IniciarAdvertencia();
            }
        }
        else inWarningWindow = false;

        if (timer <= -breathWindow)
        {
            if (!inDamageState)
            {
                inDamageState = true;
                // No reinicia la UI, ya está en rojo — solo activa daño
            }
            healthSystem.TakeDamage(damagePerSecond * Time.deltaTime);
            OnBreathMissed?.Invoke();
        }
        else inDamageState = false;

        if (Input.GetKeyDown(KeyCode.B))
            Breathe();

        // ── Máquina de estados de UI ──────────────────────────────────────
        ActualizarUI();
    }

    void ActualizarUI()
    {
        switch (uiState)
        {
            case UIState.Oculto:
                break;

            case UIState.FadeIn:
                uiTimer += Time.deltaTime;
                breathRoot.alpha = Mathf.Lerp(0f, 1f, uiTimer / fadeInDuration);
                if (uiTimer >= fadeInDuration)
                {
                    breathRoot.alpha = 1f;
                    uiState = UIState.Rojo;
                }
                break;

            case UIState.Rojo:
                // Parpadeo continuo — usa Time.time directamente, nunca se resetea
                breathRoot.alpha = Mathf.Lerp(blinkMinAlpha, 1f,
                    (Mathf.Sin(Time.time * blinkSpeed * Mathf.PI) + 1f) / 2f);
                break;

            case UIState.Confirmacion:
                // Pulso de escala continuo
                float t = (Mathf.Sin(Time.time * pulseSpeed * Mathf.PI) + 1f) / 2f;
                breathImage.transform.localScale = Vector3.one * Mathf.Lerp(pulseMinScale, pulseMaxScale, t);
                breathRoot.alpha = 1f;

                uiTimer -= Time.deltaTime;
                if (uiTimer <= 0f)
                {
                    breathImage.transform.localScale = Vector3.one;
                    uiTimer = fadeOutDuration;
                    uiState = UIState.FadeOut;
                }
                break;

            case UIState.FadeOut:
                uiTimer -= Time.deltaTime;
                breathRoot.alpha = Mathf.Lerp(0f, 1f, uiTimer / fadeOutDuration);
                if (uiTimer <= 0f)
                    OcultarInmediato();
                break;
        }
    }

    void IniciarAdvertencia()
    {
        // Si ya está en rojo o haciendo fade in, no reiniciar
        if (uiState == UIState.Rojo || uiState == UIState.FadeIn) return;

        breathImage.color = colorAdvertencia;
        breathImage.gameObject.SetActive(true);
        breathImage.transform.localScale = Vector3.one;

        breathText.color = colorAdvertencia;
        breathText.text = mensajeAdvertencia;
        breathText.gameObject.SetActive(primeraVez);

        breathRoot.gameObject.SetActive(true);
        breathRoot.alpha = 0f;

        uiTimer = 0f;
        uiState = UIState.FadeIn;
    }

    public void Breathe()
    {
        noiseEmitter.EmitNoise(0.1f);
        if (!canBreathe) return;
        if (!(inWarningWindow || inDamageState)) return;

        timer = breathInterval;
        inWarningWindow = false;
        inDamageState = false;

        // Cambiar a verde sin cortar el seno — solo cambia color y estado
        breathImage.color = colorCorrecto;
        breathImage.gameObject.SetActive(true);
        breathImage.transform.localScale = Vector3.one;

        breathText.color = colorCorrecto;
        breathText.text = mensajeCorrecto;
        breathText.gameObject.SetActive(primeraVez);

        breathRoot.alpha = 1f;
        uiTimer = delayFadeOut;
        uiState = UIState.Confirmacion;

        StartCoroutine(BreatheCooldown());
        primeraVez = false;
    }

    void OcultarInmediato()
    {
        uiState = UIState.Oculto;
        if (breathRoot != null) { breathRoot.alpha = 0f; breathRoot.gameObject.SetActive(false); }
        if (breathImage != null) breathImage.transform.localScale = Vector3.one;
    }

    IEnumerator BreatheCooldown()
    {
        canBreathe = false;
        yield return new WaitForSeconds(breatheCooldown);
        canBreathe = true;
    }
}