using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;
//using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEngine.Rendering.DebugUI;

public class EnemyBehavior_Ranged : MonoBehaviour
{

    private GameHandler gameHandler;
    public EnemyStatHandler enemyStatHandler;
    public GameObject projectile;
    public GameObject firePoint;
    //public AttackHandler AttackHandler;

    [Header("Movement")]
    public float moveSpeed = 1f;
    public float patrolSpeed = 0.5f;
    private float patrolTimer; //controls time between move attempts
    public float patrolFailsafe; //stops patrol if stuck
    public bool isOnPatrol = false;
    public bool isIdlePatrol = false;

    public float recoilTimer = 0f;
    bool inRecoil = false;
        
    private Vector3 moveLocation;
    public GameObject player;
    public LayerMask playerMask;
    //public Transform enemyLocation;

    public int damage = 5;
    public float projectileSpeed = 20f;
    public float aimVariance = 3f;

    [Header("Behavior")]

    public bool isAggro = false;
    public float aggroRange = 3f; //player detection range


    public bool isAttackWindup = false;
    public bool isAttackCooldown = false;
    public bool isAttackActive = false;

    public float windupTime = 0.1f; //time to initiate attack
    public float attackTime = 0.3f; //time to pause while attacking
    public float cooldownTime = 3f; //time until attack can be executed again

    private bool isAttacking = false; //in the middle of the attack
    public float ApproachSpeed = 0.01f; //movespeed while attacking
    public float AttackRange = 1f; //distance when attack will be executed
    //public GameObject Hitbox; //attack collider guide
    //private Collider hitCollider;
    //private float AttackTimer; //controls attack length
    public float CDTimer = 0f; //controls attack cooldown

    private Vector3 EnemyHome;
    public float MaxHomeDist = 10f;

    public AudioSource meleeHurtSFX;

    void Start()    
    {
        
        //Hitbox.SetActive(false);

        //Initialize patrol home
        EnemyHome = new Vector3(transform.position.x, transform.position.y, transform.position.z); //home is where the me is :)
        
        //Debug.Log("Setting new home at: x - " + EnemyHome.transform.position.x + " y - " + EnemyHome.transform.position.y + " z - " + EnemyHome.transform.position.z);


        player = GameObject.Find("Player"); //get player

        //hitCollider = new Collider.SphereCollider();

        if (GameObject.FindWithTag("GameHandler") != null){
            gameHandler = GameObject.FindWithTag("GameHandler").GetComponent<GameHandler>();
        }

        enemyStatHandler = gameObject.GetComponent<EnemyStatHandler>();

        float damageMult = 1; //damage scaling

        switch (GameHandler.currentLevel)
        {
            case 0: damageMult = 1; break;

            case 1: damageMult = 1.3f; break;

            case 2: damageMult = 1.5f; break;

            case 3: damageMult = 1.7f; break;

        }

        //Debug.Log("damage multiplier: " + damageMult);
        damage *= Mathf.RoundToInt(damageMult);
    }

