using FMODUnity;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed;
    public float sprintMultiplier = 2f;
    public float gravity = -9.81f;

    [Header("Sonidos")]
    public EventReference pasosExt;

    public float speed => moveSpeed * (Input.GetKey(KeyCode.LeftShift) ? sprintMultiplier : 1f);

    CharacterController controller;
    NoiseEmitter noiseEmitter;
    Vector3 velocity;

    EventInstance pasosInstance;
    bool isPlayingPasos = false;

    TutorialBarrier barreraActual = null;
    float tiempoUltimoContacto = -999f;
    const float separacionTimeout = 0.15f;

    void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (isPlayingPasos)
        {
            AudioManager.Instance.StopLoop(pasosInstance, true);
            isPlayingPasos = false;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) => velocity = Vector3.zero;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        noiseEmitter = GetComponent<NoiseEmitter>();
    }

    bool wasInMenu = false;

    void Update()
    {
        // Detectar entrada al menú
        if (GameState.InMenu && !wasInMenu)
        {
            if (isPlayingPasos)
            {
                AudioManager.Instance.StopLoop(pasosInstance, true); // true = fadeout
                isPlayingPasos = false;
            }
        }
        wasInMenu = GameState.InMenu;

        if (GameState.InMenu) return;

        Move();
        ApplyGravity();
        //CheckSeparacionBarrera();
    }

    bool isPivoting = false;
    void Move()
    {
        if (Input.GetMouseButton(1)) return;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        float mouseX = Input.GetAxis("Mouse X");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        bool isWalking = move.magnitude > 0.1f;
        bool isPivotingNow = Mathf.Abs(mouseX) > 2f && !isWalking;
        bool isMoving = isWalking || isPivotingNow;
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);

        // Iniciar o detener loop
        if (isMoving && !isPlayingPasos)
        {
            pasosInstance = AudioManager.Instance.CreateLoop(pasosExt);
            isPlayingPasos = true;
        }
        else if (!isMoving && isPlayingPasos)
        {
            AudioManager.Instance.StopLoop(pasosInstance);
            isPlayingPasos = false;
        }

        // Actualizar parametro siempre que el loop este activo
        if (isPlayingPasos)
        {
            float valorVelocidad;

            if (isPivotingNow)
                valorVelocidad = 2f;
            else if (isSprinting)
                valorVelocidad = 1f;
            else
                valorVelocidad = 0f;

            pasosInstance.setParameterByName("Velocidad", valorVelocidad);
            //Debug.Log($"Velocidad enviada: {valorVelocidad} | isWalking: {isWalking} | isPivoting: {isPivotingNow}");
        }

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
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        TutorialBarrier barrera = hit.gameObject.GetComponent<TutorialBarrier>();
        if (barrera == null) return;

        tiempoUltimoContacto = Time.time;

        if (barreraActual != barrera)
            barreraActual = barrera;

        barrera.NotificarContacto(hit.point);
    }
}