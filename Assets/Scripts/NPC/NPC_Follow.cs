using UnityEngine;

public class NPC_Follow : MonoBehaviour
{
    public Transform player;
    public float speed;
    public float stopDistance = 2f;

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > stopDistance)
        {
            Vector3 dir = (player.position - transform.position).normalized;

            transform.position += dir * speed * Time.deltaTime;

            // Rotar suavemente hacia el jugador
            Quaternion lookRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, 5f * Time.deltaTime);
        }
    }
}