using UnityEngine;

public class NoiseEmitter : MonoBehaviour
{
    public float noiseRadius = 8f;

    public void EmitNoise(float loudness)
    {
        float radius = noiseRadius * loudness;
        NoiseSystem.Instance.MakeNoise(transform.position, radius);
    }
}