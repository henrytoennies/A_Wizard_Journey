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
    public GameObject boomerang;
    public GameObject pointer;
    
    [Header("Set Dynamically")]
    public elemType type = elemType.water;
    public elemDef def;
    public float attackID;
    public int numberOfAttacks;
    public Renderer render;
    public Rigidbody rigid;
    public bool doingWaterAttack = false;
    public bool movingToCenter = false;
    public Vector3 prevPos;
    public float attackAngle;
    public float attackDelay;
    public Vector3 velocity;
    
    private float maxHealth, height, width, growthRate, timeStart;
    private bool attackUp;

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
        rigid = GetComponent<Rigidbody>();
        velocity = rigid.linearVelocity;
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

        if (movingToCenter)
        {
            float t = (Time.time - timeStart) / 1f;

            if (t >= 1)
            {
                t = 1;
            }

            Vector3 tempPos;

            tempPos = (1 - t) * prevPos;

            transform.position = tempPos;
        }
        
        // if (doingGrassAttack)
        // {
        //     if (rigid.linearVelocity == velocity)
        //     {
        //         if (Random.Range(1,3) == 1)
        //         {
        //             rigid.linearVelocity = 5 * Vector3.up;
        //         } else
        //         {
        //             rigid.linearVelocity = 5 * Vector3.down;
        //         }
        //     }
            
        //     if (transform.position.y >= 6)
        //     {
        //         rigid.linearVelocity = 5 * Vector3.down;
        //     } else if (transform.position.y <= -6)
        //     {
        //         rigid.linearVelocity = 5 * Vector3.up;
        //     }
        // }
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
                attackID = Random.Range(0,4);
                numberOfAttacks = 50;
                if (attackID == 0)
                    attackID = -1;
                if (attackID == 2)
                {
                    attackAngle = 50;
                    attackUp = true;
                } else if (attackID == 3)
                {
                    attackAngle = -50;
                    attackUp = false;
                }
                Invoke("WaterAttack", .5f);
                break;
            case 2:
                type = elemType.fire;
                def = Main.GetElemDef(elemType.fire);
                // change for more attacks
                attackID = Random.Range(1,3);
                if (attackID == 1)
                {
                    
                    numberOfAttacks = Random.Range(5,11);
                } else
                {
                    prevPos = transform.position;
                    movingToCenter = true;
                    timeStart = Time.time;
                    numberOfAttacks = Random.Range(1,4);
                    attackAngle = 0;
                }
                Invoke("FireAttack", 1f);
                break;
            case 3:
                type = elemType.grass;
                def = Main.GetElemDef(elemType.grass);
                transform.position = new Vector3(14,0,0);
                // change for more attacks
                attackID = Random.Range(1,3);
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
        Projectile p;
        switch (attackID)
        {
            case 1:
                transform.position = safePoints[Random.Range(0,safePoints.Length)];

                float deltaX = transform.position.x - Wizard.S.transform.position.x;
                float deltaY = transform.position.y - Wizard.S.transform.position.y;
                float rotY = 180f / 3.14159f * Mathf.Atan2(deltaY, deltaX);

                transform.rotation = Quaternion.Euler(0,0,rotY);
                
                p = MakeProjectile();
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
                
                if (i >= numberOfAttacks)
                {
                    i = 0;
                    Invoke("ElemSelect", 2);
                } else
                {
                    i++;
                    Invoke("FireAttack", 0.5f);
                }
                break;

            case 2:
                
                transform.rotation = Quaternion.Euler(0,0,attackAngle);
                
                p = MakeProjectile();
                p.rigid.linearVelocity = Quaternion.Euler(0,0,attackAngle) * Vector3.left * def.velocity * 0.5f;
                p = MakeProjectile();
                p.rigid.linearVelocity = Quaternion.Euler(0,0,attackAngle) * Vector3.right * def.velocity * 0.5f;

                attackAngle += 6;

                if (i >= (numberOfAttacks * 60))
                {
                    i = 0;
                    Invoke("ElemSelect", 2);
                    movingToCenter = false;
                } else
                {
                    i++;
                    Invoke("FireAttack", 0.1f);
                }
                break;
            default:
                break;
        }
    }

    void GrassAttack()
    {
        GameObject go;
        float deltaX;
        float deltaY;
        float rot;
        switch (attackID)
        {
            case 1:
                Vector3 attackPos = attackPoints[Random.Range(0,attackPoints.Length)];
                
                deltaX = transform.position.x - attackPos.x;
                deltaY = transform.position.y - attackPos.y;
                rot = 180f / 3.14159f * Mathf.Atan2(deltaY, deltaX);

                transform.rotation = Quaternion.Euler(0,0,rot);

                go = Instantiate<GameObject>(grassBomb);
                
                go.transform.position = transform.position;
                
                GrassBomb bomb = go.GetComponent<GrassBomb>();

                bomb.attackPoint = attackPos;
                
                bomb.rigid.linearVelocity = Quaternion.Euler(0,0,rot) * Vector3.left * def.velocity * 0.5f;
                
                attackDelay = 1f;
                break;
            case 2:
                go = Instantiate(boomerang);

                go.transform.position = transform.position;

                GrassBoomerang rang = go.GetComponent<GrassBoomerang>();

                rang.points[0] = transform.position;

                rang.type = elemType.grass;

                deltaX = transform.position.x - rang.points[1].x;
                deltaY = transform.position.y - rang.points[1].y;
                rot = 180f / 3.14159f * Mathf.Atan2(deltaY, deltaX);

                transform.rotation = Quaternion.Euler(0,0,rot);

                attackDelay = 2f;

                break;

            default:
                break;
        }

        if (i >= numberOfAttacks)
        {
            rigid.linearVelocity = Vector3.zero;
            i = 0;
            Invoke("ElemSelect", 2);
        } else
        {
            i++;
            Invoke("GrassAttack", attackDelay);
        }
    }
    
    void WaterAttack()
    {
        switch (attackID)
        {
            case -1:
            case 1:
                transform.position = new Vector3(14,attackID * 6,0);

                growthRate = 0.25f;
                doingWaterAttack = true;
                break;
            case 2:
            case 3:
                transform.position = new Vector3(14,0,0);
                
                transform.rotation = Quaternion.Euler(0,0,attackAngle);
                
                Projectile p = MakeProjectile();
                p.rigid.linearVelocity =  Quaternion.Euler(0,0,attackAngle) * Vector3.left * def.velocity * 0.5f;

                if (attackUp)
                {
                    attackAngle -= 10;
                } else
                {
                    attackAngle += 10;
                }

                if (attackAngle >= 50)
                {
                    attackUp = true;
                    attackAngle -= 5;
                }
                else if (attackAngle <= -50)
                {
                    attackUp = false;
                    attackAngle += 5;
                }

                if (i >= numberOfAttacks)
                {
                    i = 0;
                    Invoke("ElemSelect", 2);
                } else
                {
                    i++;
                    Invoke("WaterAttack", .1f);
                }
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

        Projectile p = go.GetComponent<Projectile>();
        p.damage = 2;
        p.render.material.color = def.projectileColor;
        p.type = type;

        switch (p.type)
        {
            case elemType.fire:
                p.transform.localScale = new Vector3(.4f,.4f,.8f);
                break;
            case elemType.water:
                p.transform.localScale = new Vector3(1,1,1);
                break;
            default:
                break;
        }

        return p;
    }
}
