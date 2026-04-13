using UnityEngine;

public abstract class AltarCondition : MonoBehaviour
{
    [TextArea] public string displayText;
    public abstract bool IsMet();
    public abstract string GetStatusText();
    public abstract void OnFulfill();
}