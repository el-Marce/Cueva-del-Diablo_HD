using System.Collections.Generic;
using UnityEngine;

// Todas las clases de este archivo son puramente serializables a JSON.
// No tienen dependencias de Unity Engine salvo tipos básicos (Vector3, Quaternion).

[System.Serializable]
public class CheckpointData
{
    // --- Escena ---
    public int sceneIndex;
    public string sceneName;

    // --- Posición del jugador ---
    public SerializableVector3 playerPosition;
    public SerializableQuaternion playerRotation;

    // --- Inventario ---
    public List<SavedItem> items = new List<SavedItem>();
    public List<SavedWeapon> weapons = new List<SavedWeapon>();
    public List<SavedScroll> scrolls = new List<SavedScroll>();
    public string equippedWeapon;

    // --- Estado del mundo ---
    public List<string> activatedAltars = new List<string>();  // "NombreGameObject"
    public List<SavedDoor> doors = new List<SavedDoor>();

    // --- Pickups recogidos (se destruyen al restaurar para evitar duplicados) ---
    public List<string> pickedUpIds = new List<string>();

    // --- Opciones ---
    public bool respawnEnemigos = false;

    // --- Slot de guardado asociado (-1 = sin slot) ---
    public int saveSlot = -1;
    public string saveDate;
}

[System.Serializable]
public class SavedItem
{
    public string name;
    public int uses;
}

[System.Serializable]
public class SavedWeapon
{
    public string name;
    public int weaponType; // cast desde PlayerCombat.WeaponType
    public int durability;
}

[System.Serializable]
public class SavedScroll
{
    public string name;
    public string text;
}

[System.Serializable]
public class SavedDoor
{
    public string objectName; // nombre del GameObject de la puerta
    public bool isOpen;
    public bool isLocked;
}

// Vector3 serializable a JSON (JsonUtility no serializa Vector3 directamente en listas anidadas)
[System.Serializable]
public class SerializableVector3
{
    public float x, y, z;

    public SerializableVector3(Vector3 v) { x = v.x; y = v.y; z = v.z; }
    public Vector3 ToVector3() => new Vector3(x, y, z);
}

[System.Serializable]
public class SerializableQuaternion
{
    public float x, y, z, w;

    public SerializableQuaternion(Quaternion q) { x = q.x; y = q.y; z = q.z; w = q.w; }
    public Quaternion ToQuaternion() => new Quaternion(x, y, z, w);
}