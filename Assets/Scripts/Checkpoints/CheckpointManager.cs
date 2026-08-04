using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using static PlayerCombat;

// Singleton persistente entre escenas. Gestiona:
// - Guardar el estado completo del juego al activar un checkpoint
// - Restaurar ese estado al morir o cargar partida
// - Integrarse con el SaveSystem de slots del menú principal
public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;

    [Header("Base de datos de iconos")]
    public ItemIconDatabase iconDatabase;

    [Header("Opciones")]
    [Tooltip("Si true, los enemigos vuelven a aparecer al cargar desde checkpoint")]
    public bool respawnEnemigos = false;

    // Slot activo en esta sesión (-1 = sin slot asociado)
    int activeSlot = -1;

    // Datos del último checkpoint guardado en memoria (para restaurar sin leer disco).
    // Se inicializa vacío en Awake para que RegistrarPickup funcione incluso antes
    // de que el jugador active su primer checkpoint.
    CheckpointData pendingRestore = new CheckpointData();
    bool shouldRestore = false;

    static string SavePath(int slot) =>
        Path.Combine(Application.persistentDataPath, $"checkpoint_slot{slot}.json");

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Llamado por GameManager o MenuPrincipal al cargar una partida desde un slot.
    // Debe llamarse ANTES de cargar la escena.
    public void PrepararCargaDesdeSlot(int slot)
    {
        activeSlot = slot;
        shouldRestore = true;
        pendingRestore = CargarDesdeArchivo(slot);
    }

    // Llamado por Checkpoint.cs cuando el jugador activa un checkpoint físico.
    public void GuardarCheckpoint(Vector3 playerPos, Quaternion playerRot)
    {
        Inventory inventory = FindObjectOfType<Inventory>();
        if (inventory == null)
        {
            Debug.LogWarning("[CheckpointManager] No se encontró Inventory en la escena.");
            return;
        }

        // Preservar los pickups recogidos antes de este checkpoint
        List<string> pickupsAcumulados = pendingRestore.pickedUpIds;
        CheckpointData data = new CheckpointData();
        data.pickedUpIds.AddRange(pickupsAcumulados);

        // Escena
        data.sceneIndex = SceneManager.GetActiveScene().buildIndex;
        data.sceneName = SceneManager.GetActiveScene().name;

        // Posición
        data.playerPosition = new SerializableVector3(playerPos);
        data.playerRotation = new SerializableQuaternion(playerRot);

        // Inventario
        foreach (var item in inventory.items)
            data.items.Add(new SavedItem { name = item.name, uses = item.uses });

        foreach (var weapon in inventory.weapons)
            data.weapons.Add(new SavedWeapon
            {
                name = weapon.name,
                weaponType = (int)weapon.weaponType,
                durability = weapon.durability
            });

        foreach (var scroll in inventory.scrolls)
            data.scrolls.Add(new SavedScroll { text = scroll.text, name = scroll.name });

        data.equippedWeapon = inventory.equippedWeapon;

        // Altares activados
        foreach (var altar in FindObjectsOfType<AltarRitual_Generic>())
        {
            // Accedemos al campo activated via reflexión mínima: usamos el nombre del GO
            // como clave y preguntamos si ya no es interactable (collider desactivado).
            Collider col = altar.GetComponent<Collider>();
            if (col != null && !col.enabled)
                data.activatedAltars.Add(altar.gameObject.name);
        }

        // Puertas
        foreach (var door in FindObjectsOfType<Door>())
        {
            data.doors.Add(new SavedDoor
            {
                objectName = door.gameObject.name,
                isOpen = door.isOpen,
                isLocked = door.isLocked
            });
        }

        // Opciones
        data.respawnEnemigos = respawnEnemigos;

        // Slot y fecha
        data.saveSlot = activeSlot;
        data.saveDate = System.DateTime.Now.ToString("dd/MM/yyyy HH:mm");

        // Guardar en archivo JSON
        if (activeSlot >= 0)
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(SavePath(activeSlot), json);

            // Actualizar también el SaveSystem del menú principal para que
            // el slot muestre la escena y fecha correctas en la pantalla de carga.
            SaveSystem.SaveSlotData(activeSlot, data.sceneIndex, data.sceneName);

            Debug.Log($"[CheckpointManager] Checkpoint guardado en slot {activeSlot}: {SavePath(activeSlot)}");
        }

        // Guardar en memoria como pendingRestore por si el jugador muere en esta sesión
        pendingRestore = data;
    }

    // Llamado al morir: recarga la escena del checkpoint y restaura el estado.
    public void RespawnDesdeCheckpoint()
    {
        if (pendingRestore == null)
        {
            Debug.LogWarning("[CheckpointManager] No hay checkpoint guardado en esta sesión.");
            return;
        }

        shouldRestore = true;
        Time.timeScale = 1f;
        GameState.InMenu = false;
        SceneManager.LoadScene(pendingRestore.sceneIndex);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!shouldRestore || pendingRestore == null) return;
        shouldRestore = false;

        // Espera un frame antes de restaurar para que todos los Awake/Start
        // de la escena nueva hayan terminado.
        StartCoroutine(RestaurarEstado(pendingRestore));
    }

    System.Collections.IEnumerator RestaurarEstado(CheckpointData data)
    {
        yield return null; // esperar un frame

        // --- Jugador ---
        GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
        if (playerGO != null)
        {
            CharacterController cc = playerGO.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            playerGO.transform.SetPositionAndRotation(
                data.playerPosition.ToVector3(),
                data.playerRotation.ToQuaternion()
            );

            if (cc != null) cc.enabled = true;
        }

        // --- Inventario ---
        Inventory inventory = FindObjectOfType<Inventory>();
        if (inventory != null)
        {
            inventory.items.Clear();
            inventory.weapons.Clear();
            inventory.scrolls.Clear();
            inventory.equippedWeapon = data.equippedWeapon;

            foreach (var saved in data.items)
            {
                Sprite icon = iconDatabase != null ? iconDatabase.GetIcon(saved.name) : null;
                inventory.AddItem(saved.name, icon, saved.uses);
            }

            foreach (var saved in data.weapons)
            {
                Sprite icon = iconDatabase != null ? iconDatabase.GetIcon(saved.name) : null;
                inventory.AddWeapon(saved.name, (WeaponType)saved.weaponType, icon, saved.durability);
            }

            foreach (var saved in data.scrolls)
            {
                Sprite icon = iconDatabase != null ? iconDatabase.GetIcon(saved.name) : null;
                inventory.AddScroll(saved.text, icon, saved.name);
            }
        }

        // --- Altares activados ---
        foreach (var altar in FindObjectsOfType<AltarRitual_Generic>())
        {
            if (data.activatedAltars.Contains(altar.gameObject.name))
            {
                // Reproducir el estado final del altar: collider propio desactivado
                Collider col = altar.GetComponent<Collider>();
                if (col != null) col.enabled = false;

                // Activar el collider del hijo (igual que en ActivationSequence)
                if (altar.transform.childCount > 0)
                {
                    Collider childCol = altar.transform.GetChild(0).GetComponent<Collider>();
                    if (childCol != null) childCol.enabled = true;
                }

                // Abrir la puerta asociada si la tiene
                if (altar.door != null && !altar.door.isOpen)
                    altar.door.isLocked = false;
            }
        }

        // --- Puertas ---
        foreach (var door in FindObjectsOfType<Door>())
        {
            SavedDoor saved = data.doors.Find(d => d.objectName == door.gameObject.name);
            if (saved == null) continue;

            door.isLocked = saved.isLocked;

            if (saved.isOpen && !door.isOpen)
                door.OpenDoor();
        }

        // --- Enemigos ---
        // Si respawnEnemigos es false, los enemigos de la escena se destruyen.
        // Si es true, se dejan como están (aparecen de nuevo al recargar la escena).
        if (!data.respawnEnemigos)
        {
            // Ajusta los tags/tipos según tus enemigos reales.
            // Por ahora destruye todo lo que sea EntePsicologico o Pueblerino.
            foreach (var ente in FindObjectsOfType<EntePsicologico>())
                Destroy(ente.gameObject);

            // Si tienes otros tipos de enemigos, agrégalos aquí.
        }

        // --- Pickups ya recogidos ---
        // Destruye los objetos que el jugador ya recogió antes del checkpoint
        // para evitar que reaparezcan al recargar la escena.
        foreach (string pickupId in data.pickedUpIds)
        {
            GameObject obj = FindByPath(pickupId);
            if (obj != null)
                Destroy(obj);
        }

        Debug.Log("[CheckpointManager] Estado restaurado desde checkpoint.");
    }

    // Carga los datos del checkpoint desde archivo JSON del slot indicado.
    public CheckpointData CargarDesdeArchivo(int slot)
    {
        string path = SavePath(slot);
        if (!File.Exists(path))
        {
            Debug.LogWarning($"[CheckpointManager] No existe archivo de checkpoint para slot {slot}.");
            return null;
        }

        string json = File.ReadAllText(path);
        CheckpointData data = JsonUtility.FromJson<CheckpointData>(json);
        return data;
    }

    // Busca un GameObject en la escena por su ruta completa en la jerarquía.
    GameObject FindByPath(string path)
    {
        string[] parts = path.Split('/');
        GameObject[] roots = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();

        foreach (GameObject root in roots)
        {
            if (root.name != parts[0]) continue;
            if (parts.Length == 1) return root;

            Transform current = root.transform;
            bool found = true;
            for (int i = 1; i < parts.Length; i++)
            {
                Transform child = current.Find(parts[i]);
                if (child == null) { found = false; break; }
                current = child;
            }
            if (found) return current.gameObject;
        }
        return null;
    }

    // Devuelve el slot activo en esta sesión.
    public int GetActiveSlot() => activeSlot;

    // Permite al menú de pausa o sistema externo establecer el slot activo.
    public void SetActiveSlot(int slot) => activeSlot = slot;

    // Llamado por PickupItem/WeaponPickup/Pergamino al ser recogidos.
    // Registra el ID del pickup en el checkpoint actual para que no reaparezca al restaurar.
    public void RegistrarPickup(string pickupId)
    {
        if (pendingRestore == null) return;
        if (!pendingRestore.pickedUpIds.Contains(pickupId))
            pendingRestore.pickedUpIds.Add(pickupId);
    }
}