using UnityEngine;
using System.Collections.Generic;

public class TutorialBarrierState : MonoBehaviour
{
    public static TutorialBarrierState Instance;

    static HashSet<string> barrerasDesactivadas = new HashSet<string>();

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public static void RegistrarDesactivada(string id) => barrerasDesactivadas.Add(id);
    public static bool EstaDesactivada(string id) => barrerasDesactivadas.Contains(id);

    [ContextMenu("Resetear barreras")]
    public void Resetear() => barrerasDesactivadas.Clear();
}