using UnityEngine;
using Cinemachine;
using System.Collections;

public class PlayerDeathCamera : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public float fallDuration = 1f;
    public float groundY = 0f;

    HealthSystem healthSystem;
    float originalY;

    void Start()
    {
        GameState.InMenu = false;
        healthSystem = GetComponentInChildren<HealthSystem>();
        healthSystem.OnPlayerDeath += OnDeath;

        var composer = virtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        originalY = composer.ShoulderOffset.y;
    }

    void OnDeath()
    {
        GameState.InMenu = true;
        StartCoroutine(FallRoutine());
    }

    IEnumerator FallRoutine()
    {
        var follow = virtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        float elapsed = 0f;
        float startY = follow.ShoulderOffset.y;

        while (elapsed < fallDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fallDuration;
            var offset = follow.ShoulderOffset;
            offset.y = Mathf.Lerp(startY, groundY, t);
            follow.ShoulderOffset = offset;
            yield return null;
        }
    }

    void OnDestroy()
    {
        if (healthSystem != null)
            healthSystem.OnPlayerDeath -= OnDeath;
    }
}