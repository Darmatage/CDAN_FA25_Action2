using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameHandler : MonoBehaviour
{
    public float levelTimer;
    public float maxTime = 180f;

    [Header("Player Stats")]
    public static float playerCurrentHealth = 100f;
    public static float playerMaxHealth = 100f;
    public static float playerArmor = 0.9f; //direct multiplier to damage taken
    public static float IFrames = 30f; //frames of immunity after taking damage

    public static float meleeDamage = 10f; //damage of bite
    public static float projectileDamage = 5f; //damage of projectile

    void Start()
    {
        
    }

    void FixedUpdate()
    {
        levelTimer = Time.timeSinceLevelLoad; //seconds since scene load

        if (levelTimer > maxTime || playerCurrentHealth <= 0) //like 3 minutes or death
        {
            SceneManager.LoadScene("EndLose");
            //game over!!
            //probably play an animation before this in the final game
        }

        
    }

    private void Update()
    {
        
    }

    public float DamageCalc(float takenDamage, float Armor) //calculates damage taken based on attacker's attack and defender's armor
    {
        float totalDamage = takenDamage * Armor;

        //other things can go here, like crits or weaknesses

        return totalDamage;
    }
}
