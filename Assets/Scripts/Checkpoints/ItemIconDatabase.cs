using System.Collections.Generic;
using UnityEngine;

// ScriptableObject que mapea nombre de item/arma/pergamino a su Sprite.
// Crear via: Assets > Create > Checkpoint > ItemIconDatabase
// Luego asignar en el Inspector del CheckpointManager.
[CreateAssetMenu(fileName = "ItemIconDatabase", menuName = "Checkpoint/ItemIconDatabase")]
public class ItemIconDatabase : ScriptableObject
{
    [System.Serializable]
    public struct IconEntry
    {
        public string itemName;
        public Sprite icon;
    }

    public List<IconEntry> entries = new List<IconEntry>();

    // Devuelve el sprite correspondiente al nombre, o null si no existe.
    public Sprite GetIcon(string itemName)
    {
        foreach (var entry in entries)
            if (entry.itemName == itemName)
                return entry.icon;
        return null;
    }
}
