using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerProjectile : MonoBehaviour
{
    public GameHandler GameHandler;

    public float SelfDestructTime = 4.0f; //time until projectile disappears
    public float SelfDestructSFX = 0.5f;
    public GameObject projectileArt;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameHandler = GameObject.FindWithTag("GameHandler").GetComponent<GameHandler>();
        selfDestruct();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy")) //if bullet hits an enemy
        {
            other.gameObject.GetComponent<EnemyStatHandler>().EnemyDamage(GameHandler.ProjCalc(), false); //deal damage based on player's projectile damage
        }
        if (other.gameObject.tag != "Player") //if bullet hits anything but the player
        {
            gameObject.GetComponent<Collider>().enabled = false;
            //projectileArt.SetActive(false);
            StartCoroutine(selfDestructHit());
        }
    }

    IEnumerator selfDestructHit()
    {
        yield return new WaitForSeconds(SelfDestructSFX);
        Destroy(gameObject);
    }

    IEnumerator selfDestruct()
    {
        yield return new WaitForSeconds(SelfDestructTime);
        Destroy(gameObject);
    }
}
