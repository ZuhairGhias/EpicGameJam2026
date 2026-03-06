using UnityEngine;

public class MultiplierSystem : MonoBehaviour
{
    [Header("Heat")]
    public float maxHeat = 50f;
    public float heatPerPoint = 1f;      // heat gained per point earned
    public float heatDecayPerSecond = 0.5f; // e.g. 0.5 = -1 heat every 2 seconds

    [Header("Multiplier Levels")]
    public int maxMultiplier = 5;
    public float heatPerLevel = 10f;     // 0-9 x1, 10-19 x2, etc.

    [Header("UI")]
    public MultiplierUI ui;

    public float Heat { get; private set; }
    public int Multiplier { get; private set; } = 1;

    public float CurrentLevelMinHeat => (Multiplier - 1) * heatPerLevel;
    public float NextLevelHeat => Mathf.Min(Multiplier * heatPerLevel, maxHeat);

    public float HeatIntoLevel => Mathf.Clamp(Heat - CurrentLevelMinHeat, 0f, heatPerLevel);

    public float HeatNeededForNextLevel
    {
        get
        {
            if (Multiplier >= maxMultiplier) return heatPerLevel; // arbitrary; UI can show full
            return NextLevelHeat - CurrentLevelMinHeat; // usually heatPerLevel
        }
    }

    public bool IsMaxMultiplier => Multiplier >= maxMultiplier;

    void Update()
    {
        // Decay heat
        Heat = Mathf.Max(0f, Heat - heatDecayPerSecond * Time.deltaTime);
        RecomputeMultiplier();
        UpdateUI();
    }

    public void AddHeatFromPoints(int basePoints)
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
        if (ui == null) return;

        float barMax = HeatNeededForNextLevel;
        float barValue = HeatIntoLevel;

        // If max multiplier, keep bar full
        if (IsMaxMultiplier)
        {
            barMax = heatPerLevel;
            barValue = heatPerLevel;
        }

        ui.SetHeatAndMultiplier(barValue, barMax, Multiplier);
    }
}