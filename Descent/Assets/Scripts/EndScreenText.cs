using UnityEngine;
using TMPro;


public class EndScreenText : MonoBehaviour
{
    public TextMeshProUGUI explainText;

    void OnEnable()
    {
        UpdateStats();
    }
    void UpdateStats()
    {
        explainText.text = $"You have reached the bottom!\n" + $"You get a cookie! Or two\n" + 
        $"Enemies Defeated: \"{GameHandler.enemiesKilled}\"\n" + $"BossesDefeated:\"{GameHandler.bossesDefeated}\"\n" +
        $"Total Coins Gained: \"{GameHandler.totalCoinsGained}\"";
    }
}
