using UnityEngine;

public class EnemyHearing : MonoBehaviour
{
    public float hearingDistance;

    Vector3 lastHeardPosition;
    bool heardSomething = false;

    bool knowsPlayerPosition = false;
    Vector3 sharedPlayerPosition;

    public void HearNoise(Vector3 noisePosition, float radius, Vector3 playerPosition)
    {
        float distance = Vector3.Distance(transform.position, noisePosition);

        Debug.Log("Distancia: " + distance + " | Umbral: " + hearingDistance);
        if (distance <= hearingDistance)
        {
            heardSomething = true;
            lastHeardPosition = noisePosition;
            Debug.Log(name + "ha escuchado el sonido");
        }
        if (playerPosition != Vector3.zero)
        {
            knowsPlayerPosition = true;
            sharedPlayerPosition = playerPosition;

            Debug.Log(name + " recibió la POSICIÓN DEL JUGADOR por alerta: " + playerPosition);
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
}