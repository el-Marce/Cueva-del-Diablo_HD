using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class SanityVisualEffects : MonoBehaviour
{
    public SanitySystem sanitySystem;

    public Volume volume;

    LensDistortion lensDistortion;

    void Start()
    {
        volume.profile.TryGet(out lensDistortion);
    }

    void Update()
    {
        float sanity = sanitySystem.GetSanityNormalized();

        float distortion = Mathf.Lerp(-0.6f, 0f, sanity);

        float noise = Mathf.Sin(Time.time * 3f) * 0.05f;

        lensDistortion.intensity.value = distortion + noise;
    }
}