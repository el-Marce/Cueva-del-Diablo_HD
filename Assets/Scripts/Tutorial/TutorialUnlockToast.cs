using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class TutorialUnlockToast : MonoBehaviour
{
    [Header("Referencias")]
    public CanvasGroup canvasGroup;
    public RectTransform panelRect;
    public TMP_Text mensajeText;
    public Image iconoImage;

    [Header("Contenido por defecto")]
    public string mensajePorDefecto = "¡Camino despejado!";
    public Sprite iconoPorDefecto;

    [Header("Animación - Entrada")]
    public SlideDirection direccionEntrada = SlideDirection.Bottom;
    public float slideDistance = 80f;
    public float slideInDuration = 0.35f;
    public AnimationCurve slideInCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Animación - Salida")]
    public float displayDuration = 2.5f;
    public float slideOutDuration = 0.25f;
    public AnimationCurve slideOutCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    public enum SlideDirection { Top, Bottom, Left, Right }

    Vector2 anchoredOrigin;
    Coroutine toastCoroutine;

    void Awake()
    {
        anchoredOrigin = panelRect.anchoredPosition;
        canvasGroup.alpha = 0f;
        panelRect.anchoredPosition = anchoredOrigin + GetSlideOffset();
        gameObject.SetActive(false);
    }

    // Muestra el toast con contenido por defecto
    public void Mostrar()
    {
        Mostrar(mensajePorDefecto, iconoPorDefecto);
    }

    // Muestra el toast con contenido personalizado por step
    public void Mostrar(string mensaje, Sprite icono)
    {
        if (toastCoroutine != null)
            StopCoroutine(toastCoroutine);

        mensajeText.text = mensaje;

        if (iconoImage != null)
        {
            iconoImage.sprite = icono != null ? icono : iconoPorDefecto;
            iconoImage.gameObject.SetActive(iconoImage.sprite != null);
        }

        toastCoroutine = StartCoroutine(ToastRoutine());
    }

    IEnumerator ToastRoutine()
    {
        // Reset estado inicial
        gameObject.SetActive(true);
        canvasGroup.alpha = 0f;
        panelRect.anchoredPosition = anchoredOrigin + GetSlideOffset();

        // Slide in
        float elapsed = 0f;
        Vector2 offsetInicio = GetSlideOffset();

        while (elapsed < slideInDuration)
        {
            elapsed += Time.deltaTime;
            float t = slideInCurve.Evaluate(elapsed / slideInDuration);
            canvasGroup.alpha = t;
            panelRect.anchoredPosition = Vector2.Lerp(anchoredOrigin + offsetInicio, anchoredOrigin, t);
            yield return null;
        }

        canvasGroup.alpha = 1f;
        panelRect.anchoredPosition = anchoredOrigin;

        // Espera visible
        yield return new WaitForSeconds(displayDuration);

        // Slide out (sale hacia el mismo borde por el que entró)
        elapsed = 0f;

        while (elapsed < slideOutDuration)
        {
            elapsed += Time.deltaTime;
            float t = slideOutCurve.Evaluate(elapsed / slideOutDuration);
            canvasGroup.alpha = 1f - t;
            panelRect.anchoredPosition = Vector2.Lerp(anchoredOrigin, anchoredOrigin + offsetInicio, t);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        panelRect.anchoredPosition = anchoredOrigin + offsetInicio;
        gameObject.SetActive(false);
    }

    Vector2 GetSlideOffset()
    {
        return direccionEntrada switch
        {
            SlideDirection.Top => new Vector2(0f, slideDistance),
            SlideDirection.Bottom => new Vector2(0f, -slideDistance),
            SlideDirection.Left => new Vector2(-slideDistance, 0f),
            SlideDirection.Right => new Vector2(slideDistance, 0f),
            _ => Vector2.zero
        };
    }
}