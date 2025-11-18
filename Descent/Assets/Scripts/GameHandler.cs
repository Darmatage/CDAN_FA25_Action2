using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 

public class GameHandler : MonoBehaviour
{
    public float levelTimer;
    public float maxTime = 180f;
	private string sceneName;
	public static string lastLevelDied;  //allows replaying the Level where you died

    [Header("Player Stats")]

	private GameObject player;
	public TMP_Text healthText;

    public static float playerCurrentHealth = 100f;
    public static float playerMaxHealth = 100f;
    public static float playerArmor = 0.9f; //direct multiplier to damage taken
    public static float IFrames = 30f; //frames of immunity after taking damage

    public static float meleeDamage = 10f; //damage of bite
    public static float projectileDamage = 5f; //damage of projectile

    void Start()
    {
        
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

    

    
    public float DamageCalc(float takenDamage, float Armor) //calculates damage taken based on attacker's attack and defender's armor
    {
        float totalDamage = takenDamage * Armor;

        //other things can go here, like crits or weaknesses

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
