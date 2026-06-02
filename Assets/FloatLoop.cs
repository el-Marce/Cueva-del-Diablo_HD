using UnityEngine;

public class FloatLoop : MonoBehaviour
{
    [Header("Movimiento")]
    public float amplitude = 0.2f; // Altura máxima
    public float speed = 1f;       // Velocidad de oscilación

    Vector3 startPosition;

    void Start()
    {
        startPosition = transform.localPosition;
    }

    void Update()
    {
        float offset = Mathf.Sin(Time.time * speed) * amplitude;

        transform.localPosition =
            startPosition + Vector3.up * offset;
    }
}