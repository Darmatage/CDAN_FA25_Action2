using JetBrains.Annotations;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class EnemyBehavior_Melee : MonoBehaviour
{
    //MOVEMENT
    public float moveSpeed = 1f;
    public float patrolSpeed = 0.5f;
    private float patrolTimer; //controls time between move attempts
    private bool isOnPatrol;
    private bool isIdlePatrol;
    private bool isMoving = false;

    private Vector3 moveLocation;
    public Transform player;
    public Transform enemyLocation;
    
    
    
    //BEHAVIOR
    public bool isAggro = false;
    public float aggroRange = 3f; //player detection range


    private bool isAttackWindup = false;
    private bool isAttackCooldown = false;
    private bool isAttackActive = false;
    private bool isAttacking = false;
    public float AttackRange = 0.5f; //distance when attack will be executed
    public GameObject Hurtbox; //attack collider
    private float AttackTimer; //controls attack length

    private GameObject EnemyHome;
    private float MaxHomeDist = 10f;

    //STATS
    public float enemyHealth = 10f;
    public static float enemyStrength = 5f;
    public float enemyArmor = .5f; //multiplier to damage taken? 

    void Start()
    {
        Hurtbox.SetActive(false);
        EnemyHome = new GameObject("EnemyHome"); //create home
        EnemyHome.transform.parent = transform; //home is where the me is :)
    }

    void Update()
    {
        float distToPlayer = Vector3.Distance(transform.position, player.position); //get distance to player
        float distToHome = Vector3.Distance(transform.position, EnemyHome.transform.position); //get distance from home point

        //MOVEMENT
        if (isAggro && isAttacking) //Actively executing attack
        {
            transform.LookAt(player);
        }
        else if (isAggro) //Chase state
        {
            //move towards player
            Vector3 LERPposition = Vector3.Lerp(transform.position, player.position, moveSpeed * Time.deltaTime);
            transform.position = LERPposition;
            transform.LookAt(player);
            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
        }
        else if (distToHome < MaxHomeDist) //Patrol state
        {
            float distToTarget;

            if (!isOnPatrol && !isIdlePatrol) //not currently moving or waiting
            {
                moveLocation = getRandomVector();
                Vector3 LERPposition = Vector3.Lerp(transform.position, moveLocation, moveSpeed * Time.deltaTime);
                transform.position = LERPposition;
                transform.LookAt(moveLocation);
                //go to point
                isOnPatrol = true;
            }
            else if (isOnPatrol) //actively moving, checking for target reach
            {
                distToTarget = Vector3.Distance(transform.position, moveLocation);
                
                if(distToTarget < 0.1) //location reached
                {
                    isOnPatrol = false;
                    isIdlePatrol = true;
                    patrolTimer = Random.Range(3f, 10f);//wait 3-10 seconds
                }
            }
            
        }
        else //if outside of home range and player is out of aggro range
        {
            Vector3 LERPposition = Vector3.Lerp(transform.position, EnemyHome.transform.position, moveSpeed * Time.deltaTime);
            transform.position = LERPposition;
            transform.LookAt(EnemyHome.transform.position);
        }

        //DETECTION
        if(distToPlayer <= AttackRange) //player is in attack range
        {
            isAttacking = true;
        }
        else if (distToPlayer <= aggroRange) //player is in aggro range
        {
            isAggro = true;
            isAttacking = false;
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
            if (!isAttackCooldown)
            {
                isAttackWindup = true; //begin windup
                //play the animation here also
            }
            //attacking, activate hurtbox
            else if (!isAttackCooldown && !isAttackWindup)
            {
                Hurtbox.SetActive(true);
                isAttackActive = true; //begin active timer
            }
        }
    }

    void FixedUpdate()
    {
        //attack timers
        if (isAttacking) 
        {
            if (isAttackWindup) //winding up
            {
                AttackTimer = 30f; //time to windup

                while (AttackTimer > 0f)
                {
                    AttackTimer--; //countdown frames until 0
                }

                isAttackWindup = false;
            }
            if (isAttackActive) //attacking
            {
                AttackTimer = 30f; //time to attack

                while (AttackTimer > 0f)
                {
                    AttackTimer--; //countdown frames until 0
                }

                isAttackActive = false;
                isAttackCooldown = true;
            }
            if (isAttackCooldown) //cooling down
            {
                AttackTimer = 30f; //time to cooldown

                while (AttackTimer > 0f)
                {
                    AttackTimer--; //countdown frames until 0
                }

                Hurtbox.SetActive(false);
                isAttackCooldown = false;
                isAttacking = false; //done attacking
                //check if this hit the player
                EnemyHome.transform.position = transform.position; //if attack hit player, set home to current position
                //this is to prevent the enemy from taking a walk of shame back to their spawn if they chased the player far away
            }
        }

        //patrol movement timer
        if (isIdlePatrol) 
        {
            while(patrolTimer > 0)
            {
                patrolTimer -= Time.deltaTime; //countdown seconds until 0
            }

            isIdlePatrol = false; //done waitin
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
        } while (!attemptValid); //keep checking until valid location is found

        return moveAttempt; //return valid location
          
    }

}
