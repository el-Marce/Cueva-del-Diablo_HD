using UnityEngine;

public class RotarObjeto : MonoBehaviour
{
    public float velocidad = 100f;

    void Update()
    {
        transform.Rotate(0, velocidad * Time.deltaTime, 0);
    }
}