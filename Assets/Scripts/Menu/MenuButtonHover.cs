using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class MenuButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Texto")]
    public List<TMP_Text> tmpTexts;
    public float outlineThickness = 0.25f;

    [Header("Imagen (opcional)")]
    public Image hoverImage;
    public float imageFadeDuration = 0.15f;

    [Header("Fade color (opcional — para botones sin texto)")]
    public Image fadeImage;
    public float fadeTargetAlpha = 1f;      // alpha en hover
    float fadeOriginalAlpha;

    Coroutine imageFadeCoroutine;

    void Start()
    {
        if (tmpTexts == null || tmpTexts.Count == 0)
            tmpTexts = new List<TMP_Text>(GetComponentsInChildren<TMP_Text>());

        if (hoverImage != null)
        {
            Color c = hoverImage.color;
            c.a = 0f;
            hoverImage.color = c;
        }

        // Guarda el alpha original del fadeImage
        if (fadeImage != null)
            fadeOriginalAlpha = fadeImage.color.a;
    }

    void SetOutline(float value)
    {
        foreach (var tmp in tmpTexts)
            if (tmp != null)
                tmp.fontMaterial.SetFloat(ShaderUtilities.ID_OutlineWidth, value);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetOutline(outlineThickness);

        if (hoverImage != null)
        {
            if (imageFadeCoroutine != null) StopCoroutine(imageFadeCoroutine);
            imageFadeCoroutine = StartCoroutine(FadeImage(hoverImage, 1f));
        }

        if (fadeImage != null)
        {
            if (imageFadeCoroutine != null) StopCoroutine(imageFadeCoroutine);
            imageFadeCoroutine = StartCoroutine(FadeImage(fadeImage, fadeTargetAlpha));
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetOutline(0f);

        if (hoverImage != null)
        {
            if (imageFadeCoroutine != null) StopCoroutine(imageFadeCoroutine);
            imageFadeCoroutine = StartCoroutine(FadeImage(hoverImage, 0f));
        }

        if (fadeImage != null)
        {
            if (imageFadeCoroutine != null) StopCoroutine(imageFadeCoroutine);
            imageFadeCoroutine = StartCoroutine(FadeImage(fadeImage, fadeOriginalAlpha));
        }
    }

    IEnumerator FadeImage(Image img, float targetAlpha)
    {
        float elapsed = 0f;
        float startAlpha = img.color.a;

        while (elapsed < imageFadeDuration)
        {
            elapsed += Time.deltaTime;
            Color c = img.color;
            c.a = Mathf.Lerp(startAlpha, targetAlpha, elapsed / imageFadeDuration);
            img.color = c;
            yield return null;
        }

        Color final = img.color;
        final.a = targetAlpha;
        img.color = final;
    }

    public IEnumerator BlinkOutline(int times, float speed)
    {
        for (int i = 0; i < times; i++)
        {
            SetOutline(outlineThickness);
            yield return new WaitForSeconds(speed);
            SetOutline(0f);
            yield return new WaitForSeconds(speed);
        }
    }
}