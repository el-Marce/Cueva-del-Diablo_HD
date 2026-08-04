using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    EventInstance musicaActual;

    [System.Serializable]
    public struct TransicionExcepcion
    {
        public string escenaOrigen;
        public string escenaDestino;
    }

    [Header("Excepciones de StopAllSounds")]
    [Tooltip("Solo se preserva el audio si la transición coincide EXACTAMENTE con origen -> destino")]
    public TransicionExcepcion[] transicionesExcluidas =
    {
        new TransicionExcepcion { escenaOrigen = "PantallaPortada", escenaDestino = "MenuPrincipal" }
    };

    private string escenaAnterior = "";

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        SetMasterVolume(AudioSettings.MasterVolume);

        // Registramos la escena activa al inicio (por si el AudioManager nace en la primera escena)
        escenaAnterior = SceneManager.GetActiveScene().name;
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
        bool esExcepcion = false;

        foreach (var transicion in transicionesExcluidas)
        {
            if (escenaAnterior == transicion.escenaOrigen && scene.name == transicion.escenaDestino)
            {
                esExcepcion = true;
                break;
            }
        }

        if (!esExcepcion)
            StopAllSounds();

        // Actualizamos SIEMPRE la referencia, para la próxima transición
        escenaAnterior = scene.name;
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