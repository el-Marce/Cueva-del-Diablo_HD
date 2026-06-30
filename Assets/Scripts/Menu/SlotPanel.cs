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
    CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        btnCerrar.onClick.AddListener(Cerrar);
        OcultarPanel();
    }

    public void Abrir(int slotIndex, PanelCargar panel)
    {
        selectedSlot = slotIndex;
        panelCargar = panel;

        titleText.text = "Ranura " + (slotIndex + 1) + " seleccionada";

        SaveSlot data = SaveSystem.GetSlot(slotIndex);
        btnCargar.interactable = data.hasData;

        btnCargar.onClick.RemoveListener(OnCargar);
        btnBorrar.onClick.RemoveListener(OnBorrar);

        btnCargar.onClick.AddListener(OnCargar);
        btnBorrar.onClick.AddListener(OnBorrar);

        MostrarPanel();
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
        OcultarPanel();
    }

    void MostrarPanel()
    {
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    void OcultarPanel()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}