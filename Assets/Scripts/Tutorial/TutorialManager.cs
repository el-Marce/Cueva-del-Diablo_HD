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
    static HashSet<string> pasosVistos = new HashSet<string>();

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    static bool YaVisto(TutorialStep paso) => pasosVistos.Contains(paso.name);
    static void MarcarVisto(TutorialStep paso) => pasosVistos.Add(paso.name);

    [ContextMenu("Resetear tutoriales vistos")]
    public void ResetearTutorialesVistos() => pasosVistos.Clear();

    // Comprueba si la barrera asociada a un paso ya fue desactivada de forma permanente.
    // Si tiene ID, usa TutorialBarrierState. Si se pasó la instancia directamente, la consulta.
    static bool BarreraYaDesactivada(TutorialStep paso, TutorialBarrier barrera)
    {
        if (barrera == null) return false;
        if (!string.IsNullOrEmpty(barrera.barrierID))
            return TutorialBarrierState.EstaDesactivada(barrera.barrierID);
        return false;
    }

    public void MostrarPaso(TutorialStep paso, TutorialBarrier barrera = null)
    {
        if (YaVisto(paso))
        {
            // Solo reactivar la barrera si el paso bloquea el avance
            // Y la barrera NO fue desactivada previamente en esta sesión.
            if (paso.bloqueaAvance && barrera != null && !BarreraYaDesactivada(paso, barrera))
                barrera.Activar();
            return;
        }

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

        if (paso.bloqueaAvance && barreraActiva != null && !BarreraYaDesactivada(paso, barreraActiva))
        {
            barreraActiva.Activar();
            barreraActiva.SetMensajeToast(paso.toastMensaje);
        }

        if (!paso.silencioso)
            tutorialPanel.Mostrar(
                paso.mensaje,
                paso.icono,
                paso.mensajesSecuencia,
                paso.teclasConfirmacion,
                paso.tiempoAutoCierre,
                paso.posicionAnclaje,
                paso.tamañoPanel,
                paso.tamañoTexto
            );

        MarcarVisto(paso);
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

    public void CompletarTrigger(string trigger)
    {
        if (pasoActual == null) return;
        if (pasoActual.triggerDeDesbloqueo != trigger) return;

        DesactivarBarreraActual();
        unlockToast?.Mostrar();
        pasoActual = null;

        if (colaPasos.Count > 0)
        {
            var siguiente = colaPasos.Dequeue();
            MostrarPaso(siguiente.paso, siguiente.barrera);
        }
    }

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
                if (b.estaActiva) b.Desactivar();
        }
    }

    public void MostrarToastInfo(string mensaje) => unlockToast?.Mostrar(mensaje);

    public void ActivarPasoSilencioso(TutorialStep paso, TutorialBarrier barrera = null)
    {
        if (YaVisto(paso))
        {
            // Mismo fix: respetar TutorialBarrierState antes de reactivar.
            if (paso.bloqueaAvance && barrera != null && !BarreraYaDesactivada(paso, barrera))
                barrera.Activar();
            return;
        }

        if (pasoActual != null)
        {
            colaPasos.Enqueue((paso, barrera));
            return;
        }

        pasoActual = paso;
        barreraActiva = barrera;
        triggersCompletados.Clear();

        if (paso.bloqueaAvance && barrera != null && !BarreraYaDesactivada(paso, barrera))
            barrera.Activar();

        MarcarVisto(paso);
    }
}