using UnityEngine;

public class LeerPergaminos : MonoBehaviour
{
    Inventory inventory;

    bool readingMode = false;

    void Start()
    {
        inventory = GetComponent<Inventory>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            ToggleReadingMode();
        }

        if (readingMode)
        {
            CheckScrollSelection();
        }
    }

    void ToggleReadingMode()
    {
        readingMode = !readingMode;

        if (readingMode)
        {
            inventory.ShowScrollList();
        }
        else
        {
            Debug.Log("Cerrar lectura");
        }
    }

    void CheckScrollSelection()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            inventory.ReadScroll(0);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            inventory.ReadScroll(1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            inventory.ReadScroll(2);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            inventory.ReadScroll(3);
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            inventory.ReadScroll(4);
        }
    }
}