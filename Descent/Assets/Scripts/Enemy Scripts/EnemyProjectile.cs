using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public GameHandler gameHandlerObj;
    public int damage = 1;
    public float speed = 10f;
    private Transform playerTrans;
    private Vector2 target;
    public GameObject hitEffectAnim;
    public float SelfDestructTime = 2.0f;

    void Start()
    {
        if (gameHandlerObj == null)
        {
            gameHandlerObj = GameObject.FindWithTag("GameHandler").GetComponent<GameHandler>();
        }
        

    }

    void Update()
    {
        //transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }

    //if the bullet hits a collider, play the explosion animation, then destroy the effect and the bullet
    void OnTriggerEnter(Collider collision)
    {
        

        if (collision.gameObject.tag == "Player")
        {
            gameHandlerObj.playerGetHit(damage);
        }
        if (collision.gameObject.tag != "Hurtbox")
        {   
            //Debug.Log("hit a " + collision.gameObject.tag);
            StartCoroutine(selfDestruct());
            //GameObject animEffect = Instantiate(hitEffectAnim, transform.position, Quaternion.identity);
            //Destroy(animEffect, 0.5f);
            Destroy(gameObject);
        }
    }

    IEnumerator selfDestruct()
    {
        yield return new WaitForSeconds(SelfDestructTime);
        Destroy(gameObject);
    }
}
