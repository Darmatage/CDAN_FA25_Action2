using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class ShopHandler : MonoBehaviour
{
    public GameHandler GameHandler;

    public TMP_Text description;
    //private int UpgradeType;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void ChangeDescription(string text)
    {
        description.text = text;
    }

    public void Buy()
    {

    }

}
