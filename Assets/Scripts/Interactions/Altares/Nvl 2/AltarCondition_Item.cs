using UnityEngine;
using UnityEngine.SceneManagement;

public class AltarCondition_Item : AltarCondition
{
    public string itemName = "Agua Bendita";
    [Min(1)] public int requiredAmount = 1;

    Inventory inventory;
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        inventory = FindObjectOfType<Inventory>();
    }
    void Start()
    {
        inventory = FindObjectOfType<Inventory>();
        if (inventory == null)
            Debug.LogError("[AltarCondition_Item] No se encontró Inventory en la escena");
    }

    public override bool IsMet()
    {
        if (inventory == null)
            inventory = FindObjectOfType<Inventory>();
        if (inventory == null) return false;
        return inventory.GetItemUses(itemName) >= requiredAmount;
    }

    public override string GetStatusText()
    {
        //if (inventory == null) return itemName + ": cargando...";
        int current = inventory.GetItemUses(itemName);
        return $"{itemName}: {Mathf.Min(current, requiredAmount)}/{requiredAmount}";
    }

    public override void OnFulfill()
    {
        for (int i = 0; i < requiredAmount; i++)
            inventory.RemoveItem(itemName);
    }
}