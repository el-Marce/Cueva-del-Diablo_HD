using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class TriggerInteriorCueva : MonoBehaviour
{
    public Door door;
    //public NPC_Controller companion;
    bool triggered = false;
    [Header("Sonido")]
    public EventReference derrumbePiedras;
    public EventReference cerrarPuertaViolent;
    void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;

            if (door.gameObject.layer == LayerMask.NameToLayer("Piedras"))
            {
                AudioManager.Instance.Play(derrumbePiedras, door.transform.position);
                //Transform childA = door.transform.GetChild(0);
                Transform childB = door.transform.GetChild(1);

                //childA.gameObject.SetActive(false);
                childB.gameObject.SetActive(true);

            }
            else
            {
                AudioManager.Instance.Play(cerrarPuertaViolent, door.transform.position);
                door.CloseDoor();
            }

            door.isLocked = true;
            Debug.Log("Jugador Detectado");
        }
    }
}
