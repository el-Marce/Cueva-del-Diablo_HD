using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    public string requiredKey;

    bool isOpen = false;

    Transform playerCamera;

    void Start()
    {
        playerCamera = Camera.main.transform;
    }

    public void Interact()
    {
        Inventory inventory = FindObjectOfType<Inventory>();

        if (inventory.HasItem(requiredKey))
        {
            OpenDoor();
        }
        else
        {
            Debug.Log("Necesitas la llave");
        }
    }

    void OpenDoor()
    {
        if (isOpen) return;

        transform.Rotate(0, -90, 0);
        isOpen = true;
    }

    void Update()
    {
        if (!isOpen) return;

        Vector3 dir = transform.position - playerCamera.position;
        float dot = Vector3.Dot(playerCamera.forward, dir.normalized);

        if (dot < 0)
        {
            CloseDoor();
        }
    }

    void CloseDoor()
    {
        transform.Rotate(0, 90, 0);
        isOpen = false;
    }
}