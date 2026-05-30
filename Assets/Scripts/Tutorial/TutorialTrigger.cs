using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    public TutorialStep paso;
    public bool soloUnaVez = true;

    [Header("Barrera")]
    [Tooltip("Opcional: barrera que se activa junto con este paso")]
    public TutorialBarrier barrera;

    bool activado = false;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (activado && soloUnaVez) return;
        if (TutorialManager.Instance == null) return;

        activado = true;
        TutorialManager.Instance.MostrarPaso(paso, barrera);
    }
}