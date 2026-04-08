using UnityEngine;

public class EnemyHearing : MonoBehaviour
{
    public float hearingDistance;

    Vector3 lastHeardPosition;
    bool heardSomething = false;

    bool knowsPlayerPosition = false;
    Vector3 sharedPlayerPosition;

    public void HearNoise(Vector3 noisePosition, float radius, Vector3 playerPosition, GameObject source = null)
    {
        if (source != null && source == gameObject) return;

        float distance = Vector3.Distance(transform.position, noisePosition);
        if (distance <= radius + hearingDistance)
        {
            heardSomething = true;
            lastHeardPosition = noisePosition;
            if (playerPosition != Vector3.zero)
            {
                knowsPlayerPosition = true;
                sharedPlayerPosition = playerPosition;
            }
        }
    }

    public bool HasHeardSomething()
    {
        return heardSomething;
    }

    public Vector3 GetNoisePosition()
    {
        heardSomething = false;
        return lastHeardPosition;
    }

    public bool HasSharedPlayerPosition()
    {
        return knowsPlayerPosition;
    }

    public Vector3 GetSharedPlayerPosition()
    {
        knowsPlayerPosition = false;
        return sharedPlayerPosition;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, hearingDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(lastHeardPosition, 0.3f);
    }
}