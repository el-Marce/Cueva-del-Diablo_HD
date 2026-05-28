// AudioSettings.cs
public static class AudioSettings
{
    private const string KEY_MIC_SENSITIVITY = "mic_sensitivity";
    private const string KEY_MOUSE_SENSITIVITY = "mouse_sensitivity";

    public static float DefaultMicSensitivity = 100f;
    public static float DefaultMouseSensitivity = 100f;

    public static float MicSensitivity
    {
        get => UnityEngine.PlayerPrefs.GetFloat(KEY_MIC_SENSITIVITY, DefaultMicSensitivity);
        set
        {
            UnityEngine.PlayerPrefs.SetFloat(KEY_MIC_SENSITIVITY, value);
            UnityEngine.PlayerPrefs.Save();
        }
    }

    public static float MouseSensitivity
    {
        get => UnityEngine.PlayerPrefs.GetFloat(KEY_MOUSE_SENSITIVITY, DefaultMouseSensitivity);
        set
        {
            UnityEngine.PlayerPrefs.SetFloat(KEY_MOUSE_SENSITIVITY, value);
            UnityEngine.PlayerPrefs.Save();
        }
    }
}