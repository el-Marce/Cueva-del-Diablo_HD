using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Manager del menú de pausa. Se abre/cierra con Escape, congela el juego
// con Time.timeScale = 0, y permite navegar a Opciones o salir al menú principal.
public class MenuPausa : MonoBehaviour
{
    [Header("Paneles")]
    public GameObject panelPausa;
    public GameObject panelOpcionesPausa;
    public GameObject panelConfirmarSalida;

    [Header("Escena del menú principal")]
    public string escenaMenuPrincipal = "MenuPrincipal";

    [Header("Sonido de botones")]
    [Tooltip("Botones que reproducen el sonido de Click normal")]
    public Button[] botonesClick;
    [Tooltip("Botones que reproducen el sonido de Back/Cancelar (ej. Volver, Cancelar)")]
    public Button[] botonesBack;

    bool pausaAbierta = false;

    void Start()
    {
        if (panelPausa != null) panelPausa.SetActive(false);
        if (panelOpcionesPausa != null) panelOpcionesPausa.SetActive(false);
        if (panelConfirmarSalida != null) panelConfirmarSalida.SetActive(false);

        RegistrarSonidoClicEnBotones();
    }

    // Conecta el sonido correspondiente (Click o Back) a los botones
    // asignados en el Inspector, sin tener que tocar el OnClick() de cada uno ahí.
    void RegistrarSonidoClicEnBotones()
    {
        if (botonesClick != null)
        {
            foreach (Button btn in botonesClick)
            {
                if (btn == null) continue;
                btn.onClick.AddListener(() =>
                {
                    if (UIAudio.Instance != null)
                        UIAudio.Instance.PlayClick();
                });
            }
        }

        if (botonesBack != null)
        {
            foreach (Button btn in botonesBack)
            {
                if (btn == null) continue;
                btn.onClick.AddListener(() =>
                {
                    if (UIAudio.Instance != null)
                        UIAudio.Instance.PlayBack();
                });
            }
        }
    }

    void Update()
    {
        if (panelPausa == null) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // No abrir/cerrar pausa si hay otro menú usando InMenu (ej. inventario o lectura de pergamino)
            if (!pausaAbierta && GameState.InMenu) return;

            // Si el panel de confirmación de salida está activo, Escape vuelve a pausa.
            if (pausaAbierta && panelConfirmarSalida != null && panelConfirmarSalida.activeSelf)
            {
                VolverAlPausa();
                return;
            }

            // Si la pausa está abierta pero el panel de opciones está activo,
            // Escape primero vuelve al panel de pausa en lugar de cerrar todo.
            if (pausaAbierta && panelOpcionesPausa != null && panelOpcionesPausa.activeSelf)
            {
                VolverAlPausa();
                return;
            }

            if (pausaAbierta)
                Reanudar();
            else
                Pausar();
        }
    }

    void Pausar()
    {
        pausaAbierta = true;
        GameState.InMenu = true;
        Time.timeScale = 0f;
        ShowCursor(true);

        panelPausa.SetActive(true);
        if (panelOpcionesPausa != null)
            panelOpcionesPausa.SetActive(false);
    }

    public void Reanudar()
    {
        pausaAbierta = false;
        GameState.InMenu = false;
        Time.timeScale = 1f;
        ShowCursor(false);

        panelPausa.SetActive(false);
        if (panelOpcionesPausa != null)
            panelOpcionesPausa.SetActive(false);
    }

    void ShowCursor(bool show)
    {
        Cursor.visible = show;
        Cursor.lockState = show ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public void OnOpciones()
    {
        panelPausa.SetActive(false);
        if (panelOpcionesPausa != null)
            panelOpcionesPausa.SetActive(true);
    }

    public void VolverAlPausa()
    {
        if (panelOpcionesPausa != null)
            panelOpcionesPausa.SetActive(false);
        if (panelConfirmarSalida != null)
            panelConfirmarSalida.SetActive(false);
        panelPausa.SetActive(true);
    }

    public void OnSalirAlMenu()
    {
        // Abre el panel de confirmación en vez de salir directamente.
        panelPausa.SetActive(false);
        if (panelConfirmarSalida != null)
            panelConfirmarSalida.SetActive(true);
    }

    public void ConfirmarSalidaAlMenu()
    {
        // Restaurar timeScale ANTES de cargar la escena del menú,
        // o el menú principal quedaría congelado.
        Time.timeScale = 1f;
        pausaAbierta = false;
        GameState.InMenu = false;
        ShowCursor(true);

        // El Player es persistente entre niveles, pero no debe seguir
        // activo en el menú principal (controla cámara/cursor y bloquea la UI).
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            Destroy(player);

        SceneManager.LoadScene(escenaMenuPrincipal);
    }

    public void CancelarSalidaAlMenu()
    {
        VolverAlPausa();
    }
}