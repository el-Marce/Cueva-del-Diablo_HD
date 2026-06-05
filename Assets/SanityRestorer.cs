using UnityEngine;
using System.Collections;
using System;
using TMPro;
public class SanityRestorer : MonoBehaviour, IInteractable
{
    [Header("Configuración")]
    public float sanityAmount = 20f;
    public float cooldownSegundos = 60f;
    public GameObject mensajeCruz;
    public TextMeshProUGUI mensaje;
    float ultimoUso = -999f;

    public void Interact()
    {
        if (mensajeCruz != null) {mensajeCruz.SetActive(true); StartCoroutine(OcultarMensaje()); }
        if (Time.time - ultimoUso < cooldownSegundos)
        {
            float restante = cooldownSegundos - (Time.time - ultimoUso);
            Debug.Log("[SanityRestorer] Cooldown activo, faltan: " + Mathf.CeilToInt(restante) + "s");
            mensaje.text = "En espera: " + Mathf.CeilToInt(restante) + " s";
            return;
        }
        else { mensaje.text = "Salud mental restaurada"; }

            SanitySystem sanity = FindObjectOfType<SanitySystem>();
        if (sanity == null) return;

        sanity.RestoreSanity(sanityAmount);
        ultimoUso = Time.time;
    }
    IEnumerator OcultarMensaje()
    {
        yield return new WaitForSeconds(3f);
        mensajeCruz.SetActive (false);
    }

}