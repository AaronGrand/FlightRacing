using UnityEngine;
using TMPro;

public class ScoreBoardPlayer : MonoBehaviour
{
    public TextMeshProUGUI time;
    public TextMeshProUGUI playerName;

    public void SetPlayer(Menu.PlayerStats playerStats)
    {
        if(playerStats.time == 0f)
        {
            time.text = "Crashed";
        } else
        {
            time.text = playerStats.time.ToString(".00");
        }

        playerName.text = playerStats.name.ToString();
    }
}
