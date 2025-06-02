using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public float timeCount;
    public int killCount;
    public int styleCount;
    public int brutalityCount;
    public int prescionCount;

    public int score;
    public int comboCount;
    public float comboTimer;
    public float comboDuration = 3f;
    public float scoreMultiplier = 1f;

    float timer; 

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        timer += Time.deltaTime;
        timeCount = Mathf.FloorToInt(timer);

        if (comboCount > 0)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0f)
            {
                ResetCombo();
            }
        }
        Debug.Log($"Score: {score} | Combo: {comboCount} | Multiplier: {scoreMultiplier:F2}");
    }

    void AddScore(int baseScore)
    {
        score += Mathf.RoundToInt(baseScore * scoreMultiplier);
        AdvanceCombo();
    }

    void AdvanceCombo()
    {
        comboCount++;
        comboTimer = comboDuration;
        scoreMultiplier = Mathf.Min(1f + (comboCount / 5f), 5f); // Cap at 5x
    }

    public void ResetCombo()
    {
        comboCount = 0;
        scoreMultiplier = 1f;
        comboTimer = 0f;
    }

    // === Score Triggers ===
    public void AddMeleeScore()
    {
        AddScore(10);
        brutalityCount++;
    }

    public void AddGunScore()
    {
        AddScore(25);
        prescionCount++;
    }

    public void AddThrowScore()
    {
        AddScore(50);
        styleCount++;
    }
}
