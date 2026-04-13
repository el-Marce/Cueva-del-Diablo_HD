using UnityEngine;

public class AltarCondition_Entes : AltarCondition
{
    public int requiredCount = 3;
    public float detectionRadius = 10f;

    AltarRitual_Generic altar;

    void Awake()
    {
        altar = GetComponent<AltarRitual_Generic>();
    }

    public override bool IsMet() => CountEntes() >= requiredCount;

    public override string GetStatusText() =>
        "Almas cercanas: " + CountEntes() + "/" + requiredCount;

    public override void OnFulfill() { }

    int CountEntes()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, altar.pueblerinoLayer);
        int count = 0;
        foreach (Collider hit in hits)
            if (hit.GetComponent<Pueblerino>() != null) count++;
        return count;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}