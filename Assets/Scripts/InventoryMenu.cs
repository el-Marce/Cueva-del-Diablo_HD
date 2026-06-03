// InventoryMenu.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class InventoryMenu : MonoBehaviour
{
    Inventory inventory;

    public GameObject inventoryPanel;
    public TMP_Text tabText;
    public TMP_Text[] itemTexts;
    public TMP_Text descriptionText;
    public GameObject lecturaPanel;
    public int columns = 4;
    public Image[] selectors;
    public Image[] itemIcons;

    public TMP_Text scrollText;

    [Header("Tutorial (opcional)")]
    public TutorialStep stepAlCerrarLectura;
    public TutorialBarrier barreraAlCerrarLectura;
    public TutorialStep stepEncolado;        // Step 3: NPC/item, se encola tras el step 2
    public TutorialBarrier barreraEncolada;

    bool menuOpen = false;

    void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        menuOpen = false;
        GameState.InMenu = false;

        Transform canvas = GameObject.Find("UI").transform.Find("Canvas");

        inventoryPanel = canvas.Find("InventoryPanel").gameObject;
        lecturaPanel = canvas.Find("LecturaPanel").gameObject;
        tabText = canvas.Find("InventoryPanel/TabsText").GetComponent<TMP_Text>();
        descriptionText = canvas.Find("InventoryPanel/DescTextInvent").GetComponent<TMP_Text>();

        itemTexts = new TMP_Text[]
        {
            canvas.Find("InventoryPanel/Grid/Item0/UsesText").GetComponent<TMP_Text>(),
            canvas.Find("InventoryPanel/Grid/Item1/UsesText").GetComponent<TMP_Text>(),
            canvas.Find("InventoryPanel/Grid/Item2/UsesText").GetComponent<TMP_Text>(),
            canvas.Find("InventoryPanel/Grid/Item3/UsesText").GetComponent<TMP_Text>(),
            canvas.Find("InventoryPanel/Grid/Item4/UsesText").GetComponent<TMP_Text>(),
            canvas.Find("InventoryPanel/Grid/Item5/UsesText").GetComponent<TMP_Text>(),
            canvas.Find("InventoryPanel/Grid/Item6/UsesText").GetComponent<TMP_Text>(),
            canvas.Find("InventoryPanel/Grid/Item7/UsesText").GetComponent<TMP_Text>(),
            canvas.Find("InventoryPanel/Grid/Item8/UsesText").GetComponent<TMP_Text>(),
            canvas.Find("InventoryPanel/Grid/Item9/UsesText").GetComponent<TMP_Text>(),
            canvas.Find("InventoryPanel/Grid/Item10/UsesText").GetComponent<TMP_Text>(),
            canvas.Find("InventoryPanel/Grid/Item11/UsesText").GetComponent<TMP_Text>()
        };

        selectors = new Image[]
        {
            canvas.Find("InventoryPanel/Grid/Item0/Selector").GetComponent<Image>(),
            canvas.Find("InventoryPanel/Grid/Item1/Selector").GetComponent<Image>(),
            canvas.Find("InventoryPanel/Grid/Item2/Selector").GetComponent<Image>(),
            canvas.Find("InventoryPanel/Grid/Item3/Selector").GetComponent<Image>(),
            canvas.Find("InventoryPanel/Grid/Item4/Selector").GetComponent<Image>(),
            canvas.Find("InventoryPanel/Grid/Item5/Selector").GetComponent<Image>(),
            canvas.Find("InventoryPanel/Grid/Item6/Selector").GetComponent<Image>(),
            canvas.Find("InventoryPanel/Grid/Item7/Selector").GetComponent<Image>(),
            canvas.Find("InventoryPanel/Grid/Item8/Selector").GetComponent<Image>(),
            canvas.Find("InventoryPanel/Grid/Item9/Selector").GetComponent<Image>(),
            canvas.Find("InventoryPanel/Grid/Item10/Selector").GetComponent<Image>(),
            canvas.Find("InventoryPanel/Grid/Item11/Selector").GetComponent<Image>()
        };

        itemIcons = new Image[]
        {
            canvas.Find("InventoryPanel/Grid/Item0/Icon").GetComponent<Image>(),
            canvas.Find("InventoryPanel/Grid/Item1/Icon").GetComponent<Image>(),
            canvas.Find("InventoryPanel/Grid/Item2/Icon").GetComponent<Image>(),
            canvas.Find("InventoryPanel/Grid/Item3/Icon").GetComponent<Image>(),
            canvas.Find("InventoryPanel/Grid/Item4/Icon").GetComponent<Image>(),
            canvas.Find("InventoryPanel/Grid/Item5/Icon").GetComponent<Image>(),
            canvas.Find("InventoryPanel/Grid/Item6/Icon").GetComponent<Image>(),
            canvas.Find("InventoryPanel/Grid/Item7/Icon").GetComponent<Image>(),
            canvas.Find("InventoryPanel/Grid/Item8/Icon").GetComponent<Image>(),
            canvas.Find("InventoryPanel/Grid/Item9/Icon").GetComponent<Image>(),
            canvas.Find("InventoryPanel/Grid/Item10/Icon").GetComponent<Image>(),
            canvas.Find("InventoryPanel/Grid/Item11/Icon").GetComponent<Image>()
        };
    }

    void Start()
    {
        inventory = GetComponent<Inventory>();
    }

    void Update()
    {
        if (lecturaPanel == null || inventoryPanel == null) return;

        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Escape))
        {
            if (lecturaPanel.activeSelf)
            {
                lecturaPanel.SetActive(false);
                GameState.InMenu = false;

                if (stepAlCerrarLectura != null)
                {
                    TutorialManager.Instance?.MostrarPaso(stepAlCerrarLectura, barreraAlCerrarLectura);
                    if (stepEncolado != null)
                        TutorialManager.Instance?.MostrarPaso(stepEncolado, barreraEncolada);
                    stepAlCerrarLectura = null;
                    stepEncolado = null;
                }
                return;
            }

            if (menuOpen)
            {
                ToggleMenu();
                return;
            }

            if (Input.GetKeyDown(KeyCode.Tab) && !GameState.InMenu)
                ToggleMenu();
        }

        if (!menuOpen) return;

        GameState.InMenu = true;
        Navigate();
        ChangeTab();
        Select();
    }

    void ToggleMenu()
    {
        menuOpen = !menuOpen;
        GameState.InMenu = menuOpen;
        inventoryPanel.SetActive(menuOpen);

        if (menuOpen)
            PrintMenu();
        else
            OnInventarioCerrado();
    }

    void OnInventarioCerrado()
    {
        if (string.IsNullOrEmpty(inventory.equippedWeapon)) return;
        inventory.NotificarArmaEquipada(inventory.equippedWeapon);
    }

    void Navigate()
    {
        int count = inventory.GetCount();
        int row = inventory.selectedIndex / columns;
        int col = inventory.selectedIndex % columns;
        int totalRows = Mathf.CeilToInt((float)count / columns);

        if (Input.GetKeyDown(KeyCode.D))
        {
            col++;
            if (col >= columns || row * columns + col >= count) col = 0;
            inventory.selectedIndex = row * columns + col;
            PrintMenu();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            col--;
            if (col < 0)
            {
                col = columns - 1;
                if (row * columns + col >= count) col = (count - 1) % columns;
            }
            inventory.selectedIndex = row * columns + col;
            PrintMenu();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            row++;
            if (row >= totalRows || row * columns + col >= count) row = 0;
            inventory.selectedIndex = row * columns + col;
            PrintMenu();
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            row--;
            if (row < 0)
            {
                row = totalRows - 1;
                if (row * columns + col >= count) row--;
            }
            inventory.selectedIndex = row * columns + col;
            PrintMenu();
        }
    }

    void ChangeTab()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            inventory.currentTab = inventory.currentTab switch
            {
                Inventory.Tab.Items => Inventory.Tab.Scrolls,
                Inventory.Tab.Scrolls => Inventory.Tab.Weapons,
                Inventory.Tab.Weapons => Inventory.Tab.Items,
                _ => Inventory.Tab.Items
            };
            inventory.selectedIndex = 0;
            PrintMenu();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            inventory.currentTab = inventory.currentTab switch
            {
                Inventory.Tab.Items => Inventory.Tab.Weapons,
                Inventory.Tab.Weapons => Inventory.Tab.Scrolls,
                Inventory.Tab.Scrolls => Inventory.Tab.Items,
                _ => Inventory.Tab.Items
            };
            inventory.selectedIndex = 0;
            PrintMenu();
        }
    }

    void Select()
    {
        if (!Input.GetKeyDown(KeyCode.Return)) return;
        if (inventory.currentTab == Inventory.Tab.Scrolls)
        {
            // Abrir panel de lectura completa reutilizando el existente
            string textoCompleto = inventory.scrolls[inventory.selectedIndex].text;
            scrollText.text = textoCompleto;
            lecturaPanel.SetActive(true);
            GameState.InMenu = true;
        }
        else
        {
            inventory.EquipSelected();
            PrintMenu();
        }
    }
    void UpdateDescription()
    {
        if (inventory.GetCount() == 0)
        {
            descriptionText.text = "No hay objetos";
            return;
        }

        if (inventory.currentTab == Inventory.Tab.Scrolls)
        {
            string texto = inventory.scrolls[inventory.selectedIndex].text;
            // Truncar a 120 caracteres con indicador de que hay más
            descriptionText.text = texto.Length > 120
                ? texto.Substring(0, 200) + "...\n<align=right>[Enter para leer]"
                : texto;
        }
        else
        {
            descriptionText.text = "Objeto: " + inventory.GetSelected();
        }
    }
    void PrintMenu()
    {
        if (itemTexts == null || itemTexts.Length == 0 || itemTexts[0] == null) return;
        if (selectors == null || selectors.Length == 0 || selectors[0] == null) return;
        if (itemIcons == null || itemIcons.Length == 0 || itemIcons[0] == null) return;

        tabText.text = inventory.currentTab switch
        {
            Inventory.Tab.Items => "(Q) < Items > (E)",
            Inventory.Tab.Scrolls => "(Q) < Pergaminos > (E)",
            Inventory.Tab.Weapons => "(Q) < Armas > (E)",
            _ => ""
        };

        int count = inventory.GetCount();

        for (int i = 0; i < itemTexts.Length; i++)
        {
            if (i >= count)
            {
                itemTexts[i].gameObject.SetActive(false);
                selectors[i].gameObject.SetActive(false);
                itemIcons[i].gameObject.SetActive(false);
                continue;
            }

            if (inventory.currentTab == Inventory.Tab.Items)
            {
                itemTexts[i].gameObject.SetActive(true);
                itemTexts[i].text = "x" + inventory.items[i].uses;
                itemIcons[i].gameObject.SetActive(true);
                itemIcons[i].sprite = inventory.items[i].icon;
            }
            else if (inventory.currentTab == Inventory.Tab.Scrolls)
            {
                itemTexts[i].gameObject.SetActive(false);
                itemIcons[i].gameObject.SetActive(true);
                itemIcons[i].sprite = inventory.scrolls[i].icon;
            }
            else
            {
                itemTexts[i].gameObject.SetActive(true);
                itemTexts[i].text = "x" + inventory.weapons[i].durability;
                itemIcons[i].gameObject.SetActive(true);
                itemIcons[i].sprite = inventory.weapons[i].icon;
            }

            bool isSelected = i == inventory.selectedIndex;
            bool isEquipped = inventory.currentTab == Inventory.Tab.Weapons
                && i < inventory.weapons.Count
                && inventory.equippedWeapon == inventory.weapons[i].name;

            selectors[i].gameObject.SetActive(isSelected || isEquipped);
            if (isSelected || isEquipped)
                selectors[i].color = isEquipped ? Color.white : Color.red;
        }

        UpdateDescription();
    }
}