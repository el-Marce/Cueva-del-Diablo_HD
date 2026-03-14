using UnityEngine;

public class FloatMotion : MonoBehaviour
{
    public float amplitude = 0.3f;
    public float speed = 1f;

    Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float y = Mathf.Sin(Time.time * speed) * amplitude;
        transform.position = new Vector3(transform.position.x, startPos.y + y, transform.position.z);
    }
}