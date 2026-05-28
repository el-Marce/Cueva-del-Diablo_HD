using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class CinematicaManager : MonoBehaviour
{
    [Header("Referencias")]
    public Image ilustracionImage;
    public TMP_Text subtituloText;
    public Image subtituloPanel;
    public GameObject skipPrompt;

    [Header("Frames")]
    public CinematicaFrame[] frames;

    [Header("Transición - Ilustración")]
    public float fadeDuration = 0.5f;

    [Header("Transición - Panel de Subtítulo (Fade IN)")]
    public float panelFadeInDuration = 0.3f;
    [Tooltip("Segundos de espera tras el fade in del panel antes de empezar a escribir el texto")]
    public float delayTextAfterPanelFadeIn = 0.2f;

    [Header("Transición - Texto (Fade OUT)")]
    [Tooltip("Segundos de espera antes de iniciar el fade out del texto al avanzar frame")]
    public float delayTextFadeOut = 0f;
    public float textFadeOutDuration = 0.3f;

    [Header("Transición - Panel de Subtítulo (Fade OUT)")]
    [Tooltip("Segundos de espera tras el inicio del fade out del texto antes de iniciar el fade out del panel")]
    public float delayPanelFadeOutAfterText = 0.1f;
    public float panelFadeOutDuration = 0.3f;

    public string escenaSiguiente = "Nivel_01";

    int currentFrame = 0;
    bool subtituloCompleto = false;
    bool puedeAvanzar = false;
    bool saltando = false;
    Coroutine escrituraCoroutine;

    void Start()
    {
        CinematicaFrame frame = frames[0];
        AudioManager.Instance.PlayMusica("event:/Cinematicas/MusicBack");

        SetPanelAlpha(0f);
        SetTextAlpha(0f);

        ilustracionImage.enabled = true;
        ilustracionImage.sprite = frame.ilustracion;

        StartCoroutine(MostrarFrame(0));
    }

    void Update()
    {
        if (saltando || !puedeAvanzar) return;

        if (Input.GetKeyDown(KeyCode.N))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            AudioManager.Instance.StopMusica();
        }

        if (Input.anyKeyDown)
        {
            if (!subtituloCompleto)
                SkipEscritura();
            else
                StartCoroutine(AvanzarFrame());
        }
    }

    IEnumerator MostrarFrame(int index)
    {
        saltando = true;
        subtituloCompleto = false;
        puedeAvanzar = false;
        subtituloText.text = "";
        SetTextAlpha(1f);

        CinematicaFrame frame = frames[index];

        yield return StartCoroutine(FadeIlustracion(0f, 1f));

        yield return new WaitForSeconds(0.75f);
        puedeAvanzar = true;
        saltando = false;

        escrituraCoroutine = StartCoroutine(EscribirSubtituloConPanel(frame));
    }

    IEnumerator EscribirSubtituloConPanel(CinematicaFrame frame)
    {
        subtituloText.text = "";

        yield return StartCoroutine(FadePanel(0f, 1f, panelFadeInDuration));

        // Espera configurable antes de empezar a escribir
        if (delayTextAfterPanelFadeIn > 0f)
            yield return new WaitForSeconds(delayTextAfterPanelFadeIn);

        foreach (char c in frame.subtitulo)
        {
            subtituloText.text += c;
            yield return new WaitForSeconds(frame.velocidadEscritura);
        }

        subtituloCompleto = true;
    }

    void SkipEscritura()
    {
        if (escrituraCoroutine != null)
            StopCoroutine(escrituraCoroutine);

        subtituloText.text = frames[currentFrame].subtitulo;
        SetPanelAlpha(1f);
        SetTextAlpha(1f);
        subtituloCompleto = true;
    }

    IEnumerator AvanzarFrame()
    {
        saltando = true;

        // Espera antes de iniciar el fade out del texto
        if (delayTextFadeOut > 0f)
            yield return new WaitForSeconds(delayTextFadeOut);

        // Fade out del texto y, tras el delay configurable, fade out del panel
        yield return StartCoroutine(FadeOutTextoYPanel());

        subtituloText.text = "";
        SetTextAlpha(1f); // reset para el siguiente frame

        yield return StartCoroutine(FadeIlustracion(1f, 0f));

        currentFrame++;

        if (currentFrame >= frames.Length)
        {
            AudioManager.Instance.StopMusica();
            if (SceneTransition.Instance != null)
                SceneTransition.Instance.TransitionTo(escenaSiguiente, holdDuration: 3f);
            else
                UnityEngine.SceneManagement.SceneManager.LoadScene(escenaSiguiente);
            yield break;
        }

        CinematicaFrame frame = frames[currentFrame];
        ilustracionImage.enabled = true;
        ilustracionImage.sprite = frame.ilustracion;

        StartCoroutine(MostrarFrame(currentFrame));
    }

    // Fade out del texto primero, luego el panel con delay configurable
    IEnumerator FadeOutTextoYPanel()
    {
        // Lanzamos el fade out del texto
        Coroutine fadeTexto = StartCoroutine(FadeText(1f, 0f, textFadeOutDuration));

        // Esperamos el delay antes de iniciar el fade out del panel
        if (delayPanelFadeOutAfterText > 0f)
            yield return new WaitForSeconds(delayPanelFadeOutAfterText);

        // Iniciamos el fade out del panel (puede solaparse con el del texto o empezar después)
        Coroutine fadePanel = StartCoroutine(FadePanel(1f, 0f, panelFadeOutDuration));

        // Esperamos a que ambos terminen
        yield return fadeTexto;
        yield return fadePanel;
    }

    IEnumerator FadeIlustracion(float from, float to)
    {
        float elapsed = 0f;
        Color c = ilustracionImage.color;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, elapsed / fadeDuration);
            ilustracionImage.color = c;
            yield return null;
        }
        c.a = to;
        ilustracionImage.color = c;
    }

    IEnumerator FadePanel(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            SetPanelAlpha(Mathf.Lerp(from, to, elapsed / duration));
            yield return null;
        }
        SetPanelAlpha(to);
    }

    IEnumerator FadeText(float from, float to, float duration)
    {
        float elapsed = 0f;
        Color c = subtituloText.color;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, elapsed / duration);
            subtituloText.color = c;
            yield return null;
        }
        c.a = to;
        subtituloText.color = c;
    }

    void SetPanelAlpha(float alpha)
    {
        if (subtituloPanel == null) return;
        Color c = subtituloPanel.color;
        c.a = alpha;
        subtituloPanel.color = c;
    }

    void SetTextAlpha(float alpha)
    {
        Color c = subtituloText.color;
        c.a = alpha;
        subtituloText.color = c;
    }
}