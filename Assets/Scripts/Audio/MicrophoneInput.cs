using UnityEngine;

public class MicrophoneInput : MonoBehaviour
{
    public string selectedDevice;
    private AudioClip micClip;
    private const int sampleWindow = 128;

    public float sensitivity = 100f;
    public float minThreshold = 0.02f;
    public float maxThreshold = 0.05f;

    public float requiredDuration = 0.25f;

    float breathTimer = 0f;
    float smoothedLoudness = 0f;

    float previousLoudness = 0f;
    float loudnessChange = 0f;

    private BreathingSystem breathingSystem;

    void Start()
    {
        breathingSystem = GetComponent<BreathingSystem>();

        if (Microphone.devices.Length > 0)
        {
            selectedDevice = Microphone.devices[0];
            micClip = Microphone.Start(selectedDevice, true, 1, 44100);
        }
        else
        {
            Debug.LogError("No se detectó micrófono");
        }
    }

    void Update()
    {
        float loudness = GetLoudnessFromMicrophone() * sensitivity;

        // suavizado de señal
        smoothedLoudness = Mathf.Lerp(smoothedLoudness, loudness, Time.deltaTime * 10);

        loudnessChange = smoothedLoudness - previousLoudness;
        previousLoudness = smoothedLoudness;

        //loudnessChange > 0.001;
        if (smoothedLoudness > minThreshold && smoothedLoudness < maxThreshold)
        {
            breathTimer += Time.deltaTime;

            if (breathTimer >= requiredDuration)
            {
                breathingSystem.Breathe();
                breathTimer = 0f;
            }
        }
        else
        {
            breathTimer = 0f;
        }
    }

    void OnGUI()
    {
        GUI.Label(new Rect(20, 20, 300, 20), "Loudness: " + smoothedLoudness);
        //GUI.Label(new Rect(20, 40, 300, 20), "Min Threshold: " + minThreshold);
        //GUI.Label(new Rect(20, 60, 300, 20), "Max Threshold: " + maxThreshold);
        //GUI.Label(new Rect(20, 80, 300, 20), "Breath Timer: " + breathTimer);
        //GUI.Label(new Rect(20, 100, 300, 20), "Change: " + loudnessChange);

        GUI.Box(new Rect(20, 40, smoothedLoudness * 300, 20), "");
    }

    float GetLoudnessFromMicrophone()
    {
        int micPosition = Microphone.GetPosition(selectedDevice) - sampleWindow + 1;
        if (micPosition < 0)
            return 0;

        float[] waveData = new float[sampleWindow];
        micClip.GetData(waveData, micPosition);

        float total = 0;
        for (int i = 0; i < sampleWindow; i++)
        {
            total += Mathf.Abs(waveData[i]);
        }

        return total / sampleWindow;
    }
    public void PrintDebugs()
    {
        Debug.Log(
    " | Smooth: " + smoothedLoudness +
    " | BreathTimer: " + breathTimer +
    " | Change: " + loudnessChange);
    }
}