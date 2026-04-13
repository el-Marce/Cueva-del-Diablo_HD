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

    [Header("Radio de efecto")]
    public float effectRadius = 10f;
    public LayerMask enteLayer;
    //AltarCondition_Entes enteCondition;

    [HideInInspector] public List<AltarCondition> conditions = new List<AltarCondition>();

    bool activated = false;

    void Awake()
    {
        // Recoge automáticamente todas las condiciones en este GameObject
        conditions.AddRange(GetComponents<AltarCondition>());
    }
    void Start()
    {
        //enteCondition = GetComponent<AltarCondition_Entes>();
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
        //Debug.Log("[Altar] TryActivate - activated: " + activated + " | AllMet: " + AllConditionsMet());
        if (activated || !AllConditionsMet()) return;
        StartCoroutine(ActivationSequence());
    }

    IEnumerator ActivationSequence()
    {
        activated = true;
        //var enteCondition = GetComponent<AltarCondition_Entes>();

        foreach (var c in conditions)
            c.OnFulfill();

        altarUI.Close();

        FreezeNearbyEnemies();

        // Aquí tu cinemática / sonido
        yield return new WaitForSeconds(activationDelay);

        door.isLocked = false;
        door.OpenDoor();

        KillNearbyEntes();
        LiberateNearbyPueblerinos();

        GetComponent<Collider>().enabled = false;
        //Debug.Log("[Altar] Ritual completado.");
    }
    void KillNearbyEntes()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, effectRadius, enteLayer);
        foreach (Collider hit in hits)
        {
            EntePsicologico ente = hit.GetComponent<EntePsicologico>();
            if (ente != null) ente.Die();
        }
    }

    void LiberateNearbyPueblerinos()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, effectRadius, pueblerinoLayer);
        int index = 0;
        foreach (Collider hit in hits)
        {
            Pueblerino p = hit.GetComponent<Pueblerino>() ?? hit.GetComponentInParent<Pueblerino>();
            if (p != null)
            {
                StartCoroutine(LiberateRoutine(p));
                index++;
            }
        }
    }

    void FreezeNearbyEnemies()
    {
        Collider[] entes = Physics.OverlapSphere(transform.position, effectRadius, enteLayer);
        foreach (Collider hit in entes)
        {
            EntePsicologico ente = hit.GetComponent<EntePsicologico>();
            if (ente != null) ente.Freeze();
        }

        Collider[] pueblerinos = Physics.OverlapSphere(transform.position, effectRadius, pueblerinoLayer);
        foreach (Collider hit in pueblerinos)
        {
            Pueblerino p = hit.GetComponent<Pueblerino>();
            if (p != null) p.Freeze();
        }
    }

    IEnumerator LiberateRoutine(Pueblerino pueblerino)
    {
        yield return new WaitForSeconds(liberationDelay);

        if (pueblerino == null) yield break;

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