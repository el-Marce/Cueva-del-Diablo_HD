using UnityEngine;
using UnityEngine.UI;

// Componente único que conecta los sonidos de UIAudio a todos los botones
// del menú principal (PanelMenu, PanelOpciones, PanelCargar, SlotPanel),
// sin tener que tocar el OnClick() de cada uno en el Inspector.
// Mismo patrón que MenuPausa: listas explícitas por tipo de sonido.
public class MenuPrincipalAudio : MonoBehaviour
{
    [Header("Sonido de botones")]
    [Tooltip("Botones que reproducen el sonido de Click normal")]
    public Button[] botonesClick;

    [Tooltip("Botones que reproducen el sonido de Back/Cancelar (ej. Volver, Cerrar)")]
    public Button[] botonesBack;

    [Tooltip("Botones que reproducen el sonido de Borrar (ej. Borrar partida)")]
    public Button[] botonesBorrar;

    [Tooltip("Botón(es) que reproducen el sonido especial de Nuevo Juego")]
    public Button[] botonesNuevoJuego;

    void Awake()
    {
        Registrar(botonesClick, () => UIAudio.Instance?.PlayClick());
        Registrar(botonesBack, () => UIAudio.Instance?.PlayBack());
        Registrar(botonesBorrar, () => UIAudio.Instance?.PlayBorrar());
        Registrar(botonesNuevoJuego, () => UIAudio.Instance?.PlayNewGame());
    }

    void Registrar(Button[] botones, UnityEngine.Events.UnityAction accion)
    {
        if (botones == null) return;

        foreach (Button btn in botones)
        {
            if (btn == null) continue;
            btn.onClick.AddListener(accion);
        }
    }
}