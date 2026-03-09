using UnityEngine;

public class Pergamino : MonoBehaviour, IInteractable
{
    [TextArea]
    public string text;

    public void Interact()
    {
        Inventory inventory = FindObjectOfType<Inventory>();

        if (inventory != null)
        {
            inventory.AddScroll(text);
        }

        Destroy(gameObject);
    }
}