using UnityEngine;
using TMPro;


public class LossScreenText : MonoBehaviour
{
    public TextMeshProUGUI explainText;

    void OnEnable()
    {
        UpdateStats();
    }
    void UpdateStats()
    {
        explainText.text = $"You Lost!\n" + $"Your body will rest on the seafloor\n" + 
        $"Enemies Defeated: \"{GameHandler.enemiesKilled}\"\n" + $"BossesDefeated:\"{GameHandler.bossesDefeated}\"\n" +
        $"Total Coins Gained: \"{GameHandler.totalCoinsGained}\"";
    }
}
