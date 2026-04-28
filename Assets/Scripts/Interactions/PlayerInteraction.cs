using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInteraction : MonoBehaviour
{
    public float interactDistance = 1.5f;
    GameObject interactUI;

    IInteractable currentInteractable;

    Renderer currentRenderer;
    Material[] originalMaterials;
    public Material highlightMaterial;
    //Material currentOutlineMat;

    float outlineOn = 0.015f;
    float outlineOff = 0f;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject ui = GameObject.Find("UI");
        interactUI = ui.transform.Find("Canvas/InteractUI").gameObject;
    }

    void Update()
    {
        if (GameState.InMenu) return;

        CheckInteraction();

        if (Input.GetKeyDown(KeyCode.E) && currentInteractable != null)
        {
            currentInteractable.Interact();

            ClearInteraction();
        }
    }

    void CheckInteraction()
    {
        Camera cam = Camera.main;
        Vector3 origin = cam.transform.position;
        Vector3 forward = cam.transform.forward;

        Collider[] hits = Physics.OverlapSphere(origin, interactDistance);

        IInteractable bestInteractable = null;
        Renderer bestRenderer = null;

        float bestAngle = 999f;

        foreach (Collider col in hits)
        {
            IInteractable interactable = col.GetComponent<IInteractable>();
            if (interactable == null) continue;

            Vector3 dirToTarget = (col.bounds.center - origin).normalized;
            float angle = Vector3.Angle(forward, dirToTarget);

            if (angle < 35f)
            {
                if (angle < bestAngle)
                {
                    bestAngle = angle;
                    bestInteractable = interactable;
                    bestRenderer = col.GetComponent<Renderer>();
                }
            }
        }

        if (bestInteractable != null)
        {
            currentInteractable = bestInteractable;
            interactUI.SetActive(true);

            if (bestRenderer != null)
            {
                Material[] mats = bestRenderer.materials;

                if (mats.Length >= 1)
                {
                    if (currentRenderer != bestRenderer)
                    {
                        ClearOutline();

                        currentRenderer = bestRenderer;

                        originalMaterials = bestRenderer.materials;

                        Material[] highlightArray = new Material[originalMaterials.Length];

                        for (int i = 0; i < highlightArray.Length; i++)
                        {
                            highlightArray[i] = highlightMaterial;
                        }

                        bestRenderer.materials = highlightArray;
                    }
                }
            }

            return;
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
        if (currentRenderer != null)
        {
            if (originalMaterials != null)
            {
                currentRenderer.materials = originalMaterials;
            }

            currentRenderer = null;
            originalMaterials = null;
        }
    }
}