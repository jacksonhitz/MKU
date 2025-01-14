using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewsPanel : MonoBehaviour
{

    public NewsText newsTextPrefab;
    [Range(1f, 5f)]
    public float textDuration = 2.0f;
    public string[] story;

    float width;
    float pixelsPerSec;
    NewsText currentText;
    int currentStory = 0;


    // Start is called before the first frame update
    void Start()
    {
        width = GetComponent<RectTransform>().rect.width;
        pixelsPerSec = width / textDuration;
        if (story.Length > 0)
        {
            AddNewsText(story[currentStory]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (currentText != null && currentText.OffScreen())
        {
            currentStory++;
            if (currentStory >= story.Length)
            {
                currentStory = 0;
            }

                AddNewsText(story[currentStory]);
        }
    }

    void AddNewsText(string message)
    {
        currentText = Instantiate(newsTextPrefab, transform);
        currentText.Initialize(width, pixelsPerSec, message);
    }
}
