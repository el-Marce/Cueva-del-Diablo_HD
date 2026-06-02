using UnityEngine;
using System.Collections;

public class PlayerSpawn : MonoBehaviour
{
    void Start()
    {
        //yield return null; // espera un frame a que DontDestroyOnLoad entregue el jugador

        GameObject player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            player.transform.position = transform.position;
            player.transform.rotation = transform.rotation;

            if (cc != null) cc.enabled = true;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        Gizmos.DrawRay(transform.position, transform.forward);
    }
}