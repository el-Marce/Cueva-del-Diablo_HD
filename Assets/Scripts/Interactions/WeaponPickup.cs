using UnityEngine;

public class WeaponPickup : MonoBehaviour, IInteractable
{
    public PlayerCombat.WeaponType weaponType;
    public int durability = 5;
    public string weaponName = "Arma";
    public Sprite icon;

    public void Interact()
    {
        Inventory inventory = FindObjectOfType<Inventory>();
        if (inventory != null)
            inventory.AddWeapon(weaponName, weaponType, icon, durability);
        Destroy(gameObject);
    }
}