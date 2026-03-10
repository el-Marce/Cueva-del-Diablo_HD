using UnityEngine;

public class InventoryMenu : MonoBehaviour
{
    Inventory inventory;

    bool menuOpen = false;

    void Start()
    {
        inventory = GetComponent<Inventory>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleMenu();
        }

        if (!menuOpen) return;

        Navigate();
        ChangeTab();
        Select();
    }

    void ToggleMenu()
    {
        menuOpen = !menuOpen;

        GameState.InMenu = menuOpen;

        if (menuOpen)
        {
            Debug.Log("Inventario abierto");
            PrintMenu();
        }
        else
        {
            Debug.Log("Inventario cerrado");
        }
    }

    void Navigate()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            inventory.selectedIndex--;

            if (inventory.selectedIndex < 0)
                inventory.selectedIndex = inventory.GetCount() - 1;

            PrintMenu();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            inventory.selectedIndex++;

            if (inventory.selectedIndex >= inventory.GetCount())
                inventory.selectedIndex = 0;

            PrintMenu();
        }
    }

    void ChangeTab()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            inventory.currentTab = Inventory.Tab.Items;
            inventory.selectedIndex = 0;
            PrintMenu();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            inventory.currentTab = Inventory.Tab.Scrolls;
            inventory.selectedIndex = 0;
            PrintMenu();
        }
    }

    void Select()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            string selected = inventory.GetSelected();

            if (inventory.currentTab == Inventory.Tab.Scrolls)
            {
                Debug.Log("LEYENDO PERGAMINO:");
                Debug.Log(selected);
            }
            else
            {
                Debug.Log("USANDO ITEM: " + selected);
            }
        }
    }

    void PrintMenu()
    {
        Debug.Log("------ INVENTARIO ------");
        Debug.Log("TAB: " + inventory.currentTab);

        int count = inventory.GetCount();

        for (int i = 0; i < count; i++)
        {
            string text;

            if (inventory.currentTab == Inventory.Tab.Items)
                text = inventory.items[i];
            else
                text = "Pergamino " + (i + 1);

            if (i == inventory.selectedIndex)
                Debug.Log("> " + text);
            else
                Debug.Log(text);
        }
    }
}