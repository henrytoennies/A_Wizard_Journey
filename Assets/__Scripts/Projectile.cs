using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Set Dynamically")]
    public float damage = 1;
    public Rigidbody rigid;
    public Renderer render;
    public elemType type;

    private BoundsCheck bndCheck;
    


    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        bndCheck = GetComponent<BoundsCheck>();
        render = GetComponent<Renderer>();

    }

    // Update is called once per frame
    void Update()
    {
        if (!bndCheck.isOnScreen)
        {
            Destroy(gameObject);
        }
    }
}
