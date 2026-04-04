using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerNivelDos : MonoBehaviour
{
    EnemyStats stats;

    void Start()
    {
        stats = GetComponent<EnemyStats>();
        stats.OnHit += CheckDeath;
    }

    void CheckDeath()
    {
        if (stats.health <= 0)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    void OnDestroy()
    {
        if (stats != null)
            stats.OnHit -= CheckDeath;
    }
}