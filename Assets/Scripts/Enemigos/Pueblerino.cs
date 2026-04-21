using UnityEngine;
using Cinemachine;
using System.Collections;

public class Pueblerino : MonoBehaviour
{
    Renderer rend;
    Material defaultMat;
    public Material windUpMat;

    EnemyVision vision;
    EnemyHearing hearing;
    EnemyNavigation navigation;
    EnemyStats stats;
    NoiseEmitter noiseEmitter;

    Transform player;
    HealthSystem playerHealth;

    CinemachineImpulseSource impulseSource;

    Animator anim;
    
    [Header("Combate")]
    public float attackDistance = 1.8f;
    public float attackCooldown = 2f;
    public float attackWindUp = 0.6f;

    float attackTimer = 0;
    public float attackWindUpTimer = 0f;

    public bool isPreparingAttack = false;
    bool wasPreparingAttack = false;

    float lookAtTimer = 0f;
    public float lookAtDuration = 0.2f;
    bool isLookingAtPlayer = false;

    [Header("Impacto")]
    public float hitPushForce = 4f;
    public float upwardForce = 1.5f;

    public float cameraShakeIntensity = 1.2f;
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

    [Header("Rotación")]
    public float rotationSpeed = 8f;
    bool isDying = false;

    enum AlertType { Vision, Sound }
    enum State { Patrol, Alert, Investigate, Chase, Attack }

    AlertType alertType;
    State currentState;
    State nextState;

    Vector3 currentTarget;
    bool hasExactPlayerPosition = false;

    void Start()
    {
        rend = GetComponent<Renderer>();
        defaultMat = rend.material;

        vision = GetComponent<EnemyVision>();
        hearing = GetComponent<EnemyHearing>();
        navigation = GetComponent<EnemyNavigation>();
        stats = GetComponent<EnemyStats>();
        anim = GetComponentInChildren<Animator>();

        stats.OnHit += OnHit;

        noiseEmitter = GetComponent<NoiseEmitter>();

        player = vision.player;
        playerHealth = player.GetComponentInChildren<HealthSystem>();

        impulseSource = GetComponent<CinemachineImpulseSource>();

        currentState = State.Patrol;

        GeneratePatrolPoints(transform.position);
        if (anim != null)
            anim.speed = stats.animationSpeed;
    }

    void Update()
    {
        if (stats.IsDead)
        {
            if (!isDying)
                StartCoroutine(HandleDeath());

            return;
        }
        if (GameState.InMenu)
        {
            navigation.Pause();
            return;
        }
        else
        {
            navigation.Resume();
        }

        //Debug.Log(currentState);

        if (isPreparingAttack && !wasPreparingAttack)
        {
            //Debug.Log("ENTER WIND-UP");
            anim.ResetTrigger("IsPushed");
            anim.SetBool("Attack", true);
            rend.material = windUpMat;
        }

        if (!isPreparingAttack && wasPreparingAttack)
        {
            //Debug.Log("EXIT WIND-UP");
            rend.material = defaultMat;
            anim.SetBool("Attack", false);
        }

        wasPreparingAttack = isPreparingAttack;

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

    IEnumerator HandleDeath()
    {
        isDying = true;

        anim.SetTrigger("IsDead");

        yield return null;

        float deathDuration = anim.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(deathDuration + 1);

        if (navigation.agent.enabled && navigation.agent.isOnNavMesh)
        {
            navigation.agent.isStopped = true;
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            GetComponent<CapsuleCollider>().enabled = false;
            //navigation.agent.velocity = Vector3.zero;
            //navigation.agent.updatePosition = false;
            //navigation.agent.updateRotation = false;
        }

        vision.enabled = false;
        hearing.enabled = false;
    }
    void OnHit()
    {
        staggerTimer = staggerDuration;
        if (currentState != State.Attack)
            ForceChase();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            if (currentState == State.Attack) return;
            //Debug.Log("Contactu");
            ForceChase();
        }
    }

    public void OnPushed()
    {
        anim.SetTrigger("IsPushed");
        staggerTimer = staggerDuration * 2f;
        ForceChase();
    }

