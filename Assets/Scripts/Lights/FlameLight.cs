using UnityEngine;

public class FlameLight : MonoBehaviour
{
    Light lt;

    [Header("Intensidad")]
    public float baseIntensity = 1.5f;
    public float flickerAmount = 0.4f;

    [Header("Velocidad")]
    public float speed = 8f;

    float seed;

    void Start()
    {
        lt = GetComponent<Light>();
        seed = Random.Range(0f, 100f);
    }

    void Update()
    {
        float noise = Mathf.PerlinNoise(seed + Time.time * speed, seed);
        lt.intensity = baseIntensity + (noise - 0.5f) * 2f * flickerAmount;
    }
}