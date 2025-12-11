using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PillarAttack : MonoBehaviour
{
    public GameHandler gameHandlerObj;
    //public GameObject pillarPrefab;
    public GameObject player;
    public GameObject warningMSG;

    public Renderer pillarRenderer;
    public Material warningMat;
    public Material attackMat;

    public int damage = 30;
    public float warningTime = 3f; //time warning is displayed
    public float attackTime = 2f; //time attack is active
    public float moveSpeed = 10f; //current speed
    //public float moveAccel = 0f; //current acceleration
    public float moveDrag = 0.1f; //drag
    public float turnSpeed = 1f; //speed of turn

    Vector3 target;

    public bool isAttacking; //attack is active
    void Start()
    {
        if (gameHandlerObj == null)
        {
            gameHandlerObj = GameObject.FindWithTag("GameHandler").GetComponent<GameHandler>();
        }

        player = GameObject.Find("Player"); //get player

        warningMSG.SetActive(false);
        StartCoroutine(Activate());

    }

    private void Update()
    {
        if (!isAttacking)
        {
            Chase();
        }

        //set heigh of warning to player plus a bit
        warningMSG.transform.position = new Vector3(transform.position.x, player.transform.position.y + 2, transform.position.z);
    }

    void Chase()
    {
        Vector3 faceDirection = Vector3.forward; //get facing direction
        Vector3 playerPosition = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z); //get player position, aligned horizontally with pillar
        Vector3 playerDirection = (playerPosition - transform.position).normalized;
        target = Vector3.RotateTowards(faceDirection, playerDirection, turnSpeed, moveSpeed); //set target rotation

        Debug.DrawRay(transform.position, target, Color.red);
        Debug.DrawRay(transform.position, playerDirection, Color.green);
        //transform.LookAt(target);
        transform.Translate(target * moveSpeed * Time.deltaTime);

        //moveSpeed -= moveDrag;

        //if (moveSpeed < 0) { moveSpeed = 0; } //if below 0, set to 0
    }

    IEnumerator Activate()
    {   
        pillarRenderer.material = warningMat; //make that thing yellow
        warningMSG.SetActive(true); //enable warning display

        yield return new WaitForSeconds(warningTime);

        pillarRenderer.material = attackMat;
        warningMSG.SetActive(false);
        isAttacking = true;


        yield return new WaitForSeconds(attackTime);

        //isAttacking = false;
        Destroy(gameObject); //after attacking, kill self

    }

    private void OnTriggerStay(Collider other) //when hitbox impacts an object
    {
        if (other.gameObject.tag == "Player"  && isAttacking)
        {
            //gameHandlerObj.immuneTimer = 100; //ignores player immunity
            gameHandlerObj.playerGetHit(damage);    
        }
        
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(target, 1); //gizmo of aggro range
        Gizmos.DrawRay(transform.position, target);
    }
}
