using UnityEngine;
using System.Collections;
using FMODUnity;
using FMOD.Studio;

public class NPC_Liberado : MonoBehaviour, IInteractable
{
    [Header("Sanidad")]
    public SanitySystem playerSanity;
    public float sanityRestore = 7f;

    [Header("Audio FMOD")]
    public EventReference thankEvent;

    [Header("Timing")]
    public float disappearDelay = 2f;

    bool interacted = false;

    void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
            playerSanity = player.GetComponentInChildren<SanitySystem>();
    }

    public void Interact()
    {
        if (interacted) return;
        interacted = true;

        DisableInteraction();

        StartCoroutine(ThanksSequence());
    }

    IEnumerator ThanksSequence()
    {
        if (playerSanity != null)
            playerSanity.RestoreSanity(sanityRestore);

        RuntimeManager.PlayOneShot(thankEvent, transform.position); 

        yield return new WaitForSeconds(disappearDelay);

        Destroy(gameObject);
    }

    void DisableInteraction()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;
    }
}