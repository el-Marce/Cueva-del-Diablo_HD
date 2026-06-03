// TutorialBarrier.cs
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
    public ParticleSystem contactParticles;

    public bool estaActiva = false;

    [Header("Comportamiento")]
    public bool activarAlInicio = false;

    [Header("Toast de contacto")]
    public TutorialUnlockToast barreraToast;

    Material barrierMat;
    bool debePulsar = false;          // <-- flag central
    bool jugadorEnContacto = false;

    void Awake()
    {
        if (barrierRenderer != null)
        {
            barrierMat = new Material(barrierRenderer.material);
            barrierRenderer.material = barrierMat;
        }
        SetAlpha(0f);
        if (barrierCollider != null)
            barrierCollider.enabled = false;
    }

    void Start()
    {
        if (activarAlInicio) Activar();
    }

    // Un solo loop que vive toda la vida del objeto
    void Update()
    {
        if (debePulsar)
        {
            float t = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;
            SetAlpha(Mathf.Lerp(pulseMinAlpha, pulseMaxAlpha, t));
        }
    }

    public void Activar()
    {
        estaActiva = true;
        jugadorEnContacto = false;
        debePulsar = true;

        if (barrierCollider != null)
            barrierCollider.enabled = true;
    }

    public void Desactivar()
    {
        estaActiva = false;
        jugadorEnContacto = false;
        debePulsar = false;           // apaga el pulso sin importar nada más

        StopAllCoroutines();          // cancela cualquier fade en curso
        StartCoroutine(FadeTo(0f, fadeDuration, () =>
        {
            if (barrierCollider != null)
                barrierCollider.enabled = false;

        }));

        gameObject.SetActive(false);
    }

    string mensajeToastActual = "";

    public void SetMensajeToast(string mensaje)
    {
        mensajeToastActual = string.IsNullOrEmpty(mensaje) ? "" : mensaje;
    }
    float cooldownContacto = 0f;
    float tiempoRefresco = 1f; // ajustable en Inspector

    public void NotificarContacto(Vector3 contactPoint)
    {
        if (!estaActiva) return;
        ReaccionarAlContacto(contactPoint);

        cooldownContacto -= Time.deltaTime;
        if (cooldownContacto <= 0f)
        {
            cooldownContacto = tiempoRefresco;
            if (barreraToast != null)
            {
                string mensaje = TutorialManager.Instance?.pasoActual?.toastMensaje;
                Debug.Log("Mensaje: " + mensaje);
                barreraToast.Mostrar(string.IsNullOrEmpty(mensaje) ? null : mensaje);
            }
        }
    }

    public void NotificarSeparacion()
    {
        jugadorEnContacto = false;
    }
    public void ReaccionarAlContacto(Vector3 contactPoint)
    {
        if (contactParticles != null)
        {
            contactParticles.transform.position = contactPoint;
            contactParticles.Play();
        }

        StopAllCoroutines();
        StartCoroutine(Flash());
    }

    IEnumerator Flash()
    {
        debePulsar = false;
        yield return StartCoroutine(FadeTo(pulseMaxAlpha, 0.08f, null));
        yield return StartCoroutine(FadeTo(pulseMinAlpha, 0.2f, null));
        if (estaActiva) debePulsar = true;    // solo reactiva si sigue activa
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
    //public void SetMensajeToast(string mensaje)
    //{
    //    if (barreraToast != null)
    //        barreraToast.mensajePorDefecto = string.IsNullOrEmpty(mensaje)
    //            ? barreraToast.mensajePorDefecto
    //            : mensaje;
    //}
}