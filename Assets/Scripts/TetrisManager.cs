using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class TetrisManager : MonoBehaviour
{
    public int score { get; private set; }

    public UnityEvent OnScoreChanged;
    public UnityEvent OnGameOver;

    // Game over bool for end game state (exposed for UI)
    public bool GameOver { get; private set; }

    void Start()
    {
        SetGameOver(false);
    }

    public int CalculateScore(int clearedRows)
    {
        switch (clearedRows)
        {
            case 1: return 100;
            case 2: return 300;
            case 3: return 500;
            case 4: return 800;
            default: return 0;
        }
    }

    public void ChangeScore(int amount)
    {
        score += amount;
        OnScoreChanged?.Invoke();
    }

    // Set game over state. If isGameOver == true -> invoke event and show UI.
    // If isGameOver == false -> treat as "play again" and restart the scene.
    public void SetGameOver(bool isGameOver)
    {
        GameOver = isGameOver;

        if (!GameOver)
        {
            score = 0;
            ChangeScore(0);
           
        }
        OnGameOver?.Invoke();
        Debug.Log("Game Over");
        return;

        // PlayAgain: reload the active scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
