using UnityEngine;
using System.Collections;

public class AltarRitual : MonoBehaviour, IInteractable
{
    public GameObject altarUI;
    public GameObject altarPanel;
    bool ritualCompleted = false;
    Inventory inventory;

    [Header("Estado del ritual")]
    public bool cocaEntregada = false;
    public bool alcoholEntregado = false;
    public bool sulluEntregado = false;

    [Header("Modelos de ofrenda")]
    public GameObject modeloCoca;
    public GameObject modeloAlcohol;
    public GameObject modeloSullu;

    PlayerCombat playerCombat;

    public string GetNextItemName()
    {
        if (!cocaEntregada) return "Coca";
        if (!alcoholEntregado) return "Alcohol";
        if (!sulluEntregado) return "Sullu";
        return null;
    }

    void Start()
    {
        inventory = FindObjectOfType<Inventory>();
        playerCombat = FindObjectOfType<PlayerCombat>();

        if (modeloCoca != null) modeloCoca.SetActive(false);
        if (modeloAlcohol != null) modeloAlcohol.SetActive(false);
        if (modeloSullu != null) modeloSullu.SetActive(false);
    }

    public void Interact()
    {
        if (ritualCompleted) return;
        altarPanel.SetActive(true);
        altarUI.GetComponent<AltarUI>().OpenUI();
        GameState.InMenu = true;
    }

    public bool OfferNextItem()
    {
        if (!cocaEntregada && inventory.HasItem("Coca"))
        {
            StartCoroutine(AnimarYEntregar("Coca"));
            return false;
        }
        if (!alcoholEntregado && inventory.HasItem("Alcohol"))
        {
            StartCoroutine(AnimarYEntregar("Alcohol"));
            return false;
        }
        if (!sulluEntregado && inventory.HasItem("Sullu"))
        {
            StartCoroutine(AnimarYEntregar("Sullu"));
            return true;
        }
        return false;
    }

    IEnumerator AnimarYEntregar(string item)
    {
        GameObject modelo = item switch
        {
            "Coca" => modeloCoca,
            "Alcohol" => modeloAlcohol,
            "Sullu" => modeloSullu,
            _ => null
        };

        Vector3 destino = transform.position + Vector3.up * 0.5f;
        yield return StartCoroutine(AnimarOfrenda(modelo, destino));

        // Procesar entrega después de la animación
        switch (item)
        {
            case "Coca":
                inventory.RemoveItem("Coca");
                cocaEntregada = true;
                break;
            case "Alcohol":
                inventory.RemoveItem("Alcohol");
                alcoholEntregado = true;
                break;
            case "Sullu":
                inventory.RemoveItem("Sullu");
                sulluEntregado = true;
                ritualCompleted = true;
                DisableInteraction();
                break;
        }
    }

    IEnumerator AnimarOfrenda(GameObject modelo, Vector3 posicionFinal)
    {
        if (modelo == null) yield break;

        OcultarArmaActual();

        // Posición inicial frente a la cámara, donde aparecen las armas
        Transform cam = Camera.main.transform;
        Vector3 posInicial = cam.position + cam.right * 0.3f + cam.up * -0.25f + cam.forward * 0.5f;
        Vector3 posBolsillo = posInicial + Vector3.down * 0.4f;

        modelo.SetActive(true);
        modelo.transform.position = posBolsillo;
        modelo.transform.rotation = cam.rotation;

        // Sube desde abajo como sacándolo del bolsillo
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / 0.3f;
            float curva = 1f - Mathf.Pow(1f - t, 3f);
            modelo.transform.position = Vector3.Lerp(posBolsillo, posInicial, curva);
            yield return null;
        }

        yield return new WaitForSeconds(0.3f); // pausa sosteniendo el objeto

        // Se lanza hacia el altar
        Quaternion rotInicial = cam.rotation;
        Quaternion rotFinal = Quaternion.Euler(
            cam.rotation.eulerAngles + new Vector3(180f, 0f, 0f)
        );

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / 0.5f;
            float curva = t * t;
            modelo.transform.position = Vector3.Lerp(posInicial, posicionFinal, curva);
            modelo.transform.rotation = Quaternion.Slerp(rotInicial, rotFinal, curva);
            yield return null;
        }

        modelo.SetActive(false);
        RestaurarArmaActual();
    }

    void OcultarArmaActual()
    {
        if (playerCombat == null) return;
        if (playerCombat.aguaBenditaModel != null) playerCombat.aguaBenditaModel.SetActive(false);
        if (playerCombat.stickModel != null) playerCombat.stickModel.SetActive(false);
        if (playerCombat.rockModel != null) playerCombat.rockModel.SetActive(false);
    }

    void RestaurarArmaActual()
    {
        if (playerCombat == null) return;
        if (playerCombat.currentWeapon == PlayerCombat.WeaponType.AguaBendita && playerCombat.aguaBenditaModel != null)
            playerCombat.aguaBenditaModel.SetActive(true);
        if (playerCombat.currentWeapon == PlayerCombat.WeaponType.Stick && playerCombat.stickModel != null)
            playerCombat.stickModel.SetActive(true);
        if (playerCombat.currentWeapon == PlayerCombat.WeaponType.Rock && playerCombat.rockModel != null)
            playerCombat.rockModel.SetActive(true);
    }

    void DisableInteraction()
    {
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;
    }

    public void HideUI()
    {
        altarPanel.SetActive(false);
        altarUI.GetComponent<AltarUI>().CloseUI();
    }

    public void ShowUI()
    {
        altarPanel.SetActive(true);
        altarUI.GetComponent<AltarUI>().OpenUI();
    }
}