    void UpdatePatrol()
    {
        //navigation.ResetSpeed();
        navigation.agent.speed = stats.patrolSpeed;
        if (navigation.agent.hasPath)
            RotateTowards(navigation.agent.steeringTarget);

        if (vision.CanSeePlayer())
        {
            nextState = State.Chase;

            float distanceDetect = Vector3.Distance(transform.position, player.position);
            alertTimer = Mathf.Lerp(0.2f, 2f, distanceDetect / vision.visionDistance);

            alertType = AlertType.Vision;
            anim.SetInteger("AlertType", 1);
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
            anim.SetInteger("AlertType", 2);
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
        {
            anim.SetInteger("AlertType", 0); // <- resetear al salir

            if (nextState == State.Chase)
                anim.SetBool("IsChasing", true);
            else if (nextState == State.Investigate)
                anim.SetBool("IsInvestigating", true);

            currentState = nextState;
        }
    }

    void UpdateInvestigate()
    {
        navigation.SetSpeedMultiplier(investigateSpeedMultiplier);

        if (navigation.agent.hasPath)
            RotateTowards(navigation.agent.steeringTarget);

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
            anim.SetBool("IsInvestigating", false);
            GeneratePatrolPoints(currentTarget);
            currentState = State.Patrol;
        }

        if (vision.CanSeePlayer())
        {
            anim.SetBool("IsInvestigating", false);
            anim.SetBool("IsChasing", true);
            currentState = State.Chase;
        }
    }

    void UpdateChase()
    {
        navigation.SetSpeedMultiplier(chaseSpeedMultiplier);
        RotateTowardsPlayer();
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
                anim.SetBool("IsChasing", false);
                currentState = State.Patrol;
                return;
            }
        }

        if (distance <= attackDistance)
        {
            anim.SetBool("IsChasing", false);
            currentState = State.Attack;
            attackTimer = 0.2f;
            return;
        }
    }

    void UpdateAttack()
    {
        if (!isPreparingAttack)
        {
            attackTimer -= Time.deltaTime;
            //Debug.Log("Cooldown: " + attackTimer);

            if (attackTimer <= 0f)
            {                
                isPreparingAttack = true;
                attackWindUpTimer = attackWindUp;
                //Debug.Log("Wind-up iniciado. attackWindUp vale: " + attackWindUp);
            }
        }

        navigation.ResetSpeed();

        navigation.StopMoving();

        if (isLookingAtPlayer)
        {
            Vector3 target = player.position;

            // mantener el "cabezazo", NO limites el eje Y
            // solo rotación horizontal, descomenta la siguiente línea:
            // target.y = transform.position.y;

            RotateTowardsPlayer();

            lookAtTimer -= Time.deltaTime;

            if (lookAtTimer <= 0f)
            {
                isLookingAtPlayer = false;
            }
        }

        float distance = Vector3.Distance(transform.position, player.position);

        anim.SetFloat("AttackDistance", distance);

        if (distance > attackDistance)
        {
            anim.SetBool("Attack", false);
            anim.SetBool("IsChasing", true);
            isPreparingAttack = false;
            currentState = State.Chase;
            return;
        }

        if (isPreparingAttack)
        {
            if (distance > attackDistance)  // <- añadir
            {
                isPreparingAttack = false;
                currentState = State.Chase;
                return;
            }

            attackWindUpTimer -= Time.deltaTime;
            //Debug.Log("WindUp timer: " + attackWindUpTimer);
            if (attackWindUpTimer <= 0f)
            {

                attackTimer = attackCooldown;
                //Debug.Log("Nuevo Timer: " + attackTimer);
                Attack();

                isPreparingAttack = false;

                isLookingAtPlayer = true;
                lookAtTimer = lookAtDuration;

                return; 
            }
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

        if (impulseSource != null)
        {
            Vector3 hitDir = (player.position - transform.position).normalized;

            Vector3 sideDir = Vector3.Cross(Vector3.up, hitDir).normalized;

            if (Random.value > 0.5f)
                sideDir *= -1f;

            impulseSource.GenerateImpulse(sideDir * cameraShakeIntensity);
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
        Vector3 random = Random.insideUnitSphere * radius + transform.position;

        if (UnityEngine.AI.NavMesh.SamplePosition(random, out var hit, radius, UnityEngine.AI.NavMesh.AllAreas))
            return hit.position;

        return transform.position;
    }


    void ForceChase()
    {
        anim.SetBool("IsInvestigating", false);
        anim.SetBool("IsChasing", true); 
        anim.SetInteger("AlertType", 0);

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
    void RotateTowardsPlayer()
    {
        Vector3 dir = (player.position - transform.position);
        dir.y = 0f;
        if (dir == Vector3.zero) return;
        Quaternion targetRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);
    }
    void RotateTowards(Vector3 target)
    {
        Vector3 dir = (target - transform.position);
        dir.y = 0f;
        if (dir == Vector3.zero) return;
        Quaternion targetRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);
    }
    public void Freeze()
    {
        currentState = State.Patrol;
        navigation.Pause();
        hearing.enabled = false;
        vision.enabled = false;
        this.enabled = false;
    }
}