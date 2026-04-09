using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class RitmoUI : MonoBehaviour
{
    [Header("Panel")]
    public GameObject ritmoPanel;

    [Header("UI")]
    public TMP_Text titleText;
    public TMP_Text statusText;
    public Image[] circulos;

    [Header("Colores")]
    public Color colorApagado = new Color(0.2f, 0.2f, 0.2f, 1f);
    public Color colorEncendido = Color.white;
    public Color colorError = Color.red;

    AltarCondition_RhythmChallenge rhythmCondition;
    AltarRitual_Generic altar;
    int currentStep = 0;

    public void Open(AltarRitual_Generic _altar, AltarCondition_RhythmChallenge _rhythm)
    {
        altar = _altar;
        rhythmCondition = _rhythm;
        rhythmCondition.acceptingInput = true;
        rhythmCondition.OnPulseRegistered += OnPulse;
        rhythmCondition.OnPatternFailed += OnFail;
        rhythmCondition.OnPatternSolved += OnSolved;

        ritmoPanel.SetActive(true);
        currentStep = 0;

        if (titleText != null)
            titleText.text = rhythmCondition.displayText;

        if (statusText != null)
            statusText.text = "Escucha y repite...";

        ResetCirculos();
    }

    public void Close()
    {
        if (rhythmCondition != null)
        {
            rhythmCondition.acceptingInput = false;
            rhythmCondition.OnPulseRegistered -= OnPulse;
            rhythmCondition.OnPatternFailed -= OnFail;
            rhythmCondition.OnPatternSolved -= OnSolved;
        }
        ritmoPanel.SetActive(false);
    }

    void OnPulse()
    {
        if (currentStep < circulos.Length)
        {
            circulos[currentStep].color = colorEncendido;
            currentStep++;
        }
    }

    void OnFail()
    {
        StartCoroutine(FailRoutine());
    }

    void OnSolved()
    {
        // Ilumina todos por si quedó alguno
        foreach (var c in circulos)
            c.color = colorEncendido;

        if (statusText != null)
            statusText.text = "¡Ritual completado!";

        StartCoroutine(CloseAfterDelay());
    }

    IEnumerator FailRoutine()
    {
        foreach (var c in circulos)
            c.color = colorError;

        if (statusText != null)
            statusText.text = "Inténtalo de nuevo...";

        yield return new WaitForSeconds(0.6f);

        currentStep = 0;
        ResetCirculos();

        if (statusText != null)
            statusText.text = "Escucha y repite...";
    }

    IEnumerator CloseAfterDelay()
    {
        yield return new WaitForSeconds(1.2f);
        Close();
        altar.TryActivate();
    }

    void ResetCirculos()
    {
        foreach (var c in circulos)
            c.color = colorApagado;
    }
}