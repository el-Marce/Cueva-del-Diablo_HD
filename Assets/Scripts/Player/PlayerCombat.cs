using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using FMODUnity;

public class PlayerCombat : MonoBehaviour
{
    [Header("Combat")]
    public float attackRange = 2f;
    public float pushForce = 6f;
    public float hitForce = 2f;
    public LayerMask enemyLayer;

    [Header("Weapon Timing")]
    public float fistWindUp = 0.1f;
    public float fistCooldown = 0.5f;

    public float stickWindUp = 0.3f;
    public float stickCooldown = 0.8f;

    public float rockWindUp = 0.5f;
    public float rockCooldown = 1.2f;

    private bool isAttacking = false;

    [Header("Sonidos")]
    public EventReference ataquePaloSound;
    public EventReference ataqueRocaSound;
    public EventReference windAguaBendita;
    public EventReference usarAguaBendita;

    [Header("Agua Bendita Rotura")]
    public ParticleSystem[] roturaParticles;

    [Header("Push")]
    public float pushCooldown = 3f;
    float pushTimer = 0f;

    [Header("Damage")]
    public float fistDamage = 5f;
    public float stickDamage = 12f;
    public float rockDamage = 20f;

    [Header("Fists Visual")]
    public GameObject fistsModel;
    public float fistPunchDistance = 0.3f;

    [Header("Agua Bendita Visual")]
    public GameObject aguaBenditaModel;
    [Header("Stick Visual")]
    public GameObject stickModel;
    [Header("Rock Visual")]
    public GameObject rockModel;

    [Header("Agua Bendita Ataque")]
    public float punchDistance = 0.3f;
    public float punchDuration = 0.1f;
    public float returnDuration = 0.2f;

    [Header("Stick Ataque")]
    public float stickPunchDuration = 0.1f;

    [Header("Rock Ataque")]
    public float rockPunchDuration = 0.18f;

    public enum WeaponType
    {
        Fists,
        Stick,
        Rock,
        AguaBendita
    }

    [Header("Weapon State")]
    public WeaponType currentWeapon = WeaponType.Fists;

    int currentDurability = 0;
    int maxDurability = 0;

    Camera cam;

    [Header("Escenas de Combate")]
    [Tooltip("Solo en estas escenas el jugador puede atacar/empujar")]
    public string[] escenasDeCombate = { "Nivel1", "Nivel2", "Nivel3" };

    private bool combatePermitido = false;

