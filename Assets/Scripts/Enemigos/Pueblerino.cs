using UnityEngine;

public class Pueblerino : MonoBehaviour
{
    EnemyVision vision;
    EnemyHearing hearing;
    EnemyNavigation navigation;
    EnemyStats stats;

    Transform player;
    HealthSystem playerHealth;

    public float attackDistance = 1.8f;
    public float attackCooldown = 2f;

    float attackTimer = 0;

    [Header("Patrulla")]

    public float patrolRadius = 6f;
    public int patrolPointsAmount = 3;

    Vector3[] patrolPoints;
    int currentPatrolIndex = 0;

    float pointReachedDistance = 1.2f;

    public float minDistanceBetweenPoints = 4f;

    enum State
    {
        Patrol,
        Investigate,
        Chase,
        Attack
    }

    State currentState;

    void Start()
    {
        vision = GetComponent<EnemyVision>();
        hearing = GetComponent<EnemyHearing>();
        navigation = GetComponent<EnemyNavigation>();
        stats = GetComponent<EnemyStats>();

        player = vision.player;
        playerHealth = player.GetComponentInChildren<HealthSystem>();

        currentState = State.Patrol;

        GeneratePatrolPoints(transform.position);
    }

    void GeneratePatrolPoints(Vector3 center)
    {
        patrolPoints = new Vector3[patrolPointsAmount];

        int createdPoints = 0;
        int attempts = 0;

        while (createdPoints < patrolPointsAmount && attempts < 30)
        {
            attempts++;

            Vector2 randomCircle = Random.insideUnitCircle * patrolRadius;
            Vector3 candidatePoint = center + new Vector3(randomCircle.x, 0, randomCircle.y);

            bool valid = true;

            for (int i = 0; i < createdPoints; i++)
            {
                float dist = Vector3.Distance(candidatePoint, patrolPoints[i]);

                if (dist < minDistanceBetweenPoints)
                {
                    valid = false;
                    break;
                }
            }

            if (valid)
            {
                patrolPoints[createdPoints] = candidatePoint;
                createdPoints++;
            }
        }

        currentPatrolIndex = 0;
    }
    void Update()
    {
        attackTimer -= Time.deltaTime;

        switch (currentState)
        {
            case State.Patrol:
                UpdatePatrol();
                break;

            case State.Investigate:
                UpdateInvestigate();
                break;

            case State.Chase:
                UpdateChase();
                break;

            case State.Attack:
                UpdateAttack();
                break;
        }
    }

    void UpdatePatrol()
    {
        if (vision.CanSeePlayer())
        {
            currentState = State.Chase;
            return;
        }

        if (hearing.HasHeardSomething())
        {
            currentState = State.Investigate;
            return;
        }

        Vector3 target = patrolPoints[currentPatrolIndex];

        navigation.MoveTo(target);

        float distance = Vector3.Distance(transform.position, target);

        if (distance < pointReachedDistance)
        {
            currentPatrolIndex++;

            if (currentPatrolIndex >= patrolPoints.Length)
            {
                currentPatrolIndex = 0;
            }
        }
    }

    void UpdateChase()
    {
        navigation.MoveTo(player.position);

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= attackDistance)
        {
            currentState = State.Attack;
        }

        if (!vision.CanSeePlayer())
        {
            GeneratePatrolPoints(transform.position);

            currentState = State.Patrol;
        }
    }

    void UpdateAttack()
    {
        navigation.StopMoving();

        transform.LookAt(player);

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > attackDistance)
        {
            currentState = State.Chase;
            return;
        }

        if (attackTimer <= 0)
        {
            Attack();
            attackTimer = attackCooldown;
        }
    }

    void Attack()
    {
        Debug.Log("El pueblerino golpea al jugador");

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(stats.damage);
            Debug.Log("Jugador recibe daño: " + stats.damage + ": Vida restante: " + playerHealth.currentHealth);
        }
    }

    void UpdateInvestigate()
    {
        Vector3 noisePos = hearing.GetNoisePosition();

        navigation.MoveTo(noisePos);

        float distance = Vector3.Distance(transform.position, noisePos);

        if (distance < 1.5f)
        {
            GeneratePatrolPoints(transform.position);
            currentState = State.Patrol;
        }

        if (vision.CanSeePlayer())
        {
            currentState = State.Chase;
        }

        Debug.Log("Investigando ruido");
    }
    void OnDrawGizmos()
    {
        if (patrolPoints == null) return;

        Gizmos.color = Color.green;

        foreach (Vector3 point in patrolPoints)
        {
            Gizmos.DrawSphere(point, 0.3f);
        }
    }
}