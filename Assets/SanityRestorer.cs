using UnityEngine;
using System.Collections;
using TMPro;

public class SanityRestorer : MonoBehaviour, IInteractable
{
    [Header("Configuración")]
    public float sanityAmount = 20f;
    public float cooldownSegundos = 60f;
    public GameObject mensajeCruz;
    public TextMeshProUGUI mensaje;

    [Header("Checkpoint")]
    [Tooltip("Si true, guardar el progreso al interactuar con este objeto")]
    public bool esCheckpoint = true;

    float ultimoUso = -999f;

    public void Interact()
    {
        // --- Checkpoint ---
        if (esCheckpoint && CheckpointManager.Instance != null)
        {
            GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
            if (playerGO != null)
            {
                CheckpointManager.Instance.GuardarCheckpoint(
                    playerGO.transform.position,
                    playerGO.transform.rotation
                );
            }
        }

        // --- Mensaje ---
        if (mensajeCruz != null)
        {
            mensajeCruz.SetActive(true);
            StartCoroutine(OcultarMensaje());
        }

        // --- Cooldown ---
        if (Time.time - ultimoUso < cooldownSegundos)
        {
            float restante = cooldownSegundos - (Time.time - ultimoUso);
            Debug.Log("[SanityRestorer] Cooldown activo, faltan: " + Mathf.CeilToInt(restante) + "s");
            if (mensaje != null) mensaje.text = "En espera: " + Mathf.CeilToInt(restante) + " s";
            return;
        }
        else
        {
            if (mensaje != null) mensaje.text = "Salud mental restaurada";
        }

        // --- Restaurar cordura ---
        SanitySystem sanity = FindObjectOfType<SanitySystem>();
        if (sanity == null) return;

        sanity.RestoreSanity(sanityAmount);
        ultimoUso = Time.time;
    }

    IEnumerator OcultarMensaje()
    {
        yield return new WaitForSeconds(3f);
        if (mensajeCruz != null)
            mensajeCruz.SetActive(false);
    }
}