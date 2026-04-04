using UnityEngine;
using System.Collections;

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

    [Header("Push")]
    public float pushCooldown = 3f;
    float pushTimer = 0f;

    [Header("Damage")]
    public float fistDamage = 5f;
    public float stickDamage = 12f;
    public float rockDamage = 20f;

    public enum WeaponType
    {
        Fists,
        Stick,
        Rock
    }

    [Header("Weapon State")]
    public WeaponType currentWeapon = WeaponType.Fists;

    int currentDurability = 0;
    int maxDurability = 0;

    Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (GameState.InMenu) return;

        pushTimer -= Time.deltaTime;

        if (Input.GetMouseButtonDown(0))
        {
            TryAttack();
        }

        if (Input.GetMouseButtonDown(1))
        {
            Push();
        }
    }

    void TryAttack()
    {
        if (isAttacking) return;

        StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;

        float windUp = GetWindUp();
        float cooldown = GetCooldown();

        yield return new WaitForSeconds(windUp);

        PerformAttack();

        yield return new WaitForSeconds(cooldown);

        isAttacking = false;
    }

    void PerformAttack()
    {
        float damage = GetCurrentDamage();

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, attackRange, enemyLayer))
        {
            EnemyStats enemy = hit.collider.GetComponent<EnemyStats>();
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Debug.Log("Golpeaste con " + currentWeapon + " e hiciste " + damage);

                UseDurability();
            }

            if (rb != null)
            {
                ApplyForce(rb, hitForce * GetForceMultiplier());
            }
        }
    }

    void Push()
    {
        if (pushTimer > 0f) return;

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, attackRange, enemyLayer))
        {
            Pueblerino enemy = hit.collider.GetComponent<Pueblerino>();

            if (enemy != null && enemy.isPreparingAttack && enemy.attackWindUpTimer <= enemy.attackWindUp * 1f)
            {
                Rigidbody rb = hit.collider.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    ApplyForce(rb, pushForce);
                    Debug.Log("Empujaste | isPreparingAttack: " + enemy.isPreparingAttack);
                }

                if (enemy != null)
                {
                    enemy.OnPushed();
                }
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

    public void EquipWeapon(WeaponType weapon, int durability)
    {
        currentWeapon = weapon;
        maxDurability = durability;
        currentDurability = durability;

        Debug.Log("Equipado: " + weapon + " | Durabilidad: " + durability);
    }

    void UseDurability()
    {
        if (currentWeapon == WeaponType.Fists) return;

        currentDurability--;

        Debug.Log("Durabilidad restante: " + currentDurability);

        if (currentDurability <= 0)
        {
            BreakWeapon();
        }
    }

    void BreakWeapon()
    {
        Debug.Log(currentWeapon + " se rompió");

        currentWeapon = WeaponType.Fists;
        currentDurability = 0;
        maxDurability = 0;
    }

    float GetCurrentDamage()
    {
        switch (currentWeapon)
        {
            case WeaponType.Stick:
                return stickDamage;
            case WeaponType.Rock:
                return rockDamage;
            default:
                return fistDamage;
        }
    }
}