using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting;

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
    public LayerMask enemyLayers;
    public GameObject FaceTarget; //direction player is facing
    public Vector3 FaceDirection;
    public Collider MyHurtbox;
    public GameHandler GameHandler;
    public AttackHandler AttackHandler;

    [Header("Behavior")]
    [Tooltip("Length of attack in seconds.")]
    public static float attackLength = 1f;
    [Tooltip("Length of attack cooldown in seconds.")]
    public static float attackCooldown = 1f;
    private bool isAttacking = false;
    private bool isCooldown = false;
    public float attackTimerMelee = 0f;
    public float immuneTimer = 60f;

    void Start()
    {
        AttackHandler script = GetComponentInChildren<AttackHandler>(true);
        //script.damageSource = GameHandler.meleeDamage; //assign strength to hitbox damage
        Hitbox.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= attackTimerMelee)
            if (Input.GetMouseButton(0)) //detect left mouse click
            {
                Debug.Log("I hit left mouse button");
                HandleAttackBite();
                attackTimerMelee = Time.time + 1f / attackCooldown;
            }

        HandleAttackProjectile();
        //Debug.DrawRay(transform.position, FaceTarget.transform.position, Color.red);
    }

    private void FixedUpdate()
    {
        //attack timers
        
        
        /*
        if (isAttacking)
        {
            //Hitbox.SetActive(true);
            
                
            attackTimerMelee += Time.deltaTime; //increment attack timer

            if(attackTimerMelee >= attackLength)
            {
                isCooldown = true;
                attackTimerMelee = 0f;

                isAttacking = false;
            }
        }
        
        if (isCooldown)
        {
            //Hitbox.SetActive(false);
            attackTimerMelee += Time.deltaTime;

            if (attackTimerMelee >= attackCooldown)
            {
                isCooldown = false;
                attackTimerMelee = 0f;
            }
        }
        */
    }

    void HandleAttackBite()
    {
        Debug.Log("Player Attacks");

        Collider[] hitEnemies = Physics.OverlapSphere(Hitbox.transform.position, Hitbox.GetComponent<SphereCollider>().radius * GameHandler.attackRadius, enemyLayers);

            foreach (Collider enemy in hitEnemies)
            {
                Debug.Log("We hit " + enemy.name);
                //enemy.GetComponent<EnemyStatHandler>().EnemyDamage(GameHandler.MeleeCalc());
                enemy.GetComponent<EnemyStatHandler>().EnemyDamage(10);
             }
    }

    void HandleAttackProjectile()
    {
        if (Input.GetMouseButton(1)) //detect right mouse click
        {
            
        }
    }

    public void Immunity()//managing immunity frames upon hit
    {

    }
    
    /*
    private void OnTriggerEnter(Collider other) //when thing hits player
    {
        Debug.Log("Something hit me, the player!");

        if (other.gameObject.tag == "Hitbox")
        {
            AttackHandler Hit = other.gameObject.GetComponent<AttackHandler>();
            GameHandler.playerCurrentHealth -= GameHandler.DamageCalc(Hit.damageSource, GameHandler.playerArmor); //calculate + apply damage
            Debug.Log("Something hit me! Current health: " + GameHandler.playerCurrentHealth);
            Immunity();

        }
    }
    */

}
