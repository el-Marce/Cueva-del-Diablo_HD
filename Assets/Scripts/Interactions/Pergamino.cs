using FMODUnity;
using TMPro;
using UnityEngine;

public class Pergamino : MonoBehaviour, IInteractable
{
    [Header("Identificación")]
    [Tooltip("Nombre corto para identificar este pergamino en el ItemIconDatabase. " +
             "No afecta el texto mostrado al jugador.")]
    public string scrollName;

    [TextArea]
    public string text;
    public GameObject scrollPanel;
    public TMP_Text scrollText;
    public Sprite icon;
    public EventReference pergaminoSound;

    public void Interact()
    {
        // Registrar pickup ANTES de destruir el objeto
        CheckpointManager.Instance?.RegistrarPickup(transform.GetFullPath());

        Inventory inventory = FindObjectOfType<Inventory>();
        if (inventory != null)
        {
            AudioManager.Instance.Play(pergaminoSound);
            inventory.AddScroll(text, icon, scrollName);
            TutorialManager.Instance.CompletarTrigger("pergamino_recogido");
        }

        scrollPanel.SetActive(true);
        scrollText.text = text;
        GameState.InMenu = true;
        Destroy(gameObject);
    }
}