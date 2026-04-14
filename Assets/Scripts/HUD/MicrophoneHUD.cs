using UnityEngine;
using UnityEngine.UI;

public class MicrophoneHUD : MonoBehaviour
{
    [Header("Referencias")]
    public MicrophoneInput microphoneInput;

    [Header("Barras")]
    public RectTransform[] bars;

    //[Header("Líneas de umbral")]
    //public RectTransform lineMin;
    //public RectTransform lineBreathe;
    //public RectTransform lineRhythm;
    //public RectTransform lineMax;

    [Header("Escala")]
    public float scaleMin = 0f;
    public float scaleMax = 0.1f;

    //[Header("Colores")]
    public Color colorOff = new Color(0.27f, 0.27f, 0.27f, 0.2f);
    public Color colorNormal = new Color(0.11f, 0.62f, 0.46f, 1f);
    //public Color colorBreath = new Color(0.94f, 0.62f, 0.15f, 1f);
    //public Color colorRhythm = new Color(0.50f, 0.47f, 0.87f, 1f);
    //public Color colorNoise = new Color(0.89f, 0.29f, 0.29f, 1f);

    float totalWidth;
    Image[] barImages;

    void Start()
    {
        // Busca automáticamente en la escena
        if (microphoneInput == null)
            microphoneInput = FindObjectOfType<MicrophoneInput>();

        if (microphoneInput == null)
        {
            Debug.LogWarning("[MicrophoneHUD] No se encontró MicrophoneInput en la escena — HUD desactivado.");
            gameObject.SetActive(false);
            return;
        }

        barImages = new Image[bars.Length];
        for (int i = 0; i < bars.Length; i++)
            barImages[i] = bars[i].GetComponent<Image>();

        for (int i = 0; i < bars.Length; i++)
        {
            float t = (float)i / (bars.Length - 1);
            float h = Mathf.Lerp(8f, 56f, Mathf.Pow(t, 0.65f));
            //bars[i].sizeDelta = new Vector2(bars[i].sizeDelta.x, h);
        }

        RectTransform container = bars[0].parent.GetComponent<RectTransform>();
        totalWidth = container.rect.width;

       // PositionThresholdLines();
    }

    //void PositionThresholdLines()
    //{
    //    float maxVal = microphoneInput.maxThreshold * 1.3f; // escala visual

    //    SetLineX(lineMin, microphoneInput.minThreshold / maxVal);
    //    SetLineX(lineBreathe, microphoneInput.maxThreshold / maxVal * 0.5f);
    //    SetLineX(lineRhythm, microphoneInput.rhythmThreshold / maxVal);
    //    SetLineX(lineMax, microphoneInput.maxThreshold / maxVal);
    //}

    void SetLineX(RectTransform line, float normalizedX)
    {
        if (line == null) return;
        Vector2 pos = line.anchoredPosition;
        pos.x = normalizedX * totalWidth;
        line.anchoredPosition = pos;
    }

    void Update()
    {
        float level = Mathf.InverseLerp(scaleMin, scaleMax, microphoneInput.smoothedLoudness);

        for (int i = 0; i < bars.Length; i++)
        {
            float ratio = (float)(i + 1) / bars.Length;
            bool active = level >= ratio;
            barImages[i].color = active ? GetBarColor(ratio, scaleMax) : colorOff;
        }
    }

    Color GetBarColor(float ratio, float maxVal)
    {
    //    float min = microphoneInput.minThreshold / maxVal;
    //    float breath = microphoneInput.maxThreshold / maxVal * 0.5f;
    //    float rhythm = microphoneInput.rhythmThreshold / maxVal;
    //    float max = microphoneInput.maxThreshold / maxVal;

    //    if (ratio > max) return colorNoise;
    //    if (ratio > rhythm) return colorRhythm;
    //    if (ratio > breath) return colorBreath;
    //    if (ratio > min) return colorNormal;
        return colorNormal;
    }
}