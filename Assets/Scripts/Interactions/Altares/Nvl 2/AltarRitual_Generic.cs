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

    [Header("Liberación")]
    public GameObject npcPrefab;
    public GameObject entePrefab;
    public float liberationDelay = 0f;
    public LayerMask pueblerinoLayer;

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

        KillNearbyEntes();
        LiberateNearbyPueblerinos();

        GetComponent<Collider>().enabled = false;
        Debug.Log("[Altar] Ritual completado.");
    }
    void KillNearbyEntes()
    {
        // Reutiliza el radio de la condición de entes si existe
        AltarCondition_Entes enteCondition = GetComponent<AltarCondition_Entes>();
        if (enteCondition == null) return;

        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            enteCondition.detectionRadius,
            enteCondition.enteLayer
        );

        foreach (Collider hit in hits)
        {
            EntePsicologico ente = hit.GetComponent<EntePsicologico>();
            if (ente != null)
            {
                ente.Die();
            }
        }
    }
    void LiberateNearbyPueblerinos()
    {
        AltarCondition_Entes enteCondition = GetComponent<AltarCondition_Entes>();
        if (enteCondition == null) return;

        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            enteCondition.detectionRadius,
            pueblerinoLayer  //usa su propio layer
        );

        int index = 0;
        foreach (Collider hit in hits)
        {
            Pueblerino pueblerino = hit.GetComponent<Pueblerino>();
            if (pueblerino != null)
            {
                StartCoroutine(LiberateRoutine(pueblerino));
                index++;
            }
        }
    }

    IEnumerator LiberateRoutine(Pueblerino pueblerino)
    {
        yield return new WaitForSeconds(liberationDelay);

        if (pueblerino == null) yield break; // ya fue destruido

        Vector3 spawnPos = pueblerino.transform.position;
        Quaternion spawnRot = pueblerino.transform.rotation;

        if (entePrefab != null)
        {
            if (entePrefab!= null)
                Instantiate(entePrefab, spawnPos, spawnRot);
        }

        Destroy(pueblerino.gameObject);

        if (npcPrefab != null)
            Instantiate(npcPrefab, spawnPos, spawnRot);
    }
}