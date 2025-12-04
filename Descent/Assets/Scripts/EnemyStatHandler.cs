using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyStatHandler : MonoBehaviour
{
    //private Animator anim; 

    [Header("Stats")]
    public float enemyMaxHealth = 10f;
    public float enemyCurrentHealth;
    public float enemyArmor = 0.5f; //multiplier to damage taken!

    public Renderer enemyRenderer;
    public Color whiteColor;
    public Color redColor;

    void Start(){
         enemyCurrentHealth = enemyMaxHealth;
         //anim = GetComponentInChildren<Animator>();
         //Renderer enemyRenderer = GetComponent<Renderer>();
    }

    private void Update()
    {
        
    }

    public void EnemyDamage(float damageSource)
    {
        //Debug.Log(damageSource);
        //Debug.Log(enemyArmor);

        //anim.SetTrigger("EnemyHurt");
        StartCoroutine(EnemyHurtFlash());

        float totalDamage = damageSource * enemyArmor;
        //other things can go here, like crits or weaknesses
        //Debug.Log("Total Damage: " + totalDamage);
        enemyCurrentHealth -= totalDamage;
        if (enemyCurrentHealth <= 0)
        {
            //anim.SetBool("EnemyDead", true);
            GameHandler.enemiesKilled += 1;
            //if (GameHandler.enemiesKilled >= GameHandler.enemyGoal)
            //{
            //    
            //}

            StartCoroutine(EnemyDeath());
        }
    }

    IEnumerator EnemyDeath()
    {
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
