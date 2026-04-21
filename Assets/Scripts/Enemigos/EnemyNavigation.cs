using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyNavigation : MonoBehaviour
{
    public NavMeshAgent agent;
    EnemyStats stats;

    float currentMultiplier = 1f;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        stats = GetComponent<EnemyStats>();

        agent.speed = stats.moveSpeed;
        agent.updateRotation = false;
    }

    public void Pause()
    {
        if (agent != null)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero; //corte inmediato
        }
    }

    public void Resume()
    {
        if (agent != null)
            agent.isStopped = false;
    }

    public void MoveTo(Vector3 target)
    {
        agent.SetDestination(target);
    }

    public void StopMoving()
    {
        agent.ResetPath();

    }
    public void SetSpeedMultiplier(float multiplier)
    {
        if (currentMultiplier == multiplier) return;

        currentMultiplier = multiplier;
        agent.speed = stats.moveSpeed * multiplier;
    }

    public void ResetSpeed()
    {
        if (currentMultiplier == 1f) return;

        currentMultiplier = 1f;
        agent.speed = stats.moveSpeed;
    }
}