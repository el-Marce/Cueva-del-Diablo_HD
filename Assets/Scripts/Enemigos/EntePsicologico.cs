using UnityEngine;
public class EntePsicologico : MonoBehaviour
{
    EnemyVision vision;
    EnemyHearing hearing;
    EnemyNavigation navigation;
    FloatMotion floatMotion;
    Transform player;

    NoiseEmitter noiseEmitter;

    SanitySystem playerSanity;

    float floatTimer = 0.1f;
    public float sanityDamagePerSecond;
    public float effectDistance;

    float wanderTimer;
    public float wanderInterval;
    public float wanderRadius;

    float alertTimer = 0f;
    public float alertDelay = 1f;

    bool alertNoiseEmitted = false;

    Vector3 currentTarget;
    bool hasExactPlayerPosition = false;

    float shareTimer = 0f;
    public float shareInterval = 0.3f;

    [Header("Velocidad")]
    public float chaseSpeedMultiplier;
    public float investigateSpeedMultiplier;

    [Header("Repulsión")]
    public float repelDuration = 10f;
    float repelTimer = 0f;
    enum AlertType
    {
        Vision,
        Sound
    }

    AlertType alertType;

    State nextState;
    enum State
    {
        Idle,
        Alert,
        HuntSound,
        Chase,
        AffectMind,
        Repelled
    }

    State currentState;

    void Start()
    {
        vision = GetComponent<EnemyVision>();
        hearing = GetComponent<EnemyHearing>();
        navigation = GetComponent<EnemyNavigation>();
        floatMotion = GetComponent<FloatMotion>();
        noiseEmitter = GetComponent<NoiseEmitter>();

        floatMotion.enabled = false;

        player = vision.player;
        playerSanity = player.GetComponentInChildren<SanitySystem>();

        currentState = State.Idle;
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

        if (!floatMotion.enabled)
        {
            floatTimer -= Time.deltaTime;

            if (floatTimer <= 0f)
            {
                floatMotion.enabled = true;
            }
        }

        switch (currentState)
        {
            case State.Idle:
                UpdateIdle();
                break;

            case State.Alert:
                UpdateAlert();
                break;

            case State.HuntSound:
                UpdateHuntSound();
                break;

            case State.Chase:
                UpdateChase();
                break;

            case State.AffectMind:
                UpdateAffectMind();
                break;

            case State.Repelled: 
                UpdateRepelled(); 
                break;
        }
    }

    void UpdateIdle()
    {
        navigation.ResetSpeed();

        floatMotion.SetOffset(5f);
        floatMotion.EnableOscillation(true);

        wanderTimer -= Time.deltaTime;
        if (wanderTimer <= 0f)
        {
            Vector3 randomPoint = GetRandomNavPoint(wanderRadius);
            navigation.MoveTo(randomPoint);

            wanderTimer = wanderInterval;
        }

        if (vision.CanSeePlayer())
        {
            Debug.Log("[Ente] Ve al jugador -> Alert -> Chase");
            nextState = State.Chase;

            float distanceDetect = Vector3.Distance(transform.position, player.position);
            alertTimer = Mathf.Lerp(0.2f, 2f, distanceDetect / vision.visionDistance);

            alertType = AlertType.Vision;
            alertNoiseEmitted = false;

            currentState = State.Alert;
        }

        if (hearing.HasSharedPlayerPosition())
        {
            Debug.Log("[Ente] Escucha posición compartida -> HuntSound");
            currentTarget = hearing.GetSharedPlayerPosition();
            hasExactPlayerPosition = true;

            navigation.MoveTo(currentTarget);
            currentState = State.HuntSound;
        }
        else if (hearing.HasHeardSomething())
        {
            Debug.Log("[Ente] Escucha ruido -> Alert -> HuntSound");
            nextState = State.HuntSound;

            currentTarget = hearing.GetNoisePosition();
            hasExactPlayerPosition = false;

            float distanceDetect = Vector3.Distance(transform.position, currentTarget);
            alertTimer = Mathf.Lerp(0.2f, 2f, distanceDetect / hearing.hearingDistance);

            alertType = AlertType.Sound;

            currentState = State.Alert;
        }
    }

