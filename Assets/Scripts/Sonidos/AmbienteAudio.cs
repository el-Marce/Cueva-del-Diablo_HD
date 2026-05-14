using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;
public class AmbienteAudio : MonoBehaviour
{
    [Header("Música")]
    public EventReference vientoAmbiente;
    EventInstance vientoInstance;

    public EventReference cuevaAmbiente;
    EventInstance cuevaInstance; 

    public EventReference musicaNivel01;
    public EventReference musicaNivel02;

    EventInstance musica;
    EventInstance ambiente;

    void Start()
    {
        string nombreEscena = SceneManager.GetActiveScene().name;

        if(nombreEscena == "Nivel_01")
        {
            vientoInstance = AudioManager.Instance.CreateLoop(vientoAmbiente);
        } else
            if(nombreEscena == "Nivel_02")
            {
                vientoInstance = AudioManager.Instance.CreateLoop(cuevaAmbiente);

            }
    }

    void OnDestroy()
    {
        AudioManager.Instance.StopLoop(vientoInstance);
    }
}
