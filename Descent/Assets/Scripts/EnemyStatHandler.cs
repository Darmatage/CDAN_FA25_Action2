using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem.LowLevel;

public class EnemyStatHandler : MonoBehaviour
{
    //private Animator anim; 
    public GameHandler GameHandler;

    [Header("Stats")]
    public float enemyMaxHealth = 10f;
    public float enemyCurrentHealth;
    public float enemyArmor = 0.5f; //multiplier to damage taken!
    public int enemyReward = 5; //amount of coins dropped
    public bool isBoss = false;
    bool isDying = false;

    public ParticleSystem coins;
    public ParticleSystem enemydeath;
    public Renderer enemyRenderer;
    public Color whiteColor;
    public Color redColor;

    void Start(){
         enemyCurrentHealth = enemyMaxHealth;
        GameHandler = GameObject.FindWithTag("GameHandler").GetComponent<GameHandler>();
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
        //Debug.Log("Total Damage: " + totalDamage);
        enemyCurrentHealth -= totalDamage;
        if (enemyCurrentHealth <= 0)
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
                GameHandler.playerGetCoins(enemyReward);
                StartCoroutine(EnemyDeath());
            }
            

            isDying = true;
        }
    }

    IEnumerator EnemyDeath()
    {
        var emitParams = new ParticleSystem.EmitParams();

        enemyRenderer.material.color = Color.black;
        yield return new WaitForSeconds(1f);
        coins.Emit(emitParams, enemyReward);
        Destroy(gameObject);
    }

    IEnumerator EnemyHurtFlash()
    {
        enemyRenderer.material.color = redColor;
        yield return new WaitForSeconds(1f);
        enemyRenderer.material.color = whiteColor;
    }

}
