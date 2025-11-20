using UnityEngine;

public class EnemyStatHandler : MonoBehaviour
{

    GameHandler gameHandler;

    [Header("Stats")]
    public static float enemyMaxHealth = 10f;
    public float enemyCurrentHealth = enemyMaxHealth;
    public static float enemyStrength = 5f;
    public static float enemyArmor = .5f; //multiplier to damage taken? 
     public void EnemyDamage(float damageSource)
    {
        enemyCurrentHealth -= gameHandler.DamageCalc(damageSource, enemyArmor); //calculate + apply damage
    }
}
