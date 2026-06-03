using UnityEngine;

public class SpawnRestrictedZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            PlayerSpawnUpdater.zonaRestringida = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            PlayerSpawnUpdater.zonaRestringida = false;
    }
}