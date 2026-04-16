using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int savedSceneIndex = -1; // -1 = sin partida guardada

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void NuevoJuego()
    {
        if (SceneTransition.Instance != null)
            SceneTransition.Instance.TransitionTo("Cinematica");
        else
            SceneManager.LoadScene("Cinematica");
    }

    public void CargarPartida(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void GuardarPartida()
    {
        PlayerPrefs.SetInt("SavedScene", SceneManager.GetActiveScene().buildIndex);
        PlayerPrefs.Save();
        Debug.Log("[GameManager] Partida guardada en escena: " + SceneManager.GetActiveScene().name);
    }

    public bool TienePartidaGuardada()
    {
        return PlayerPrefs.HasKey("SavedScene");
    }

    public int GetSavedSceneIndex()
    {
        return PlayerPrefs.GetInt("SavedScene", 2); // 2 = Nivel_01 como fallback
    }

    public void Salir()
    {
        Application.Quit();
        Debug.Log("[GameManager] Salir");
    }
}