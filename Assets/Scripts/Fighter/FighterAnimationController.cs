using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerStats{
    public string name {get; set;}
    public int health {get; set;}
    public int killCount {get; set;}
    public int flagsCaptured {get; set;}

    public PlayerStats(string name, int health, int killCount, int flagsCaptured){
        this.name = name;
        this.health = health;
        this.killCount = killCount;
        this.flagsCaptured = flagsCaptured;
    }
    
}


public class FighterAnimationController : MonoBehaviour
{

    PlayerStats[] playersArray =  new PlayerStats[3] 
    {new PlayerStats("Cem Koç", 40, 23, 3), new PlayerStats("Randy Orton", 60, 45, 1), new PlayerStats("John Cena", 50, 37, 2)};

    private delegate int TopScoreDelegate(PlayerStats stats);

    private void DisplayEndScreen(){
        string playerWithMostKills = GetPlayerWithMostScore(playersArray, stats => stats.killCount);
        string playerWithMostCaptures = GetPlayerWithMostScore(playersArray, stats => stats.flagsCaptured);
        Debug.Log("Player with most kills: " + playerWithMostKills);
        Debug.Log("Player with most captures: " + playerWithMostCaptures);
    }

    string GetPlayerWithMostScore(PlayerStats[] players, TopScoreDelegate scoreDelegate){
        int score = 0;
        string name = "";

        foreach (PlayerStats player in players)
        {
            if(scoreDelegate(player) > score){
                score = scoreDelegate(player);
                name = player.name;
            }
        }
        return name;
    }

    void Start(){
        DisplayEndScreen();
    }


}
