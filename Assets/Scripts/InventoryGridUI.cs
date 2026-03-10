using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class InventoryGridUI : MonoBehaviour
{
    public GameObject inventoryPanel;

    public Transform gridParent;
    public GameObject itemTemplate;

    public TextMeshProUGUI tabsText;
    public TextMeshProUGUI descriptionText;

    Inventory inventory;

    int columns = 4;
    int selectedIndex = 0;

    Vector2 mouseMove;

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

        if (!GameState.InMenu) return;

        KeyboardNavigation();
        MouseNavigation();
        SelectItem();
    }

    void ToggleMenu()
    {
        GameState.InMenu = !GameState.InMenu;

        inventoryPanel.SetActive(GameState.InMenu);

        if (GameState.InMenu)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            selectedIndex = 0;
            RefreshGrid();
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void KeyboardNavigation()
    {
        int count = inventory.GetCount();

        if (count == 0) return;

        if (Input.GetKeyDown(KeyCode.W))
            selectedIndex -= columns;

        if (Input.GetKeyDown(KeyCode.S))
            selectedIndex += columns;

        if (Input.GetKeyDown(KeyCode.A))
            selectedIndex -= 1;

        if (Input.GetKeyDown(KeyCode.D))
            selectedIndex += 1;

        selectedIndex = Mathf.Clamp(selectedIndex, 0, count - 1);

        RefreshGrid();
    }

    void MouseNavigation()
    {
        float x = Input.GetAxis("Mouse X");
        float y = Input.GetAxis("Mouse Y");

        mouseMove += new Vector2(x, y);

        if (mouseMove.x > 3f)
        {
            selectedIndex++;
            mouseMove.x = 0;
        }

        if (mouseMove.x < -3f)
        {
            selectedIndex--;
            mouseMove.x = 0;
        }

        if (mouseMove.y > 3f)
        {
            selectedIndex -= columns;
            mouseMove.y = 0;
        }

        if (mouseMove.y < -3f)
        {
            selectedIndex += columns;
            mouseMove.y = 0;
        }

        int count = inventory.GetCount();
        selectedIndex = Mathf.Clamp(selectedIndex, 0, count - 1);

        RefreshGrid();
    }

    void SelectItem()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
        {
            string selected = inventory.GetSelected();

            if (inventory.currentTab == Inventory.Tab.Scrolls)
            {
                descriptionText.text = selected;
            }
            else
            {
                descriptionText.text = "Item: " + selected;
            }
        }
    }

    void RefreshGrid()
    {
        foreach (Transform child in gridParent)
        {
            if (child != itemTemplate.transform)
                Destroy(child.gameObject);
        }

        int count = inventory.GetCount();

        for (int i = 0; i < count; i++)
        {
            GameObject newItem = Instantiate(itemTemplate, gridParent);
            newItem.SetActive(true);

            Text txt = newItem.GetComponent<Text>();

            string name;

            if (inventory.currentTab == Inventory.Tab.Items)
                name = inventory.items[i];
            else
                name = "Pergamino " + (i + 1);

            if (i == selectedIndex)
                txt.text = "> " + name;
            else
                txt.text = name;
        }

        tabsText.text = inventory.currentTab.ToString();
    }
}