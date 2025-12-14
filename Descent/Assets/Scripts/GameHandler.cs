using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 

public class GameHandler : MonoBehaviour
{
    [Header("Level Stats")]
    public static int currentLevel = 0; //current level
    public float levelTimer; //current time elapsed in level
    public float maxTime = 180f; //time limit for level
	private string sceneName;
    public static int enemiesKilled = 0; //current enemies killed
    public static int totalCoinsGained = 0;
    public static int bossesDefeated = 0;
    public int enemyGoal = 25; //enemies to kill for gate unlocking
	public static string lastLevelDied;  //allows replaying the Level where you died
    

    public static bool inMenu = false;

    [Header("Player Stats")]

	private GameObject player;
    //private Renderer playerRenderer;
	
    public static int gotCoins = 100;
    
    //health and armor
    public static float playerCurrentHealth = 100f;
    public static float playerMaxHealth = 100f;
    public static float playerMaxHealthBase = 100f;
    public static float playerArmor = 1f; //direct multiplier to damage taken
    public static float playerArmorBase = 1f; //default armor value
    public static float IFrames = 30f; //frames of immunity after taking damage
    public bool isImmune = false;
    public float immuneTimer = 0f;

    //attacks
    public static float meleeDamage; //damage of bite
    public static float meleeCD = 1f; //cooldown of bite
    public static float projectileDamage; //damage of projectile
    public static float projectileCD = 1f; //cooldown of projectile
    public static float critRate = 0.2f; //critical rate, 1 = 100%, 0.5 = 50% etc
    public static float critDamage = 1.5f; //critical damage multiplier
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

    public AudioSource SFX_Damage;

    [Header("Mutation handler")]

