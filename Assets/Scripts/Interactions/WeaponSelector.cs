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

        // Encuentra el índice del arma equipada actualmente
        int currentIndex = inventory.weapons.FindIndex(w => w.name == inventory.equippedWeapon);

        // Avanza al siguiente, o vuelve a puños si estaba en la última
        if (currentIndex >= inventory.weapons.Count - 1)
        {
            inventory.equippedWeapon = null;
            combat.EquipWeapon(PlayerCombat.WeaponType.Fists, 0);
            Debug.Log("Equipado: Puños");
        }
        else
        {
            int nextIndex = currentIndex + 1;
            WeaponData next = inventory.weapons[nextIndex];
            inventory.equippedWeapon = next.name;
            combat.EquipWeapon(next.weaponType, next.durability);
            Debug.Log("Equipado: " + next.name);
        }
    }
}