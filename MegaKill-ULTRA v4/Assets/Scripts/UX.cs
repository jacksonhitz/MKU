using UnityEngine;
using TMPro;
using UnityEngine.UI; 
using System.Collections;

public class UX : MonoBehaviour
{
    public Camera cam; 
    public Vector3 offset; 
    public float camDistance = 2f;

    public TextMeshProUGUI tutorial; 
    public TextMeshProUGUI score; 
    public TextMeshProUGUI popup; 
    public GameManager gameManager;

    public Image flashImage;

    void Update()
    {
        UpdateCanvas();
    }

    void Start()
    {
        WASD();
    }

    void UpdateCanvas()
    {
        transform.position = cam.transform.position + cam.transform.forward * camDistance + offset;
        transform.rotation = Quaternion.LookRotation(cam.transform.forward);

        float scaleFactor = Scale(cam);
        transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
    }

    float Scale(Camera cam)
    {
        float fov = cam.fieldOfView;
        float aspect = cam.aspect;
        float height = 2.0f * Mathf.Tan((fov / 2) * Mathf.Deg2Rad) * camDistance; 
        float width = height * aspect; 

        float desiredWidth = 1920; 
        float desiredHeight = 1080; 

        float scale = Mathf.Min(width / desiredWidth, height / desiredHeight);

        return scale;
    }

    public void PopUp(int newScore)
    {
        popup.gameObject.SetActive(true);
        
        popup.text = "+" + newScore;

        float randomX = Random.Range(-1000f, 1000f); 
        float randomY = Random.Range(-350f, 350f); 

        popup.rectTransform.anchoredPosition = new Vector2(randomX, randomY);

        StartCoroutine(ShowPopup());
    }

    private IEnumerator ShowPopup()
    {
        float duration = 0.5f; 
        float elapsedTime = 0f;

        Vector3 startPosition = popup.rectTransform.anchoredPosition;
        Vector3 targetPosition = startPosition + new Vector3(0, 50, 0); 

        while (elapsedTime < duration)
        {
           
            float t = elapsedTime / duration;

            popup.rectTransform.anchoredPosition = Vector3.Lerp(startPosition, targetPosition, t);

            elapsedTime += Time.deltaTime;
            yield return null; 
        }
        popup.gameObject.SetActive(false);
    }

    public void Score()
    {
        score.text = "SCORE: " + (gameManager.score);
    }
    
    public void Off()
    {
        tutorial.enabled = false;
    }
    
    public void WASD()
    {
        tutorial.text = "WASD TO MOVE"; 
    }

    public void Kill()
    {
        tutorial.text = "MOUSE TO KILL"; 
    }

    public void Reload()
    {
        tutorial.text = "R TO RELOAD";
    }

    public void Drive()
    {
        tutorial.text = "E TO DRIVE"; 
    }
    
    public void Bail()
    {
        tutorial.text = "Q TO BAIL"; 
    }

    public void Hit()
    {
        StartCoroutine(FlashRed());
    }

    public void Died()
    {
        StartCoroutine(FadeRed());
    }

    private IEnumerator FlashRed()
    {
        flashImage.color = new Color(1, 0, 0, 0.6f); 
        flashImage.gameObject.SetActive(true); 
        
        yield return new WaitForSeconds(0.2f); 
        
        float fadeDuration = 0.5f; 
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            float t = elapsedTime / fadeDuration;
            flashImage.color = new Color(1, 0, 0, Mathf.Lerp(0.6f, 0f, t)); 
            elapsedTime += Time.deltaTime; 
            yield return null; 
        }

        flashImage.color = new Color(1, 0, 0, 0); 
        flashImage.gameObject.SetActive(false); 
    }

    IEnumerator FadeRed()
    {
        float fadeDuration = 2.0f; 
        float elapsedTime = 0f;

        flashImage.color = new Color(0, 0, 0, 0); 
        flashImage.gameObject.SetActive(true); 

        while (elapsedTime < fadeDuration)
        {
            float t = elapsedTime / fadeDuration;
            flashImage.color = new Color(1, 0, 0, Mathf.Lerp(0.6f, 1f, t)); 
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        flashImage.color = new Color(1, 0, 0, 1f); 
    }
}
