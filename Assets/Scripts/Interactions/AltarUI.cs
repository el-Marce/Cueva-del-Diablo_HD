using UnityEngine;
using TMPro;

public class AltarUI : MonoBehaviour
{
    public AltarRitual altar;

    [Header("Eventos")]
    public Door door;
    public NPC_Controller NPC;

    [Header("UI Opciones")]
    public TMP_Text[] optionsText;
    string[] baseOptions = { "Ofrecer", "Cerrar" };

    [Header("UI Items")]
    public TMP_Text cocaText;
    public TMP_Text alcoholText;
    public TMP_Text sulluText;

    [Header("Texto principal")]
    public TMP_Text titleText;

    int selectedIndex = 0;
    bool ritualDone = false;

    void OnEnable()
    {
        selectedIndex = 0;

        PrintMenu();
        UpdateItemStatus();
        UpdateTitle();
    }

    void Update()
    {
        if (!gameObject.activeSelf) return;

        Navigate();
        Select();
    }

    void Navigate()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            selectedIndex++;
            if (selectedIndex >= optionsText.Length)
                selectedIndex = 0;

            PrintMenu();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            selectedIndex--;
            if (selectedIndex < 0)
                selectedIndex = optionsText.Length - 1;

            PrintMenu();
        }
    }

    void Select()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (selectedIndex == 0)
                Offer();
            else
                Close();
        }
    }

    void PrintMenu()
    {
        for (int i = 0; i < optionsText.Length; i++)
        {
            if (i == selectedIndex)
                optionsText[i].text = "<b>[ " + baseOptions[i] + " ]</b>";
            else
                optionsText[i].text = baseOptions[i];
        }
    }

    void UpdateItemStatus()
    {
        cocaText.text = "Coca: " + (altar.cocaEntregada ? "1" : "0");
        alcoholText.text = "Alcohol: " + (altar.alcoholEntregado ? "1" : "0");
        sulluText.text = "Sullu: " + (altar.sulluEntregado ? "1" : "0");
    }

    void UpdateTitle()
    {
        string nextItem = altar.GetNextItemName();

        if (nextItem != null)
            titleText.text = "¿Ofrecer " + nextItem + "?";
        else
            titleText.text = "";
    }

    void Offer()
    {
        if (ritualDone) return;

        bool completed = altar.OfferNextItem();

        UpdateItemStatus();

        if (completed)
        {
            ritualDone = true;

            gameObject.SetActive(false);
            GameState.InMenu = false;

            StartRitual();
            return;
        }

        UpdateTitle();
    }

    void StartRitual()
    {
        door.CloseDoor();
        door.isLocked = true;

        NPC.Possess();

        Debug.Log("RITUAL ACTIVADO");
    }

    void Close()
    {
        gameObject.SetActive(false);
        GameState.InMenu = false;
    }
}