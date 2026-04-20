using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SlotPanel : MonoBehaviour
{
    [Header("Referencias")]
    public TMP_Text titleText;
    public Button btnCargar;
    public Button btnBorrar;
    public Button btnCerrar;

    PanelCargar panelCargar;
    int selectedSlot = -1;

    void Awake()
    {
        btnCerrar.onClick.AddListener(Cerrar);
        gameObject.SetActive(false);
    }

    public void Abrir(int slotIndex, PanelCargar panel)
    {
        selectedSlot = slotIndex;
        panelCargar = panel;

        titleText.text = "Ranura " + (slotIndex + 1) + " seleccionada";

        // Configura botones según si tiene datos
        SaveSlot data = SaveSystem.GetSlot(slotIndex);
        btnCargar.interactable = data.hasData;

        btnCargar.onClick.RemoveAllListeners();
        btnBorrar.onClick.RemoveAllListeners();

        btnCargar.onClick.AddListener(OnCargar);
        btnBorrar.onClick.AddListener(OnBorrar);

        gameObject.SetActive(true);
    }

    void OnCargar()
    {
        panelCargar.CargarSlot(selectedSlot);
        Cerrar();
    }

    void OnBorrar()
    {
        panelCargar.BorrarSlot(selectedSlot);
        Cerrar();
    }

    void Cerrar()
    {
        selectedSlot = -1;
        gameObject.SetActive(false);
    }
}