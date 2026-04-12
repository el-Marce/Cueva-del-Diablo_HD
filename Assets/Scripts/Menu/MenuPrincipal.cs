using UnityEngine;

public class MenuPrincipal : MonoBehaviour
{
    [Header("Paneles")]
    public GameObject panelMenu;
    public GameObject panelOpciones;
    public GameObject panelCargar;

    void Start()
    {
        VolverAlMenu();
    }

    public void OnNuevoJuego() => GameManager.Instance.NuevoJuego();

    public void OnCargarPartida()
    {
        panelMenu.SetActive(false);
        panelCargar.SetActive(true);
        panelCargar.GetComponent<PanelCargar>().Refresh();
    }

    public void OnOpciones()
    {
        panelMenu.SetActive(false);
        panelOpciones.SetActive(true);
    }

    public void OnSalir() => GameManager.Instance.Salir();

    public void VolverAlMenu()
    {
        panelMenu.SetActive(true);
        panelOpciones.SetActive(false);
        panelCargar.SetActive(false);
    }
}