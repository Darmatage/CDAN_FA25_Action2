using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.UI;

public class ShopHandler : MonoBehaviour
{
    public GameHandler GameHandler;

    public TMP_Text description;
    public TMP_Text upgradeTypeText;
    public TMP_Text priceText;

    public int selectedButton = 0;
    public int upgradetype;
    public int upgradeprice;
    public GameObject button1;
    public GameObject button2;
    public GameObject button3;

    private void Start()
    {
        ResetDescription();
        GameHandler.EnterShop();
    }
    void ResetDescription() //sets description to default
    {
        description.text = "";
        priceText.text = "Price: " + "XX";
        selectedButton = 0;
        upgradetype = 0;
        upgradeTypeText.text = "Upgrade Type";
        upgradeTypeText.color = Color.white;
    }
    public void ChangeDescription(string desc, int type, int num, int price, int button) //change description and others
    {
        description.text = desc;
        priceText.text = "Price: " + price;
        upgradeprice = price;
        selectedButton = button;
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
        }
        else if (upgradeprice > GameHandler.gotCoins)
        {
            Warning("You can't afford this item!");
        }
        else if(GameHandler.MeleeType == upgradetype) //check if purchased weapon is already equipped
        {
            Warning("You already have this weapon equipped!");
        }
        else if (GameHandler.RangedType == upgradetype) //check if purchased weapon is already equipped
        {
            Warning("You already have this projectile equipped!");
        }
        else
        {
            SuccessfulPurchase();
        }
    }

    void SuccessfulPurchase()
    {
        if (upgradetype >= 21) //if upgrade is a passive
        {
            GameHandler.AddUpgrade(upgradetype); //call upgrade stack increase
        }
        else if (upgradetype >= 11) //if upgrade is a projectile
        {
            GameHandler.UpdateProjectile(upgradetype); //call weapon change
        }
        else //if upgrade is a weapon
        {
            GameHandler.UpdateWeapon(upgradetype); //call weapon change
        }

        //reset purchased item
            GameHandler.playerLoseCoins(upgradeprice); //subtract price
        UnityEngine.Debug.Log("spent " + upgradeprice);
            //GameHandler.updateStatsDisplay(); //update stats
            ResetDescription(); //reset description

        if (upgradeprice <= GameHandler.gotCoins)
            switch (selectedButton)
            {
                case 1: //button1
                    button1.GetComponent<ShopButton>().ResetButton();
                    break;
                case 2: //button2
                    button2.GetComponent<ShopButton>().ResetButton();
                    break;
                case 3: //button3
                    button3.GetComponent<ShopButton>().ResetButton();
                    break;
            }
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
            
        }
        else
        {
            ResetDescription();
            GameHandler.playerLoseCoins(25); //subtract price
        }

    }
}
