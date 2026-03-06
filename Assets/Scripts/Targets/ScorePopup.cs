using UnityEngine;
using TMPro;

public class ScorePopup : MonoBehaviour
{
    public TMP_Text text;
    public float floatUpSpeed = 1.5f;
    public float life = 0.8f;
    public float drift = 0.5f;

    private float t;
    private Color startColor;
    private Vector3 driftDir;

    void Awake()
    {
        if (text == null) text = GetComponentInChildren<TMP_Text>();
        startColor = text.color;
        driftDir = new Vector3(Random.Range(-drift, drift), 1f, Random.Range(-drift, drift));
    }

    public void Init(string msg, Color color)
    {
        text.text = msg;
        text.color = color;
        startColor = color;
    }

    void Update()
    {
        t += Time.deltaTime;
        transform.position += driftDir * floatUpSpeed * Time.deltaTime;

        float a = Mathf.Lerp(1f, 0f, t / life);
        Color c = startColor; c.a = a;
        text.color = c;

        if (t >= life) Destroy(gameObject);
    }
}