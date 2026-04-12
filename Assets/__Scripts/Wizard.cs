using UnityEngine;

public class Wizard : MonoBehaviour
{
    static public Wizard S;

    [Header("Set In Inspector")]
    public float speed = 10f;
    public float speedFocus = 5f;
    public float health = 10f;
    public Color color;
    public float invincibilityTime;

    [Header("Set Dynamically")] [SerializeField]
    public elemType type = elemType.water;
    public elemDef def;
    public float lastShotTime;
    public Renderer render;
    public bool showingDamage;
    public float lastTakenDamage;


    void Awake()
    {
        if (S == null)
        {
            S = this;
        } else
        {
            Debug.LogError("Wizard.Awake Attempted to assign second Wizard.S");
        }

        render = GetComponent<Renderer>();
    }

    void Start()
    {
        def = Main.GetElemDef(type);
        Invoke("Shoot", def.fireRate);
    }

    void Update()
    {
        float xAxis = Input.GetAxis("Horizontal");
        float yAxis = Input.GetAxis("Vertical");
        float slow = Input.GetAxis("Jump") + 1;

        Vector3 pos = transform.position;
        pos.x += speed * xAxis * Time.deltaTime / slow;
        pos.y += speed * yAxis * Time.deltaTime / slow;
        transform.position = pos;

        if (showingDamage && (Time.time - lastTakenDamage) > invincibilityTime)
        {
            DoneDamageTaken();
        }

        if (Input.GetAxis("Fire1") == 1)
        {
            def = Main.GetElemDef(elemType.water);
            type = elemType.water;
        } else if (Input.GetAxis("Fire2") == 1)
        {
            def = Main.GetElemDef(elemType.fire);
            type = elemType.fire;
        } else if (Input.GetAxis("Fire3") == 1)
        {
            def = Main.GetElemDef(elemType.grass);
            type = elemType.grass;
        }

        if (showingDamage)
        {
            def.color.a = 0.5f;
        }
        render.material.color = def.color;
    }

    void Shoot()
    {
        Projectile p;

        switch (type)
        {
            case elemType.water:
                p = MakeProjectile();
                Invoke("Shoot", def.fireRate);
                break;
            case elemType.fire:
                p = MakeProjectile();
                Invoke("Shoot", def.fireRate);
                break;
            case elemType.grass:
                p = MakeProjectile();
                Invoke("Shoot", def.fireRate);
                break;
            default:

                break;
        }
    }

    void OnTriggerEnter(Collider collision)
    {
        GameObject other = collision.gameObject;

        switch (other.tag)
        {
            case "EnemyProjectile":
                Projectile p = other.GetComponent<Projectile>();

                DamageTaken(p.damage);
                Destroy(other); 
                break;

            case "GrassBomb":
                GrassBomb bomb = other.GetComponent<GrassBomb>();

                DamageTaken(bomb.damage);
                Destroy(other); 
                break;

            case "Enemy":
                DamageTaken(2);
                break;

            default:
                break;
        }
    }

    public Projectile MakeProjectile()
    {
        GameObject go = Instantiate<GameObject>(def.projectilePrefab);
        if (transform.gameObject.tag == "Wizard")
        {
            go.tag = "WizardProjectile";
            go.layer = LayerMask.NameToLayer("WizardProjectile");
        } else
        {
            go.tag = "EnemyProjectile";
            go.layer = LayerMask.NameToLayer("EnemyProjectile");
        }

        go.transform.position = transform.position + new Vector3(0.6f,0,0);
        Projectile p = go.GetComponent<Projectile>();
        p.damage = def.damage;
        p.render.material.color = def.projectileColor;
        p.type = type;

        Vector3 vel = Vector3.right * def.velocity;
        p.rigid.linearVelocity = vel;

        return p;
    }

    public void DamageTaken(float dmg)
    {
        if (showingDamage)
        {
            return;
        }
        
        health -= dmg;

        HealthBar.TakeDamage(dmg);

        if (health <= 0)
        {
            Destroy(gameObject);
            Time.timeScale = 0;
            Main.S.restartButton.SetActive(true);
        }

        showingDamage = true;
        lastTakenDamage = Time.time;

        Color tempColor = def.color;
        tempColor.a = 0.5f;
        def.color = tempColor;
    }

    public void DoneDamageTaken()
    {
        showingDamage = false;
        Color tempColor = def.color;
        tempColor.a = 1f;
        def.color= tempColor;
    }
}
