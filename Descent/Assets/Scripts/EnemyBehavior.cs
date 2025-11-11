using JetBrains.Annotations;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class EnemyBehavior : MonoBehaviour
{
    //MOVEMENT
    private Rigidbody rb;
    public float moveSpeed = 1f;
    private Vector3 moveDirection;
    //private Vector3 target;
    private Transform target;
    
    //BEHAVIOR
    public int aggressionState = 0;
    /*
     player detected = 1
     idle = 0
     */

    //public PlayerBehavior PlayerBehavior;

    //STATS
    public int enemyHealth = 10;
    public static int enemyStrength = 5;
    public float enemyArmor = .5f; //multiplier to damage taken? 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        target = GameObject.FindWithTag("Player").transform;

        if (Vector3.Distance(transform.position, target.position) < 1f)
        {
            //GameObject.FindWithTag("Player").GetComponent<PlayerBehavior>().PlayerHurt(enemyStrength);
            Debug.Log(PlayerBehavior.playerHealth);
           // PlayerBehavior.PlayerHurt(enemyStrength);
            Debug.Log("got them!");
            StartCoroutine(Waitin());
            Debug.Log("done waitin");
        }
        /*
        else if (Vector3.Distance(transform.position, target.position) < 100f)
        {
            Idle();
        }
        */
        else
        {
            MoveToPlayer();
        }
    }

    void Idle()
    {

        //wait random amount of time
        Debug.Log("started waiting");
        Waitin();
        //pick a direction
        Debug.Log("movin");
        //move random length in that direction
    }

    IEnumerator Waitin()
    {
        yield return new WaitForSeconds(Random.Range(0, 5));
    }
    
    void MoveToPlayer()
    {
        //target = GameObject.FindWithTag("Player").GetComponent<Transform>().position;
        moveDirection = (GameObject.FindWithTag("Player").GetComponent<Transform>().position - transform.position).normalized; //get direction to player
        transform.Translate((moveDirection * moveSpeed) * Time.deltaTime);
        
         //find player
        //transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed); //get their ass
    }

    public void Attack()
    {
        
    }
}
