using UnityEngine;
using TMPro;


public class EndScreenText : MonoBehaviour
{
    public TextMeshProUGUI explainText;
    public GameHandler gameHandlerObj;

    void OnEnable()
    {
        gameHandlerObj = GameObject.FindWithTag("GameHandler").GetComponent<GameHandler>();

        gameHandlerObj.EnterMenu();
        UpdateStats();
    }
    void UpdateStats()
    {
        explainText.text = $"You have reached the bottom!\n" + $"You get a cookie! Or two\n" + 
        $"Enemies Defeated: \"{GameHandler.enemiesKilled}\"\n" + $"BossesDefeated:\"{GameHandler.bossesDefeated}\"\n" +
        $"Total Coins Gained: \"{GameHandler.totalCoinsGained}\"";
    }
}
