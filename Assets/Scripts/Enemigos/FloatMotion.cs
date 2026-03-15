using UnityEngine;
using UnityEngine.AI;

public class FloatMotion : MonoBehaviour
{
    public float amplitude = 0.3f;
    public float speed = 1f;
    public float transitionSpeed = 2f;

    NavMeshAgent agent;

    float currentOffset;
    float targetOffset;

    public bool oscillationEnabled = true;

    void OnEnable()
    {
        agent = GetComponent<NavMeshAgent>();

        currentOffset = agent.baseOffset;
        targetOffset = currentOffset;
    }

    void Update()
    {
        float y = 0f;

        if (oscillationEnabled)
        {
            y = Mathf.Sin(Time.time * speed) * amplitude;
        }

        currentOffset = Mathf.Lerp(currentOffset, targetOffset, Time.deltaTime * transitionSpeed);

        agent.baseOffset = currentOffset + y;
    }

    public void SetOffset(float offset)
    {
        targetOffset = offset;
    }

    public void EnableOscillation(bool value)
    {
        oscillationEnabled = value;
    }
}