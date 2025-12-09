using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem.LowLevel;

public class EnemyStatHandler : MonoBehaviour
{
    //private Animator anim; 
    public GameHandler GameHandler;
    public GameObject coins;
    public GameObject deathParticles;
    public GameObject hitParticles;
    public GameObject criticalParticles;
    //aa
    [Header("Stats")]
    public float enemyMaxHealth = 15f;
    public float enemyCurrentHealth;
    public float enemyArmor = 0.5f; //multiplier to damage taken!
    public int enemyReward = 5; //amount of coins dropped
    public bool isBoss = false;
    public bool isDying = false;

    
    public Renderer enemyRenderer;
    public Color whiteColor;
    public Color redColor;

    void Start(){
         
        GameHandler = GameObject.FindWithTag("GameHandler").GetComponent<GameHandler>();
        //anim = GetComponentInChildren<Animator>();
        //Renderer enemyRenderer = GetComponent<Renderer>();

        //Health scaling
        float healthMult = 1;

        switch (GameHandler.currentLevel + 1)
        {
            case 1: healthMult = 1; break;

            case 2: healthMult = 1.3f; break;

            case 3: healthMult = 1.5f; break;

            case 4: healthMult = 1.7f; break;

        }

        enemyMaxHealth *= healthMult;
        enemyCurrentHealth = enemyMaxHealth;
    }

    private void Update()
    {
        
    }

    public void EnemyDamage(float damageSource, bool isCrit)
    {
        //Debug.Log(damageSource);
        //Debug.Log(enemyArmor);

        //anim.SetTrigger("EnemyHurt");

        if (!isDying) //if enemy is alive
        {
            float totalDamage = damageSource * enemyArmor;
            enemyCurrentHealth -= totalDamage;
        }

        if (enemyCurrentHealth <= 0) //if enemy dies
        {
            
            //anim.SetBool("EnemyDead", true);
            if(!isDying)
            {
                GameHandler.enemiesKilled++;
                if (isBoss) //if enemy is a boss
                {
                    GateScript gate;
                    gate = GameObject.FindWithTag("Door").GetComponent<GateScript>();
                    gate.defeatABoss();
                }
                GameObject deathPS = Instantiate(deathParticles, transform.position, Quaternion.identity);
                
                StartCoroutine(EnemyDeath());
            }
            

            isDying = true;
        }
        else
        {   
            if (isCrit) //change particles based on criticals
            {
                GameObject critPS = Instantiate(criticalParticles, transform.position, Quaternion.identity);
            }
            else
            {
                GameObject hitPS = Instantiate(hitParticles, transform.position, Quaternion.identity);
            }

                
            StartCoroutine(EnemyHurtFlash());
        }

            
    }

    IEnumerator EnemyDeath()
    {
        var emitParams = new ParticleSystem.EmitParams();

        enemyRenderer.material.color = Color.black;
        yield return new WaitForSeconds(1f);
        GameObject coinPS = Instantiate(coins, transform.position, Quaternion.identity);
        coinPS.GetComponent<ParticleSystem>().Emit(emitParams, enemyReward);
        GameHandler.playerGetCoins(enemyReward);
        Destroy(gameObject);
    }

    IEnumerator EnemyHurtFlash()
    {
        
        enemyRenderer.material.color = redColor;
        yield return new WaitForSeconds(0.2f);
        enemyRenderer.material.color = whiteColor;
        
    }

}
