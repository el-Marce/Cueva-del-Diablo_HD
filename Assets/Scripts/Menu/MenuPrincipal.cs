using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class MenuPrincipal : MonoBehaviour
{
    [Header("Paneles")]
    public GameObject panelMenu;
    public GameObject panelOpciones;
    public GameObject panelCargar;

    [Header("Animación botones")]
    public float dilateDuration = 0.6f;
    public List<TMP_Text> botonesTexto;

    [Header("Hover botones")]
    public float outlineThicknessHover = 0.25f;

    [Header("Transición Nuevo Juego")]
    public MenuButtonHover btnNuevoJuegoHover;
    public int blinkTimes = 4;
    public float blinkSpeed = 0.08f;
    public string escenaJuego = "Nivel_01";

    void Start()
    {
        VolverAlMenu();
        StartCoroutine(EntradaSequence());
    }

    IEnumerator EntradaSequence()
    {
        // Oculta botones durante el fade
        foreach (var tmp in botonesTexto)
            tmp.alpha = 0f;

        if (SceneTransition.Instance != null)
            yield return new WaitUntil(() => !SceneTransition.Instance.IsTransitioning);

        // Hace visibles y anima
        foreach (var tmp in botonesTexto)
            tmp.alpha = 1f;

        StartCoroutine(AnimarBotones());
    }

    IEnumerator AnimarBotones()
    {
        float elapsed = 0f;

        foreach (var tmp in botonesTexto)
            tmp.fontMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, -1f);

        while (elapsed < dilateDuration)
        {
            elapsed += Time.deltaTime;
            float dilate = Mathf.Lerp(-1f, 0f, Mathf.Clamp01(elapsed / dilateDuration));

            foreach (var tmp in botonesTexto)
                tmp.fontMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, dilate);

            yield return null;
        }

        foreach (var tmp in botonesTexto)
            tmp.fontMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, 0f);
    }

    public void OnNuevoJuego()
    {
        StartCoroutine(NuevoJuegoRoutine());
    }

    IEnumerator NuevoJuegoRoutine()
    {
        if (btnNuevoJuegoHover != null)
            yield return StartCoroutine(btnNuevoJuegoHover.BlinkOutline(blinkTimes, blinkSpeed));

        GameManager.Instance.NuevoJuego();
    }

    public void OnCargarPartida()
    {
        panelMenu.SetActive(false);
        panelCargar.SetActive(true);
        panelCargar.GetComponent<PanelCargar>().Refresh();
    }

    public void OnOpciones()
    {
        panelMenu.SetActive(false);
        panelOpciones.SetActive(true);
    }

    public void OnSalir() => GameManager.Instance.Salir();

    public void VolverAlMenu()
    {
        panelMenu.SetActive(true);
        panelOpciones.SetActive(false);
        panelCargar.SetActive(false);
    }
}