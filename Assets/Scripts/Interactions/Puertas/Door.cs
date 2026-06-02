using System.Collections;
using FMODUnity;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class Door : MonoBehaviour, IInteractable
{
    public string requiredKey;
    public bool isLocked = false;
    public bool isOpen = false;

    [Header("Animación")]
    public float openDuration = 1.5f;
    public float closeDuration = 0.5f;
    Animator lockAnimator;

    [Header("Sonido")]
    public EventReference puertaVieja;
    public EventReference puertaCerrada;

    //public bool canInteract = true;
    bool isMoving = false;
    Quaternion closedRotation;
    Quaternion openRotation;

    public bool debeDesactivarCollider = true;

    void Start()
    {
        lockAnimator = GetComponentInChildren<Animator>();

        if (!isOpen)
        {
            closedRotation = transform.rotation;
            openRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, -90, 0));
        }
        else
        {
            closedRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, +90, 0));
            openRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, -90, 0));
        }
    }

    public void Interact()
    {
        if (isLocked)
        {
            AudioManager.Instance.Play(puertaCerrada);
            Debug.Log("La puerta está bloqueada.");
            return;
        }

        if (string.IsNullOrEmpty(requiredKey))
        {
            OpenDoor();
            return;
        }

        Inventory inventory = FindObjectOfType<Inventory>();

        if (inventory.HasItem(requiredKey))
        {
            OpenDoor();
        }
        else
        {
            AudioManager.Instance.Play(puertaCerrada);
            Debug.Log("Puerta cerrada. Necesitas: " + requiredKey);
        }
    }

    public void OpenDoor()
    {
        if (isOpen || isMoving) return;

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        if (transform.childCount > 1 && debeDesactivarCollider)
        {
            Collider childCol = transform.GetChild(1).GetComponent<Collider>();
            if (childCol != null) childCol.enabled = true;
        }

        StartCoroutine(OpenSequence());
    }

    IEnumerator OpenSequence()
    {
        isMoving = true;

        if(lockAnimator != null)
        {
            lockAnimator.SetTrigger("OpenLock");
            yield return new WaitForSeconds(3f);
        }

        AudioManager.Instance.Play(puertaVieja);
        yield return StartCoroutine(RotateDoor(openRotation, openDuration));

        isOpen = true;

        isMoving = false;
    }
    public void CloseDoor()
    {
        if (!isOpen || isMoving) return;
        StartCoroutine(RotateDoor(closedRotation, closeDuration));
        isOpen = false;
    }

    IEnumerator RotateDoor(Quaternion targetRotation, float duration)
    {
        //isMoving = true;

        Quaternion startRotation = transform.rotation;
        float time = 0f;

        while (time < duration)
        {
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, time / duration);
            //transform.rotation = Quaternion.Lerp(startRotation, targetRotation, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        transform.rotation = targetRotation;

        //isMoving = false;
    }

    public bool IsOpen()
    {
        return isOpen;
    }
}