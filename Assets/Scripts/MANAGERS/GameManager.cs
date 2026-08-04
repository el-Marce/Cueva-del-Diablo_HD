using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void NuevoJuego()
    {
        // Buscar el primer slot vacío para asignarlo como slot activo.
        // Si todos están ocupados, usa el slot 0 como fallback.
        int slotParaUsar = 0;
        for (int i = 0; i < SaveSystem.SlotCount; i++)
        {
            if (!SaveSystem.GetSlot(i).hasData)
            {
                slotParaUsar = i;
                break;
            }
        }

        CheckpointManager.Instance?.SetActiveSlot(slotParaUsar);

        if (SceneTransition.Instance != null)
            SceneTransition.Instance.TransitionTo("Cinematica", holdDuration: 5f);
        else
            SceneManager.LoadScene("Cinematica");
    }


    public void Salir()
    {
        Application.Quit();
        Debug.Log("[GameManager] Salir");
    }
}