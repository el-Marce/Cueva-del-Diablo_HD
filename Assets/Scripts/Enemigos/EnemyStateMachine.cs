using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{
    EnemyVision vision;
    EnemyNavigation navigation;

    Transform player;

    enum State
    {
        Idle,
        Chase
    }

    State currentState;

    void Start()
    {
        vision = GetComponent<EnemyVision>();
        navigation = GetComponent<EnemyNavigation>();

        player = vision.player;

        currentState = State.Idle;
    }

    void Update()
    {
        if (vision.CanSeePlayer())
        {
            currentState = State.Chase;
        }

        if (currentState == State.Chase)
        {
            navigation.MoveTo(player.position);
        }

        //if (currentState == State.Chase)
           // Debug.Log(currentState);
    }
}