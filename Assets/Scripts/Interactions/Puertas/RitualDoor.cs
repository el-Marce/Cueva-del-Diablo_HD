using UnityEngine;
using System.Collections;

public class RitualDoor : MonoBehaviour
{
    public Door door;
    public Inventory inventory;

    [Header("Items requeridos")]
    public string item1 = "coca";
    public string item2 = "alcohol";
    public string item3 = "sullu";

    [Header("Timing")]
    public float openDelay = 3f;

    [Header("Cinemática")]
    public GameObject cinematicCamera;

    bool opened = false;
    bool sequenceStarted = false;

    void Update()
    {
        if (opened || sequenceStarted) return;

        if (inventory.HasItem(item1) &&
            inventory.HasItem(item2) &&
            inventory.HasItem(item3))
        {
            StartCoroutine(OpenSequence());
        }
    }

    IEnumerator OpenSequence()
    {
        sequenceStarted = true;

        GameState.InMenu = true;

        if (cinematicCamera != null)
            cinematicCamera.SetActive(true);

        yield return new WaitForSeconds(openDelay);

        door.isLocked = false;
        door.OpenDoor();

        opened = true;

        yield return new WaitForSeconds(2f);

        if (cinematicCamera != null)
            cinematicCamera.SetActive(false);

        GameState.InMenu = false;
    }
}