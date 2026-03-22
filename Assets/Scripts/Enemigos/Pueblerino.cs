using UnityEngine;

public class Pueblerino : MonoBehaviour
{
    EnemyVision vision;
    EnemyHearing hearing;
    EnemyNavigation navigation;
    EnemyStats stats;

    NoiseEmitter noiseEmitter;

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

    float alertTimer = 0f;
    public float alertDelay = 1f;

    bool alertNoiseEmitted = false;

    float shareTimer = 0f;
    public float shareInterval = 0.3f;

    float wanderTimer;
    public float wanderInterval = 3f;

    enum AlertType
    {
        Vision,
        Sound
    }

    AlertType alertType;

    State nextState;
    enum State
    {
        Patrol,
        Alert,
        Investigate,
        Chase,
        Attack
    }

    State currentState;

    Vector3 currentTarget;
    bool hasExactPlayerPosition = false;
    void Start()
    {
        vision = GetComponent<EnemyVision>();
        hearing = GetComponent<EnemyHearing>();
        navigation = GetComponent<EnemyNavigation>();
        stats = GetComponent<EnemyStats>();

        noiseEmitter = GetComponent<NoiseEmitter>();

        player = vision.player;
        playerHealth = player.GetComponentInChildren<HealthSystem>();

        currentState = State.Patrol;

        GeneratePatrolPoints(transform.position);
    }
    void Update()
    {
        if (GameState.InMenu)
        {
            navigation.Pause();
            return;
        }
        else
        {
            navigation.Resume();
        }

        attackTimer -= Time.deltaTime;

        switch (currentState)
        {
            case State.Patrol:
                UpdatePatrol();
                break;

            case State.Alert:
                UpdateAlert();
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
            nextState = State.Chase;

            float distanceDetect = Vector3.Distance(transform.position, player.position);
            alertTimer = Mathf.Lerp(0.2f, 2f, distanceDetect / vision.visionDistance);

            alertType = AlertType.Vision;
            alertNoiseEmitted = false;

            Debug.Log("Pueblerino ha VISTO al jugador -> entrando en ALERTA VISUAL");

            currentState = State.Alert;
            return;
        }

        if (hearing.HasSharedPlayerPosition())
        {
            currentTarget = hearing.GetSharedPlayerPosition();
            hasExactPlayerPosition = true;

            navigation.MoveTo(currentTarget);
            currentState = State.Investigate;

            Debug.Log(name + " investiga ALERTA de otro enemigo");
            return;
        }

        if (hearing.HasHeardSomething())
        {
            nextState = State.Investigate;

            currentTarget = hearing.GetNoisePosition();
            hasExactPlayerPosition = false;

            float distanceDetect = Vector3.Distance(transform.position, currentTarget);
            alertTimer = Mathf.Lerp(0.2f, 2f, distanceDetect / hearing.hearingDistance);

            alertType = AlertType.Sound;

            Debug.Log("Pueblerino ha ESCUCHADO un ruido -> entrando en ALERTA SONORA");

            currentState = State.Alert;
            return;
        }

        wanderTimer -= Time.deltaTime;

        if (wanderTimer <= 0f)
        {
            Vector3 randomPoint = GetRandomNavPoint(patrolRadius);
            navigation.MoveTo(randomPoint);

            wanderTimer = wanderInterval;
        }
    }

    void UpdateAlert()
    {
        navigation.StopMoving();

        if (alertType == AlertType.Vision && !alertNoiseEmitted)
        {
            Debug.Log("Pueblerino EMITE GRITO DE ALERTA (ruido detectado por enemigos)");
            Debug.Log("Posicion enviada por el Pueblerino: " + player.position);

            noiseEmitter.EmitNoise(2f, player.position);
            alertNoiseEmitted = true;
        }

        alertTimer -= Time.deltaTime;

        if (alertTimer <= 0f)
        {
            currentState = nextState;
        }
    }

    void UpdateInvestigate()
    {
        navigation.MoveTo(currentTarget);

        float distance = Vector3.Distance(transform.position, currentTarget);

        if (distance < 1.5f)
        {
            if (hearing.HasSharedPlayerPosition()) hearing.GetSharedPlayerPosition();
            if (hearing.HasHeardSomething()) hearing.GetNoisePosition();

            GeneratePatrolPoints(currentTarget);
            currentState = State.Patrol;
        }

        if (vision.CanSeePlayer())
        {
            currentState = State.Chase;
        }

        Debug.Log("Pueblerino Investigando ruido");
    }

    void UpdateChase()
    {
        navigation.MoveTo(player.position);

        float distance = Vector3.Distance(transform.position, player.position);

        if (vision.CanSeePlayer())
        {
            shareTimer -= Time.deltaTime;

            if (shareTimer <= 0f)
            {
                noiseEmitter.EmitNoise(1f, player.position);
                shareTimer = shareInterval;
            }
        }

        if (distance <= attackDistance)
        {
            currentState = State.Attack;
        }

        if (!vision.CanSeePlayer())
        {
            currentTarget = player.position;
            hasExactPlayerPosition = true;

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

    Vector3 GetRandomNavPoint(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;

        UnityEngine.AI.NavMeshHit hit;

        if (UnityEngine.AI.NavMesh.SamplePosition(randomDirection, out hit, radius, UnityEngine.AI.NavMesh.AllAreas))
        {
            return hit.position;
        }

        return transform.position;
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