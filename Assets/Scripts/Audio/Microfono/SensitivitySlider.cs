// SensitivitySlider.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SensitivitySlider : MonoBehaviour
{
    public enum SensitivityTarget { Mouse, Microphone }

    [Header("Referencias")]
    public Slider slider;
    public TMP_Text valueLabel;
    public SensitivityTarget target;

    [Header("Rango")]
    public float minValue = 10f;
    public float maxValue = 100f;

    void Start()
    {
        slider.minValue = minValue;
        slider.maxValue = maxValue;

        slider.value = target == SensitivityTarget.Mouse
            ? AudioSettings.MouseSensitivity
            : AudioSettings.MicSensitivity;

        UpdateLabel(slider.value);
        slider.onValueChanged.AddListener(OnSliderChanged);
    }

    void OnSliderChanged(float value)
    {
        if (target == SensitivityTarget.Mouse)
            AudioSettings.MouseSensitivity = value;
        else
            AudioSettings.MicSensitivity = value;

        UpdateLabel(value);
    }

    void UpdateLabel(float value)
    {
        if (valueLabel != null)
            valueLabel.text = Mathf.RoundToInt(value).ToString();
    }

    void OnDestroy()
    {
        slider.onValueChanged.RemoveListener(OnSliderChanged);
    }
}