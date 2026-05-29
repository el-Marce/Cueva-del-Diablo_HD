using UnityEngine;
using System.Collections;

public class TutorialBarrier : MonoBehaviour
{
    [Header("Colisión")]
    public Collider barrierCollider;

    [Header("Visual - Mundo")]
    public MeshRenderer barrierRenderer;
    public float fadeDuration = 0.4f;

    [Header("Pulso")]
    public float pulseSpeed = 2f;
    public float pulseMinAlpha = 0.2f;
    public float pulseMaxAlpha = 0.6f;

    [Header("Partículas")]
    public ParticleSystem contactParticles; // opcional

    Material barrierMat;
    Coroutine pulseCoroutine;
    Coroutine fadeCoroutine;

    void Awake()
    {
        if (barrierRenderer != null)
        {
            // Instancia propia para no afectar el material original
            barrierMat = new Material(barrierRenderer.material);
            barrierRenderer.material = barrierMat;
        }

        SetAlpha(0f);
        if (barrierCollider != null)
            barrierCollider.enabled = false;
    }

    public void Activar()
    {
        if (barrierCollider != null)
            barrierCollider.enabled = true;

        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeTo(pulseMinAlpha, fadeDuration, () =>
        {
            pulseCoroutine = StartCoroutine(Pulsar());
        }));
    }

    public void Desactivar()
    {
        if (pulseCoroutine != null)
        {
            StopCoroutine(pulseCoroutine);
            pulseCoroutine = null;
        }

        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeTo(0f, fadeDuration, () =>
        {
            if (barrierCollider != null)
                barrierCollider.enabled = false;
        }));
    }

    // Llamar desde PlayerMovement al chocar con la barrera
    public void ReaccionarAlContacto(Vector3 contactPoint)
    {
        if (contactParticles != null)
        {
            contactParticles.transform.position = contactPoint;
            contactParticles.Play();
        }

        // Flash rápido de alpha
        if (pulseCoroutine != null) StopCoroutine(pulseCoroutine);
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(Flash());
    }

    IEnumerator Pulsar()
    {
        while (true)
        {
            float t = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;
            SetAlpha(Mathf.Lerp(pulseMinAlpha, pulseMaxAlpha, t));
            yield return null;
        }
    }

    IEnumerator Flash()
    {
        yield return StartCoroutine(FadeTo(pulseMaxAlpha, 0.08f, null));
        yield return StartCoroutine(FadeTo(pulseMinAlpha, 0.2f, null));
        pulseCoroutine = StartCoroutine(Pulsar());
    }

    IEnumerator FadeTo(float targetAlpha, float duration, System.Action onComplete)
    {
        float startAlpha = GetAlpha();
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            SetAlpha(Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration));
            yield return null;
        }

        SetAlpha(targetAlpha);
        onComplete?.Invoke();
    }

    void SetAlpha(float alpha)
    {
        if (barrierMat == null) return;
        Color c = barrierMat.color;
        c.a = alpha;
        barrierMat.color = c;
    }

    float GetAlpha()
    {
        if (barrierMat == null) return 0f;
        return barrierMat.color.a;
    }
}