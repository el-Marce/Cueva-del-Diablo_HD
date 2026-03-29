using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerInteriorCueva : MonoBehaviour
{
    public Door door;
    public NPC_Controller companion;

    bool triggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;

            door.CloseDoor();
            door.isLocked = true;

            companion.Possess();

            Debug.Log("Puerta sellada.");
        }
    }
}
