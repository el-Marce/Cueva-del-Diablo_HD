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
        return "Cantidad de almas a ofrecer: " + CountEntes() + "/" + requiredCount;
    }

    public override void OnFulfill() { } // los entes no se "consumen", solo se verifica

    int CountEntes()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, enteLayer);
        int count = 0;
        foreach (Collider hit in hits)
            if (hit.GetComponent<EntePsicologico>() != null) count++;
        return count;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}