using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PlayerBehavior : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    //STATS
    public static int playerHealth = 20;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayerHurt(int damageTaken)
    {
        //when collision with an enemy hurtbox is detected
        //attack strength * player's armor value
        //subtract from health
    }
}
