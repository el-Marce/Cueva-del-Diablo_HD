using UnityEngine;

public class MicrophoneInput : MonoBehaviour
{
    public string selectedDevice;
    private AudioClip micClip;
    private const int sampleWindow = 128;
    private const int clipDuration = 10;

    public float sensitivity = 100f;

    [Header("Noise Gate + Compresión")]
    [Tooltip("Nivel de ruido de fondo a ignorar. Ajusta hasta que el HUD no se mueva en silencio")]
    public float noiseFloor = 0.01f;

    [Tooltip("Cuánto se amplifica la señal que supera el noise floor. Más alto = más sensible a sonidos relevantes")]
    public float gainAboveFloor = 3f;

    [Tooltip("Límite máximo de la señal procesada antes de aplicar sensitivity")]
    public float signalCap = 1f;

    [Header("Suavizado")]
    public float smoothUpSpeed = 25f;
    public float smoothDownSpeed = 5f;

    public float smoothedLoudness = 0f;
    public float peakLoudness = 0f;

    [Header("Respiración")]
    public float minThreshold = 0.02f;
    public float maxThreshold = 0.05f;
    public float requiredDuration = 0.25f;
    float breathTimer = 0f;

    [Header("Ritmo")]
    public float rhythmThreshold = 0.75f;
    public AltarCondition_RhythmChallenge rhythmCondition;
    float lastPulseTime = -999f;
    public float pulseCooldown = 0.3f;

    bool micActive = true;
    BreathingSystem breathingSystem;
    NoiseEmitter noiseEmitter;

    void Start()
    {
        breathingSystem = GetComponent<BreathingSystem>();
        noiseEmitter = GetComponent<NoiseEmitter>();
        sensitivity = AudioSettings.MicSensitivity;
        IniciarMic();
    }

    void IniciarMic()
    {
        if (Microphone.devices.Length == 0)
        {
            Debug.LogError("[Mic] No se detectó micrófono");
            return;
        }

        selectedDevice = Microphone.devices[0];
        micClip = Microphone.Start(selectedDevice, true, clipDuration, 44100);
        micActive = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
            ToggleMic();

        if (!micActive) return;

        float rawLoudness = GetLoudnessFromMicrophone();
        float processedLoudness = ProcessSignal(rawLoudness) * sensitivity;

        peakLoudness = processedLoudness;

        float smoothSpeed = processedLoudness > smoothedLoudness ? smoothUpSpeed : smoothDownSpeed;
        smoothedLoudness = Mathf.Lerp(smoothedLoudness, processedLoudness, Time.deltaTime * smoothSpeed);

        if (peakLoudness > maxThreshold)
            noiseEmitter.EmitNoise(peakLoudness);

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

        bool cooldownReady = (Time.time - lastPulseTime) >= pulseCooldown;
        if (peakLoudness > rhythmThreshold && cooldownReady)
        {
            lastPulseTime = Time.time;
            rhythmCondition?.RegisterPulse();
        }

        if (Input.GetKeyDown(KeyCode.I))
            rhythmCondition?.RegisterPulse();
    }

    // Noise gate + ganancia: ignora el ruido de fondo y amplifica lo relevante
    float ProcessSignal(float raw)
    {
        if (raw <= noiseFloor)
            return 0f; // ruido de fondo: ignorar completamente

        // Cuánto supera el noise floor
        float signalAboveFloor = raw - noiseFloor;

        // Amplificar esa diferencia y limitar al cap
        float amplified = signalAboveFloor * gainAboveFloor;
        return Mathf.Min(amplified, signalCap);
    }

    float GetLoudnessFromMicrophone()
    {
        if (micClip == null) return 0f;

        int micPosition = Microphone.GetPosition(selectedDevice);
        if (micPosition < sampleWindow) return 0f;

        int startPosition = micPosition - sampleWindow;
        float[] waveData = new float[sampleWindow];
        micClip.GetData(waveData, startPosition);

        float peak = 0f;
        for (int i = 0; i < sampleWindow; i++)
        {
            float sample = Mathf.Abs(waveData[i]);
            if (sample > peak) peak = sample;
        }

        return peak;
    }

    public void ToggleMic()
    {
        micActive = !micActive;

        if (micActive)
            IniciarMic();
        else
        {
            Microphone.End(selectedDevice);
            smoothedLoudness = 0f;
            peakLoudness = 0f;
            Debug.Log("[Mic] Desactivado");
        }
    }
}