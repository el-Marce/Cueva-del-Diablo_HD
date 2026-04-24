using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;
using System.Collections;

public class CinematicaManager : MonoBehaviour
{
    [Header("Referencias")]
    public Image ilustracionImage;
    //public VideoPlayer videoPlayer;
    public TMP_Text subtituloText;
    public GameObject skipPrompt;
    //public AudioSource audioSource;

    [Header("Frames")]
    public CinematicaFrame[] frames;

    [Header("Transición")]
    public float fadeDuration = 0.5f;
    public string escenaSiguiente = "Nivel_01";

    int currentFrame = 0;
    bool subtituloCompleto = false;
    bool puedeAvanzar = false;
    bool saltando = false;
    Coroutine escrituraCoroutine;

    void Start()
    {
        // Carga el primer frame directamente
        CinematicaFrame frame = frames[0];
        AudioManager.Instance.PlayMusica("event:/Cinematicas/MusicBack");
        if (frame.videoAnimado != null)
        {
            //videoPlayer.clip = frame.videoAnimado;
            //videoPlayer.Play();
            ilustracionImage.enabled = false;
        }
        else
        {
            //videoPlayer.Stop();
            ilustracionImage.enabled = true;
            ilustracionImage.sprite = frame.ilustracion;
        }

        StartCoroutine(MostrarFrame(0));
    }

    void Update()
    {
        if (saltando || !puedeAvanzar) return;

        if (Input.anyKeyDown)
        {
            if (!subtituloCompleto)
            {
                // Primer skip — completa el subtítulo inmediatamente
                SkipEscritura();
            }
            else
            {
                // Segundo skip — avanza al siguiente frame
                StartCoroutine(AvanzarFrame());
            }
        }
    }

    IEnumerator MostrarFrame(int index)
    {
        saltando = true;
        subtituloCompleto = false;
        puedeAvanzar = false;
        subtituloText.text = "";

        CinematicaFrame frame = frames[index];

        // Fade in con el contenido ya asignado por AvanzarFrame
        yield return StartCoroutine(FadeIlustracion(0f, 1f));

        // Narración
        //if (frame.narracion != null)
        //{
        //    audioSource.clip = frame.narracion;
        //    audioSource.Play();
        //}

        yield return new WaitForSeconds(0.75f);
        puedeAvanzar = true;
        saltando = false;

        escrituraCoroutine = StartCoroutine(EscribirSubtitulo(frame));
    }

    IEnumerator EscribirSubtitulo(CinematicaFrame frame)
    {
        subtituloText.text = "";
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
        subtituloCompleto = true;
    }

    IEnumerator AvanzarFrame()
    {
        saltando = true;
        //audioSource.Stop();

        yield return StartCoroutine(FadeIlustracion(1f, 0f));

        currentFrame++;

        if (currentFrame >= frames.Length)
        {
            if (SceneTransition.Instance != null)
            {
                AudioManager.Instance.StopMusica();
                SceneTransition.Instance.TransitionTo(escenaSiguiente, holdDuration: 3f);
            }
            else
            {
                // Fallback directo si no hay SceneTransition
                UnityEngine.SceneManagement.SceneManager.LoadScene(escenaSiguiente);
            }
            yield break;
        }

        // Cambia el contenido MIENTRAS está en negro
        CinematicaFrame frame = frames[currentFrame];

        if (frame.videoAnimado != null)
        {
            //videoPlayer.clip = frame.videoAnimado;
            //videoPlayer.Play();
            ilustracionImage.enabled = false;
        }
        else
        {
            //videoPlayer.Stop();
            ilustracionImage.enabled = true;
            ilustracionImage.sprite = frame.ilustracion;
        }
        StartCoroutine(MostrarFrame(currentFrame));
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
}