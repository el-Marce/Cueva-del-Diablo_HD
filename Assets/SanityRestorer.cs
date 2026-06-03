using UnityEngine;

public class SanityRestorer : MonoBehaviour, IInteractable
{
    [Header("Configuración")]
    public float sanityAmount = 20f;
    public float cooldownSegundos = 120f;

    float ultimoUso = -999f;

    public void Interact()
    {
        if (Time.time - ultimoUso < cooldownSegundos)
        {
            float restante = cooldownSegundos - (Time.time - ultimoUso);
            Debug.Log("[SanityRestorer] Cooldown activo, faltan: " + Mathf.CeilToInt(restante) + "s");
            return;
        }

        SanitySystem sanity = FindObjectOfType<SanitySystem>();
        if (sanity == null) return;

        sanity.RestoreSanity(sanityAmount);
        ultimoUso = Time.time;
    }
}