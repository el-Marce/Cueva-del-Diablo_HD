using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class PantallaPortada : MonoBehaviour
{
    [Header("Referencias")]
    public Image fondoImage;
    public TMP_Text promptText;

    [Header("Fade portada")]
    public float fadeDuration = 1.5f;

    [Header("Espera antes del prompt")]
    public float waitBeforePrompt = 1f;

    [Header("Blink del prompt")]
    public float blinkFadeDuration = 0.4f; // velocidad del fade in/out

    bool skipped = false;
    bool promptActive = false;

    void Start()
    {
        SetPromptAlpha(0f);
        StartCoroutine(PortadaSequence());
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            if (promptActive)
                SceneManager.LoadScene("MenuPrincipal");
            else
                skipped = true;
        }
    }

    IEnumerator PortadaSequence()
    {
        // Fade in saltable
        float elapsed = 0f;
        Color c = fondoImage.color;
        c.a = 0f;
        fondoImage.color = c;

        while (elapsed < fadeDuration && !skipped)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Clamp01(elapsed / fadeDuration);
            fondoImage.color = c;
            yield return null;
        }

        // Asegura alpha final
        c.a = 1f;
        fondoImage.color = c;

        // Espera antes del prompt
        yield return new WaitForSeconds(waitBeforePrompt);

        // Activa prompt y arranca blink
        promptActive = true;
        StartCoroutine(BlinkPrompt());
    }

    IEnumerator BlinkPrompt()
    {
        while (true)
        {
            yield return StartCoroutine(FadePrompt(0f, 1f));
            yield return StartCoroutine(FadePrompt(1f, 0f));
        }
    }

    IEnumerator FadePrompt(float from, float to)
    {
        float elapsed = 0f;
        while (elapsed < blinkFadeDuration)
        {
            elapsed += Time.deltaTime;
            SetPromptAlpha(Mathf.Lerp(from, to, elapsed / blinkFadeDuration));
            yield return null;
        }
        SetPromptAlpha(to);
    }

    void SetPromptAlpha(float alpha)
    {
        Color c = promptText.color;
        c.a = alpha;
        promptText.color = c;
    }
}