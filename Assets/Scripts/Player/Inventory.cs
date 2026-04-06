using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    //public List<string> items = new List<string>();
    public List<ItemData> items = new List<ItemData>();
    public List<string> scrolls = new List<string>();

    public string equippedItem = null;

    public int selectedIndex = 0;

    public enum Tab
    {
        Items,
        Scrolls
    }

    public Tab currentTab = Tab.Items;

    public void AddItem(string itemName, int uses = 1)
    {
        // Si ya existe, suma usos en lugar de duplicar
        ItemData existing = items.Find(i => i.name == itemName);
        if (existing != null)
        {
            existing.uses += uses;
            Debug.Log("Recogiste más " + itemName + ". Usos totales: " + existing.uses);
            return;
        }
        items.Add(new ItemData(itemName, uses));
        Debug.Log("Recogiste: " + itemName + " (" + uses + " usos). Pulsa TAB para ver tus objetos");
    }

    public void AddScroll(string scrollText)
    {
        scrolls.Add(scrollText);
        Debug.Log("Pergamino guardado. Pulsa TAB para leerlo");
    }

    public bool HasItem(string itemName)
    {
        return items.Exists(i => i.name == itemName);
    }

    public string GetSelected()
    {
        if (currentTab == Tab.Items)
            return items[selectedIndex].name;
        return scrolls[selectedIndex];
    }
    public int GetCount()
    {
        if (currentTab == Tab.Items) return items.Count;
        return scrolls.Count;
    }

    public void RemoveItem(string itemName)
    {
        ItemData item = items.Find(i => i.name == itemName);
        if (item == null) return;

        item.uses--;
        Debug.Log("Usaste: " + itemName + " | Usos restantes: " + item.uses);

        if (item.uses <= 0)
        {
            items.Remove(item);
            Debug.Log(itemName + " agotado");
        }
    }
    public int GetItemUses(string itemName)
    {
        ItemData item = items.Find(i => i.name == itemName);
        return item != null ? item.uses : 0;
    }

    public void EquipSelected()
    {
        if (currentTab != Tab.Items || items.Count == 0) return;
        string selected = items[selectedIndex].name;
        equippedItem = (equippedItem == selected) ? null : selected; // toggle
        Debug.Log(equippedItem != null ? "Equipado: " + equippedItem : "Desequipado");
    }
}