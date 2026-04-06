using UnityEngine;

public class AltarCondition_Item : AltarCondition
{
    public string itemName = "Agua Bendita";
    Inventory inventory;

    void Awake()
    {
        inventory = FindObjectOfType<Inventory>();
    }

    public override bool IsMet()
    {
        return inventory.HasItem(itemName);
    }

    public override string GetStatusText()
    {
        bool has = inventory.HasItem(itemName);
        return itemName + " en inventario : " + (has ? "Y" : "N");
    }

    public override void OnFulfill()
    {
        inventory.RemoveItem(itemName);
    }
}