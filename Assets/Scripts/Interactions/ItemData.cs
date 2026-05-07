using UnityEngine;
[System.Serializable]
public class ItemData
{
    public string name;
    public int uses;
    public Sprite icon;
    public ItemData(string name, int uses = 1)
    {
        this.name = name;
        this.uses = uses;
    }
}