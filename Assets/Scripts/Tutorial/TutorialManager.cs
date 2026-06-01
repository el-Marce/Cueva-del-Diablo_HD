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

        if (paso.bloqueaAvance && barreraActiva != null)
            barreraActiva.Activar();

        tutorialPanel.Mostrar(paso.mensaje, paso.icono, paso.mensajesSecuencia);
    }

    public void OnPanelConfirmado()
    {
        // Si el paso requiere una acción externa para completarse,
        // no limpiar pasoActual — CompletarTrigger lo hará cuando corresponda
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

    public void CompletarTrigger(string trigger)
    {
        if (pasoActual == null) return;
        if (pasoActual.triggerDeDesbloqueo != trigger) return;

        if (barreraActiva != null)
        {
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
            MostrarPaso(siguiente.paso, siguiente.barrera);
        }
    }
}