using FMOD;
using FMOD.Studio;
using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    EventInstance musicaActual;

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
        StopAllSounds();
    }
    public void StopAllSounds()
    {
        FMODUnity.RuntimeManager.StudioSystem.getBus("bus:/", out FMOD.Studio.Bus masterBus);
        masterBus.stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
    public void SetMasterVolume(float volume)
    {
        RuntimeManager.StudioSystem.setParameterByName(
            "Master",
            volume / 100f
        );
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

        musicaActual.stop(
            fadeOut ? FMOD.Studio.STOP_MODE.ALLOWFADEOUT
                    : FMOD.Studio.STOP_MODE.IMMEDIATE);

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
    public EventInstance CreateLoop(EventReference eventRef, Transform follow = null)
    {
        if (eventRef.IsNull) return default;

        EventInstance instance = RuntimeManager.CreateInstance(eventRef);

        if (follow != null)
            instance.set3DAttributes(RuntimeUtils.To3DAttributes(follow));

        instance.start();

        return instance;
    }
    //public EventInstance CreateLoop(string eventPath, Vector3 position = default)
    //{
    //    EventInstance instance = RuntimeManager.CreateInstance(eventPath);
    //    instance.set3DAttributes(RuntimeUtils.To3DAttributes(position));
    //    instance.start();
    //    return instance;
    //}

    public void StopLoop(EventInstance instance, bool fadeOut = true)
    {
        if (!instance.isValid()) return;
        instance.stop(fadeOut
            ? FMOD.Studio.STOP_MODE.ALLOWFADEOUT
            : FMOD.Studio.STOP_MODE.IMMEDIATE);
        instance.release();
    }
}