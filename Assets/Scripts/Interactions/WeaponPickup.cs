using UnityEngine;

public class WeaponPickup : MonoBehaviour, IInteractable
{
    public PlayerCombat.WeaponType weaponType;
    public int durability = 5;

    public void Interact()
    {
        PlayerCombat player = FindObjectOfType<PlayerCombat>();

        if (player != null)
        {
            player.EquipWeapon(weaponType, durability);
        }

        Destroy(gameObject);
    }
}