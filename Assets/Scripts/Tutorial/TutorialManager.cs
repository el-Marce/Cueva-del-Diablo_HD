// TutorialManager.cs
using UnityEngine;
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

        ActivarPaso(paso, barrera);
    }

    void ActivarPaso(TutorialStep paso, TutorialBarrier barrera)
    {
        pasoActual = paso;
        barreraActiva = barrera;

        if (paso.bloqueaAvance && barreraActiva != null)
            barreraActiva.Activar();

        tutorialPanel.Mostrar(paso.mensaje, paso.icono);
    }

    public void OnPanelConfirmado()
    {
    }

    public void CompletarTrigger(string trigger)
    {
        if (pasoActual == null) return;
        if (pasoActual.triggerDeDesbloqueo != trigger) return;

        if (barreraActiva != null)
        {
            //barreraActiva.barreraToast?.Ocultar();
            barreraActiva.Desactivar();
            barreraActiva = null;
        }
        else
        {
            foreach (TutorialBarrier b in FindObjectsByType<TutorialBarrier>(FindObjectsSortMode.None))
            {
                if (b.estaActiva)
                {
                    b.barreraToast?.Ocultar();
                    b.Desactivar();
                }
            }
        }

        unlockToast?.Mostrar();
        pasoActual = null;

        if (colaPasos.Count > 0)
        {
            var siguiente = colaPasos.Dequeue();
            ActivarPaso(siguiente.paso, siguiente.barrera);
        }
    }
}