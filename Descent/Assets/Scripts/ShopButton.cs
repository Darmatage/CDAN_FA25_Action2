using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour
{
    public ShopHandler ShopHandler;

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
    void Start()
    {
        getUpgradeNum();
        DisplayUpgrade(UpgradeNum);
    }



    void getUpgradeNum()
    {
        //randomize type of upgrade
        int UpgradeType = Random.Range(1, 10);

        if (UpgradeType >= 3)
        {
            UpgradeType = 3;
        }

        //generate specific upgrade
        switch (UpgradeType)
        {
            case 1: //WEAPONS
                UpgradeNum = Random.Range(1, 2);
                break;
            case 2: //PROJECTILES
                UpgradeNum = Random.Range(11, 12);
                break;
            case 3: //PASSIVES
                UpgradeNum = Random.Range(21, 24);
                break;
        }
    }

    void DisplayUpgrade(int num)
    {
        switch (num)
        {
            case 1: //BITE
                gameObject.GetComponent<Image>().sprite = weapon_bite;
                label.text = "Bite";
                desc = "A basic bite attack.\n";
                break;
            case 2: //CLAW
                gameObject.GetComponent<Image>().sprite = weapon_claw;
                label.text = "Claw";
                desc = "Claw slash, shorter range and higher attack speed and crit rate.\n";
                break;
            case 11: //SHOT
                gameObject.GetComponent<Image>().sprite = proj_shot;
                label.text = "Shot";
                desc = "Shoot a ball of steam from your mouth.\n";
                break;
            case 12: //BEAM
                gameObject.GetComponent<Image>().sprite = proj_beam;
                label.text = "Beam";
                desc = "Shoot a concentrated beam of heat.\n";
                break;
            case 21: //HEALTH
                gameObject.GetComponent<Image>().sprite = upgrade_health;
                label.text = "HP+";
                desc = "Increases max hp by 20.\n";
                break;
            case 22: //ATTACK
                gameObject.GetComponent<Image>().sprite = upgrade_attack;
                label.text = "Attack+";
                desc = "Boosts attack slightly.\n";
                break;
            case 23: //GREED
                gameObject.GetComponent<Image>().sprite = upgrade_greed;
                label.text = "Greed+";
                desc = "Boosts coins gained slightly.\n";
                break;
            case 24: //LIFESTEAL
                gameObject.GetComponent<Image>().sprite = upgrade_lifesteal;
                label.text = "Vampirism+";
                desc = "Heal a percentage of damage dealt per hit.\nAmount healed scales with stacks.";
                break;
        }
    }

    public void ShopButtonPress()
    {
        Debug.Log("press!");
        ShopHandler.ChangeDescription(desc);
    }
    
    public void RerollUpgrade()
    {
        getUpgradeNum();
        DisplayUpgrade(UpgradeNum);
    }
}
