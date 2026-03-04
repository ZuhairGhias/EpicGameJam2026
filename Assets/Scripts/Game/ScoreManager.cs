using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public int score;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void AddScore(int points, Target.TargetType type)
    {
        score += points;
        // Later: multiplier, UI, combo logic, etc.
        Debug.Log($"Score +{points} ({type}) -> {score}");
    }
}