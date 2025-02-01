using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UX : MonoBehaviour
{
    private Camera cam;
    public Vector3 offset;
    public Canvas canvas; // Canvas containing the UI elements

    public TextMeshProUGUI tutorial;
    public TextMeshProUGUI score;
    public TextMeshProUGUI popup;
    public TextMeshProUGUI ammo;

    private GameManager gameManager;
    private PlayerController player;
    private float initialFoV;
    private Vector3 initialCanvasScale;

    void Start()
    {
        cam = FindObjectOfType<Camera>();
        gameManager = FindObjectOfType<GameManager>();
        player = FindAnyObjectByType<PlayerController>();

        if (cam != null)
        {
            initialFoV = cam.fieldOfView;
        }
        
        if (canvas != null)
        {
            initialCanvasScale = canvas.transform.localScale;
        }

        UpdateAmmo();
        WASD();
    }

    void Update()
    {
        UpdateCanvasFoVScaling();
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

    private void UpdateCanvasFoVScaling()
    {
        if (cam != null && canvas != null)
        {
            float currentFoV = cam.fieldOfView;
            float scaleFactor = currentFoV / initialFoV;
            canvas.transform.localScale = initialCanvasScale * scaleFactor;
        }
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
        // score.text = "SCORE: " + gameManager.score;
    }

    private IEnumerator Off()
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
        tutorial.text = "Not time to play in the sun yet, there's still plenty of bad people to take care of down here...";
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
