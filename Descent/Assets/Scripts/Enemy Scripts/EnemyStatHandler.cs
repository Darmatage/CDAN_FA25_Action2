using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class EnemyStatHandler : MonoBehaviour
{
    //private Animator anim; 
    public GameHandler GameHandler;
    public GameObject coins;
    public GameObject deathParticles;
    public GameObject hitParticles;
    public GameObject criticalParticles;

    //health bar display
    public GameObject healthBarObj;
    public Image healthBar;
    public TMP_Text healthBarText;
    public Color healthyColor = new Color(0.3f, 0.8f, 0.3f);
    public Color unhealthyColor = new Color(0.8f, 0.3f, 0.3f);
    //aa
    [Header("Stats")]
    public float enemyMaxHealth = 15f;
    public float enemyCurrentHealth;
    public float enemyArmor = 0.5f; //multiplier to damage taken!
    public int enemyReward = 5; //amount of coins dropped
    public bool isBoss = false;
    public bool isImmune = false;
    public bool isDying = false;

    
    public Renderer enemyRenderer;
    public Color whiteColor;
    public Color redColor;

    void Start(){
         
        GameHandler = GameObject.FindWithTag("GameHandler").GetComponent<GameHandler>();
        healthBarObj.SetActive(false);
        //anim = GetComponentInChildren<Animator>();
        //Renderer enemyRenderer = GetComponent<Renderer>();

        //Health scaling
        float healthMult = 1;

        switch (GameHandler.currentLevel)
        {
            case 0: healthMult = 1; break;

            case 1: healthMult = 1.3f; break;

            case 2: healthMult = 1.5f; break;

            case 3: healthMult = 2f; break;

        }

        enemyMaxHealth *= healthMult;
        enemyCurrentHealth = enemyMaxHealth;

        //Debug.Log("health multiplier: " + healthMult + "\nmaxhealth: " + enemyMaxHealth);
    }
    public void SetColor(Color newColor) //set healthbar color
    {
        healthBar.GetComponent<Image>().color = newColor;
    }

    public void EnemyDamage(float damageSource, bool isCrit)
    {
        if (!isImmune)
        { 
        //Debug.Log(damageSource);
        //Debug.Log(enemyArmor);

        //anim.SetTrigger("EnemyHurt");

        if (!isDying) //if enemy is alive
        {
            float totalDamage = damageSource * enemyArmor;
            enemyCurrentHealth -= totalDamage;
        }

        //health bar management

        healthBarObj.SetActive(true); //enable healthbar if damage taken

        healthBar.fillAmount = enemyCurrentHealth / enemyMaxHealth;
        healthBarText.text = (Math.Round(enemyCurrentHealth * 2, MidpointRounding.AwayFromZero) / 2) + "/" + enemyMaxHealth;

        if (enemyCurrentHealth < (0.3f * enemyMaxHealth))
        {
            if ((enemyCurrentHealth * 100f) % 3 <= 0) //?
            {
                SetColor(Color.white);
            }
            else
            {
                SetColor(unhealthyColor);
            }
        }
        else
        {
            SetColor(healthyColor);
        }

        if (enemyCurrentHealth <= 0) //if enemy dies
        {
            healthBarText.text = 0 + "/" + enemyMaxHealth;
            //anim.SetBool("EnemyDead", true);
            if (!isDying)
            {
                GameHandler.enemiesKilled++;

                if (isBoss) //if enemy is a boss
                {
                    GateScript gate;
                    gate = GameObject.FindWithTag("Door").GetComponent<GateScript>();
                    gate.defeatABoss();
                }
            }
            GameObject deathPS = Instantiate(deathParticles, transform.position, Quaternion.identity);

            StartCoroutine(EnemyDeath());

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
