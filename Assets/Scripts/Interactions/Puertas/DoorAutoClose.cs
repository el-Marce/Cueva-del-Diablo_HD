//using UnityEngine;

//public class DoorAutoClose : MonoBehaviour
//{
//    Door door;
//    DoorLockCondition lockCondition;
//    Transform player;

//    bool playerInside = false;

//    float previousDot;
//    float currentDot;

//    bool hasCrossed = false;

//    public float distanceToClose = 3f;
//    public float delayBeforeClose = 1.5f;

//    float timer = 0f;
//    bool shouldClose = false;

//    Vector3 doorPosition;
//    public float noiseEmitterAlcance;

//    NoiseEmitter noiseEmitter;

//    void Start()
//    {
//        door = GetComponentInParent<Door>();
//        lockCondition = GetComponentInParent<DoorLockCondition>();
//        player = GameObject.FindGameObjectWithTag("Player").transform;

//        noiseEmitter = GetComponent<NoiseEmitter>();

//        if (door != null)
//            doorPosition = door.transform.position;
//    }

//    void Update()
//    {
//        if (door == null || !door.IsOpen()) return;

//        // Detectar cruce real de la puerta
//        if (playerInside)
//        {
//            Vector3 dir = player.position - door.transform.position;
//            currentDot = Vector3.Dot(door.transform.forward, dir);

//            if (previousDot > 0 && currentDot < 0)
//            {
//                TriggerCross();
//            }

//            previousDot = currentDot;
//        }

//        // Esperar a que el jugador se aleje antes de cerrar
//        if (hasCrossed)
//        {
//            float distance = Vector3.Distance(player.position, doorPosition);

//            if (distance >= distanceToClose)
//            {
//                shouldClose = true;
//                hasCrossed = false;
//                timer = 0f;
//            }
//        }

//        // Ejecutar cierre con delay
//        if (shouldClose)
//        {
//            timer += Time.deltaTime;

//            if (timer >= delayBeforeClose)
//            {
//                door.CloseDoor();

//                if (noiseEmitter != null)
//                {
//                    noiseEmitter.EmitNoise(noiseEmitterAlcance);
//                }

//                shouldClose = false;
//                timer = 0f;

//                if (lockCondition != null)
//                    lockCondition.enabled = true;
//            }
//        }
//    }

//    void OnTriggerEnter(Collider other)
//    {
//        if (!other.CompareTag("Player")) return;

//        playerInside = true;

//        Vector3 dir = player.position - door.transform.position;
//        previousDot = Vector3.Dot(door.transform.forward, dir);
//    }

//    void OnTriggerExit(Collider other)
//    {
//        if (!other.CompareTag("Player")) return;

//        playerInside = false;
//    }

//    void TriggerCross()
//    {
//        if (hasCrossed) return;

//        hasCrossed = true;
//    }
//}