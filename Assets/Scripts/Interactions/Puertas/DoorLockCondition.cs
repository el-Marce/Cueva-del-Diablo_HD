using UnityEngine;

public class DoorLockCondition : MonoBehaviour
{
    Door door;
    Inventory inventory;

    [Header("Condición")]
    public string requiredItemToUnlock;

    bool wasClosedOnce = false;

    void Start()
    {
        door = GetComponent<Door>();
        inventory = FindObjectOfType<Inventory>();
    }

    void Update()
    {
        if (!door.IsOpen() && !wasClosedOnce)
        {
            door.isLocked = true;
            wasClosedOnce = true;
        }

        if (door.isLocked && inventory.HasItem(requiredItemToUnlock))
        {
            door.isLocked = false;
            Debug.Log("Puerta desbloqueada");
        }
    }
}