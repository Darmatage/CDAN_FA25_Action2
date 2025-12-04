using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 

public class GameHandler : MonoBehaviour
{
    [Header("Level Stats")]
    public static int currentLevel = 1; //current level
    public float levelTimer; //current time elapsed in level
    public float maxTime = 180f; //time limit for level
	private string sceneName;
    public static int enemiesKilled = 0; //current enemies killed
    public int enemyGoal = 25; //enemies to kill for gate unlocking
	public static string lastLevelDied;  //allows replaying the Level where you died

    public static bool inShop = false;

    [Header("Player Stats")]

	private GameObject player;
	
    public static int gotCoins = 0;
    
    //health and armor
    public static float playerCurrentHealth = 100f;
    public static float playerMaxHealth = 100f;
    public static float playerMaxHealthBase = 100f;
    public static float playerArmor = 1f; //direct multiplier to damage taken
    public static float playerArmorBase = 1f; //default armor value
    public static float IFrames = 30f; //frames of immunity after taking damage

    //attacks
    public static float meleeDamage; //damage of bite
    public static float meleeCD = 1f; //cooldown of bite
    public static float projectileDamage; //damage of projectile
    public static float projectileCD = 1f; //cooldown of projectile
    public static float critRate = 0.2f; //critical rate, 1 = 100%, 0.5 = 50% etc
    public static float attackRadius = 1f;

    //dashing
    public static float dashCD = 3f; //cooldown between dashes
    public static float speedMultiplier = 1f; //multiplier to speed

    [Header("UI elements")]

    public TMP_Text healthText;
    public TMP_Text coinsText;
    public GameObject weaponIcon;
    public GameObject projectileIcon;

    public Sprite weapon_claw;
    public Sprite weapon_bite;

    public Sprite proj_shot;
    public Sprite proj_beam;

    [Header("Mutation handler")]

    public static int MeleeType = 1; //tracks what "main weapon" currently is
    /*
     * 1 = bite, default
     * 2 = claws, bleed effect, faster attack speed, shorter range
     * 3 = tail whip, huge range that scales off max hp, slow speed?
     */
    public static int RangedType = 11; //tracks current ranged weapon
    /*
     * 11 = steam shot, default
     * 12 = beam, hitscan held, pierce
     * 13 = bomb, aoe blast?
     */
    public static int extraHP = 0; //stacks of extra hp
    public static int extraAttack = 0; //stacks of extra attack
    public static int extraGreed = 0; //stacks of extra greed
    public static int extraArmor = 0; //stacks of extra armor
    public static int lifesteal = 0; //stacks of extra lifesteal
    public static int extraDashCD = 0; //stacks of extra dash cooldown


    void Start()
    {
        updateStatsDisplay();
        UpdateWeapon(1);
        UpdateProjectile(1);
    }

    void FixedUpdate()
    {
        levelTimer = Time.timeSinceLevelLoad; //seconds since scene load

        if ((levelTimer > maxTime || playerCurrentHealth <= 0) && !inShop) //like 3 minutes or death
        {
            SceneManager.LoadScene("EndLose");
            //game over!!
            //probably play an animation before this in the final game
        }

        
    }

    

    //COIN COUNTING
    public void playerGetCoins(int newCoins){ //gettin coins
            gotCoins += Mathf.RoundToInt(newCoins * (1 + (extraGreed * 0.05f))); //multiply gained coins by 5% per greed stack
            updateStatsDisplay();
    }

    public void playerLoseCoins(int newCoins) //losin coins
    {
        gotCoins -= newCoins;
        updateStatsDisplay();
    }

    public void updateStatsDisplay(){ //update totals
            healthText.text = playerCurrentHealth + "/" + playerMaxHealth;
            coinsText.text = gotCoins.ToString();
            
    }
    
    //DAMAGE CALCULATION
    public void playerGetHit(int damage){ //player gets hit
           //if (isDefending == false){
                  playerCurrentHealth -= damage;
                  if (playerCurrentHealth >=0){
                        updateStatsDisplay();
                  }
                  if (damage > 0){
                        //play GetHit animation:
                        //player.GetComponent<PlayerHurt>().playerHit();
                  }
            //}

           if (playerCurrentHealth > playerMaxHealth){
                  playerCurrentHealth = playerMaxHealth;
                  updateStatsDisplay();
            }

           if (playerCurrentHealth <= 0){
                  playerCurrentHealth = 0;
                  updateStatsDisplay();
                  //playerDies();
            }
      }

