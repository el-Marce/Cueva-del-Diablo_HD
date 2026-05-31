using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;
using FMOD.Studio;

public class EntePsicologico : MonoBehaviour
{
    EnemyVision vision;
    EnemyHearing hearing;
    EnemyNavigation navigation;
    FloatMotion floatMotion;
    NoiseEmitter noiseEmitter;
    Transform player;
    SanitySystem playerSanity;

    enum State { Idle, Alert, HuntSound, Chase, AffectMind, Repelled }
    enum AlertType { Vision, Sound }

    State currentState;
    State nextState;
    AlertType alertType;

    [Header("Flotación")]
    public float wanderRadius;
    public float wanderInterval;
    float wanderTimer;
    float floatTimer = 0.1f;

    [Header("Velocidad")]
    public float chaseSpeedMultiplier;
    public float investigateSpeedMultiplier;

    [Header("Alerta")]
    public float alertDelay = 1f;
    float alertTimer = 0f;
    bool alertNoiseEmitted = false;

    [Header("Comunicación")]
    public float shareInterval = 0.3f;
    float shareTimer = 0f;

    [Header("Memoria")]
    public float chaseMemoryDuration = 2.5f;
    float chaseMemoryTimer = 0f;
    Vector3 currentTarget;
    bool hasExactPlayerPosition = false;

    [Header("Daño Mental")]
    public float effectDistance;
    public float sanityDamagePerSecond;
    public float damageDelay = 2f;
    float damageDelayTimer = 0f;

    [Header("Repulsión")]
    public float repelDuration = 10f;
    float repelTimer = 0f;

    [Header("Muerte")]
    public GameObject shockwaveEffect;

    [Header("Audio")]
    public EventReference sfxAlert;
    public EventReference sfxRepelled;
    public EventReference loopIdle;
    public EventReference loopHuntSound;
    public EventReference loopChase;
    public EventReference loopAffectMind;

    EventInstance currentLoop;
    State audioState = (State)(-1); // estado inválido para forzar el primer cambio

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

    // --- Audio helpers ---

    void UpdateAudio()
    {
        if (currentState == audioState) return;
        audioState = currentState;

        AudioManager.Instance.StopLoop(currentLoop);
        currentLoop = default;

        switch (currentState)
        {
            case State.Idle:
                currentLoop = AudioManager.Instance.CreateLoop(loopIdle, transform);
                break;
            case State.Alert:
                AudioManager.Instance.Play(sfxAlert, transform.position);
                break;
            case State.HuntSound:
                currentLoop = AudioManager.Instance.CreateLoop(loopHuntSound, transform);
                break;
            case State.Chase:
                currentLoop = AudioManager.Instance.CreateLoop(loopChase, transform);
                break;
            case State.AffectMind:
                currentLoop = AudioManager.Instance.CreateLoop(loopAffectMind, transform);
                break;
            case State.Repelled:
                AudioManager.Instance.Play(sfxRepelled, transform.position);
                break;
        }
    }

    // --- Update principal ---

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

        if (currentState == State.Repelled)
        {
            UpdateAudio();
            UpdateRepelled();
            return;
        }

        if (!floatMotion.enabled)
        {
            floatTimer -= Time.deltaTime;
            if (floatTimer <= 0f)
                floatMotion.enabled = true;
        }

        UpdateAudio();

        switch (currentState)
        {
            case State.Idle: UpdateIdle(); break;
            case State.Alert: UpdateAlert(); break;
            case State.HuntSound: UpdateHuntSound(); break;
            case State.Chase: UpdateChase(); break;
            case State.AffectMind: UpdateAffectMind(); break;
            case State.Repelled: UpdateRepelled(); break;
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
            currentTarget = player.position;
            chaseMemoryTimer = chaseMemoryDuration;
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
            currentState = nextState;
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
            currentState = State.Idle;

        if (vision.CanSeePlayer())
            currentState = State.Chase;

        RotarHaciaObjetivo(currentTarget);
    }

    void UpdateChase()
    {
        navigation.SetSpeedMultiplier(chaseSpeedMultiplier);
        floatMotion.SetOffset(1f);
        floatMotion.EnableOscillation(true);

        if (vision.CanSeePlayer())
        {
            currentTarget = player.position;
            chaseMemoryTimer = chaseMemoryDuration;
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
            chaseMemoryTimer -= Time.deltaTime;

            float distToTarget = Vector3.Distance(transform.position, currentTarget);
            if (distToTarget > 2f)
            {
                navigation.MoveTo(currentTarget);
            }
            else
            {
                wanderTimer -= Time.deltaTime;
                if (wanderTimer <= 0f)
                {
                    Vector3 searchPoint = GetRandomNavPoint(wanderRadius * 0.5f);
                    navigation.MoveTo(searchPoint);
                    wanderTimer = wanderInterval * 0.5f;
                }
            }

            if (chaseMemoryTimer <= 0f)
            {
                if (hearing.HasSharedPlayerPosition() || hearing.HasHeardSomething())
                    currentState = State.HuntSound;
                else
                    currentState = State.Idle;
                return;
            }
        }

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= effectDistance)
            currentState = State.AffectMind;

