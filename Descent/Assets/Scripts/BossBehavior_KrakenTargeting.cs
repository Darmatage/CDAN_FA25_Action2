using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class BossBehavior_KrakenTargeting : MonoBehaviour
{
    public GameObject player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void TargetPlayer()
    {

    }
}
