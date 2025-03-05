using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class UX : MonoBehaviour
{
    Camera cam;
    GameManager gameManager;
    PlayerController player;

    [SerializeField] Canvas screenSpace; 
    [SerializeField] Canvas worldSpace; 
    [SerializeField] Canvas menu;

    [SerializeField] Dialogue tutorial;
    [SerializeField] Dialogue intro;
    
    [SerializeField] GameObject crosshair;

    float initialFoV;
    Vector3 initialCanvasScale;

    [SerializeField] TextMeshProUGUI popup;

    [SerializeField] RawImage currentEye;
    [SerializeField] Texture eye100Texture;
    [SerializeField] Texture eye75Texture;
    [SerializeField] Texture eye50Texture;
    [SerializeField] Texture eye25Texture;
    [SerializeField] Texture damagedEyeTexture;
    float damagedDuration = 0.2f;
    float health;

    void Awake()
    {
        cam = FindObjectOfType<Camera>();
        gameManager = FindObjectOfType<GameManager>();
        player = FindAnyObjectByType<PlayerController>();
    }
    void Start()
    {
        initialFoV = cam.fieldOfView;
        initialCanvasScale = worldSpace.transform.localScale;
        UIOff();
    }

    public void TutorialOn()
    {
        tutorial.CallDialogue();
    }
    public void IntroOn()
    {
        intro.CallDialogue();
    }

    public void UIOn()
    {
        worldSpace.gameObject.SetActive(true);
        if(!gameManager.isIntro)
        {
            screenSpace.gameObject.SetActive(true);
            crosshair.gameObject.SetActive(true);
        }
    }
    public void UIOff()
    {
        if (gameManager.isPaused)
        {
            worldSpace.gameObject.SetActive(false);
        }
        screenSpace.gameObject.SetActive(false);
        crosshair.gameObject.SetActive(false);
    }
    public void Paused()
    {
        menu.gameObject.SetActive(true);
        UIOff();
    }
    public void UnPaused()
    {
        menu.gameObject.SetActive(false);
        UIOn();
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

    IEnumerator ShowPopup()
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

    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        health = (currentHealth / maxHealth) * 100;
        StartCoroutine(HitEye());
    }
    IEnumerator HitEye()
    {
        currentEye.texture = damagedEyeTexture; 
        yield return new WaitForSeconds(damagedDuration);
        currentEye.texture = GetEye(); 
    }
    
    Texture GetEye()
    {
        if (health> 75) return eye100Texture;
        if (health > 50) return eye75Texture;
        if (health > 25) return eye50Texture;
        return eye25Texture;
    }
    
}
