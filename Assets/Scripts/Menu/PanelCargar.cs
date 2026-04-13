using UnityEngine;
using UnityEngine.SceneManagement;

public class PanelCargar : MonoBehaviour
{
    [Header("Slots")]
    public SlotUI[] slots;

    [Header("Navegación")]
    public MenuPrincipal menu;

    public UnityEngine.UI.Button btnVolver;

    void Awake()
    {
        btnVolver.onClick.AddListener(menu.VolverAlMenu);
    }

    public void Refresh()
    {
        for (int i = 0; i < slots.Length; i++)
            slots[i].Setup(i, this);
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