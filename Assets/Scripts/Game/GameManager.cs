using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

[RequireComponent(typeof(ScoreManager))]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Bird")]
    public BirdGlideController bird;

    [Header("Score")]
    public TMP_Text scoreText;
    public MultiplierUI multiplierUI;
    public int currentScore = 0;
    public ScoreManager scoreManager;

    [Header("UI Panels")]
    public GameObject mainMenuUI;
    public GameObject inGameUI;
    public GameObject listeningUI;
    public GameObject gameOverPanel;

    [Header("Game Over Text")]
    public TMP_Text finalScoreText;
    public TMP_Text highScoresText;

    private bool isGameOver = false;
    private bool hasGameStarted = false;
    private const int MaxSavedScores = 5;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (scoreManager == null)
            scoreManager = GetComponent<ScoreManager>();

        if (scoreManager == null)
        {
            Debug.LogError("GameManager requires a ScoreManager component on the same GameObject.");
            enabled = false;
            return;
        }

        if (multiplierUI == null && scoreManager != null)
            multiplierUI = scoreManager.multiplierUI;

        if (multiplierUI == null)
            multiplierUI = FindFirstObjectByType<MultiplierUI>();
    }

    private void Start()
    {
        currentScore = 0;
        UpdateScoreText();
        ShowMainMenu();
    }

    public void ShowMainMenu()
    {
        hasGameStarted = false;
        isGameOver = false;
        currentScore = 0;
        UpdateScoreText();

        if (mainMenuUI != null)
            mainMenuUI.SetActive(true);

        if (inGameUI != null)
            inGameUI.SetActive(false);

        if (listeningUI != null)
            listeningUI.SetActive(false);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;
    }

    public void StartGame()
    {
        hasGameStarted = true;
        isGameOver = false;
        currentScore = 0;
        UpdateScoreText();

        if (mainMenuUI != null)
            mainMenuUI.SetActive(false);

        if (inGameUI != null)
            inGameUI.SetActive(true);

        if (listeningUI != null)
            listeningUI.SetActive(false);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Time.timeScale = 1f;

        if (bird != null)
            bird.BeginFlight();
    }

    public void AddScore(int amount)
    {
        if (!hasGameStarted || isGameOver) return;

        currentScore += amount;
        UpdateScoreText();
    }

    public void RegisterHit(Target target, Vector3 hitPosition)
    {
        if (!hasGameStarted || isGameOver) return;
        if (target == null) return;

        int awarded = scoreManager != null ? scoreManager.RegisterHit(target, hitPosition) : target.points;
        AddScore(awarded);
    }

    private void UpdateScoreText()
    {
        if (multiplierUI != null)
            multiplierUI.SetScore(currentScore);
        else if (scoreText != null)
            scoreText.text = "Score: " + currentScore;
    }

    public void EndGame()
    {
        if (!hasGameStarted || isGameOver) return;

        isGameOver = true;

        SaveScore(currentScore);
        ShowGameOverUI(currentScore);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;
    }

    private void ShowGameOverUI(int finalScore)
    {
        if (mainMenuUI != null)
            mainMenuUI.SetActive(false);

        if (inGameUI != null)
            inGameUI.SetActive(false);

        if (listeningUI != null)
            listeningUI.SetActive(false);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (finalScoreText != null)
            finalScoreText.text = "Final Score: " + finalScore;

        if (highScoresText != null)
        {
            List<int> scores = LoadScores();

            string display = "Top 5 Scores:\n";
            for (int i = 0; i < scores.Count; i++)
            {
                display += (i + 1) + ". " + scores[i] + "\n";
            }

            if (scores.Count == 0)
                display += "No scores yet.";

            highScoresText.text = display;
        }
    }

    private void SaveScore(int newScore)
    {
        List<int> scores = LoadScores();

        scores.Add(newScore);
        scores.Sort((a, b) => b.CompareTo(a));

        if (scores.Count > MaxSavedScores)
            scores.RemoveRange(MaxSavedScores, scores.Count - MaxSavedScores);

        for (int i = 0; i < MaxSavedScores; i++)
        {
            if (i < scores.Count)
                PlayerPrefs.SetInt("HighScore_" + i, scores[i]);
            else
                PlayerPrefs.DeleteKey("HighScore_" + i);
        }

        PlayerPrefs.Save();
    }

    private List<int> LoadScores()
    {
        List<int> scores = new List<int>();

        for (int i = 0; i < MaxSavedScores; i++)
        {
            string key = "HighScore_" + i;
            if (PlayerPrefs.HasKey(key))
                scores.Add(PlayerPrefs.GetInt(key));
        }

        return scores;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
