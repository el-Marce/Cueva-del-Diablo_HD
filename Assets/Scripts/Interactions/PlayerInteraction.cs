using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInteraction : MonoBehaviour
{
    public float interactDistance = 1.5f;
    public float outlineDistance = 4f;
    public float outlineMaxThickness = 0.015f;

    GameObject interactUI;
    IInteractable currentInteractable;
    Renderer currentRenderer;
    Material[] originalMaterials;
    public Material highlightMaterial;

    Renderer outlineRenderer;
    Material outlineMat;

    void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentInteractable = null;
        currentRenderer = null;
        originalMaterials = null;
        outlineRenderer = null;
        outlineMat = null;

        GameObject ui = GameObject.Find("UI");
        if (ui == null) return;
        Transform interactTransform = ui.transform.Find("Canvas/InteractUI");
        if (interactTransform != null)
            interactUI = interactTransform.gameObject;
    }

    void Update()
    {
        if (GameState.InMenu) return;
        CheckProximityOutline();
        CheckInteraction();

        if (Input.GetKeyDown(KeyCode.E) && currentInteractable != null)
        {
            currentInteractable.Interact();
            ClearInteraction();
        }
    }

    void CheckProximityOutline()
    {
        Camera cam = Camera.main;
        Vector3 origin = cam.transform.position;

        Collider[] hits = Physics.OverlapSphere(origin, outlineDistance);
        Renderer closestRenderer = null;
        float closestDist = outlineDistance;

        foreach (Collider col in hits)
        {
            IInteractable interactable = col.GetComponent<IInteractable>();
            if (interactable == null) continue;

            float dist = Vector3.Distance(origin, col.bounds.center);
            if (dist < closestDist)
            {
                closestDist = dist;
                closestRenderer = col.GetComponent<Renderer>();
            }
        }

        if (closestRenderer != null)
        {
            if (outlineRenderer != closestRenderer)
            {
                ClearProximityOutline();
                outlineRenderer = closestRenderer;

                Material[] mats = outlineRenderer.materials;
                Material[] newMats = new Material[mats.Length + 1];
                for (int i = 0; i < mats.Length; i++)
                    newMats[i] = mats[i];
                outlineMat = new Material(highlightMaterial);
                newMats[mats.Length] = outlineMat;
                outlineRenderer.materials = newMats;
            }

            float t = 1f - Mathf.Clamp01(closestDist / outlineDistance);
            float thickness = Mathf.Lerp(0f, outlineMaxThickness, t);
            if (outlineMat != null && outlineMat.HasProperty("_Thickness"))
                outlineMat.SetFloat("_Thickness", thickness);
        }
        else
        {
            ClearProximityOutline();
        }
    }

    void ClearProximityOutline()
    {
        if (outlineRenderer == null) return;

        Material[] mats = outlineRenderer.materials;
        if (mats.Length > 1)
        {
            Material[] original = new Material[mats.Length - 1];
            for (int i = 0; i < original.Length; i++)
                original[i] = mats[i];
            outlineRenderer.materials = original;
        }

        outlineRenderer = null;
        outlineMat = null;
    }

    void CheckInteraction()
    {
        if (interactUI == null) return;

        Camera cam = Camera.main;
        Vector3 origin = cam.transform.position;
        Vector3 forward = cam.transform.forward;

        IInteractable bestInteractable = null;
        Renderer bestRenderer = null;
        float bestAngle = 999f;

        Collider[] hits = Physics.OverlapSphere(origin, interactDistance);
        foreach (Collider col in hits)
        {
            IInteractable interactable = col.GetComponent<IInteractable>();
            if (interactable == null) continue;

            Vector3 dirToTarget = (col.bounds.center - origin).normalized;
            float angle = Vector3.Angle(forward, dirToTarget);
            if (angle < 60f && angle < bestAngle)
            {
                bestAngle = angle;
                bestInteractable = interactable;
                bestRenderer = col.GetComponent<Renderer>();
            }
        }

        if (bestInteractable == null)
        {
            if (Physics.SphereCast(origin, 0.3f, forward, out RaycastHit hit, interactDistance))
            {
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    bestInteractable = interactable;
                    bestRenderer = hit.collider.GetComponent<Renderer>();
                }
            }
        }

        if (bestInteractable != null)
        {
            currentInteractable = bestInteractable;
            interactUI.SetActive(true);
            return;
        }

        ClearInteraction();
    }

    void ClearInteraction()
    {
        currentInteractable = null;
        if (interactUI != null) interactUI.SetActive(false);
        ClearOutline();
    }

    void ClearOutline()
    {
        if (currentRenderer != null)
        {
            if (originalMaterials != null)
                currentRenderer.materials = originalMaterials;
            currentRenderer = null;
            originalMaterials = null;
        }
    }
}