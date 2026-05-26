using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInteraction : MonoBehaviour
{
    public float interactDistance = 1.5f;
    public float outlineDistance = 4f;      // distancia a la que aparece el outline
    public float outlineMaxThickness = 0.015f;

    GameObject interactUI;
    IInteractable currentInteractable;
    Renderer currentRenderer;
    Material[] originalMaterials;
    public Material highlightMaterial;

    // Para outline gradual por distancia
    Renderer outlineRenderer;
    Material outlineMat;

    void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject ui = GameObject.Find("UI");
        interactUI = ui.transform.Find("Canvas/InteractUI").gameObject;
    }

    void Update()
    {
        if (GameState.InMenu) return;
        CheckProximityOutline(); // outline por distancia
        CheckInteraction();      // interacción al acercarse

        if (Input.GetKeyDown(KeyCode.E) && currentInteractable != null)
        {
            currentInteractable.Interact();
            ClearInteraction();
        }
    }

    // Detecta interactuables en radio amplio y aplica outline gradual
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
            // Si cambió el objeto más cercano
            if (outlineRenderer != closestRenderer)
            {
                ClearProximityOutline();
                outlineRenderer = closestRenderer;

                // Agrega el material de outline sin reemplazar los originales
                Material[] mats = outlineRenderer.materials;
                Material[] newMats = new Material[mats.Length + 1];
                for (int i = 0; i < mats.Length; i++)
                    newMats[i] = mats[i];
                outlineMat = new Material(highlightMaterial);
                newMats[mats.Length] = outlineMat;
                outlineRenderer.materials = newMats;
            }

            // Escala el outline según distancia — más cerca más grueso
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

        // Restaura materiales originales sin el outline extra
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
        interactUI.SetActive(false);
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