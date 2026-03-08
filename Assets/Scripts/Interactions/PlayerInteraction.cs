using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactDistance = 1f;
    public GameObject interactUI;

    IInteractable currentInteractable;

    void Update()
    {
        CheckInteraction();

        if (Input.GetKeyDown(KeyCode.E) && currentInteractable != null)
        {
            currentInteractable.Interact();
        }
    }

    void CheckInteraction()
    {
        Vector3 origin = Camera.main.transform.position + Camera.main.transform.forward * 0.2f;
        Ray ray = new Ray(origin, Camera.main.transform.forward);
        RaycastHit hit;

        Debug.DrawRay(origin, Camera.main.transform.forward * interactDistance, Color.green);

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            //if (hit.collider.CompareTag("Player"))
              //  return;

            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                currentInteractable = interactable;
                interactUI.SetActive(true);
                return;
            }
        }
        ClearInteraction();
    }

    void ClearInteraction()
    {
        currentInteractable = null;
        interactUI.SetActive(false);
    }
}