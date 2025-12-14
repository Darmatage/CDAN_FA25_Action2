using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class PauseMenuHandler : MonoBehaviour
{

    public static bool GameisPaused = false;
    public GameObject pauseMenuUI;
    public AudioMixer mixer;
	public AudioMixer mixer2;
    public static float volumeLevel = 0.5f;
	public static float volumeLevel2 = 0.5f;
    private Slider sliderVolumeCtrl;
	private Slider sliderVolumeCtrl2;
    public GameObject controlMenuUI;
    public TMP_Text statText;
    private GameHandler GameHandler;

    void Awake()
    {
        SetLevel(volumeLevel);
        GameObject sliderTemp = GameObject.FindWithTag("PauseMenuSlider");
        if (sliderTemp != null)
        {
            sliderVolumeCtrl = sliderTemp.GetComponent<Slider>();
            sliderVolumeCtrl.value = volumeLevel;
        }

		SetLevel(volumeLevel2);
        GameObject sliderTemp2 = GameObject.FindWithTag("PauseMenuSlider2");
        if (sliderTemp2 != null)
        {
            sliderVolumeCtrl2 = sliderTemp2.GetComponent<Slider>();
            sliderVolumeCtrl2.value = volumeLevel2;
        }
    }

    void Start()
    {
        pauseMenuUI.SetActive(false);
        controlMenuUI.SetActive(false);
        GameisPaused = false;
        GameHandler = GameObject.FindWithTag("GameHandler").GetComponent<GameHandler>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameisPaused&&controlMenuUI.activeSelf)//if viewing controls go back
            {
                controlMenuUI.SetActive(false);
                pauseMenuUI.SetActive(true);
            }
            else if (GameisPaused)
            { Resume(); }

            else { Pause(); }
        }
    }

    public void Pause()
    {
        if (!GameisPaused)
        {
            pauseMenuUI.SetActive(true);
            Time.timeScale = 0f;
            GameisPaused = true;

            Cursor.lockState = CursorLockMode.None; //Moved from Flight Controller due to conflict
            Cursor.visible = true; //Moved from Flight Controller
            
            //initialize damage stats
            GameHandler.MeleeCalc(false);
            GameHandler.ProjCalc();

            //display current stats
            statText.text = (GameHandler.meleeDamage * ((GameHandler.extraAttack * 0.1f) + 1) + "\n" //melee attack
                + GameHandler.projectileDamage * ((GameHandler.extraAttack * 0.1f) + 1) + "\n" //ranged attack
                + (1 - GameHandler.playerArmor) * 100 + "%\n" //armor as percentage
                + "x" + (GameHandler.extraGreed * 0.05f) + "\n" //greed as multiplier
                + (1 - (GameHandler.lifesteal * 0.05f)) * 100 + "%\n" //lifesteal as percentage
                + "x" + GameHandler.critDamage + "\n" //crit damage as multiplier
                + GameHandler.critRate * 100 + "%\n" //crit rate as percentage
                );


        }
        else
        { Resume(); }
        //NOTE: This function is for the pause button
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameisPaused = false;

        if(GameHandler.inMenu == false)
        {
            Cursor.lockState = CursorLockMode.Locked; //Moved from flight controller due to conflict
            Cursor.visible = false;
        }
            
    }

//music:
    public void SetLevel(float sliderValue)
    {
        mixer.SetFloat("MusicVolume", Mathf.Log10(sliderValue) * 20);
        volumeLevel = sliderValue;
    }

//SFX:
	 public void SetLevel2(float sliderValue)
    {
        mixer2.SetFloat("SFXVolume", Mathf.Log10(sliderValue) * 20);
        volumeLevel2 = sliderValue;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        GameisPaused = false;

        SceneManager.LoadScene("MainMenu");
        // Please also reset all static variables here, for new games!
        
        
    }

    public void ControlMenu()
    {
        controlMenuUI.SetActive(true);
        pauseMenuUI.SetActive(false);
    }

    public void BackToPauseMenu()
    {
        controlMenuUI.SetActive(false);
        pauseMenuUI.SetActive(true);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
    }
}