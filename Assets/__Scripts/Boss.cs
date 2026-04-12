using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{
    static public Boss S;

    [Header("Set in Inspector")]
    public float health = 1000;
    public float elemDamageMod;
    public Vector3[] safePoints;
    public Vector3[] attackPoints;
    public Image healthBar;
    public GameObject grassBomb;
    public GameObject pointer;
    
    [Header("Set Dynamically")]
    public elemType type = elemType.water;
    public elemDef def;
    public float attackID;
    public int numberOfAttacks;
    public Renderer render;
    public bool doingWaterAttack = false;
    
    private float maxHealth, height, width, growthRate;

    private int i = 0;

    void Awake()
    {
        if (S == null)
        {
            S = this;
        }
        
        render = GetComponent<Renderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Invoke("ElemSelect", 3);
        maxHealth = health;
        width = healthBar.rectTransform.rect.width;
        height = healthBar.rectTransform.rect.height;
    }

    // Update is called once per frame
    void Update()
    {
        if (doingWaterAttack)
        {
            if (growthRate < 4)
            {
                growthRate += 2f * Time.deltaTime;
                Vector3 temp = new Vector3(-growthRate,0,0);
                pointer.transform.localPosition = temp;
                temp = new Vector3(.1f,growthRate,5f);
                pointer.transform.localScale = temp;
            } else if ((transform.position.y * attackID) > -4)
            {
                Vector3 tempPos = transform.position;
                tempPos.y -= 5 * attackID * Time.deltaTime;
                transform.position = tempPos;
            } else
            {
                doingWaterAttack = false;
                Invoke("ElemSelect", 2);
            }
        }
    }


    void OnTriggerEnter(Collider other)
    {
        GameObject coll = other.gameObject;

        switch (coll.tag)
        {
            case "WizardProjectile":
                Projectile p = coll.GetComponent<Projectile>();

                switch (type)
                {
                    case elemType.fire:
                        switch (p.type)
                        {
                            case elemType.water:
                                health -= p.damage * (1f + elemDamageMod);
                                break;
                            case elemType.fire:
                                health -= p.damage;
                                break;
                            case elemType.grass:
                                health -= p.damage * (1f - elemDamageMod);
                                break;
                            default:
                                break;
                        }
                        break;

                    case elemType.water:
                        switch (p.type)
                        {
                            case elemType.water:
                                health -= p.damage;
                                break;
                            case elemType.fire:
                                health -= p.damage * (1f - elemDamageMod);
                                break;
                            case elemType.grass:
                                health -= p.damage * (1f + elemDamageMod);
                                break;
                            default:
                                break;
                        }
                        break;

                    case elemType.grass:
                        switch (p.type)
                        {
                            case elemType.water:
                                health -= p.damage * (1f - elemDamageMod);
                                break;
                            case elemType.fire:
                                health -= p.damage * (1f + elemDamageMod);
                                break;
                            case elemType.grass:
                                health -= p.damage;
                                break;
                            default:
                                break;
                        }
                        break;
                    
                    default:
                        break;
                }

                float temp = (health / maxHealth) * width;
                healthBar.rectTransform.sizeDelta = new Vector2(temp, height);
                Destroy(coll);
                if (health <= 0)
                {
                    Destroy(gameObject);
                    Time.timeScale = 0;
                    Main.S.restartButton.SetActive(true);
                }
                break;

            default:
                break;
        }       
    }

    void ElemSelect()
    {
        pointer.transform.localScale = new Vector3(0.1f,0.25f,5);
        pointer.transform.localPosition = new Vector3(-0.25f,0,0);
        transform.rotation = Quaternion.Euler(0,0,0);
        switch (Random.Range(1,4))
        {
            case 1:
                type = elemType.water;
                def = Main.GetElemDef(elemType.water);
                attackID = Random.Range(0,2);
                if (attackID == 0)
                    attackID = -1;
                transform.position = new Vector3(14,attackID * 6,0);
                Invoke("WaterAttack", .5f);
                break;
            case 2:
                type = elemType.fire;
                def = Main.GetElemDef(elemType.fire);
                transform.position = safePoints[Random.Range(0,safePoints.Length)];
                // change for more attacks
                attackID = Random.Range(1,1);
                numberOfAttacks = Random.Range(5,10);
                Invoke("FireAttack", .5f);
                break;
            case 3:
                type = elemType.grass;
                def = Main.GetElemDef(elemType.grass);
                transform.position = new Vector3(14,0,0);
                // change for more attacks
                attackID = Random.Range(1,1);
                numberOfAttacks = Random.Range(3,5);
                Invoke("GrassAttack", 0.5f);
                break;
            default:
                break;
        }

        render.material.color = def.color;
    }

    void FireAttack()
    {
        switch (attackID)
        {
            case 1:
                float deltaX = transform.position.x - Wizard.S.transform.position.x;
                float deltaY = transform.position.y - Wizard.S.transform.position.y;
                float rotY = 180f / 3.14159f * Mathf.Atan2(deltaY, deltaX);

                transform.rotation = Quaternion.Euler(0,0,rotY);
                
                Projectile p = MakeProjectile();
                p.transform.rotation = Quaternion.Euler(-rotY, 45, 45 + rotY);
                p.rigid.linearVelocity =  Quaternion.Euler(0,0,rotY) * Vector3.left * def.velocity * 0.5f;
                p = MakeProjectile();
                p.transform.rotation = Quaternion.Euler(-rotY + 15, 45, 45 + rotY - 15);
                p.rigid.linearVelocity =  Quaternion.Euler(0,0,rotY - 15) * Vector3.left * def.velocity * 0.5f;
                p = MakeProjectile();
                p.transform.rotation = Quaternion.Euler(-rotY - 15, 45, 45 + rotY + 15);
                p.rigid.linearVelocity =  Quaternion.Euler(0,0,rotY + 15) * Vector3.left * def.velocity * 0.5f;
                p = MakeProjectile();
                p.transform.rotation = Quaternion.Euler(-rotY + 30, 45, 45 + rotY - 30);
                p.rigid.linearVelocity =  Quaternion.Euler(0,0,rotY - 30) * Vector3.left * def.velocity * 0.5f;
                p = MakeProjectile();
                p.transform.rotation = Quaternion.Euler(-rotY - 30, 45, 45 + rotY + 30);
                p.rigid.linearVelocity =  Quaternion.Euler(0,0,rotY + 30) * Vector3.left * def.velocity * 0.5f;
                

                break;
            default:
                break;
        }

        if (i >= numberOfAttacks)
        {
            i = 0;
            Invoke("ElemSelect", 2);
        } else
        {
            i++;
            Invoke("FireAttack", 0.5f);
        }
    }

    void GrassAttack()
    {
        switch (attackID)
        {
            case 1:
                Vector3 attackPos = attackPoints[Random.Range(0,attackPoints.Length)];
                
                float deltaX = transform.position.x - attackPos.x;
                float deltaY = transform.position.y - attackPos.y;
                float rotY = 180f / 3.14159f * Mathf.Atan2(deltaY, deltaX);

                transform.rotation = Quaternion.Euler(0,0,rotY);

                GameObject go = Instantiate<GameObject>(grassBomb);
                
                go.transform.position = transform.position;
                
                GrassBomb bomb = go.GetComponent<GrassBomb>();

                bomb.attackPoint = attackPos;
                
                bomb.rigid.linearVelocity = Quaternion.Euler(0,0,rotY) * Vector3.left * def.velocity * 0.5f;

                break;

            default:
                break;
        }

        if (i >= numberOfAttacks)
        {
            i = 0;
            Invoke("ElemSelect", 2);
        } else
        {
            i++;
            Invoke("GrassAttack", 1f);
        }
    }
    
    void WaterAttack()
    {
        switch (attackID)
        {
            case -1:
            case 1:
                growthRate = 0.25f;
                doingWaterAttack = true;
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

        go.transform.position = transform.position;
        go.transform.localScale = new Vector3(.4f,.4f,.8f);
        Projectile p = go.GetComponent<Projectile>();
        p.damage = def.damage;
        p.render.material.color = def.projectileColor;
        p.type = type;

        return p;
    }
}
