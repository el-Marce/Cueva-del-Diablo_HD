using UnityEngine;

[System.Serializable]
public class ScrollData
{
    public string text;
    public Sprite icon;

    public ScrollData(string text, Sprite icon)
    {
        this.text = text;
        this.icon = icon;
    }
}