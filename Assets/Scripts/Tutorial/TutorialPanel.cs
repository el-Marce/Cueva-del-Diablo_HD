using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class TutorialPanel : MonoBehaviour
{
    [Header("Referencias")]
    public CanvasGroup canvasGroup;
    public TMP_Text mensajeText;
    public Image iconoImage;
    public RectTransform panelRect;
    public RectTransform textoRect;

    [Header("Animación")]
    public float fadeDuration = 0.3f;

    // Mensaje principal
    string mensajePrincipal;
    Sprite iconoPrincipal;
    KeyCode[] teclasPrincipal;
    float timerPrincipal;
    bool autoCierrePrincipal;

    // Secuencia
    MensajeSecuencia[] secuencia;
    int indiceSecuencia = -1; // -1 = estamos en el mensaje principal

    // Estado actual
    KeyCode[] teclasActuales;
    float timerActual;
    bool usaAutoCierre;

    Coroutine fadeCoroutine;

    void Awake()
    {
        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (canvasGroup.alpha < 1f) return;

        if (usaAutoCierre)
        {
            timerActual -= Time.deltaTime;
            if (timerActual <= 0f)
                Confirmar();
        }
        else
        {
            if (teclasActuales == null || teclasActuales.Length == 0) return;
            foreach (KeyCode key in teclasActuales)
            {
                if (Input.GetKeyDown(key))
                {
                    Confirmar();
                    break;
                }
            }
        }
    }

    public void Mostrar(string mensaje, Sprite icono = null, MensajeSecuencia[] mensajesExtra = null,
                        KeyCode[] teclas = null, float autoCierre = 0f,
                        Vector2? anclaje = null, Vector2? tamaño = null,
                        Vector2? tamañoTexto = null)
    {
        mensajePrincipal = mensaje;
        iconoPrincipal = icono;
        teclasPrincipal = teclas;
        timerPrincipal = autoCierre;
        autoCierrePrincipal = autoCierre > 0f;

        secuencia = mensajesExtra;
        indiceSecuencia = -1;

        AplicarContenido(mensaje, icono);
        AplicarComportamiento(autoCierre > 0f, teclas, autoCierre);

        // Posición y tamaño del panel
        if (panelRect != null)
        {
            if (anclaje.HasValue)
            {
                panelRect.anchorMin = anclaje.Value;
                panelRect.anchorMax = anclaje.Value;
                panelRect.anchoredPosition = Vector2.zero;
            }
            if (tamaño.HasValue)
                panelRect.sizeDelta = tamaño.Value;
        }

        if (textoRect != null && tamañoTexto.HasValue)
            textoRect.sizeDelta = tamañoTexto.Value;

        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        gameObject.SetActive(true);
        fadeCoroutine = StartCoroutine(FadeTo(1f, fadeDuration));
    }

    // Aplica teclas o timer según el mensaje actual
    void AplicarComportamiento(bool esAutoCierre, KeyCode[] teclas, float timer)
    {
        usaAutoCierre = esAutoCierre;
        teclasActuales = esAutoCierre ? null : teclas;
        timerActual = esAutoCierre ? timer : 0f;
    }

    void Confirmar()
    {
        indiceSecuencia++;

        bool haySecuencia = secuencia != null && secuencia.Length > 0;

        if (haySecuencia && indiceSecuencia < secuencia.Length)
        {
            MensajeSecuencia actual = secuencia[indiceSecuencia];
            bool esAuto = actual.tiempoAutoCierre > 0f;

            AplicarComportamiento(esAuto, actual.teclas, actual.tiempoAutoCierre);

            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(TransicionSecuencia(actual.mensaje));
        }
        else
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeYDesactivar());
        }
    }

    void AplicarContenido(string mensaje, Sprite icono)
    {
        mensajeText.text = mensaje;
        if (iconoImage != null)
        {
            iconoImage.sprite = icono;
            iconoImage.gameObject.SetActive(icono != null);
        }
    }

    IEnumerator TransicionSecuencia(string nuevoMensaje)
    {
        yield return FadeTo(0f, fadeDuration);
        mensajeText.text = nuevoMensaje;
        yield return FadeTo(1f, fadeDuration);
    }

    IEnumerator FadeYDesactivar()
    {
        yield return FadeTo(0f, fadeDuration);
        gameObject.SetActive(false);
        TutorialManager.Instance?.OnPanelConfirmado();
    }

    IEnumerator FadeTo(float target, float duration)
    {
        float start = canvasGroup.alpha;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, target, elapsed / duration);
            yield return null;
        }
        canvasGroup.alpha = target;
    }
}