using UnityEngine;

public class EnemyHearing : MonoBehaviour
{
    public float hearingDistance;

    Vector3 lastHeardPosition;
    bool heardSomething = false;

    public void HearNoise(Vector3 noisePosition)
    {
        float distance = Vector3.Distance(transform.position, noisePosition);

        Debug.Log("Casi se ha escuchado el sonido");
        Debug.Log("Distancia: " + distance + " | Umbral: " + hearingDistance);
        if (distance <= hearingDistance)
        {
            heardSomething = true;
            lastHeardPosition = noisePosition;
            Debug.Log("Se ha escuchado el sonido");
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