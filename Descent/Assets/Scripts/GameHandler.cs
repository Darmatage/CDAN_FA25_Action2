using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 

public class GameHandler : MonoBehaviour
{
    [Header("Level Stats")]
    public float levelTimer;
    public float maxTime = 180f;
	private string sceneName;
    public static int enemiesKilled = 0; //current enemies killed
    public int enemyGoal = 25; //enemies to kill for gate unlocking
	public static string lastLevelDied;  //allows replaying the Level where you died

    [Header("Player Stats")]

	private GameObject player;
	public TMP_Text healthText;
    public static int gotCoins = 0;
    public TMP_Text coinsText;

    public static float playerCurrentHealth = 100f;
    public static float playerMaxHealth = 100f;
    public static float playerArmor = 0.9f; //direct multiplier to damage taken
    public static float IFrames = 30f; //frames of immunity after taking damage

    public static float meleeDamage = 10f; //damage of bite
    public static float projectileDamage = 5f; //damage of projectile
    public static float critRate = 0.2f; //critical rate, 1 = 100%, 0.5 = 50% etc

    [Header("Mutation handler")]

    public static int MeleeType = 0; //tracks what "main weapon" currently is
    /*
     * 0 = bite, default
     * 1 = claws, bleed effect, faster attack speed, shorter range
     * 2 = tail whip, huge range that scales off max hp, slow speed
     */
    public static int RangedType = 0; //tracks current ranged weapon
    /*
     * 0 = steam shot, default
     * idkkk
     */
    public static int extraHP; //stacks of extra hp
    public static bool Vampirism = false;


    void Start()
    {
        updateStatsDisplay();
    }

    void FixedUpdate()
    {
        levelTimer = Time.timeSinceLevelLoad; //seconds since scene load

        if (levelTimer > maxTime || playerCurrentHealth <= 0) //like 3 minutes or death
        {
            SceneManager.LoadScene("EndLose");
            //game over!!
            //probably play an animation before this in the final game
        }

        
    }

    private void Update()
    {
        
    }

    public void playerGetCoins(int newCoins){
            gotCoins += newCoins;
            updateStatsDisplay();
    }

    public void updateStatsDisplay(){
            healthText.text = "HEALTH: " + playerCurrentHealth;
            coinsText.text = "COINS: " + gotCoins;
    }


    public void playerGetHit(int damage){
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






//FOR PLAYER AND ENEMIES: calculates damage taken based on attacker's attack and defender's armor
    

    public float MeleeCalc()
    {
        

        switch (MeleeType)
        {
            case 0: //bite
                meleeDamage = 10f;
                critRate = 0.1f;
                break;
            case 1: //claws
                meleeDamage = 5f;
                critRate = 0.3f;
                break;
        }

        float totalDamage = meleeDamage;

        if (critRate > Random.value)
        {
            totalDamage *= 1.5f; //critical hits increase damage by 50%. effects can also be added here :3
        }
         Debug.Log("Sent Damage: " + totalDamage);
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
      public void StartGame() {
            SceneManager.LoadScene("Level1");
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



}
