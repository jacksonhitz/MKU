using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class UX : MonoBehaviour
{
    private Camera cam;
    public Vector3 offset;
    public Canvas canvas; 
    public TextMeshProUGUI score;
    public TextMeshProUGUI popup;
    public TextMeshProUGUI ammo;

    public Slider healthBar;
    public Slider focusBar;

    private GameManager gameManager;
    private PlayerController player;
    private float initialFoV;
    private Vector3 initialCanvasScale;

    public GameObject ammoIcon;

    [SerializeField] private Image healthStateImage;
    [SerializeField] private Sprite health100;
    [SerializeField] private Sprite health75;
    [SerializeField] private Sprite health50;
    [SerializeField] private Sprite health25;
    [SerializeField] private Sprite damagedState;
    
    private float damageDisplayDuration = 0.2f;
    private float damageTimer = 0f;
    private Sprite lastHealthState;
    private float previousHealth;

    //private PlayerController playerController;
    [SerializeField] private PlayerController playerController;

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

        {
        healthStateImage.sprite = health100;
        lastHealthState = health100;
        previousHealth = 100f;  // Assume starting at full health
        }

    }

    void Update()
    {
        UpdateCanvasFoVScaling();

        if (Input.GetKeyDown(KeyCode.T))
        {
            if (playerController != null)
            {
                playerController.Hit();
            }
            else
            {
                Debug.LogError("Cannot test damage - PlayerController is missing");
            }
        }

    }


    //public void UpdateHealth(float health, float maxHealth)
    //{
        //healthBar.value = health / maxHealth;
    //}

    public void UpdateFocus(float focus, float maxFocus)
    {
        focusBar.value = focus / maxFocus;
    }
    void UpdateCanvasFoVScaling()
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

    public void Score()
    {
        // score.text = "SCORE: " + gameManager.score;
    }

    
    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        float healthPercentage = (currentHealth / maxHealth) * 100;
        
        // Only proceed if health has decreased
        if (currentHealth < previousHealth)
        {
            Sprite newHealthState = GetHealthStateSprite(healthPercentage);
            lastHealthState = newHealthState;  // Store the new health state
            healthStateImage.sprite = damagedState;  // Show damage sprite
            StartCoroutine(ReturnToHealthState());   // Start timer to show new health state
        }
        
        previousHealth = currentHealth;  // Update previous health
    }
    
    private Sprite GetHealthStateSprite(float healthPercentage)
    {
        if (healthPercentage > 75) return health100;
        if (healthPercentage > 50) return health75;
        if (healthPercentage > 25) return health50;
        return health25;
    }
    
    private IEnumerator ReturnToHealthState()
    {
        yield return new WaitForSeconds(damageDisplayDuration);
        healthStateImage.sprite = lastHealthState;
    }
    
}
