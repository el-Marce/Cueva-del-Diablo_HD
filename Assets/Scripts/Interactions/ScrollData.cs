using UnityEngine;

[System.Serializable]
public class ScrollData
{
    public string name;
    public string text;
    public Sprite icon;

    public ScrollData(string text, Sprite icon, string name = "Pergamino")
    {
        this.name = name;
        this.text = text;
        this.icon = icon;
    }
}