    public float MeleeCalc() //calculates player's melee damage
    {
        

        switch (MeleeType) //Get stats of current weapon
        {
            case 1: //bite
                meleeDamage = 30f;
                meleeCD = 2f;
                critRate = 0.1f;
                attackRadius = 1f;
                break;
            case 2: //claws
                meleeDamage = 15f;
                meleeCD = 3f;
                critRate = 0.3f;
                attackRadius = 0.8f;
                break;
        }

        float totalDamage = meleeDamage * //base damage 
            ((extraAttack * 0.1f) + 1); //attack passive modifier

        if (critRate > Random.value)
        {
            totalDamage *= 1.5f; //critical hits increase damage by 50%. effects can also be added here :3
        }
         Debug.Log("Sent Damage: " + totalDamage);

        //lifesteal
        if(lifesteal != 0)
        {
            playerCurrentHealth += totalDamage * (lifesteal * 0.05f); // heal 5% damage dealt(additive) for each stack
            if (playerCurrentHealth > playerMaxHealth) { playerCurrentHealth = playerMaxHealth; } //make sure it doesnt go over
        }

        return totalDamage;
    }

    public float ProjCalc()//calculates player's ranged damage
    {
        switch (RangedType) //Get stats of current weapon
        {
            case 11: //shot
                projectileDamage = 20f;
                projectileCD = 2f;
                break;
            case 12: //beam
                projectileDamage = 15f;
                projectileCD = 3f;
                break;
        }

        float totalDamage = projectileDamage * //base damage 
            ((extraAttack * 0.1f) + 1); //attack passive modifier

        return totalDamage;
    }

    /*
        public void playerDies(){
                player.GetComponent<PlayerHurt>().playerDead();       //play Death animation
                lastLevelDied = sceneName;       //allows replaying the Level where you died
                StartCoroutine(DeathPause());
          }

          IEnumerator DeathPause(){
                player.GetComponent<PlayerMove>().isAlive = false;
                player.GetComponent<PlayerJump>().isAlive = false;
                yield return new WaitForSeconds(1.0f);
                SceneManager.LoadScene("EndLose");
          }
    */

    //MUTATIONS
    public void UpdateWeapon(int type) //changes current weapon type
    {
        switch (type)
        {
            case 1: //BITE
                MeleeType = 1;
                weaponIcon.GetComponent<Image>().sprite = weapon_bite;
                break;
            case 2: //CLAW
                MeleeType = 2;
                weaponIcon.GetComponent<Image>().sprite = weapon_claw;
                break;
            
        }
    }

    public void UpdateProjectile(int type) //changes current projectile type
    {
        switch (type)
        {
            case 11: //SHOT
                RangedType = 1;
                projectileIcon.GetComponent<Image>().sprite = proj_shot;
                break;
            case 12: //BEAM
                RangedType = 2;
                projectileIcon.GetComponent<Image>().sprite = proj_beam;
                break;
        }
    }
    public void AddUpgrade(int type) //increments stacking upgrades
    {
        switch (type)
        {
            case 21: //HEALTH
                extraHP++;
                UpdateHealth();
                break;
            case 22: //ATTACK
                extraAttack++;
                break;
            case 23: //GREED
                extraGreed++;
                break;
            case 24: //LIFESTEAL
                lifesteal++;
                break;
            case 25: //ARMOR
                extraArmor++;
                playerArmor -= 0.05f;
                break;
            case 26: //DASH COOLDOWN
                extraDashCD++;
                dashCD *= 0.95f;
                break;

        }
    }

    void UpdateHealth() //changes hp when upgrading max hp
    {
        float healthRatio = playerCurrentHealth / playerMaxHealth; 
        playerMaxHealth += 20;
        playerCurrentHealth = playerMaxHealth * healthRatio;
        
    }

    //STATE CHANGES
    
    public void EnterShop() //entering a shop
    {
        inShop = true;
    }
    public void LoadLevel() //exiting shop, entering level
    {
        SceneManager.LoadScene("Level" + currentLevel);
        //SceneManager.LoadScene("WORK_Rennie");
        inShop = false;
    }
    public void StartGame() {
            SceneManager.LoadScene("WORK_Rennie");
      }

      // Return to MainMenu
      public void RestartGame() {
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu");
             // Reset all static variables here, for new games:
            playerCurrentHealth = playerMaxHealth;
      }

      // Replay the Level where you died
      public void ReplayLastLevel() {
            Time.timeScale = 1f;
            SceneManager.LoadScene(lastLevelDied);
             // Reset all static variables here, for new games:
            playerCurrentHealth = playerMaxHealth;
      }

      public void QuitGame() {
                #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
                #else
                Application.Quit();
                #endif
      }

      public void Credits() {
            SceneManager.LoadScene("Credits");
      } 

      //item info on hover
      

}
