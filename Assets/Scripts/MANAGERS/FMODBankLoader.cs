using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;
using FMOD.Studio;
using System.Collections.Generic;

public class FMODBankLoader : MonoBehaviour
{
    public static FMODBankLoader Instance;

    [System.Serializable]
    public class SceneBanks
    {
        public string sceneName;
        public string[] bankNames;
    }

    [Header("Configuración")]
    public SceneBanks[] sceneBanks;

    List<string> loadedBanks = new List<string>();

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UnloadCurrentBanks();
        LoadBanksForScene(scene.name);
    }

    void LoadBanksForScene(string sceneName)
    {
        foreach (var entry in sceneBanks)
        {
            if (entry.sceneName != sceneName) continue;

            foreach (var bankName in entry.bankNames)
            {
                RuntimeManager.LoadBank(bankName, true);
                loadedBanks.Add(bankName);
                Debug.Log("[FMOD] Cargado: " + bankName);
            }
        }
    }

    void UnloadCurrentBanks()
    {
        foreach (var bankName in loadedBanks)
        {
            RuntimeManager.UnloadBank(bankName);
            Debug.Log("[FMOD] Descargado: " + bankName);
        }
        loadedBanks.Clear();
    }
}