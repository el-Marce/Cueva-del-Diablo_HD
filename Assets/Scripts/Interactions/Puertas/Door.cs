using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour, IInteractable
{
    public string requiredKey;
    public bool isLocked = false;
    bool isOpen = false;

    [Header("Animación")]
    public float openDuration = 1.5f;
    public float closeDuration = 0.5f;

    bool isMoving = false;
    Quaternion closedRotation;
    Quaternion openRotation;

    void Start()
    {
        closedRotation = transform.rotation;
        openRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, -90, 0));
    }

    public void Interact()
    {
        if (isLocked)
        {
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
            Debug.Log("Puerta cerrada. Necesitas: " + requiredKey);
        }
    }

    public void OpenDoor()
    {
        if (isOpen || isMoving) return;
        StartCoroutine(RotateDoor(openRotation, openDuration));
        isOpen = true;
    }

    public void CloseDoor()
    {
        if (!isOpen || isMoving) return;
        StartCoroutine(RotateDoor(closedRotation, closeDuration));
        isOpen = false;
    }

    IEnumerator RotateDoor(Quaternion targetRotation, float duration)
    {
        isMoving = true;

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

        isMoving = false;
    }

    public bool IsOpen()
    {
        return isOpen;
    }
}