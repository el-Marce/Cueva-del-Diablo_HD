using UnityEngine;

public class NoiseEmitter : MonoBehaviour
{
    public float noiseRadius = 8f;

    public void EmitNoise(float loudness, Vector3 playerPosition = default)
    {
        Debug.Log("Position utilizada por NoiseEmitter: " + playerPosition);
        float radius = noiseRadius * loudness;
        NoiseSystem.Instance.MakeNoise(transform.position, radius, playerPosition);
    }
}