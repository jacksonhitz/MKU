using UnityEngine;
using TMPro;
using UnityEngine.UI; // Add this to use UI components
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
        float duration = 0.5f; // Duration to show the popup
        float elapsedTime = 0f;

        Vector3 startPosition = popup.rectTransform.anchoredPosition;
        Vector3 targetPosition = startPosition + new Vector3(0, 50, 0); // Drift upwards by 50 units

        while (elapsedTime < duration)
        {
            // Calculate the proportion of time elapsed
            float t = elapsedTime / duration;

            // Lerp the position for a smooth upward drift
            popup.rectTransform.anchoredPosition = Vector3.Lerp(startPosition, targetPosition, t);

            elapsedTime += Time.deltaTime; // Increment the elapsed time
            yield return null; // Wait for the next frame
        }

        // Ensure the popup is disabled after the duration
        popup.gameObject.SetActive(false);
    }

    public void Score()
    {
        score.text = "SCORE: " + (gameManager.score);
    }
    
    public void Off()
    {
        
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

    private IEnumerator FlashRed()
    {
        // Set color to a vibrant red with increased opacity
        flashImage.color = new Color(1, 0, 0, 0.6f); // More opaque red
        flashImage.gameObject.SetActive(true); // Show the flash image
        
        // Wait for a short duration
        yield return new WaitForSeconds(0.2f); // Hold red for longer
        
        // Fade out gradually
        float fadeDuration = 0.5f; // Duration to fade out
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            // Calculate the proportion of time elapsed
            float t = elapsedTime / fadeDuration;

            // Lerp the alpha value for smooth fade out
            flashImage.color = new Color(1, 0, 0, Mathf.Lerp(0.6f, 0f, t)); // Fade from 0.6 to 0

            elapsedTime += Time.deltaTime; // Increment the elapsed time
            yield return null; // Wait for the next frame
        }

        flashImage.color = new Color(1, 0, 0, 0); // Ensure it is fully transparent
        flashImage.gameObject.SetActive(false); // Hide the flash image
    }


}
