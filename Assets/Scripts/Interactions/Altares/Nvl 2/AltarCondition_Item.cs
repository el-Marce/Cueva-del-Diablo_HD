using UnityEngine;

public class AltarCondition_Item : AltarCondition
{
    public string itemName = "agua bendita";
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
        return "Ofrecer " + itemName + ": " + (has ? "1" : "0");
    }

    public override void OnFulfill()
    {
        inventory.RemoveItem(itemName);
    }
}