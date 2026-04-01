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

    [Header("Combate")]
    public float attackDistance = 1.8f;
    public float attackCooldown = 2f;
    public float attackWindUp = 0.6f;
    float attackTimer = 0;
    float attackWindUpTimer = 0f;
    bool isPreparingAttack = false;

    float lookAtTimer = 0f;
    public float lookAtDuration = 0.2f;
    bool isLookingAtPlayer = false;

    [Header("Impacto")]
    public float hitPushForce = 4f;
    public float upwardForce = 1.5f;
    public float cameraShakeIntensity = 0.15f;
    public float cameraShakeDuration = 0.2f;

    [Header("Patrulla")]
    public float patrolRadius = 6f;
    public int patrolPointsAmount = 3;
    public float minDistanceBetweenPoints = 4f;
    public float wanderInterval = 3f;

    Vector3[] patrolPoints;
    int currentPatrolIndex = 0;
    float wanderTimer;

    [Header("Memoria")]
    public float lostPlayerDuration = 3f;
    float lostPlayerTimer = 0f;

    [Header("Alerta")]
    public float alertDelay = 1f;
    float alertTimer = 0f;
    bool alertNoiseEmitted = false;

    [Header("Comunicación")]
    public float shareInterval = 0.3f;
    float shareTimer = 0f;

    [Header("Velocidad")]
    public float chaseSpeedMultiplier;
    public float investigateSpeedMultiplier;

    [Header("Stagger")]
    public float staggerDuration = 0.5f;
    float staggerTimer = 0f;

    enum AlertType { Vision, Sound }
    enum State { Patrol, Alert, Investigate, Chase, Attack }

    AlertType alertType;
    State currentState;
    State nextState;

    Vector3 currentTarget;
    bool hasExactPlayerPosition = false;
    void Start()
    {
        vision = GetComponent<EnemyVision>();
        hearing = GetComponent<EnemyHearing>();
        navigation = GetComponent<EnemyNavigation>();
        stats = GetComponent<EnemyStats>();

        stats.OnHit += OnHit;

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

    void OnHit()
    {
        staggerTimer = staggerDuration;
        ForceChase();
    }

        void UpdatePatrol()
    {
        navigation.ResetSpeed();

        if (vision.CanSeePlayer())
        {
            nextState = State.Chase;

            float distanceDetect = Vector3.Distance(transform.position, player.position);
            alertTimer = Mathf.Lerp(0.2f, 2f, distanceDetect / vision.visionDistance);

            alertType = AlertType.Vision;
            alertNoiseEmitted = false;

            currentState = State.Alert;
            return;
        }

        if (hearing.HasSharedPlayerPosition())
        {
            currentTarget = hearing.GetSharedPlayerPosition();
            hasExactPlayerPosition = true;

            navigation.MoveTo(currentTarget);
            currentState = State.Investigate;
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

            currentState = State.Alert;
            return;
        }

        wanderTimer -= Time.deltaTime;

        if (wanderTimer <= 0f)
        {
            navigation.MoveTo(GetRandomNavPoint(patrolRadius));
            wanderTimer = wanderInterval;
        }
    }

    void UpdateAlert()
    {
        navigation.ResetSpeed();

        navigation.StopMoving();

        if (staggerTimer > 0)
        {
            staggerTimer -= Time.deltaTime;
            return;
        }

        if (alertType == AlertType.Vision && !alertNoiseEmitted)
        {
            noiseEmitter.EmitNoise(2f, player.position);
            alertNoiseEmitted = true;
        }

        alertTimer -= Time.deltaTime;

        if (alertTimer <= 0f)
            currentState = nextState;
    }

    void UpdateInvestigate()
    {
        navigation.SetSpeedMultiplier(investigateSpeedMultiplier);

        if (hearing.HasHeardSomething())
        {
            currentTarget = hearing.GetNoisePosition();
            hasExactPlayerPosition = false;

            navigation.MoveTo(currentTarget);

            return;
        }

        if (hearing.HasSharedPlayerPosition())
        {
            currentTarget = hearing.GetSharedPlayerPosition();
            hasExactPlayerPosition = true;

            navigation.MoveTo(currentTarget);

            return;
        }

        navigation.MoveTo(currentTarget);

        float distance = Vector3.Distance(transform.position, currentTarget);

        if (distance < 1.5f)
        {
            GeneratePatrolPoints(currentTarget);
            currentState = State.Patrol;
        }

        if (vision.CanSeePlayer())
            currentState = State.Chase;
    }

    void UpdateChase()
    {
        navigation.SetSpeedMultiplier(chaseSpeedMultiplier);

        float distance = Vector3.Distance(transform.position, player.position);

        if (vision.CanSeePlayer())
        {
            lostPlayerTimer = lostPlayerDuration;

            navigation.MoveTo(player.position);

            shareTimer -= Time.deltaTime;
            if (shareTimer <= 0f)
            {
                noiseEmitter.EmitNoise(1f, player.position);
                shareTimer = shareInterval;
            }
        }
        else
        {
            lostPlayerTimer -= Time.deltaTime;

            if (lostPlayerTimer > 0f)
            {
                navigation.MoveTo(player.position);
            }
            else
            {
                GeneratePatrolPoints(transform.position);
                currentState = State.Patrol;
                return;
            }
        }

        if (distance <= attackDistance)
        {
            currentState = State.Attack;
            isPreparingAttack = true;
            attackWindUpTimer = attackWindUp;
        }
    }

    void UpdateAttack()
    {
        navigation.ResetSpeed();

        navigation.StopMoving();

        if (isLookingAtPlayer)
        {
            Vector3 target = player.position;
            // si quieres mantener el "cabezazo", NO limites el eje Y
            // si quieres solo rotación horizontal, descomenta la siguiente línea:
            // target.y = transform.position.y;

            transform.LookAt(target);

            lookAtTimer -= Time.deltaTime;

            if (lookAtTimer <= 0f)
            {
                isLookingAtPlayer = false;
            }
        }

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > attackDistance)
        {
            currentState = State.Chase;
            return;
        }

        if (isPreparingAttack)
        {
            attackWindUpTimer -= Time.deltaTime;

            if (attackWindUpTimer <= 0f)
            {
                isPreparingAttack = false;
            }

            return;
        }

        if (attackTimer <= 0)
        {
            isLookingAtPlayer = true;
            lookAtTimer = lookAtDuration;

            Attack();
            attackTimer = attackCooldown;

            isPreparingAttack = true;
            attackWindUpTimer = attackWindUp; ;
        }
    }

    void Attack()
    {
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(stats.damage);
            Debug.Log("Jugador recibe daño: " + stats.damage + ": Vida restante: " + playerHealth.currentHealth);
        }

        Rigidbody playerRb = player.GetComponent<Rigidbody>();

        if (playerRb != null)
        {
            Vector3 dir = (player.position - transform.position).normalized;
            dir.y = 0f;

            Vector3 force = dir * hitPushForce + Vector3.up * upwardForce;

            playerRb.AddForce(force, ForceMode.Impulse);
        }

        // Efecto de cámara (si existe)
        PlayerCameraEffects camEffects = player.GetComponentInChildren<PlayerCameraEffects>();

        if (camEffects != null)
        {
            Vector3 hitDir = player.position - transform.position;
            camEffects.HitImpact(hitDir, cameraShakeIntensity, cameraShakeDuration);
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

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Debug.Log("Contactu");
            ForceChase();
        }
    }

    public void OnPushed()
    {
        ForceChase();
    }

    Vector3 GetRandomNavPoint(float radius)
    {
        Vector3 random = Random.insideUnitSphere * radius + transform.position;

        if (UnityEngine.AI.NavMesh.SamplePosition(random, out var hit, radius, UnityEngine.AI.NavMesh.AllAreas))
            return hit.position;

        return transform.position;
    }
    void ForceChase()
    {
        alertType = AlertType.Vision;
        nextState = State.Chase;

        alertTimer = 0.1f;
        alertNoiseEmitted = false;

        navigation.StopMoving();

        isPreparingAttack = false;

        currentState = State.Alert;
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