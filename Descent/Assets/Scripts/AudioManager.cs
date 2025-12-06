using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour {

        public AudioSource abyss_ambient;
        public AudioSource abyss_boss;
        public AudioSource caves_ambient;
        public AudioSource caves_boss;
        public AudioSource coral_ambient;
        public AudioSource coral_boss;
        public AudioSource river_ambient;
        public AudioSource main_menu;


        private AudioSource theMusic;
        private static float musicTimeStamp = 0.0f;
        public float currentTimeStamp;

        void Awake(){
                //set the music based on the scene
                if (SceneManager.GetActiveScene().name == "MainMenu") {theMusic = main_menu;} 
                else if (SceneManager.GetActiveScene().name == "Level 3") {theMusic = abyss_ambient;} 
                else if (SceneManager.GetActiveScene().name == "Level 3") {theMusic = abyss_boss;}
                else if (SceneManager.GetActiveScene().name == "Level 1") {theMusic = caves_ambient;}
                else if (SceneManager.GetActiveScene().name == "Level 1") {theMusic = caves_boss;}
                else if (SceneManager.GetActiveScene().name == "Level 2") {theMusic = coral_ambient;}
                else if (SceneManager.GetActiveScene().name == "Level 2") {theMusic = coral_boss;}
                else if (SceneManager.GetActiveScene().name == "River") {theMusic = river_ambient;}

                //set the time and play:
                theMusic.time = musicTimeStamp;
                theMusic.Play();
        }

        void Update(){
               //keep track of timestamp, to auto-call it in the next scene:
               musicTimeStamp = theMusic.time;
               currentTimeStamp = theMusic.time;
        }

//change timestamp (can be called by door code):
        public void SetTimeStamp(){
               musicTimeStamp = theMusic.time;
        }
}