using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossStatHandler : MonoBehaviour
{
    //private Animator anim; 
    //private gameHandler

    [Header("Stats")]
    public float enemyMaxHealth = 10f;
    public float enemyCurrentHealth;
    public float enemyArmor = 0.5f; //multiplier to damage taken!

    private GateScript gate;
    public Renderer enemyRenderer;
    public Color whiteColor;
    public Color redColor;

    void Start(){
         enemyCurrentHealth = enemyMaxHealth;
         gate = GameObject.FindWithTag("door").GetComponent<GateScript>();
         
         //anim = GetComponentInChildren<Animator>();
         //Renderer enemyRenderer = GetComponent<Renderer>();
    }

    private void Update()
    {
        
    }

    public void EnemyDamage(float damageSource)
    {
        Debug.Log(damageSource);
        Debug.Log(enemyArmor);

        //anim.SetTrigger("EnemyHurt");
        StartCoroutine(EnemyHurtFlash());

        float totalDamage = damageSource * enemyArmor;
        //other things can go here, like crits or weaknesses
        Debug.Log("Total Damage: " + totalDamage);
        enemyCurrentHealth -= totalDamage;
        if (enemyCurrentHealth <= 0)
        {
            //anim.SetBool("EnemyDead", true);
            //GameHandler.enemiesKilled += 1;
            //if (GameHandler.enemiesKilled >= GameHandler.enemyGoal)
            //{
                
            //}

            StartCoroutine(EnemyDeath());
        }
    }

    IEnumerator EnemyDeath()
    {
        gate.defeatABoss();
        GameHandler.bossesDefeated++;
        yield return new WaitForSeconds(1f);
        
        Destroy(gameObject);
        

    }

    IEnumerator EnemyHurtFlash()
    {
        enemyRenderer.material.color = redColor;
        yield return new WaitForSeconds(1f);
        enemyRenderer.material.color = whiteColor;
    }

}
