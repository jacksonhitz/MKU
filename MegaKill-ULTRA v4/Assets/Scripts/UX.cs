using TMPro;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class UX : MonoBehaviour
{
    Camera cam;
    GameManager gameManager;

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
    [SerializeField] Texture eye80Texture;
    [SerializeField] Texture eye60Texture;
    [SerializeField] Texture eye40Texture;
    [SerializeField] Texture eye20Texture;
    [SerializeField] Texture eye10Texture;
    [SerializeField] Texture eyeFlashTexture;
    [SerializeField] Texture eyeHurtTexture;
    float damagedDuration = 0.2f;
    float health;
    bool isFlashing = false;

    // Reference to the currently running coroutine
    Coroutine currentCoroutine;

    void Awake()
    {
        cam = FindObjectOfType<Camera>();
        gameManager = FindObjectOfType<GameManager>();
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
        if (!gameManager.isIntro)
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

        StopAllCoroutines();
        StartCoroutine(HurtEye());
    }

    IEnumerator HurtEye()
    {
        Debug.Log("EYE CALLED");
        
        Texture previousEye = currentEye.texture;
        currentEye.texture = eyeHurtTexture;
        yield return new WaitForSeconds(0.2f);
        currentEye.texture = previousEye;

        if (health > 10)
        {
            currentEye.texture = GetEye();
        }
        else
        {
            StartCoroutine(FlashEye());
        }
    }

    IEnumerator FlashEye()
    {
        currentEye.texture = eyeFlashTexture;
        yield return new WaitForSeconds(0.2f);
        currentEye.texture = eye10Texture;
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(HurtEye());
    }

    Texture GetEye()
    {
        if (health > 80) return eye100Texture;
        if (health > 60) return eye80Texture;
        if (health > 40) return eye60Texture;
        if (health > 20) return eye40Texture;
        if (health > 10) return eye20Texture;
        return eye10Texture;
    }
}
