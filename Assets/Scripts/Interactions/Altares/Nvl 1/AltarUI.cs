using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using FMODUnity;

public class AltarUI : MonoBehaviour
{
    public AltarRitual altar;
    public GameObject[] selectors;

    [Header("Eventos")]
    public Door door;
    public NPC_Controller NPC;
    public EventReference cerrarPuerta;
    public EventReference evilLaugh;

    [Header("UI Opciones")]
    public TMP_Text[] optionsText;
    string[] baseOptions = { "Ofrecer", "Cerrar" };

    [Header("Texto principal")]
    public TMP_Text titleText;

    [Header("Timing")]
    public float offerDelay = 2f;

    int selectedIndex = 0;
    bool ritualDone = false;
    bool isOpen = false;

    void OnEnable()
    {
        selectedIndex = 0;
        PrintMenu();
        UpdateTitle();
    }

    void Update()
    {
        if (!isOpen) return;
        Navigate();
        Select();
    }

    public void OpenUI()
    {
        isOpen = true;
        selectedIndex = 0;
        ShowCursor(true);
        PrintMenu();
        UpdateTitle();
    }

    public void CloseUI()
    {
        isOpen = false;
        ShowCursor(false);
    }

    // --- Métodos públicos para asignar a los Button.onClick desde el Inspector ---

    public void OnClickOffer()
    {
        if (!isOpen || ritualDone) return;
        selectedIndex = 0;
        PrintMenu();
        Offer();
    }

    public void OnClickClose()
    {
        if (!isOpen) return;
        selectedIndex = 1;
        PrintMenu();
        Close();
    }

    // -------------------------------------------------------------------------

    void Navigate()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            selectedIndex = (selectedIndex + 1) % optionsText.Length;
            PrintMenu();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            selectedIndex = (selectedIndex - 1 + optionsText.Length) % optionsText.Length;
            PrintMenu();
        }
    }

    void Select()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (selectedIndex == 0) Offer();
            else Close();
        }
    }

    void PrintMenu()
    {
        for (int i = 0; i < optionsText.Length; i++)
            selectors[i].SetActive(i == selectedIndex);
    }

    void UpdateTitle()
    {
        string nextItem = altar.GetNextItemName();
        titleText.text = nextItem != null ? "¿Ofrecer " + nextItem + "?" : "";
    }

    void Offer()
    {
        if (ritualDone) return;
        StartCoroutine(OfferRoutine());
    }

    IEnumerator OfferRoutine()
    {
        if (ritualDone) yield break;

        enabled = false;

        bool completed = altar.OfferNextItem();

        altar.HideUI();

        yield return new WaitForSeconds(offerDelay);

        if (completed)
        {
            ritualDone = true;
            ShowCursor(false);
            StartRitual();
            gameObject.SetActive(false);
            GameState.InMenu = false;
        }
        else
        {
            altar.ShowUI();
            UpdateTitle();
            enabled = true;
        }
    }

    void StartRitual()
    {
        AudioManager.Instance.Play(evilLaugh);
        door.CloseDoor();
        door.isLocked = true;
        AudioManager.Instance.Play(cerrarPuerta);
        NPC.PrepareForPossession();
        NPC.Possess();

        Debug.Log("RITUAL ACTIVADO");
    }

    void Close()
    {
        altar.HideUI();
        CloseUI();
        GameState.InMenu = false;
    }

    void ShowCursor(bool show)
    {
        Cursor.visible = show;
        Cursor.lockState = show ? CursorLockMode.None : CursorLockMode.Locked;
    }
}