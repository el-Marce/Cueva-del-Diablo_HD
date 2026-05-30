using UnityEngine;

public class WeaponPickup : MonoBehaviour, IInteractable
{
    public PlayerCombat.WeaponType weaponType;
    public int durability = 5;
    public string weaponName = "Arma";
    public Sprite icon;

    [Header("Tutorial (opcional)")]
    public TutorialStep tutorialStep;
    public TutorialBarrier barreraTutorial;
    public void Interact()
    {
        Inventory inventory = FindObjectOfType<Inventory>();
        if (inventory != null)
            inventory.AddWeapon(weaponName, weaponType, icon, durability);

        Destroy(gameObject);

        //PlayerCombat player = FindObjectOfType<PlayerCombat>();
        //if (player != null)
        //    player.EquipWeapon(weaponType, durability);

        // Activar tutorial al recoger
        TutorialManager.Instance?.MostrarPaso(tutorialStep, barreraTutorial);
    }
}