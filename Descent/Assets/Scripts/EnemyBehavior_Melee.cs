using JetBrains.Annotations;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class EnemyBehavior_Melee : MonoBehaviour
{

    public GameHandler GameHandler;
    public AttackHandler AttackHandler;

    [Header("Movement")]
    public float moveSpeed = 1f;
    public float patrolSpeed = 0.5f;
    private float patrolTimer; //controls time between move attempts
    public float patrolFailsafe; //stops patrol if stuck
    public bool isOnPatrol = false;
    public bool isIdlePatrol = false;
        
    private Vector3 moveLocation;
    public Transform player;
    public Transform enemyLocation;



    [Header("Behavior")]
    public bool isAggro = false;
    public float aggroRange = 3f; //player detection range


    public bool isAttackWindup = false;
    public bool isAttackCooldown = false;
    public bool isAttackActive = false;

    public float windupTime = 30f;
    public float attackTime = 30f;
    public float cooldownTime = 30f;

    private bool executeAttack = false; //start the attack
    private bool isAttacking = false; //in the middle of the attack
    public float ApproachSpeed = 0.01f; //movespeed while attacking
    public float AttackRange = 0.5f; //distance when attack will be executed
    public GameObject Hurtbox; //attack collider
    private float AttackTimer; //controls attack length

    private GameObject EnemyHome;
    public float MaxHomeDist = 10f;

    [Header("Stats")]
    public float enemyHealth = 10f;
    public static float enemyStrength = 5f;
    public float enemyArmor = .5f; //multiplier to damage taken? 

    void Start()
    {
        enemyLocation.localEulerAngles = new Vector3(transform.localEulerAngles.x, 90, transform.localEulerAngles.z);
        Hurtbox.SetActive(false);
        EnemyHome = new GameObject("EnemyHome"); //create home
        EnemyHome.transform.parent = transform; //home is where the me is :)
        Debug.Log("Setting new home at: x - " + EnemyHome.transform.position.x + " y - " + EnemyHome.transform.position.y + " z - " + EnemyHome.transform.position.z);
    }

    void Update()
    {
        float distToPlayer = Vector3.Distance(transform.position, player.position); //get distance to player
        float distToHome = Vector3.Distance(transform.position, EnemyHome.transform.position); //get distance from home point

        //MOVEMENT
        if (isAttacking) //Actively executing attack
        {
            if (isAttackActive)
            {
                Vector3 LERPposition = Vector3.Lerp(transform.position, player.position, ApproachSpeed * Time.deltaTime);
                transform.position = LERPposition;
                transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            }
            transform.LookAt(player);
        }
        else if (isAggro && !isAttacking) //Chase state
        {
            //move towards player
            Vector3 LERPposition = Vector3.Lerp(transform.position, player.position, moveSpeed * Time.deltaTime);
            transform.position = LERPposition;
            transform.LookAt(player);
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            //Debug.Log(isAttacking);
        }
        else if (distToHome < MaxHomeDist) //Patrol state
        {
            float distToTarget;
            //Debug.Log("distance to home: " + distToHome);

            if (!isOnPatrol && !isIdlePatrol && !isAttacking) //not currently moving or waiting
            {
                moveLocation = getRandomVector();
                isOnPatrol = true;
                patrolFailsafe = 100f;
                Debug.Log("Going on patrol!");
            }
            else if (isOnPatrol) //actively moving, checking for target reach
            {
                distToTarget = Vector3.Distance(transform.position, moveLocation);
                
                Vector3 LERPposition = Vector3.Lerp(transform.position, moveLocation, patrolSpeed * Time.deltaTime);//go to point
                transform.position = LERPposition;
                transform.LookAt(moveLocation);
                //Debug.Log("Movin!");
                if (distToTarget < 0.1) //location reached
                {
                    isOnPatrol = false;
                    isIdlePatrol = true;
                    patrolTimer = Random.Range(3f, 10f);//wait 3-10 seconds
                }
            }
            else
            {
                //do nothing
            }
            
        }
        else //if outside of home range and player is out of aggro range
        {
            Debug.Log("Returning home! Distance to home: " + distToHome);
            Vector3 LERPposition = Vector3.Lerp(transform.position, EnemyHome.transform.position, patrolSpeed * Time.deltaTime);
            transform.position = LERPposition;
            transform.LookAt(EnemyHome.transform.position);
        }

        //DETECTION
        if(distToPlayer <= AttackRange && !isAttacking) //player is in attack range and isn't attacking
        {
            executeAttack = true;
            //Debug.Log("Attacking!");
        }
        else if (distToPlayer <= aggroRange) //player is in aggro range
        {
            isAggro = true;
            isOnPatrol = false;
            isIdlePatrol= false;
            //Debug.Log("Distance to player: " + distToPlayer);
        }
        else if (isAttacking)
        {
            //do nothing
        }
        else
        {
            isAggro = false;
            isAttacking = false;
        }

        //ATTACKING
        if (executeAttack || isAttacking)
        {
            //windup attack
            if (executeAttack)
            {

                isAttackWindup = true;
                Debug.Log("Setting windup to true!");
                //begin windup
                //play the animation here also
            }

            //attacking, activate hurtbox
            if (isAttackActive)
            {
                Hurtbox.SetActive(true);
                //Debug.Log("Activating hurtbox!");
            }
            else
            {
                Hurtbox.SetActive(false);
            }
        }
    }

    void FixedUpdate()
    {
        //attack timers
        if (executeAttack)
        {
            isAttacking = true;
            //Debug.Log("Setting attacking to true!");
            //Debug.Log(isAttacking);
            executeAttack = false;
        }
        if (isAttacking) 
        {

            Attack();
        }

        //patrol movement timer
        if (isIdlePatrol) 
        {
            if(patrolTimer > 0)
            {
                patrolTimer -= Time.deltaTime; //countdown seconds until 0
                //Debug.Log("Waitin for " + patrolTimer);
            }
            else
            {
                isIdlePatrol = false; //done waitin
            }
                
        }

        //patrol failsafe
        if (isOnPatrol)
        {
            if (patrolFailsafe > 0)
            {
                patrolFailsafe -= Time.deltaTime; //countdown seconds until 0
                
            }
            else
            {
                isOnPatrol = false;
            }
        }
    }

    private void Attack()
    {
        //Debug.Log("Attacking!");
        if (isAttackWindup) //winding up
        {

            if (AttackTimer <= windupTime)
            {
                AttackTimer++;
                // Debug.Log(AttackTimer);
            }
            else
            {
                Debug.Log("winding up!");
                isAttackActive = true;
                AttackTimer = 0;
                isAttackWindup = false;
            }
        }
        if (isAttackActive) //attacking
        {

            if (AttackTimer <= attackTime)
            {
                AttackTimer++;
            }
            else
            {
                Debug.Log("Attacking!");
                isAttackCooldown = true;
                AttackTimer = 0;
                isAttackActive = false;
            }

        }
        if (isAttackCooldown) //cooling down
        {

            if (AttackTimer <= cooldownTime)
            {
                AttackTimer++;
            }
            else
            {
                
                isAttacking = false; //done attacking
                Debug.Log("Setting attacking to false!");
                AttackTimer = 0;
                //check if this hit the player
                EnemyHome.transform.position = transform.position;
                Debug.Log("Setting new home at: x - " + EnemyHome.transform.position.x + " y - " + EnemyHome.transform.position.y + " z - " + EnemyHome.transform.position.z);
                //if attack hit player, set home to current position
                //this is to prevent the enemy from taking a walk of shame back to their spawn if they chased the player far away
                isAttackCooldown = false;
            }

        }
    }
    private Vector3 getRandomVector() //get point within home range to move to
    {
        bool attemptValid = false;
        Vector3 moveAttempt;
        do
        {
            moveAttempt = (Random.insideUnitSphere * MaxHomeDist); //pick direction within home range
            moveAttempt += EnemyHome.transform.position; //center vector on home
            //this results in a target point i think. vectors are confusing
            if (!Physics.Linecast(transform.position, moveAttempt, 3)) //check if move intersects terrain
            {
                attemptValid = true;
            }
            else
            {
                Debug.Log("Move attempt invalid!");
            }
        } while (!attemptValid); //keep checking until valid location is found
        Debug.Log("Attempt succeded! Target position: x - " + moveAttempt.x + " y - " + moveAttempt.y + " z - " + moveAttempt.z);
        //GameObject targetlocation = new GameObject("Target location");
        //targetlocation.transform.position = moveAttempt;
        return moveAttempt; //return valid location
          
    }
    
    
    private void OnTriggerEnter(Collider other) //when collision with this object is detected
    {
        if(other.gameObject.tag == "Player") //if it's the player
        {
            
            GameHandler.playerCurrentHealth -= GameHandler.DamageCalc(enemyStrength, GameHandler.playerArmor); //calculate + apply damage
        }
        if (other.gameObject.tag == "Hitbox") //if it's a hurtbox
        {
            GameHandler.DamageCalc(GameHandler.meleeDamage, enemyArmor);
            GameHandler.playerCurrentHealth -= GameHandler.DamageCalc(enemyStrength, GameHandler.playerArmor); //calculate + apply damage
        }
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, aggroRange); //gizmo of aggro range
    }

    public string FetchSource()
    {
        return "playerMelee";
    }

}
