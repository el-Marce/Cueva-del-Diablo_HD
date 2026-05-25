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
        if (!cocaEntregada) return "Coca";
        if (!alcoholEntregado) return "Alcohol";
        if (!sulluEntregado) return "Sullu";

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
        if (!cocaEntregada && inventory.HasItem("Coca"))
        {
            inventory.RemoveItem("Coca");
            cocaEntregada = true;
            //Debug.Log("Entregaste coca");
            return false;
        }

        // Alcohol
        if (!alcoholEntregado && inventory.HasItem("Alcohol"))
        {
            inventory.RemoveItem("Alcohol");
            alcoholEntregado = true;
            //Debug.Log("Entregaste alcohol");
            return false;
        }

        // Sullu
        if (!sulluEntregado && inventory.HasItem("Sullu"))
        {
            inventory.RemoveItem("Sullu");
            sulluEntregado = true;
            ritualCompleted = true;
            DisableInteraction();
            //Debug.Log("Entregaste sullu");
            return true;
        }        

        //Debug.Log("No tienes el siguiente objeto requerido");
        return false;
    }

    void DisableInteraction()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;
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