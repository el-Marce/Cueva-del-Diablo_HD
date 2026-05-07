using UnityEngine;

public class PickupItem : MonoBehaviour, IInteractable
{
    public string itemName;
    public Sprite icon;

    public void Interact()
    {
        Inventory inventory = FindObjectOfType<Inventory>();

        if (inventory != null)
        {
            inventory.AddItem(itemName, icon);
        }

        Destroy(gameObject);
    }
}