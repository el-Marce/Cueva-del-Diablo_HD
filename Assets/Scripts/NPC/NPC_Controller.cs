using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class NPC_Controller : MonoBehaviour
{
    public Transform player;
    PlayerMovement playerMovement;

    [Header("Distancias")]
    public float followDistance = 6f;

    [Header("Exploración")]
    public float exploreRadius = 4f;
    public float exploreInterval = 3f;

    [Header("Movimiento")]
    public float followStoppingDistance = 1.5f;

    [Header("Reacción")]
    public float reactionTime = 0.3f;
    float targetSpeed;
    float speedVelocity;

    NavMeshAgent agent;
    float exploreTimer;

    enum State { Explore, Follow }
    State currentState;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentState = State.Explore;
        playerMovement = player.GetComponent<PlayerMovement>();
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        // Decidir estado
        if (distance > followDistance)
            SetState(State.Follow);
        else
            SetState(State.Explore);

        // Ejecutar estado
        if (currentState == State.Explore)
            UpdateExplore();
        else
            UpdateFollow();
    }

    void SetState(State newState)
    {
        if (currentState == newState) return;

        currentState = newState;

        if (currentState == State.Follow)
        {
            agent.stoppingDistance = followStoppingDistance;
        }
        else
        {
            agent.stoppingDistance = 0f;
            exploreTimer = 0f; // fuerza nuevo destino inmediato
        }
    }

    void UpdateFollow()
    {
        targetSpeed = Mathf.SmoothDamp(targetSpeed, playerMovement.speed, ref speedVelocity, reactionTime);
        agent.speed = targetSpeed;
        agent.SetDestination(player.position);
    }

    void UpdateExplore()
    {
        exploreTimer -= Time.deltaTime;

        if (exploreTimer <= 0f)
        {
            agent.SetDestination(GetRandomPointNearSelf());
            exploreTimer = exploreInterval;
        }
    }

    Vector3 GetRandomPointNearSelf()
    {
        Vector2 random = Random.insideUnitCircle * exploreRadius;
        Vector3 target = transform.position + new Vector3(random.x, 0, random.y);

        if (NavMesh.SamplePosition(target, out NavMeshHit hit, exploreRadius, NavMesh.AllAreas))
            return hit.position;

        return transform.position;
    }

    // --- POSESIÓN ---
    public GameObject zombiePrefab;

    public void Possess()
    {       
        StartCoroutine(PossessDelay());
    }
    IEnumerator PossessDelay()
    {
        yield return new WaitForSeconds(3f);
        GameObject zombie = Instantiate(zombiePrefab, transform.position, transform.rotation); 
        zombie.GetComponent<NoiseEmitter>().EmitNoise(999f, player.position);
        Destroy(gameObject);
    }
}