using UnityEngine;
using TMPro;
using UnityEngine.UI; 
using System.Collections;

public class UX : MonoBehaviour
{
    Camera cam; 
    public Vector3 offset; 
    float camDistance = 0.5f;

    public TextMeshProUGUI tutorial; 
    public TextMeshProUGUI score; 
    public TextMeshProUGUI popup; 
    public TextMeshProUGUI ammo; 
    GameManager gameManager;
    PlayerController player;

    void Update()
    {
        UpdateCanvas();
    }

    void Start()
    {
        cam = FindObjectOfType<Camera>();
        gameManager = FindObjectOfType<GameManager>();
        player = FindAnyObjectByType<PlayerController>();
        
        UpdateAmmo();
        WASD();
    }

    public void UpdateAmmo()
    {
        if (player.weapon == 0)
        {
            ammo.text = player.gun.bullets.ToString();
            
        }
        else if (player.weapon == 1)
        {
            ammo.text = player.gun.shells.ToString();
        }
        else
        {
            ammo.text = "";
        }
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
        //score.text = "SCORE: " + (gameManager.score);
    }
    
    IEnumerator Off()
    {
        yield return new WaitForSeconds(5f);
        
        tutorial.enabled = false;
    }
    
    public void WASD()
    {
        tutorial.text = "WASD TO MOVE"; 
    }
    public void Slow()
    {
        tutorial.text = "SHIFT TO SLOW"; 
    }

    public void Arms()
    {
        tutorial.text = "1 AND 2 TO SWITCH WEAPONS";
        StartCoroutine(Off());
    }

    public void Warning()
    {
        tutorial.enabled = true;
        tutorial.text = "Not time to play in the sun yet, theres still plenty of Bad people to take care of down here..."; 
        StartCoroutine(Off());
    }

    public void Kill()
    {
        tutorial.text = "MOUSE TO KILL"; 
    }

    public void Reload()
    {
        tutorial.text = "R TO RELOAD";
    }
}
