using UnityEngine;
using System.Collections.Generic;

public class TutorialBarrierState : MonoBehaviour
{
    public static TutorialBarrierState Instance;

    // HashSet en memoria: persiste entre recargas de escena en la misma sesión,
    // se pierde al cerrar el juego. Exactamente el comportamiento deseado.
    static HashSet<string> barrerasDesactivadas = new HashSet<string>();

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject); // <-- asegura que sobrevive recargas de escena
    }

    public static void RegistrarDesactivada(string id) => barrerasDesactivadas.Add(id);
    public static bool EstaDesactivada(string id) => barrerasDesactivadas.Contains(id);

    // Resetea solo las barreras (no los tutoriales vistos).
    // Útil si se inicia una partida nueva desde el menú principal.
    [ContextMenu("Resetear barreras")]
    public void Resetear() => barrerasDesactivadas.Clear();

    // Resetea TODO el estado de tutoriales: barreras + pasos vistos.
    // Llamar desde GameManager.NuevoJuego() para partidas nuevas.
    [ContextMenu("Resetear todo (barreras + tutoriales)")]
    public void ResetearTodo()
    {
        barrerasDesactivadas.Clear();
        TutorialManager.Instance?.ResetearTutorialesVistos();
    }
}
