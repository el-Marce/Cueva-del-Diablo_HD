using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawnUpdater : MonoBehaviour
{
    public float updateInterval = 60f;
    float timer = 0f;
    Transform player;

    static Vector3 savedPosition;
    static Quaternion savedRotation;
    static bool hasSavedPosition = false;

    public static bool zonaRestringida = false; // activado desde la zona

    void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        timer = 0f;
        zonaRestringida = false;

        if (hasSavedPosition)
            transform.SetPositionAndRotation(savedPosition, savedRotation);
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (hasSavedPosition)
            transform.SetPositionAndRotation(savedPosition, savedRotation);
    }

    void Update()
    {
        if (player == null) return;
        if (GameState.InMenu) return;
        if (zonaRestringida) return; // no actualiza en zona restringida

        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            timer = 0f;
            savedPosition = player.position;
            savedRotation = player.rotation;
            hasSavedPosition = true;
            transform.SetPositionAndRotation(savedPosition, savedRotation);
        }
    }
}