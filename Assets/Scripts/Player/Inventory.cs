using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<string> items = new List<string>();
    public List<string> scrolls = new List<string>();

    public int selectedIndex = 0;

    public enum Tab
    {
        Items,
        Scrolls
    }

    public Tab currentTab = Tab.Items;

    public void AddItem(string itemName)
    {
        items.Add(itemName);
        Debug.Log("Recogiste: " + itemName + ". Pulsa TAB para ver tus objetos");
    }

    public void AddScroll(string scrollText)
    {
        scrolls.Add(scrollText);
        Debug.Log("Pergamino guardado. Pulsa TAB para leerlo");
    }

    public bool HasItem(string itemName)
    {
        return items.Contains(itemName);
    }

    public int GetCount()
    {
        if (currentTab == Tab.Items) return items.Count;
        else return scrolls.Count;
    }

    public string GetSelected()
    {
        if (currentTab == Tab.Items)
            return items[selectedIndex];

        return scrolls[selectedIndex];
    }
}