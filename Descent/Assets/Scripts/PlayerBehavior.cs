using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PlayerBehavior : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    [Header("Stats")]
    public static float playerCurrentHealth = 100f;
    public static float playerMaxHealth = 100f;
    public static float playerArmor = 0.9f; //direct multiplier to damage taken
    public static float IFrames = 30f; //frames of immunity after taking damage
    
    public static float meleeDamage = 10f; //damage of bite
    public static float projectileDamage = 5f; //damage of projectile



    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
