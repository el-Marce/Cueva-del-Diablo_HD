using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    public string requiredKey;
    public bool isLocked = false;
    bool isOpen = false;

    public void Interact()
    {
        if (isLocked)
        {
            Debug.Log("La puerta está bloqueada.");
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
        if (isOpen) return;

        transform.Rotate(0, -90, 0);
        isOpen = true;
    }

    public void CloseDoor()
    {
        if (!isOpen) return;

        transform.Rotate(0, 90, 0);
        isOpen = false;
    }

    public bool IsOpen()
    {
        return isOpen;
    }
}