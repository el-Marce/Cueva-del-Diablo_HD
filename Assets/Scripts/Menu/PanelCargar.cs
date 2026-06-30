using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PanelCargar : MonoBehaviour
{
    [Header("Slots")]
    public SlotUI[] slots;

    [Header("Navegación")]
    public MenuPrincipal menu;
    public Button btnVolver;

    [Header("Panel selección")]
    public SlotPanel slotPanel;

    void Awake()
    {
        btnVolver.onClick.AddListener(menu.VolverAlMenu);
    }

    public void Refresh()
    {
        for (int i = 0; i < slots.Length; i++)
            slots[i].Setup(i, this);
    }

    public void AbrirSlotPanel(int slotIndex)
    {
        Debug.Log($"[PanelCargar] AbrirSlotPanel({slotIndex}) llamado en frame {Time.frameCount}");
        slotPanel.Abrir(slotIndex, this);
    }

    public void CargarSlot(int slot)
    {
        SaveSlot data = SaveSystem.GetSlot(slot);
        if (data.hasData)
            SceneManager.LoadScene(data.sceneIndex);
    }

    public void BorrarSlot(int slot)
    {
        SaveSystem.DeleteSlot(slot);
        slots[slot].Refresh();
    }
}