using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager Instance = null;
    private void Awake()
    {
        Instance = this;
    }
    #endregion

    public int score = 0; // ScoreKeeping

    public void AddScore(int scoreToAdd)
    {
        // Increase Score Value by incoming score
        score += scoreToAdd;
        // Update Score UI
        UIManager.Instance.UpdateScore(score);
    }

    // Reloads the Current Level
    public void Restart()
    {
        // Load the Active Scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel()
    {
        // Loads the next level
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void PrevLevel()
    {
        // Loads the next level
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
