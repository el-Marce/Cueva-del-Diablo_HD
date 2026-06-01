using UnityEngine;
using FMODUnity;
public class PickupItem : MonoBehaviour, IInteractable
{
    public string itemName;
    public Sprite icon;

    [Header("Sonidos")]
    public EventReference clinkBotella;
    public EventReference bolsa;
    public EventReference genericItem;
    public void Interact()
    {
        Inventory inventory = FindObjectOfType<Inventory>();

        if (inventory != null)
        {
            inventory.AddItem(itemName, icon);

            switch (itemName)
            {
                case "Agua Bendita":
                    AudioManager.Instance.Play(clinkBotella);
                    break;
                case "Coca":
                    AudioManager.Instance.Play(bolsa);
                    break;
                default:
                    Debug.Log("DEFAULT: " + itemName);
                    AudioManager.Instance.Play(genericItem);
                    break;
            }
        }
        TutorialManager.Instance?.CompletarTriggerParcial("item_" + itemName.ToLower().Replace(" ", ""));
        Destroy(gameObject);
    }
}