using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<string> items = new List<string>();
    public List<string> scrolls = new List<string>();

    public void AddItem(string itemName)
    {
        items.Add(itemName);
        Debug.Log("Recogiste: " + itemName);
    }

    public void AddScroll(string scrollText)
    {
        scrolls.Add(scrollText);
        Debug.Log("Pergamino guardado");
    }

    public bool HasItem(string itemName)
    {
        return items.Contains(itemName);
    }

    public void ShowScrollList()
    {
        Debug.Log("---- PERGAMINOS ----");

        for (int i = 0; i < scrolls.Count; i++)
        {
            Debug.Log((i + 1) + " - Pergamino " + (i + 1));
        }
    }

    public void ReadScroll(int index)
    {
        if (index >= 0 && index < scrolls.Count)
        {
            Debug.Log("----- PERGAMINO -----");
            Debug.Log(scrolls[index]);
        }
    }
}