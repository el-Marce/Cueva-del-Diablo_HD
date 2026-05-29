using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [Header("UI")]
    public GameObject tutorialPanel;
    public TMP_Text tutorialText;
    public Image tutorialIcon;
    public RectTransform panelRect;

    [Header("UI - Barrera")]
    [Tooltip("Panel/icono que aparece en UI cuando hay una barrera activa")]
    public GameObject barrierHintUI;
    public TMP_Text barrierHintText;
    public string barrierHintMensaje = "Completa el objetivo para continuar";

    [Header("Animación")]
    public float fadeDuration = 0.3f;
    public float autoHideDelay = 0f;

    [Header("Configuración")]
    public bool tutorialActivo = true;

    CanvasGroup canvasGroup;
    bool bloqueandoAvance = false;
    string triggerPendiente = null;
    TutorialBarrier barreraActiva = null;

    [Header("Toast de desbloqueo")]
    public TutorialUnlockToast unlockToast;
    TutorialStep pasoActual = null;
    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;

        canvasGroup = tutorialPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = tutorialPanel.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;
        tutorialPanel.SetActive(false);

        if (barrierHintUI != null)
            barrierHintUI.SetActive(false);
    }

    void Start()
    {
        if (PlayerPrefs.GetInt("TutorialCompletado", 0) == 1)
            tutorialActivo = false;
    }

    public void MostrarPaso(TutorialStep paso, TutorialBarrier barrera = null)
    {
        if (!tutorialActivo) return;
        StopAllCoroutines();
        StartCoroutine(MostrarPasoRoutine(paso, barrera));
    }

    IEnumerator MostrarPasoRoutine(TutorialStep paso, TutorialBarrier barrera)
    {
        pasoActual = paso;
        tutorialText.text = paso.mensaje;

        if (tutorialIcon != null)
        {
            tutorialIcon.sprite = paso.icono;
            tutorialIcon.gameObject.SetActive(paso.icono != null);
        }

        if (panelRect != null)
            panelRect.anchorMin = panelRect.anchorMax = paso.posicionAnclaje;

        if (paso.bloqueaAvance)
        {
            bloqueandoAvance = true;
            triggerPendiente = paso.triggerDeDesbloqueo;

            barreraActiva = barrera;
            if (barreraActiva != null)
                barreraActiva.Activar();

            // El hint de barrera ya NO se activa aquí
        }

        tutorialPanel.SetActive(true);
        yield return StartCoroutine(FadePanel(0f, 1f));

        if (autoHideDelay > 0f && !paso.bloqueaAvance)
        {
            yield return new WaitForSeconds(autoHideDelay);
            yield return StartCoroutine(OcultarPanel());
        }
    }

    public void CompletarTrigger(string trigger)
    {
        if (!bloqueandoAvance) return;
        if (triggerPendiente != trigger) return;

        bloqueandoAvance = false;
        triggerPendiente = null;

        // Desactivar barrera
        if (barreraActiva != null)
        {
            barreraActiva.Desactivar();
            barreraActiva = null;
        }

        // Ocultar hint UI
        if (barrierHintUI != null)
        {
            TutorialUnlockToast hintToast = barrierHintUI.GetComponent<TutorialUnlockToast>();
            if (hintToast != null)
                hintToast.StopAllCoroutines(); // corta cualquier toast activo
            barrierHintUI.SetActive(false);
        }

        if (unlockToast != null)
        {
            string msg = !string.IsNullOrEmpty(pasoActual?.toastMensaje)
                ? pasoActual.toastMensaje
                : null;

            Sprite ico = pasoActual?.toastIcono;

            if (msg != null || ico != null)
                unlockToast.Mostrar(msg ?? unlockToast.mensajePorDefecto, ico);
            else
                unlockToast.Mostrar();
        }

        StartCoroutine(OcultarPanel());
    }

    // Llamado desde el controlador del jugador al colisionar con la barrera
    public void NotificarContactoBarrera(Vector3 contactPoint)
    {
        if (barreraActiva == null) return;

        barreraActiva.ReaccionarAlContacto(contactPoint);

        // Mostrar hint solo en el primer contacto, igual que el toast
        if (unlockToast != null && barrierHintUI != null)
        {
            // Reutiliza TutorialUnlockToast que ya está en barrierHintUI
            TutorialUnlockToast hintToast = barrierHintUI.GetComponent<TutorialUnlockToast>();
            if (hintToast != null)
                hintToast.Mostrar(barrierHintMensaje, null);
        }
    }

    public void OcultarPasoActual()
    {
        StopAllCoroutines();
        StartCoroutine(OcultarPanel());
    }

    IEnumerator OcultarPanel()
    {
        yield return StartCoroutine(FadePanel(1f, 0f));
        tutorialPanel.SetActive(false);
    }

    IEnumerator FadePanel(float from, float to)
    {
        float elapsed = 0f;
        canvasGroup.alpha = from;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = to;
    }

    public void MarcarTutorialCompletado()
    {
        PlayerPrefs.SetInt("TutorialCompletado", 1);
        PlayerPrefs.Save();
    }

    public void DesactivarTutorial()
    {
        tutorialActivo = false;
        OcultarPasoActual();
    }
}