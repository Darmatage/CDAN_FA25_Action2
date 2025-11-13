using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class AttackHandler : MonoBehaviour
{
    public GameObject Hurtbox;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float DamageCalc(float takenDamage, float Armor) //calculates damage taken based on attacker's attack and defender's armor
    {
        float totalDamage = takenDamage * Armor;

        return totalDamage;
    }
}
