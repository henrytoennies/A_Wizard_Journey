using Unity.VisualScripting;
using UnityEngine;

public class GrassBoomerang : Projectile
{
    [Header("Set in Inspector")]
    public float lifeTime;
    
    [Header("Set Dynamically")]
    public elemDef def;
    public Vector3[] points;
    public float birthTime;
    public Vector3 tempPos;


    // public BoundsCheck bndCheck;

    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        render = GetComponent<Renderer>();
        bndCheck = GetComponent<BoundsCheck>();
        
        birthTime = Time.time;

        points = new Vector3[4];
        points[0] = transform.position;
        points[1] = new Vector3(Random.Range(-30f,-18f),Random.Range(-20f,20f),0);
        points[2] = new Vector3(Random.Range(-30f,-18f),Random.Range(-20f,20f),0);
        points[3] = new Vector3(20,Random.Range(-10f,10f),0);
    }

    void Start()
    {
        def = Main.GetElemDef(elemType.grass);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0,0,360 * Time.deltaTime);
        
        float t = (Time.time - birthTime) / lifeTime;

        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float ttt = tt * t;
        float uuu = uu * u;

        tempPos = uuu * points[0];
        tempPos += 3 * uu * t * points[1];
        tempPos += 3 * u * tt * points[2];
        tempPos += ttt * points[3];

        transform.position = tempPos;
        
        if (!bndCheck.isOnScreen)
        {
            Destroy(gameObject);
        }
    }
}
