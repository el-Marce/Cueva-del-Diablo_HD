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

    [Header("Posición de reposo (se lee automático del RectTransform)")]
    // No tocar, se asigna en Awake desde la posición que pongas en el editor

    [Header("Animación - Entrada")]
    public SlideDirection direccionEntrada = SlideDirection.Bottom;
    public float slideDistance = 80f;
    public float slideInDuration = 0.35f;

    [Header("Animación - Salida")]
    public float displayDuration = 2.5f; // ignorado si se llama OcultarManual()
    public float slideOutDuration = 0.25f;

    public enum SlideDirection { Top, Bottom, Left, Right }

    Vector2 anchoredOrigin;
    Coroutine toastCoroutine;

    void Awake()
    {
        anchoredOrigin = panelRect.anchoredPosition;
        canvasGroup.alpha = 0f;
        panelRect.anchoredPosition = anchoredOrigin + GetOffset();
    }

    // Aparece y desaparece solo tras displayDuration
    public void Mostrar(string mensaje = null, Sprite icono = null)
    {
        if (toastCoroutine != null) StopCoroutine(toastCoroutine);
        AplicarContenido(mensaje, icono);
        toastCoroutine = StartCoroutine(Routine_AutoHide());
    }

    // Aparece y se queda hasta que llames Ocultar()
    public void MostrarPersistente(string mensaje = null, Sprite icono = null)
    {
        if (toastCoroutine != null) StopCoroutine(toastCoroutine);
        AplicarContenido(mensaje, icono);
        toastCoroutine = StartCoroutine(Routine_SlideIn());
    }

    public void Ocultar()
    {
        if (toastCoroutine != null) StopCoroutine(toastCoroutine);
        toastCoroutine = StartCoroutine(Routine_SlideOut());
    }

    void AplicarContenido(string mensaje, Sprite icono)
    {
        mensajeText.text = string.IsNullOrEmpty(mensaje) ? mensajePorDefecto : mensaje;

        if (iconoImage != null)
        {
            Sprite spriteAUsar = icono != null ? icono : iconoPorDefecto;
            iconoImage.sprite = spriteAUsar;
            iconoImage.gameObject.SetActive(spriteAUsar != null);
        }

        // Fuerza recálculo inmediato del layout para que texto e ícono queden centrados
        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(panelRect);
    }
    IEnumerator Routine_AutoHide()
    {
        yield return Routine_SlideIn();
        yield return new WaitForSeconds(displayDuration);
        yield return Routine_SlideOut();
    }

    IEnumerator Routine_SlideIn()
    {
        float elapsed = 0f;
        Vector2 desde = anchoredOrigin + GetOffset();
        canvasGroup.alpha = 0f;
        panelRect.anchoredPosition = desde;

        while (elapsed < slideInDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / slideInDuration;
            canvasGroup.alpha = t;
            panelRect.anchoredPosition = Vector2.Lerp(desde, anchoredOrigin, t);
            yield return null;
        }

        canvasGroup.alpha = 1f;
        panelRect.anchoredPosition = anchoredOrigin;
    }

    IEnumerator Routine_SlideOut()
    {
        float elapsed = 0f;
        Vector2 hasta = anchoredOrigin + GetOffset();

        while (elapsed < slideOutDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / slideOutDuration;
            canvasGroup.alpha = 1f - t;
            panelRect.anchoredPosition = Vector2.Lerp(anchoredOrigin, hasta, t);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        panelRect.anchoredPosition = anchoredOrigin + GetOffset();
    }

    Vector2 GetOffset()
    {
        return direccionEntrada switch
        {
            SlideDirection.Top => new Vector2(0, slideDistance),
            SlideDirection.Bottom => new Vector2(0, -slideDistance),
            SlideDirection.Left => new Vector2(-slideDistance, 0),
            SlideDirection.Right => new Vector2(slideDistance, 0),
            _ => Vector2.zero
        };
    }
}