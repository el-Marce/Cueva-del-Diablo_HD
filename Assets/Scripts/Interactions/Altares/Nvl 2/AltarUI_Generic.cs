using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class AltarUI_Generic : MonoBehaviour
{
    [Header("Panel")]
    public GameObject altarPanel;

    [Header("UI")]
    public TMP_Text titleText;
    public TMP_Text[] optionsText;

    // Botones opcionales: asignar en el Inspector los Button components
    // que envuelven cada opción (índice 0 = Ofrecer, índice 1 = Cerrar).
    // Si no se asignan, el panel funciona solo con teclado.
    [Header("Botones (opcional)")]
    public Button[] optionButtons;

    [Header("Ritmo")]
    public RitmoUI ritmoUI;

    string[] baseOptions = { "Ofrecer", "Cerrar" };
    int selectedIndex = 0;
    AltarRitual_Generic currentAltar;
    bool busy = false;

    [Header("Condition Icons")]
    public AltarConditionIcon[] conditionIcons;

    void OnEnable() { selectedIndex = 0; }

    public void Open(AltarRitual_Generic altar)
    {
        currentAltar = altar;
        altarPanel.SetActive(true);
        GameState.InMenu = true;
        ShowCursor(true);
        Refresh();
    }

    public void Close()
    {
        altarPanel.SetActive(false);
        GameState.InMenu = false;
        ShowCursor(false);
        currentAltar = null;
    }

    // --- Métodos públicos para asignar a los Button.onClick desde el Inspector ---

    public void OnClickOffer()
    {
        if (currentAltar == null || busy) return;
        selectedIndex = 0;
        PrintOptions();
        StartCoroutine(OfferRoutine());
    }

    public void OnClickClose()
    {
        if (currentAltar == null || busy) return;
        selectedIndex = 1;
        PrintOptions();
        Close();
    }

    // -------------------------------------------------------------------------

    void Update()
    {
        if (currentAltar == null || busy) return;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Close();
            return;
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            selectedIndex = (selectedIndex + 1) % optionsText.Length;
            PrintOptions();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            selectedIndex = (selectedIndex - 1 + optionsText.Length) % optionsText.Length;
            PrintOptions();
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (selectedIndex == 0) StartCoroutine(OfferRoutine());
            else Close();
        }

        RefreshConditions();
    }

    void Refresh()
    {
        PrintTitle();
        RefreshConditions();
        PrintOptions();
    }

    void PrintTitle()
    {
        if (currentAltar.conditions.Count == 0) return;
        foreach (var c in currentAltar.conditions)
        {
            if (!c.IsMet())
            {
                titleText.text = "Items requeridos";
                return;
            }
        }
        titleText.text = "Todo listo. ¿Ofrecer?";
    }

    void RefreshConditions()
    {
        if (currentAltar == null) return;

        System.Collections.Generic.List<AltarCondition> visualConditions =
            new System.Collections.Generic.List<AltarCondition>();

        foreach (var c in currentAltar.conditions)
        {
            if (c.showIcon)
                visualConditions.Add(c);
        }

        for (int i = 0; i < conditionIcons.Length; i++)
        {
            if (i >= visualConditions.Count)
            {
                conditionIcons[i].gameObject.SetActive(false);
                continue;
            }

            conditionIcons[i].gameObject.SetActive(true);

            AltarCondition condition = visualConditions[i];

            conditionIcons[i].image.sprite =
                condition.IsMet()
                ? condition.activeIcon
                : condition.inactiveIcon;
        }
    }

    void PrintOptions()
    {
        for (int i = 0; i < optionsText.Length; i++)
        {
            optionsText[i].text = i == selectedIndex
                ? "<b>[ " + baseOptions[i] + " ]</b>"
                : baseOptions[i];
        }
    }

    // Centraliza toda la lógica del cursor en un solo lugar.
    void ShowCursor(bool show)
    {
        Cursor.visible = show;
        Cursor.lockState = show ? CursorLockMode.None : CursorLockMode.Locked;
    }

    IEnumerator OfferRoutine()
    {
        busy = true;
        altarPanel.SetActive(false);

        AltarCondition_RhythmChallenge rhythm =
            currentAltar.GetComponent<AltarCondition_RhythmChallenge>();

        bool previasCumplidas = true;
        foreach (var c in currentAltar.conditions)
        {
            if (c == rhythm) continue;
            if (!c.IsMet())
            {
                previasCumplidas = false;
                break;
            }
        }

        if (!previasCumplidas)
        {
            altarPanel.SetActive(true);
            busy = false;
            yield break;
        }

        if (rhythm != null && !rhythm.IsMet() && ritmoUI != null)
        {
            MicrophoneInput mic = FindObjectOfType<MicrophoneInput>();
            if (mic != null) mic.rhythmCondition = rhythm;

            // Al abrir el ritmo el panel del altar se oculta; el cursor
            // queda visible para que ritmoUI lo gestione si lo necesita.
            ritmoUI.Open(currentAltar, rhythm);
            busy = false;
            yield break;
        }

        if (!currentAltar.AllConditionsMet())
        {
            altarPanel.SetActive(true);
            busy = false;
            yield break;
        }

        // Cierra limpiamente (oculta cursor) antes de activar el altar.
        ShowCursor(false);
        currentAltar.TryActivate();
        busy = false;
    }
}