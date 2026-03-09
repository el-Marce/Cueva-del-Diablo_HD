using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactDistance = 1f;
    public GameObject interactUI;

    IInteractable currentInteractable;

    Renderer currentRenderer;
    Material currentOutlineMat;

    float outlineOn = 0.025f;
    float outlineOff = 0f;

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
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                currentInteractable = interactable;
                interactUI.SetActive(true);

                Renderer rend = hit.collider.GetComponent<Renderer>();

                if (rend != null)
                {
                    Material[] mats = rend.materials;

                    if (mats.Length > 1) // asegúrate de que tenga outline
                    {
                        if (currentRenderer != rend)
                        {
                            ClearOutline();

                            currentRenderer = rend;
                            currentOutlineMat = mats[1];
                            currentOutlineMat.SetFloat("_Thickness", outlineOn);
                        }
                    }
                }

                return;
            }
        }

        ClearInteraction();
    }

    void ClearInteraction()
    {
        currentInteractable = null;
        interactUI.SetActive(false);
        ClearOutline();
    }

    void ClearOutline()
    {
        if (currentOutlineMat != null)
        {
            currentOutlineMat.SetFloat("_Thickness", outlineOff);
            currentOutlineMat = null;
            currentRenderer = null;
        }
    }
}