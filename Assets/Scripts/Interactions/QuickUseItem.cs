using UnityEngine;

public class QuickUseItem : MonoBehaviour
{
    Inventory inventory;

    void Start()
    {
        inventory = GetComponent<Inventory>();
    }

    void Update()
    {
        if (GameState.InMenu) return;

        if (Input.GetKeyDown(KeyCode.F))
            UseEquipped();
    }

    void UseEquipped()
    {
        if (inventory.equippedItem == null)
        {
            Debug.Log("No hay item equipado");
            return;
        }

        switch (inventory.equippedItem)
        {
            case "Agua Bendita":
                UseAguaBendita();
                break;
                // futuros items van aquí como nuevos cases
        }
    }

    void UseAguaBendita()
    {
        if (!inventory.HasItem("Agua Bendita"))
        {
            inventory.equippedItem = null;
            Debug.Log("Sin agua bendita");
            return;
        }

        // Busca todos los Entes cercanos y los ahuyenta
        EntePsicologico[] entes = FindObjectsOfType<EntePsicologico>();
        foreach (EntePsicologico ente in entes)
            ente.Repel();

        inventory.RemoveItem("Agua Bendita");

        // Si se agotó, desequipar
        if (!inventory.HasItem("Agua Bendita"))
            inventory.equippedItem = null;

        Debug.Log("Usaste agua bendita");
    }
}