    void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        cam = Camera.main;
        combatePermitido = EsEscenaDeCombate(scene.name);
    }

    bool EsEscenaDeCombate(string nombreEscena)
    {
        foreach (string escena in escenasDeCombate)
        {
            if (nombreEscena == escena) return true;
        }
        return false;
    }

    void Start()
    {
        cam = Camera.main;
        combatePermitido = EsEscenaDeCombate(SceneManager.GetActiveScene().name);

        if (fistsModel != null) fistsModel.SetActive(currentWeapon == WeaponType.Fists);
        if (aguaBenditaModel != null) aguaBenditaModel.SetActive(currentWeapon == WeaponType.AguaBendita);
        if (stickModel != null) stickModel.SetActive(currentWeapon == WeaponType.Stick);
        if (rockModel != null) rockModel.SetActive(currentWeapon == WeaponType.Rock);
    }

    void Update()
    {
        if (GameState.InMenu) return;
        if (!combatePermitido) return;

        pushTimer -= Time.deltaTime;

        if (Input.GetMouseButtonDown(0)) TryAttack();
        if (Input.GetMouseButtonDown(1)) Push();
    }

    void TryAttack()
    {
        if (isAttacking) return;
        StartCoroutine(AttackRoutine());
    }

    bool HayEnteAlFrente() => GetEnteAlFrente() != null;

    IEnumerator AttackRoutine()
    {
        isAttacking = true;

        float windUp = GetWindUp();
        float cooldown = GetCooldown();

        if (currentWeapon == WeaponType.Stick)
            AudioManager.Instance.Play(ataquePaloSound);
        if (currentWeapon == WeaponType.Rock)
            AudioManager.Instance.Play(ataqueRocaSound);

        // Animación arranca inmediatamente sin esperar el windUp
        if (currentWeapon == WeaponType.AguaBendita)
        {
            bool conecto = HayEnteAlFrente();
            StartCoroutine(AnimarGolpeAguaBendita(conecto));
        }
        else if (currentWeapon == WeaponType.Stick || currentWeapon == WeaponType.Rock)
        {
            StartCoroutine(AnimarGolpeMelee());
        }
        else if (currentWeapon == WeaponType.Fists)
        {
            StartCoroutine(AnimarGolpeFists());
        }

        yield return new WaitForSeconds(windUp);

        PerformAttack();

        yield return new WaitForSeconds(cooldown);

        isAttacking = false;
    }

    void PerformAttack()
    {
        if (currentWeapon == WeaponType.AguaBendita)
        {
            UseAguaBendita();
            return;
        }

        float damage = GetCurrentDamage();
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, attackRange, enemyLayer))
        {
            EnemyStats enemy = hit.collider.GetComponent<EnemyStats>();
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                UseDurability();
            }

            if (rb != null)
                ApplyForce(rb, hitForce * GetForceMultiplier());
        }
    }

    void UseAguaBendita()
    {
        EntePsicologico ente = GetEnteAlFrente();
        if (ente == null) return;

        ente.Repel();
        UseDurability();
        AudioManager.Instance.Play(usarAguaBendita);
    }

    // ── Animaciones ────────────────────────────────────────────────────────────

    IEnumerator AnimarGolpeFists()
    {
        if (fistsModel == null) yield break;

        Vector3 localOriginal = fistsModel.transform.localPosition;
        Vector3 localImpacto = localOriginal + new Vector3(0f, 0f, fistPunchDistance);
        AudioManager.Instance.Play(windAguaBendita);
        // Golpe hacia adelante
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / punchDuration;
            fistsModel.transform.localPosition = Vector3.Lerp(localOriginal, localImpacto, t * t);
            yield return null;
        }

        // Retorno suave
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / fistCooldown;
            float curva = 1f - Mathf.Pow(1f - t, 3f);
            fistsModel.transform.localPosition = Vector3.Lerp(localImpacto, localOriginal, curva);
            yield return null;
        }

        fistsModel.transform.localPosition = localOriginal;
    }

    IEnumerator AnimarGolpeAguaBendita(bool golpeoEnemigo)
    {
        if (aguaBenditaModel == null) yield break;

        Vector3 localOriginal = aguaBenditaModel.transform.localPosition;
        Quaternion rotOriginal = aguaBenditaModel.transform.localRotation;
        Vector3 localArriba = localOriginal + new Vector3(0f, 0.2f, 0.1f);
        Vector3 localImpacto = localOriginal + new Vector3(0f, -0.15f, 0.35f);

        AudioManager.Instance.Play(windAguaBendita);

        // Sube
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / punchDuration;
            aguaBenditaModel.transform.localPosition = Vector3.Lerp(localOriginal, localArriba, t);
            yield return null;
        }

        // Baja golpeando
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / (punchDuration * 0.5f);
            aguaBenditaModel.transform.localPosition = Vector3.Lerp(localArriba, localImpacto, t * t);
            yield return null;
        }

        if (!golpeoEnemigo)
        {
            t = 0f;
            Vector3 posImpacto = aguaBenditaModel.transform.localPosition;
            while (t < 1f)
            {
                t += Time.deltaTime / returnDuration;
                aguaBenditaModel.transform.localPosition = Vector3.Lerp(posImpacto, localOriginal, t);
                yield return null;
            }
            aguaBenditaModel.transform.localPosition = localOriginal;
            yield break;
        }

        // --- FASE ROTURA (solo si conectó) ---
        if (roturaParticles != null && roturaParticles.Length > 0)
        {
            foreach (ParticleSystem ps in roturaParticles)
            {
                if (ps == null) continue;
                ps.transform.position = aguaBenditaModel.transform.position;
                ps.Play();
            }
        }

        float shakeDuration = 0.07f;
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / shakeDuration;
            float angulo = Mathf.Sin(t * Mathf.PI * 6f) * Mathf.Lerp(25f, 0f, t);
            aguaBenditaModel.transform.localRotation = rotOriginal * Quaternion.Euler(angulo, angulo * 0.5f, 0f);
            yield return null;
        }

        Vector3 posActual = aguaBenditaModel.transform.localPosition;
        Vector3 posTirada = localOriginal + new Vector3(0.1f, -0.5f, 0.2f);
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / 0.2f;
            aguaBenditaModel.transform.localPosition = Vector3.Lerp(posActual, posTirada, t * t);
            aguaBenditaModel.transform.localRotation = rotOriginal * Quaternion.Euler(
                Mathf.Lerp(0f, 60f, t), 0f, Mathf.Lerp(0f, -40f, t)
            );
            yield return null;
        }

        MeshRenderer mr = aguaBenditaModel.GetComponent<MeshRenderer>();
        if (mr != null) mr.enabled = false;
        aguaBenditaModel.transform.localPosition = localOriginal;
        aguaBenditaModel.transform.localRotation = rotOriginal;

        Inventory inv = FindObjectOfType<Inventory>();
        WeaponData w = inv?.weapons.Find(x => x.weaponType == WeaponType.AguaBendita);

        if (roturaParticles != null && roturaParticles.Length > 0)
            yield return new WaitForSeconds(roturaParticles[0].main.duration);

        if (w == null || w.durability <= 0) yield break;

        yield return new WaitForSeconds(0.3f);

        if (mr != null) mr.enabled = true;
        aguaBenditaModel.transform.localRotation = rotOriginal;

        Vector3 posBolsillo = localOriginal + new Vector3(0f, -0.4f, 0f);
        aguaBenditaModel.transform.localPosition = posBolsillo;

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / returnDuration;
            float curva = 1f - Mathf.Pow(1f - t, 3f);
            aguaBenditaModel.transform.localPosition = Vector3.Lerp(posBolsillo, localOriginal, curva);
            yield return null;
        }

        aguaBenditaModel.transform.localPosition = localOriginal;
        aguaBenditaModel.transform.localRotation = rotOriginal;
    }

    IEnumerator AnimarGolpeMelee()
    {
        GameObject model = currentWeapon == WeaponType.Stick ? stickModel : rockModel;
        if (model == null) yield break;

        float duracionGolpe = currentWeapon == WeaponType.Stick ? stickPunchDuration : rockPunchDuration;

        Vector3 localOriginal = model.transform.localPosition;
        Quaternion rotOriginal = model.transform.localRotation;
        Vector3 localArriba = localOriginal + new Vector3(-0.1f, 0.4f, 0.05f);
        Vector3 localImpacto = localOriginal + new Vector3(0.1f, -0.25f, 0.5f);

        // Sube con peso, punta hacia atrás
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / (duracionGolpe * 2f);
            model.transform.localPosition = Vector3.Lerp(localOriginal, localArriba, t);
            model.transform.localRotation = rotOriginal * Quaternion.Euler(
                Mathf.Lerp(0f, 30f, t), 0f, 0f
            );
            yield return null;
        }

        // Baja golpeando, punta cae violentamente hacia adelante
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / (duracionGolpe * 0.4f);
            float curva = t * t;
            model.transform.localPosition = Vector3.Lerp(localArriba, localImpacto, curva);
            model.transform.localRotation = rotOriginal * Quaternion.Euler(
                Mathf.Lerp(30f, -40f, curva), 0f, 0f
            );
            yield return null;
        }

        // Chequear rotura
        Inventory inv = FindObjectOfType<Inventory>();
        WeaponData w = inv?.weapons.Find(x => x.name == inv.equippedWeapon);
        bool seRompe = (w == null);

        if (seRompe)
        {
            float shakeDuration = 0.1f;
            t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / shakeDuration;
                float angulo = Mathf.Sin(t * Mathf.PI * 6f) * Mathf.Lerp(35f, 0f, t);
                model.transform.localRotation = rotOriginal * Quaternion.Euler(angulo, angulo * 0.5f, 0f);
                yield return null;
            }

            Vector3 posActual = model.transform.localPosition;
            Vector3 posTirada = localOriginal + new Vector3(0.2f, -0.7f, 0.3f);
            t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / 0.25f;
                model.transform.localPosition = Vector3.Lerp(posActual, posTirada, t * t);
                model.transform.localRotation = rotOriginal * Quaternion.Euler(
                    Mathf.Lerp(0f, -80f, t), 0f, Mathf.Lerp(0f, -50f, t)
                );
                yield return null;
            }

            MeshRenderer mr = model.GetComponent<MeshRenderer>();
            if (mr != null) mr.enabled = false;
            model.transform.localPosition = localOriginal;
            model.transform.localRotation = rotOriginal;
        }
        else
        {
            float duracionRetorno = GetCooldown();
            t = 0f;
            Vector3 posImpacto = model.transform.localPosition;
            while (t < 1f)
            {
                t += Time.deltaTime / duracionRetorno;
                float curva = 1f - Mathf.Pow(1f - t, 3f);
                model.transform.localPosition = Vector3.Lerp(posImpacto, localOriginal, curva);
                model.transform.localRotation = Quaternion.Slerp(
                    model.transform.localRotation, rotOriginal, curva
                );
                yield return null;
            }
            model.transform.localPosition = localOriginal;
            model.transform.localRotation = rotOriginal;
        }
    }

    IEnumerator AnimarEquipar(GameObject model)
    {
        MeshRenderer mr = model.GetComponent<MeshRenderer>();
        if (mr != null) mr.enabled = true;

        Vector3 localOriginal = model.transform.localPosition;
        Quaternion rotOriginal = model.transform.localRotation;
        Vector3 posBolsillo = localOriginal + new Vector3(0f, -0.4f, 0f);

        model.transform.localPosition = posBolsillo;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / returnDuration;
            float curva = 1f - Mathf.Pow(1f - t, 3f);
            model.transform.localPosition = Vector3.Lerp(posBolsillo, localOriginal, curva);
            yield return null;
        }

        model.transform.localPosition = localOriginal;
        model.transform.localRotation = rotOriginal;
    }

    // ── Equipar / Romper ───────────────────────────────────────────────────────

    public void EquipWeapon(WeaponType weapon, int durability)
    {
        currentWeapon = weapon;
        maxDurability = durability;
        currentDurability = durability;

        if (fistsModel != null) fistsModel.SetActive(currentWeapon == WeaponType.Fists);
        if (aguaBenditaModel != null) aguaBenditaModel.SetActive(currentWeapon == WeaponType.AguaBendita);
        if (stickModel != null) stickModel.SetActive(currentWeapon == WeaponType.Stick);
        if (rockModel != null) rockModel.SetActive(currentWeapon == WeaponType.Rock);

        GameObject modelEquipado = weapon switch
        {
            WeaponType.Fists => fistsModel,
            WeaponType.AguaBendita => aguaBenditaModel,
            WeaponType.Stick => stickModel,
            WeaponType.Rock => rockModel,
            _ => null
        };

        if (modelEquipado != null)
            StartCoroutine(AnimarEquipar(modelEquipado));

        Debug.Log("Equipado: " + weapon + " | Durabilidad: " + durability);
    }

    void UseDurability()
    {
        if (currentWeapon == WeaponType.Fists) return;

        Inventory inv = FindObjectOfType<Inventory>();
        if (inv == null || inv.equippedWeapon == null) return;

        inv.UseWeaponDurability(inv.equippedWeapon);

        WeaponData w = inv.weapons.Find(x => x.name == inv.equippedWeapon);
        if (w == null) BreakWeapon();
    }

    void BreakWeapon()
    {
        Debug.Log(currentWeapon + " se rompió");

        if (currentWeapon == WeaponType.AguaBendita && aguaBenditaModel != null)
            aguaBenditaModel.SetActive(false);
        if (currentWeapon == WeaponType.Stick && stickModel != null)
            stickModel.SetActive(false);
        if (currentWeapon == WeaponType.Rock && rockModel != null)
            rockModel.SetActive(false);

        currentWeapon = WeaponType.Fists;
        currentDurability = 0;
        maxDurability = 0;

        // Mostrar puños con animación de equipar
        if (fistsModel != null)
        {
            fistsModel.SetActive(true);
            StartCoroutine(AnimarEquipar(fistsModel));
        }

        Inventory inv = FindObjectOfType<Inventory>();
        if (inv != null) inv.equippedWeapon = null;
    }

    // ── Helpers ────────────────────────────────────────────────────────────────

    void Push()
    {
        if (pushTimer > 0f) return;

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, attackRange, enemyLayer))
        {
            Pueblerino enemy = hit.collider.GetComponent<Pueblerino>();

            if (enemy != null && enemy.isPreparingAttack && enemy.attackWindUpTimer <= enemy.attackWindUp * 1f)
            {
                Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
                if (rb != null) ApplyForce(rb, pushForce);
                enemy.OnPushed();
            }
            pushTimer = pushCooldown;
        }
    }

    void ApplyForce(Rigidbody rb, float forceAmount)
    {
        Vector3 forceDir = cam.transform.forward;
        forceDir.y = 0f;
        rb.AddForce(forceDir.normalized * forceAmount, ForceMode.Impulse);
        rb.angularVelocity = Vector3.zero;
    }

    float GetWindUp()
    {
        switch (currentWeapon)
        {
            case WeaponType.Stick: return stickWindUp;
            case WeaponType.Rock: return rockWindUp;
            default: return fistWindUp;
        }
    }

    float GetCooldown()
    {
        switch (currentWeapon)
        {
            case WeaponType.Stick: return stickCooldown;
            case WeaponType.Rock: return rockCooldown;
            default: return fistCooldown;
        }
    }

    float GetForceMultiplier()
    {
        switch (currentWeapon)
        {
            case WeaponType.Stick: return 1.2f;
            case WeaponType.Rock: return 1.5f;
            default: return 1f;
        }
    }

    float GetCurrentDamage()
    {
        switch (currentWeapon)
        {
            case WeaponType.Stick: return stickDamage;
            case WeaponType.Rock: return rockDamage;
            default: return fistDamage;
        }
    }

    EntePsicologico GetEnteAlFrente()
    {
        Vector3 origin = cam.transform.position;
        Vector3 forward = cam.transform.forward;

        Collider[] hits = Physics.OverlapSphere(origin, attackRange);
        EntePsicologico mejor = null;
        float mejorAngulo = 999f;

        foreach (Collider col in hits)
        {
            EntePsicologico ente = col.GetComponent<EntePsicologico>();
            if (ente == null) continue;

            Vector3 dir = (col.bounds.center - origin).normalized;
            float angulo = Vector3.Angle(forward, dir);

            if (angulo < 60f && angulo < mejorAngulo)
            {
                mejorAngulo = angulo;
                mejor = ente;
            }
        }

        return mejor;
    }
}