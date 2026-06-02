using UnityEngine;

public abstract class AltarCondition : MonoBehaviour
{
    [Header("UI")]
    public Sprite inactiveIcon;
    public Sprite activeIcon;
    public bool showIcon = true;

    [TextArea] public string displayText;
    public abstract bool IsMet();
    public abstract string GetStatusText();
    public abstract void OnFulfill();
}