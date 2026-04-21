using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MicrophoneHUD : MonoBehaviour
{
    [Header("Barras")]
    public RectTransform[] bars;

    [Header("Escala")]
    public float scaleMin = 0f;
    public float scaleMax = 0.1f;

    [Header("Colores")]
    public Color colorOff = new Color(0.27f, 0.27f, 0.27f, 0.2f);
    public Color colorNormal = new Color(0.11f, 0.62f, 0.46f, 1f);

    MicrophoneInput microphoneInput;
    Image[] barImages;

    void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    void Start()
    {
        InicializarBarras();
        StartCoroutine(BuscarMicrofonoDelayed());
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        microphoneInput = null;
        ResetearBarras();
        StartCoroutine(BuscarMicrofonoDelayed());
    }

    IEnumerator BuscarMicrofonoDelayed()
    {
        yield return null; // espera un frame
        microphoneInput = FindObjectOfType<MicrophoneInput>();

        if (microphoneInput == null)
            Debug.LogWarning("[MicrophoneHUD] MicrophoneInput no encontrado.");
    }

    void InicializarBarras()
    {
        if (bars == null || bars.Length == 0) return;
        barImages = new Image[bars.Length];
        for (int i = 0; i < bars.Length; i++)
            barImages[i] = bars[i].GetComponent<Image>();
        ResetearBarras();
    }

    void ResetearBarras()
    {
        if (barImages == null) return;
        foreach (var img in barImages)
            if (img != null) img.color = colorOff;
    }

    void Update()
    {
        if (microphoneInput == null || barImages == null) return;

        float level = Mathf.InverseLerp(scaleMin, scaleMax, microphoneInput.smoothedLoudness);

        for (int i = 0; i < barImages.Length; i++)
        {
            if (barImages[i] == null) continue;
            float ratio = (float)(i + 1) / barImages.Length;
            barImages[i].color = level >= ratio ? colorNormal : colorOff;
        }
    }
}