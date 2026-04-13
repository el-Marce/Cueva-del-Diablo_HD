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
    public bool IsTransitioning { get; private set; }

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
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Cada vez que carga una escena nueva hace fade in automático
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        IsTransitioning = true;
        yield return StartCoroutine(Fade(1f, 0f));
        IsTransitioning = false;
    }
    public void TransitionTo(string sceneName, float delay = 0f)
    {
        StartCoroutine(TransitionRoutine(sceneName, delay));
    }

    IEnumerator TransitionRoutine(string sceneName, float delay)
    {
        IsTransitioning = true;

        yield return new WaitForSeconds(delay);
        yield return StartCoroutine(Fade(0f, 1f));

        SetAlpha(1f);
        SceneManager.LoadScene(sceneName);

        yield return null;

        yield return StartCoroutine(Fade(1f, 0f));

        IsTransitioning = false;
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