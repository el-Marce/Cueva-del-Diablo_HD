using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    //public List<string> items = new List<string>();
    public List<ItemData> items = new List<ItemData>(); 
    public List<ScrollData> scrolls = new List<ScrollData>();
    public List<WeaponData> weapons = new List<WeaponData>();
    public string equippedWeapon = null;
    public string equippedItem = null;

    public int selectedIndex = 0;

    public enum Tab
    {
        Items,
        Scrolls,
        Weapons
    }

    public Tab currentTab = Tab.Items;

    public void AddItem(string itemName, Sprite icon, int uses = 1)
    {
        // Si ya existe, suma usos en lugar de duplicar
        ItemData existing = items.Find(i => i.name == itemName);
        if (existing != null)
        {
            existing.uses += uses;
            Debug.Log("Recogiste más " + itemName + ". Usos totales: " + existing.uses);
            return;
        }
        ItemData newItem = new ItemData(itemName, uses);
        newItem.icon = icon;

        items.Add(newItem);
        Debug.Log("Recogiste: " + itemName + " (" + uses + " usos). Pulsa TAB para ver tus objetos");
    }

    public void AddScroll(string scrollText, Sprite icon)
    {
        scrolls.Add(new ScrollData(scrollText, icon));
        Debug.Log("Pergamino guardado. Pulsa TAB para leerlo");
    }
    public void AddWeapon(string weaponName, PlayerCombat.WeaponType type, Sprite icon, int durability)
    {
        WeaponData existing = weapons.Find(w => w.name == weaponName);
        if (existing != null)
        {
            existing.durability += durability;
            return;
        }
        WeaponData newWeapon = new WeaponData(weaponName, type, durability);
        newWeapon.icon = icon;
        weapons.Add(newWeapon);
        Debug.Log("Arma recogida: " + weaponName);
    }
    public bool HasItem(string itemName)
    {
        return items.Exists(i => i.name == itemName);
    }

    public string GetSelected()
    {
        if (currentTab == Tab.Items) return items[selectedIndex].name;
        if (currentTab == Tab.Scrolls) return scrolls[selectedIndex].text;
        return weapons[selectedIndex].name;
    }
    public int GetCount()
    {
        if (currentTab == Tab.Items) return items.Count;
        if (currentTab == Tab.Scrolls) return scrolls.Count;
        return weapons.Count;
    }

    public void RemoveItem(string itemName)
    {
        ItemData item = items.Find(i => i.name == itemName);
        if (item == null) return;

        item.uses--;
        Debug.Log("Usaste: " + itemName + " | Usos restantes: " + item.uses);

        if (item.uses <= 0)
        {
            items.Remove(item);
            Debug.Log(itemName + " agotado");
        }
    }
    public int GetItemUses(string itemName)
    {
        ItemData item = items.Find(i => i.name == itemName);
        return item != null ? item.uses : 0;
    }
    public void UseWeaponDurability(string weaponName)
    {
        WeaponData weapon = weapons.Find(w => w.name == weaponName);
        if (weapon == null) return;
        weapon.durability--;
        if (weapon.durability <= 0)
        {
            if (equippedWeapon == weaponName) equippedWeapon = null;
            weapons.Remove(weapon);
            Debug.Log(weaponName + " rota");
        }
    }
    public void EquipWeaponSelected()
    {
        if (currentTab != Tab.Weapons || weapons.Count == 0) return;

        string selected = weapons[selectedIndex].name;
        equippedWeapon = (equippedWeapon == selected) ? null : selected;

        PlayerCombat combat = FindObjectOfType<PlayerCombat>();
        if (combat == null) return;

        if (equippedWeapon != null)
        {
            WeaponData w = weapons.Find(x => x.name == equippedWeapon);
            if (w != null)
                combat.EquipWeapon(w.weaponType, w.durability);
        }
        else
        {
            combat.EquipWeapon(PlayerCombat.WeaponType.Fists, 0);
        }
    }
    public void EquipSelected()
    {
        if (currentTab == Tab.Weapons)
            EquipWeaponSelected();
    }
}