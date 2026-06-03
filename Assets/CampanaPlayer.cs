using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class CampanaPlayer : MonoBehaviour
{
    public EventReference campana;

    EventInstance campanaInstance;
    bool sonando = false;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (sonando) return;

        campanaInstance = AudioManager.Instance.CreateLoop(campana);
        campanaInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position));
        campanaInstance.start();
        sonando = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (!sonando) return;

        AudioManager.Instance.StopLoop(campanaInstance);
        sonando = false;
    }
}