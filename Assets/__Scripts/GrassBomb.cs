using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GrassBomb : MonoBehaviour
{
    [Header("Set Dynamically")]
    public float damage = 5;
    public Rigidbody rigid;
    public Renderer render;
    public elemType type = elemType.grass;
    public elemDef def;
    public Vector3 attackPoint;
    

    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        render = GetComponent<Renderer>();

    }

    void Start()
    {
        def = Main.GetElemDef(elemType.grass);
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x < attackPoint.x)
        {
            Explode();
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

    void Explode()
    {
        Projectile p = MakeProjectile();
        p.transform.rotation = Quaternion.Euler(0, 0, 90);
        p.rigid.linearVelocity = Vector3.left * def.velocity * 0.5f;
        p = MakeProjectile();
        p.transform.rotation = Quaternion.Euler(0, 0, 135);
        p.rigid.linearVelocity = Quaternion.Euler(0,0,45) * Vector3.left * def.velocity * 0.5f;
        p = MakeProjectile();
        p.transform.rotation = Quaternion.Euler(0, 0, 180);
        p.rigid.linearVelocity = Vector3.up * def.velocity * 0.5f;
        p = MakeProjectile();
        p.transform.rotation = Quaternion.Euler(0, 0, 225);
        p.rigid.linearVelocity = Quaternion.Euler(0,0,45) * Vector3.up * def.velocity * 0.5f;
        p = MakeProjectile();
        p.transform.rotation = Quaternion.Euler(0, 0, 270);
        p.rigid.linearVelocity = Vector3.right * def.velocity * 0.5f;
        p = MakeProjectile();
        p.transform.rotation = Quaternion.Euler(0, 0, 315);
        p.rigid.linearVelocity = Quaternion.Euler(0,0,45) * Vector3.right * def.velocity * 0.5f;
        p = MakeProjectile();
        p.transform.rotation = Quaternion.Euler(0, 0, 0);
        p.rigid.linearVelocity = Vector3.down * def.velocity * 0.5f;
        p = MakeProjectile();
        p.transform.rotation = Quaternion.Euler(0, 0, 45);
        p.rigid.linearVelocity = Quaternion.Euler(0,0,45) * Vector3.down * def.velocity * 0.5f;
        
        Destroy(gameObject);
    }
}
