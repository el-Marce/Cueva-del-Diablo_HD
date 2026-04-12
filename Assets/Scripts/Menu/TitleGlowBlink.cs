using UnityEngine;
using TMPro;
using System.Collections;

public class TitleGlowBlink : MonoBehaviour
{
    [Header("Referencias")]
    public TMP_Text titleText;

    [Header("Glow")]
    public float glowMin = 0f;
    public float glowMax = 0.2f;
    public float blinkDuration = 0.8f;

    void Start()
    {
        titleText.fontMaterial = new Material(titleText.fontMaterial);
        StartCoroutine(GlowBlink());
    }

    IEnumerator GlowBlink()
    {
        while (true)
        {
            yield return StartCoroutine(FadeGlow(glowMin, glowMax));
            yield return StartCoroutine(FadeGlow(glowMax, glowMin));
        }
    }

    IEnumerator FadeGlow(float from, float to)
    {
        float elapsed = 0f;
        while (elapsed < blinkDuration)
        {
            elapsed += Time.deltaTime;
            float value = Mathf.Lerp(from, to, elapsed / blinkDuration);
            titleText.fontMaterial.SetFloat(ShaderUtilities.ID_GlowPower, value);
            yield return null;
        }
        titleText.fontMaterial.SetFloat(ShaderUtilities.ID_GlowPower, to);
    }
}