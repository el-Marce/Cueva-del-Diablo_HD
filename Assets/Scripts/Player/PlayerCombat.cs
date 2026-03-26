using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Combat")]
    public float attackRange = 2f;
    public float pushForce = 6f;
    public float hitForce = 2f;
    public LayerMask enemyLayer;

    [Header("Damage")]
    public float fistDamage = 5f;
    public float stickDamage = 15f;
    public float rockDamage = 10f;

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

        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }

        if (Input.GetMouseButtonDown(1))
        {
            Push();
        }
    }

    void Attack()
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
                ApplyForce(rb, hitForce);
            }
        }
    }

    void Push()
    {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, attackRange, enemyLayer))
        {
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();

            if (rb != null)
            {
                ApplyForce(rb, pushForce);
                Debug.Log("Empujaste al enemigo");
            }

            Pueblerino enemy = hit.collider.GetComponent<Pueblerino>();

            if (enemy != null)
            {
                enemy.OnPushed();
            }
        }
    }

    void ApplyForce(Rigidbody rb, float forceAmount)
    {
        Vector3 forceDir = cam.transform.forward;
        forceDir.y = 0f;

        rb.AddForce(forceDir.normalized * forceAmount, ForceMode.Impulse);
        rb.angularVelocity = Vector3.zero;
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