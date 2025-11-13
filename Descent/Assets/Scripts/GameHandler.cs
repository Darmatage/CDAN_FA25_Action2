using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameHandler : MonoBehaviour
{
    public float levelTimer;
    public float maxTime = 180f;
    

    void Start()
    {
        
    }

    void FixedUpdate()
    {
        levelTimer = Time.timeSinceLevelLoad; //seconds since scene load

        if (levelTimer > maxTime) //like 3 minutes
        {
            SceneManager.LoadScene("EndLose");
            //game over!!
            //probably play an animation before this in the final game
        }
    }

    public void PlayerHurt(int damageTaken)
    {
        //when collision with an enemy hurtbox is detected
        //attack strength * player's armor value
        //subtract from health
    }
}
