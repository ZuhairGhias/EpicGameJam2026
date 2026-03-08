using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [Header("Heat")]
    public float maxHeat = 50f;
    public float heatPerPoint = 1f;
    public float heatDecayPerSecond = 0.5f;

    [Header("Multiplier Levels")]
    public int maxMultiplier = 5;
    public float heatPerLevel = 10f;

    [Header("UI")]
    public MultiplierUI multiplierUI;

    [Header("Hit FX (optional)")]
    public ScorePopup popupPrefab;
    public GameObject globalHitParticlePrefab;

    public float Heat { get; private set; }
    public int Multiplier { get; private set; } = 1;

    private bool warnedPopup;
    private bool warnedParticles;

    void Awake()
    {
        if (multiplierUI == null)
            multiplierUI = FindFirstObjectByType<MultiplierUI>();

        if (multiplierUI == null)
        {
            Debug.LogError("ScoreManager: Could not find MultiplierUI in the scene. Disabling ScoreManager.", this);
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
    }

    void Update()
    {
        Heat = Mathf.Max(0f, Heat - heatDecayPerSecond * Time.deltaTime);
        RecomputeMultiplier();
        UpdateUI();
    }

    public int RegisterHit(Target target, Vector3 hitPoint)
    {
        if (!enabled) return 0;

        if (target == null)
        {
            Debug.LogError("ScoreManager.RegisterHit called with null target.");
            return 0;
        }

        int basePoints = target.points;

        AddHeatFromPoints(basePoints);
        int awarded = basePoints * Multiplier;

        SpawnScoreFx(hitPoint, awarded, Multiplier);
        return awarded;
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

    void AddHeatFromPoints(int basePoints)
    {
        Heat = Mathf.Clamp(Heat + basePoints * heatPerPoint, 0f, maxHeat);
        RecomputeMultiplier();
        UpdateUI();
    }

    void RecomputeMultiplier()
    {
        int newMult = 1 + Mathf.FloorToInt(Heat / heatPerLevel);
        Multiplier = Mathf.Clamp(newMult, 1, maxMultiplier);
    }

    void UpdateUI()
    {
        if (multiplierUI == null) return;

        float currentLevelMinHeat = (Multiplier - 1) * heatPerLevel;
        float nextLevelHeat = Mathf.Min(Multiplier * heatPerLevel, maxHeat);
        float heatIntoLevel = Mathf.Clamp(Heat - currentLevelMinHeat, 0f, heatPerLevel);
        float heatNeededForNextLevel = (Multiplier >= maxMultiplier) ? heatPerLevel : (nextLevelHeat - currentLevelMinHeat);

        float barMax = heatNeededForNextLevel;
        float barValue = heatIntoLevel;

        if (Multiplier >= maxMultiplier)
        {
            barMax = heatPerLevel;
            barValue = heatPerLevel;
        }

        multiplierUI.SetHeatAndMultiplier(barValue, barMax, Multiplier);
    }
}
