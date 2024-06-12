using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class GlobalScoreManager : MonoBehaviour
{
    public static GlobalScoreManager Instance { get; private set; }
    public int score = 0; // Universal score variable
    public TextMeshProUGUI scoreText; // Reference to a universal score TextMesh
    public TextMeshProUGUI timerText; // Reference to the timer TextMesh
    public GameObject scoreboardPanel; // Reference to the scoreboard panel
    public TextMeshProUGUI highScoresText; // Reference to the high scores TextMesh

    public int scoreFlag = 0;
    public float gameTime = 60f; // Example game time in seconds
    private bool gameActive = false;

    private List<int> highScores = new List<int>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Makes the score manager persist across scenes
        }
        else
        {
            Destroy(gameObject); // Ensures only one instance exists
        }
    }

    private void Update()
    {
        if (gameActive)
        {
            gameTime -= Time.deltaTime;
            timerText.text = "Time: " + Mathf.Ceil(gameTime).ToString();
            if (gameTime <= 0)
            {
                EndGame();
            }
        }
    }

    public void StartGame()
    {
        if (!gameActive) // Only start the game if it is not already active
        {
            gameActive = true;
            gameTime = 60f; // Reset game time
            score = 0; // Reset score
            UpdateScoreText();
            timerText.gameObject.SetActive(true);
            scoreboardPanel.SetActive(false);
        }
    }

    public void UpdateScore(int amount)
    {
        score += amount;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        scoreText.text = "Score: " + score.ToString();
    }

    private void EndGame()
    {
        gameActive = false;
        timerText.gameObject.SetActive(false);
        UpdateHighScores();
        DisplayHighScores();
        scoreboardPanel.SetActive(true);
    }

    private void UpdateHighScores()
    {
        highScores.Add(score);
        highScores.Sort((a, b) => b.CompareTo(a)); // Sort high scores in descending order
        if (highScores.Count > 5)
        {
            highScores.RemoveAt(highScores.Count - 1); // Keep only top 5 scores
        }
    }

    private void DisplayHighScores()
    {
        highScoresText.text = "\nHigh Scores:\n";
        foreach (int highScore in highScores)
        {
            highScoresText.text += highScore.ToString() + "\n";
        }
    }
}