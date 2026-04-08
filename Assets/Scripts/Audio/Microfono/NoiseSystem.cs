using UnityEngine;

public class NoiseSystem : MonoBehaviour
{
    public static NoiseSystem Instance;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    //public void MakeNoise(Vector3 position, float radius, Vector3 playerPosition)
    public void MakeNoise(Vector3 position, float radius, Vector3 playerPosition, GameObject source = null)
    {
        EnemyHearing[] enemies = FindObjectsOfType<EnemyHearing>();

        foreach (EnemyHearing enemy in enemies)
        {
            if (source != null && enemy.gameObject == source) continue;
            enemy.HearNoise(position, radius, playerPosition);
            //Debug.Log("Posicion recibida por el noise System: " + playerPosition);
        }
    }
}