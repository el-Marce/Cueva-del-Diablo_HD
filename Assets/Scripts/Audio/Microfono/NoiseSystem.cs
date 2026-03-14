using UnityEngine;

public class NoiseSystem : MonoBehaviour
{
    public static NoiseSystem Instance;

    void Awake()
    {
        Instance = this;
    }

    public void MakeNoise(Vector3 position, float radius)
    {
        EnemyHearing[] enemies = FindObjectsOfType<EnemyHearing>();

        foreach (EnemyHearing enemy in enemies)
        {
            enemy.HearNoise(position);
        }
    }
}