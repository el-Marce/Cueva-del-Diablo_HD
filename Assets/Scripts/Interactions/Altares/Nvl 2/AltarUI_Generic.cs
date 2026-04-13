using UnityEngine;
using TMPro;
using System.Collections;

public class AltarUI_Generic : MonoBehaviour
{
    [Header("Panel")]
    public GameObject altarPanel;

    [Header("UI")]
    public TMP_Text titleText;
    public TMP_Text conditionsText;
    public TMP_Text[] optionsText;

    [Header("Ritmo")]
    public RitmoUI ritmoUI;

    string[] baseOptions = { "Ofrecer", "Cerrar" };
    int selectedIndex = 0;
    AltarRitual_Generic currentAltar;
    bool busy = false;

    void OnEnable() { selectedIndex = 0; }

    public void Open(AltarRitual_Generic altar)
    {
        currentAltar = altar;
        altarPanel.SetActive(true);
        GameState.InMenu = true;
        Refresh();
    }

    public void Close()
    {
        altarPanel.SetActive(false);
        GameState.InMenu = false;
        currentAltar = null;
    }

    void Update()
    {
        if (currentAltar == null || busy) return;

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

        // Refresca condiciones dinámicas (entes moviéndose, etc.)
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
        // El título muestra la primera condición pendiente
        foreach (var c in currentAltar.conditions)
        {
            if (!c.IsMet())
            {
                titleText.text = "¿" + c.GetStatusText() + "?";
                return;
            }
        }
        titleText.text = "Todo listo. ¿Ofrecer?";
    }

    void RefreshConditions()
    {
        if (currentAltar == null) return;
        string result = "";
        foreach (var c in currentAltar.conditions)
            result += c.GetStatusText() + "\n";
        conditionsText.text = result.TrimEnd();
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

    IEnumerator OfferRoutine()
    {
        busy = true;
        altarPanel.SetActive(false);

        AltarCondition_RhythmChallenge rhythm =
            currentAltar.GetComponent<AltarCondition_RhythmChallenge>();

        // Verifica condiciones no-ritmo primero
        bool previasCumplidas = true;
        foreach (var c in currentAltar.conditions)
        {
            if (c == rhythm) continue; // ignora el ritmo en esta evaluación
            if (!c.IsMet())
            {
                previasCumplidas = false;
                break;
            }
        }

        if (!previasCumplidas)
        {
            //Debug.Log("[AltarUI] Condiciones previas no cumplidas.");
            altarPanel.SetActive(true);
            busy = false;
            yield break;
        }

        // Previas cumplidas — abre ritmo si existe y no está resuelto
        if (rhythm != null && !rhythm.IsMet() && ritmoUI != null)
        {
            MicrophoneInput mic = FindObjectOfType<MicrophoneInput>();
            if (mic != null) mic.rhythmCondition = rhythm;

            ritmoUI.Open(currentAltar, rhythm);
            busy = false;
            yield break;
        }

        // Todo cumplido incluyendo ritmo
        if (!currentAltar.AllConditionsMet())
        {
            altarPanel.SetActive(true);
            busy = false;
            yield break;
        }

        currentAltar.TryActivate();
        busy = false;
    }
}