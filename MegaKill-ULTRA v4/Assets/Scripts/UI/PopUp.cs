using System.Collections;
using TMPro;
using UnityEngine;

public class PopUp : MonoBehaviour
{
    TextMeshProUGUI popupText;
    Coroutine currentCoroutine;

    void Awake()
    {
        popupText = GetComponent<TextMeshProUGUI>();
    }

    public void UpdatePopUp(string text)
    {
        if (currentCoroutine != null)
        {
            if (popupText.text == text)
            {
                return;
            }

            StopCoroutine(currentCoroutine);
            popupText.text = "";
            currentCoroutine = null;
        }

        popupText.text = text;
        RectTransform popupTransform = popupText.GetComponent<RectTransform>();

        float randomX = Random.Range(-0.2f, 0.3f);
        float randomY = Random.Range(-0.3f, 0.1f);

        popupTransform.anchoredPosition = new Vector2(randomX, randomY);
        currentCoroutine = StartCoroutine(PopupEffect(popupTransform));
    }

    IEnumerator PopupEffect(RectTransform popupTransform)
    {
        float duration = 1f;
        float elapsedTime = 0f;
        Vector2 startPos = popupTransform.anchoredPosition;
        Vector2 endPos = startPos + new Vector2(0, .1f);

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            popupTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        popupText.text = "";
        currentCoroutine = null;
    }
}
