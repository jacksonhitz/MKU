using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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
    [SerializeField] Texture eyeHealTexture;
    float damagedDuration = 0.2f;
    float health;
    bool isFlashing = false;

    Coroutine currentCoroutine;

    void Awake()
    {
        cam = FindObjectOfType<Camera>();
        gameManager = FindObjectOfType<GameManager>();
    }

    void Start()
    {
        menu.gameObject.SetActive(false);
        if (worldSpace != null)
        {
            initialFoV = cam.fieldOfView;
            initialCanvasScale = worldSpace.transform.localScale;
        }
    }

    public void TutorialOn() => tutorial.CallDialogue();
    public void IntroOn() => intro.CallDialogue();

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

    public void PopUp(string text)
    {
        if (currentCoroutine != null)
        {
            if (popup.text == text)
            {
                return;
            }
            else
            {
                StopCoroutine(currentCoroutine);
                popup.text = "";
                currentCoroutine = null;
            }
        }

        popup.text = text;
        RectTransform popupTransform = popup.GetComponent<RectTransform>();

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

        popup.text = "";
        currentCoroutine = null;
    }

    public void UpdateHealth(float newHealth)
    {
        StopAllCoroutines();
        if (newHealth >= health)
        {
            StartCoroutine(HealEye());
        }
        else
        {
            StartCoroutine(HurtEye());
        }
        
    }
    IEnumerator HurtEye()
    {
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

    IEnumerator HealEye()
    {
        Texture previousEye = currentEye.texture;
        currentEye.texture = eyeHealTexture;
        yield return new WaitForSeconds(0.2f);
        currentEye.texture = previousEye;
        currentEye.texture = GetEye();
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
