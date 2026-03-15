using UnityEngine;
public class EntePsicologico : MonoBehaviour
{
    EnemyVision vision;
    EnemyHearing hearing;
    EnemyNavigation navigation;
    FloatMotion floatMotion;
    Transform player;

    SanitySystem playerSanity;

    float floatTimer = 0.1f;
    public float sanityDamagePerSecond;
    public float effectDistance;

    float wanderTimer;
    public float wanderInterval;
    public float wanderRadius;

    float alertTimer = 0f;
    public float alertDelay = 1f;

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

        floatMotion.enabled = false;

        player = vision.player;
        playerSanity = player.GetComponentInChildren<SanitySystem>();

        currentState = State.Idle;
    }

    void Update()
    {
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

            currentState = State.Alert;
        }

        if (hearing.HasHeardSomething())
        {
            nextState = State.HuntSound;

            Vector3 noisePos = hearing.GetNoisePosition();
            float distanceDetect = Vector3.Distance(transform.position, noisePos);
            alertTimer = Mathf.Lerp(0.2f, 2f, distanceDetect / hearing.hearingDistance);

            currentState = State.Alert;
        }
    }

    void UpdateAlert()
    {
        navigation.StopMoving();

        alertTimer -= Time.deltaTime;

        if (alertTimer <= 0f)
        {
            currentState = nextState;
        }
    }
    void UpdateHuntSound()
    {
        floatMotion.SetOffset(1f);
        floatMotion.EnableOscillation(true);

        Vector3 noisePos = hearing.GetNoisePosition();

        navigation.MoveTo(noisePos);

        float distance = Vector3.Distance(transform.position, noisePos);

        if (distance < 2f)
        {
            currentState = State.Idle;
        }

        if (vision.CanSeePlayer())
        {
            currentState = State.Chase;
        }
        Debug.Log("Investigando ruido");
    }
    void UpdateChase()
    {
        floatMotion.SetOffset(1f);
        floatMotion.EnableOscillation(true);

        navigation.MoveTo(player.position);

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= effectDistance)
        {
            currentState = State.AffectMind;
        }
    }

    void UpdateAffectMind()
    {
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