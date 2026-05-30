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
    public Button botonEntendido; // botón "Entendido" en la UI

    [Header("Animación")]
    public float fadeDuration = 0.3f;

    Coroutine fadeCoroutine;

    void Awake()
    {
        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);

        if (botonEntendido != null)
            botonEntendido.onClick.AddListener(Ocultar);
    }

    void Update()
    {
        // Enter cierra el panel si está visible
        if (canvasGroup.alpha >= 1f && Input.GetKeyDown(KeyCode.Return))
            Ocultar();
    }

    public void Mostrar(string mensaje, Sprite icono = null)
    {
        mensajeText.text = mensaje;

        if (iconoImage != null)
        {
            iconoImage.sprite = icono;
            iconoImage.gameObject.SetActive(icono != null);
        }

        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        gameObject.SetActive(true);
        fadeCoroutine = StartCoroutine(FadeTo(1f, fadeDuration));
    }

    public void Ocultar()
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeYDesactivar());
    }

    IEnumerator FadeYDesactivar()
    {
        yield return FadeTo(0f, fadeDuration);
        gameObject.SetActive(false);

        // Avisa al TutorialManager que el jugador confirmó
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