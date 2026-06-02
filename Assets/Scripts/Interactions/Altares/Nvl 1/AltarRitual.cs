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

    [System.Serializable]
    public class OfrendaConfig
    {
        public Vector3 offsetPosicion = new Vector3(0.3f, -0.25f, 0.5f);
        public float offsetBolsillo = 0.4f;
        public float offsetCaida = 0.6f;
        public float offsetCaidaZ = 0.05f;
        public float anguloVertido = 80f;   // solo usado por Alcohol
    }

    [Header("Configuración Coca")]
    public OfrendaConfig configCoca;

    [Header("Configuración Alcohol")]
    public OfrendaConfig configAlcohol;

    [Header("Configuración Sullu")]
    public OfrendaConfig configSullu;

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

        OfrendaConfig config = item switch
        {
            "Coca" => configCoca,
            "Alcohol" => configAlcohol,
            "Sullu" => configSullu,
            _ => new OfrendaConfig()
        };

        yield return StartCoroutine(item == "Alcohol"
            ? AnimarVertido(modelo, config)
            : AnimarCaida(modelo, config));

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

    IEnumerator AnimarCaida(GameObject modelo, OfrendaConfig cfg)
    {
        if (modelo == null) yield break;

        OcultarArmaActual();

        Transform cam = Camera.main.transform;
        Vector3 posInicial = cam.position
            + cam.right * cfg.offsetPosicion.x
            + cam.up * cfg.offsetPosicion.y
            + cam.forward * cfg.offsetPosicion.z;
        Vector3 posBolsillo = posInicial + Vector3.down * cfg.offsetBolsillo;

        modelo.SetActive(true);
        modelo.transform.position = posBolsillo;
        modelo.transform.rotation = cam.rotation;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / 0.3f;
            float curva = 1f - Mathf.Pow(1f - t, 3f);
            modelo.transform.position = Vector3.Lerp(posBolsillo, posInicial, curva);
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);

        Vector3 posDestino = posInicial + Vector3.down * cfg.offsetCaida + cam.forward * cfg.offsetCaidaZ;
        Quaternion rotInicial = cam.rotation;
        Quaternion rotFinal = Quaternion.Euler(
            cam.rotation.eulerAngles + new Vector3(160f, 20f, 30f)
        );

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / 0.4f;
            float curva = t * t;
            modelo.transform.position = Vector3.Lerp(posInicial, posDestino, curva);
            modelo.transform.rotation = Quaternion.Slerp(rotInicial, rotFinal, curva);
            yield return null;
        }

        modelo.SetActive(false);
        RestaurarArmaActual();
    }

    IEnumerator AnimarVertido(GameObject modelo, OfrendaConfig cfg)
    {
        if (modelo == null) yield break;

        OcultarArmaActual();

        Transform cam = Camera.main.transform;
        Vector3 posInicial = cam.position
            + cam.right * cfg.offsetPosicion.x
            + cam.up * cfg.offsetPosicion.y
            + cam.forward * cfg.offsetPosicion.z;
        Vector3 posBolsillo = posInicial + Vector3.down * cfg.offsetBolsillo;

        modelo.SetActive(true);
        modelo.transform.position = posBolsillo;
        modelo.transform.rotation = cam.rotation;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / 0.3f;
            float curva = 1f - Mathf.Pow(1f - t, 3f);
            modelo.transform.position = Vector3.Lerp(posBolsillo, posInicial, curva);
            yield return null;
        }

        yield return new WaitForSeconds(0.05f);

        Quaternion rotInicial = modelo.transform.rotation;
        Quaternion rotVertido = rotInicial * Quaternion.Euler(cfg.anguloVertido, 0f, 0f);

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / 0.4f;
            float curva = 1f - Mathf.Pow(1f - t, 3f);
            modelo.transform.rotation = Quaternion.Slerp(rotInicial, rotVertido, curva);
            yield return null;
        }

        yield return new WaitForSeconds(0.4f);

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / 0.3f;
            float curva = 1f - Mathf.Pow(1f - t, 3f);
            modelo.transform.rotation = Quaternion.Slerp(rotVertido, rotInicial, curva);
            yield return null;
        }

        Vector3 posActual = modelo.transform.position;
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / 0.3f;
            modelo.transform.position = Vector3.Lerp(posActual, posBolsillo, t * t);
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