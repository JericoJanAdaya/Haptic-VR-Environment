using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GlobalScoreManager : MonoBehaviour
{
    public static GlobalScoreManager Instance { get; private set; }
    public int score = 0; // Universal score variable
    public TextMeshProUGUI scoreText; // Reference to a universal score TextMesh
    public int scoreFlag = 0;

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

    public void UpdateScore(int amount)
    {
        score += amount;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        if (scoreFlag < score)
        {
            scoreText.text = "Score Increased!!\nScore: " + score.ToString();
            scoreFlag = score;
        }
        else
        {
            scoreText.text = "No score added...\nScore: " + score.ToString();
        }
    }
}
