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

    [Header("Velocidad")]
    public float exploreSpeedMultiplier = 0.5f;
    public float followSpeedMultiplier = 1f;
    public float baseSpeed = 5f;

    [Header("Reacción")]
    public float reactionTime = 0.3f;
    float targetSpeed;
    float speedVelocity;

    [Header("Rotación")]
    public float rotationSpeed = 5f;
    public float rotationThreshold = 10f;

    NavMeshAgent agent;
    float exploreTimer;

    Animator anim;
    enum State { Explore, Follow }
    State currentState;

    bool possessed = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentState = State.Explore;

        if (player == null)
            player = GameObject.FindWithTag("Player").transform;

        anim = GetComponentInChildren<Animator>();
        playerMovement = player.GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (possessed) return;

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > followDistance)
            SetState(State.Follow);
        else
            SetState(State.Explore);

        if (currentState == State.Explore)
            UpdateExplore();
        else
            UpdateFollow();

        UpdateAnimations();
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
            exploreTimer = 0f;
        }
    }

    void UpdateFollow()
    {
        float targetSpeedGoal = baseSpeed * followSpeedMultiplier *
                               (playerMovement != null ? playerMovement.speed / playerMovement.moveSpeed : 1f);

        targetSpeed = Mathf.SmoothDamp(targetSpeed, targetSpeedGoal, ref speedVelocity, reactionTime);
        agent.speed = targetSpeed;
        agent.SetDestination(player.position);
    }

    void UpdateExplore()
    {
        agent.speed = baseSpeed * exploreSpeedMultiplier;
        exploreTimer -= Time.deltaTime;

        if (exploreTimer <= 0f)
        {
            agent.SetDestination(GetRandomPointNearSelf());
            exploreTimer = exploreInterval;
        }

        if (agent.hasPath && !IsFacingTarget(agent.steeringTarget))
        {
            RotateTowards(agent.steeringTarget);
            agent.velocity = Vector3.zero;
        }
    }

    void UpdateAnimations()
    {
        bool isMoving = agent.velocity.magnitude > 0.1f;
        bool isFollowing = currentState == State.Follow;
        bool needsRotation = false;

        Vector3 targetPos = isFollowing ? player.position :
                            (agent.hasPath ? agent.steeringTarget : transform.position);

        Vector3 dir = targetPos - transform.position;
        dir.y = 0f;

        float turnAngle = 0f;
        if (dir.magnitude > 0.1f)
        {
            float angle = Vector3.SignedAngle(transform.forward, dir.normalized, Vector3.up);
            turnAngle = Mathf.Clamp(angle / 180f, -1f, 1f);
            needsRotation = Mathf.Abs(angle) > rotationThreshold;
        }

        anim.SetFloat("TurnAngle", turnAngle, 0.1f, Time.deltaTime);

        if (needsRotation && !isMoving)
        {
            anim.SetBool("Idle", true);
            anim.SetBool("Siguiendo", false);
            anim.SetBool("Explorando", false);
        }
        else
        {
            anim.SetBool("Idle", !isMoving);
            anim.SetBool("Siguiendo", isFollowing && isMoving);
            anim.SetBool("Explorando", !isFollowing && isMoving);
        }
    }

    bool IsFacingTarget(Vector3 target)
    {
        Vector3 dir = (target - transform.position).normalized;
        dir.y = 0f;
        float angle = Vector3.Angle(transform.forward, dir);
        return angle < rotationThreshold;
    }

    void RotateTowards(Vector3 target)
    {
        Vector3 dir = (target - transform.position).normalized;
        dir.y = 0f;
        if (dir == Vector3.zero) return;
        Quaternion targetRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);
    }

    Vector3 GetRandomPointNearSelf()
    {
        Vector2 random = Random.insideUnitCircle * exploreRadius;
        Vector3 target = transform.position + new Vector3(random.x, 0, random.y);

        if (NavMesh.SamplePosition(target, out NavMeshHit hit, exploreRadius, NavMesh.AllAreas))
            return hit.position;

        return transform.position;
    }

    public void PrepareForPossession()
    {
        possessed = true;
        agent.ResetPath();
        agent.velocity = Vector3.zero;
    }

    public GameObject zombiePrefab;

    public void Possess()
    {
        StartCoroutine(PossessDelay());
    }

    IEnumerator PossessDelay()
    {
        yield return new WaitForSeconds(3f);
        GameObject zombie = Instantiate(zombiePrefab, transform.position, transform.rotation);
        zombie.AddComponent<TriggerNivelDos>();

        yield return null;
        zombie.GetComponent<EnemyHearing>().HearNoise(player.position, 999f, player.position);
        zombie.GetComponent<EnemyStats>().health = 100;

        Destroy(gameObject);
    }
}