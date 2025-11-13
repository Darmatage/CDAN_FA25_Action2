using JetBrains.Annotations;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class EnemyBehavior_Melee : MonoBehaviour
{

    [Header("Movement")]
    public float moveSpeed = 1f;
    public float patrolSpeed = 0.5f;
    private float patrolTimer; //controls time between move attempts
    private float patrolFailsafe; //stops patrol if stuck
    private bool isOnPatrol = false;
    private bool isIdlePatrol = false;
    private bool isMoving = false;

    private Vector3 moveLocation;
    public Transform player;
    public Transform enemyLocation;



    [Header("Behavior")]
    public bool isAggro = false;
    public float aggroRange = 3f; //player detection range


    private bool isAttackWindup = false;
    private bool isAttackCooldown = false;
    private bool isAttackActive = false;

    public float windupTime = 30f;
    public float attackTime = 30f;
    public float cooldownTime = 30f;

    private bool executeAttack = false; //start the attack
    private bool isAttacking = false; //in the middle of the attack
    //public float ApproachSpeed = 0.01f; //movespeed while attacking
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
        if (isAggro && isAttacking) //Actively executing attack
        {
            /*Vector3 LERPposition = Vector3.Lerp(transform.position, player.position, ApproachSpeed * Time.deltaTime);
            transform.position = LERPposition;
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);*/
            transform.LookAt(player);
        }
        else if (isAggro) //Chase state
        {
            //move towards player
            Vector3 LERPposition = Vector3.Lerp(transform.position, player.position, moveSpeed * Time.deltaTime);
            transform.position = LERPposition;
            transform.LookAt(player);
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }
        else if (distToHome < MaxHomeDist) //Patrol state
        {
            float distToTarget;
            //Debug.Log("distance to home: " + distToHome);

            if (!isOnPatrol && !isIdlePatrol) //not currently moving or waiting
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
        if(distToPlayer <= AttackRange + 2) //player is in attack range
        {
            executeAttack = true;
            //Debug.Log("Attacking!");
        }
        else if (distToPlayer <= aggroRange) //player is in aggro range
        {
            isAggro = true;
            isAttacking = false;
            //Debug.Log("Distance to player: " + distToPlayer);
        }
        else
        {
            isAggro = false;
            isAttacking = false;
        }

        //ATTACKING
        if (isAttacking)
        {
            //windup attack
            if (executeAttack)
            {
                isAttackWindup = true; //begin windup
                //play the animation here also
            }
            //attacking, activate hurtbox
            else if (isAttackActive)
            {

                Hurtbox.SetActive(true);
                Debug.Log("Activating hurtbox!");
                isAttackActive = true; //begin active timer
            }
        }
    }

    void FixedUpdate()
    {
        //attack timers
        if (executeAttack || isAttacking) 
        {
            isAttacking = true;
            executeAttack = false;
            //Debug.Log("Attacking!");
            if (isAttackWindup) //winding up
            {
                while (AttackTimer <= windupTime)
                {
                    AttackTimer++;
                }
                Debug.Log("winding up!");
                isAttackWindup = false;
                isAttackActive = true;
                AttackTimer = 0;
            }
            if (isAttackActive) //attacking
            {

                while (AttackTimer <= attackTime)
                {
                    AttackTimer++;
                }
                Debug.Log("Attacking!");
                isAttackActive = false;
                isAttackCooldown = true;
                AttackTimer = 0;
            }
            if (isAttackCooldown) //cooling down
            {

                while (AttackTimer <= cooldownTime)
                {
                    AttackTimer++;
                }

                Hurtbox.SetActive(false);
                Debug.Log("Deactivating hurtbox!");
                isAttackCooldown = false;
                isAttacking = false; //done attacking
                AttackTimer = 0;
                //check if this hit the player
                EnemyHome.transform.position = transform.position;
                Debug.Log("Setting new home at: x - " + EnemyHome.transform.position.x + " y - " + EnemyHome.transform.position.y + " z - " + EnemyHome.transform.position.z);
                //if attack hit player, set home to current position
                //this is to prevent the enemy from taking a walk of shame back to their spawn if they chased the player far away
            }
        }

        //patrol movement timer
        if (isIdlePatrol) 
        {
            while(patrolTimer > 0)
            {
                patrolTimer -= Time.deltaTime; //countdown seconds until 0
                //Debug.Log("Waitin for " + patrolTimer);
            }

            isIdlePatrol = false; //done waitin
        }

        //patrol failsafe
        if (isOnPatrol)
        {

            while (patrolFailsafe > 0)
            {
                patrolFailsafe -= Time.deltaTime; //countdown seconds until 0
                
            }
            Debug.Log("Whoops!" + patrolFailsafe);

            isOnPatrol = false;
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
        GameObject targetlocation = new GameObject("Target location");
        targetlocation.transform.position = moveAttempt;
        return moveAttempt; //return valid location
          
    }

}
