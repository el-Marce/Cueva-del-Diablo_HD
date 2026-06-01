using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [Header("Panel principal")]
    public TutorialPanel tutorialPanel;

    [Header("Toast de desbloqueo")]
    public TutorialUnlockToast unlockToast;

    public TutorialStep pasoActual;
    TutorialBarrier barreraActiva;
    Queue<(TutorialStep paso, TutorialBarrier barrera)> colaPasos = new();
    List<string> triggersCompletados = new();

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void MostrarPaso(TutorialStep paso, TutorialBarrier barrera = null)
    {
        if (pasoActual != null)
        {
            colaPasos.Enqueue((paso, barrera));
            return;
        }

        if (paso.delayInicio > 0f)
            StartCoroutine(MostrarConDelay(paso, barrera));
        else
            ActivarPaso(paso, barrera);
    }

    IEnumerator MostrarConDelay(TutorialStep paso, TutorialBarrier barrera)
    {
        yield return new WaitForSeconds(paso.delayInicio);
        ActivarPaso(paso, barrera);
    }

    void ActivarPaso(TutorialStep paso, TutorialBarrier barrera)
    {
        pasoActual = paso;
        barreraActiva = barrera;
        triggersCompletados.Clear();

        if (paso.bloqueaAvance && barreraActiva != null)
            barreraActiva.Activar();

        if (!paso.silencioso)
            tutorialPanel.Mostrar(paso.mensaje, paso.icono, paso.mensajesSecuencia);
        else
            OnPanelConfirmado(); // si no hay panel, avanza automáticamente si no bloquea
    }
    public void OnPanelConfirmado()
    {
        if (pasoActual != null && pasoActual.bloqueaAvance)
            return;

        pasoActual = null;
        barreraActiva = null;

        if (colaPasos.Count > 0)
        {
            var siguiente = colaPasos.Dequeue();
            MostrarPaso(siguiente.paso, siguiente.barrera);
        }
    }

    // Para steps con un solo trigger
    public void CompletarTrigger(string trigger)
    {
        if (pasoActual == null) return;
        if (pasoActual.triggerDeDesbloqueo != trigger) return;

        DesactivarBarreraActual();
        unlockToast?.Mostrar();  // siempre
        pasoActual = null;

        if (colaPasos.Count > 0)
        {
            var siguiente = colaPasos.Dequeue();
            MostrarPaso(siguiente.paso, siguiente.barrera);
        }
    }

    // Para steps que requieren múltiples acciones
    public void CompletarTriggerParcial(string trigger)
    {
        if (pasoActual == null) return;
        if (pasoActual.triggersRequeridos == null || pasoActual.triggersRequeridos.Length == 0) return;

        if (!triggersCompletados.Contains(trigger))
            triggersCompletados.Add(trigger);

        foreach (string t in pasoActual.triggersRequeridos)
            if (!triggersCompletados.Contains(t)) return;

        triggersCompletados.Clear();
        DesactivarBarreraActual();
        pasoActual = null;

        if (colaPasos.Count > 0)
        {
            var siguiente = colaPasos.Dequeue();
            MostrarPaso(siguiente.paso, siguiente.barrera);
        }
        else
        {
            unlockToast?.Mostrar();
        }
    }

    void DesactivarBarreraActual()
    {
        if (barreraActiva != null)
        {
            barreraActiva.Desactivar();
            barreraActiva = null;
        }
        else
        {
            foreach (TutorialBarrier b in FindObjectsByType<TutorialBarrier>(FindObjectsSortMode.None))
                if (b.estaActiva) {/* b.barreraToast?.Ocultar(); */b.Desactivar(); }
        }
    }

    public void MostrarToastInfo(string mensaje)
    {
        unlockToast?.Mostrar(mensaje);
    }
    public void ActivarPasoSilencioso(TutorialStep paso, TutorialBarrier barrera = null)
    {
        if (pasoActual != null)
        {
            colaPasos.Enqueue((paso, barrera));
            return;
        }

        pasoActual = paso;
        barreraActiva = barrera;
        triggersCompletados.Clear();

        if (paso.bloqueaAvance && barrera != null)
            barrera.Activar();
        // No llama a tutorialPanel.Mostrar()
    }
}