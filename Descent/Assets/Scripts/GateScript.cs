using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement; 

public class GateScript : MonoBehaviour
{
    [Header("GateKeyBinds")]
    //public KeyCode summonKey = KeyCode.F;
    //public KeyCode nextLevelKey = KeyCode.F;
    //public KeyCode shopKey = KeyCode.S;

	public string nextLevel="MainMenu";
	public bool gateActivated = false;
	public GameObject MSG_NotActivated;
	public GameObject LightColumn;

    [Header("BOSS SPAWN")]
    public GameObject bossPrefab;
	public Transform[] bossSpwnPnts;
	private int bossesToDefeat;
    //Boss spawn variables? Location?
    //If modifier possibly call on modifier, or would that be a bossPrefab thing?

    //Next level scene (string?) next shop scene
    //public float interactionRange = 2f;
    
	private bool gateTriggered1 = false;
    private bool inRange;
    private bool bossDefeated;

	void Start()
    {
       bossesToDefeat = bossSpwnPnts.Length;
	   LightColumn.SetActive(false);
	   MSG_NotActivated.SetActive(false);
       Debug.Log("LightColumn set false, bosses = bosslength, msg set false.");
	   CheckGate();
       Debug.Log("CheckGate Ran");
    }

  
	//add a timer for a color / lights pulse animation?
    void FixedUpdate()
	{
		
	}

//publ function to ccess by bos enemies' death
  	public void defeatABoss()
	{
		bossesToDefeat--;
		CheckGate();
	}

//determine if gate should open
	void CheckGate()
	{
		if (bossesToDefeat <= 0)
		{
			gateActivated=true;
			LightColumn.SetActive(true);
            Debug.Log("Gate has been activated, 0 bosses found. Light column activated");
		}
	} 
	

//go through gate if gate activated:
	void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.tag=="Player")
        {
            Debug.Log("Collision other with Player tag");
			if (gateActivated)
			{
				SceneManager.LoadScene(nextLevel);
			} 
		}
	}

//spawn bosses when first touching gate range
	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag=="Player")
        {
            Debug.Log("Collider other with player tag");
			if (!gateTriggered1)
			{
				SpawnBosses();
                gateTriggered1 = true; //Bosses have been spawned
			}
			
			if (!gateActivated)
			{
				MSG_NotActivated.SetActive(true);
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.gameObject.tag=="Player")
        {
            Debug.Log("Player exit trigger");
			MSG_NotActivated.SetActive(false);
		}
	}


	void SpawnBosses()
	{
		for (int i=0; i<bossSpwnPnts.Length; i++){
			Instantiate(bossPrefab, bossSpwnPnts[i].position, Quaternion.identity);
            Debug.Log("Bosses spawned");
		}
    
	}

}


//Sarah thoughts:
// if player range < interactionRange, then inRange = true;
        //Find distance between here and player, if it's within the interaction range to 0 then allows for interaction
        //Interaction pops up as small tooltip (canvas) saying "F to Summon" or something like that
        //Finds out if boss is dead (bosshealth?)
        //If boss dead then doesn't give F to summon tooltip, but gives "F to next, G to Shop" or something similar
    
