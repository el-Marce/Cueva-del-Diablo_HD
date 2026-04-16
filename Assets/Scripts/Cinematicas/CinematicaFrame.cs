using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "Frame", menuName = "Cinematica/Frame")]
public class CinematicaFrame : ScriptableObject
{
    [Header("Visual")]
    public Sprite ilustracion;
    public VideoClip videoAnimado; // opcional, si tiene animación AE

    [Header("Audio")]
    public AudioClip narracion;

    [Header("Subtítulo")]
    [TextArea] public string subtitulo;
    public float velocidadEscritura = 0.04f; // segundos por caracter

    //[Header("Timing")]
    //public float duracionMinima = 1f; // tiempo mínimo antes de poder saltar
}