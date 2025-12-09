using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerShoot_Shot : MonoBehaviour
{
    //public GameObject cam;
    //public GameObject player;
    //public GameHandler GameHandler;
    // Update is called once per frame
    public Transform fireBase;
    public Transform firePoint;
    public GameObject projectilePrefab;
    public float projectileSpeed = 20f;
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
        Debug.Log("projectile instantiated!");
        Vector3 fwd = (firePoint.position - fireBase.position).normalized;
        //Spawn a bullet that inherits rotation from the instantiating object:
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, transform.rotation);
        projectile.GetComponent<Rigidbody>().AddForce(fwd * projectileSpeed, ForceMode.Impulse);
			SFX_Projectile.Play();
		}
    }
