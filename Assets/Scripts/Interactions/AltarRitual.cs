using UnityEngine;

public class AltarRitual : MonoBehaviour, IInteractable
{
    public GameObject altarUI;
    bool ritualCompleted = false;

    Inventory inventory;

    [Header("Estado del ritual")]
    public bool cocaEntregada = false;
    public bool alcoholEntregado = false;
    public bool sulluEntregado = false;

    public string GetNextItemName()
    {
        if (!cocaEntregada) return "coca";
        if (!alcoholEntregado) return "alcohol";
        if (!sulluEntregado) return "sullu";

        return null;
    }

    void Start()
    {
        inventory = FindObjectOfType<Inventory>();
    }

    public void Interact()
    {
        if (ritualCompleted) return;

        altarUI.SetActive(true);
        GameState.InMenu = true;
    }

    public bool OfferNextItem()
    {
        // Coca
        if (!cocaEntregada && inventory.HasItem("coca"))
        {
            inventory.RemoveItem("coca");
            cocaEntregada = true;
            Debug.Log("Entregaste coca");
            return false;
        }

        // Alcohol
        if (!alcoholEntregado && inventory.HasItem("alcohol"))
        {
            inventory.RemoveItem("alcohol");
            alcoholEntregado = true;
            Debug.Log("Entregaste alcohol");
            return false;
        }

        // Sullu
        if (!sulluEntregado && inventory.HasItem("sullu"))
        {
            inventory.RemoveItem("sullu");
            sulluEntregado = true;
            ritualCompleted = true;
            DisableInteraction();
            Debug.Log("Entregaste sullu");
            return true;
        }

        void DisableInteraction()
        {
            // Opción 1: desactivar collider (RECOMENDADO)
            Collider col = GetComponent<Collider>();
            if (col != null)
                col.enabled = false;

            // Opción 2 (extra segura): desactivar este script
            this.enabled = false;
        }

        Debug.Log("No tienes el siguiente objeto requerido");
        return false;
    }
    public void HideUI()
    {
        altarUI.SetActive(false);
    }

    public void ShowUI()
    {
        altarUI.SetActive(true);
    }
}