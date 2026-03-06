using UnityEngine;

[RequireComponent(typeof(MultiplierSystem))]
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("Score")]
    public int score;

    [Header("Required UI")]
    public ScoreUI scoreUI;

    private MultiplierSystem multiplierSystem;

    [Header("Hit FX (optional)")]
    public ScorePopup popupPrefab;
    public GameObject globalHitParticlePrefab;

    private bool warnedPopup;
    private bool warnedParticles;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("Multiple ScoreManager instances detected. There must be exactly one.");
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (scoreUI == null)
        {
            Debug.LogError("ScoreManager: scoreUI reference is REQUIRED but not assigned.");
            enabled = false;
            return;
        }

        // Auto-wire required dependency if not assigned
        if (multiplierSystem == null)
            multiplierSystem = GetComponent<MultiplierSystem>();

        if (multiplierSystem == null)
        {
            Debug.LogError("ScoreManager: MultiplierSystem is REQUIRED on the same GameObject but was not found.");
            enabled = false;
            return;
        }
    }

    void Start()
    {
        // Optional warnings (once)
        if (popupPrefab == null && !warnedPopup)
        {
            Debug.LogWarning("ScoreManager: popupPrefab not set. Floating score popups disabled.");
            warnedPopup = true;
        }

        if (globalHitParticlePrefab == null && !warnedParticles)
        {
            Debug.LogWarning("ScoreManager: globalHitParticlePrefab not set. Hit particles disabled.");
            warnedParticles = true;
        }

        scoreUI.SetScore(score);
    }

    public void RegisterHit(Target target, Vector3 hitPoint)
    {
        if (!enabled) return;

        if (target == null)
        {
            Debug.LogError("ScoreManager.RegisterHit called with null target.");
            return;
        }

        int basePoints = target.points;

        // Heat/multiplier progression
        multiplierSystem.AddHeatFromPoints(basePoints);

        int multiplier = multiplierSystem.Multiplier;
        int awarded = basePoints * multiplier;

        score += awarded;
        scoreUI.SetScore(score);

        SpawnScoreFx(hitPoint, awarded, multiplier);
    }

    void SpawnScoreFx(Vector3 pos, int awardedPoints, int multiplier)
    {
        if (globalHitParticlePrefab != null)
        {
            Instantiate(globalHitParticlePrefab, pos, Quaternion.identity);
        }

        if (popupPrefab != null)
        {
            Vector3 p = pos + Vector3.up * 1.5f;
            var popup = Instantiate(popupPrefab, p, Quaternion.identity);

            Color c = (multiplier >= 4) ? new Color(1f, 0.85f, 0.2f) : Color.white;
            popup.Init($"{awardedPoints}", c);
        }
    }
}