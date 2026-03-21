using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyNavigation : MonoBehaviour
{
    public NavMeshAgent agent;
    EnemyStats stats;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        stats = GetComponent<EnemyStats>();

        agent.speed = stats.moveSpeed;
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
}