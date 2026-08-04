using System.Collections;
using UnityEngine;

[RequireComponent(typeof(EnemyStats))]
public class EnemyDamageFlash : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Renderer targetRenderer; // si lo dejas vacío, se busca automáticamente
    [SerializeField] private string propertyName = "_RedIntensity"; // revisa el Reference exacto en el Shader Graph

    [Header("Config del flash")]
    [SerializeField] private float flashValue = 5f;
    [SerializeField] private float flashDuration = 0.15f; // tiempo en volver a 0

    private EnemyStats stats;
    private MaterialPropertyBlock mpb;
    private Coroutine flashRoutine;
    private int propId;

    void Awake()
    {
        stats = GetComponent<EnemyStats>();
        if (targetRenderer == null)
            targetRenderer = GetComponentInChildren<Renderer>();

        mpb = new MaterialPropertyBlock();
        propId = Shader.PropertyToID(propertyName);
    }

    void OnEnable() => stats.OnHit += HandleHit;
    void OnDisable() => stats.OnHit -= HandleHit;

    void HandleHit()
    {
        if (flashRoutine != null) StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        float t = 0f;
        SetIntensity(flashValue);

        while (t < flashDuration)
        {
            t += Time.deltaTime;
            float value = Mathf.Lerp(flashValue, 0f, t / flashDuration);
            SetIntensity(value);
            yield return null;
        }

        SetIntensity(0f);
        flashRoutine = null;
    }

    void SetIntensity(float value)
    {
        if (targetRenderer == null) return;
        targetRenderer.GetPropertyBlock(mpb);
        mpb.SetFloat(propId, value);
        targetRenderer.SetPropertyBlock(mpb);
    }
}