    void UpdateAlert()
    {
        navigation.ResetSpeed();

        navigation.StopMoving();

        if (alertType == AlertType.Vision && !alertNoiseEmitted)
        {
            noiseEmitter.EmitNoise(2f, player.position);

            alertNoiseEmitted = true;
        }

        alertTimer -= Time.deltaTime;

        if (alertTimer <= 0f)
        {
            currentState = nextState;
        }
    }
    void UpdateHuntSound()
    {
        navigation.SetSpeedMultiplier(investigateSpeedMultiplier);

        floatMotion.SetOffset(1f);
        floatMotion.EnableOscillation(true);

        if (hearing.HasSharedPlayerPosition())
        {
            currentTarget = hearing.GetSharedPlayerPosition();
            hasExactPlayerPosition = true;

            navigation.MoveTo(currentTarget);

            return;
        }

        if (hearing.HasHeardSomething())
        {
            currentTarget = hearing.GetNoisePosition();
            hasExactPlayerPosition = false;

            navigation.MoveTo(currentTarget);

            return;
        }

        navigation.MoveTo(currentTarget);

        float distance = Vector3.Distance(transform.position, currentTarget);

        if (distance < 2f)
        {
            Debug.Log("[Ente] Llegó al objetivo, vuelve a Idle");
            currentState = State.Idle;
        }

        if (vision.CanSeePlayer())
        {
            Debug.Log("[Ente] Ve al jugador desde HuntSound -> Chase");
            currentState = State.Chase;
        }
        //Debug.Log("Ente Investigando ruido");
    }
    void UpdateChase()
    {
        navigation.SetSpeedMultiplier(chaseSpeedMultiplier);

        floatMotion.SetOffset(1f);
        floatMotion.EnableOscillation(true);

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

        if (distance <= effectDistance)
        {
            Debug.Log("[Ente] En rango -> AffectMind");
            currentState = State.AffectMind;
        }

        if (!vision.CanSeePlayer())
        {
            Debug.Log("[Ente] Perdió al jugador -> HuntSound");
            currentTarget = player.position;
            hasExactPlayerPosition = true;

            currentState = State.HuntSound;
        }
    }

    void UpdateAffectMind()
    {
        navigation.ResetSpeed();

        floatMotion.SetOffset(2.5f);
        floatMotion.EnableOscillation(false);

        navigation.StopMoving();

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > effectDistance)
        {
            Debug.Log("[Ente] Jugador escapó -> Chase");
            currentState = State.Chase;
            return;
        }

        AffectSanity();
    }

    void AffectSanity()
    {
        playerSanity.DecreaseSanity(sanityDamagePerSecond * Time.deltaTime);
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

    Vector3 savedFleeDir; // <- añadir variable

    public void Repel()
    {
        // Guardar dirección opuesta al jugador en el momento del impacto
        Vector3 myPos = new Vector3(navigation.agent.nextPosition.x, 0f, navigation.agent.nextPosition.z);
        Vector3 playerPos = new Vector3(player.position.x, 0f, player.position.z);

        savedFleeDir = (myPos - playerPos).normalized;

        // Si está encima del jugador, usar dirección aleatoria
        if (savedFleeDir.magnitude < 0.1f)
            savedFleeDir = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;

        repelTimer = repelDuration;
        currentState = State.Repelled;
        Debug.Log("[Ente] Repelido | fleeDir guardado: " + savedFleeDir);
    }

    void UpdateRepelled()
    {
        navigation.SetSpeedMultiplier(chaseSpeedMultiplier);
        floatMotion.SetOffset(5f);
        floatMotion.EnableOscillation(true);

        Vector3 agentPos = new Vector3(
            navigation.agent.nextPosition.x, 0f,
            navigation.agent.nextPosition.z
        );

        Vector3 fleeTarget = agentPos + savedFleeDir * 15f;

        if (UnityEngine.AI.NavMesh.SamplePosition(fleeTarget, out UnityEngine.AI.NavMeshHit hit, 15f, UnityEngine.AI.NavMesh.AllAreas))
            currentTarget = hit.position;
        else
            currentTarget = fleeTarget;

        navigation.MoveTo(currentTarget);

        repelTimer -= Time.deltaTime;
        if (repelTimer <= 0f)
        {
            currentState = State.Idle;
            Debug.Log("[Ente] Repulsión terminada -> Idle");
        }
    }
}