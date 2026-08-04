using UnityEngine;

// Componente que va en el objeto físico del checkpoint (fogata, altar, bandera, etc.)
// El jugador interactúa con él para guardar el progreso.
// Implementa IInteractable igual que Door y AltarRitual_Generic.
public class Checkpoint : MonoBehaviour, IInteractable
{
    [Header("Feedback visual (opcional)")]
    [Tooltip("Objeto o partícula que se activa cuando el checkpoint está guardado")]
    public GameObject activatedVisual;

    [Header("Feedback de texto (opcional)")]
    [Tooltip("Texto que aparece al jugador al activar el checkpoint")]
    public string mensajeActivacion = "Progreso guardado";

    bool activated = false;

    void Start()
    {
        if (activatedVisual != null)
            activatedVisual.SetActive(false);
    }

    public void Interact()
    {
        if (CheckpointManager.Instance == null)
        {
            Debug.LogWarning("[Checkpoint] No hay CheckpointManager en la escena.");
            return;
        }

        // Obtener posición del jugador
        GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
        if (playerGO == null)
        {
            Debug.LogWarning("[Checkpoint] No se encontró el jugador.");
            return;
        }

        CheckpointManager.Instance.GuardarCheckpoint(
            playerGO.transform.position,
            playerGO.transform.rotation
        );

        // Feedback visual
        if (activatedVisual != null)
            activatedVisual.SetActive(true);

        activated = true;

        // Aquí puedes mostrar un mensaje al jugador vía tu sistema de UI existente
        Debug.Log("[Checkpoint] " + mensajeActivacion);
    }
}
