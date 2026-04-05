using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.SceneManagement;

public class SanityVisualEffects : MonoBehaviour
{
    public Volume volume;
    SanitySystem sanitySystem;
    LensDistortion lensDistortion;

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
        sanitySystem = GameObject.FindGameObjectWithTag("Player")
                                 .GetComponentInChildren<SanitySystem>();
    }

    void Start()
    {
        volume.profile.TryGet(out lensDistortion);
        sanitySystem = GameObject.FindGameObjectWithTag("Player")
                                 .GetComponentInChildren<SanitySystem>();
    }

    void Update()
    {
        if (sanitySystem == null) return;
        float sanity = sanitySystem.GetSanityNormalized();
        float distortion = Mathf.Lerp(-0.6f, 0f, sanity);
        float noise = Mathf.Sin(Time.time * 3f) * 0.05f;
        lensDistortion.intensity.value = distortion + noise;
    }
}