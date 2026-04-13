using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SlotUI : MonoBehaviour
{
    [Header("Referencias")]
    public TMP_Text slotNumberText;
    public TMP_Text sceneNameText;
    public TMP_Text dateText;
    public Button btnCargar;
    public Button btnBorrar;
    public GameObject emptyLabel;
    public GameObject dataGroup;

    int slotIndex;
    PanelCargar panelCargar;

    public void Setup(int index, PanelCargar panel)
    {
        slotIndex = index;
        panelCargar = panel;

        slotNumberText.text = "Ranura " + (index + 1);

        btnCargar.onClick.RemoveAllListeners();
        btnBorrar.onClick.RemoveAllListeners();

        btnCargar.onClick.AddListener(() => panelCargar.CargarSlot(slotIndex));
        btnBorrar.onClick.AddListener(() => panelCargar.BorrarSlot(slotIndex));

        Refresh();
    }

    public void Refresh()
    {
        SaveSlot data = SaveSystem.GetSlot(slotIndex);

        if (data.hasData)
        {
            emptyLabel.SetActive(false);
            dataGroup.SetActive(true);
            sceneNameText.text = data.sceneName;
            dateText.text = data.saveDate;
            btnCargar.interactable = true;
            btnBorrar.interactable = true;
        }
        else
        {
            emptyLabel.SetActive(true);
            dataGroup.SetActive(false);
            btnCargar.interactable = false;
            btnBorrar.interactable = true;
        }
    }
}