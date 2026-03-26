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
        AffectMind
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
            nextState = State.Chase;

            float distanceDetect = Vector3.Distance(transform.position, player.position);
            alertTimer = Mathf.Lerp(0.2f, 2f, distanceDetect / vision.visionDistance);

            alertType = AlertType.Vision;
            alertNoiseEmitted = false;

            currentState = State.Alert;
        }

        if (hearing.HasSharedPlayerPosition())
        {
            currentTarget = hearing.GetSharedPlayerPosition();
            hasExactPlayerPosition = true;

            navigation.MoveTo(currentTarget);
            currentState = State.HuntSound;
        }
        else if (hearing.HasHeardSomething())
        {
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
            currentState = State.Idle;
        }

        if (vision.CanSeePlayer())
        {
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
            currentState = State.AffectMind;
        }

        if (!vision.CanSeePlayer())
        {
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
}