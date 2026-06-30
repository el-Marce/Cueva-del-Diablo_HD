using UnityEngine;
using UnityEngine.UI;

// Copia de PanelOpciones pensada para usarse dentro del menú de pausa,
// sin depender de MenuPrincipal. Misma estructura de Sliders/Dropdown
// comentados para cuando los actives.
public class PanelOpcionesPausa : MonoBehaviour
{
    [Header("Navegación")]
    public MenuPausa menuPausa;
    public Button btnVolver;

    //[Header("Audio")]
    //public Slider sliderMusica;
    //public Slider sliderSFX;

    //[Header("Video")]
    //public TMP_Dropdown dropdownResolucion;

    //[Header("Resoluciones")]
    //public string[] resoluciones = { "1920x1080", "1280x720", "2560x1440" };

    void Awake()
    {
        btnVolver.onClick.AddListener(menuPausa.VolverAlPausa);
    }

    void Start()
    {
        //sliderMusica.value = PlayerPrefs.GetFloat("VolMusica", 1f);
        //sliderSFX.value = PlayerPrefs.GetFloat("VolSFX", 1f);

        //sliderMusica.onValueChanged.AddListener(v => PlayerPrefs.SetFloat("VolMusica", v));
        //sliderSFX.onValueChanged.AddListener(v => PlayerPrefs.SetFloat("VolSFX", v));

        //dropdownResolucion.ClearOptions();
        //dropdownResolucion.AddOptions(new System.Collections.Generic.List<string>(resoluciones));
    }

    public void OnResolucionChanged(int index)
    {
        //string[] partes = resoluciones[index].Split('x');
        //if (partes.Length == 2 &&
        //    int.TryParse(partes[0], out int w) &&
        //    int.TryParse(partes[1], out int h))
        //{
        //    Screen.SetResolution(w, h, true);
        //    PlayerPrefs.SetInt("Resolucion", index);
        //}
    }

    public void OnVolMusicaChanged(float value) => PlayerPrefs.SetFloat("VolMusica", value);
    public void OnVolSFXChanged(float value) => PlayerPrefs.SetFloat("VolSFX", value);
}