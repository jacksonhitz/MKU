using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UX : MonoBehaviour
{
    Camera cam;
    GameManager gameManager;
    Settings settings;

    [SerializeField] Canvas screenSpace;
    [SerializeField] Canvas worldSpace;

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

    [SerializeField] Texture eye100Texture2;
    [SerializeField] Texture eye80Texture2;
    [SerializeField] Texture eye60Texture2;
    [SerializeField] Texture eye40Texture2;
    [SerializeField] Texture eye20Texture2;
    [SerializeField] Texture eye10Texture2;
    [SerializeField] Texture eyeFlashTexture2;
    
    [SerializeField] Texture eyeHurtTexture;
    [SerializeField] Texture eyeHealTexture;

    float damagedDuration = 0.2f;
    float health;
    bool isFlashing = false;

    Coroutine currentCoroutine;

    private float eyeIdleAnimationDuration = 1.5f;
    private Coroutine eyeIdleCoroutine;
    private bool useAlternateTexture = false;

    void Awake()
    {
        cam = FindObjectOfType<Camera>();
        gameManager = FindObjectOfType<GameManager>();
    }

    void Start()
    {
        health = 100f;
        
        if (worldSpace != null)
        {
            initialFoV = cam.fieldOfView;
            initialCanvasScale = worldSpace.transform.localScale;
        }
        
        eyeIdleCoroutine = StartCoroutine(IdleEyeAnimation());
    }

    public void IntroOn()
    {
        UIOn();
        intro.CallDialogue();
    }
    public void TutorialOn()
    {
        UIOn();
        tutorial.CallDialogue();
    }

    public void TutorialOff()
    {
        tutorial.Off();
    }
    public void IntroOff()
    {
        intro.Off();
    }

    public void UIOn()
    {
        if (StateManager.state != StateManager.GameState.Intro)
        {
            screenSpace.gameObject.SetActive(true);
            crosshair.gameObject.SetActive(true);
        }
        worldSpace.gameObject.SetActive(true);
    }

    public void UIOff()
    {
        worldSpace.gameObject.SetActive(false);
        screenSpace.gameObject.SetActive(false);
        crosshair.gameObject.SetActive(false);
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
        // Store the current health for comparison
        float oldHealth = health;
        
        health = newHealth;
        
        StopAllCoroutines();


        if (newHealth >= oldHealth)
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
            eyeIdleCoroutine = StartCoroutine(IdleEyeAnimation());
        }
        else
        {
            // Start the flashing sequence when health is low
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

        //Restart the idle animation
        eyeIdleCoroutine = StartCoroutine(IdleEyeAnimation());
    }

    IEnumerator FlashEye()
    {
        Texture[] flashSequence = new Texture[] 
        { 
            eye10Texture, 
            eye10Texture2, 
            eyeFlashTexture, 
            eyeFlashTexture2 
        };
        
        int currentIndex = 0;
        
        while (health <= 10)
        {
            // Set the current texture from the sequence
            currentEye.texture = flashSequence[currentIndex];
            
            // Move to the next texture in the sequence
            currentIndex = (currentIndex + 1) % flashSequence.Length;
            
            // Wait before switching to the next texture
            yield return new WaitForSeconds(0.2f);
        }
        
        // If health went above 10, return to normal eye
        currentEye.texture = GetEye();
        
        // Restart the idle animation
        eyeIdleCoroutine = StartCoroutine(IdleEyeAnimation());
    }

    IEnumerator IdleEyeAnimation()
    {
        while (true)
        {
            // Toggle between primary and alternate texture
            useAlternateTexture = !useAlternateTexture;
            
            // Apply the appropriate texture
            currentEye.texture = GetEye();
            
            // Wait before switching to the other texture
            yield return new WaitForSeconds(eyeIdleAnimationDuration);
        }
    }

    Texture GetEye()
    {
        // Return the appropriate texture based on health and whether to use the alternate texture
        if (!useAlternateTexture)
        {
            if (health > 80) return eye100Texture;
            if (health > 60) return eye80Texture;
            if (health > 40) return eye60Texture;
            if (health > 20) return eye40Texture;
            if (health > 10) return eye20Texture;
            return eye10Texture;
        }
        else
        {
            if (health > 80) return eye100Texture2;
            if (health > 60) return eye80Texture2;
            if (health > 40) return eye60Texture2;
            if (health > 20) return eye40Texture2;
            if (health > 10) return eye20Texture2;
            return eye10Texture2;
        }
    }

    Texture GetEyeFlash()
    {
        // Return the appropriate flash texture based on whether to use the alternate texture
        return useAlternateTexture ? eyeFlashTexture2 : eyeFlashTexture;
    }
}
