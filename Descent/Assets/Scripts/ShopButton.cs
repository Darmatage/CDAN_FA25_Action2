using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour
{
    public GameHandler GameHandler;
    public ShopHandler ShopHandler;
    public int buttonNum;

    private int UpgradeType; //type of upgrade
    /*
     * 1 = weapon, 10%
     * 2 = projectile, 10%
     * 3 = passive,  80%
     */
    private int UpgradeNum; //specific upgrade in type
    
    public TMP_Text label;
    public string desc;
    public int price;

    public Sprite weapon_claw;
    public Sprite weapon_bite;

    public Sprite proj_shot;
    public Sprite proj_beam;

    public Sprite upgrade_health;
    public Sprite upgrade_attack;
    public Sprite upgrade_greed;
    public Sprite upgrade_lifesteal;
    public Sprite upgrade_armor;
    void Start()
    {
        getUpgradeNum();
        DisplayUpgrade(UpgradeNum);
    }



    void getUpgradeNum()
    {
        //randomize type of upgrade
        UpgradeType = Random.Range(1, 10);

        if (UpgradeType >= 3)
        {
            UpgradeType = 3;
        }

        //generate specific upgrade
        switch (UpgradeType)
        {
            case 1: //WEAPONS
                UpgradeNum = Random.Range(1, 3);
                break;
            case 2: //PROJECTILES
                UpgradeNum = Random.Range(11, 13);
                break;
            case 3: //PASSIVES
                UpgradeNum = Random.Range(21, 26);
                break;
        }
    }

    void DisplayUpgrade(int num) //Change icon and labels
    {
        switch (num)
        {
            //WEAPONS
            case 1: //BITE
                gameObject.GetComponent<Image>().sprite = weapon_bite;
                label.text = "Bite";
                price = 50;
                desc = "A basic bite attack.\n";
                break;
            case 2: //CLAW
                gameObject.GetComponent<Image>().sprite = weapon_claw;
                label.text = "Claw";
                price = 50;
                desc = "Claw slash, shorter range and higher attack speed and crit rate.\n";
                break;

            //PROJECTILES
            case 11: //SHOT
                gameObject.GetComponent<Image>().sprite = proj_shot;
                label.text = "Shot";
                price = 50;
                desc = "Shoot a ball of steam from your mouth.\n";
                break;
            case 12: //BEAM
                gameObject.GetComponent<Image>().sprite = proj_beam;
                label.text = "Beam";
                price = 50;
                desc = "Shoot a concentrated beam of heat.\n";
                break;

            //UPGRADES
            case 21: //HEALTH
                gameObject.GetComponent<Image>().sprite = upgrade_health;
                label.text = "HP+";
                price = 30;
                desc = "Increases max hp by 20.\n";
                break;
            case 22: //ATTACK
                gameObject.GetComponent<Image>().sprite = upgrade_attack;
                label.text = "Attack+";
                price = 30;
                desc = "Boosts attack slightly.\n";
                break;
            case 23: //GREED
                gameObject.GetComponent<Image>().sprite = upgrade_greed;
                label.text = "Greed+";
                price = 30;
                desc = "Boosts coins gained slightly.\n";
                break;
            case 24: //LIFESTEAL
                gameObject.GetComponent<Image>().sprite = upgrade_lifesteal;
                label.text = "Vampirism+";
                price = 30;
                desc = "Heal a percentage of damage dealt per hit.\nAmount healed scales with stacks.";
                break;
            case 25: //ARMOR
                gameObject.GetComponent<Image>().sprite = upgrade_armor;
                label.text = "Armor+";
                price = 30;
                desc = "Increase damage resistance by 5% per stack.";
                break;
        }
    }

    public void ShopButtonPress()
    {
        //Debug.Log("press!");
        ShopHandler.ChangeDescription(desc, UpgradeType, UpgradeNum, price, buttonNum);
    }
    
    public void RerollUpgrade()
    {
        if(GameHandler.gotCoins < 25) //check if player can afford to reroll
        {
            ShopHandler.Warning("You can't afford this item!");
        }
        else
        {
            getUpgradeNum();
            DisplayUpgrade(UpgradeNum);
            //GameHandler.playerLoseCoins(25); //subtract price
        }
            
    }
}
