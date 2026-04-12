using Unity.VisualScripting;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [Header("Set in Inspector")]
    static public float health, maxHealth, width, height;

    static public RectTransform rectTransform;

    // Start is called before the first frame update
    void Start()
    {
        if (gameObject.tag == "Wizard")
        {
            maxHealth = health = Wizard.S.health;
        } else if (gameObject.tag == "Enemy")
        {
            maxHealth = health = Boss.S.health;
        }
        
        rectTransform = GetComponent<RectTransform>();
        width = rectTransform.rect.width;
        height = rectTransform.rect.height;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    static public void TakeDamage(float dmg)
    {
        health -= dmg;
        float temp = (health / maxHealth) * width;
        rectTransform.sizeDelta = new Vector2(temp, height);
    }
}
