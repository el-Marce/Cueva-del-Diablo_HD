using UnityEngine;
using Cinemachine;

public class PlayerLean : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public float leanAmount = 1f;
    public float leanSpeed = 5f;

    Cinemachine3rdPersonFollow follow;
    float targetX = 0f;

    void Start()
    {
        follow = virtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
    }

    void Update()
    {
        if (GameState.InMenu) return;

        if (Input.GetMouseButton(1))
        {
            if (Input.GetKey(KeyCode.A))
                targetX = leanAmount;
            else if (Input.GetKey(KeyCode.D))
                targetX = -leanAmount;
            else
                targetX = 0f;
        }
        else
        {
            targetX = 0f;
        }

        var offset = follow.ShoulderOffset;
        offset.x = Mathf.Lerp(offset.x, targetX, Time.deltaTime * leanSpeed);
        follow.ShoulderOffset = offset;
    }
}