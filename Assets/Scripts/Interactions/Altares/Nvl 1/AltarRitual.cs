using UnityEngine;
public class AltarRitual : MonoBehaviour, IInteractable
{
    public GameObject altarUI;
    public GameObject altarPanel;
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
        altarPanel.SetActive(true);
        altarUI.GetComponent<AltarUI>().OpenUI();
        GameState.InMenu = true;
    }

    public bool OfferNextItem()
    {
        if (!cocaEntregada && inventory.HasItem("Coca"))
        {
            inventory.RemoveItem("Coca");
            cocaEntregada = true;
            return false;
        }
        if (!alcoholEntregado && inventory.HasItem("Alcohol"))
        {
            inventory.RemoveItem("Alcohol");
            alcoholEntregado = true;
            return false;
        }
        if (!sulluEntregado && inventory.HasItem("Sullu"))
        {
            inventory.RemoveItem("Sullu");
            sulluEntregado = true;
            ritualCompleted = true;
            DisableInteraction();
            return true;
        }
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
        altarPanel.SetActive(false);
        altarUI.GetComponent<AltarUI>().CloseUI();
    }

    public void ShowUI()
    {
        altarPanel.SetActive(true);
        altarUI.GetComponent<AltarUI>().OpenUI();
    }
}