using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerInteriorCueva : MonoBehaviour
{
    public Door door;
    //public NPC_Controller companion;

    bool triggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;

            if (door.gameObject.layer == LayerMask.NameToLayer("Piedras"))
            {
                //Transform childA = door.transform.GetChild(0);
                Transform childB = door.transform.GetChild(1);

                //childA.gameObject.SetActive(false);
                childB.gameObject.SetActive(true);

            }
            else
            {
                door.CloseDoor();
            }

            door.isLocked = true;
            Debug.Log("Jugador Detectado");
        }
    }
}
