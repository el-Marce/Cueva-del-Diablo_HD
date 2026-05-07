using FMODUnity;
using TMPro;
using UnityEngine;
public class Pergamino : MonoBehaviour, IInteractable
{
    [TextArea]
    public string text;

    public GameObject scrollPanel;
    public TMP_Text scrollText;
    public Sprite icon;
    public EventReference pergaminoSound;

    public void Interact()
    {
        Inventory inventory = FindObjectOfType<Inventory>();

        if (inventory != null)
        {
            AudioManager.Instance.Play(pergaminoSound);
            inventory.AddScroll(text, icon);
        }

        Destroy(gameObject);

        scrollPanel.SetActive(true);

        scrollText.text = text + "\n\n<i>Pergamino almacenado en inventario</i>";

        GameState.InMenu = true;

        Destroy(gameObject);
    }
}