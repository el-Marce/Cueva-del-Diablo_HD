using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AltarRitual_Generic : MonoBehaviour, IInteractable
{
    [Header("Cambio de escena (opcional)")]
    public string escenaDestino;

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

    [HideInInspector] public List<AltarCondition> conditions = new List<AltarCondition>();

    [Header("Teletransporte")]
    public Transform teleportTarget;

    [Header("Emisión del Altar")]
    [SerializeField] private Renderer[] emissiveRenderers; // si lo dejas vacío, se busca automáticamente
    [SerializeField] private string emissionProperty = "_EmissiveIntensity"; // verifica en modo Debug del material
    [SerializeField] private float emissionFadeDuration = 1.5f;

    bool activated = false;

    void Awake()
    {
        conditions.AddRange(GetComponents<AltarCondition>());

        if (emissiveRenderers == null || emissiveRenderers.Length == 0)
            emissiveRenderers = GetComponentsInChildren<Renderer>();
    }

    Transform playerTransform;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void TeleportPlayer()
    {
        if (teleportTarget == null) return;

        if (playerTransform == null)
            playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (playerTransform == null)
        {
            Debug.LogError("[Altar] No se encontró al jugador para teleportar");
            return;
        }

        CharacterController cc = playerTransform.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        playerTransform.position = teleportTarget.position;
        playerTransform.rotation = teleportTarget.rotation;

        if (cc != null) cc.enabled = true;
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

        GetComponent<Collider>().enabled = false;
        Transform child = transform.GetChild(0);
        child.gameObject.GetComponent<Collider>().enabled = true;

        foreach (var c in conditions)
            c.OnFulfill();

        altarUI.Close();

        FreezeNearbyEnemies();

        StartCoroutine(FadeEmissionToZero()); // <- apaga la emisión permanentemente

        yield return new WaitForSeconds(activationDelay);

        if (door != null)
        {
            door.isLocked = false;

            if (door.gameObject.layer == LayerMask.NameToLayer("Piedras"))
            {
                Transform childA = door.transform.GetChild(0);
                Transform childB = door.transform.GetChild(1);

                childA.gameObject.SetActive(false);
                childB.gameObject.SetActive(true);

                StartCoroutine(FreezeChildrenRigidbodies(childB));
            }
            else
            {
                door.OpenDoor();
            }
        }

        KillNearbyEntes();
        LiberateNearbyPueblerinos();

        IEnumerator FreezeChildrenRigidbodies(Transform parent)
        {
            yield return new WaitForSeconds(10f);

            Rigidbody[] rbs = parent.GetComponentsInChildren<Rigidbody>();

            foreach (Rigidbody rb in rbs)
                rb.drag = 5f;

            yield return new WaitForSeconds(1f);

            foreach (Rigidbody rb in rbs)
                rb.isKinematic = true;
        }

        if (!string.IsNullOrEmpty(escenaDestino))
        {
            yield return new WaitForSeconds(1f);
            UnityEngine.SceneManagement.SceneManager.LoadScene(escenaDestino);
        }
        else if (teleportTarget != null)
        {
            TeleportPlayer();
        }
    }

    IEnumerator FadeEmissionToZero()
    {
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        int colorPropId = Shader.PropertyToID("_EmissiveColor");

        Color[] startColors = new Color[emissiveRenderers.Length];
        for (int i = 0; i < emissiveRenderers.Length; i++)
        {
            if (emissiveRenderers[i] == null) continue;
            startColors[i] = emissiveRenderers[i].sharedMaterial.GetColor(colorPropId);
        }

        float t = 0f;
        while (t < emissionFadeDuration)
        {
            t += Time.deltaTime;
            float lerpT = t / emissionFadeDuration;

            for (int i = 0; i < emissiveRenderers.Length; i++)
            {
                if (emissiveRenderers[i] == null) continue;
                Color value = Color.Lerp(startColors[i], Color.black, lerpT);

                emissiveRenderers[i].GetPropertyBlock(mpb);
                mpb.SetColor(colorPropId, value);
                emissiveRenderers[i].SetPropertyBlock(mpb);
            }

            yield return null;
        }

        foreach (var r in emissiveRenderers)
        {
            if (r == null) continue;
            r.GetPropertyBlock(mpb);
            mpb.SetColor(colorPropId, Color.black);
            r.SetPropertyBlock(mpb);
        }
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
            Instantiate(entePrefab, spawnPos, spawnRot);

        Destroy(pueblerino.gameObject);

        if (npcPrefab != null)
        {
            GameObject npc = Instantiate(npcPrefab, spawnPos, spawnRot);
            npc.transform.localScale = new Vector3(1.3f, 1.88f, 1.15f);
        }
    }
}