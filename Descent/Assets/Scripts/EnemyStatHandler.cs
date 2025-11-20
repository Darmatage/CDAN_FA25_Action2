using UnityEngine;

public class EnemyStatHandler : MonoBehaviour
{

    public GameHandler gameHandler;

    [Header("Stats")]
    public static float enemyMaxHealth = 10f;
    public float enemyCurrentHealth = enemyMaxHealth;
    public static float enemyStrength = 5f;
    public static float enemyArmor = .5f; //multiplier to damage taken? 

    private void Update()
    {
        enemyDeath();
    }

    public void EnemyDamage(float damageSource)
    {
        Debug.Log(damageSource);
        Debug.Log(enemyArmor);
        enemyCurrentHealth -= gameHandler.DamageCalc(damageSource, enemyArmor); //calculate + apply damage
    }

    void enemyDeath()
    {
        if (enemyCurrentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
