using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyNavigation : MonoBehaviour
{
    NavMeshAgent agent;
    EnemyStats stats;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        stats = GetComponent<EnemyStats>();

        agent.speed = stats.moveSpeed;
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