        RotarHaciaObjetivo(currentTarget);
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
            damageDelayTimer = 0f;
            currentState = State.Chase;
            return;
        }

        damageDelayTimer += Time.deltaTime;
        damageDelayTimer += Time.deltaTime;
        if (damageDelayTimer >= damageDelay)
        {
            playerSanity.DecreaseSanity(sanityDamagePerSecond * Time.deltaTime);
            Debug.Log("Jugador recibe daño psicologico, Vida restante: " + playerSanity.currentSanity);
        }
    }
    void RotarHaciaObjetivo(Vector3 target)
    {
        Vector3 dir = new Vector3(target.x, transform.position.y, target.z) - transform.position;
        if (dir.sqrMagnitude < 0.01f) return;
        Quaternion targetRot = Quaternion.LookRotation(dir.normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 8f);
    }
    Vector3 GetRandomNavPoint(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        UnityEngine.AI.NavMeshHit hit;
        if (UnityEngine.AI.NavMesh.SamplePosition(randomDirection, out hit, radius, UnityEngine.AI.NavMesh.AllAreas))
            return hit.position;
        return transform.position;
    }

    public void Repel()
    {
        repelTimer = repelDuration;
        currentState = State.Repelled;

        Vector3 bestPoint = transform.position;
        float bestDistance = 0f;

        // Radio ampliado al doble para buscar puntos más lejanos
        for (int i = 0; i < 40; i++)
        {
            Vector3 candidate = GetRandomNavPoint(wanderRadius * 2f);

            Vector3 candidateFlat = new Vector3(candidate.x, 0f, candidate.z);
            Vector3 playerFlat = new Vector3(player.position.x, 0f, player.position.z);
            float distFromPlayer = Vector3.Distance(candidateFlat, playerFlat);

            if (distFromPlayer > bestDistance)
            {
                bestDistance = distFromPlayer;
                bestPoint = candidate;
            }
        }

        currentTarget = bestPoint;
        navigation.MoveTo(currentTarget);
    }

    void UpdateRepelled()
    {
        navigation.SetSpeedMultiplier(chaseSpeedMultiplier);
        floatMotion.SetOffset(5f);
        floatMotion.EnableOscillation(true);

        // Rotar hacia el punto de huida, no hacia el jugador
        RotarHaciaObjetivo(currentTarget);

        repelTimer -= Time.deltaTime;
        if (repelTimer <= 0f)
        {
            currentState = State.Idle;
            return;
        }

        float distanceToTarget = Vector3.Distance(transform.position, currentTarget);
        if (distanceToTarget < 2f)
        {
            Vector3 playerFlat = new Vector3(player.position.x, 0f, player.position.z);
            Vector3 selfFlat = new Vector3(transform.position.x, 0f, transform.position.z);
            float currentDistFromPlayer = Vector3.Distance(selfFlat, playerFlat);

            Vector3 bestPoint = transform.position;
            float bestDistance = 0f;

            for (int i = 0; i < 20; i++)
            {
                Vector3 candidate = GetRandomNavPoint(wanderRadius * 4f);
                Vector3 candidateFlat = new Vector3(candidate.x, 0f, candidate.z);
                float distFromPlayer = Vector3.Distance(candidateFlat, playerFlat);

                if (distFromPlayer > bestDistance)
                {
                    bestDistance = distFromPlayer;
                    bestPoint = candidate;
                }
            }

            if (bestDistance > currentDistFromPlayer)
                currentTarget = bestPoint;

            navigation.MoveTo(currentTarget);
        }
    }
    public void Die()
    {
        AudioManager.Instance.StopLoop(currentLoop);
        currentLoop = default;
        StartCoroutine(DeathSequence());
    }

    IEnumerator DeathSequence()
    {
        currentState = State.Idle;
        navigation.Pause();
        hearing.enabled = false;
        vision.enabled = false;
        this.enabled = false;

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        float duration = 1.8f;
        float elapsed = 0f;
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + Vector3.up * 4f;
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            transform.position = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0f, 1f, t));
            foreach (Renderer r in renderers)
            {
                foreach (Material mat in r.materials)
                {
                    if (mat.HasProperty("_BaseColor"))
                    {
                        Color c = mat.GetColor("_BaseColor");
                        c.a = Mathf.Lerp(1f, 0f, t);
                        mat.SetColor("_BaseColor", c);
                    }
                }
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (shockwaveEffect != null)
        {
            Renderer mainRenderer = GetComponentInChildren<Renderer>();
            Vector3 spawnPos = mainRenderer != null ? mainRenderer.bounds.center : transform.position;
            Instantiate(shockwaveEffect, spawnPos, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    public void Freeze()
    {
        AudioManager.Instance.StopLoop(currentLoop);
        currentLoop = default;
        currentState = State.Idle;
        navigation.Pause();
        hearing.enabled = false;
        vision.enabled = false;
        this.enabled = false;
    }
}