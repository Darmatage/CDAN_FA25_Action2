using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerShoot_Beam : MonoBehaviour
{
    //public GameObject cam;
    //public GameObject player;
    //public GameHandler GameHandler;
    // Update is called once per frame
    public Transform fireBase;
    public Transform firePoint;
    //public GameObject projectilePrefab;
    //public float attackRate = 2f;
    //private float nextAttackTime = 0f;
    public AudioSource SFX_Projectile;

    private void Start()
    {
        //GameHandler = GameObject.FindWithTag("GameHandler").GetComponent<GameHandler>();
    }
    void LateUpdate()
    {
        //transform.rotation = cam.transform.rotation; //aim matches camera rotation
        //transform.position = player.transform.position; //position matches player
    }

    public void playerFireShot()
    {

    }
}
