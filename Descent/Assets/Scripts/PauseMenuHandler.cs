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
    public static float volumeLevel = 1.0f;
    private Slider sliderVolumeCtrl;
    public GameObject controlMenuUI;

    void Awake()
    {
        SetLevel(volumeLevel);
        GameObject sliderTemp = GameObject.FindWithTag("PauseMenuSlider");
        if (sliderTemp != null)
        {
            sliderVolumeCtrl = sliderTemp.GetComponent<Slider>();
            sliderVolumeCtrl.value = volumeLevel;
        }
    }

    void Start()
    {
        pauseMenuUI.SetActive(false);
        controlMenuUI.SetActive(false);
        GameisPaused = false;
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

        Cursor.lockState = CursorLockMode.Locked; //Moved from flight controller due to conflict
        Cursor.visible = false;
    }

    public void SetLevel(float sliderValue)
    {
        mixer.SetFloat("MusicVolume", Mathf.Log10(sliderValue) * 20);
        volumeLevel = sliderValue;
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