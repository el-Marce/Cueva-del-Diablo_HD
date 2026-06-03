using UnityEngine;

public class WeaponSelector : MonoBehaviour
{
    Inventory inventory;
    PlayerCombat combat;

    void Start()
    {
        inventory = GetComponent<Inventory>();
        combat = GetComponent<PlayerCombat>();
    }

    void Update()
    {
        if (GameState.InMenu) return;
        if (Input.GetKeyDown(KeyCode.F))
            CycleWeapon();
    }

    void CycleWeapon()
    {
        if (inventory.weapons.Count == 0)
        {
            combat.EquipWeapon(PlayerCombat.WeaponType.Fists, 0);
            inventory.equippedWeapon = null;
            return;
        }

        int currentIndex = inventory.weapons.FindIndex(w => w.name == inventory.equippedWeapon);

        if (currentIndex >= inventory.weapons.Count - 1)
        {
            inventory.equippedWeapon = null;
            combat.EquipWeapon(PlayerCombat.WeaponType.Fists, 0);
        }
        else
        {
            int nextIndex = currentIndex + 1;
            WeaponData next = inventory.weapons[nextIndex];
            inventory.equippedWeapon = next.name;
            combat.EquipWeapon(next.weaponType, next.durability);
            inventory.NotificarArmaEquipada(next.name); // notifica al tutorial
        }
    }
}