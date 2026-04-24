using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SlotUI : MonoBehaviour
{
    [Header("Referencias")]
    public TMP_Text slotNumberText;
    public TMP_Text sceneNameText;
    public TMP_Text dateText;
    public GameObject emptyLabel;
    public GameObject dataGroup;

    int slotIndex;
    PanelCargar panelCargar;
    private void Start()
    {
        //SaveSystem.SaveSlotData(0, 1, "Nivel1");
        SaveSystem.SaveSlotData(1, 2, "Nivel2");
        SaveSystem.SaveSlotData(2, 3, "Nivel3");
    }
    public void Setup(int index, PanelCargar panel)
    {
        slotIndex = index;
        panelCargar = panel;

        slotNumberText.text = "Ranura " + (index + 1);

        // El slot completo es clickeable
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(OnSlotClick);
        }

        Refresh();
    }

    void OnSlotClick()
    {
        panelCargar.AbrirSlotPanel(slotIndex);
    }

    public void Refresh()
    {
        SaveSlot data = SaveSystem.GetSlot(slotIndex);

        Button btn = GetComponent<Button>();
        MenuButtonHover hover = GetComponent<MenuButtonHover>();

        if (data.hasData)
        {
            emptyLabel.SetActive(false);
            dataGroup.SetActive(true);
            sceneNameText.text = data.sceneName;
            dateText.text = data.saveDate;

            if (btn != null) btn.interactable = true;
            if (hover != null) hover.enabled = true;
        }
        else
        {
            emptyLabel.SetActive(true);
            dataGroup.SetActive(false);

            if (btn != null) btn.interactable = false;
            if (hover != null) hover.enabled = false;

            // Asegura que la imagen de hover quede invisible
            if (hover != null && hover.hoverImage != null)
            {
                Color c = hover.hoverImage.color;
                c.a = 0f;
                hover.hoverImage.color = c;
            }
        }
    }
}