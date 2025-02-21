using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class UX : MonoBehaviour
{
    private Camera cam;
    public Vector3 offset;
    public Canvas screenSpace; 
    public Canvas worldSpace; 
    private GameManager gameManager;
    private PlayerController player;
    private float initialFoV;
    private Vector3 initialCanvasScale;

    //public GameObject ammoIcon;
    //public TextMeshProUGUI score;
    public TextMeshProUGUI popup;

    [SerializeField] RawImage currentEye;
    [SerializeField] Texture eye100Texture;
    [SerializeField] Texture eye75Texture;
    [SerializeField] Texture eye50Texture;
    [SerializeField] Texture eye25Texture;
    [SerializeField] Texture damagedEyeTexture;

    float damagedDuration = 0.2f;
    float damageTimer = 0f;
    float healthPercentage;

    void Awake()
    {
        cam = FindObjectOfType<Camera>();
        gameManager = FindObjectOfType<GameManager>();
        player = FindAnyObjectByType<PlayerController>();
        
    }

    public void FindEye()
    {
        GameObject eyeObj = GameObject.Find("Eye");
        currentEye = eyeObj.GetComponent<RawImage>();
    }

    void Start()
    {
        initialFoV = cam.fieldOfView;
        initialCanvasScale = worldSpace.transform.localScale;
    }

    void Update()
    {
        // UpdateCanvasFoVScaling();
    }
    void ScaleWorldSpace()
    {
        float currentFoV = cam.fieldOfView;
        float scaleFactor = currentFoV / initialFoV;
        worldSpace.transform.localScale = initialCanvasScale * scaleFactor;
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
        healthPercentage = (currentHealth / maxHealth) * 100;
        StartCoroutine(HitEye());
        Debug.Log("updating");
    }
    
    Texture GetEye()
    {
        if (healthPercentage > 75) return eye100Texture;
        if (healthPercentage > 50) return eye75Texture;
        if (healthPercentage > 25) return eye50Texture;
        return eye25Texture;
    }
    
    IEnumerator HitEye()
    {
        currentEye.texture = damagedEyeTexture; 
        yield return new WaitForSeconds(damagedDuration);
        currentEye.texture = GetEye(); 
    }
}
