using UnityEngine;

public class NPC_Controller : MonoBehaviour
{
    public enum State
    {
        Follow,
        Possessed,
        Dead
    }

    public State currentState;

    NPC_Follow follow;

    [Header("Poseído")]
    public GameObject zombiePrefab;

    bool alreadyPossessed = false;

    void Start()
    {
        follow = GetComponent<NPC_Follow>();
        currentState = State.Follow;
    }

    public void Possess()
    {
        if (alreadyPossessed) return;

        alreadyPossessed = true;
        currentState = State.Possessed;

        if (follow != null)
            follow.enabled = false;

        TransformIntoEnemy();
    }

    void TransformIntoEnemy()
    {
        Vector3 pos = transform.position;
        Quaternion rot = transform.rotation;

        Instantiate(zombiePrefab, pos, rot);

        Destroy(gameObject);
    }
}