using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PantallaInicio : MonoBehaviour
{
    [Header("Referencias")]
    public Image logoImage;

    [Header("Timing")]
    public float fadeDuration = 1.5f;
    public float displayDuration = 2f;

    void Start()
    {
        StartCoroutine(IntroSequence());
    }

    IEnumerator IntroSequence()
    {
        // Fade in
        yield return StartCoroutine(Fade(0f, 1f, fadeDuration));

        // Mostrar logo
        yield return new WaitForSeconds(displayDuration);

        // Fade out
        yield return StartCoroutine(Fade(1f, 0f, fadeDuration));

        SceneManager.LoadScene("PantallaPortada");
    }

    IEnumerator Fade(float from, float to, float duration)
    {
        float elapsed = 0f;
        Color c = logoImage.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, elapsed / duration);
            logoImage.color = c;
            yield return null;
        }

        c.a = to;
        logoImage.color = c;
    }
}