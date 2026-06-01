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

    [Header("Tutorial (opcional)")]
    public TutorialStep tutorialStep;
    public TutorialBarrier barreraTutorial;
    public void Interact()
    {
        Inventory inventory = FindObjectOfType<Inventory>();

        if (inventory != null)
        {
            AudioManager.Instance.Play(pergaminoSound);
            inventory.AddScroll(text, icon);
            TutorialManager.Instance.CompletarTrigger("pergamino_recogido");
        }

        Destroy(gameObject);

        scrollPanel.SetActive(true);

        scrollText.text = text;

        GameState.InMenu = true;

        Destroy(gameObject);


        // Activar tutorial al recoger
        if (tutorialStep != null)
            TutorialManager.Instance?.MostrarPaso(tutorialStep, barreraTutorial);
    }
}