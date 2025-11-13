using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PlayerBehavior : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    /*
    [Header("Stats")]
    public static float playerCurrentHealth = 100f;
    public static float playerMaxHealth = 100f;
    public static float playerArmor = 0.9f; //direct multiplier to damage taken
    public static float IFrames = 30f; //frames of immunity after taking damage
    
    public static float meleeDamage = 10f; //damage of bite
    public static float projectileDamage = 5f; //damage of projectile
    */

    public GameObject Hitbox;
    public AttackHandler AttackHandler;

    [Header("Behavior")]
    public static float attackLength = 3f;
    public static float attackCooldown = 3f;
    private bool isAttacking = false;
    private bool isCooldown = false;
    float attackTimer = 0f;

    void Start()
    {
        Hitbox.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        HandleAttack();
    }

    private void FixedUpdate()
    {
        //attack timers
        

        if (isAttacking)
        {
            Hitbox.SetActive(true);
            attackTimer += Time.deltaTime;

            if(attackTimer >= attackLength)
            {
                isCooldown = true;
                attackTimer = 0f;

                isAttacking = false;
            }
        }
        if (isCooldown)
        {
            Hitbox.SetActive(false);
            attackTimer += Time.deltaTime;

            if (attackTimer >= attackCooldown)
            {
                isCooldown = false;
                attackTimer = 0f;
            }
        }
        
    }

    void HandleAttack()
    {
        if (Input.GetMouseButton(0))
        {
            if (!isCooldown)
            {
                isAttacking = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if ( other.gameObject.tag = "Hitbox")
        {
            
        }
    }

}
