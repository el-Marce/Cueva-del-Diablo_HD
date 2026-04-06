using UnityEngine;

// Clase base que cualquier condición hereda
public abstract class AltarCondition : MonoBehaviour
{
    [TextArea] public string displayText; // "Agua Bendita" / "Almas a ofrecer: X"
    public abstract bool IsMet();
    public abstract string GetStatusText(); // texto dinámico para la UI
    public abstract void OnFulfill();       // qué hacer al cumplirse (consumir item, etc.)
}