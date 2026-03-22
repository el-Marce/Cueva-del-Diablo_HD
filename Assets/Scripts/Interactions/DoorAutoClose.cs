using UnityEngine;

public class DoorAutoClose : MonoBehaviour
{
    Door door;
    DoorLockCondition lockCondition;
    bool playerInside = false;
    float entryZ = 0f;

    public float delayBeforeClose = 1.5f;
    float timer = 0f;
    bool shouldClose = false;

    void Start()
    {
        door = GetComponentInParent<Door>();
        lockCondition = GetComponentInParent<DoorLockCondition>();
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

        Vector3 localPos = door.transform.InverseTransformPoint(other.transform.position);
        entryZ = localPos.x;
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player") || !playerInside) return;

        playerInside = false;

        Vector3 localPos = door.transform.InverseTransformPoint(other.transform.position);
        float exitZ = localPos.x;

        if (entryZ < 0 && exitZ > 0 || entryZ > 0 && exitZ < 0)
        {
            shouldClose = true;
            timer = 0f;
            if(lockCondition != null)
                lockCondition.enabled = true;
        }
    }
}