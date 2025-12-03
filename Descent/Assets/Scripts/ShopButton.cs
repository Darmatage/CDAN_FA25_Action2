using UnityEngine;

public class ShopButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private int UpgradeType; //type of upgrade
    /*
     * 1 = weapon, 10%
     * 2 = projectile, 10%
     * 3 = passive,  80%
     */
    private int UpgradeNum;
    void Start()
    {
        //randomize type of upgrade
        UpgradeType = Random.Range(1, 10);

        if (UpgradeType >= 3)
        {
            UpgradeType = 3;
        }

        getUpgrade(UpgradeType);
    }

    void getUpgrade(int type)
    {
        //generate specific upgrade
        switch(type){
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
        }
    }
}
