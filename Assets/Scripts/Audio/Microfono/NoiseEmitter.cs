using UnityEngine;

public class NoiseEmitter : MonoBehaviour
{
    public float noiseRadius = 8f;

    float lastNoiseRadius = 0f;
    float gizmoTimer;
    public void EmitNoise(float loudness, Vector3 playerPosition = default)
    {
        //Debug.Log("Position utilizada por NoiseEmitter: " + playerPosition);
        float radius = noiseRadius * loudness;

        //NoiseSystem.Instance.MakeNoise(transform.position, radius, playerPosition, gameObject);
        NoiseSystem.Instance.MakeNoise(transform.position, radius, playerPosition, gameObject);

        lastNoiseRadius = radius;
        gizmoTimer = 5f;
    }

    void Update()
    {
        gizmoTimer -= Time.deltaTime;

        if (gizmoTimer <= 0f)
        {
            lastNoiseRadius = 0f;
        }
    }
    void OnDrawGizmos()
    {
        // Radio base
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, noiseRadius);

        // Último ruido emitido (más fuerte visualmente)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lastNoiseRadius);
    }
}