using UnityEngine;
using TMPro;
public class Pergamino : MonoBehaviour, IInteractable
{
    [TextArea]
    public string text;

    public GameObject scrollPanel;
    public TMP_Text scrollText;
    public void Interact()
    {
        Inventory inventory = FindObjectOfType<Inventory>();

        if (inventory != null)
        {
            inventory.AddScroll(text);
        }

        Destroy(gameObject);

        scrollPanel.SetActive(true);

        scrollText.text = text + "\n\n<i>Pergamino almacenado en inventario</i>";

        GameState.InMenu = true;

        Destroy(gameObject);
    }
}