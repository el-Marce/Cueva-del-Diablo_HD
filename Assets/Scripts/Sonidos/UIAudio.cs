using UnityEngine;
using FMODUnity;

public class UIAudio : MonoBehaviour
{
    public static UIAudio Instance;

    [Header("Menú Principal")]
    public EventReference hover;
    public EventReference click;
    public EventReference back;
    public EventReference borrar;
    public EventReference newGame;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlayHover() => AudioManager.Instance.Play(hover);
    public void PlayClick() => AudioManager.Instance.Play(click);
    public void PlayBack() => AudioManager.Instance.Play(back);
    public void PlayNewGame() {AudioManager.Instance.Play(newGame); AudioManager.Instance.StopMusica();} 
    public void PlayBorrar() => AudioManager.Instance.Play(borrar);
}