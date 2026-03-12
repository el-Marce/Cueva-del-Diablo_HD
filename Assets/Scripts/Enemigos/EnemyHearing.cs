using UnityEngine;

public class EnemyHearing : MonoBehaviour
{
    public float hearingDistance = 8f;

    Vector3 lastHeardPosition;
    bool heardSomething = false;

    public void HearNoise(Vector3 noisePosition)
    {
        float distance = Vector3.Distance(transform.position, noisePosition);

        if (distance <= hearingDistance)
        {
            heardSomething = true;
            lastHeardPosition = noisePosition;
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
}