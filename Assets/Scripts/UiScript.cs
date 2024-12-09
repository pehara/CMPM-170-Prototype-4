using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using System.Collections.Generic;

public class UiScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText; // Timer display text
    [SerializeField] private GameObject pauseMenu; // Reference to the Pause Menu UI
    [SerializeField] private TextMeshProUGUI highscore1Text; // Text for Highscore 1
    [SerializeField] private TextMeshProUGUI highscore2Text; // Text for Highscore 2
    [SerializeField] private TextMeshProUGUI highscore3Text; // Text for Highscore 3

    private float timerValue = 0;
    private bool isPaused = false;
    private List<float> topScores = new List<float>(); // Stores the top 3 times

    void Update()
    {
        if (!isPaused)
        {
            CountUpTimer();
        }
    }

    // Timer logic
    private void CountUpTimer()
    {
        timerValue += Time.deltaTime;

        if (timerText != null) // Null check for timerText
        {
            timerText.text = "Timer: " + Convert.ToInt32(timerValue);
        }
    }
    // Start the game by loading the puzzle scene
    public void StartGridGame()
    {
        // StartScene = 0
        // GridScene = 1
        // FreeformScene = 2
        SceneManager.LoadScene(1);
    }

    public void StartFreeformGame()
    {
        // StartScene = 0
        // GridScene = 1
        // FreeformScene = 2
        SceneManager.LoadScene(2);
    }
    
    // Pause the game
    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0; // Freezes the game
        if (pauseMenu != null) // Null check for pauseMenu
        {
            pauseMenu.SetActive(true);
        }
    }

    // Resume the game
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1; // Unfreezes the game
        if (pauseMenu != null) // Null check for pauseMenu
        {
            pauseMenu.SetActive(false);
        }
    }

    // Return to main menu
    public void ReturnToMainMenu()
    {
        Time.timeScale = 1; // Ensure the game isn't paused when returning
        SceneManager.LoadScene(0);
    }

    // Add a score when the player completes the puzzle
    public void AddScore(float score)
    {
        topScores.Add(score);
        topScores.Sort(); // Sort times in ascending order
        if (topScores.Count > 3)
        {
            topScores.RemoveAt(topScores.Count - 1); // Keep only the top 3 times
        }
        UpdateHighScoresUI();
    }

    // Update the high scores display
    private void UpdateHighScoresUI()
    {
        if (highscore1Text != null)
        {
            highscore1Text.text = topScores.Count > 0
                ? $"Highscore 1: {Mathf.Round(topScores[0])}s"
                : "Highscore 1: --";
        }

        if (highscore2Text != null)
        {
            highscore2Text.text = topScores.Count > 1
                ? $"Highscore 2: {Mathf.Round(topScores[1])}s"
                : "Highscore 2: --";
        }

        if (highscore3Text != null)
        {
            highscore3Text.text = topScores.Count > 2
                ? $"Highscore 3: {Mathf.Round(topScores[2])}s"
                : "Highscore 3: --";
        }
    }

    // Quit the game
    public void QuitGame()
    {
        Application.Quit();
    }
}
