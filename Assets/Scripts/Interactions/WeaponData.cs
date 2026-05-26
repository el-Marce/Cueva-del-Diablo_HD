using UnityEngine;

[System.Serializable]
public class WeaponData
{
    public string name;
    public int durability;
    public Sprite icon;
    public PlayerCombat.WeaponType weaponType;

    public WeaponData(string name, PlayerCombat.WeaponType weaponType, int durability)
    {
        this.name = name;
        this.weaponType = weaponType;
        this.durability = durability;
    }
}