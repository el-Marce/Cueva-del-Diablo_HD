using UnityEngine;

// Esqueleto listo para implementar cuando lo necesites
public class AltarCondition_RhythmChallenge : AltarCondition
{
    [TextArea] public string riddleText = "El ritmo de las campanas abre los caminos";
    bool solved = false;

    public override bool IsMet() => solved;

    public override string GetStatusText() => riddleText;

    public override void OnFulfill() { }

    // Llama esto desde tu sistema de ritmo cuando el jugador complete el patrón
    public void SolveChallenge()
    {
        solved = true;
        Debug.Log("[Reto] Completado: " + riddleText);
    }
}