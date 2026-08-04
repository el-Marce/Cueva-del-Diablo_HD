using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

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
            StartCoroutine(TransicionNivelDos());
    }

    IEnumerator TransicionNivelDos()
    {
        yield return new WaitForSeconds(4f); // ajusta según duración de la animación de muerte

        if (SceneTransition.Instance != null)
            SceneTransition.Instance.TransitionTo("Cinematica1al2");
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    void OnDestroy()
    {
        if (stats != null)
            stats.OnHit -= CheckDeath;
    }
}