using UnityEngine;

public class DoorAutoClose : MonoBehaviour
{
    Door door;
    DoorLockCondition lockCondition;
    Transform player;

    bool playerInside = false;
    float entryDot = 0f;

    public float delayBeforeClose = 1.5f;
    float timer = 0f;
    bool shouldClose = false;

    void Start()
    {
        door = GetComponentInParent<Door>();
        lockCondition = GetComponentInParent<DoorLockCondition>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (door == null || !door.IsOpen()) return;

        if (shouldClose)
        {
            timer += Time.deltaTime;

            if (timer >= delayBeforeClose)
            {
                door.CloseDoor();
                shouldClose = false;
                timer = 0f;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = true;

        Vector3 dir = player.position - door.transform.position;
        entryDot = Vector3.Dot(door.transform.forward, dir);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player") || !playerInside) return;

        playerInside = false;

        Vector3 dir = player.position - door.transform.position;
        float exitDot = Vector3.Dot(door.transform.forward, dir);

        if (entryDot > 0 && exitDot < 0)
        {
            shouldClose = true;
            timer = 0f;

            if (lockCondition != null)
                lockCondition.enabled = true;
        }
    }
}