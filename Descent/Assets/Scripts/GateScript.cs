using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;

public class GateScript : MonoBehaviour
{
    GameHandler GameHandler;
	Transform player;
	public float MSG_range = 10; 
	
	[Header("GateKeyBinds")]
    public KeyCode summonKey = KeyCode.F;
    public KeyCode nextLevelKey = KeyCode.F;
    //public KeyCode shopKey = KeyCode.S;

	public string nextLevel="MainMenu";
	public bool gateActivated = false;
	public GameObject MSG_Interaction;
	public TMP_Text interactionText;
	public float detectHeight = 3f; //boosts height of range a bit so player isn't too close to the ground
	public GameObject LightColumn;

    [Header("BOSS SPAWN")]
    public GameObject bossPrefab;
	public Transform[] bossSpwnPnts;
	private int bossesToDefeat;
    //Boss spawn variables? Location?
    //If modifier possibly call on modifier, or would that be a bossPrefab thing?

    //Next level scene (string?) next shop scene
    //public float interactionRange = 2f;
    
	private bool gateTriggered = false;
    private bool inRange;
    private bool bossDefeated;

	public AudioSource gateOpenSFX;

	void Start()
    {
       bossesToDefeat = bossSpwnPnts.Length;
	   LightColumn.SetActive(false);
		
		MSG_Interaction.SetActive(false);;
		interactionText.text = ("Press F to Summon boss");


	   CheckGate();
       //Debug.Log("CheckGate Ran");
	   if (GameObject.FindWithTag("Player") != null){
        	player = GameObject.FindWithTag("Player").GetComponent<Transform>();
		}
    }

   void Update()
	{
		float playerDistance = Vector3.Distance(transform.position, new Vector3(player.position.x, (player.position.y - detectHeight), player.position.z));
		if (playerDistance <= MSG_range && !(gateTriggered && !gateActivated)) //if player is in range and boss is not alive
		{
			MSG_Interaction.SetActive(true);

            if (Input.GetKeyDown(summonKey))
			{
				if (!gateActivated) //if boss hasn't been spawned, spawn the boss
				{
					SpawnBosses();
					gateTriggered = true;
					gateOpenSFX.Play();
				}
				else //enter shop
				{
                    GameHandler.currentLevel++;
                    SceneManager.LoadScene(nextLevel);
                }
			}

        } 
		else
		{
            MSG_Interaction.SetActive(false);
        }


	}

  
	//add a timer for a color / lights pulse animation?
    void FixedUpdate()
	{
		
	}

//public function to access by boss enemies' death
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
			gateOpenSFX.Play();

            interactionText.text = ("Press F to exit level");
            Debug.Log("Gate has been activated, " + bossesToDefeat + " bosses found. Light column activated");
			
			if(SceneManager.GetActiveScene().name == "Level0")
            {
                Debug.Log("The Music Did Not Change");
            }
			else {GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>().StartAmbientMusic();}
		}
	} 
	
//go through gate if gate activated:
/*
	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag=="Player")
        {
			//Debug.Log("Gate hit player");
            //Debug.Log("Collision other with Player tag. Gate Activated = " + gateActivated);
			if (gateActivated)
			{
				Debug.Log("Gate hit player, and gatActivated = " + gateActivated);
                //Debug.Log("Loading scene!");
                GameHandler.currentLevel++;
                SceneManager.LoadScene(nextLevel);
			} 
		}
	}
*/
	void SpawnBosses()
	{
		if (SceneManager.GetActiveScene().name == "Level0")
        {
            Debug.Log("Music Did Not Change");
        }
		else {GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>().StartBossMusic();}
		
		
		
		for (int i=0; i<bossSpwnPnts.Length; i++){
			GameObject boss = Instantiate(bossPrefab, bossSpwnPnts[i].position, Quaternion.identity);
			boss.GetComponent<EnemyStatHandler>().isBoss = true;
            Debug.Log("Bosses spawned");
		}
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(new Vector3(transform.position.x, (transform.position.y + detectHeight), transform.position.z), MSG_range); //gizmo of interaction range
	}
}

//Sarah thoughts:
// if player range < interactionRange, then inRange = true;
        //Find distance between here and player, if it's within the interaction range to 0 then allows for interaction
        //Interaction pops up as small tooltip (canvas) saying "F to Summon" or something like that
        //Finds out if boss is dead (bosshealth?)
        //If boss dead then doesn't give F to summon tooltip, but gives "F to next, G to Shop" or something similar
    
