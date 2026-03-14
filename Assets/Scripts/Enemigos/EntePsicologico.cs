using UnityEngine;

public class EntePsicologico : MonoBehaviour
{
    EnemyVision vision;
    EnemyNavigation navigation;

    Transform player;

    SanitySystem playerSanity;

    public float sanityDamagePerSecond = 10f;
    public float effectDistance = 6f;

    enum State
    {
        Idle,
        Chase,
        AffectMind
    }

    State currentState;

    void Start()
    {
        vision = GetComponent<EnemyVision>();
        navigation = GetComponent<EnemyNavigation>();

        player = vision.player;

        playerSanity = player.GetComponentInChildren<SanitySystem>();

        currentState = State.Idle;
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Idle:
                UpdateIdle();
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
        if (vision.CanSeePlayer())
        {
            currentState = State.Chase;
        }
    }

    void UpdateChase()
    {
        navigation.MoveTo(player.position);

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= effectDistance)
        {
            currentState = State.AffectMind;
        }
    }

    void UpdateAffectMind()
    {
        navigation.StopMoving();

        transform.LookAt(player);

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
}