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
        // Registrar pickup ANTES de destruir el objeto
        CheckpointManager.Instance?.RegistrarPickup(transform.GetFullPath());

        Inventory inventory = FindObjectOfType<Inventory>();
        if (inventory != null)
        {
            inventory.AddItem(itemName, icon);
            switch (itemName)
            {
                case "Agua Bendita":
                    AudioManager.Instance.Play(clinkBotella);
                    FindObjectOfType<HealthSystem>()?.Heal(15f);
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