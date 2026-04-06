using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AltarRitual_Generic : MonoBehaviour, IInteractable
{
    [Header("Puerta")]
    public Door door;

    [Header("UI")]
    public AltarUI_Generic altarUI;

    [Header("Timing")]
    public float activationDelay = 2f;

    [HideInInspector] public List<AltarCondition> conditions = new List<AltarCondition>();

    bool activated = false;

    void Awake()
    {
        // Recoge automáticamente todas las condiciones en este GameObject
        conditions.AddRange(GetComponents<AltarCondition>());
    }

    public void Interact()
    {
        if (activated) return;
        altarUI.Open(this);
    }

    public bool AllConditionsMet()
    {
        foreach (var c in conditions)
            if (!c.IsMet()) return false;
        return true;
    }

    public void TryActivate()
    {
        if (activated || !AllConditionsMet()) return;
        StartCoroutine(ActivationSequence());
    }

    IEnumerator ActivationSequence()
    {
        activated = true;

        foreach (var c in conditions)
            c.OnFulfill();

        altarUI.Close();

        // Aquí tu cinemática / sonido
        yield return new WaitForSeconds(activationDelay);

        door.isLocked = false;
        door.OpenDoor();

        GetComponent<Collider>().enabled = false;
        Debug.Log("[Altar] Ritual completado.");
    }
}