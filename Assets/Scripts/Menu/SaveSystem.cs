using UnityEngine;

[System.Serializable]
public class SaveSlot
{
    public bool hasData;
    public int sceneIndex;
    public string sceneName;
    public string saveDate;
}

public static class SaveSystem
{
    const int SLOT_COUNT = 3;

    public static SaveSlot GetSlot(int slot)
    {
        SaveSlot data = new SaveSlot();
        data.hasData = PlayerPrefs.GetInt($"Slot{slot}_HasData", 0) == 1;
        data.sceneIndex = PlayerPrefs.GetInt($"Slot{slot}_Scene", 2);
        data.sceneName = PlayerPrefs.GetString($"Slot{slot}_Name", "");
        data.saveDate = PlayerPrefs.GetString($"Slot{slot}_Date", "");
        return data;
    }

    public static void SaveSlotData(int slot, int sceneIndex, string sceneName)
    {
        PlayerPrefs.SetInt($"Slot{slot}_HasData", 1);
        PlayerPrefs.SetInt($"Slot{slot}_Scene", sceneIndex);
        PlayerPrefs.SetString($"Slot{slot}_Name", sceneName);
        PlayerPrefs.SetString($"Slot{slot}_Date", System.DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
        PlayerPrefs.Save();
    }

    public static void DeleteSlot(int slot)
    {
        PlayerPrefs.DeleteKey($"Slot{slot}_HasData");
        PlayerPrefs.DeleteKey($"Slot{slot}_Scene");
        PlayerPrefs.DeleteKey($"Slot{slot}_Name");
        PlayerPrefs.DeleteKey($"Slot{slot}_Date");
        PlayerPrefs.Save();
    }

    public static int SlotCount => SLOT_COUNT;
}