using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NewsText : MonoBehaviour
{
    float newsWidth;
    float pixelsPerSec;
    RectTransform rt;

    public float GetXPosition { get { return rt.anchoredPosition.x; } }
    public float GetWidth { get { return rt.rect.width;  } }

    public void Initialize(float newsWidth, float pixelsPerSec, string message)
    {
        this.newsWidth = newsWidth;
        this.pixelsPerSec = pixelsPerSec;
        rt = GetComponent<RectTransform>();
        GetComponent<TextMeshProUGUI>().text = message + " |  ";
    }


    // Update is called once per frame
    void Update()
    {
        rt.position += Vector3.left * pixelsPerSec * Time.deltaTime;
        if (GetXPosition <= 0 - newsWidth - GetWidth)
        {
            Destroy(gameObject);
        }
    }
    public bool OffScreen()
    {
        return GetXPosition <= -GetWidth;
    }
}
