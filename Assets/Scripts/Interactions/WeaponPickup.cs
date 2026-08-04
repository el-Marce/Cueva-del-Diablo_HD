using FMODUnity;
using UnityEngine;
using static PlayerCombat;

public class WeaponPickup : MonoBehaviour, IInteractable
{
    public PlayerCombat.WeaponType weaponType;
    public int durability = 5;
    public string weaponName;
    public Sprite icon;

    [Header("Tutorial (opcional)")]
    public TutorialStep tutorialStep;
    public TutorialBarrier barreraTutorial;
    public TutorialStep stepSilencioso;
    public TutorialBarrier barreraEncolada;

    [Header("Sonidos")]
    public EventReference recogerPaloSound;
    public EventReference recogerRocaSound;
    public EventReference recogerAguaBendita;

    public void Interact()
    {
        // Registrar pickup ANTES de destruir el objeto
        CheckpointManager.Instance?.RegistrarPickup(transform.GetFullPath());

        Inventory inventory = FindObjectOfType<Inventory>();
        if (inventory != null)
            inventory.AddWeapon(weaponName, weaponType, icon, durability);

        switch (weaponType)
        {
            case WeaponType.Stick: AudioManager.Instance.Play(recogerPaloSound); break;
            case WeaponType.Rock: AudioManager.Instance.Play(recogerRocaSound); break;
            case WeaponType.AguaBendita: AudioManager.Instance.Play(recogerAguaBendita); break;
        }

        if (tutorialStep != null)
            TutorialManager.Instance?.MostrarPaso(tutorialStep, barreraTutorial);
        if (stepSilencioso != null)
            TutorialManager.Instance?.MostrarPaso(stepSilencioso, barreraEncolada);

        Destroy(gameObject);
    }
}