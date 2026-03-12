using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    public float visionDistance = 10f;
    public float visionAngle = 60f;

    public Transform player;

    public bool CanSeePlayer()
    {
        Vector3 dirToPlayer = player.position - transform.position;

        if (dirToPlayer.magnitude > visionDistance)
            return false;

        float angle = Vector3.Angle(transform.forward, dirToPlayer);

        if (angle > visionAngle)
            return false;

        RaycastHit hit;

        Vector3 origin = transform.position + Vector3.up * 1.6f;

        if (Physics.Raycast(origin, dirToPlayer.normalized, out hit, visionDistance))
        {
            //Debug.Log("Raycast golpeó: " + hit.transform.name + " | Tag: " + hit.transform.tag);

            if (hit.transform.CompareTag("Player"))
            {
                //Debug.Log("Jugador Detectado");
                return true;
            }
        }
        Debug.DrawRay(transform.position + Vector3.up, dirToPlayer.normalized * visionDistance, Color.red);
        return false;
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Vector3 left = Quaternion.Euler(0, -visionAngle, 0) * transform.forward;
        Vector3 right = Quaternion.Euler(0, visionAngle, 0) * transform.forward;

        Gizmos.DrawRay(transform.position, left * visionDistance);
        Gizmos.DrawRay(transform.position, right * visionDistance);
        Gizmos.DrawRay(transform.position, transform.forward * visionDistance);
    }

}