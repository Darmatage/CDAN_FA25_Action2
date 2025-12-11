using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
//using UnityEditor.ShaderGraph;
using UnityEngine.UI;

public class ShopHandler : MonoBehaviour
{
    public GameHandler gameHandler;

    public TMP_Text description;
    public TMP_Text upgradeTypeText;
    public TMP_Text priceText;

    public int selectedButton = 0;
    public int upgradetype;
    public int upgradeprice;
    public GameObject button1;
    public GameObject button2;
    public GameObject button3;
    public AudioSource SFX_Buy;
    public AudioSource SFX_Fail;

	void Awake()
	{
		//GameObject.FindWithTag("GameHandler").GetComponent<GameHandler>();
	}

    private void Start()
    {
        ResetDescription();
        gameHandler.EnterShop();
    }

    void ResetDescription() //sets description to default
    {
        description.text = "";
        priceText.text = "Price: " + "XX";
        selectedButton = 0;
        upgradetype = 0;
        upgradeTypeText.text = "Item Type";
        upgradeTypeText.color = Color.white;
    }
    public void ChangeDescription(string desc, int type, int num, int price, int button) //change description and others
    {
        description.text = desc;
        priceText.text = "Price: " + price;
        upgradeprice = price;
        selectedButton = button;
        //Debug.Log("selectedButton: " + selectedButton);
        upgradetype = num;
        //UnityEngine.Debug.Log(type);
        switch (type)
        {
            case 1: //WEAPON
                upgradeTypeText.text = "Weapon";
                upgradeTypeText.color = new Color(0.6446381f, 0.7798742f, 0.2869347f);
                break;
            case 2: //PROJECTILE
                upgradeTypeText.text = "Projectile";
                upgradeTypeText.color = new Color(0.9371068f, 0.5957444f, 0.380147f);
                break;
            case 3: //UPGRADE
                upgradeTypeText.text = "Upgrade";
                upgradeTypeText.color = Color.white;
                break;
        }
    }
    public void Buy() //hit buy button
    {
        //purchase item
        if (selectedButton == 0 || upgradetype == 0)
        {
            Warning("You don't have anything selected...");
            SFX_Fail.Play();
        }
        else if (upgradeprice > GameHandler.gotCoins)
        {
            Warning("You can't afford this item!");
            SFX_Fail.Play();
        }
        else if(GameHandler.MeleeType == upgradetype) //check if purchased weapon is already equipped
        {
            Warning("You already have this weapon equipped!");
            SFX_Fail.Play();
        }
        else if (GameHandler.RangedType == upgradetype) //check if purchased weapon is already equipped
        {
            Warning("You already have this projectile equipped!");
            SFX_Fail.Play();
        }
        else
        {
            SuccessfulPurchase();
            SFX_Buy.Play();
        }
    }

    void SuccessfulPurchase()
    {
        if (upgradetype >= 21) //if upgrade is a passive
        {
            gameHandler.AddUpgrade(upgradetype); //call upgrade stack increase
        }
        else if (upgradetype >= 11) //if upgrade is a projectile
        {
            gameHandler.UpdateProjectile(upgradetype); //call weapon change
        }
        else //if upgrade is a weapon
        {
            gameHandler.UpdateWeapon(upgradetype); //call weapon change
        }

        //reset purchased item
            gameHandler.playerLoseCoins(upgradeprice); //subtract price
        	//Debug.Log("spent " + upgradeprice);
            //GameHandler.updateStatsDisplay(); //update stats
            
        //Debug.Log(selectedButton);
        //if (upgradeprice <= GameHandler.gotCoins)
        switch (selectedButton)
            {
                case 1: //button1
					//Debug.Log("am I called? Trying to switch a button off1");
                    button1.GetComponent<ShopButton>().RefreshUpgrade();
                    break;
                case 2: //button2
				    //Debug.Log("am I called? Trying to switch a button off2");
                    button2.GetComponent<ShopButton>().RefreshUpgrade();
                    break;
                case 3: //button3
				    //Debug.Log("am I called? Trying to switch a button off3");
                    button3.GetComponent<ShopButton>().RefreshUpgrade();
                    break;
            }
            ResetDescription(); //reset description
    }


    public void Warning(string message)
    {
        description.text = message;
        upgradeTypeText.text = "Warning";
        upgradeTypeText.color = Color.white;
    }

    public void RerollUpgradeCost()
    {
        if (GameHandler.gotCoins < 25) //check if player can afford to reroll
        {
            SFX_Fail.Play();
        }
        else
        {
            ResetDescription();
            gameHandler.playerLoseCoins(25); //subtract price
            SFX_Buy.Play();
        }

    }
}
