using UnityEngine;

public class GateScript : MonoBehaviour
{
    [Header("GateKeyBinds")]
    public KeyCode summonKey = KeyCode.F;
    public KeyCode nextLevelKey = KeyCode.F;
    public KeyCode shopKey = KeyCode.S;

    [Header("Other")]
    public GameObject bossPrefab;
    //Boss spawn variables? Location?
    //If modifier possibly call on modifier, or would that be a bossPrefab thing?

    //Next level scene (string?) next shop scene
    public float interactionRange = 2f;
    
    private bool inRange;
    private bool bossDefeated;

    
    void Start()
    {
        //Where player is?
    }

    // Update is called once per frame
    void Update()
    {
        // if player range < interactionRange, then inRange = true;
        //Find distance between here and player, if it's within the interaction range to 0 then allows for interaction
        //Interaction pops up as small tooltip (canvas) saying "F to Summon" or something like that
        //Finds out if boss is dead (bosshealth?)
        //If boss dead then doesn't give F to summon tooltip, but gives "F to next, G to Shop" or something similar
    }
}
