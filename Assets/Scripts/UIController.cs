using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TetrisManager tetrisManager;
    public GameObject EndGamePanel;


    public void UpdateScore()
    {
        // Update score text when the event is invoked
        scoreText.text = $"SCORE: {tetrisManager.score:n0}";
    }

    public void UpdateGameOver()
    {
        // Show end game panel if game is over when the event is invoked
        EndGamePanel.SetActive(tetrisManager.GameOver);
    }   

    public void PlayAgain()
    {
        tetrisManager.SetGameOver(false);
    }
}