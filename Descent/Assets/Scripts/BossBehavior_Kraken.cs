using UnityEngine;

public class BossBehavior_Kraken : MonoBehaviour
{
    //this script should be attached to the kraken itself. The tentacle should track the player's horizontal position, while remaining on the ground.
    //After locking onto the player, it should pause briefly, before attacking straight up.
    //While it seems easily avoided, the boss could shoot arcing projectiles that make staying above the boss dangerous (higher relative to boss = less warning on shots)

    [Header("Behavior")]
    public bool isAggro = false;
    public float aggroRange = 3f; //player detection range

    public bool isAttackWindup = false;
    public bool isAttackCooldown = false;
    public bool isAttackActive = false;

    public float windupTime = 30f;
    public float attackTime = 30f;
    public float cooldownTime = 30f;

    private bool executeAttack = false; //start the attack
    private bool isAttacking = false; //in the middle of the attack
    public float ApproachSpeed = 0.01f; //movespeed while attacking
    public float AttackRange = 0.5f; //distance when attack will be executed
    public GameObject Hitbox; //attack collider
    private float AttackTimer; //controls attack length

    [Header("Stats")]
    public static float enemyMaxHealth = 100f;
    public float enemyCurrentHealth = enemyMaxHealth;
    public static float enemyStrength = 5f;
    public float enemyArmor = .5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
