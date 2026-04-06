using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed;
    public float sprintMultiplier = 2f;
    public float gravity = -9.81f;

    private CharacterController controller;
    NoiseEmitter noiseEmitter;
    private Vector3 velocity;

    public float speed => moveSpeed * (Input.GetKey(KeyCode.LeftShift) ? sprintMultiplier : 1f);

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        velocity = Vector3.zero; // <- resetea la gravedad acumulada
    }
    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        noiseEmitter = GetComponent<NoiseEmitter>();
    }

    void Update()
    {
        if (GameState.InMenu) return;

        Move();
        ApplyGravity();
    }

    void Move()
    {
        if (Input.GetMouseButton(1)) return;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        bool isMoving = move.magnitude > 0.1f;
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        if (isMoving && isSprinting)
            noiseEmitter.EmitNoise(1f);
    }

    void ApplyGravity()
    {
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}