    void Update()
    {
        player = GameObject.Find("Player");
        if (player == null){
            //Debug.Log("FUCK");
        }
        float distToPlayer = Vector3.Distance(transform.position, player.transform.position); //get distance to player
        float distToHome = Vector3.Distance(transform.position, EnemyHome); //get distance from home point

        //MOVEMENT

        if (recoilTimer > 0)
        {
            recoilTimer--;
        }
        else if (recoilTimer <= 0)
        {
            recoilTimer = 0;
            inRecoil = false;
        }


        if (enemyStatHandler.isDying) //Dying
        {
            transform.Translate(Vector3.back * ApproachSpeed * Time.deltaTime);
        }
        else if (inRecoil) //experiencing recoil
        {
            //recoil away from player
            transform.LookAt(player.transform);
            //Vector3 LERPposition = Vector3.Lerp(transform.position, player.transform.position, ApproachSpeed * Time.deltaTime);
            //transform.position = LERPposition;
            transform.Translate(Vector3.back * (moveSpeed * (recoilTimer / 100)) * Time.deltaTime);
        }   
        else if (isAttacking) //Actively executing attack
        {
            //move towards player at approach speed
            Vector3 LERPposition = Vector3.Lerp(transform.position, player.transform.position, ApproachSpeed * Time.deltaTime);
            transform.position = LERPposition;
            transform.Translate(Vector3.forward * ApproachSpeed * Time.deltaTime);
            transform.LookAt(player.transform);
        }
        else if (isAggro && !isAttacking) //Chase state
        {
            //move towards player
            Vector3 LERPposition = Vector3.Lerp(transform.position, player.transform.position, moveSpeed * Time.deltaTime);
            transform.position = LERPposition;
            transform.LookAt(player.transform.position);
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            //Debug.Log(isAttacking);
        }
        else if (distToHome < MaxHomeDist) //Patrol state
        {
            //Debug.Log("going on patrol! dist to player: " + distToPlayer);
        Patrol();
        }
        else //if outside of home range and player is out of aggro range
        {
            //Debug.Log("Returning home! Distance to home: " + distToHome);
            Vector3 LERPposition = Vector3.Lerp(transform.position, EnemyHome, patrolSpeed * Time.deltaTime);
            transform.position = LERPposition;
            transform.LookAt(EnemyHome);
        }

        //DETECTION
        if(distToPlayer <= AttackRange) //player is in attack range
        {
            isAggro = true;
            if (!isAttackCooldown && !enemyStatHandler.isDying)
            {
                //Debug.Log("Attacking!");
                isAttackCooldown = true;
                StartCoroutine(Attack());
                CDTimer = 0;
            }
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

    }

    void FixedUpdate()
    {
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

    private Vector3 getRandomVector() //get point within home range to move to
    {
        bool attemptValid = false;
        Vector3 moveAttempt;
        do
        {
            moveAttempt = (UnityEngine.Random.insideUnitSphere * MaxHomeDist); //pick direction within home range
            moveAttempt += EnemyHome; //center vector on home
            //this results in a target point i think. vectors are confusing
            if (!Physics.Linecast(transform.position, moveAttempt, 3)) //check if move intersects terrain
            {
                attemptValid = true;
            }
            else
            {
                //Debug.Log("Move attempt invalid!");
            }
        } while (!attemptValid); //keep checking until valid location is found
        //Debug.Log("Attempt succeded! Target position: x - " + moveAttempt.x + " y - " + moveAttempt.y + " z - " + moveAttempt.z);
        return moveAttempt; //return valid location
          
    }
    
    private void Patrol()
    {
        float distToTarget;
        //Debug.Log("distance to home: " + distToHome);

        if (!isOnPatrol && !isIdlePatrol && !isAttacking && !isAggro) //not currently moving or waiting or chasing
        {
            moveLocation = getRandomVector();
            isOnPatrol = true;
            patrolFailsafe = 100f;
            //Debug.Log("Going on patrol!");
        }
        else if (isOnPatrol) //actively moving, checking for target
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
                patrolTimer = UnityEngine.Random.Range(3f, 10f);//wait 3-10 seconds
            }
        }
        else
        {
            //do nothing
        }
    }

    

    /*
     * private void OnTriggerEnter(Collider other) //when thing hits me
    {
        Debug.Log("Something hit me, the enemy!");

        if (other.gameObject.tag == "Hitbox")
        {
            AttackHandler Hit = other.gameObject.GetComponent<AttackHandler>(); //asks for damage value of the hitbox
            
            Debug.Log("Something hit me! Current health: " + enemyCurrentHealth);
        }
    }
    */


    void OnCollisionEnter(Collision other){ //ATTACKING
        if (other.gameObject.tag=="Player")
            if (isAttackActive){
                gameHandler.playerGetHit(damage); 
            }
    }

 void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, aggroRange); //gizmo of aggro range
        Gizmos.DrawWireSphere(transform.position, AttackRange); //gizmo of aggro range
        Gizmos.DrawWireSphere(EnemyHome, 1); //gizmo of aggro range
    }

    IEnumerator Attack()
    {
        //Debug.Log("Starting attack coroutine!");

        
        isAttacking = true;

        yield return new WaitForSeconds(windupTime);

        for (int i = 0; i < 3; i++)
        {
            //Debug.Log("lionfish firing!");
            transform.LookAt(player.transform);
            Vector3 fwd = ( new Vector3
                ((firePoint.transform.position.x + UnityEngine.Random.Range(-aimVariance, aimVariance)), 
                (firePoint.transform.position.y + UnityEngine.Random.Range(-aimVariance, aimVariance)), 
                (firePoint.transform.position.z + UnityEngine.Random.Range(-aimVariance, aimVariance))) 
                - transform.position).normalized; //initialize vector projectile will travel in, some randomness added to aim
            
            
            
            GameObject Enemyprojectile = Instantiate(projectile, firePoint.transform.position, transform.rotation);
            Enemyprojectile.GetComponent<Rigidbody>().AddForce(fwd * projectileSpeed, ForceMode.Impulse);
            Enemyprojectile.GetComponent<EnemyProjectile>().damage = damage;
            recoilTimer = 60;
            inRecoil = true;
            yield return new WaitForSeconds(0.5f);
        }
        

        //yield return new WaitForSeconds(attackTime);
        //Debug.Log("done attacking.");
        isAttacking = false;

        yield return new WaitForSeconds(cooldownTime);

        isAttackCooldown = false;
    }

}