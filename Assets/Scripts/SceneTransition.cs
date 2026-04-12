using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance;

    [Header("Referencias")]
    public Image fadePanel;

    [Header("Timing")]
    public float fadeDuration = 0.8f;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Asegura que el FadePanel esté siempre al frente
        Canvas canvas = fadePanel.GetComponentInParent<Canvas>();
        canvas.sortingOrder = 999;

        SetAlpha(0f);
    }

    public void TransitionTo(string sceneName, float delay = 0f)
    {
        StartCoroutine(TransitionRoutine(sceneName, delay));
    }

    IEnumerator TransitionRoutine(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        yield return StartCoroutine(Fade(0f, 1f));

        // Mantiene el panel opaco durante la carga
        SetAlpha(1f);

        SceneManager.LoadScene(sceneName);

        // Espera un frame para que la escena cargue
        yield return null;

        yield return StartCoroutine(Fade(1f, 0f));
    }

    IEnumerator Fade(float from, float to)
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            SetAlpha(Mathf.Lerp(from, to, elapsed / fadeDuration));
            yield return null;
        }
        SetAlpha(to);
    }

    void SetAlpha(float alpha)
    {
        Color c = fadePanel.color;
        c.a = alpha;
        fadePanel.color = c;
    }
}