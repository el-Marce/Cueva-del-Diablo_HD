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
    public Button botonEntendido;

    [Header("Animación")]
    public float fadeDuration = 0.3f;

    string[] secuencia;
    int indiceSecuencia = 0;
    Coroutine fadeCoroutine;

    void Awake()
    {
        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);

        if (botonEntendido != null)
            botonEntendido.onClick.AddListener(Confirmar);
    }

    void Update()
    {
        if (canvasGroup.alpha >= 1f && Input.GetKeyDown(KeyCode.Return))
            Confirmar();
    }

    public void Mostrar(string mensaje, Sprite icono = null, string[] mensajesExtra = null)
    {
        // Armar secuencia completa: primer mensaje + extras
        if (mensajesExtra != null && mensajesExtra.Length > 0)
        {
            secuencia = new string[1 + mensajesExtra.Length];
            secuencia[0] = mensaje;
            for (int i = 0; i < mensajesExtra.Length; i++)
                secuencia[i + 1] = mensajesExtra[i];
        }
        else
        {
            secuencia = new string[] { mensaje };
        }

        indiceSecuencia = 0;

        AplicarContenido(secuencia[0], icono);

        GameState.InMenu = true;

        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        gameObject.SetActive(true);
        fadeCoroutine = StartCoroutine(FadeTo(1f, fadeDuration));
    }

    void Confirmar()
    {
        indiceSecuencia++;

        if (indiceSecuencia < secuencia.Length)
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(TransicionSecuencia(secuencia[indiceSecuencia]));
        }
        else
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeYDesactivar());
        }
    }

    IEnumerator TransicionSecuencia(string nuevoMensaje)
    {
        yield return FadeTo(0f, fadeDuration);
        mensajeText.text = nuevoMensaje;
        yield return FadeTo(1f, fadeDuration);
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

    IEnumerator FadeYDesactivar()
    {
        yield return FadeTo(0f, fadeDuration);
        gameObject.SetActive(false);
        GameState.InMenu = false;
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