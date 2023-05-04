using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreBoardPlayer : MonoBehaviour
{
    public TextMeshProUGUI time;
    public TextMeshProUGUI playerName;

    public void SetPlayer(Menu.PlayerStats playerStats)
    {
        time.text = playerStats.time.ToString(".00");
        playerName.text = playerStats.name.ToString();
    }
}
