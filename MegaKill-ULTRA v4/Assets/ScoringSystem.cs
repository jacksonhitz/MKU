using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoringSystem : MonoBehaviour
{
    TextMeshProUGUI TextMeshPro;
    public TextMeshProUGUI scoreText;

    int score = 0;
    // Start is called before the first frame update
    void Start()
    {
        TextMeshPro = GetComponent<TextMeshProUGUI>();
        scoreText.text = "HEAT: " + score.ToString();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
