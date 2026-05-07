using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class AmbienteAudio : MonoBehaviour
{
    [Header("Música")]
    public EventReference vientoAmbiente;
    EventInstance vientoInstance;
    public EventReference musicaNivel01;
    public EventReference musicaNivel02;

    EventInstance musica;
    EventInstance ambiente;

    void Start()
    {
        vientoInstance = AudioManager.Instance.CreateLoop(vientoAmbiente);
    }

    void OnDestroy()
    {
        AudioManager.Instance.StopLoop(vientoInstance);
    }
}
