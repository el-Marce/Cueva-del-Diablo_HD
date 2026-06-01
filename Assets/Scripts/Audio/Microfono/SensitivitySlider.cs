// SensitivitySlider.cs
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SensitivitySlider : MonoBehaviour
{
    public enum SensitivityTarget
    {
        Mouse,
        Microphone,
        MasterVolume
    }

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

        switch (target)
        {
            case SensitivityTarget.Mouse:
                slider.value = AudioSettings.MouseSensitivity;
                break;

            case SensitivityTarget.Microphone:
                slider.value = AudioSettings.MicSensitivity;
                break;

            case SensitivityTarget.MasterVolume:
                slider.value = AudioSettings.MasterVolume;
                break;
        }

        UpdateLabel(slider.value);
        slider.onValueChanged.AddListener(OnSliderChanged);
    }

    void OnSliderChanged(float value)
    {
        switch (target)
        {
            case SensitivityTarget.Mouse:
                AudioSettings.MouseSensitivity = value;
                break;

            case SensitivityTarget.Microphone:
                AudioSettings.MicSensitivity = value;
                break;

            case SensitivityTarget.MasterVolume:
                AudioSettings.MasterVolume = value;
                AudioManager.Instance.SetMasterVolume(AudioSettings.MasterVolume);
                break;
        }

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