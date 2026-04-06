using UnityEngine;

public class AltarCondition_Entes : AltarCondition
{
    public int requiredCount = 3;
    public float detectionRadius = 10f;
    public LayerMask enteLayer;

    public override bool IsMet()
    {
        return CountEntes() >= requiredCount;
    }

    public override string GetStatusText()
    {
        return "Almas cercanas: " + CountEntes() + "/" + requiredCount;
    }

    public override void OnFulfill() { } // los entes no se "consumen", solo se verifica

    int CountEntes()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, enteLayer);
        Debug.Log("[Altar] Colliders detectados: " + hits.Length);
        int count = 0;
        foreach (Collider hit in hits)
        {
            Debug.Log("[Altar] Hit: " + hit.name + " tiene EntePsicologico: " + (hit.GetComponent<EntePsicologico>() != null));
            if (hit.GetComponent<EntePsicologico>() != null) count++;
        }
        return count;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}