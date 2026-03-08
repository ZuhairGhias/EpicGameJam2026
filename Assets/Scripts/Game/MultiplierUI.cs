using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MultiplierUI : MonoBehaviour
{
    [Header("UI")]
    public Slider heatBar;
    public TMP_Text multiplierText;
    public TMP_Text scoreText;

    [Header("Animation")]
    public float punchScale = 1.25f;
    public float punchDuration = 0.12f;

    [Header("Colors")]
    public Color normalColor = Color.white;
    public Color levelUpColor = new Color(0.4f, 1f, 0.4f);
    public Color levelDownColor = new Color(1f, 0.4f, 0.4f);

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip levelUpSfx;
    public AudioClip levelDownSfx;

    private int lastMultiplier = 1;
    private Vector3 baseScale;

    void Awake()
    {
        // Required references
        if (heatBar == null)
        {
            Debug.LogError("MultiplierUI: heatBar reference is not assigned.", this);
            enabled = false;
            return;
        }

        if (multiplierText == null)
        {
            Debug.LogError("MultiplierUI: multiplierText reference is not assigned.", this);
            enabled = false;
            return;
        }

        // Optional references
        if (audioSource == null)
            Debug.LogWarning("MultiplierUI: audioSource not assigned. Multiplier sounds will not play.", this);

        if (levelUpSfx == null)
            Debug.LogWarning("MultiplierUI: levelUpSfx not assigned.", this);

        if (levelDownSfx == null)
            Debug.LogWarning("MultiplierUI: levelDownSfx not assigned.", this);

        baseScale = multiplierText.transform.localScale;
    }

    /// Call this whenever heat/multiplier changes
    public void SetHeatAndMultiplier(float heat, float maxHeat, int multiplier)
    {
        if (heatBar != null)
        {
            heatBar.maxValue = maxHeat;
            heatBar.value = heat;
            Debug.Log($"SetHeatAndMultiplier: heat={heat}, maxHeat={maxHeat}, multiplier={multiplier}");
        }

        if (multiplierText != null)
            multiplierText.text = $"x{multiplier}";

        if (multiplier != lastMultiplier)
        {
            if (multiplier > lastMultiplier) OnLevelUp();
            else OnLevelDown();

            lastMultiplier = multiplier;
        }
    }

    public void SetScore(int score)
    {
        if (scoreText != null)
            scoreText.text = $"Score: {score}";
    }

    void OnLevelUp()
    {
        Pulse(levelUpColor);
        Play(levelUpSfx);
    }

    void OnLevelDown()
    {
        Pulse(levelDownColor);
        Play(levelDownSfx);
    }

    void Pulse(Color c)
    {
        if (multiplierText == null) return;

        StopAllCoroutines();

        if (gameObject.activeInHierarchy)
            StartCoroutine(PunchRoutine(c));
    }

    System.Collections.IEnumerator PunchRoutine(Color c)
    {
        multiplierText.color = c;

        float t = 0f;
        while (t < punchDuration)
        {
            t += Time.deltaTime;
            float k = t / punchDuration;
            float s = Mathf.Lerp(1f, punchScale, Mathf.Sin(k * Mathf.PI));
            multiplierText.transform.localScale = baseScale * s;
            yield return null;
        }

        multiplierText.transform.localScale = baseScale;
        multiplierText.color = normalColor;
    }

    void Play(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }
}
