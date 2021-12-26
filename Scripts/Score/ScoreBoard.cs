using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MEC;

public class ScoreBoard : MonoBehaviour
{
    [Header("Score Text:")]
    public TextMeshProUGUI score;

    int curScore = 0;   //When it reaches 3 => end game => who won? :O

    [Header("Team:")]
    public Team team;

    public void Point()
    {
        curScore++;
        score.text = $"{curScore}/3";

        //You won:
        //Destroy the basket, instnatiate end game confetti
        if (curScore == 3)
        {
            //Instantiate(cubeFetti);
            GameController.instance.EndGame();
            Debug.Log($"{team} won! :D");
        }
    }
}
