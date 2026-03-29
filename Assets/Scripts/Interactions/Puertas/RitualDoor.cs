using UnityEngine;

public class RitualDoor : MonoBehaviour
{
    public Door door;
    public Inventory inventory;

    public string item1 = "coca";
    public string item2 = "alcohol";
    public string item3 = "sullu";

    bool opened = false;

    void Update()
    {
        if (opened) return;

        if (inventory.HasItem(item1) &&
            inventory.HasItem(item2) &&
            inventory.HasItem(item3))
        {
            OpenDoorAutomatically();
            Debug.Log("Puerta Abierta");
        }
    }

    void OpenDoorAutomatically()
    {
        door.isLocked = false;
        door.OpenDoor();

        opened = true;
    }
}