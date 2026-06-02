using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    EventInstance musicaActual;

    [Header("Excepciones de StopAllSounds")]
    public string[] escenasExcluidas = { "MenuPrincipal" }; // escenas donde NO se detienen los sonidos

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        SetMasterVolume(AudioSettings.MasterVolume);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // No detener sonidos si la escena destino está en la lista de excepciones
        foreach (string excluida in escenasExcluidas)
        {
            if (scene.name == excluida) return;
        }
        StopAllSounds();
    }

    public void StopAllSounds()
    {
        RuntimeManager.StudioSystem.getBus("bus:/", out Bus masterBus);
        masterBus.stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    // --- Volumen ---
    public void SetMasterVolume(float volume)
    {
        RuntimeManager.StudioSystem.getBus("bus:/", out Bus masterBus);
        masterBus.setVolume(volume / 100f);
    }

    // --- Música adaptativa ---
    public void PlayMusica(string eventPath)
    {
        if (musicaActual.isValid())
        {
            musicaActual.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            musicaActual.release();
        }
        musicaActual = RuntimeManager.CreateInstance(eventPath);
        musicaActual.start();
    }

    public void StopMusica(bool fadeOut = true)
    {
        if (!musicaActual.isValid()) return;
        musicaActual.stop(fadeOut ? FMOD.Studio.STOP_MODE.ALLOWFADEOUT : FMOD.Studio.STOP_MODE.IMMEDIATE);
        musicaActual.release();
    }

    public void SetMusicaParametro(string parametro, float valor)
    {
        if (musicaActual.isValid())
            musicaActual.setParameterByName(parametro, valor);
    }

    // --- Sonidos de una sola vez ---
    public void Play(EventReference eventRef, Vector3 position = default)
    {
        if (eventRef.IsNull) return;
        RuntimeManager.PlayOneShot(eventRef, position);
    }

    // --- Loops ---
    public EventInstance CreateLoop(EventReference eventRef, Transform follow = null)
    {
        if (eventRef.IsNull) return default;
        EventInstance instance = RuntimeManager.CreateInstance(eventRef);
        if (follow != null)
            instance.set3DAttributes(RuntimeUtils.To3DAttributes(follow));
        instance.start();
        return instance;
    }

    public void StopLoop(EventInstance instance, bool fadeOut = true)
    {
        if (!instance.isValid()) return;
        instance.stop(fadeOut ? FMOD.Studio.STOP_MODE.ALLOWFADEOUT : FMOD.Studio.STOP_MODE.IMMEDIATE);
        instance.release();
    }
}