using UnityEngine;

[System.Serializable]
public class MensajeSecuencia
{
    [TextArea] public string mensaje;
    public KeyCode[] teclas;           // vacío = usa tiempoAutoCierre
    public float tiempoAutoCierre = 0f; // 0 = usa teclas
}

[CreateAssetMenu(fileName = "TutorialStep", menuName = "Tutorial/Step")]
public class TutorialStep : ScriptableObject
{
    [Header("Contenido")]
    [TextArea] public string mensaje;
    public Sprite icono;

    [Header("Secuencia de mensajes")]
    public MensajeSecuencia[] mensajesSecuencia;  // reemplaza string[] mensajesSecuencia

    [Header("Comportamiento")]
    public bool bloqueaAvance = false;
    public bool silencioso = false;
    public string triggerDeDesbloqueo;
    public float delayInicio = 0f;

    [Header("Triggers múltiples (si se usan, ignorar triggerDeDesbloqueo)")]
    public string[] triggersRequeridos;

    [Header("Posición del panel")]
    public Vector2 posicionAnclaje = new Vector2(0.5f, 0.8f);
    public Vector2 tamañoPanel = new Vector2(600f, 300f);

    [Header("Tamaño del texto")]
    public Vector2 tamañoTexto = new Vector2(560f, 260f);

    [Header("Toast de desbloqueo")]
    [Tooltip("Dejar vacío para usar el mensaje por defecto del toast")]
    public string toastMensaje;
    public Sprite toastIcono;

    [Header("Teclas para cerrar el panel (mensaje principal)")]
    public KeyCode[] teclasConfirmacion;

    [Header("Auto-cierre del mensaje principal (0 = desactivado, usa teclas)")]
    public float tiempoAutoCierre = 0f;
}