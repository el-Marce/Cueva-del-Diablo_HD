using UnityEngine;

public class AltarCondition_Item : AltarCondition
{
    public string itemName = "Agua Bendita";
    [Min(1)] public int requiredAmount = 1;

    Inventory inventory;

    void Awake()
    {
        inventory = FindObjectOfType<Inventory>();
    }

    public override bool IsMet()
    {
        return inventory.GetItemUses(itemName) >= requiredAmount;
    }

    public override string GetStatusText()
    {
        int current = inventory.GetItemUses(itemName);
        return $"{itemName}: {Mathf.Min(current, requiredAmount)}/{requiredAmount}";
    }

    public override void OnFulfill()
    {
        for (int i = 0; i < requiredAmount; i++)
            inventory.RemoveItem(itemName);
    }
}