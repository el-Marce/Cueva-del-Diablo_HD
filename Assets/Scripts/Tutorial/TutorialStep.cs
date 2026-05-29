using UnityEngine;

[CreateAssetMenu(fileName = "TutorialStep", menuName = "Tutorial/Step")]
public class TutorialStep : ScriptableObject
{
    [Header("Contenido")]
    [TextArea] public string mensaje;
    public Sprite icono; // opcional

    [Header("Comportamiento")]
    public bool bloqueaAvance = false;
    public string triggerDeDesbloqueo; // nombre del evento que desbloquea este paso

    [Header("Posición del panel")]
    public Vector2 posicionAnclaje = new Vector2(0.5f, 0.8f); // posición en pantalla

    [Header("Toast de desbloqueo")]
    [Tooltip("Dejar vacío para usar el mensaje por defecto del toast")]
    public string toastMensaje;
    public Sprite toastIcono;
}