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

    float timer; 

    void Awake()
    {
        if (Instance != null && Instance != this)
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
    }
}