    public static int MeleeType = 0; //tracks what "main weapon" currently is
    /*
     * 0 = starting value
     * 1 = bite, default
     * 2 = claws, bleed effect, faster attack speed, shorter range
     * 3 = tail whip, huge range that scales off max hp, slow speed?
     */
    public static int RangedType = 10; //tracks current ranged weapon
    /*
     * 10 = starting value
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
    public static int extraCritDMG = 0; //stacks of extra crit DMG

    private void Awake()
    {
        if (SceneManager.GetActiveScene().name != "Shop")
        {
            sceneName = SceneManager.GetActiveScene().name;
        }

        //make sure currentlevel is accurate
        switch (sceneName)
        {
            case "Level0": currentLevel = 0; break;
            case "Level1": currentLevel = 1; break;
            case "Level2": currentLevel = 2; break;
            case "Level3": currentLevel = 3; break;
        }

        Debug.Log("current level: " + currentLevel);
    }

    void Start()
    {
        if (SceneManager.GetActiveScene().name != "EndLose" && SceneManager.GetActiveScene().name != "Endwin"&& SceneManager.GetActiveScene().name != "Credits"){
          updateStatsDisplay();  
        }
        

        //make sure weapons are loaded properly
        if (MeleeType == 0)
        {
            UpdateWeapon(1);
        }
        else
        {
            UpdateWeapon(MeleeType);
        }

        if (RangedType == 10)
        {
            UpdateProjectile(11);
        }
        else
        {
            UpdateProjectile(RangedType);
        }

    }

    private void Update()
    {
            immuneTimer++;
    }
    void FixedUpdate()
    {
        levelTimer = Time.timeSinceLevelLoad; //seconds since scene load

        if ((playerCurrentHealth <= 0 && !inMenu) || Input.GetKeyDown(KeyCode.V)) //like 3 minutes or death
        {
            SceneManager.LoadScene("EndLose");
            EnterMenu();
            //game over!!
            //probably play an animation before this in the final game
        }

        
    }

    

    //COIN COUNTING
    public void playerGetCoins(int newCoins){ //gettin coins
            gotCoins += Mathf.RoundToInt(newCoins * (1 + (extraGreed * 0.05f))); //multiply gained coins by 5% per greed stack
            updateStatsDisplay();
            totalCoinsGained = totalCoinsGained + Mathf.RoundToInt(newCoins * (1 + (extraGreed * 0.05f)));
    }

    public void playerLoseCoins(int newCoins) //losin coins
    {
        gotCoins -= newCoins;
        updateStatsDisplay();
    }

    public void updateStatsDisplay(){ //update totals
        if (!inMenu)
        {
            healthText.text = playerCurrentHealth + "/" + playerMaxHealth;
            coinsText.text = gotCoins.ToString();
        }
    }
    
    //DAMAGE CALCULATION
    public void playerGetHit(int damage){ //player gets hit

        if (immuneTimer <= IFrames)
        {
            isImmune = true;
            //playerRenderer.material.color.a = 0.5f;
            
        }
        else
        {
            isImmune = false;
        }

        //Debug.Log("immunity: " + isImmune);

        if (!isImmune)
        {
            playerCurrentHealth -= (damage * playerArmor);

                  if (playerCurrentHealth >=0){
                        updateStatsDisplay();
                  }
                  if (damage > 0){{SFX_Damage.Play();}
                        //play GetHit animation:
                        //player.GetComponent<PlayerHurt>().playerHit();
                  }
            immuneTimer = 0; //reset immunity timer
            
         }

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

    public void playerOutOfBounds(int damage)
    {
        playerGetHit(damage); //Damage taken when out of bounds
    }

    

    public float MeleeCalc(bool isCrit) //calculates player's melee damage
    {
        float totalDamage = meleeDamage * //base damage 
            ((extraAttack * 0.2f) + 1); //attack passive modifier

        if (isCrit)
        {
            totalDamage *= critDamage * (1 + (extraCritDMG * 0.05f)); //critical hits increase damage by 50%. effects can also be added here :3
        }
         //Debug.Log("Sent Damage: " + totalDamage);

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
                projectileDamage = 10f;
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
                meleeDamage = 20f;
                meleeCD = 2f;
                critRate = 0.1f;
                attackRadius = 1f;
                weaponIcon.GetComponent<Image>().sprite = weapon_bite;
                break;
            case 2: //CLAW
                MeleeType = 2;
                meleeDamage = 10f;
                meleeCD = 3f;
                critRate = 0.6f;
                attackRadius = 0.8f;
                weaponIcon.GetComponent<Image>().sprite = weapon_claw;
                break;
            
        }
    }

    public void UpdateProjectile(int type) //changes current projectile type
    {
        switch (type)
        {
            case 11: //SHOT
                RangedType = 11;
                projectileIcon.GetComponent<Image>().sprite = proj_shot;
                break;
            case 12: //BEAM
                RangedType = 12;
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
            case 27: //CRIT DAMAGE
                extraCritDMG++;
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
    
    public void EnterMenu() //entering a shop
    {
		//Debug.Log("In a shop am I");
        inMenu = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void LoadLevel() //exiting shop, entering level
    {
        playerCurrentHealth += 20;
        SceneManager.LoadScene("Level" + (currentLevel + 1));
        //SceneManager.LoadScene("WORK_Rennie");
        inMenu = false;
    }
    public void StartGame() {
            SceneManager.LoadScene("Level0");
        inMenu = false;
      }

      // Return to MainMenu
      public void RestartGame() {
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu");
            ResetStats(); // Reset all static variables here, for new games:
            playerCurrentHealth = playerMaxHealth;
            EnterMenu();
    }

      // Replay the Level where you died
      public void ReplayLastLevel() {
            Time.timeScale = 1f;
            SceneManager.LoadScene(lastLevelDied);
            ResetStats(); // Reset all static variables here, for new games:
            playerCurrentHealth = playerMaxHealth;
        inMenu = false;
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

      public void ResetStats()
      {
        currentLevel = 0; //current level
    //float levelTimer = 0; //current time elapsed in level
    enemiesKilled = 0; //current enemies killed
    totalCoinsGained = 0;
    bossesDefeated = 0;
    inMenu = false;
    gotCoins = 100;
    playerCurrentHealth = 100f;
    playerMaxHealth = 100f;
    playerMaxHealthBase = 100f;
    playerArmor = 1f; //direct multiplier to damage taken
    playerArmorBase = 1f; //default armor value
    IFrames = 30f; //frames of immunity after taking damage
    isImmune = false;
    immuneTimer = 0f;
    meleeDamage = 30f; //damage of bite
    meleeCD = 1f; //cooldown of bite
    projectileDamage = 20f; //damage of projectile
    projectileCD = 1f; //cooldown of projectile
    critRate = 0.2f; //critical rate, 1 = 100%, 0.5 = 50% etc
    critDamage = 1.5f; //critical damage multiplier
    attackRadius = 1f;
    dashCD = 3f; //cooldown between dashes
    speedMultiplier = 1f; //multiplier to speed
      }

      //item info on hover
      

}
