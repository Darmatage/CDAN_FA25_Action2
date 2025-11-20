using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.EventSystems;

public class BossBehavior_KrakenTargeting : MonoBehaviour
{
    //this script should be attached to the kraken tentacle prefab. The tentacle should track the player's horizontal position, while remaining on the ground.
    //This only controls the movement of the tentacle.
    //is there a way to have this script receive signals from the kraken behavior script?

    public GameObject player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        TargetPlayer();
    }

    void TargetPlayer()
    {
        float targetY = 0f; //find location of ground at target location
        RaycastHit groundTargeter;

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out groundTargeter, Mathf.Infinity, 3)) //cast a ray downwards, colliding with terrain
        {
            targetY = groundTargeter.point.y; //if ray collides with terrain, set target y to ground level
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * groundTargeter.distance, Color.yellow);
            Debug.Log("Did Hit");
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * groundTargeter.distance, Color.red);
            Debug.Log("Uh oh!!!");
        }

            Vector3 targetLocation = new Vector3(player.transform.position.x, targetY + 0.5f, player.transform.position.z);
    }
}
