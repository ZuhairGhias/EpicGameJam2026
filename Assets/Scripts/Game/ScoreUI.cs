using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    [Header("Required")]
    public TMP_Text scoreText;

    void Awake()
    {
        if (scoreText == null)
        {
            Debug.LogError("ScoreUI: scoreText reference not set.");
            enabled = false;
            return;
        }
    }

    public void SetScore(int score)
    {
        if (!enabled) return;
        scoreText.text = $"Score: {score}";
    }
}